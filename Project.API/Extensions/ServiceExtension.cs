using AutoMapper;

namespace Project.API.Extensions
{
    public static class ServiceExtension
    {
        public static IServiceCollection RegisterService(this IServiceCollection services)
        {
            #region Services
            // services.AddScoped<>();
            #endregion

            #region Repositories
            // services.AddTransient<>();
            #endregion

            #region Mapper
            var configuration = new MapperConfiguration(cfg =>
            {
                // cfg.CreateMap<>();
            });

            IMapper mapper = configuration.CreateMapper();

            // services.AddSingleton<IBaseMapper<>>(new BaseMapper<>(mapper));
            #endregion

            return services;
        }
    }
}
