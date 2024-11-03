﻿using Meowmentum.Server.Dotnet.Business.Abstractions;
using Meowmentum.Server.Dotnet.Shared.Results;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Meowmentum.Server.Dotnet.Infrastructure.Implementations;

public class OtpService : IOtpService
{
    private readonly IMemoryCache _memoryCache;
    private readonly TimeSpan _otpExpirationTime = TimeSpan.FromMinutes(5);

    public OtpService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public string GenerateOtp()
    {
        var rng = new byte[4];
        RandomNumberGenerator.Fill(rng);
        return (BitConverter.ToUInt32(rng, 0) % 1000000).ToString("D6");
    }

    public async Task SaveOtpForUserAsync(long userId, string otp)
    {
        _memoryCache.Set(userId, otp, _otpExpirationTime);
        await Task.CompletedTask;
    }

    public async Task<Result<bool>> ValidateOtpAsync(long userId, string otp)
    {
        if (!_memoryCache.TryGetValue(userId, out string storedOtp))
        {
            return Result.Failure<bool>("OTP not found or expired!");
        }

        if (storedOtp != otp)
        {
            return Result.Failure<bool>("Wrong OTP!");
        }

        _memoryCache.Remove(userId);
        return Result.Success(true);
    }
}
