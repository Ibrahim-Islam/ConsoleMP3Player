using static System.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;

namespace CmdPlayer
{
    class Program
    {
        static Player player;

        public static IEnumerable<FileInfo> currentDirMp3s
        {
            get {
                return Directory.GetFiles(Environment.CurrentDirectory)
                                        .Select(s => new FileInfo(s))
                                        .Where(s => s.Extension.Equals(".mp3", StringComparison.OrdinalIgnoreCase));
            }
        }

        static void Main(string[] args)
        {
            try
            {
                player = new Player(currentDirMp3s);
                player.PlaybackComplete += Player_PlaybackComplete;

                while (true)
                {
                    var read = ReadLine().ToLower();

                    switch (read)
                    {
                        case "exit":
                            return;

                        case "play":
                            WriteLine(player.Play());
                        break;

                        case "stop":
                            WriteLine(player.Stop());
                        break;

                        case "pause":
                            WriteLine(player.Pause());
                        break;

                        case "next":
                            WriteLine(player.Next());
                        break;

                        case "previous":
                            WriteLine(player.Previous());
                        break;

                        case "resume":
                            WriteLine(player.Resume());
                        break;
                    }
                    
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private static void Player_PlaybackComplete(object sender, EventArgs e)
        {
            //WriteLine(player.Next());
        }
    }
}