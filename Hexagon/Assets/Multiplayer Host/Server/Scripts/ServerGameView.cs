using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerGameView : MonoBehaviour {

    NetworkManager nmanager;
    ServerGameController controller;
    ServerGameModel model;
    Texture2D bg;
    public Texture2D lobbyTexture;
    Texture2D blueled;
    public GameObject[] navs;
    public bool serverOption = false;
    public int slidervalue;
    public bool used;
    public GUIStyle startButton;

    // Use this for initialization
    void Start () {
        nmanager = GetComponent<NetworkManager>();
        model = GetComponent<ServerGameModel>();
        controller = GetComponent<ServerGameController>();
        bg = GameObject.Find("Main Camera").GetComponent<MenuGUI>().bg;
        blueled = new Texture2D(1, 1);
        blueled.SetPixel(0, 0, new Color(0f, 0.25f, 1f, 1));
        blueled.Apply();
        slidervalue = 1;
    }
	
	// Update is called once per frame
	void Update () {
        if(nmanager.server==null && used)
        {
            GameObject.Find("Main Camera").GetComponent<MenuGUI>().isMenuActive = true;
            Destroy(gameObject);
        }
		if(model.isGameStarted)
        {
            if (navs != null) {
                float speed = 12f * Input.GetAxis("Horizontal") * Time.deltaTime;
                if (Input.GetKey((KeyCode)InputPreferences.getKeyBoardInput("A")))
                    speed = -12f * Time.deltaTime;
                if (Input.GetKey((KeyCode)InputPreferences.getKeyBoardInput("D")))
                    speed = 12f * Time.deltaTime;
                if (navs[0]!=null && speed!=0 && navs[0].transform.position.x + speed<4f && navs[0].transform.position.x + speed > -4f)
                {
                    navs[0].GetComponent<Transform>().position = new Vector3(navs[0].transform.position.x + speed, navs[0].transform.position.y, navs[0].transform.position.z);
                    nmanager.sendUpdateNav(0, navs[0].transform.position.x);
                }
            }
            if(Input.GetKeyDown((KeyCode)InputPreferences.getKeyBoardInput("SH")))
            {
                controller.useAbility(0, 1);
            }
            if (Input.GetKeyDown((KeyCode)InputPreferences.getKeyBoardInput("RE")))
            {
                controller.useAbility(0, 0);
            }
            if (Input.GetKeyDown((KeyCode)InputPreferences.getKeyBoardInput("ES")))
            {
                serverOption = !serverOption;
            }
            
        }
	}

    void OnGUI()
    {
            if (!model.isGameStarted)
            {
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), lobbyTexture, ScaleMode.ScaleAndCrop);
                if(nmanager.canMatchStart())
                {
                    if(GUI.Button(new Rect(387, 212, 26, 26), "",startButton))
                    {
                        //START MATCH
                        controller.startGame();
                        model.isGameStarted = true;
                    }
                }
                GUI.Label(new Rect(360, 375, 160, 40), nmanager.server.ownerData.username);
                if (nmanager.server.clientSocketList.Exist(0))
                {
                    if(nmanager.server.clientSocketList.Get(0).clientData!=null && nmanager.server.clientSocketList.Get(0).clientData.lobbyReady)
                        GUI.DrawTexture(new Rect(320, 123, 160, 4), blueled, ScaleMode.ScaleAndCrop);
                    GUI.Label(new Rect(360, 70, 160, 40), nmanager.server.getClientInfo(0));
                    if(GUI.Button(new Rect(375,16,50,50),""))
                    {
                        nmanager.server.kickClient(0);
                    }
                }
                if (nmanager.server.clientSocketList.Exist(1))
                {
                    if(nmanager.server.clientSocketList.Get(1).clientData != null && nmanager.server.clientSocketList.Get(1).clientData.lobbyReady)
                        GUI.DrawTexture(new Rect(298, 145, 4, 160), blueled, ScaleMode.ScaleAndCrop);
                    GUI.Label(new Rect(15, 205, 160, 40), nmanager.server.getClientInfo(1));
                    if (GUI.Button(new Rect(191, 201, 50, 50), ""))
                    {
                        nmanager.server.kickClient(1);
                    }
                }
                if (nmanager.server.clientSocketList.Exist(2))
                {
                    if(nmanager.server.clientSocketList.Get(2).clientData != null && nmanager.server.clientSocketList.Get(2).clientData.lobbyReady)
                        GUI.DrawTexture(new Rect(498, 145, 4, 160), blueled, ScaleMode.ScaleAndCrop);
                    GUI.Label(new Rect(625, 205, 160, 40), nmanager.server.getClientInfo(2));
                    if (GUI.Button(new Rect(559, 201, 50, 50), ""))
                    {
                        nmanager.server.kickClient(2);
                    }
                }
                if (GUI.Button(new Rect(500, 100, 100, 20), "Shutdown"))
                {
                    nmanager.server.shutdownServer();
                    nmanager.server = null;
                }
            }else
            {
                if (serverOption)
                {
                    if (GUI.Button(new Rect(Screen.width / 2 - 85, Screen.height / 2 - 25, 50, 50), "Restart"))
                    {
                        controller.resetMap();
                        nmanager.sendRestart();
                        controller.startGame();
                        serverOption = false;
                    }
                    if (GUI.Button(new Rect(Screen.width / 2 - 25, Screen.height / 2 - 25, 50, 50), "Restart"))
                    {
                        controller.resetMap();
                        model.isGameFinished = false;
                        model.isGameStarted = false;
                        nmanager.sendReturnToLobby();
                        serverOption = false;
                    }
                    if (GUI.Button(new Rect(Screen.width / 2 + 35, Screen.height / 2 - 25, 50, 50), "Shutdown"))
                    {
                        nmanager.server.shutdownServer();
                        controller.resetMap();
                        Destroy(gameObject);
                        GameObject.Find("Main Camera").GetComponent<MenuGUI>().isMenuActive = true;
                        serverOption = false;
                    }
                }
            }
        
    }
}
