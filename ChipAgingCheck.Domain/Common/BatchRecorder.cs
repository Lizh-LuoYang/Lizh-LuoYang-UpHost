using ChipAgingCheck.Domain.Modbus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace ChipAgingCheck.Domain.Common
{
    // 一次老化的批次信息，断电重启后靠它接着跑同一批
    public class BatchInfo
    {
        public string BatchNo { get; set; } = ""; // 形如 B20261101-143012
        public string ProductModel { get; set; } = "";
        public string RecipeName { get; set; } = "";
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool Ended { get; set; }
    }

    // 批次 + CSV 落盘：数据一行一条，Excel 双击能打开，中文不乱码
    public static class BatchRecorder
    {
        private static readonly object WriteLock = new object(); // 写 CSV、写批次状态都在这把锁里

        // 不变文化。否则系统语言一变，小数点成了逗号，CSV 就错列了
        private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;
        private const int SnapshotIntervalSeconds = 60;

        private static BatchInfo currentBatch = null;
        private static StreamWriter writer = null;
        private static DateTime lastSnapshotTime = DateTime.MinValue;
        private static bool headerWritten = false; // 接着上次的批次跑时表头已经在了，不能重复写

        // 跟配置同一个根目录
        public static string DataDirectory
        {
            get { return Path.Combine(ConfigManager.RootDirectory, "Data"); }
        }

        public static BatchInfo Current
        {
            get { return currentBatch; }
        }

        public static string CsvPath
        {
            get
            {
                bool noBatch = currentBatch == null;
                if (noBatch)
                {
                    return "";
                }
                return Path.Combine(DataDirectory, currentBatch.BatchNo + ".csv");
            }
        }

        public static BatchInfo Start(string productModel, string recipeName)
        {
            lock (WriteLock)
            {
                CloseWriter();
                BatchInfo batch = new BatchInfo();
                batch.BatchNo = "B" + DateTime.Now.ToString("yyyyMMdd-HHmmss");
                batch.ProductModel = productModel;
                batch.RecipeName = recipeName;
                batch.StartTime = DateTime.Now;
                batch.Ended = false;
                currentBatch = batch;
                OpenWriter();
                SaveState();
                return batch;
            }
        }

        // 断电恢复：上次那批没标结束，就接着用它，不新开
        public static BatchInfo ResumeUnfinished()
        {
            lock (WriteLock)
            {
                BatchInfo saved = LoadState();
                bool hasSaved = saved != null;
                if (hasSaved == false)
                {
                    return null;
                }
                if (saved.Ended)
                {
                    return null;
                }
                currentBatch = saved;
                OpenWriter(); // 追加写，表头不会重来一遍
                return saved;
            }
        }

        public static void Stop()
        {
            lock (WriteLock)
            {
                if (currentBatch == null)
                {
                    return;
                }
                currentBatch.Ended = true;
                currentBatch.EndTime = DateTime.Now;
                SaveState(); // 存过状态 ResumeUnfinished 就不会再捡这一批
                CloseWriter();
            }
        }

        // 每 60 秒整批落一轮，8 台 × 64 路
        public static void WriteSnapshot(List<ModbusStatus> statusList, Recipe recipe)
        {
            lock (WriteLock)
            {
                if (currentBatch == null || writer == null)
                {
                    return;
                }
                if (recipe == null)
                {
                    return;
                }
                TimeSpan sinceLast = DateTime.Now - lastSnapshotTime;
                if (sinceLast.TotalSeconds < SnapshotIntervalSeconds)
                {
                    return;
                }
                lastSnapshotTime = DateTime.Now;
                string stamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                for (int device = 0; device < statusList.Count; device++)
                {
                    ModbusStatus status = statusList[device];
                    bool online = status.ConnState == ConnectState.Connect;
                    double[] currents = status.Current;
                    double[] voltages = status.Voltage;
                    if (currents == null || voltages == null)
                    {
                        continue;
                    }
                    int channelCount = currents.Length;
                    for (int channel = 0; channel < channelCount; channel++)
                    {
                        StringBuilder row = new StringBuilder();
                        row.Append(stamp).Append(',');
                        row.Append("DATA").Append(',');
                        row.Append(currentBatch.BatchNo).Append(',');
                        row.Append(device + 1).Append(',');
                        row.Append(channel + 1).Append(',');
                        row.Append(device * 64 + channel + 1).Append(',');
                        row.Append(currents[channel].ToString("F1", Culture)).Append(',');
                        row.Append(voltages[channel].ToString("F1", Culture)).Append(',');
                        row.Append(online ? "在线" : "离线");
                        writer.WriteLine(row.ToString());
                    }
                }
                writer.Flush();
            }
        }

        // 报警发生那一刻立刻写一条，不等 60 秒那一轮
        public static void WriteAlarm(Alarm alarm, int deviceIndex)
        {
            lock (WriteLock)
            {
                if (currentBatch == null || writer == null || alarm == null)
                {
                    return;
                }
                StringBuilder row = new StringBuilder();
                row.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")).Append(',');
                row.Append("ALARM").Append(',');
                row.Append(currentBatch.BatchNo).Append(',');
                row.Append(deviceIndex + 1).Append(',');
                row.Append(alarm.Index + 1).Append(',');
                row.Append(deviceIndex * 64 + alarm.Index + 1).Append(',');
                row.Append(','); // 电流、电压留空
                row.Append(',');
                row.Append(alarm.Message);
                writer.WriteLine(row.ToString());
                writer.Flush();
            }
        }

        private static void OpenWriter()
        {
            Directory.CreateDirectory(DataDirectory);
            string path = CsvPath;
            bool fileExists = File.Exists(path);
            UTF8Encoding encoding = new UTF8Encoding(true); // 带 BOM，不然 Excel 打开是乱码
            FileStream stream = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            writer = new StreamWriter(stream, encoding);
            headerWritten = fileExists; // 文件已经在了，说明表头写过了
            if (headerWritten == false)
            {
                string header = "时间,类型,批次,设备号,通道号,位置号,电流mA,电压V,说明";
                writer.WriteLine(header);
                writer.Flush();
                headerWritten = true;
            }
        }

        private static void CloseWriter()
        {
            if (writer == null)
            {
                return;
            }
            try
            {
                writer.Flush();
                writer.Dispose();
            }
            catch (Exception)
            {
                // 关不上也不能影响停止流程
            }
            writer = null;
        }

        private static string StatePath
        {
            get { return Path.Combine(ConfigManager.RootDirectory, "batch.json"); }
        }

        private static void SaveState()
        {
            try
            {
                string json = JsonConvert.SerializeObject(currentBatch, Formatting.Indented);
                File.WriteAllText(StatePath, json);
            }
            catch (Exception)
            {
                // 状态存不上，下次就当新批次处理
            }
        }

        private static BatchInfo LoadState()
        {
            try
            {
                bool exists = File.Exists(StatePath);
                if (exists == false)
                {
                    return null;
                }
                string json = File.ReadAllText(StatePath);
                return JsonConvert.DeserializeObject<BatchInfo>(json);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
