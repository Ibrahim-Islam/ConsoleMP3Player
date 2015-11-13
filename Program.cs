using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using static System.Console;

namespace ConsoleMP3Player
{
    class Program
    {
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
                var player = new Player(currentDirMp3s);
                player.StatusChanged += Player_StatusChange;

                WriteLine(string.Format("Available commands are play, pause, stop, next and previous"));

                while (true)
                {
                    switch (ReadLine().ToLower())
                    {
                        case "play":
                            player.Play();
                            break;

                        case "stop":
                            player.Stop();
                            break;

                        case "pause":
                            player.Pause();
                            break;

                        case "next":
                            player.Next();
                            break;

                        case "previous":
                            player.Previous();
                            break;

                        default:
                            WriteLine("Command not recognized");
                            break;
                    }                    
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private static void Player_StatusChange(string status)
        {
            WriteLine(status);
        }
    }
}