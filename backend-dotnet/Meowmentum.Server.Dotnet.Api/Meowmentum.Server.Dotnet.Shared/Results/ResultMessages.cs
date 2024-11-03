﻿using System;
using System.Collections.Generic;
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
    }

    public static class Registration
    {
        public const string FailedToCreateUser = "Failed to create user:\n";
        public const string OperationError = "An operation error occurred during registration. Please try again.";
        public const string InvalidArgument = "Invalid argument provided. Please check your input and try again.";
        public const string UnexpectedError = "An unexpected error occurred during registration:\n";
    }

    public static class Otp
    {
        public const string OtpNotFound = "OTP not found or expired!";
        public const string WrongOtp = "Wrong OTP!";
        public const string OperationError = "An operation error occurred during OTP verification. Please try again.";
        public const string InvalidArgument = "Invalid argument provided for OTP verification. Please check and try again.";
        public const string UnexpectedError = "An unexpected error occurred while sending OTP:\n";
    }

    public static class Email
    {
        public const string NetworkError = "Network error while sending OTP:\n";
        public const string TimeoutError = "Request to send OTP timed out:\n";
        public const string UnexpectedError = "An unexpected error occurred while sending to email:\n";
    }
}
