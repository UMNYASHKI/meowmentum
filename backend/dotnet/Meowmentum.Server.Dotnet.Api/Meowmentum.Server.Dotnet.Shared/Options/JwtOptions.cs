﻿namespace Meowmentum.Server.Dotnet.Shared.Options;

public class JwtOptions
{
    public const string SectionName = "JwtSettings";

    public required string Key { get; set; }

    public required string Issuer { get; set; }

    public required string Audience { get; set; }

    public int TokenValidityFromDays { get; set; }
}

