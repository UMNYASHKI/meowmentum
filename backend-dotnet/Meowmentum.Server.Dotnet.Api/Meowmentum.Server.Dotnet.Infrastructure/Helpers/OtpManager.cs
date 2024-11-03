using Meowmentum.Server.Dotnet.Business.Abstractions;
using Meowmentum.Server.Dotnet.Shared.Results;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Meowmentum.Server.Dotnet.Infrastructure.Helpers;

public class OtpManager(IMemoryCache memoryCache) : IOtpManager
{
    private readonly TimeSpan _otpExpirationTime = TimeSpan.FromMinutes(5);

    public string GenerateOtp()
    {
        var rng = new byte[4];
        RandomNumberGenerator.Fill(rng);
        return (BitConverter.ToUInt32(rng, 0) % 1000000).ToString("D6");
    }

    public async Task SaveOtpForUserAsync(long userId, string otp, CancellationToken token = default)
    {
        memoryCache.Set(userId, otp, _otpExpirationTime);
        await Task.CompletedTask;
    }

    public async Task<Result<bool>> ValidateOtpAsync(long userId, string otp, CancellationToken token = default)
    {
        token.ThrowIfCancellationRequested();

        if (!memoryCache.TryGetValue(userId, out string storedOtp))
        {
            return Result.Failure<bool>(ResultMessages.Otp.OperationError);
        }

        if (storedOtp != otp)
        {
            return Result.Failure<bool>(ResultMessages.User.InvalidOtpCode);
        }

        memoryCache.Remove(userId);
        return Result.Success(true);
    }
}
