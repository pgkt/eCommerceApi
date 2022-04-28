using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCommerceApi.Domain.Entities
{
    [Table("Merchants")]
    public class Merchant
    {
        [Key]
        public int MerchantId { get; set; }
        [Required]
        public string MerchantName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        [Required]
        public string LoginID { get; set; }
        [Required]
        public byte[] Password { get; set; }
        public int RoleId { get; set; }
        public string STATUS { get; set; }
    }
}