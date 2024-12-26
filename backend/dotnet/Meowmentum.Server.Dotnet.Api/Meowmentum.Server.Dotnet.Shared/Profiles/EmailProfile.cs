using AutoMapper;
using Meowmentum.Server.Dotnet.Proto.Email;
using Meowmentum.Server.Dotnet.Shared.Requests.Email;

namespace Meowmentum.Server.Dotnet.Shared.Profiles
{
    public class EmailProfile : Profile
    {
        public EmailProfile()
        {
            CreateMap<OtpEmailSendingRequest, RegistrationConfirmationRequest>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.ConfirmationCode, opt => opt.MapFrom(src => src.Otp))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));

            CreateMap<ResetPasswordEmailSendingRequest, PasswordResetRequest>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.ConfirmationCode, opt => opt.MapFrom(src => src.Otp));

            CreateMap<UpcomingTaskSendingRequest, DeadlineOneDayAlertRequest>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.TaskName, opt => opt.MapFrom(src => src.TaskName))
                .ForMember(dest => dest.TaskURL, opt => opt.MapFrom(src => src.TaskUrl));

            CreateMap<OverdueTaskSendingRequest, OverdueAlertRequest>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.TaskCount, opt => opt.MapFrom(src => src.TaskCount));
        }
    }
}
