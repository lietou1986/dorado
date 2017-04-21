using System;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;

namespace Dorado.SystemInfo
{
    public class SystemInfo
    {
        private const int CHAR_COUNT = 128;

        [DllImport("kernel32")]
        private static extern void GetWindowsDirectory(StringBuilder WinDir, int count);

        [DllImport("kernel32")]
        private static extern void GetSystemDirectory(StringBuilder SysDir, int count);

        [DllImport("kernel32")]
        private static extern void GetSystemInfo(ref CpuInfo cpuInfo);

        [DllImport("kernel32")]
        private static extern void GlobalMemoryStatus(ref MemoryInfo memInfo);

        [DllImport("kernel32")]
        private static extern void GetSystemTime(ref SystemTimeInfo sysInfo);

        /// <summary>

        /// 查询CPU编号
        /// </summary>
        /// <returns></returns>
        public static string GetCpuId()
        {
            ManagementClass mClass = new ManagementClass("Win32_Processor");
            ManagementObjectCollection moc = mClass.GetInstances();
            string cpuId = null;
            foreach (ManagementObject mo in moc)
            {
                cpuId = mo.Properties["ProcessorId"].Value.ToString();
                break;
            }
            return cpuId;
        }

        /// <summary>

        /// 查询硬盘编号
        /// </summary>
        /// <returns></returns>
        public static string GetMainHardDiskId()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMedia");
            String hardDiskID = null;
            foreach (ManagementObject mo in searcher.Get())
            {
                hardDiskID = mo["SerialNumber"].ToString().Trim();
                break;
            }
            return hardDiskID;
        }

        /// <summary>

        /// 获取Windows目录
        /// </summary>
        /// <returns></returns>
        public static string GetWinDirectory()
        {
            StringBuilder sBuilder = new StringBuilder(CHAR_COUNT);
            GetWindowsDirectory(sBuilder, CHAR_COUNT);
            return sBuilder.ToString();
        }

        /// <summary>

        /// 获取系统目录
        /// </summary>
        /// <returns></returns>
        public static string GetSysDirectory()
        {
            StringBuilder sBuilder = new StringBuilder(CHAR_COUNT);
            GetSystemDirectory(sBuilder, CHAR_COUNT);
            return sBuilder.ToString();
        }

        /// <summary>

        /// 获取CPU信息
        /// </summary>
        /// <returns></returns>
        public static CpuInfo GetCpuInfo()
        {
            CpuInfo cpuInfo = new CpuInfo();
            GetSystemInfo(ref cpuInfo);
            return cpuInfo;
        }

        /// <summary>

        /// 获取系统内存信息
        /// </summary>
        /// <returns></returns>
        public static MemoryInfo GetMemoryInfo()
        {
            MemoryInfo memoryInfo = new MemoryInfo();
            GlobalMemoryStatus(ref memoryInfo);
            return memoryInfo;
        }

        /// <summary>

        /// 获取系统时间信息
        /// </summary>
        /// <returns></returns>
        public static SystemTimeInfo GetSystemTimeInfo()
        {
            SystemTimeInfo systemTimeInfo = new SystemTimeInfo();
            GetSystemTime(ref systemTimeInfo);
            return systemTimeInfo;
        }

        /// <summary>

        /// 获取系统名称
        /// </summary>
        /// <returns></returns>
        public static string GetOperationSystemInName()
        {
            OperatingSystem os = Environment.OSVersion;
            string osName = "UNKNOWN";
            switch (os.Platform)
            {
                case PlatformID.Win32Windows:
                    switch (os.Version.Minor)
                    {
                        case 0: osName = "Windows 95"; break;
                        case 10: osName = "Windows 98"; break;
                        case 90: osName = "Windows ME"; break;
                    }
                    break;

                case PlatformID.Win32NT:
                    switch (os.Version.Major)
                    {
                        case 3: osName = "Windws NT 3.51"; break;
                        case 4: osName = "Windows NT 4"; break;
                        case 5:
                            if (os.Version.Minor == 0)
                            {
                                osName = "Windows 2000";
                            }
                            else if (os.Version.Minor == 1)
                            {
                                osName = "Windows XP";
                            }
                            else if (os.Version.Minor == 2)
                            {
                                osName = "Windows Server 2003";
                            }
                            break;

                        case 6: osName = "Longhorn"; break;
                    }
                    break;
            }
            return string.Format("{0},{1}", osName, os.Version.ToString());
        }
    }
}