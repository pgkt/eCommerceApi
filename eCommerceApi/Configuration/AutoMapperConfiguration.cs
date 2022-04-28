using AutoMapper;
using eCommerceApi.Configuration.AutoMapperProfiles;

namespace eCommerceApi.Configuration
{
    public class AutoMapperConfiguration
    {
        public static IMapper Configure()
        {
            var config = new MapperConfiguration(mc =>
            {
                mc.AddProfile( new ECommerceMappingProfile());
            });

            return config.CreateMapper();
        }
    }
}
