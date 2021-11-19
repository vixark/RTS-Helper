using System;
using System.Windows.Forms;



namespace RTSHelper {



    class MediaPlayer {


        public static WMPLib.WindowsMediaPlayer Player;


        public static void PlayFile(String url, int volume) {

            if (Player is null) {
                Player = new WMPLib.WindowsMediaPlayer();
                Player.PlayStateChange += new WMPLib._WMPOCXEvents_PlayStateChangeEventHandler(Player_PlayStateChange);
                Player.MediaError += new WMPLib._WMPOCXEvents_MediaErrorEventHandler(Player_MediaError);
            }
            Player.settings.volume = volume;
            Player.URL = url;
            Player.controls.play();

        } // PlayFile>


        private static void Player_PlayStateChange(int NewState) { }


        private static void Player_MediaError(object pMediaObject) 
            => MessageBox.Show("Error playing sound.");


    } // MediaPlayer>



} // RTSHelper>
