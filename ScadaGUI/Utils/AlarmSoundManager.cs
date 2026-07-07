using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media;

namespace ScadaGUI.Utils
{
    // Pusta zvuk u petlji dok god postoji makar jedan neaktivan/neacknowledge-ovan alarm.
    // Koristi standardne Windows zvukove iz %WINDIR%\Media (uvek dostupni na Windows masinama).
    public static class AlarmSoundManager
    {
        private static readonly MediaPlayer player = new MediaPlayer();
        private static bool isPlaying;

        public static Dictionary<string, string> AvailableSounds { get; } = new Dictionary<string, string>
        {
            { "Alarm 01", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Media", "Alarm01.wav") },
            { "Alarm 02", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Media", "Alarm02.wav") },
            { "Alarm 03", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Media", "Alarm03.wav") },
        };

        public static string SelectedSoundName { get; set; } = "Alarm 01";

        public static double Volume
        {
            get => player.Volume;
            set => player.Volume = value;
        }

        static AlarmSoundManager()
        {
            player.Volume = 0.5;
            player.MediaEnded += (s, e) =>
            {
                if (isPlaying)
                {
                    player.Position = TimeSpan.Zero;
                    player.Play();
                }
            };
        }

        public static void PlayLoop()
        {
            if (isPlaying)
                return;

            if (!AvailableSounds.TryGetValue(SelectedSoundName, out string path) || !File.Exists(path))
                return;

            isPlaying = true;
            player.Open(new Uri(path));
            player.Play();
        }

        public static void Stop()
        {
            isPlaying = false;
            player.Stop();
        }
    }
}
