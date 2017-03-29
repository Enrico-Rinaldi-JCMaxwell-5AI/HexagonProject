using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class HostClientView : MonoBehaviour {
    Texture2D bg;
    HostClientController controller;
    HostClientModel model;
    ClientNManager nmanager;
    public Texture lobby;
    Texture2D blueled;
    public Object auxiliaryMovement;
    public GameObject auxObject;
    public float gameTime=0;
    public float nextMoveUpdate=0;
    
    // Use this for initialization
    void Start () {
        bg = GameObject.Find("Main Camera").GetComponent<MenuGUI>().bg;
        controller = GetComponent<HostClientController>();
        model = GetComponent<HostClientModel>();
        nmanager = GetComponent<ClientNManager>();
        blueled = new Texture2D(1, 1);
        blueled.SetPixel(0, 0, new Color(0f, 0.25f, 1f, 1));
        blueled.Apply();
        auxObject = (GameObject)Instantiate(auxiliaryMovement, new Vector3(0, 0, 0),Quaternion.identity);
    }
	
	// Update is called once per frame
	void Update () {
		if((Input.GetAxis("Horizontal")!=0 || Input.GetKey((KeyCode)InputPreferences.getKeyBoardInput("A")) || Input.GetKey((KeyCode)InputPreferences.getKeyBoardInput("D"))) && model.isGameStarted && !model.isGameFinished)
        {
            float speed = 12f * Input.GetAxis("Horizontal") * Time.deltaTime;
            if (Input.GetKey((KeyCode)InputPreferences.getKeyBoardInput("A")))
                speed = -12f * Time.deltaTime;
            if (Input.GetKey((KeyCode)InputPreferences.getKeyBoardInput("D")))
                speed = 12f * Time.deltaTime;
            if (model.getMyPort() == 1 && auxObject.transform.position.x - speed>-4 && auxObject.transform.position.x - speed<4)
            {
                auxObject.GetComponent<Transform>().position = new Vector3(auxObject.transform.position.x - speed, auxObject.transform.position.y, auxObject.transform.position.z);
                if (nextMoveUpdate < gameTime)
                {
                    nmanager.sendMovement(auxObject.GetComponent<Transform>().position.x);
                    nextMoveUpdate = gameTime + 1 / 16;
                }
            }
            if (model.getMyPort() == 2 && auxObject.transform.position.z - speed > -4 && auxObject.transform.position.z - speed < 4)
            {
                auxObject.GetComponent<Transform>().position = new Vector3(auxObject.transform.position.x , auxObject.transform.position.y, auxObject.transform.position.z - speed);
                if (nextMoveUpdate < gameTime)
                {
                    nmanager.sendMovement(auxObject.GetComponent<Transform>().position.z);
                    nextMoveUpdate = gameTime + 1 / 16;
                }
        }
            if (model.getMyPort() == 3 && auxObject.transform.position.z + speed > -4 && auxObject.transform.position.z - speed < 4)
            {
                auxObject.GetComponent<Transform>().position = new Vector3(auxObject.transform.position.x - speed, auxObject.transform.position.y, auxObject.transform.position.z+ speed);
                if (nextMoveUpdate < gameTime)
                {
                    nmanager.sendMovement(auxObject.GetComponent<Transform>().position.z);
                    nextMoveUpdate = gameTime + 1 / 16;
                }
            } 
        }
        if(model.isGameStarted)
        {
            gameTime = gameTime + Time.deltaTime;
            if (Input.GetKeyDown((KeyCode)InputPreferences.getKeyBoardInput("SH")))
            {
                nmanager.sendAbility(1);
            }
            if (Input.GetKeyDown((KeyCode)InputPreferences.getKeyBoardInput("RE")))
            {
                nmanager.sendAbility(0);
            }
        }
	}

    private void OnGUI()
    {
        if (nmanager.client == null)
        {
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), bg, ScaleMode.ScaleAndCrop);
            model.remoteIP = GUI.TextField(new Rect(100, 180, 80, 20), model.remoteIP);
            model.port = GUI.TextField(new Rect(180, 180, 80, 20), model.port);
            if (GUI.Button(new Rect(180, 100, 80, 20), "Connect") && !model.remoteIP.Equals("") && !model.port.Equals("") && System.Int32.Parse(model.port) > 0 && System.Int32.Parse(model.port) < 65535 && System.Int32.Parse(model.port) != 80 && System.Int32.Parse(model.port) != 53000)
            {
                nmanager.client = new HostClient(model.remoteIP, System.Int32.Parse(model.port));
            }
            return;
        }
        if (!model.usernameAk)
        {
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), bg, ScaleMode.ScaleAndCrop);
            model.username = GUI.TextField(new Rect(100, 180, 80, 20), model.username);
            if (GUI.Button(new Rect(180, 100, 80, 20), "Send") && !model.username.Equals("") && model.username.Length <= 16 && !model.username.Contains(" "))
            {
                byte[] header = new byte[2];
                header[0] = 0x00;
                header[1] = 0x00;
                nmanager.client.SendInfo(header, Encoding.ASCII.GetBytes(PacketSize.composeString(model.username)));
            }
            return;
        }
        else
        {
            if (!model.isGameStarted)
            {
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), lobby, ScaleMode.ScaleAndCrop);
                GUI.Label(new Rect(360, 375, 160, 40), model.clientData[0].username);
                if (model.clientData[1]!=null)
                {
                    if (model.clientData[1].lobbyReady)
                        GUI.DrawTexture(new Rect(320, 123, 160, 4), blueled, ScaleMode.ScaleAndCrop);
                    GUI.Label(new Rect(360, 70, 160, 40), model.clientData[1].username);
                }
                if (model.clientData[2] != null)
                {
                    if (model.clientData[2].lobbyReady)
                        GUI.DrawTexture(new Rect(298, 145, 4, 160), blueled, ScaleMode.ScaleAndCrop);
                    GUI.Label(new Rect(15, 205, 160, 40), model.clientData[2].username);
                }
                if (model.clientData[3] != null)
                {
                    if (model.clientData[3].lobbyReady)
                        GUI.DrawTexture(new Rect(498, 145, 4, 160), blueled, ScaleMode.ScaleAndCrop);
                    GUI.Label(new Rect(625, 205, 160, 40), model.clientData[3].username);
                }
                if(model.getMyPort()==1)
                {
                    if(GUI.Button(new Rect(375, 16, 50, 50), ""))
                    {
                        nmanager.softDisconnection();
                    }
                    if (GUI.Button(new Rect(320, 116, 160, 18), ""))
                    {
                        nmanager.sendReady();
                    }
                }
                if(model.getMyPort() == 2)
                {
                    if (GUI.Button(new Rect(191, 201, 50, 50), ""))
                    {
                        nmanager.softDisconnection();
                    }
                    if (GUI.Button(new Rect(291, 145, 160, 18), ""))
                    {
                        nmanager.sendReady();
                    }
                }
                if(model.getMyPort() == 3)
                {
                    if (GUI.Button(new Rect(559, 201, 50, 50), ""))
                    {
                        nmanager.softDisconnection();
                    }
                    if (GUI.Button(new Rect(491, 201, 160, 18), ""))
                    {
                        nmanager.sendReady();
                    }
                }
            }
        }
        if (model.isGameStarted)
        {
            for(int i=0;i<4;i++)
            {
                if(model.clientData[i]!=null)
                {
                    GUI.Label(new Rect(100 * i, 20, 100, 20), model.clientData[i].balls.ToString());
                }
            }
        }
    }
}
