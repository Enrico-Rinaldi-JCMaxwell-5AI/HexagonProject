using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class ClientData
{
    public string username;
    public bool lobbyReady;
    public int balls;
    public bool isAlive=true;
    public float shieldCoolDown = 0;
    public float repulsionCoolDown = 0;

    public ClientData(string username)
    {
        this.username = username;
        lobbyReady = false;
    }

    public void resetData()
    {
        balls = 60;
        isAlive = true;
        shieldCoolDown = 0;
        repulsionCoolDown = 0;
    }

    public void reduceCooldown(float time)
    {
        shieldCoolDown = shieldCoolDown - time;
        repulsionCoolDown = repulsionCoolDown - time;
    }
}

