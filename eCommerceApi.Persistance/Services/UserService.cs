using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using eCommerceApi.Domain.Entities;
using eCommerceApi.Domain.Helpers;
using eCommerceApi.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace eCommerceApi.Persistance.Services
{
    public class UserService : IUserService
    {
        private IApplicationDbContext _dbContext { get; }
        private readonly AppSettings _appSettings;
        private IMapper _mapper;

        public UserService(IApplicationDbContext dbContext, IOptions<AppSettings> appSettings, IMapper mapper)
        {
            _dbContext = dbContext;
            _appSettings = appSettings.Value;
            _mapper = mapper;
        }

        public async Task<User> Authenticate(string LoginId, string Password)
        {
            var userData = await _dbContext.Merchants.Where(m => m.STATUS != "Deleted" && m.LoginID == LoginId && m.Password == Encoding.ASCII.GetBytes(Password))
                                                .FirstOrDefaultAsync();

            if (userData == null)
                return null;

            var user = _mapper.Map<User>(userData);

            var role = await _dbContext.Roles.Where(r => r.RoleID == userData.RoleId).FirstOrDefaultAsync();
            user.Role = role.RoleName;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, Convert.ToString(userData.MerchantId)),
                    new Claim(ClaimTypes.Role, role.RoleName)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);

            return user;
        }
    }
}
