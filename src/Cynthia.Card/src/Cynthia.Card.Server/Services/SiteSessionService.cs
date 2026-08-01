using Microsoft.AspNetCore.DataProtection;
using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Cynthia.Card.Server.Services
{
    public class SiteSessionService
    {
        private static readonly TimeSpan SessionLifetime = TimeSpan.FromDays(7);
        private readonly IDataProtector _protector;

        public SiteSessionService(IDataProtectionProvider dataProtectionProvider)
        {
            _protector = dataProtectionProvider.CreateProtector(
                "LegacyGwent.DiyAi.SiteSession.v1");
        }

        public string Create(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentException("A user name is required.", nameof(userName));
            }

            var expiresUtc = DateTime.UtcNow.Add(SessionLifetime);
            var encodedUser = Convert.ToBase64String(Encoding.UTF8.GetBytes(userName));
            var payload = string.Concat(
                expiresUtc.Ticks.ToString(CultureInfo.InvariantCulture),
                ":",
                encodedUser);
            return _protector.Protect(payload);
        }

        public bool TryRead(string token, out string userName)
        {
            userName = string.Empty;
            if (string.IsNullOrWhiteSpace(token))
            {
                return false;
            }

            try
            {
                var payload = _protector.Unprotect(token);
                var separator = payload.IndexOf(':');
                if (separator <= 0 || separator == payload.Length - 1)
                {
                    return false;
                }

                if (!long.TryParse(
                    payload.Substring(0, separator),
                    NumberStyles.None,
                    CultureInfo.InvariantCulture,
                    out var expiryTicks))
                {
                    return false;
                }

                var expiresUtc = new DateTime(expiryTicks, DateTimeKind.Utc);
                if (expiresUtc <= DateTime.UtcNow)
                {
                    return false;
                }

                var encodedUser = payload.Substring(separator + 1);
                userName = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUser));
                if (string.IsNullOrWhiteSpace(userName))
                {
                    userName = string.Empty;
                    return false;
                }

                return true;
            }
            catch (CryptographicException)
            {
                return false;
            }
            catch (FormatException)
            {
                return false;
            }
            catch (ArgumentOutOfRangeException)
            {
                return false;
            }
        }
    }
}
