using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour {

    public HostServer server;
    public ServerGameController controller;
    public ServerGameModel model;
    public ServerGameView view;
    public float nextGameTimeUpdate;

    // Use this for initialization
    void Start () {
        controller = GetComponent<ServerGameController>();
        model = GetComponent<ServerGameModel>();
        view = GetComponent<ServerGameView>();
    }

    // Update is called once per frame
    void Update () {
        if(nextGameTimeUpdate<=model.gameTime)
        {
            for(int i=0;i<controller.allInGameObjects.Count;i++)
            {
                try
                {
                    if (controller.allInGameObjects[i].GetComponent<ServerBallScript>().ground)     //SIAMO NEL CASO IN CUI QUESTA E' LA PALLINA A TERRA
                    {
                        byte[] header = new byte[2];
                        header[0] = 0x00;
                        header[1] = 0x18;
                        byte[] message = new byte[9];
                        message[0] = (byte)System.Int32.Parse(controller.allInGameObjects[i].name);
                        System.Array.Copy(System.BitConverter.GetBytes(controller.allInGameObjects[i].transform.position.x), 0, message, 1, 4);
                        System.Array.Copy(System.BitConverter.GetBytes(controller.allInGameObjects[i].transform.position.z), 0, message, 5, 4);
                        server.broadCastData(header, message, true);
                    }else
                    {
                        byte[] header = new byte[2];
                        header[0] = 0x00;
                        header[1] = 0x09;
                        byte[] message = new byte[13];
                        message[0] = (byte)System.Int32.Parse(controller.allInGameObjects[i].name);
                        System.Array.Copy(System.BitConverter.GetBytes(controller.allInGameObjects[i].transform.position.x), 0, message, 1, 4);
                        System.Array.Copy(System.BitConverter.GetBytes(controller.allInGameObjects[i].transform.position.y), 0, message, 5, 4);
                        System.Array.Copy(System.BitConverter.GetBytes(controller.allInGameObjects[i].transform.position.z), 0, message, 9, 4);
                        server.broadCastData(header, message, true);
                    }
                }
                catch (System.Exception) {
                    if(System.Int32.Parse(controller.allInGameObjects[i].name)>3)
                    {
                        byte[] header = new byte[2];
                        header[0] = 0x00;
                        header[1] = 0x09;
                        byte[] message = new byte[13];
                        message[0] = (byte)System.Int32.Parse(controller.allInGameObjects[i].name);
                        System.Array.Copy(System.BitConverter.GetBytes(controller.allInGameObjects[i].transform.position.x), 0, message, 1, 4);
                        System.Array.Copy(System.BitConverter.GetBytes(controller.allInGameObjects[i].transform.position.y), 0, message, 5, 4);
                        System.Array.Copy(System.BitConverter.GetBytes(controller.allInGameObjects[i].transform.position.z), 0, message, 9, 4);
                        server.broadCastData(header, message, true);
                    }
                }
            }
            nextGameTimeUpdate = nextGameTimeUpdate + (1f / model.updatesPerSecond);
        }
        if(server!=null)
        readCommand();
	}

    private void FixedUpdate()
    {

    }

    private void OnGUI()
    {
        if(server!=null)
        GUI.Label(new Rect(0, 0, 100, 20), server.byteUploaded.ToString());
    }

    public void readCommand()
    {
        for (int i = 0; i < server.commands.Count; i++)
        {
            byte[] command = server.getCommand();
            if(command[0] == 0xFF && command[1] == 0xFF && model.isGameStarted)
            {
                controller.changePortState(command[2]);
                for(int j=0;j< controller.allInGameObjects.Count;j++)
                {
                    if(controller.allInGameObjects[j].name.Equals(((int)command[2]).ToString()))
                    {
                        controller.allInGameObjects.RemoveAt(j);
                        view.navs[command[2]] = null;
                    }
                }
            }
            if (command[0] == 0x00 && command[1] == 0x0c)
            {
                controller.moveNav(System.BitConverter.ToSingle(command, 2), command[6]);
            }
            if (command[0] == 0x00 && command[1] == 0x15)
            {
                controller.useAbility(command[3]+1, command[2]);
            }
        }
    }

    public bool canMatchStart()
    {
        int player = 1;
        if(server!=null)
        {
            for(int i=0;i<3;i++)
            {
                if(server.clientSocketList.Exist(i) && server.clientSocketList.Get(i).clientData != null)
                {
                    if (server.clientSocketList.Get(i).clientData.lobbyReady)
                        player++;
                    else
                        return false;
                }
            }
        }
        if (player >= 2)
            return true;
        else
            return false;
    }

    public void sendInstantiate(int objectName, int objectIdentifier, Vector3 position)
    {
        byte[] header = new byte[2];
        header[0] = 0x00;
        header[1] = 0x08;
        byte[] message = new byte[14];
        message[0] = (byte)objectName;
        message[1] = (byte)objectIdentifier;
        System.Array.Copy(System.BitConverter.GetBytes(position.x), 0, message, 2, 4);
        System.Array.Copy(System.BitConverter.GetBytes(position.y), 0, message, 6, 4);
        System.Array.Copy(System.BitConverter.GetBytes(position.z), 0, message, 10, 4);
        server.broadCastData(header, message,false);
    }

    public void sendStartGame()
    {
        byte[] start = new byte[2];
        start[0] = 0x00;
        start[1] = 0x07;
        byte[] message = new byte[1];
        for (int i = 0; i < server.clientSocketList.Count(); i++)
        {
            message[0] = (byte)(i+1);
            server.sendData(i, start, message, false);
        }
        nextGameTimeUpdate = 1f / model.updatesPerSecond;
    }

    public void sendFinishGame(int winner)
    {
        byte[] start = new byte[2];
        start[0] = 0x00;
        start[1] = 0x0a;
        byte[] message = new byte[1];
        message[0] = (byte)winner;
        server.broadCastData(start, message,false);
        
    }

    public void sendDeadPort(int portdead)
    {
        byte[] start = new byte[2];
        start[0] = 0x00;
        start[1] = 0x0b;
        byte[] message = new byte[1];
        message[0] = (byte)portdead;
        server.broadCastData(start, message,false);
    }

    public void sendCentralCannonUp()
    {
        byte[] start = new byte[2];
        start[0] = 0x00;
        start[1] = 0x0e;
        server.broadCastData(start, new byte[0], false);
    }
    public void sendCentralCannonDown()
    {
        byte[] start = new byte[2];
        start[0] = 0x00;
        start[1] = 0x0f;
        server.broadCastData(start, new byte[0], false);
    }

    public void sendCentralCannonPos(float rot,float height)
    {
        byte[] start = new byte[2];
        start[0] = 0x00;
        start[1] = 0x0d;
        byte[] message = new byte[8];
        System.Array.Copy(System.BitConverter.GetBytes(rot), 0, message, 0, 4);
        System.Array.Copy(System.BitConverter.GetBytes(height), 0, message, 4, 4);
        server.broadCastData(start, message, false);
    }

    public void sendDestroyObject(int id)
    {
        byte[] start = new byte[2];
        start[0] = 0x00;
        start[1] = 0x10;
        byte[] message = new byte[1];
        message[0] = (byte)id;
        server.broadCastData(start, message, false);
    }

    public void sendCentralCannonLeds(int id)
    {
        byte[] start = new byte[2];
        start[0] = 0x00;
        start[1] = 0x11;
        byte[] message = new byte[1];
        message[0] = (byte)id;
        server.broadCastData(start, message, false);
    }

    public void sendCentralBallRest(bool on)
    {
        byte[] start = new byte[2];
        start[0] = 0x00;
        start[1] = 0x12;
        byte[] message = new byte[1];
        if (on)
            message[0] = 1;           
        else
            message[0] = 0;
        server.broadCastData(start, message, false);
    }

    public void sendRefreshPortBalls(int id,int balls)
    {
        byte[] start = new byte[2];
        start[0] = 0x00;
        start[1] = 0x13;
        byte[] message = new byte[2];
        message[0] = (byte)id;
        message[1] = (byte)balls;
        server.broadCastData(start, message, false);
    }

    public void sendUsedAbility(int port,int ability)
    {
        byte[] start = new byte[2];
        start[0] = 0x00;
        start[1] = 0x14;
        byte[] message = new byte[2];
        message[0] = (byte)port;
        message[1] = (byte)ability;
        server.broadCastData(start, message, false);
    }

    public void sendRestart()
    {
        byte[] start = new byte[2];
        start[0] = 0x00;
        start[1] = 0x16;
        server.broadCastData(start, new byte[0], false);
    }

    public void sendReturnToLobby()
    {
        byte[] start = new byte[2];
        start[0] = 0x00;
        start[1] = 0x17;
        server.broadCastData(start, new byte[0], false);
    }

    public void sendUpdateNav(int port,float pos)
    {
        byte[] start = new byte[2];
        start[0] = 0x00;
        start[1] = 0x19;
        byte[] message = new byte[5];
        message[0] = (byte)port;
        System.Array.Copy(System.BitConverter.GetBytes(pos), 0, message, 1, 4);
        server.broadCastData(start, message, true);
    }

}
