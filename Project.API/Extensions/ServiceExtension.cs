using AutoMapper;
using Net.payOS;
using Project.API.SignalR.Service;
using Project.Core.Entities.Business.DTOs.LoginDTOs;
using Project.Core.Entities.Business.DTOs.ProfileDTOs;
using Project.Core.Interfaces.IMapper;
using Project.Core.Interfaces.IRepositories;
using Project.Core.Mapper;
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
            // services.AddScoped<>();
            #endregion

            #region Repositories
            services.AddTransient<IProfileRepository, ProfileRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddScoped<IMessageService, MessageService>();
            services.AddScoped<IMeetingRepository, MeetingRepository>();
            services.AddScoped<ILanguageRepository, LanguageRepository>();
            services.AddScoped<IUserCourseProgressRepository, UserCourseProgressRepository>();
            services.AddScoped<IUserModuleProgressRepository, UserModuleProgressRepository>();
            services.AddScoped<IUserLessonProgressRepository, UserLessonProgressRepository>();
            services.AddScoped<ICourseRepository, CourseRepository>();
            services.AddScoped<IModuleRepository, ModuleRepository>();
            services.AddScoped<ILessonRepository, LessonRepository>();
            services.AddScoped<IVideoSuggestRepository, VideoSuggestRepository>();

            // services.AddTransient<>();
            #endregion

            #region Mapper
            var configuration = new MapperConfiguration(cfg => {
                cfg.CreateMap<CognitoTokenResponse, LoginResponseDTO>();
                cfg.CreateMap<Core.Entities.General.Profile, ProfileByEmailResponse>()
                    .ForMember(dest => dest.LanguageCode, opt => { // Chỉ gọi MapFrom khi src.PreferredLanguage != null
                        opt.PreCondition(src => src.PreferredLanguage is not null);
                        opt.MapFrom(src => src.PreferredLanguage!.Code);
                    }
                    );
            });

            IMapper mapper = configuration.CreateMapper();
            services.AddSingleton<IBaseMapper<CognitoTokenResponse, LoginResponseDTO>>(new BaseMapper<CognitoTokenResponse, LoginResponseDTO>(mapper));
            services.AddSingleton<IBaseMapper<Core.Entities.General.Profile, ProfileByEmailResponse>>(new BaseMapper<Core.Entities.General.Profile, ProfileByEmailResponse>(mapper));
            #endregion

            return services;
        }
    }
}
