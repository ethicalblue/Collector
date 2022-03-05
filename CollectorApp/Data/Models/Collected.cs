namespace CollectorApp.Data
{
    public class Collected
    {
        public int Id { get; set; }
        public string? Campaign { get; set; }
        public string? HostnameIP { get; set; }
        public string? UserName { get; set; }
        public string? MachineName { get; set; }
        public string? BIOSSerialNumber { get; set; } /* wmic bios get serialnumber */
        public DateTime? CollectedDateTime { get; set; }
        public bool IsArchived { get; set; }
    }
}
