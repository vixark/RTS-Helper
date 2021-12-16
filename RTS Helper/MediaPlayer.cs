using System;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;
using static RTSHelper.Global;



namespace RTSHelper {



    class MediaPlayer {


        public static WMPLib.WindowsMediaPlayer? Player;


        public static void PlayFile(String path, int volume) {

            var file = Path.GetFileName(path);
            var extension = Path.GetExtension(path);

            if (file == Global.NoneSoundString) {
                return;
            } if (string.IsNullOrEmpty(extension)) {

                var m = Regex.Match(path, "Windows Beep +([0-9]+)Hz +([0-9]+)ms", RegexOptions.IgnoreCase);
                if (m.Success) {
                    var frequency = m.Groups[1].Value;
                    var duration = m.Groups[2].Value;
                    Console.Beep(int.Parse(frequency), int.Parse(duration));
                }
          
            } else {

                if (Player is null) {
                    Player = new WMPLib.WindowsMediaPlayer();
                    Player.PlayStateChange += new WMPLib._WMPOCXEvents_PlayStateChangeEventHandler(Player_PlayStateChange);
                    Player.MediaError += new WMPLib._WMPOCXEvents_MediaErrorEventHandler(Player_MediaError);
                }
                Player.settings.volume = volume;
                Player.URL = path; // A veces puede paralizarse la aplicación aquí. Una solución simple sería que el usuario cambie a beeps de Windows, pero sería preferible investigar por qué sucede y resolverlo.
                Player.controls.play();

            }

        } // PlayFile>


        public static void PlaySonidoInicio()
            => PlayFile(Path.Combine(DirectorioSonidosCortos, Preferencias.StepStartSound), Preferencias.StepStartSoundVolume);


        public static void PlaySonidoFinal()
            => PlayFile(Path.Combine(DirectorioSonidosLargos, Preferencias.StepEndSound), Preferencias.StepEndSoundVolume);


        private static void Player_PlayStateChange(int NewState) { }


        private static void Player_MediaError(object pMediaObject) 
            => MessageBox.Show("Error playing sound.");


        public static double GetDuration(string MediaFile) {

            try {
                var w = new WMPLib.WindowsMediaPlayer();
                var m = w.newMedia(MediaFile);
                w.close();
                return m.duration;
            } catch (Exception) {
                return 0;
            }

        } // GetDuration>


    } // MediaPlayer>



} // RTSHelper>
