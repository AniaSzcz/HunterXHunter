using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class Server
{
    static int port = 1115;
    // Start is called before the first frame update
    static void Main(string[] args)
    {
        Console.WriteLine("Starting server...");
        TcpListener server = new TcpListener(IPAddress.Any, port);
        server.Start();
        while (true)
        {
            TcpClient client = server.AcceptTcpClient();
            if (client == null) { continue; }
            Console.WriteLine("You are connected");

            NetworkStream networkStream = client.GetStream();
            StreamWriter streamWriter = new StreamWriter(networkStream);

            byte[] receivedData = new byte[2000];

            while (true)
            {
                int streamRead = networkStream.Read(receivedData, 0, receivedData.Length);
                if (streamRead == 0) { continue; }
                string data = System.Text.Encoding.ASCII.GetString(receivedData, 0, streamRead);

                Console.WriteLine("The data:" + data);
            }
        }
    }
}