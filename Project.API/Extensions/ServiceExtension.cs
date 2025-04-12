using AutoMapper;
using Net.payOS;
using Project.Core.Interfaces.IRepositories;
using Project.Infrastructure.Model.ASLPredictor;
using Project.Infrastructure.Repositories;

namespace Project.API.Extensions {
    public static class ServiceExtension {
        public static IServiceCollection RegisterService(this IServiceCollection services) {
            #region Services
            IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            PayOS payOS = new PayOS(config["Environment:PAYOS_CLIENT_ID"] ?? throw new Exception("Cannot find environment"),
                    config["Environment:PAYOS_API_KEY"] ?? throw new Exception("Cannot find environment"),
                    config["Environment:PAYOS_CHECKSUM_KEY"] ?? throw new Exception("Cannot find environment"));
            services.AddSingleton(payOS);

            services.AddSingleton(provider => {
                var env = provider.GetRequiredService<IWebHostEnvironment>();
                var modelPath = Path.Combine(env.ContentRootPath, "Models", "WLASL20c_model.onnx");
                return new ASLPredictor(modelPath);
            });
            // services.AddScoped<>();
            #endregion

            #region Repositories
            services.AddTransient<IProfileRepository, ProfileRepository>();
            // services.AddTransient<>();
            #endregion

            #region Mapper
            var configuration = new MapperConfiguration(cfg => {
                // cfg.CreateMap<>();
            });

            IMapper mapper = configuration.CreateMapper();

            // services.AddSingleton<IBaseMapper<>>(new BaseMapper<>(mapper));
            #endregion

            return services;
        }
    }
}
