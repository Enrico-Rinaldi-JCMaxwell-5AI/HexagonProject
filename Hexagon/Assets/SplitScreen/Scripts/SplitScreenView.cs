using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitScreenView : MonoBehaviour {

    public GameObject mainCamera;
    public GameController gameController;
    public Texture2D endGameBG;
    public Texture2D outTexture;
    public Texture2D blue;
    public Texture page1;
    public Texture page2;
    public Texture page3;
    public GUIStyle invisibleButtons;
    public bool P1;
    public bool P2;
    public bool P3;
    public bool P4;
    public int pauseSelectedItem = 0;
    public int endGameSelectedItem = 0;
    public bool selMoving = false;
    public bool paused = false;
    public int[] joyNums;
    public bool hint;
    public int hintpage= 1;
    Texture2D bg;
    public GUIStyle textStyle;
    public GUIStyle outTextStyle;
    public Texture2D graybg;
    public GUIStyle endGameTextStyle;
    public GUIStyle restartButton;
    public GUIStyle homeButton;
    public GUIStyle optionButton;
    public GUIStyle resumeButton;
    public GUIStyle countdown;
    public Texture shield;

    // Use this for initialization
    void Start () {
        if(PlayerPrefs.GetInt("GHint")==1)
        {
            Time.timeScale = 0;
            hint = true;
        }
        mainCamera = GameObject.Find("Main Camera");
        joyNums = mainCamera.GetComponent<InputManager>().Joys;
        gameController = GetComponent<GameController>();
        endGameBG = new Texture2D(1, 1);
        endGameBG.SetPixel(0, 0, new Color(0.11f, 0.11f, 0.12f, 1));
        endGameBG.Apply();
        bg = new Texture2D(1, 1);
        bg.SetPixel(0, 0, new Color(0f, 0f, 0f, 1f));
        bg.Apply();
        blue = new Texture2D(1, 1);
        blue.SetPixel(0, 0, new Color(0f, 0f, 1f, 1f));
        blue.Apply();
        outTexture = new Texture2D(1, 1);
        outTexture.SetPixel(0, 0, new Color(0f, 0, 0, 1f));
        outTexture.Apply();
        P1 = true;
        P2 = true;
        if (mainCamera.GetComponent<InputManager>().numberOfPlayer() > 2)
            P3 = true;
        if (mainCamera.GetComponent<InputManager>().numberOfPlayer() == 4)
            P4 = true;
        outTextStyle = new GUIStyle();
        outTextStyle.fontSize = (int)Screen.width/10;
        outTextStyle.normal.textColor = new Color(1f, 1f, 1f, 1f);
        countdown.fontSize = (int)Screen.width / 5;
        countdown.normal.textColor = new Color(1f, 1f, 1f, 1f);
    }

    // Update is called once per frame

    void Update()
    {
        manageInput();
    }

    private void OnGUI()
    {
        if (hint)
        {
            switch(hintpage)
            {
                case 1:
                    {
                        GUI.DrawTexture(new Rect(Screen.width / 2 - 300, Screen.height / 2 - 225, 600, 450), page1);
                        if(GUI.Button(new Rect(Screen.width / 2 +95, Screen.height / 2 +130, 80, 80), "",invisibleButtons))
                        {
                            hintpage = 2;
                        }
                        if (GUI.Button(new Rect(Screen.width / 2 + 190, Screen.height / 2 + 130, 80, 80), "", invisibleButtons))
                        {
                            hintpage = 3;
                        }
                    }
                    break;
                case 2:
                    {
                        GUI.DrawTexture(new Rect(Screen.width / 2 - 300, Screen.height / 2 - 225, 600, 450), page2);
                        if (GUI.Button(new Rect(Screen.width / 2 + 95, Screen.height / 2 + 130, 80, 80), "", invisibleButtons))
                        {
                            hintpage = 3;
                        }
                        if (GUI.Button(new Rect(Screen.width / 2 - 175, Screen.height / 2 + 130, 80, 80), "",invisibleButtons))
                        {
                            hintpage = 1;
                        }
                    }
                    break;
                case 3:
                    {
                        GUI.DrawTexture(new Rect(Screen.width / 2 - 300, Screen.height / 2 - 225, 600, 450), page3);
                        if (GUI.Button(new Rect(Screen.width / 2 - 270, Screen.height / 2 + 130, 80, 80), "",invisibleButtons))
                        {
                            hintpage = 1;
                        }
                        if (GUI.Button(new Rect(Screen.width / 2 - 175, Screen.height / 2 + 130, 80, 80), "", invisibleButtons))
                        {
                            hintpage = 2;
                        }
                        if (GUI.Button(new Rect(Screen.width / 2 + 195, Screen.height / 2 + 132, 80, 80), "", invisibleButtons))
                        {
                            hint = false;
                            Time.timeScale = 1;
                        }
                    }
                    break;
            }
            
        }
        if (!mainCamera.GetComponent<MenuGUI>().optionMenù)
        {
            if (!GetComponent<GameController>().isGameFinished && mainCamera.GetComponent<MenuGUI>().splitMode)
            {

                if (!P1 && (mainCamera.GetComponent<InputManager>().numberOfPlayer() == 4 || mainCamera.GetComponent<InputManager>().numberOfPlayer() == 3))
                {
                    GUI.DrawTexture(new Rect(0, 0, Screen.width / 2, Screen.height / 2), outTexture);
                    GUI.Label(new Rect(Screen.width * 0.14f, Screen.height * 0.15f, Screen.width / 4, Screen.height / 4), "OUT",outTextStyle);
                }
                if (!P2 && (mainCamera.GetComponent<InputManager>().numberOfPlayer() == 4 || mainCamera.GetComponent<InputManager>().numberOfPlayer() == 3))
                {
                    GUI.DrawTexture(new Rect(Screen.width / 2, 0, Screen.width / 2, Screen.height / 2), outTexture);
                    GUI.Label(new Rect(Screen.width *0.65f, Screen.height*0.15f, Screen.width / 4, Screen.height / 4), "OUT", outTextStyle);
                }
                if (!P3 && mainCamera.GetComponent<InputManager>().numberOfPlayer() == 4)
                {
                    GUI.DrawTexture(new Rect(0, Screen.height / 2, Screen.width / 2, Screen.height / 2), outTexture);
                    GUI.Label(new Rect(Screen.width * 0.15f, Screen.height * 0.65f, Screen.width / 4, Screen.height / 4), "OUT", outTextStyle);
                }
                if (!P4 && mainCamera.GetComponent<InputManager>().numberOfPlayer() == 4)
                {
                    GUI.DrawTexture(new Rect(Screen.width / 2, Screen.height / 2, Screen.width / 2, Screen.height / 2), outTexture);
                    GUI.Label(new Rect(Screen.width * 0.65f, Screen.height * 0.65f, Screen.width / 4, Screen.height / 4), "OUT", outTextStyle);
                }
                if (!P3 && mainCamera.GetComponent<InputManager>().numberOfPlayer() == 3)
                {
                    GUI.DrawTexture(new Rect(0, Screen.height / 2, Screen.width, Screen.height / 2), outTexture);
                    GUI.Label(new Rect(Screen.width * 0.385f, Screen.height * 0.65f, Screen.width / 4, Screen.height / 4), "OUT", outTextStyle);
                }
            }
            if (paused)
            {
                if (GUI.Button(new Rect(Screen.width / 2 -165, Screen.height / 2 - 37, 75, 75), "",resumeButton))
                {
                    Time.timeScale = 1;
                    paused = false;
                }
                if (GUI.Button(new Rect(Screen.width / 2 -80, Screen.height / 2 - 37, 75, 75), "",optionButton))
                {
                    mainCamera.GetComponent<MenuGUI>().optionMenù = true;
                }
                if (GUI.Button(new Rect(Screen.width / 2 +5, Screen.height / 2-37, 75, 75), "",restartButton))
                {
                    gameController.resetMap();
                    mainCamera.GetComponent<MenuGUI>().instantiateSplit();
                    Time.timeScale = 1;
                    mainCamera.GetComponent<MenuGUI>().setScreenCameras();
                }
                if (GUI.Button(new Rect(Screen.width / 2 +90, Screen.height / 2 -37, 75, 75), "",homeButton))
                {
                    mainCamera.GetComponent<MenuGUI>().isMenuActive = true;
                    mainCamera.GetComponent<MenuGUI>().changeMenuState(0);
                    mainCamera.GetComponent<InputManager>().backReset();
                    gameController.resetMap();
                    Time.timeScale = 1;
                    mainCamera.GetComponent<Camera>().rect = new Rect(0, 0, 1, 1);
                    Screen.SetResolution(800, 450, false);
                }
            }
            else
            {
                if (!GetComponent<GameController>().isGameFinished && !hint)
                {
                    
                    if(gameController.gameTime>-3 && gameController.gameTime < -2)
                    {
                        GUI.Label(new Rect(0, 0, Screen.width, Screen.height), "3" ,countdown);
                    }
                    if (gameController.gameTime > -2 && gameController.gameTime < -1)
                    {
                        GUI.Label(new Rect(0, 0, Screen.width, Screen.height), "2", countdown);
                    }
                    if (gameController.gameTime > -1 && gameController.gameTime < 0)
                    {
                        GUI.Label(new Rect(0, 0, Screen.width, Screen.height), "1", countdown);
                    }
                    if (gameController.gameTime > 0 && gameController.gameTime < 1)
                    {
                        GUI.Label(new Rect(0, 0, Screen.width, Screen.height), "VIA!", countdown);
                    }
                    if (gameController.gameTime > 1)
                    {
                        string s0, s1, s2, s3;
                        if (!P1)
                            s0 = "OUT";
                        else
                            s0 = gameController.playersRemainingBalls[0].ToString();
                        if (!P2)
                            s1 = "OUT";
                        else
                            s1 = gameController.playersRemainingBalls[1].ToString();
                        if (!P3)
                            s2 = "OUT";
                        else
                            s2 = gameController.playersRemainingBalls[2].ToString();
                        if (!P4)
                            s3 = "OUT";
                        else
                            s3 = gameController.playersRemainingBalls[3].ToString();
                        /*if (!mainCamera.GetComponent<MenuGUI>().splitMode)
                        {
                            GUI.Label(new Rect(Screen.width / 2 - 50, 20, 100, 20), visualizeTime().ToString(), textStyle);
                            GUI.Label(new Rect(Screen.width / 2 - 50, 50, 100, 20), SplitScreenModel.getMultiplier(gameController.currentTime()).ToString("F2"), textStyle);
                            if (mainCamera.GetComponent<InputManager>().numberOfPlayer() == 2)
                            {
                                GUI.Label(new Rect(Screen.width / 4 - 50, 20, 100, 20), s0);
                                GUI.DrawTexture(new Rect(Screen.width / 4 - 50, 40, calculatePercent(0) / 2, 10), blue);
                                GUI.Label(new Rect(Screen.width * 0.75f - 50, 20, 100, 20), s1);
                                GUI.DrawTexture(new Rect(Screen.width * 0.75f, 40, calculatePercent(1) / 2, 10), blue);
                            }
                            if (mainCamera.GetComponent<InputManager>().numberOfPlayer() == 3)
                            {
                                GUI.Label(new Rect(Screen.width / 6 - 50, 20, 100, 20), s0);
                                GUI.DrawTexture(new Rect(Screen.width / 6 - 50, 40, calculatePercent(0) / 2, 10), blue);
                                GUI.Label(new Rect(Screen.width / 3 - 50, 20, 100, 20), s1);
                                GUI.DrawTexture(new Rect(Screen.width / 3 - 50, 40, calculatePercent(1) / 2, 10), blue);
                                GUI.Label(new Rect(Screen.width * 0.75f - 50, 20, 100, 20), s2);
                                GUI.DrawTexture(new Rect(Screen.width * 0.75f - 50, 40, calculatePercent(2) / 2, 10), blue);
                            }
                            if (mainCamera.GetComponent<InputManager>().numberOfPlayer() == 4)
                            {
                                GUI.Label(new Rect(Screen.width / 6 - 50, 20, 100, 20), s0);
                                GUI.DrawTexture(new Rect(Screen.width / 6 - 50, 40, calculatePercent(0) / 2, 10), blue);
                                GUI.Label(new Rect(Screen.width / 3 - 50, 20, 100, 20), s1);
                                GUI.DrawTexture(new Rect(Screen.width / 3 - 50, 40, calculatePercent(1) / 2, 10), blue);
                                GUI.Label(new Rect(Screen.width * 0.66f - 50, 20, 100, 20), s2);
                                GUI.DrawTexture(new Rect(Screen.width * 0.66f - 50, 40, calculatePercent(2) / 2, 10), blue);
                                GUI.Label(new Rect(Screen.width * 0.83f - 50, 20, 100, 20), s3);
                                GUI.DrawTexture(new Rect(Screen.width * 0.83f - 50, 40, calculatePercent(3) / 2, 10), blue);
                            }
                        }
                        else*/
                            GUI.DrawTexture(new Rect(0, Screen.height / 2 - 1, Screen.width, 2), bg);
                            if (mainCamera.GetComponent<InputManager>().numberOfPlayer() == 2)
                            {
                                GUI.Label(new Rect(Screen.width / 2 - 22, 5, 100, 20), visualizeTime().ToString(), textStyle);
                                GUI.Label(new Rect(Screen.width / 80, Screen.height/40, 100, 20), gameController.playersRemainingBalls[0].ToString(), textStyle);
                            GUI.DrawTexture(new Rect(Screen.width / 80, Screen.height / 40 + 22, 140, 42), shield);
                            GUI.DrawTexture(new Rect(Screen.width / 80, Screen.height / 1.9f +22, 140, 42), shield);
                            GUI.DrawTexture(new Rect(Screen.width / 80+1, Screen.height / 40 + 33, calculatePercent(0), 20), blue);
                            
                            GUI.Label(new Rect(Screen.width /80, Screen.height / 1.9f, 100, 20), gameController.playersRemainingBalls[1].ToString(), textStyle);
                                GUI.DrawTexture(new Rect(Screen.width /80 +1, Screen.height / 1.9f + 33, calculatePercent(1), 20), blue);
                            }
                            if (mainCamera.GetComponent<InputManager>().numberOfPlayer() == 3)
                            {
                                GUI.Label(new Rect(Screen.width / 2 - 22, Screen.height / 2, 100, 20), visualizeTime().ToString(), textStyle);
                                GUI.DrawTexture(new Rect(Screen.width / 2 - 1, 0, 2, Screen.height / 2), bg);
                                if (P1)
                                {
                                    GUI.Label(new Rect(Screen.width / 4 - 70, Screen.height / 40, 100, 20), gameController.playersRemainingBalls[0].ToString(),textStyle);
                                GUI.DrawTexture(new Rect(Screen.width / 4-20, Screen.height / 80, 140, 42), shield);
                                GUI.DrawTexture(new Rect(Screen.width / 4 - 19, Screen.height / 80+11, calculatePercent(0) , 20), blue);
                                }
                                if (P2)
                                {
                                    GUI.Label(new Rect(Screen.width * 0.75f -70, Screen.height / 40, 100, 20), gameController.playersRemainingBalls[1].ToString(),textStyle);
                                GUI.DrawTexture(new Rect(Screen.width *0.75f-20, Screen.height / 80, 140, 42), shield);
                                GUI.DrawTexture(new Rect(Screen.width * 0.75f - 19, Screen.height / 80+11, calculatePercent(1) , 20), blue);
                                }
                                if (P3)
                                {
                                    GUI.Label(new Rect(Screen.width / 80, Screen.height / 1.9f, 100, 20), gameController.playersRemainingBalls[2].ToString(),textStyle);
                                GUI.DrawTexture(new Rect(Screen.width / 80, Screen.height / 1.9f + 22, 140, 42), shield);
                                GUI.DrawTexture(new Rect(Screen.width / 80, Screen.height / 1.9f+33, calculatePercent(2), 20), blue);
                                }
                            }
                            if (mainCamera.GetComponent<InputManager>().numberOfPlayer() == 4)
                            {
                            GUI.DrawTexture(new Rect(Screen.width / 2 - 1, 0, 2, Screen.height), bg);
                            GUI.Label(new Rect(Screen.width / 2 - 22, Screen.height / 2, 100, 20), visualizeTime().ToString(), textStyle);
                            if (P1)
                                {
                                GUI.Label(new Rect(Screen.width / 4 - 70, Screen.height / 40, 100, 20), gameController.playersRemainingBalls[0].ToString(), textStyle);
                                GUI.DrawTexture(new Rect(Screen.width / 4 - 20, Screen.height / 80, 140, 42), shield);
                                GUI.DrawTexture(new Rect(Screen.width / 4 - 19, Screen.height / 80 + 11, calculatePercent(0), 20), blue);
                            }
                                if (P2)
                                {
                                GUI.Label(new Rect(Screen.width * 0.75f - 70, Screen.height / 40, 100, 20), gameController.playersRemainingBalls[1].ToString(), textStyle);
                                GUI.DrawTexture(new Rect(Screen.width * 0.75f - 20, Screen.height / 80, 140, 42), shield);
                                GUI.DrawTexture(new Rect(Screen.width * 0.75f - 19, Screen.height / 80 + 11, calculatePercent(1), 20), blue);
                            }
                                if (P3)
                                {
                                    GUI.Label(new Rect(Screen.width / 4 - 70, Screen.height / 1.9f, 100, 20), gameController.playersRemainingBalls[2].ToString(),textStyle);
                                GUI.DrawTexture(new Rect(Screen.width / 4 - 20, Screen.height / 1.9f-7, 140, 42), shield);
                                GUI.DrawTexture(new Rect(Screen.width / 4 - 19, Screen.height / 1.9f+4, calculatePercent(2), 20), blue);
                                }
                                if (P4)
                                {
                                    GUI.Label(new Rect(Screen.width * 0.75f - 70, Screen.height / 1.9f, 100, 20), gameController.playersRemainingBalls[3].ToString(),textStyle);
                                GUI.DrawTexture(new Rect(Screen.width * 0.75f - 20, Screen.height / 1.9f-7, 140, 42), shield);
                                GUI.DrawTexture(new Rect(Screen.width * 0.75f - 19, Screen.height / 1.9f+4, calculatePercent(3), 20), blue);
                                }
                            }

                        
                    }
                }
                else
                {
                    if (!hint)
                    {
                        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), endGameBG);
                        string text = "PLAYER  " + (gameController.winner + 1) + " WINS!";// + " Balls remaining: " + gameController.playersRemainingBalls[gameController.winner] + "\n";
                        /*for (int i = gameController.scoreboard.Count - 1; i >= 0; i--)
                        {
                            text = text + " Player: " + (gameController.scoreboard[i].player + 1) + " Time: " + gameController.scoreboard[i].time + "\n";
                        }*/

                        endGameTextStyle.fontSize = Screen.width / 9;
                        GUI.Label(new Rect(0, Screen.height / 15, Screen.width, Screen.height/2), text,endGameTextStyle);
                        endGameTextStyle.fontSize = Screen.width / 18;
                        GUI.Label(new Rect(0, Screen.height / 3.15f, Screen.width, Screen.height / 2), "With " + gameController.playersRemainingBalls[gameController.winner] + " ball(s)!",endGameTextStyle);
                        endGameTextStyle.fontSize = Screen.width / 40;
                        GUI.Label(new Rect(0, Screen.height / 1.8f, Screen.width, Screen.height / 2), "Total game time: " + Mathf.Round(gameController.totalGameTime) + "s\nSpeed multiplier reached: "+System.Math.Round(SplitScreenModel.getMultiplier(gameController.totalGameTime),2)+"x", endGameTextStyle);
                        if (GUI.Button(new Rect(Screen.width / 2-80, Screen.height / 1.3f, 75, 75), "",restartButton))
                        {
                            gameController.resetMap();
                            mainCamera.GetComponent<MenuGUI>().instantiateSplit();
                            Time.timeScale = 1;
                            mainCamera.GetComponent<MenuGUI>().setScreenCameras();
                        }
                        if (GUI.Button(new Rect(Screen.width / 2 + 5, Screen.height / 1.3f, 75, 75), "",homeButton))
                        {
                            mainCamera.GetComponent<MenuGUI>().isMenuActive = true;
                            mainCamera.GetComponent<MenuGUI>().changeMenuState(0);
                            mainCamera.GetComponent<InputManager>().backReset();
                            gameController.resetMap();
                            Time.timeScale = 1;
                            mainCamera.GetComponent<Camera>().rect = new Rect(0, 0, 1, 1);
                            Screen.SetResolution(800, 450, false);
                        }
                    }
                }
            }
        }
    
}

    public int calculatePercent(int player)
    {
        if(gameController.cooldowns[player].shieldTime<= gameController.currentTime())
        {
            return 100;
        }else
        {
            return 100 - (int)Mathf.Ceil((gameController.cooldowns[player].shieldTime - gameController.currentTime()) * 6.66f);
        }
    }

    public string visualizeTime()
    {
        int ctime = (int)gameController.currentTime();
        string time = "";
        if (gameController.currentTime() < 0)
        {
            time = "-";
            ctime = ctime * -1;
        }
        time = time + Mathf.Floor(ctime / 60).ToString()+":";
        if ((int)gameController.currentTime() % 60 < 10)
            time = time + "0" + ctime % 60;
        else
            time = time + ctime % 60;
        return time;
    }

    public void inGamePlayerChange(int n)
    {
        switch(n)
        {
            case 0:
                {
                    mainCamera.GetComponent<Camera>().rect = new Rect(0, 0, 0, 0);
                    P1 = false;
                }break;
            case 1:
                {
                    GameObject.Find("AuxFront").GetComponent<Camera>().rect = new Rect(0, 0, 0, 0);
                    P2 = false;
                }
                break;
            case 2:
                {
                    GameObject.Find("AuxLeft").GetComponent<Camera>().rect = new Rect(0, 0, 0, 0);
                    P3 = false;
                }
                break;
            case 3:
                {
                    GameObject.Find("AuxRight").GetComponent<Camera>().rect = new Rect(0, 0, 0, 0);
                    P4 = false;
                }
                break;
        }

    }

    void manageInput()
    {
        if (hint && Input.GetKeyDown("joystick "+joyNums[0]+" button "+InputPreferences.getInput(1,0)))
        {
            hint = false;
            Time.timeScale = 1;
        }
        if (Input.GetKeyDown("joystick " + joyNums[0] + " button " + InputPreferences.getInput(1, 7)) || Input.GetKeyDown(KeyCode.Escape))
        {
            if (!paused)
            {
                Time.timeScale = 0;
                paused = true;
            }
            else
            {
                Time.timeScale = 1;
                paused = false;
            }
        }
        if (Input.GetAxis("Vertical") == 0)
        {
            selMoving = false;
        }
        if (gameController.isGameFinished)
        {
            if (Input.GetAxis("Vertical") == 1 && !selMoving && endGameSelectedItem < 1)
            {
                endGameSelectedItem++;
                selMoving = true;
            }
            if (Input.GetAxis("Vertical") == -1 && !selMoving && endGameSelectedItem > 0)
            {
                endGameSelectedItem--;
                selMoving = true;
            }
            if(Input.GetKeyDown("joystick " + joyNums[0] + " button " + InputPreferences.getInput(1, 0)) && endGameSelectedItem ==0)
            {
                gameController.resetMap();
                GameObject.Find("Main Camera").GetComponent<MenuGUI>().instantiateSplit();
                Time.timeScale = 1;
                GameObject.Find("Main Camera").GetComponent<MenuGUI>().setScreenCameras();
            }
            if (Input.GetKeyDown("joystick " + joyNums[0] + " button " + InputPreferences.getInput(1, 0)) && endGameSelectedItem == 1)
            {
                GameObject.Find("Main Camera").GetComponent<MenuGUI>().isMenuActive = true;
                mainCamera.GetComponent<MenuGUI>().changeMenuState(0);
                GameObject.Find("Main Camera").GetComponent<InputManager>().backReset();
                gameController.resetMap();
                Time.timeScale = 1;
                GameObject.Find("Main Camera").GetComponent<Camera>().rect = new Rect(0, 0, 1, 1);
            }

        }
        if (paused)
        {
            if (Input.GetAxis("Vertical") == 1 && !selMoving && pauseSelectedItem < 3)
            {
                pauseSelectedItem++;
                selMoving = true;
            }
            if (Input.GetAxis("Vertical") == -1 && !selMoving && pauseSelectedItem > 0)
            {
                pauseSelectedItem--;
                selMoving = true;
            }
            if (Input.GetKeyDown("joystick " + joyNums[0] + " button " + InputPreferences.getInput(1, 0)))
            {
                switch (pauseSelectedItem)
                {
                    case 0:
                        {
                            Time.timeScale = 1;
                            paused = false;
                        }
                        break;
                    case 1:
                        {
                            mainCamera.GetComponent<MenuGUI>().optionMenù = true;
                        }
                        break;
                    case 2:
                        {
                            gameController.resetMap();
                            GameObject.Find("Main Camera").GetComponent<MenuGUI>().instantiateSplit();
                            Time.timeScale = 1;
                            GameObject.Find("Main Camera").GetComponent<MenuGUI>().setScreenCameras();
                        }
                        break;
                    case 3:
                        {
                            GameObject.Find("Main Camera").GetComponent<MenuGUI>().isMenuActive = true;
                            mainCamera.GetComponent<MenuGUI>().changeMenuState(1);
                            GameObject.Find("Main Camera").GetComponent<InputManager>().backReset();
                            gameController.resetMap();
                            Time.timeScale = 1;
                            GameObject.Find("Main Camera").GetComponent<Camera>().rect = new Rect(0, 0, 1, 1);
                            Screen.SetResolution(800, 450, false);
                        }
                        break;
                }
            }
        }
        else
        {
            for (int j = 0; j < 4; j++)
            {
                for (int i = 1; i < 12; i++)
                {
                    if (Input.GetKeyDown("joystick " + i + " button " + InputPreferences.getInput(j+1, 0)))
                    {
                        if (joyNums[j] != -1 && joyNums[j] == i)
                        {
                            gameController.useAbility(j, 0);
                        }
                    }
                    if (Input.GetKeyDown("joystick " + i + " button " + InputPreferences.getInput(j+1, 2)))
                    {
                        if (joyNums[j] != -1 && joyNums[j] == i)
                        {
                            gameController.useAbility(j, 1);
                        }
                    }
                }
            }
        }

    }
}
