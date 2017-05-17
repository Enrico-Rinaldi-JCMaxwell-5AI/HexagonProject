using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class ClientSideData
{
    public string username;
    public bool lobbyReady;
    public int balls;
    public bool isAlive=true;
    public float shieldtime;

    public ClientSideData()
    {
        lobbyReady = false;
        username = "";
        shieldtime = 0;
    }
}

