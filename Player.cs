using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WMPLib;

namespace CmdPlayer
{
    public class Player
    {
        public delegate void StatusChangedDelegate(string status);

        public event StatusChangedDelegate StatusChanged;


        WindowsMediaPlayer _player;        
        double _currentPosition;
        IWMPPlaylist playlist;
        

        public Player(IEnumerable<FileInfo> Playlist)
        {
            _player = new WindowsMediaPlayer();
            playlist = _player.playlistCollection.newPlaylist("myplaylist");
            foreach (var file in Playlist)
            {
                var media = _player.newMedia(file.FullName);
                playlist.appendItem(media);
            }
            _player.currentPlaylist = playlist;
            _player.controls.stop();
            _player.PlayStateChange += _player_PlayStateChange;
        }

        private void _player_PlayStateChange(int NewState)
        {
            switch (NewState)
            {
                case 1:
                    StatusChanged("Stopped " + _player.currentMedia.name + ".mp3");
                break;

                case 2:
                    StatusChanged("Paused " + _player.currentMedia.name + ".mp3");
                break;

                case 3:
                    StatusChanged("Playing " + _player.currentMedia.name + ".mp3");
                break;
            }
        }
        

        public void Play()
        {
            _player.controls.play();
        }
        
        
        public void Pause()
        {
            _currentPosition = _player.controls.currentPosition;
            _player.controls.pause();
        }


        public void Resume()
        {
            _player.controls.currentPosition = _currentPosition;
            _player.controls.play();
        }


        public void Stop()
        {
            _player.controls.stop();
        }


        public void Next()
        {
            _player.controls.next();
        }


        public void Previous()
        {
            _player.controls.previous();
        }
    }
}
