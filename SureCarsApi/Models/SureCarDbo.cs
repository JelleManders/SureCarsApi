using System.ComponentModel.DataAnnotations;

namespace SureCarsApi.Models
{
    public class SureCarDbo
    {
        [Key]
        public Guid ID { get; set; }
        public string Color { get; set; }
        public string PlateNumber { get; set; }
        public int Year { get; set; }
        public Guid? LeasedBy { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }

        public SureCarDto ToDto()
        {
            SureCarDto dto = new SureCarDto();
            dto.Color = Color;
            dto.PlateNumber = PlateNumber;
            dto.Year = Year;
            dto.LeasedBy = LeasedBy.ToString();
            dto.Status = Status;
            dto.Remarks = Remarks;

            return dto;
        }
    }
}
