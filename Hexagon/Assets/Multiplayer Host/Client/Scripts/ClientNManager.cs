using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class ClientNManager : MonoBehaviour {

    public HostClient client;
    HostClientController controller;
    HostClientModel model;
    HostClientView view;
    public Object Shield;
    public Object Repulsion;

    // Use this for initialization
    void Start()
    {
        controller = GetComponent<HostClientController>();
        model = GetComponent<HostClientModel>();
        view = GetComponent<HostClientView>();
    }

    // Update is called once per frame
    void FixedUpdate () {
        if(client!=null && client.commands.Count!=0)
        readCommand();
	}

    

    public void readCommand()
    {
        for (int i = 0; i < client.commands.Count; i++)
        {
            byte[] command = client.getCommand();
            if (command[0] == 0xFF && command[1] == 0xFF)
            {
                forcedDisconnection();
                controller.resetMap();
                break;
            }
            if (command[0] == 0xFF && command[1] == 0xFE)
            {
                forcedDisconnection();
                controller.resetMap();
                GameObject.Find("Main Camera").GetComponent<MenuGUI>().message = true;
                GameObject.Find("Main Camera").GetComponent<MenuGUI>().messageText = "You can't join, the game is already started!";
                break;
            }
            if (command[0] == 0xFF && command[1] == 0xFD)
            {
                forcedDisconnection();
                controller.resetMap();
                GameObject.Find("Main Camera").GetComponent<MenuGUI>().message = true;
                GameObject.Find("Main Camera").GetComponent<MenuGUI>().messageText = "You can't join because the lobby is full!";
                break;
            }
            if (command[0] == 0x00 && command[1] == 0x01)
            {
                model.usernameAk = true;
            }
            if (command[0] == 0x00 && command[1] == 0x02)
            {
                byte[] username = new byte[16];
                Debug.Log(command.Length);
                System.Array.Copy(command, 2, username, 0, 16);
                model.username = PacketSize.decomposeString(Encoding.ASCII.GetString(username));
                model.usernameAk = true;
            }
            if (command[0] == 0x00 && command[1] == 0x03)
            {
                byte[] username = new byte[16];
                System.Array.Copy(command, 3, username, 0, 16);
                model.clientData[command[2]] = new ClientSideData();
                model.clientData[command[2]].username = PacketSize.decomposeString(Encoding.ASCII.GetString(username));
            }
            if (command[0] == 0x00 && command[1] == 0x04)
            {
                Debug.Log("User disconnect");
                if (!model.isGameStarted)
                    model.shiftClientDataAt(command[2]);
                else
                {
                    model.clientData[command[2]] = null;
                    controller.changePortState(command[2]);
                }
            }
            if (command[0] == 0x00 && command[1] == 0x06)
            {
                model.clientData[command[2]].lobbyReady = !model.clientData[command[2]].lobbyReady;
            }
            if (command[0] == 0x00 && command[1] == 0x07)
            {
                model.isGameStarted = true;
                controller.startGame();
            }
            if (command[0] == 0x00 && command[1] == 0x08)
            {
                GameObject instantiated;
                float x = System.BitConverter.ToSingle(command, 4);
                float y = System.BitConverter.ToSingle(command, 8);
                float z = System.BitConverter.ToSingle(command, 12);
                instantiated = (GameObject)Instantiate(model.objectPool[command[3]], new Vector3(x, y, z), Quaternion.identity);
                instantiated.name = ((int)command[2]).ToString();
                controller.allInGameObject.Add(instantiated);
            }
            if (command[0] == 0x00 && command[1] == 0x09)
            {
                float x = System.BitConverter.ToSingle(command, 3);
                float y = System.BitConverter.ToSingle(command, 7);
                float z = System.BitConverter.ToSingle(command, 11);
                for (int j = 0; j < controller.allInGameObject.Count; j++)
                {
                    if (controller.allInGameObject[j].name.Equals(((int)command[2]).ToString()))
                    {
                        controller.allInGameObject[j].GetComponent<Transform>().position = new Vector3(x, y, z);
                    }
                }
            }
            if (command[0] == 0x00 && command[1] == 0x0a)
            {
                controller.gameFinished(command[2]);
            }
            if (command[0] == 0x00 && command[1] == 0x0b)
            {
                controller.portDead(command[2]);
            }
            if (command[0] == 0x00 && command[1] == 0x0d)
            {
                GameObject turret = GameObject.Find("Turret");
                turret.GetComponent<Transform>().rotation = Quaternion.Euler(0, System.BitConverter.ToSingle(command, 2), 0);
                turret.GetComponent<Transform>().position = new Vector3(0, System.BitConverter.ToSingle(command, 6), 0);
            }
            if (command[0] == 0x00 && command[1] == 0x0e)
            {
                GameObject.Find("Circle").transform.GetChild(0).GetComponent<Renderer>().material = controller.LedBlue;
            }
            if (command[0] == 0x00 && command[1] == 0x0f)
            {
                GameObject.Find("Circle").transform.GetChild(0).GetComponent<Renderer>().material = controller.LedOff;
                GameObject.Find("SO Down Led").transform.GetChild(0).GetComponent<Renderer>().material = controller.LedOff;
                GameObject.Find("NO Down Led").transform.GetChild(0).GetComponent<Renderer>().material = controller.LedOff;
                GameObject.Find("NE Down Led").transform.GetChild(0).GetComponent<Renderer>().material = controller.LedOff;
                GameObject.Find("SE Down Led").transform.GetChild(0).GetComponent<Renderer>().material = controller.LedOff;
                GameObject.Find("Turret").GetComponent<Transform>().position = new Vector3(0, -1, 0);
            }
            if (command[0] == 0x00 && command[1] == 0x10)
            {
                GameObject obj = GameObject.Find(command[2].ToString());
                controller.allInGameObject.Remove(obj);
                Destroy(obj);
            }
            if (command[0] == 0x00 && command[1] == 0x11)
            {
                if (command[2] == 0)
                {
                    GameObject.Find("SO Down Led").transform.GetChild(0).GetComponent<Renderer>().material = controller.LedBlue;
                    GameObject.Find("NO Down Led").transform.GetChild(0).GetComponent<Renderer>().material = controller.LedBlue;
                    GameObject.Find("SE Down Led").transform.GetChild(0).GetComponent<Renderer>().material = controller.LedOff;
                }
                if (command[2] == 1)
                {
                    GameObject.Find("NO Down Led").transform.GetChild(0).GetComponent<Renderer>().material = controller.LedBlue;
                    GameObject.Find("NE Down Led").transform.GetChild(0).GetComponent<Renderer>().material = controller.LedBlue;
                    GameObject.Find("SO Down Led").transform.GetChild(0).GetComponent<Renderer>().material = controller.LedOff;
                }
                if (command[2] == 2)
                {
                    GameObject.Find("NE Down Led").transform.GetChild(0).GetComponent<Renderer>().material = controller.LedBlue;
                    GameObject.Find("SE Down Led").transform.GetChild(0).GetComponent<Renderer>().material = controller.LedBlue;
                    GameObject.Find("NO Down Led").transform.GetChild(0).GetComponent<Renderer>().material = controller.LedOff;
                }
                if (command[2] == 3)
                {
                    GameObject.Find("SO Down Led").transform.GetChild(0).GetComponent<Renderer>().material = controller.LedBlue;
                    GameObject.Find("SE Down Led").transform.GetChild(0).GetComponent<Renderer>().material = controller.LedBlue;
                    GameObject.Find("NE Down Led").transform.GetChild(0).GetComponent<Renderer>().material = controller.LedOff;
                }
            }
            if (command[0] == 0x00 && command[1] == 0x12)
            {
                if (command[2] == 1)
                {
                    GameObject.Find("Circle").transform.GetChild(0).GetComponent<Renderer>().material = controller.LedRed;
                    GameObject.Find("SO Down Led").transform.GetChild(0).GetComponent<Renderer>().material = controller.LedRed;
                    GameObject.Find("NO Down Led").transform.GetChild(0).GetComponent<Renderer>().material = controller.LedRed;
                    GameObject.Find("NE Down Led").transform.GetChild(0).GetComponent<Renderer>().material = controller.LedRed;
                    GameObject.Find("SE Down Led").transform.GetChild(0).GetComponent<Renderer>().material = controller.LedRed;
                }
                else
                {
                    GameObject.Find("Circle").transform.GetChild(0).GetComponent<Renderer>().material = controller.LedOff;
                    GameObject.Find("SO Down Led").transform.GetChild(0).GetComponent<Renderer>().material = controller.LedOff;
                    GameObject.Find("NO Down Led").transform.GetChild(0).GetComponent<Renderer>().material = controller.LedOff;
                    GameObject.Find("NE Down Led").transform.GetChild(0).GetComponent<Renderer>().material = controller.LedOff;
                    GameObject.Find("SE Down Led").transform.GetChild(0).GetComponent<Renderer>().material = controller.LedOff;
                }
            }
            if (command[0] == 0x00 && command[1] == 0x13)
            {
                model.clientData[command[2]].balls = command[3];
            }
            if (command[0] == 0x00 && command[1] == 0x14)
            {
                if (command[3] == 1)
                {
                    GameObject shield = (GameObject)Instantiate(Shield);
                    shield.GetComponent<Transform>().position = GameObject.Find(command[2].ToString()).transform.position;
                    shield.transform.parent = GameObject.Find(command[2].ToString()).transform;
                    model.clientData[command[2]].shieldtime = Time.time + 15;
                }
                else
                {
                    GameObject shield = (GameObject)Instantiate(Repulsion);
                    shield.GetComponent<Transform>().position = GameObject.Find(command[2].ToString()).transform.position;
                    shield.transform.parent = GameObject.Find(command[2].ToString()).transform;
                }
            }
            if (command[0] == 0x00 && command[1] == 0x16)
            {
                controller.resetMap();
            }
            if (command[0] == 0x00 && command[1] == 0x17)
            {
                controller.resetMap();
                model.isGameStarted = false;
                model.isGameFinished = false;
            }
            if (command[0] == 0x00 && command[1] == 0x18)
            {
                float x = System.BitConverter.ToSingle(command, 3);
                float z = System.BitConverter.ToSingle(command, 7);
                for (int j = 0; j < controller.allInGameObject.Count; j++)
                {
                    if (controller.allInGameObject[j].name.Equals(((int)command[2]).ToString()))
                    {
                        controller.allInGameObject[j].GetComponent<Transform>().position = new Vector3(x, controller.allInGameObject[j].transform.position.y, z);
                    }
                }
            }
            if (command[0] == 0x00 && command[1] == 0x19)
            {
                float coord = System.BitConverter.ToSingle(command, 3);
                for (int j = 0; j < controller.allInGameObject.Count; j++)
                {
                    if (controller.allInGameObject[j].name.Equals(((int)command[2]).ToString()))
                    {
                        if(command[2]==0 || command[2]==1)
                            controller.allInGameObject[j].GetComponent<Transform>().position = new Vector3(coord, controller.allInGameObject[j].transform.position.y, controller.allInGameObject[j].transform.position.z);
                        if (command[2] == 2 || command[2] == 3)
                            controller.allInGameObject[j].GetComponent<Transform>().position = new Vector3(controller.allInGameObject[j].transform.position.x, controller.allInGameObject[j].transform.position.y, coord);
                    }
                }
            }
        }
    }

    public void softDisconnection()
    {
        client.disconnect();
        Destroy(gameObject);
    }

    public void sendMovement(float value)
    {
        byte[] header = new byte[2];
        header[0] = 0x00;
        header[1] = 0x0c;
        byte[] message = new byte[5];
        System.Array.Copy(System.BitConverter.GetBytes(value),0,message,0,4);
        message[4] = (byte)model.confirmedPort;
        client.SendUdpInfo(header, message);
    }

    public void sendReady()
    {
        byte[] header = new byte[2];
        header[0] = 0x00;
        header[1] = 0x05;
        client.SendInfo(header, new byte[0]);
    }

    public void sendAbility(int ability)
    {
        byte[] header = new byte[2];
        header[0] = 0x00;
        header[1] = 0x15;
        byte[] message = new byte[1];
        message[0] = (byte)ability;
        client.SendInfo(header, message);
    }

    public void forcedDisconnection()
    {
        client = null;
        model.isGameStarted = false;
        model.usernameAk = false;
        GameObject.Find("Main Camera").GetComponent<MenuGUI>().message = true;
        GameObject.Find("Main Camera").GetComponent<MenuGUI>().messageText = "You've been kicked or the server closed";
        model.resetClientData();
    }
}
