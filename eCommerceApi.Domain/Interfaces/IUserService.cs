using System.Threading.Tasks;
using eCommerceApi.Domain.Entities;

namespace eCommerceApi.Domain.Interfaces
{
    public interface IUserService
    {
        Task<User> Authenticate(string LoginId, string Password);
    }
}
