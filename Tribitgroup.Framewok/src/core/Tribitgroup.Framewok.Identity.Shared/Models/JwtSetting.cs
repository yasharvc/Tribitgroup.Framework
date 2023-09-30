﻿namespace Tribitgroup.Framewok.Identity.Shared.Models
{
    public sealed class JwtSetting
    {
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public IEnumerable<string> Audiences { get; set; } = Enumerable.Empty<string>();
        public uint ExpiresInSeconds { get; set; }
        public uint RefreshTokenExpiresInMinutes { get; set; }
        public string Secret { get; set; } = string.Empty;
    }
}