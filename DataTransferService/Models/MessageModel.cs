using System.ComponentModel.DataAnnotations;

namespace DataTransferService.Models
{
    public class MessageModel
    {
        [Required]
        public int Id { get; set; }

        // [JsonProperty("dateTime")]
        public DateTime? DateTime { get; set; }

        [Required]
        public bool IsChecked { get; set; }
    }
}
