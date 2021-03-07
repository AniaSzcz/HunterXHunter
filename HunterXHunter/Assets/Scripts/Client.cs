using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class Client : MonoBehaviour
{
    void Start()
    {
        Thread thread = new Thread(new ThreadStart(GiveDataMethod));
        thread.Start();
    }
    private void GiveDataMethod()
    {
        TcpClient client = new TcpClient("127.0.0.1", 1115);
        NetworkStream stream = client.GetStream();
        
        byte[] data = System.Text.Encoding.ASCII.GetBytes("Hello??? Please");
        
        stream.Write(data, 0, data.Length);
    }
}
