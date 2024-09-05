namespace SureCarsApi.Models
{
    public class SureCarDto
    {
        public string Color { get; set; }
        public string PlateNumber { get; set; }
        public int Year { get; set; }
        public string? LeasedBy { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
    }
}
