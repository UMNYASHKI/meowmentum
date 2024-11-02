using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meowmentum.Server.Dotnet.Business.Abstractions;

public interface IEmailService
{
    Task SendOtpByEmailAsync(string email, string otp);
}
