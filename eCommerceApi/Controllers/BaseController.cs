using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace eCommerceApi.Controllers
{
    [ApiController]
    [Route("")]
    public class BaseController : ControllerBase
    {
        public int GetUser()
        {
            var claimsIndentiy = this.User.Identity as ClaimsIdentity;
            var userId = claimsIndentiy.FindFirst(ClaimTypes.Name)?.Value;

            return Convert.ToInt32(userId);
        }
    }
}
