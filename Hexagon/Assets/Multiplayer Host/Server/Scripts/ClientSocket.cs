using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;


public class ClientSocket
{
    public Socket clientSocket;
    public ClientData clientData;

    public ClientSocket(Socket clientSocket)
    {
        this.clientSocket = clientSocket;
    }

    public string getIp()
    {
        IPEndPoint remoteIpEndPoint = clientSocket.RemoteEndPoint as IPEndPoint;
        return remoteIpEndPoint.Address.ToString();
    }

    public int getUdpPort()
    {
        IPEndPoint remoteIpEndPoint = clientSocket.RemoteEndPoint as IPEndPoint;
        return remoteIpEndPoint.Port + 1;
    }
}

