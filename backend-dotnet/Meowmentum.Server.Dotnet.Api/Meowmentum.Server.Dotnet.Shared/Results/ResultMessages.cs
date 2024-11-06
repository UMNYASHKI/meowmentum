using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meowmentum.Server.Dotnet.Shared.Results;

public static class ResultMessages
{
    public static class User
    {
        public const string EmailAlreadyExists = "User with this email already exists!";
        public const string UserNotFound = "User not found!";
        public const string InvalidOtpCode = "Invalid OTP code!";
        public const string WrongPassword = "Wrong password";
        public const string InvalidToken = "Invalid token";    
        public const string ExpiredToken = "Expired token";
        public const string LogoutFailed = "Logout failed. Please try again.";
    }

    public static class Registration
    {
        public const string Success = "User registered successfully. Please verify your email!";
        public const string FailedToCreateUser = "Failed to create user:\n";
        public const string OperationError = "An operation error occurred during registration. Please try again.";
        public const string InvalidArgument = "Invalid argument provided. Please check your input and try again.";
        public const string UnexpectedError = "An unexpected error occurred during registration:\n";
    }

    public static class Otp
    {
        public const string OtpVerified = "OTP verification succeeded.";
        public const string OtpNotFound = "OTP not found or expired!";
        public const string WrongOtp = "Wrong OTP!";
        public const string OperationError = "An operation error occurred during OTP verification. Please try again.";
        public const string InvalidArgument = "Invalid argument provided for OTP verification. Please check and try again.";
        public const string UnexpectedError = "An unexpected error occurred while sending OTP:\n";
        public const string FailedToSaveOtp = "Failed to save OTP for user:\n";
    }

    public static class Email
    {
        public const string NetworkError = "Network error while sending OTP:\n";
        public const string TimeoutError = "Request to send OTP timed out:\n";
        public const string UnexpectedError = "An unexpected error occurred while sending to email:\n";
    }

    public static class Cancellation
    {
        public const string OperationCanceled = "Operation was canceled.";
    }

    public static class Json
    {
        public const string SerializationError = "Failed to serialize object";
    }

    public static class RedisCache
    {
        public const string FailToSet = "Failed to set value in Redis cache";
        public const string FailToCheck = "Failed to check value in Redis cache";
        public const string FailToRemove = "Failed to remove value from Redis cache";
        public const string FailToGet = "Failed to get value from Redis cache";

        public const string KeyNotFound = "Key not found in Redis cache";
        public const string NullOrEmptyValue = "Value is null or empty";
    }
}
