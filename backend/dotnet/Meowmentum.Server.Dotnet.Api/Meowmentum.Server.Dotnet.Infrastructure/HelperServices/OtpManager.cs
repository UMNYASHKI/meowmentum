using Meowmentum.Server.Dotnet.Infrastructure.Abstractions;
using Meowmentum.Server.Dotnet.Persistence.Abstractions;
using Meowmentum.Server.Dotnet.Shared.Options.Redis;
using Meowmentum.Server.Dotnet.Shared.Results;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using Meowmentum.Server.Dotnet.Shared.Extensions;

namespace Meowmentum.Server.Dotnet.Infrastructure.HelperServices;

public class OtpManager(
    IRedisCacheService redisCacheService,
    IOptions<OtpDbConfig> otpConfigOptions,
    ILogger<IOtpManager> logger) : IOtpManager
{
    private readonly OtpDbConfig _otpConfig = otpConfigOptions.Value;
    private readonly TimeSpan _otpExpirationTime = TimeSpan.FromMinutes(otpConfigOptions.Value.ExpirationTimeInMinutes);

    public string GenerateOtp()
    {
        var rng = new byte[4];
        RandomNumberGenerator.Fill(rng);
        return (BitConverter.ToUInt32(rng, 0) % 1000000).ToString("D6");
    }

    public async Task<Result<bool>> SaveOtpForUserAsync(long userId, string otp, CancellationToken token = default)
    {
        var setResult = await redisCacheService.SetAsync(_otpConfig.Prefix.Append(userId.ToString()), otp, _otpExpirationTime, _otpConfig.DbNumber, true, token);
        if (!setResult.IsSuccess)
        {
            logger.LogError(ResultMessages.Otp.OperationError);
            return Result.Failure<bool>(ResultMessages.Otp.OperationError);
        }

        return Result.Success(setResult.Data);
    }

    public async Task<Result<bool>> ValidateOtpAsync(long userId, string otp, CancellationToken token = default)
    {
        if (string.IsNullOrWhiteSpace(otp))
        {
            return Result.Failure<bool>(ResultMessages.User.InvalidOtpCode);
        }

        var storedOtp = await redisCacheService.GetAsync<string>(_otpConfig.Prefix.Append(userId.ToString()), _otpConfig.DbNumber, token);

        if (!storedOtp.IsSuccess)
        {
            logger.LogError(ResultMessages.Otp.OperationError);
            return Result.Failure<bool>(ResultMessages.Otp.OperationError);
        }

        if (storedOtp.Data != otp)
        {
            return Result.Failure<bool>(ResultMessages.User.InvalidOtpCode);
        }

        await redisCacheService.RemoveAsync(userId.ToString(), _otpConfig.DbNumber, token);

        return Result.Success(true);
    }
}
