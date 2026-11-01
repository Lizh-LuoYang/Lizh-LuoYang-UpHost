using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChipAgingCheck.Domain.Common
{
    /// <summary>日志级别，数字越大越严重。</summary>
    public enum LogLevel
    {
        Debug = 0,
        Info = 1,
        Warn = 2,
        Error = 3,
        Fatal = 4,
    }

    public static class Logger
    {
        private static readonly object _lock = new object();
        private static readonly string _logRoot = ResolveLogRoot();
        private static readonly string _fallbackFile = Path.Combine(Path.GetTempPath(), "ChipAgingCheck_fallback.log");
        private const long MaxFileSize = 10 * 1024 * 1024;

        /// <summary>低于这个级别的日志直接丢弃。生产用 Info，排查问题时临时改 Debug。</summary>
        public static LogLevel MinLevel = LogLevel.Info;

        // 缓存"今天"和"当前文件"，避免每条日志都去扫目录。
        private static string _currentDay;
        private static string _currentFile;
        private static long _currentSize;

        public static void LogDebug(string message, Exception ex = null) => Write(message, LogLevel.Debug, ex);
        public static void LogInfo(string message, Exception ex = null) => Write(message, LogLevel.Info, ex);
        public static void LogWarn(string message, Exception ex = null) => Write(message, LogLevel.Warn, ex);
        public static void LogError(string message, Exception ex = null) => Write(message, LogLevel.Error, ex);
        public static void LogFatal(string message, Exception ex = null) => Write(message, LogLevel.Fatal, ex);

        public static void Write(string message, LogLevel level, Exception ex = null)
        {
            if (level < MinLevel)
            {
                return;
            }

            lock (_lock)
            {
                try
                {
                    DateTime now = DateTime.Now;
                    string day = now.ToString("yyyy-MM-dd");

                    if (_currentDay != day || _currentFile == null || _currentSize >= MaxFileSize)
                    {
                        string dir = Path.Combine(_logRoot, day);
                        Directory.CreateDirectory(dir);

                        _currentDay = day;
                        _currentFile = FindOrCreateFile(dir, day, out _currentSize);
                    }

                    var sb = new StringBuilder();
                    sb.AppendFormat("{0:yyyy-MM-dd HH:mm:ss.fff} [{1}] [T{2}] {3}",
                        now, level, Environment.CurrentManagedThreadId, message);

                    // 异常必须写完整堆栈
                    if (ex != null)
                    {
                        sb.AppendLine();
                        sb.Append(ex.ToString());
                    }

                    string line = sb.ToString();
                    File.AppendAllText(_currentFile, line + Environment.NewLine);
                    _currentSize += Encoding.UTF8.GetByteCount(line) + Environment.NewLine.Length;
                }
                catch (Exception writeEx)
                {
                    try
                    {
                        string fallback = _fallbackFile;
                        string line = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " 日志写入失败：" + writeEx;
                        File.AppendAllText(fallback, line + Environment.NewLine);
                    }
                    catch
                    {
                        // 兜底也失败：此处之后没有任何出口
                    }
                }
            }
        }

        /// <summary>按"能不能真的写进去"选日志目录，三级降级。</summary>
        private static string ResolveLogRoot()
        {
            // 1. 首选：C:\ProgramData\ChipAgingCheck\Logs —— 所有用户可写，不会被系统清理
            string programData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            string first = Path.Combine(programData, "ChipAgingCheck", "Logs");
            bool canWrite = CanWrite(first);
            if (canWrite == true)
            {
                return first;
            }

            // 2. 次选：exe 旁边的 Logs —— 开发时就是这里（bin\Debug\Logs）
            string second = Path.Combine(AppContext.BaseDirectory, "Logs");
            canWrite = CanWrite(second);
            if (canWrite == true)
            {
                return second;
            }

            // 3. 最后：临时目录，重启可能被清空，但至少能留下东西
            string third = Path.Combine(Path.GetTempPath(), "ChipAgingCheck_Logs");
            CanWrite(third);
            return third;
        }

        /// <summary>真写一个探针文件再删掉，比只看目录存不存在可靠。</summary>
        private static bool CanWrite(string dir)
        {
            try
            {
                Directory.CreateDirectory(dir);
                string probe = Path.Combine(dir, ".write-probe");
                File.WriteAllText(probe, "1");
                File.Delete(probe);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        /// <summary>找当天最后一个文件继续写；超过大小上限就换下一个序号。</summary>
        private static string FindOrCreateFile(string dir, string day, out long size)
        {
            int index = 0;

            while (File.Exists(Path.Combine(dir, FileName(day, index))))
            {
                index++;
            }

            if (index > 0)
            {
                index--;
            }

            string path = Path.Combine(dir, FileName(day, index));
            size = File.Exists(path) ? new FileInfo(path).Length : 0;

            if (size >= MaxFileSize)
            {
                index++;
                path = Path.Combine(dir, FileName(day, index));
                size = 0;
            }

            return path;
        }

        private static string FileName(string day, int index) => $"log_{day}_{index}.log";
    }
}
