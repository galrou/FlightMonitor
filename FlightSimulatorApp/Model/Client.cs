using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace FlightSimulatorApp.Model
{
    /// <summary></summary>
    public class Client
    {
        private volatile TcpClient client;
        private StreamWriter sw;
        private StreamReader sr;
        private NetworkStream ns;

        /// <summary>Connects the specified ip.</summary>
        /// <param name="ip">The ip.</param>
        /// <param name="port">The port.</param>
        /// <returns></returns>
        public async Task<bool> Connect(string ip, int port)
        {
            try
            {
                client = new TcpClient();
                var clientTask = client.ConnectAsync(ip, port);

                var delayTask = Task.Delay(2000);

                var CompletedTask = await Task.WhenAny(new[] { clientTask, delayTask });
                this.ns = client.GetStream();
                this.sw = new StreamWriter(ns);
                this.sr = new StreamReader(ns);

                // Timeout set o 5 seconds
                this.ns.ReadTimeout = 5000;
                return CompletedTask == clientTask;
            }

            catch (Exception)
            {
                (Application.Current as App).Model.Errormsg +=
                    "Could not connect to server check if the ip or port are correct\n";
                return false;
            }
        }

        /// <summary>Isconnecteds this instance.</summary>
        /// <returns></returns>
        public Boolean Isconnected()
        {
            if (this.client != null)
            {
                return client.Connected;
            }
            return false;

        }

        //writes command to the server
        /// <summary>Writes the specified command.</summary>
        /// <param name="command">The command.</param>
        public void Write(String command)
        {
            try
            {
                if (client != null)
                {
                    sw.AutoFlush = true;
                    sw.WriteLine(command);
                }
            }

            catch
            {
                (Application.Current as App).Model.Errormsg +=
                    "Could not write to server check if the ip or port are correct\n";
            }
        }

        /// <summary>Reads the linee.</summary>
        /// <returns></returns>
        /// <exception cref="Exception">Unable to read
        /// or
        /// client is null</exception>
        public String ReadLinee()
        {
            if (client != null)
            {
                try
                {
                    string line = sr.ReadLine();
                    return line;
                }

                catch (TimeoutException)
                {
                    if (this.client.Connected)
                    {
                        (Application.Current as App).Model.Errormsg +=
                            "The server is slow!please wait for ALL the dashboard values to arrive!\n";
                        return "slowloris";
                    }

                    else
                    {
                        (Application.Current as App).Model.Errormsg +=
                            "Server is disconnected!- please press disconnect button!!\n";
                        (Application.Current as App).Model.Disconnect();
                        return "slowloris";
                    }
                }

                catch (IOException)
                {
                    // Timeout
                    if (this.client.Connected)
                    {
                        (Application.Current as App).Model.Errormsg +=
                            "The server is slow!please wait for ALL the dashboard values to arrive!\n";
                        return "slowloris";
                    }

                    else
                    {
                        (Application.Current as App).Model.Errormsg +=
                            "Server is disconnected!- please press disconnect button!!\n";
                        return "dissc";
                    }
                }
                catch (Exception)
                {
                    (Application.Current as App).Model.Errormsg +=
                        "Unable to read values the server is slow! please wait\n";
                    return "slowloris";
                }
            }
            else
            {
                return "slowloris";
            }
        }

        /// <summary>Disconnects this instance.</summary>
        public void Disconnect()
        {
            if (client != null)
            {
                client.Close();
            }
        }
    }
}