using System;

namespace Nebula.Utils.Extensions
{
    /// <summary>
    /// Provide extensions for  <see cref="TimeSpan"/>
    /// </summary>
    public static class TimeSpanExtensions
    {
        /// <summary>
        /// Format the specified TimeSpan
        /// </summary>
        /// <param name="timeSpan">The TimeSpan to format</param>
        /// <returns>Formatted TimeSpan</returns>
        public static string ToFormattedHuman(this TimeSpan timeSpan)
        {
            string hours = timeSpan.Hours > 9 ? timeSpan.Hours.ToString() : $"0{timeSpan.Hours}";
            string minutes = timeSpan.Minutes > 9 ? timeSpan.Minutes.ToString() : $"0{timeSpan.Minutes}";
            string seconds = timeSpan.Seconds > 9 ? timeSpan.Seconds.ToString() : $"0{timeSpan.Seconds}";
            return $"{hours}:{minutes}:{seconds}";
        }
    }
}