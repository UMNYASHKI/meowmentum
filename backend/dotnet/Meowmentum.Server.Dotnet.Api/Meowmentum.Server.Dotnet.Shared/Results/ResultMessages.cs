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
        public const string InvalidResetToken = "Invalid Reset Password Token";
        public const string PasswordUpdated = "Password succesfully updated";
        public const string LogoutFailed = "Logout failed. Please try again.";
        public const string LogoutSuccess = "Logout successful.";
        public const string TokenBlacklisted = "Token is blacklisted";
        public const string NoCurrentUser = "No current user in the context";
        public const string FailedToGetCurrentUser = "Failed to get current user";
        public const string FailedToGetUserId = "Failed to get user ID from current user";
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
        public const string FailToSend = "Failed to send email";
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

    public static class Task
    {
        public const string TaskNotFound = "Task not found.";
        public const string NoFilteredTasks = "No tasks with filter.";
        public const string UnauthorizedAccess = "You are not authorized to access this task.";
        public const string UnauthorizedUpdate = "You are not authorized to update this task.";
        public const string UnauthorizedDelete = "You are not authorized to delete this task.";
        public const string InvalidTagId = "Invalid tag ID.";
        public const string UnexpectedError = "An unexpected error occurred.";
        public const string UserNotFound = "User not found or not authorized.";
        public const string InvalidFilterRequest = "Filter request contains invalid data.";
    }
    public static class Tag
    {
        public const string TagNotFound = "Tag not found!";
        public const string FetchTagsError = "Error fetching tags";
        public const string UnexpectedError = "An unexpected error:\n";
        public const string TagNameAlreadyExists = "A tag with the same name already exists.";
    }

    public static class Timer
    {
        public const string NoActiveTimer = "There is no active timers for this task";
        public const string ActiveTimerAlreadyExists = "There is already active timer for this task";
        public const string TimerNotFound = "Timer with this ID not found";
        public const string TimerNotBelongsToUser = "Timer not belongs to this user";
    }
}
