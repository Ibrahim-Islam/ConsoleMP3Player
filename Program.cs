using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using static System.Console;

namespace ConsoleMP3Player
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
                player.StatusChanged += Player_StatusChange;

                while (true)
                {
                    var read = ReadLine().ToLower();

                    switch (read)
                    {
                        case "exit":
                            return;

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

                        case "resume":
                            player.Resume();
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