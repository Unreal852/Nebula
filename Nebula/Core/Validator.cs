using System.Text.RegularExpressions;
using Nebula.Model;

namespace Nebula.Core
{
    public static class Validator
    {
        // Im bad at regex : https://stackoverflow.com/questions/19693622/how-to-get-text-between-nested-parentheses
        private static readonly Regex BracketsRegex    = new(@"\[(?>[^\[\]]+|\( (?<Depth>)|\)(?<-Depth>))*(?(Depth)(?!))\]", RegexOptions.IgnorePatternWhitespace);
        private static readonly Regex ParenthesesRegex = new(@"\((?>[^()]+|\( (?<Depth>)|\)(?<-Depth>))*(?(Depth)(?!))\)", RegexOptions.IgnorePatternWhitespace);

        public static string ValidateMediaTitle(MediaInfo mediaInfo)
        {
            string title = mediaInfo.Title;
            // Step 1 - Check if the title start with the author name followed by a '-'. Ex : Billie Eilish - ...
            string[] split = title.Split('-');
            if (split.Length > 1)
            {
                string left = split[0].ToLower();
                if (left.Contains(mediaInfo.Author.ToLower()))
                    title = split[1].Trim();
            }

            // Step 2 - Check if the title contains something between brackets. Ex: [Official Video]
            title = BracketsRegex.Replace(title, "");
            // Step 3 - Check if the title contains something between parentheses. Ex: (Official Video)
            title = ParenthesesRegex.Replace(title, "");

            return title.Trim();
        }
    }
}