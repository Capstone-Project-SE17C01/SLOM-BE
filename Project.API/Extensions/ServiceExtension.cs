using AutoMapper;
using Net.payOS;
using Project.API.SignalR.Service;
using Project.Core.Entities.Business.DTOs.LoginDTOs;
using Project.Core.Entities.Business.DTOs.PaymentDTOs;
using Project.Core.Entities.Business.DTOs.ProfileDTOs;
using Project.Core.Entities.Business.DTOs.ReminderDTOs;
using Project.Core.Entities.Business.DTOs.ReportDTOs;
using Project.Core.Entities.General;
using Project.Core.Interfaces.IMapper;
using Project.Core.Interfaces.IRepositories;
using Project.Core.Interfaces.IServices;
using Project.Core.Mapper;
using Project.Infrastructure.Repositories;
using Project.Infrastructure.Services;

namespace Project.API.Extensions {
    public static class ServiceExtension {
        public static IServiceCollection RegisterService(this IServiceCollection services, IConfiguration config) {
            #region Services
            PayOS payOS = new PayOS(config["Environment:PAYOS_CLIENT_ID"] ?? throw new Exception("Cannot find environment"),
                    config["Environment:PAYOS_API_KEY"] ?? throw new Exception("Cannot find environment"),
                    config["Environment:PAYOS_CHECKSUM_KEY"] ?? throw new Exception("Cannot find environment"));
            services.AddSingleton(payOS);
            services.AddScoped<IEmailService, EmailService>();
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
            services.AddScoped<IReportTypeRepository, ReportTypeRepository>();
            services.AddScoped<IReportRepository, ReportRepository>();
            services.AddScoped<IReminderRepository, ReminderRepository>();

            // services.AddTransient<>();
            #endregion

            #region Mapper
            var configuration = new MapperConfiguration(cfg => {
                cfg.CreateMap<CognitoTokenResponse, LoginResponseDTO>();
                cfg.CreateMap<Core.Entities.General.Profile, ProfileByEmailResponse>()
                    .ForMember(dest => dest.LanguageCode, opt => { // Chỉ gọi MapFrom khi src.PreferredLanguage != null
                        opt.PreCondition(src => src.PreferredLanguage is not null);
                        opt.MapFrom(src => src.PreferredLanguage!.Code);
                    });
                cfg.CreateMap<Payment, HistoryPaymentDTO>()
                   .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                   .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                   .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.PaymentMethod))
                   .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                   .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
                   .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));
                cfg.CreateMap<CreateReportRequestDTO, Report>();

                cfg.CreateMap<CreateReminderDTO, Reminder>();

            });

            IMapper mapper = configuration.CreateMapper();
            services.AddSingleton<IBaseMapper<CognitoTokenResponse, LoginResponseDTO>>(new BaseMapper<CognitoTokenResponse, LoginResponseDTO>(mapper));
            services.AddSingleton<IBaseMapper<Core.Entities.General.Profile, ProfileByEmailResponse>>(new BaseMapper<Core.Entities.General.Profile, ProfileByEmailResponse>(mapper));
            services.AddSingleton<IBaseMapper<Payment, HistoryPaymentDTO>>(new BaseMapper<Payment, HistoryPaymentDTO>(mapper));
            services.AddSingleton<IBaseMapper<CreateReportRequestDTO, Report>>(new BaseMapper<CreateReportRequestDTO, Report>(mapper));
            services.AddSingleton<IBaseMapper<CreateReminderDTO, Reminder>>(new BaseMapper<CreateReminderDTO, Reminder>(mapper));

            #endregion

            return services;
        }
    }
}
