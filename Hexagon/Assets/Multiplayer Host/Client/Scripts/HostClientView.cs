using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class HostClientView : MonoBehaviour {
    Texture2D bg;
    HostClientController controller;
    public HostClientModel model;
    ClientNManager nmanager;
    public Texture lobby;
    Texture2D blueled;
    public Object auxiliaryMovement;
    public GameObject auxObject;
    public float gameTime=0;
    public float nextMoveUpdate=0;
    public bool used=false;
    public GUIStyle endgameText;
    public GUIStyle homebutton;
    public bool isMenuOpen=false;
    public Texture shield;
        public Texture2D blue;
    public GUIStyle textStyle;

    // Use this for initialization
    void Start () {
        bg = GameObject.Find("Main Camera").GetComponent<MenuGUI>().bg;
        controller = GetComponent<HostClientController>();
        model = GetComponent<HostClientModel>();
        nmanager = GetComponent<ClientNManager>();
        blueled = new Texture2D(1, 1);
        blueled.SetPixel(0, 0, new Color(0f, 0.25f, 1f, 1));
        blueled.Apply();
        blue = new Texture2D(1, 1);
        blue.SetPixel(0, 0, new Color(0f, 0f, 1f, 1));
        blue.Apply();
        auxObject = (GameObject)Instantiate(auxiliaryMovement, new Vector3(0, 0, 0),Quaternion.identity);
        endgameText.normal.textColor = new Color(1, 1, 1, 1);
    }
	
	// Update is called once per frame
	void Update () {
        if (used && nmanager.client == null)
        {
            GameObject.Find("Main Camera").GetComponent<MenuGUI>().isMenuActive = true;
            Destroy(gameObject);
        }
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
        
        if (model.isGameStarted)
        {
            if (!controller.imAlive() && !model.isGameFinished)
            {
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), bg);
                endgameText.fontSize = Screen.width / 9;
                GUI.Label(new Rect(0, Screen.height / 40, Screen.width, Screen.height / 2), "GAME OVER", endgameText);
                endgameText.fontSize = Screen.width / 20;
                GUI.Label(new Rect(0, Screen.height / 5f, Screen.width, Screen.height / 2), "In attesa che la partita finisca", endgameText);
                if (GUI.Button(new Rect(Screen.width / 2 - 37, Screen.height / 1.3f, 75, 75), "", homebutton))
                {
                    controller.resetMap();
                    nmanager.softDisconnection();
                    GameObject.Find("Main Camera").GetComponent<MenuGUI>().isMenuActive = true;
                }
            }
            else
            {
                if (model.isGameFinished)
                {
                    GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), bg);
                    endgameText.fontSize = Screen.width / 14;
                    GUI.Label(new Rect(0, Screen.height / 60, Screen.width, Screen.height / 2), model.clientData[model.winner].username +"\nHA VINTO!", endgameText);
                    endgameText.fontSize = Screen.width / 40;
                    GUI.Label(new Rect(0, Screen.height / 3f, Screen.width, Screen.height / 2), "In attesa del proprietario della lobby.", endgameText);
                    if (GUI.Button(new Rect(Screen.width / 2 - 37, Screen.height / 1.3f, 75, 75), "", homebutton))
                    {
                        controller.resetMap();
                        nmanager.softDisconnection();
                        GameObject.Find("Main Camera").GetComponent<MenuGUI>().isMenuActive = true;
                    }
                }
                else
                {
                    if (isMenuOpen)
                    {
                        if (GUI.Button(new Rect(Screen.width / 2 - 37, Screen.height / 2 - 37, 75, 75), "", homebutton))
                            nmanager.softDisconnection();
                    }
                    GUI.Label(new Rect(Screen.width/2-85, 20, 100, 20), model.clientData[model.getMyPort()].balls.ToString(),textStyle);
                    GUI.DrawTexture(new Rect(Screen.width /2- 55, 13, 140, 42), shield);
                    GUI.DrawTexture(new Rect(Screen.width / 2 - 54, 24, calculatePercent(), 20), blue);
                }
            }
        }
    }

    public int calculatePercent()
    {
        if (Time.time > model.clientData[model.getMyPort()].shieldtime)
            return 100;
        else
            return 100 - (int)((model.clientData[model.getMyPort()].shieldtime - Time.time) * 6.66f);
    }
}
