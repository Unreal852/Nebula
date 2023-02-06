using System.Text.RegularExpressions;
using Nebula.Common.Medias;

namespace Nebula.Common.Extensions;

public static partial class MediaInfosExtensions
{
    [GeneratedRegex("\\[(?>[^\\[\\]]+|\\( (?<Depth>)|\\)(?<-Depth>))*(?(Depth)(?!))\\]", RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace)]
    private static partial Regex BracketsRegex();
    [GeneratedRegex("\\((?>[^()]+|\\( (?<Depth>)|\\)(?<-Depth>))*(?(Depth)(?!))\\)", RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace)]
    private static partial Regex ParenthesesRegex();

    public static string GetFormattedTitle(this IMediaInfo mediaInfo)
    {
        string title = mediaInfo.Title!;
        // Step 1 - Check if the title start with the author name followed by a '-'. Ex : Billie Eilish - ...
        string[] split = title.Split('-', 2);
        if(split.Length > 1 && mediaInfo.Author != null)
        {
            string left = split[0].Replace(" ", string.Empty);
            string right = mediaInfo.Author.Replace(" ", string.Empty);
            if(right.Contains(left, StringComparison.CurrentCultureIgnoreCase))
                title = split[1].Trim();
        }

        // Step 2 - Check if the title contains something between brackets. Ex: [Official Video]
        title = BracketsRegex().Replace(title, "");
        // Step 3 - Check if the title contains something between parentheses. Ex: (Official Video)
        title = ParenthesesRegex().Replace(title, "");

        return title.Trim();
    }
}