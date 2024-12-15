using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Assets.Scripts.Common.Helpers
{
    public class Helpers
    {
        public static bool IsEmailValid(string email)
        {
            return email != null && new Regex("^[0-9a-zA-Z]([.-]?\\w+)*@[0-9a-zA-Z]([.-]?[0-9a-zA-Z])*(\\.[0-9a-zA-Z]{2,4})+$").IsMatch(email);
        }

        public static bool IsPasswordValid(string password)
        {
            if (password.Length < 6)
            {
                return false;
            }
            if (!password.Any(char.IsUpper))
            {
                return false;
            }
            if (!password.Any(char.IsLower))
            {
                return false;
            }
            if (!password.Any(char.IsDigit))
            {
                return false;
            }

            return true;
        }

        public static DateTime ConvertUtcToLocalTime(DateTime utcDateTime)
        {
            var localTimeZone = TimeZoneInfo.Local;

            return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, localTimeZone);
        }
    }
}
