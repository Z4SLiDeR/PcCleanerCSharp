using Microsoft.Win32;
using System;
using System.Management;
using System.Text;

namespace TutoPcCleaner.Helpers
{
    public class Sysinfos
    {
        /// <summary>
        /// Avoir la version de windows
        /// </summary>
        /// <returns>Windows version string</returns>
        public string GetWinVer()
        {
            try
            {
                return Environment.OSVersion.ToString();
            }
            catch
            {
                return "Windows";
            }
        }

        /// <summary>
        /// Récuperer les infos hardware (CPU, GPU)
        /// </summary>
        /// <returns>String</returns>
        public string GetHardwareInfos()
        {
            StringBuilder sb = new StringBuilder();

            // CPU
            RegistryKey processor_name = Registry.LocalMachine.OpenSubKey(@"Hardware\Description\System\CentralProcessor\0", RegistryKeyPermissionCheck.ReadSubTree);

            if (processor_name != null)
            {
                sb.AppendLine($"CPU: {processor_name.GetValue("ProcessorNameString")}");
            }

            try
            {
                string gpuInfo = GetGpuInfo();
                if (!string.IsNullOrEmpty(gpuInfo))
                {
                    sb.AppendLine($"GPU: {gpuInfo}");
                }
            }
            catch (Exception ex)
            {
                sb.AppendLine("GPU: Not available");
            }

            return sb.ToString();
        }

        /// <summary>
        /// Méthode pour récupérer les informations sur les GPU via WMI
        /// </summary>
        /// <returns>String contenant le nom du GPU</returns>
        private string GetGpuInfo()
        {
            StringBuilder gpuInfo = new StringBuilder();

            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from Win32_VideoController");
                foreach (ManagementObject obj in searcher.Get())
                {
                    gpuInfo.AppendLine(obj["Name"].ToString());
                }
            }
            catch
            {
                return "Unknown GPU";
            }

            return gpuInfo.ToString().Trim();
        }
    }
}
