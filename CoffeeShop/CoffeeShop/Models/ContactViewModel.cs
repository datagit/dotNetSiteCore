using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CoffeeShop.Models
{
    public class ContactViewModel
    {
        public string FormId { get; set; }
        [DisplayName("Name")]
        [Required]
        public string Name { get; set; }
        [DisplayName("Email Address")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [DisplayName("Phone Number")]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }
        [DisplayName("Message")]
        public string Message { get; set; }
    }
}