using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuGUI : MonoBehaviour {
    public bool isMenuActive = true;
    public int menuState = 0;
    public bool split = false;
    public Texture JoyBack;
    public Texture LobbyBg;
    public GUIStyle startButton;
    public Texture2D redLed;
    public GUIStyle OptionSel;
    public GUIStyle SplitSel;
    public GUIStyle MultiSel;
    public GUIStyle hexFont;
    public Texture2D bg;
    public Object splitGameObject;
    public Object HostGameManager;
    public Object ClientHostGameManager;
    public bool splitMode = true;
    public bool optionMenù;
    public string width;
    public string height;
    public bool fullscreen=false;
    public bool inGameHint=false;
    public int optionTab = 0;
    public int joychoose;
    public bool enterListening;
    public int buttonNum;
    public bool message;
    public string messageText;
    public bool keyBoardChoose=false;
    public bool keyEnterListening = false;
    public string keyButtonName="";
    public GUIStyle bgMessage;
    public int port=30000;
    public string Username;
    public string IP;
    public int slidervalue=1;

    // Use this for initialization
    void Start () {
        
        Screen.SetResolution(800, 450, false);
        if(PlayerPrefs.GetInt("SWidth")==0)
        {
            PlayerPrefs.SetInt("SWidth", 1024);
            PlayerPrefs.SetInt("SHeight", 768);
            PlayerPrefs.SetInt("GHint", 1);
            InputPreferences.setDefault();
            message = true;
            messageText = "Benvenuto su Hexagon Project!\nSembra essere la prima volta che giochi.\nTi consigliamo quindi di impostare nelle opzioni i comandi\ne la risoluzione desiderata.\nBuon divertimento!\n\n -Team di Hexagon Project";
        }
        Texture2D temp = new Texture2D(1, 1);
        temp.SetPixel(0, 0, new Color(0, 0, 0, 1));
        temp.Apply();
        bgMessage.normal.background = temp;
        bg = new Texture2D(1, 1);
        bg.SetPixel(0, 0, new Color(0.11f, 0.11f, 0.12f, 1));
        bg.Apply();
        redLed = new Texture2D(1, 1);
        redLed.SetPixel(0, 0, new Color(0f, 0f, 1f, 1));
        redLed.Apply();
        loadSettings();
    }

    public void loadSettings()
    {
        if (PlayerPrefs.GetInt("GHint") == 1)
            inGameHint = true;
        if (PlayerPrefs.GetInt("SFull") == 1)
            fullscreen = true;
    }
	
	// Update is called once per frame
	void Update () {
        if(enterListening)
        {
            for(int i = 0; i < 20; i++)
            {
                if (Input.GetKeyDown("joystick button " + i))
                {
                    InputPreferences.setInput(joychoose, buttonNum, i);
                    enterListening = false;
                    buttonNum = -1;
                    message = true;
                    messageText = "Input changed successfully!";
                }
            }
        }
        if(keyBoardChoose)
        {
            if(InputPreferences.getKeyId()!=-1&&!keyButtonName.Equals(""))
            {
                InputPreferences.setKeyBoardInput(keyButtonName, InputPreferences.getKeyId());
                keyEnterListening = false;
                keyButtonName = "";
                message = true;
                messageText = "Input changed successfully!";
            }
        }
        if (menuState == 1 && isMenuActive && Input.GetKeyDown("joystick button " + InputPreferences.getInput(1, 1)) && !optionMenù)
        {
            changeMenuState(0);
        }
        if (menuState == 2 && isMenuActive && Input.GetKeyDown("joystick button " + InputPreferences.getInput(1, 1)) && !optionMenù)
        {
            changeMenuState(0);
            GetComponent<InputManager>().backReset();
        }
        if (GetComponent<InputManager>().isReady() && Input.GetKeyDown("joystick button " + InputPreferences.getInput(1, 7)) && isMenuActive && menuState == 2 && !optionMenù)
        {
            instantiateSplit();
            menuState = 0;
            isMenuActive = false;
            setSavedScreenRes();
            setScreenCameras();
        }
        if (isMenuActive && menuState == 0)
        {
            if (Random.Range(50, 255) > (hexFont.normal.textColor.g*255))
            {
                hexFont.normal.textColor = new Color(0, hexFont.normal.textColor.g + 0.01953125f, 0);
            }
            else
            {
                hexFont.normal.textColor = new Color(0, hexFont.normal.textColor.g - 0.01953125f, 0);
            }
        }

    }

    void OnGUI()
    {   
        if (optionMenù)
        {
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), bg, ScaleMode.ScaleAndCrop);
            if (GUI.Button(new Rect(600, 0, 200, 30), "Ritorna alle impostazioni iniziali"))
            {
                PlayerPrefs.SetInt("SWidth", 1024);
                PlayerPrefs.SetInt("SHeight", 768);
                PlayerPrefs.SetInt("SFull", 0);
                PlayerPrefs.SetInt("GHint", 1);
                InputPreferences.setDefault();
                if (!isMenuActive)
                    setSavedScreenRes();
                loadSettings();
            }
            if (GUI.Button(new Rect(20, 20, 64, 64), "Comandi"))
            {
                optionTab = 0;
            }
            if (GUI.Button(new Rect(94, 20, 64, 64), "Grafica"))
            {
                optionTab = 1;
            }
            if (GUI.Button(new Rect(0, Screen.height - 50, Screen.width, 50), "Torna al menù principale"))
            {
                optionMenù = false;
            }
            switch (optionTab)
            {
                case 0:
                    {
                        GUI.Label(new Rect(25, 104, Screen.width, 50), "1. Seleziona il device di input da configurare");
                        if (GUI.Button(new Rect(20, 134, 70, 30), "Tastiera")) keyBoardChoose = true;
                        if (GUI.Button(new Rect(100, 134, 50, 30), "Joy1"))
                        {
                            joychoose = 1;
                            keyBoardChoose = false;
                        }
                        if (GUI.Button(new Rect(160, 134, 50, 30), "Joy2"))
                        {
                            joychoose = 2;
                            keyBoardChoose = false;
                        }
                        if (GUI.Button(new Rect(220, 134, 50, 30), "Joy3"))
                        {
                            joychoose = 3;
                            keyBoardChoose = false;
                        }
                        if (GUI.Button(new Rect(280, 134, 50, 30), "Joy4"))
                        {
                            joychoose = 4;
                            keyBoardChoose = false;
                        }
                        GUI.Label(new Rect(25, 174, Screen.width, 50), "2. Selezionare successivamente l'azione da configurare, premendo l'azione\ndesiderata qui in basso con il mouse e successivamente con il tasto desiderato.");
                        if (!keyBoardChoose)
                        {
                            if (GUI.Button(new Rect(20, 224, 50, 30), "Pausa") && joychoose != 0)
                            {
                                enterListening = true;
                                buttonNum = 7;
                                message = true;
                                messageText = "Listening for a joystick input";
                            }
                            if (GUI.Button(new Rect(80, 224, 50, 30), "Scudo") && joychoose != 0)
                            {
                                enterListening = true;
                                buttonNum = 2;
                                message = true;
                                messageText = "Listening for a joystick input";
                            }
                            if (GUI.Button(new Rect(140, 224, 80, 30), "Repulsione") && joychoose != 0)
                            {
                                enterListening = true;
                                buttonNum = 0;
                                message = true;
                                messageText = "Listening for a joystick input";
                            }
                        }else
                        {
                            if(GUI.Button(new Rect(20,224,60,30),"Sinistra"))
                            {
                                keyEnterListening = true;
                                keyButtonName = "A";
                            }
                            if (GUI.Button(new Rect(90, 224, 50, 30), "Destra"))
                            {
                                keyEnterListening = true;
                                keyButtonName = "D";
                            }
                            if (GUI.Button(new Rect(150, 224, 50, 30), "Scudo"))
                            {
                                keyEnterListening = true;
                                keyButtonName = "RE";
                            }
                            if (GUI.Button(new Rect(210, 224, 80, 30), "Repulsione"))
                            {
                                keyEnterListening = true;
                                keyButtonName = "SH";
                            }
                            if (GUI.Button(new Rect(300, 224, 50, 30), "Pausa"))
                            {
                                keyEnterListening = true;
                                keyButtonName = "ES";
                            }
                        }
                    }
                    break;
                case 1:
                    {
                        GUI.Label(new Rect(20, 120, 100, 20), "Risoluzione");
                        width = GUI.TextField(new Rect(20, 150, 100, 20), width);
                        height = GUI.TextField(new Rect(130, 150, 100, 20), height);
                        fullscreen = GUI.Toggle(new Rect(240, 150, 150, 20), fullscreen, "Fullscreen");
                        if (GUI.Button(new Rect(20, 180, 120, 30), "Imposta e salva"))
                        {
                            if (!width.Equals("") && !height.Equals("") && System.Int32.Parse(width) >= 800 && System.Int32.Parse(height) >= 450)
                            {
                                PlayerPrefs.SetInt("SWidth", System.Int32.Parse(width));
                                PlayerPrefs.SetInt("SHeight", System.Int32.Parse(height));
                                if (fullscreen)
                                    PlayerPrefs.SetInt("SFull", 1);
                                else
                                    PlayerPrefs.SetInt("SFull", 0);
                            }
                            if (inGameHint)
                                PlayerPrefs.SetInt("GHint", 1);
                            else
                                PlayerPrefs.SetInt("GHint", 0);
                            setSavedScreenRes();
                        }
                    }
                    break;
            }
            
        }
        else
        {
            if (isMenuActive)
            {
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), bg, ScaleMode.ScaleAndCrop);
                switch (menuState)
                {
                    case 0:
                        {
                            GUI.Label(new Rect(0, 0, Screen.width, Screen.height / 2), "Hexagon\nProject", hexFont);
                            hexFont.fontSize = (int)(Screen.width * 0.075f);
                            if (GUI.Button(new Rect((Screen.width * 0.58f) - Screen.width * 0.15625f, (Screen.height * 0.85f) - Screen.width * 0.15625f, Screen.width * 0.15625f, Screen.width * 0.15625f), "", OptionSel))
                            {
                                optionMenù = true;
                            }
                            if (GUI.Button(new Rect((Screen.width * 0.81f) - Screen.width * 0.15625f, (Screen.height * 0.85f) - Screen.width * 0.15625f, Screen.width * 0.15625f, Screen.width * 0.15625f), "", MultiSel))
                            {
                                menuState = 1;
                            }
                            if (GUI.Button(new Rect((Screen.width*0.35f)- Screen.width * 0.15625f, (Screen.height * 0.85f)- Screen.width * 0.15625f, Screen.width * 0.15625f, Screen.width * 0.15625f), "", SplitSel))
                            {
                                changeMenuState(2);
                            }
                        }
                        break;
                    case 1:
                        {
                            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), LobbyBg);
                            if(GUI.Button(new Rect(0,0,50,50),"Indietro"))
                            {
                                menuState = 0;
                            }
                            if (GUI.Button(new Rect(20, 355, 350, 30), "Crea la lobby") && port > 0 && port < 65535 && port != 80 && port != 53000 && port != 52999 && port != 52998 && !Username.Equals("") && Username.Length <= 16 && !Username.Contains(" "))
                            {
                                GameObject server = Instantiate(HostGameManager) as GameObject;
                                server.GetComponent<ServerGameModel>().port = port.ToString();
                                server.GetComponent<ServerGameModel>().user = Username;
                                server.GetComponent<ServerGameModel>().updatesPerSecond = (int)Mathf.Pow(2, slidervalue - 1) * 8;
                                server.GetComponent<NetworkManager>().server = new HostServer(port, server.GetComponent<ServerGameModel>());
                                server.GetComponent<NetworkManager>().server.ownerData = new ClientData(Username);
                                server.GetComponent<ServerGameView>().used = true;
                                isMenuActive = false;
                            }
                            port = System.Int32.Parse(GUI.TextField(new Rect(112, 123, 250, 20), port.ToString()));
                            Username = GUI.TextField(new Rect(112, 153, 250, 20), Username);
                            slidervalue = (int)GUI.HorizontalSlider(new Rect(20, 229, 325, 20), slidervalue, 1, 4);
                            if (GUI.Button(new Rect(425, 355, 350, 30), "Connettiti") && !IP.Equals("") && !port.Equals("") && port > 0 && port < 65535 && port != 80 && port != 53000 && !Username.Equals("") && Username.Length <= 16 && !Username.Contains(" "))
                            {
                                GameObject client = Instantiate(ClientHostGameManager) as GameObject;
                                client.GetComponent<HostClientModel>().remoteIP = IP;
                                client.GetComponent<HostClientModel>().port = port.ToString();
                                client.GetComponent<HostClientModel>().username = Username;
                                try
                                {
                                    client.GetComponent<ClientNManager>().client = new HostClient(IP, port);
                                    byte[] header = new byte[2];
                                    header[0] = 0x00;
                                    header[1] = 0x00;
                                    client.GetComponent<ClientNManager>().client.SendInfo(header, System.Text.Encoding.ASCII.GetBytes(PacketSize.composeString(Username)));
                                    client.GetComponent<HostClientView>().used = true;
                                    isMenuActive = false;
                                }catch(System.Exception)
                                {
                                    message = true;
                                    messageText = "Impossibile raggiungere l'host remoto";
                                    Destroy(client);
                                }
                            }
                            IP = GUI.TextField(new Rect(517, 123, 250, 20), IP);
                            port = System.Int32.Parse(GUI.TextField(new Rect(517, 153, 250, 20), port.ToString()));
                            Username = GUI.TextField(new Rect(517, 183, 250, 20), Username);

                        } break;
                    case 2:
                        {
                            GUI.DrawTexture(new Rect(0, 0, 800, 450), JoyBack, ScaleMode.ScaleToFit);
                            if (GUI.Button(new Rect(0, 420, Screen.width, 30), "Torna al menù principale"))
                            {
                                changeMenuState(0);
                                GetComponent<InputManager>().backReset();
                            }
                            if (GetComponent<InputManager>().Joys[0] != -1)
                            {
                                GUI.DrawTexture(new Rect(148, 323, 160, 4), redLed, ScaleMode.ScaleAndCrop);
                            }
                            if (GetComponent<InputManager>().Joys[1] != -1)
                            {
                                GUI.DrawTexture(new Rect(148, 123, 160, 4), redLed, ScaleMode.ScaleAndCrop);
                            }
                            if (GetComponent<InputManager>().Joys[2] != -1)
                            {
                                GUI.DrawTexture(new Rect(126, 145, 4, 160), redLed, ScaleMode.ScaleAndCrop);
                            }
                            if (GetComponent<InputManager>().Joys[3] != -1)
                            {
                                GUI.DrawTexture(new Rect(326, 145, 4, 160), redLed, ScaleMode.ScaleAndCrop);
                            }
                            if (GetComponent<InputManager>().isReady())
                            {
                                if (GUI.Button(new Rect(215, 212, 26, 26), "" ,startButton))
                                {
                                    instantiateSplit();
                                    menuState = 0;
                                    isMenuActive = false;
                                    setSavedScreenRes();
                                    setScreenCameras();
                                }
                            }
                        }
                        break;
                }
            }          
        }
        if (message)
        {
            GUI.Box(new Rect(200, 112, 400, 225),messageText,bgMessage);
            GUI.Label(new Rect(200, 112, 400, 200), messageText);
            if (GUI.Button(new Rect(580, 112, 20, 20), "X"))
            {
                message = false;
            }
            
        }
    }

    public void setSavedScreenRes()
    {
        if(PlayerPrefs.GetInt("SFull") == 1)
        {
            Screen.SetResolution(PlayerPrefs.GetInt("SWidth"), PlayerPrefs.GetInt("SHeight"), true);
        }
        else
        {
            Screen.SetResolution(PlayerPrefs.GetInt("SWidth"), PlayerPrefs.GetInt("SHeight"), false);
        }
    }

    public void changeMenuState(int state)
    {
        menuState = state;
    }

    public void setScreenCameras()
    {
        if(splitMode && GetComponent<InputManager>().numberOfPlayer()==2)
        {
            GameObject.Find("Main Camera").GetComponent<Camera>().rect = new Rect(0, 0.5f, 1, 0.5f);
            GameObject.Find("AuxFront").GetComponent<Camera>().rect = new Rect(0, 0, 1, 0.5f);
            GameObject.Find("Main Camera").GetComponent<Camera>().fieldOfView = 60f;
            GameObject.Find("AuxFront").GetComponent<Camera>().fieldOfView = 60f;
            return;
        }
        if (splitMode && GetComponent<InputManager>().numberOfPlayer() == 3)
        {
            GameObject.Find("Main Camera").GetComponent<Camera>().rect = new Rect(0, 0.5f, 0.5f, 0.5f);
            GameObject.Find("AuxFront").GetComponent<Camera>().rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
            GameObject.Find("AuxLeft").GetComponent<Camera>().rect = new Rect(0, 0, 1, 0.5f);
            GameObject.Find("AuxLeft").GetComponent<Camera>().fieldOfView = 60f;
            return;
        }
        if (splitMode && GetComponent<InputManager>().numberOfPlayer() == 4)
        {
            GameObject.Find("Main Camera").GetComponent<Camera>().rect = new Rect(0, 0.5f, 0.5f, 0.5f);
            GameObject.Find("AuxFront").GetComponent<Camera>().rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
            GameObject.Find("AuxLeft").GetComponent<Camera>().rect = new Rect(0, 0f, 0.5f, 0.5f);
            GameObject.Find("AuxRight").GetComponent<Camera>().rect = new Rect(0.5f, 0, 0.5f, 0.5f);
            return;
        }
        GameObject.Find("Main Camera").GetComponent<Camera>().rect = new Rect(0, 0, 1, 1);
    }

    public void instantiateSplit()
    {
        Instantiate(splitGameObject).name = "SplitScreenGameManager";
        GetComponent<Transform>().position = new Vector3(0, 4.28f, -9.49f);
        GetComponent<Transform>().rotation = Quaternion.Euler(22.853f, 0, 0);
    }
}
