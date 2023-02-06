namespace Nebula.Common.Audio;

public interface IPositionChangedHandler
{
    void OnPositionChanged(in TimeSpan timeSpan);
}
