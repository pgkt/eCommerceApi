﻿namespace eCommerceApi.Domain.Entities
{
    public class User
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string LoginID { get; set; }
        public string Role { get; set; }
        public string Token { get; set; }
    }
}
