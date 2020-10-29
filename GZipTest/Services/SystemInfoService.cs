using System.Management;

namespace GZipTest.Services
{
    sealed class SystemInfoService
    {
        private ObjectQuery _objectQuery { get; set; } = new ObjectQuery("SELECT * FROM Win32_OperatingSystem");

        public long FreeVirtualMemory()
        {
            ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(_objectQuery);
            ManagementObjectCollection managementObjectCollection = managementObjectSearcher.Get();
            foreach (ManagementObject managementObject in managementObjectCollection)
            {
                if (long.TryParse(managementObject["FreeVirtualMemory"].ToString(), out long amount))
                {
                    return amount;
                }

                return 0;
            }

            return 0;
        }
    }
}
