using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCommerceApi.ViewModels
{
    public class MerchantViewModel
    {
        public int MerchantId { get; set; }
        public string MerchantName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string LoginID { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string STATUS { get; set; }
    }
}
