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
        /// <summary>
        /// Gets the current active file
        /// </summary>
        private FileInfo _currentFile;
        public FileInfo CurrentFile
        {
            get { return _currentFile; }
            private set
            {
                _currentFile = value;
                _currentFileFullName = value == null ? null : value.FullName;
            }
        }

        public event EventHandler PlaybackComplete;

        #region Private

        [DllImport("winmm.dll")]
        static extern int mciSendString(string mciCommand, StringBuilder buffer, int bufferSize, IntPtr callback);

        IEnumerable<FileInfo> _playlist;

        string _currentFileFullName;

        Timer _timer;

        void Send(string mciCommand)
        {
            mciSendString(mciCommand, null, 0, IntPtr.Zero);
        }

        int GetMediaLength()
        {
            StringBuilder returnData = new StringBuilder(128);
            var Pcommand = "status " + _currentFileFullName + " length";
            var error = mciSendString(Pcommand, returnData, returnData.Capacity, IntPtr.Zero);
            return int.Parse(returnData.ToString());
        }

        #endregion


        /// <summary>
        /// Initializes player with current dir files as the playlist
        /// </summary>
        /// <param name="Playlist">Playlist for the player</param>
        public Player(IEnumerable<FileInfo> Playlist)
        {
            Console.WriteLine("Inside ctor");
            _playlist = Playlist;
        }


        public string Play()
        {
            if (CurrentFile == null) CurrentFile = _playlist.FirstOrDefault();

            if (CurrentFile != null)
            {
                Send("open " + _currentFileFullName);

                var length = GetMediaLength();

                if (_timer == null)
                    _timer = new Timer(length);
                else
                    _timer.Interval = length;

                _timer.Elapsed += _timer_Elapsed;
                _timer.Enabled = true;
                _timer.Start();

                Send("play " + _currentFileFullName);

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
            if (CurrentFile != null)
            {
                Send("pause " + _currentFileFullName);
                return "Paused " + CurrentFile.Name;
            }

            return "No file to pause";
        }


        public string Resume()
        {
            if (CurrentFile != null)
            {
                Send("resume " + _currentFileFullName);
                return "Resume " + CurrentFile.Name;
            }

            return "No file to resume";
        }


        public string Stop()
        {
            if (CurrentFile != null)
            {
                Send("close " + _currentFileFullName);
                var stoppedFileName = CurrentFile.Name.ToString();
                CurrentFile = null;
                return "Stopped " + stoppedFileName;
            }

            return "No file to stop";
        }


        public string Next()
        {
            if (CurrentFile != null)
            {
                var nextFile = _playlist
                        .SkipWhile(f => f.FullName != CurrentFile.FullName)
                        .Skip(1)
                        .Take(1)
                        .SingleOrDefault();

                if (nextFile == null)
                {
                    return "This is the last file";
                }
                else
                {
                    Stop();
                    CurrentFile = nextFile;
                    return Play(); 
                }
            }

            return "Start playing first";
        }


        public string Previous()
        {
            if (CurrentFile != null)
            {
                var previousFile = _playlist
                                    .Reverse()
                                    .SkipWhile(f => f.FullName != CurrentFile.FullName)
                                    .Skip(1)
                                    .Take(1)
                                    .SingleOrDefault();

                if (previousFile == null)
                {
                    return "This is the first file";
                }
                else
                {
                    Stop();
                    CurrentFile = previousFile;
                    return Play();
                }
            }

            return "Start playing first";
        }
    }
}
