﻿using Meowmentum.Server.Dotnet.Business.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meowmentum.Server.Dotnet.Infrastructure.Implementations;

public class EmailService : IEmailService
{
    public Task SendOtpByEmailAsync(string email, string otp)
    {
        Console.WriteLine($"OTP {otp} sent to {email}.");
        return Task.CompletedTask;
    }
}
