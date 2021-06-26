using System;
using NAudio.Wave;

namespace Nebula.Core.Player
{
    public class SoundOut
    {
        public MediaFoundationReader Reader  { get; private set; }
        public WaveOutEvent          Out     { get; private set; }
        public bool                  IsReady => Reader != null && Out != null;

        public void Prepare(Uri uri)
        {
            CleanUp();
            Reader = new MediaFoundationReader(uri.ToString());
            Out ??= new WaveOutEvent();
            Out.Stop();
            Out.Init(Reader);
        }

        private void CleanUp()
        {
            Reader?.Dispose();
            Reader = null;
        }
    }
}