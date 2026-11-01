using ChipAgingCheck.Domain.Modbus;
using Newtonsoft.Json;
using System;
using System.IO;

namespace ChipAgingCheck.Domain.Common
{
    // 配置文件的读写：读不到、读坏了、版本不对，一律回默认值，绝不给调用方 null
    public static class ConfigManager
    {
        private const int CurrentVersion = 1;
        private static readonly object FileLock = new object(); // Load/Save 和下面的文件操作都靠它串起来

        // 配置、数据都放它下面，所以得先找着能写的地儿（普通用户也能写）
        public static readonly string RootDirectory = ResolveRootDirectory();

        private static readonly string ConfigDirectory = Path.Combine(RootDirectory, "Config");

        public static string ConfigFilePath
        {
            get { return Path.Combine(ConfigDirectory, "config.json"); }
        }

        // %ProgramData% 能写就用它，不能写退到程序目录，再不行退临时目录
        private static string ResolveRootDirectory()
        {
            string programData = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                "ChipAgingCheck");
            bool programDataWritable = CanWrite(programData);
            if (programDataWritable)
            {
                return programData;
            }
            string besideExe = AppContext.BaseDirectory;
            bool exeDirectoryWritable = CanWrite(besideExe);
            if (exeDirectoryWritable)
            {
                return besideExe;
            }
            return Path.Combine(Path.GetTempPath(), "ChipAgingCheck_Data");
        }

        // 目录存在不等于能写，所以真写一个文件试一下
        private static bool CanWrite(string directory)
        {
            try
            {
                Directory.CreateDirectory(directory);
                string probePath = Path.Combine(directory, ".write-probe");
                File.WriteAllText(probePath, "probe");
                File.Delete(probePath);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static ConfigModel Load()
        {
            lock (FileLock)
            {
                string path = ConfigFilePath;
                bool fileExists = File.Exists(path);
                if (fileExists == false)
                {
                    ConfigModel brandNew = new ConfigModel();
                    Save(brandNew); // 第一次运行，先落一份盘，下次就有文件了
                    return brandNew;
                }

                string text = "";
                try
                {
                    text = File.ReadAllText(path);
                }
                catch (Exception)
                {
                    return RebuildDefault();
                }

                ConfigModel loaded = null;
                try
                {
                    loaded = JsonConvert.DeserializeObject<ConfigModel>(text);
                }
                catch (Exception)
                {
                    return RebuildDefault();
                }

                bool parseFailed = loaded == null;
                if (parseFailed)
                {
                    return RebuildDefault();
                }

                if (loaded.Version != CurrentVersion) // 老格式不猜，重建一份新的
                {
                    return RebuildDefault();
                }

                // 老文件可能缺块，缺了补上，后面就不用到处判 null
                if (loaded.Param == null)
                {
                    loaded.Param = new ModbusParam();
                }
                if (loaded.Recipe == null)
                {
                    loaded.Recipe = new Recipe();
                }
                return loaded;
            }
        }

        // 先写 .tmp 再替换：写一半断电也不会把原来那份好的配置文件写坏
        public static void Save(ConfigModel config)
        {
            lock (FileLock)
            {
                if (config == null)
                {
                    return;
                }
                config.Version = CurrentVersion;
                try
                {
                    Directory.CreateDirectory(ConfigDirectory);
                    string json = JsonConvert.SerializeObject(config, Formatting.Indented);
                    string path = ConfigFilePath;
                    string tempPath = path + ".tmp";
                    File.WriteAllText(tempPath, json);
                    bool oldExists = File.Exists(path);
                    if (oldExists)
                    {
                        File.Replace(tempPath, path, null); // 原子替换
                        return;
                    }
                    File.Move(tempPath, path);
                }
                catch (Exception)
                {
                    // 存不进去不该让程序崩，这一轮用内存里的值接着跑
                }
            }
        }

        private static ConfigModel RebuildDefault()
        {
            ConfigModel fresh = new ConfigModel();
            Save(fresh);
            return fresh;
        }
    }
}
