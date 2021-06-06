using System;
using static Nebula.NebulaClient;

namespace Nebula.Core.Extensions
{
    /// <summary>
    /// Provide extensions for  <see cref="TimeSpan"/>
    /// </summary>
    public static class TimeSpanExtensions
    {
        public static string ToFormattedHuman(this TimeSpan timeSpan)
        {
            string hours =
                $"{(timeSpan.Hours > 9 ? timeSpan.Hours.ToString() : $"0{timeSpan.Hours}")} {GetLang(timeSpan.Hours > 1 ? "global_hours" : "global_hour")}";
            string minutes =
                $"{(timeSpan.Minutes > 9 ? timeSpan.Minutes.ToString() : $"0{timeSpan.Minutes}")} {GetLang(timeSpan.Minutes > 1 ? "global_minutes" : "global_minute")}";
            string seconds =
                $"{(timeSpan.Seconds > 9 ? timeSpan.Seconds.ToString() : $"0{timeSpan.Seconds}")} {GetLang(timeSpan.Seconds > 1 ? "global_seconds" : "global_second")}";
            string result = $"{hours} {minutes} {seconds}";
            if (timeSpan.Days > 0)
            {
                string day = $"{timeSpan.Days} {GetLang(timeSpan.Days > 1 ? "global_days" : "global_day")}";
                result = $"{day} {result}";
            }

            return result;
        }

        /// <summary>
        /// Format the specified TimeSpan
        /// </summary>
        /// <param name="timeSpan">The TimeSpan to format</param>
        /// <returns>Formatted TimeSpan</returns>
        public static string ToSimpleFormattedHuman(this TimeSpan timeSpan)
        {
            string hours = timeSpan.Hours > 9 ? timeSpan.Hours.ToString() : $"0{timeSpan.Hours}";
            string minutes = timeSpan.Minutes > 9 ? timeSpan.Minutes.ToString() : $"0{timeSpan.Minutes}";
            string seconds = timeSpan.Seconds > 9 ? timeSpan.Seconds.ToString() : $"0{timeSpan.Seconds}";
            string result = $"{hours}:{minutes}:{seconds}";
            return timeSpan.Days > 0 ? SimpleIncludeDays(timeSpan, result) : result;
        }

        private static string SimpleIncludeDays(TimeSpan timeSpan, string formattedStr)
        {
            string days = timeSpan.Days > 9 ? timeSpan.Days.ToString() : $"0{timeSpan.Days}";
            return $"{days}:{formattedStr}";
        }
    }
}