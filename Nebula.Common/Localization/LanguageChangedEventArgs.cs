namespace Nebula.Common.Localization;
public sealed class LanguageChangedEventArgs : EventArgs
{
    public LanguageChangedEventArgs(LanguageInfo old, LanguageInfo @new)
    {
        Old = old;
        New = @new;
    }

    public LanguageInfo Old { get; }
    public LanguageInfo New { get; }
}
