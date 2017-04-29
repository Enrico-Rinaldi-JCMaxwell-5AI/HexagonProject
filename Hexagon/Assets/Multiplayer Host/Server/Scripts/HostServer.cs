using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Timers;
using System.Threading;

public class HostServer
{
    private ServerGameModel model;
    private Socket serverSocket;
    public HostPortArray clientSocketList = new HostPortArray();
    private readonly byte[] _buffer = new byte[2048];
    public List<byte[]> commands = new List<byte[]>();
    System.Timers.Timer connectionChecker;
    public ClientData ownerData;
    UdpClient udpSocket;
    UdpClient receiver;
    public int port;
    public long byteUploaded=0;

    void udpReceive(IAsyncResult ar)
    {
        UdpClient socket = ar.AsyncState as UdpClient;
        IPEndPoint source = new IPEndPoint(0, 0);
        byte[] message = socket.EndReceive(ar, ref source);
        socket.BeginReceive(new AsyncCallback(udpReceive), socket);
        commands.Add(message);
    }

    public HostServer(int PORT,ServerGameModel model)
    {
        this.model = model;
        serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        serverSocket.Bind(new IPEndPoint(IPAddress.Any, PORT));
        serverSocket.Listen(5);
        serverSocket.BeginAccept(AcceptCallback, null);
        serverSocket.NoDelay = true;
        connectionChecker = new System.Timers.Timer();
        connectionChecker.Elapsed += new ElapsedEventHandler(checkConnections);
        connectionChecker.Interval = 1000;
        connectionChecker.Enabled = true;
        udpSocket = new UdpClient();
        port = PORT+1;
        receiver = new UdpClient(PORT+2);
        receiver.BeginReceive(udpReceive, receiver);
    }

    private void AcceptCallback(IAsyncResult AR)
    {
        Socket socket;
        try
        {
            if(model.isGameStarted)
            {
                socket = serverSocket.EndAccept(AR);
                byte[] dataToSend = new byte[2];
                dataToSend[0] = 0xFF;
                dataToSend[1] = 0xFE;
                socket.Send(dataToSend, 0, dataToSend.Length, SocketFlags.None);
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
                serverSocket.BeginAccept(AcceptCallback, null);
                return;
            }
            if (clientSocketList.isFull())
            {
                for (int i = 0; i < 3; i++)
                {
                    if (clientSocketList.Get(i).clientData == null)
                    {
                        kickClient(i);
                        socket = serverSocket.EndAccept(AR);
                        clientSocketList.Add(new ClientSocket(socket));
                        socket.BeginReceive(_buffer, 0, 2048, SocketFlags.None, ReceiveCallback, socket);
                        socket.NoDelay=true;
                        serverSocket.BeginAccept(AcceptCallback, null);
                        return;
                    }
                }
                socket = serverSocket.EndAccept(AR);
                byte[] dataToSend = new byte[2];
                dataToSend[0] = 0xFF;
                dataToSend[1] = 0xFD;
                socket.Send(dataToSend, 0, dataToSend.Length, SocketFlags.None);
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
                serverSocket.BeginAccept(AcceptCallback, null);
            }
            else
            {
                socket = serverSocket.EndAccept(AR);
                clientSocketList.Add(new ClientSocket(socket));
                socket.BeginReceive(_buffer, 0, 2048, SocketFlags.None, ReceiveCallback, socket);
                serverSocket.BeginAccept(AcceptCallback, null);
            }
        }
        catch (ObjectDisposedException)
        {
            return;
        }
    }
        

    private void ReceiveCallback(IAsyncResult AR)
    {
        Socket current = (Socket)AR.AsyncState;
        int id = 0;
        for(int i = 0; i < 3; i++)
        {
            if(clientSocketList.Exist(i) && current == clientSocketList.Get(i).clientSocket)
            {
                id = i;
            }
        }
        int received;
        try
        {
            received = current.EndReceive(AR);
        }
        catch (SocketException)
        {
            kickClient(current);
            return;
        }
        byte[] recBuf = new byte[received];
        Array.Copy(_buffer, recBuf, received);
        splitCommands(recBuf,id);
        current.BeginReceive(_buffer, 0, 2048, SocketFlags.None, ReceiveCallback, current);
    }

    private void splitCommands(byte[] data,int id)
    {
        int index = 0;
        byte[] header = new byte[2];
        byte[] command;
        while(index < data.Length)
        {
            header[0] = data[index];
            header[1] = data[index+1];
            command = new byte[PacketSize.getPacketLenght(header)+2];
            Array.Copy(data, index, command, 0, PacketSize.getPacketLenght(header) + 2);
            index = index + 2 + PacketSize.getPacketLenght(header);
            executeCommand(command, id);
        }
    }

    public void sendData(int id, byte[] header, byte[] data,bool udp)
    {
        try
        {
            if (clientSocketList.Exist(id))
            {
                byte[] dataToSend = new byte[header.Length + data.Length];
                Array.Copy(header, dataToSend, 2);
                Array.Copy(data, 0, dataToSend, 2, data.Length);
                if (!udp)
                    clientSocketList.Get(id).clientSocket.Send(dataToSend, 0, dataToSend.Length, SocketFlags.None);
                else
                    udpSocket.Send(dataToSend, dataToSend.Length, clientSocketList.Get(id).getIp(), port);
                byteUploaded = byteUploaded + dataToSend.Length;
            }
        }catch(SocketException)
        {
            if (clientSocketList.Exist(id) && (clientSocketList.Get(id).clientSocket.Poll(1, SelectMode.SelectRead) && clientSocketList.Get(id).clientSocket.Available == 0))
            {
                kickClient(id);
            }
        }
    }

    public void broadCastData(byte[] header, byte[] data,bool udp)
    {
        for (int i = 0; i < 3; i++)
        {
            if (clientSocketList.Exist(i))
            {
                sendData(i, header, data,udp);
            }
        }
    
    }

    private void executeCommand(byte[] received, int id)
    {

        byte[] header = new byte[2];
        header[0] = received[0];
        header[1] = received[1];
        byte[] message = new byte[received.Length - 2];
        byte[] sendHeader = new byte[2];
        Array.Copy(received, 2, message, 0, received.Length - 2);
        if (header[0] == 0x00 && header[1] == 0x00)     //USERNAME RECEIVED
        {
            string username = PacketSize.decomposeString(Encoding.ASCII.GetString(message));
            for (int i=0;i<3;i++)
            {
                if(clientSocketList.Exist(i) && clientSocketList.Get(i).clientData!= null && PacketSize.decomposeString(Encoding.ASCII.GetString(message)).Equals(clientSocketList.Get(i).clientData.username) || PacketSize.decomposeString(Encoding.ASCII.GetString(message)).Equals(ownerData.username))
                {
                    sendHeader[0] = 0x00;
                    sendHeader[1] = 0x02;
                    username = PacketSize.decomposeString(Encoding.ASCII.GetString(message)) + "bis";
                    Debug.Log(username);
                    sendData(id, sendHeader, Encoding.ASCII.GetBytes(PacketSize.composeString(username)), false);
                }
            }
            sendHeader[0] = 0x00;
            sendHeader[1] = 0x01;
            sendData(id, sendHeader, new byte[0],false);
            clientSocketList.Get(id).clientData = new ClientData(username);
            sendHeader[0] = 0x00;
            sendHeader[1] = 0x03;
            //MANDO AL CLIENT APPENA CONNESSO TUTTI I DATI DEGLI ALTRI GIOCATORI
            byte[] data = new byte[17];
            data[0] = 0x00;
            Array.Copy(Encoding.ASCII.GetBytes(PacketSize.composeString(ownerData.username)), 0, data, 1, 16);
            sendData(id, sendHeader, data,false);
            for (int i = 0; i < 3; i++)
            {
                if (clientSocketList.Exist(i) && clientSocketList.Get(i).clientData != null)
                {
                    data[0] = (byte)(i + 1);
                    Array.Copy(Encoding.ASCII.GetBytes(PacketSize.composeString(clientSocketList.Get(i).clientData.username)), 0, data, 1, 16);
                    sendData(id, sendHeader, data,false);
                }
            }
            for (int i = 0; i < 3; i++)
            {   
                if (clientSocketList.Exist(i) && clientSocketList.Get(i).clientData != null && clientSocketList.Get(i).clientData.lobbyReady)
                {
                    sendHeader[0] = 0x00;
                    sendHeader[1] = 0x06;
                    byte[] sendMessage = new byte[1];
                    sendMessage[0] = (byte)(i + 1);
                    sendData(id, sendHeader, sendMessage,false);
                }
            }
            //INVIO IL NUOVO ARRIVATO AGLI ALTRI CLIENT
            sendHeader[0] = 0x00;
            sendHeader[1] = 0x03;
            for (int i = 0; i < 3; i++)
            {
                if (id != i)
                {
                    data[0] = (byte)(id + 1);
                    Array.Copy(message, 0, data, 1, 16);
                    sendData(i, sendHeader, data,false);
                }
            }
            return;
        }
        if(header[0] == 0x00 && header[1] == 0x05)
        {
            clientSocketList.Get(id).clientData.lobbyReady = !clientSocketList.Get(id).clientData.lobbyReady;
            for(int i = 0; i < 3; i++)
            {
                sendHeader[0] = 0x00;
                sendHeader[1] = 0x06;
                byte[] sendMessage = new byte[1];
                sendMessage[0] = (byte)(id+1);
                sendData(i, sendHeader, sendMessage,false);
            }
            return;
        }
        byte[] uppermessage = new byte[received.Length + 1];
        Array.Copy(received, 0, uppermessage, 0, received.Length);
        uppermessage[received.Length] = (byte)id;
        commands.Add(uppermessage);  
    }

    public byte[] getCommand()
    {
        byte[] command = commands[0];
        commands.RemoveAt(0);
        return command;
    }

    public string[] getAllIpAddress(int n)
    {
        IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
        string[] returnVector = new string[host.AddressList.Length];
        for(int i=0;i<host.AddressList.Length;i++)
        {
            returnVector[i] = host.AddressList[i].ToString();
        }
        return returnVector;
    }

    public int getIdFromUsername(string username)
    {
        for(int i=0;i<3;i++)
        {
            if (clientSocketList.Exist(i) && clientSocketList.Get(i).clientData != null && clientSocketList.Get(i).clientData.username.Equals(username))
                return i;
        }
        return -1;
    }

    public void kickClient(int n)
    {
        if (clientSocketList.Exist(n))
        {
            try
            {
                clientSocketList.Get(n).clientSocket.Shutdown(SocketShutdown.Both);
                clientSocketList.Get(n).clientSocket.Close();
            }
            catch (SocketException)
            {

            }
            clientSocketList.RemoveAt(n, !model.isGameStarted);
            byte[] sendHeader = new byte[2];
            sendHeader[0] = 0x00;
            sendHeader[1] = 0x04;
            byte[] message = new byte[1];
            message[0] = (byte)(n + 1);
            for (int i = 0; i < 3; i++)
            {
                sendData(i, sendHeader, message,false);
            }
            message = new byte[3];
            message[0] = 0xFF;
            message[1] = 0xFF;
            message[2] = (byte)(n + 1);
            commands.Add(message);
        }
    }

    public void kickClient(Socket socket)
    {
        for(int i=0;i<3;i++)
        {
            if(clientSocketList.Exist(i) && clientSocketList.Get(i).clientSocket == socket)
            {
                clientSocketList.Get(i).clientSocket.Shutdown(SocketShutdown.Both);
                clientSocketList.Get(i).clientSocket.Close();
                clientSocketList.RemoveAt(i,!model.isGameStarted);
                byte[] sendHeader = new byte[2];
                sendHeader[0] = 0x00;
                sendHeader[1] = 0x04;
                byte[] message = new byte[1];
                message[0] = (byte)(i + 1);
                for (int j = 0; j < 3; j++)
                {
                    sendData(j, sendHeader, message,false);
                }
                message = new byte[3];
                message[0] = 0xFF;
                message[1] = 0xFF;
                message[2] = (byte)(i + 1);
                commands.Add(message);
                return;
            }
        }
    }

    public string getClientInfo(int n)
    {
        IPEndPoint remoteIpEndPoint = clientSocketList.Get(n).clientSocket.RemoteEndPoint as IPEndPoint;
        if (clientSocketList.Get(n).clientData == null)
        {
            return "IP: "+remoteIpEndPoint.Address.ToString();
        }
        else
        {
            return "IP: " + remoteIpEndPoint.Address + "\n" + clientSocketList.Get(n).clientData.username;
        }
    }

    public void shutdownServer()
    {
        for(int i=0;i<3;i++)
        {
            kickClient(0);
        }
        receiver.Close();
        serverSocket.Close();
        connectionChecker.Stop();
        connectionChecker.Dispose();
    }

    private void checkConnections(object source, ElapsedEventArgs e)
    {
        for(int i=0;i<3;i++)
        {
            if(clientSocketList.Exist(i) && (clientSocketList.Get(i).clientSocket.Poll(1, SelectMode.SelectRead) && clientSocketList.Get(i).clientSocket.Available == 0))
            {
                kickClient(i);
            }
        }
    }
}