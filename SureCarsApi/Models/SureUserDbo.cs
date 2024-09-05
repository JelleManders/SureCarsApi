using System.ComponentModel.DataAnnotations;

namespace SureCarsApi.Models
{
    public class SureUserDbo
    {
        [Key]
        public Guid ID { get; set; }
        public string Name { get; set; }
    }
}
