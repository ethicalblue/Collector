namespace CollectorApp.Data
{
    public class Link
    {
        public int Id { get; set; }
        public string? Campaign { get; set; }
        public string? Description { get; set; }
        public string? HostnameIP { get; set; }
        public bool? LinkWasClicked { get; set; }
        public int? ClickCounter { get; set; }
        public string? RedirectToURL { get; set; }
        public DateTime? ClickedDateTime { get; set; }
        public bool IsArchived { get; set; }
    }
}
