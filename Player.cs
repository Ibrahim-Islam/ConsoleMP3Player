using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Timers;

namespace CmdPlayer
{
    public class Player
    {

        public event EventHandler PlaybackComplete;
        
        public FileInfo CurrentFile
        {
            get { return _playlist.ElementAt(_currentFileIndex); }
        }

        
        #region Private

        [DllImport("winmm.dll")]
        static extern int mciSendString(string mciCommand, StringBuilder buffer, int bufferSize, IntPtr callback);

        IEnumerable<FileInfo> _playlist;

        int _currentFileIndex;
        
        Timer _timer;

        void Send(string mciCommand)
        {
            mciSendString(mciCommand, null, 0, IntPtr.Zero);
        }

        int GetMediaLength()
        {
            StringBuilder returnData = new StringBuilder(128);
            var Pcommand = "status " + CurrentFile.FullName + " length";
            var error = mciSendString(Pcommand, returnData, returnData.Capacity, IntPtr.Zero);
            return int.Parse(returnData.ToString());
        }

        #endregion

        
        public Player(IEnumerable<FileInfo> Playlist)
        {
            _playlist = Playlist;
        }


        public string Play()
        {
            if (_playlist.Count() == 0) return "No file to play";

            //if (CurrentFile == null) CurrentFile = _playlist.FirstOrDefault();

            if (CurrentFile != null)
            {
                Send("open " + CurrentFile.FullName);

                var length = GetMediaLength();

                if (_timer == null)
                    _timer = new Timer(length);
                else
                    _timer.Interval = length;

                _timer.Elapsed += _timer_Elapsed;
                _timer.Enabled = true;
                _timer.Start();

                Send("play " + CurrentFile.FullName);

                return "Playing " + CurrentFile.Name;
            }
            else
            {
                return "No file to play";
            }
        }


        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _timer.Elapsed -= _timer_Elapsed;
            _timer.Enabled = false;
            _timer.Stop();
            if (PlaybackComplete != null) PlaybackComplete(sender, e);
        }


        public string Pause()
        {
            Send("pause " + CurrentFile.FullName);
            return "Paused " + CurrentFile.Name;
        }


        public string Resume()
        {
            Send("resume " + CurrentFile.FullName);
            return "Resume " + CurrentFile.Name;
        }


        public string Stop()
        {
            Send("close " + CurrentFile.FullName);
            var stoppedFileName = CurrentFile.Name.ToString();
            return "Stopped " + stoppedFileName;
        }


        public string Next()
        {
            _currentFileIndex = _currentFileIndex + 1;
            var nextFile = _playlist.ElementAtOrDefault(_currentFileIndex);

            if (nextFile == null)
            {
                return "This is the last file";
            }
            else
            {
                Stop();
                return Play(); 
            }
        }


        public string Previous()
        {
            _currentFileIndex = _currentFileIndex - 1;
            var previousFile = _playlist.ElementAtOrDefault(_currentFileIndex);

            if (previousFile == null)
            {
                return "This is the first file";
            }
            else
            {
                Stop();
                return Play();
            }
        }
    }
}
