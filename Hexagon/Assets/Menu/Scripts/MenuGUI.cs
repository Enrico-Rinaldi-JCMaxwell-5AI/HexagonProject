using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuGUI : MonoBehaviour {
    public bool isMenuActive = true;
    public int menuState = 0;
    public string username;
    public string password;
    public bool split = false;
    public Texture JoyBack;
    public Texture P1;
    public Texture P2;
    public Texture P3;
    public Texture P4;
    public Texture2D bg;
    public Object splitGameObject;
    public Object HostGameManager;
    public Object ClientHostGameManager;
    public bool logged=false;
    public bool splitMode = false;
    public bool optionMenù;
    public string width;
    public string height;
    public bool fullscreen=false;
    public bool inGameHint=false;
    public int optionTab = 0;
    public int joychoose;
    public bool enterListening;
    public int buttonNum;
    public Texture joybg;
    public bool message;
    public string messageText;
    public bool keyBoardChoose=false;
    public bool keyEnterListening = false;
    public string keyButtonName="";

    // Use this for initialization
    void Start () {
        
        Screen.SetResolution(800, 450, false);
        if(PlayerPrefs.GetInt("SWidth")==0)
        {
            PlayerPrefs.SetInt("SWidth", 1024);
            PlayerPrefs.SetInt("SHeight", 768);
            PlayerPrefs.SetInt("GHint", 1);
            InputPreferences.setDefault();
        }
        bg = new Texture2D(1, 1);
        bg.SetPixel(0, 0, new Color(0.11f, 0.11f, 0.12f, 1));
        bg.Apply();
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
        if (menuState == 1 && isMenuActive && Input.GetKeyDown("joystick button "+InputPreferences.getInput(1,0)) && split && !optionMenù)
        {
            changeMenuState(2);
        }
        if (menuState==0 && isMenuActive && Input.GetKeyDown("joystick button " + InputPreferences.getInput(1, 0)) && !optionMenù)
        {
            changeMenuState(1);
        }
        if (menuState == 1 && isMenuActive && Input.GetKeyDown("joystick button " + InputPreferences.getInput(1, 1)) && !optionMenù)
        {
            changeMenuState(0);
        }
        if(menuState == 1 && isMenuActive && Input.GetAxis("Horizontal") == 1 && !optionMenù)
        {
            split = true;
        }
        if (menuState == 1 && isMenuActive && Input.GetAxis("Horizontal") == -1 && !optionMenù)
        {
            split =false;
        }
        if (menuState == 2 && isMenuActive && Input.GetKeyDown("joystick button " + InputPreferences.getInput(1, 1)) && !optionMenù)
        {
            changeMenuState(1);
            GetComponent<InputManager>().backReset();
        }
        if (menuState == 2 && isMenuActive && Input.GetKeyDown("joystick button " + InputPreferences.getInput(1, 6)) && !optionMenù)
        {
            splitMode = !splitMode;
        }
        if (GetComponent<InputManager>().isReady() && Input.GetKeyDown("joystick button " + InputPreferences.getInput(1, 7)) && isMenuActive && menuState == 2 && !optionMenù)
        {
            instantiateSplit();
            menuState = 0;
            isMenuActive = false;
            setSavedScreenRes();
            setScreenCameras();
        }

    }

    void OnGUI()
    {   
        if (optionMenù)
        {
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), bg, ScaleMode.ScaleAndCrop);
            if (GUI.Button(new Rect(700, 0, 100, 20), "Restore Default Settings"))
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
            if(GUI.Button(new Rect(0,0,50,50),"GS"))
            {
                optionTab = 0;
            }
            if (GUI.Button(new Rect(0, 50, 50, 50), "Input"))
            {
                optionTab = 1;
            }
            if (GUI.Button(new Rect(0, 100, 50, 50), "Graphic"))
            {
                optionTab = 2;
            }
            if (GUI.Button(new Rect(0, Screen.height - 50, Screen.width, 50), "Back"))
            {
                optionMenù = false;
            }
            switch (optionTab)
            {
                case 0:
                    {
                        inGameHint = GUI.Toggle(new Rect(320, 160, 150, 20), inGameHint, "Enable in game hints"); 
                    }break;
                case 1:
                    {
                        
                        if (GUI.Button(new Rect(70, 20, 20, 20), "Key")) keyBoardChoose = true;
                        if (GUI.Button(new Rect(100, 20, 20, 20), "1"))
                        {
                            joychoose = 1;
                            keyBoardChoose = false;
                        }
                        if (GUI.Button(new Rect(130, 20, 20, 20), "2"))
                        {
                            joychoose = 2;
                            keyBoardChoose = false;
                        }
                        if (GUI.Button(new Rect(160, 20, 20, 20), "3"))
                        {
                            joychoose = 3;
                            keyBoardChoose = false;
                        }
                        if (GUI.Button(new Rect(190, 20, 20, 20), "4"))
                        {
                            joychoose = 4;
                            keyBoardChoose = false;
                        }
                        if (!keyBoardChoose)
                        {
                            GUI.DrawTexture(new Rect(175, 50, 446, 300), joybg);
                            if (GUI.Button(new Rect(234, 109, 78, 30), "LB") && joychoose != 0)
                            {
                                enterListening = true;
                                buttonNum = 4;
                                message = true;
                                messageText = "Listening for a joystick input";
                            }
                            if (GUI.Button(new Rect(482, 109, 78, 30), "RB") && joychoose != 0)
                            {
                                enterListening = true;
                                buttonNum = 5;
                                message = true;
                                messageText = "Listening for a joystick input";
                            }
                            if (GUI.Button(new Rect(244, 180, 55, 55), "LA") && joychoose != 0)
                            {
                                enterListening = true;
                                buttonNum = 8;
                                message = true;
                                messageText = "Listening for a joystick input";
                            }
                            if (GUI.Button(new Rect(428, 250, 55, 45), "RA") && joychoose != 0)
                            {
                                enterListening = true;
                                buttonNum = 9;
                                message = true;
                                messageText = "Listening for a joystick input";
                            }
                            if (GUI.Button(new Rect(342, 192, 21, 15), "Sel") && joychoose != 0)
                            {
                                enterListening = true;
                                buttonNum = 6;
                                message = true;
                                messageText = "Listening for a joystick input";
                            }
                            if (GUI.Button(new Rect(431, 192, 21, 15), "Ent") && joychoose != 0)
                            {
                                enterListening = true;
                                buttonNum = 7;
                                message = true;
                                messageText = "Listening for a joystick input";
                            }
                            if (GUI.Button(new Rect(507, 156, 30, 30), "Y") && joychoose != 0)
                            {
                                enterListening = true;
                                buttonNum = 3;
                                message = true;
                                messageText = "Listening for a joystick input";
                            }
                            if (GUI.Button(new Rect(473, 185, 30, 26), "X") && joychoose != 0)
                            {
                                enterListening = true;
                                buttonNum = 2;
                                message = true;
                                messageText = "Listening for a joystick input";
                            }
                            if (GUI.Button(new Rect(538, 182, 30, 27), "B") && joychoose != 0)
                            {
                                enterListening = true;
                                buttonNum = 1;
                                message = true;
                                messageText = "Listening for a joystick input";
                            }
                            if (GUI.Button(new Rect(503, 211, 31, 27), "A") && joychoose != 0)
                            {
                                enterListening = true;
                                buttonNum = 0;
                                message = true;
                                messageText = "Listening for a joystick input";
                            }
                        }else
                        {
                            if(GUI.Button(new Rect(100,50,20,20),"left"))
                            {
                                keyEnterListening = true;
                                keyButtonName = "A";
                            }
                            if (GUI.Button(new Rect(130, 50, 20, 20), "right"))
                            {
                                keyEnterListening = true;
                                keyButtonName = "D";
                            }
                            if (GUI.Button(new Rect(100, 80, 20, 20), "ab1"))
                            {
                                keyEnterListening = true;
                                keyButtonName = "RE";
                            }
                            if (GUI.Button(new Rect(130, 80, 20, 20), "ab2"))
                            {
                                keyEnterListening = true;
                                keyButtonName = "SH";
                            }
                        }
                    }
                    break;
                case 2:
                    {
                        GUI.Label(new Rect(100, 100, 100, 20), "Resolution");
                        width = GUI.TextField(new Rect(100, 130, 100, 20), width);
                        height = GUI.TextField(new Rect(210, 130, 100, 20), height);
                        fullscreen = GUI.Toggle(new Rect(320, 130, 150, 20), fullscreen, "Fullscreen");
                        if (GUI.Button(new Rect(100, 160, 100, 50), "Set and save"))
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
                if (GUI.Button(new Rect(0, 430, 100, 20), "Options"))
                {
                    optionMenù = true;
                }
                switch (menuState)
                {
                    case 0:
                        {
                            username = GUI.TextField(new Rect(20, 20, 260, 30), username);
                            password = GUI.PasswordField(new Rect(20, 70, 260, 30), password, '*');
                            GUI.Button(new Rect(200, 120, 80, 20), "Login");
                            if(GUI.Button(new Rect(20, 150, 80, 20), "Host a game"))
                            {
                                Instantiate(HostGameManager);
                                isMenuActive = false;
                            }
                            if (GUI.Button(new Rect(20, 180, 80, 20), "Enter an hosted game"))
                            {
                                Instantiate(ClientHostGameManager);
                                isMenuActive = false;
                            }
                            
                            if (GUI.Button(new Rect(20, 250, 260, 30), "Play as guest"))
                            {
                                changeMenuState(1);
                            }
                        }
                        break;
                    case 1:
                        {
                            if (GUI.Button(new Rect(0, 210, 405, 40), "Back"))
                            {
                                changeMenuState(0);
                            }
                            if (GUI.Button(new Rect(15, 15, 180, 180), "Training"))
                            {

                            }
                            if (GUI.Button(new Rect(210, 15, 180, 180), "Multigiocatore \n offline"))
                            {
                                changeMenuState(2);
                            }
                        }
                        break;
                    case 2:
                        {
                            GUI.DrawTexture(new Rect(0, 0, 300, 350), JoyBack, ScaleMode.ScaleToFit);
                            if (GUI.Button(new Rect(0, 310, 300, 40), "Back"))
                            {
                                changeMenuState(1);
                                GetComponent<InputManager>().backReset();
                            }

                            if (GetComponent<InputManager>().Joys[0] != -1)
                            {
                                GUI.DrawTexture(new Rect(126, 157, 50, 50), P1, ScaleMode.ScaleAndCrop);
                            }
                            if (GetComponent<InputManager>().Joys[1] != -1)
                            {
                                GUI.DrawTexture(new Rect(126, 7, 50, 50), P2, ScaleMode.ScaleAndCrop);
                            }
                            if (GetComponent<InputManager>().Joys[2] != -1)
                            {
                                GUI.DrawTexture(new Rect(99, 132, 50, 50), P3, ScaleMode.ScaleAndCrop);
                            }
                            if (GetComponent<InputManager>().Joys[3] != -1)
                            {
                                GUI.DrawTexture(new Rect(249, 132, 50, 50), P4, ScaleMode.ScaleAndCrop);
                            }
                            splitMode = GUI.Toggle(new Rect(0, 250, 20, 20), splitMode, "On");
                            if (GetComponent<InputManager>().isReady())
                            {
                                if (GUI.Button(new Rect(105, 64, 90, 70), "Start"))
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
            GUI.Box(new Rect(200, 112, 400, 225),messageText);
            if (GUI.Button(new Rect(200, 312, 400, 25), "Ok"))
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
