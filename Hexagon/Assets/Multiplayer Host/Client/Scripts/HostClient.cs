using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Timers;
using UnityEngine;

public class HostClient
{
    static byte[] bufferRec = new byte[1024];
    public Socket _clientSocket = new Socket
        (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    public List<byte[]> commands = new List<byte[]>();
    System.Timers.Timer connectionChecker;
    string ip;
    int port;
    public UdpClient serverUdp;
    public UdpClient sender;

    void udpReceive(IAsyncResult ar)
    {
        UdpClient socket = ar.AsyncState as UdpClient;
        IPEndPoint source = new IPEndPoint(0, 0);
        byte[] message = socket.EndReceive(ar, ref source);
        socket.BeginReceive(new AsyncCallback(udpReceive), socket);
        commands.Add(message);
    }

    public HostClient(string ip, int port)
    {
        this.ip = ip;
        this.port = port;
        try
        {
            _clientSocket.Connect(ip, port);
            _clientSocket.NoDelay = true;
            _clientSocket.BeginReceive(bufferRec, 0, bufferRec.Length, SocketFlags.None, new AsyncCallback(ReceiveResponse), _clientSocket);
            connectionChecker = new System.Timers.Timer();
            connectionChecker.Elapsed += new ElapsedEventHandler(checkConnection);
            connectionChecker.Interval = 1000;
            connectionChecker.Start();
            serverUdp = new UdpClient(port+1);
            serverUdp.BeginReceive(udpReceive,serverUdp);
            sender = new UdpClient();
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
            byte[] thiscommand = new byte[2];
            thiscommand[0] = 0xFF;
            thiscommand[1] = 0xFF;
            commands.Add(thiscommand);
            disconnect();
        }
        if (!_clientSocket.Connected)
        {
            byte[] thiscommand = new byte[2];
            thiscommand[0] = 0xFF;
            thiscommand[1] = 0xFF;
            commands.Add(thiscommand);
        }
    }

    public void SendInfo(byte[] header, byte[] message)
    {

        byte[] buffer = new byte[header.Length + message.Length];
        System.Array.Copy(header, buffer, header.Length);
        System.Array.Copy(message, 0, buffer, header.Length, message.Length);
        _clientSocket.Send(buffer, 0, buffer.Length, SocketFlags.None);
    }

    public void SendUdpInfo(byte[] header, byte[] message)
    {
        byte[] buffer = new byte[header.Length + message.Length];
        System.Array.Copy(header, buffer, header.Length);
        System.Array.Copy(message, 0, buffer, header.Length, message.Length);
        sender.Send(buffer, buffer.Length, ip, port + 2);
    }

    private void ReceiveResponse(IAsyncResult AR)
    {
        Socket socket = (Socket)AR.AsyncState;
        int received = socket.EndReceive(AR);
        byte[] dataBuf = new byte[received];
        Array.Copy(bufferRec, dataBuf, received);
        splitCommands(dataBuf);
        try
        {
            _clientSocket.BeginReceive(bufferRec, 0, bufferRec.Length, SocketFlags.None, new AsyncCallback(ReceiveResponse), _clientSocket);
        }
        catch (SocketException)
        {

        }
    }

    public void splitCommands(byte[] data)
    {
        int index = 0;
        byte[] header = new byte[2];
        byte[] command;
        while (index < data.Length)
        {
            header[0] = data[index];
            header[1] = data[index + 1];
            command = new byte[PacketSize.getPacketLenght(header) + 2];
            Array.Copy(data, index, command, 0, PacketSize.getPacketLenght(header) + 2);
            index = index + 2 + PacketSize.getPacketLenght(header);
            executeCommand(command);
        }
    }

    private void executeCommand(byte[] received)
    {
        byte[] header = new byte[2];
        header[0] = received[0];
        header[1] = received[1];
        byte[] message = new byte[received.Length - 2];
        //All if with a end return;
        Array.Copy(received, 2, message, 0, received.Length - 2);
        commands.Add(received);
    }

    private void checkConnection(object source, ElapsedEventArgs e)
    {
        if ((_clientSocket.Poll(1, SelectMode.SelectRead) && _clientSocket.Available == 0))
        {
            _clientSocket.Shutdown(SocketShutdown.Both);
            _clientSocket.Close();
            byte[] thiscommand = new byte[2];
            thiscommand[0] = 0xFF;
            thiscommand[1] = 0xFF;
            serverUdp.Close();
            commands.Add(thiscommand);
            connectionChecker.Enabled = false;
        }
    }

    public byte[] getCommand()
    {
        byte[] command = commands[0];
        commands.RemoveAt(0);
        return command;
    }

    public void disconnect()
    {
        _clientSocket.Shutdown(SocketShutdown.Both);
        _clientSocket.Close();
        serverUdp.Close();
        connectionChecker.Stop();
        connectionChecker.Dispose();
    }

}

