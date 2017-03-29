using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitScreenView : MonoBehaviour {

    public GameObject mainCamera;
    public GameController gameController;
    public Texture2D endGameBG;
    public Texture2D outTexture;
    public Texture2D blue;
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
    Texture2D bg;
    public GUIStyle textStyle;

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
        endGameBG.SetPixel(0, 0, new Color(0.52f, 0.9f, 1f, 0.66f));
        endGameBG.Apply();
        bg = new Texture2D(1, 1);
        bg.SetPixel(0, 0, new Color(0f, 0f, 0f, 1f));
        bg.Apply();
        blue = new Texture2D(1, 1);
        blue.SetPixel(0, 0, new Color(0f, 0f, 1f, 1f));
        blue.Apply();
        outTexture = new Texture2D(1, 1);
        outTexture.SetPixel(0, 0, new Color(1f, 0, 0, 1f));
        outTexture.Apply();
        P1 = true;
        P2 = true;
        if (mainCamera.GetComponent<InputManager>().numberOfPlayer() > 2)
            P3 = true;
        if (mainCamera.GetComponent<InputManager>().numberOfPlayer() == 4)
            P4 = true;

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
            if (GUI.Button(new Rect(100, 100, 100, 100), "Proceed"))
            {
                hint = false;
                Time.timeScale = 1;
            }
        }
        if (!mainCamera.GetComponent<MenuGUI>().optionMenù)
        {
            if (!GetComponent<GameController>().isGameFinished && mainCamera.GetComponent<MenuGUI>().splitMode)
            {

                if (!P1 && (mainCamera.GetComponent<InputManager>().numberOfPlayer() == 4 || mainCamera.GetComponent<InputManager>().numberOfPlayer() == 3))
                    GUI.DrawTexture(new Rect(0, 0, Screen.width / 2, Screen.height / 2), outTexture);
                if (!P2 && (mainCamera.GetComponent<InputManager>().numberOfPlayer() == 4 || mainCamera.GetComponent<InputManager>().numberOfPlayer() == 3))
                    GUI.DrawTexture(new Rect(Screen.width / 2, 0, Screen.width / 2, Screen.height / 2), outTexture);
                if (!P3 && mainCamera.GetComponent<InputManager>().numberOfPlayer() == 4)
                    GUI.DrawTexture(new Rect(0, Screen.height / 2, Screen.width / 2, Screen.height / 2), outTexture);
                if (!P4 && mainCamera.GetComponent<InputManager>().numberOfPlayer() == 4)
                    GUI.DrawTexture(new Rect(Screen.width / 2, Screen.height / 2, Screen.width / 2, Screen.height / 2), outTexture);
                if (!P3 && mainCamera.GetComponent<InputManager>().numberOfPlayer() == 3)
                    GUI.DrawTexture(new Rect(0, Screen.height / 2, Screen.width, Screen.height / 2), outTexture);
            }
            if (paused)
            {
                if (GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 100, 200, 40), "Resume"))
                {
                    Time.timeScale = 1;
                    paused = false;
                }
                if (GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 50, 200, 40), "Options"))
                {
                    mainCamera.GetComponent<MenuGUI>().optionMenù = true;
                }
                if (GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height / 2, 200, 40), "Restart"))
                {
                    gameController.resetMap();
                    mainCamera.GetComponent<MenuGUI>().instantiateSplit();
                    Time.timeScale = 1;
                    mainCamera.GetComponent<MenuGUI>().setScreenCameras();
                }
                if (GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height / 2 + 50, 200, 40), "Main Menu"))
                {
                    mainCamera.GetComponent<MenuGUI>().isMenuActive = true;
                    if (!mainCamera.GetComponent<MenuGUI>().logged)
                    {
                        mainCamera.GetComponent<MenuGUI>().changeMenuState(1);
                    }
                    mainCamera.GetComponent<InputManager>().backReset();
                    gameController.resetMap();
                    Time.timeScale = 1;
                    mainCamera.GetComponent<Camera>().rect = new Rect(0, 0, 1, 1);
                    Screen.SetResolution(800, 450, false);
                }
            }
            else
            {
                if (!GetComponent<GameController>().isGameFinished)
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
                    if (!mainCamera.GetComponent<MenuGUI>().splitMode)
                    {
                        GUI.Label(new Rect(Screen.width / 2 - 50, 20, 100, 20), visualizeTime().ToString(),textStyle);
                        GUI.Label(new Rect(Screen.width / 2 - 50, 50, 100, 20), SplitScreenModel.getMultiplier(gameController.currentTime()).ToString("F2"), textStyle);
                        if (mainCamera.GetComponent<InputManager>().numberOfPlayer() == 2)
                        {
                            GUI.Label(new Rect(Screen.width / 4 - 50, 20, 100, 20), s0);
                            GUI.DrawTexture(new Rect(Screen.width / 4 - 50, 40, calculatePercent(0)/2, 10), blue);
                            GUI.Label(new Rect(Screen.width * 0.75f - 50, 20, 100, 20), s1);
                            GUI.DrawTexture(new Rect(Screen.width * 0.75f, 40, calculatePercent(1)/2, 10), blue);
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
                    else
                    {
                        GUI.DrawTexture(new Rect(0, Screen.height / 2 - 1, Screen.width, 2), bg);
                        if (mainCamera.GetComponent<InputManager>().numberOfPlayer() == 2)
                        {
                            GUI.Label(new Rect(Screen.width / 2 - 50, 5, 100, 20), visualizeTime().ToString(), textStyle);
                            GUI.Label(new Rect(Screen.width / 2 - 50, 35, 100, 20), SplitScreenModel.getMultiplier(gameController.currentTime()).ToString("F2"), textStyle);
                            GUI.Label(new Rect(Screen.width / 2 - 50, 20, 100, 20), gameController.playersRemainingBalls[0].ToString());
                            GUI.DrawTexture(new Rect(Screen.width / 2 - 50, 70, calculatePercent(0) / 2, 10), blue);
                            GUI.Label(new Rect(Screen.width * 0.75f - 50, Screen.height / 2, 100, 20), gameController.playersRemainingBalls[1].ToString());
                            GUI.DrawTexture(new Rect(Screen.width *0.75f -50, 70, calculatePercent(1) / 2, 10), blue);
                        }
                        if (mainCamera.GetComponent<InputManager>().numberOfPlayer() == 3)
                        {
                            GUI.Label(new Rect(Screen.width / 2 - 50, Screen.height / 2, 100, 20), visualizeTime().ToString(), textStyle);
                            GUI.Label(new Rect(Screen.width / 2 - 50, Screen.height / 2 + 30, 100, 20), SplitScreenModel.getMultiplier(gameController.currentTime()).ToString("F2"), textStyle);
                            GUI.DrawTexture(new Rect(Screen.width/2-1, 0, 2, Screen.height / 2), bg);
                            if (P1)
                            {
                                GUI.Label(new Rect(Screen.width / 4 - 50, 10, 100, 20), gameController.playersRemainingBalls[0].ToString());
                                GUI.DrawTexture(new Rect(Screen.width / 4 - 50, 40, calculatePercent(0) / 2, 10), blue);
                            }
                            if (P2)
                            {
                                GUI.Label(new Rect(Screen.width * 0.75f - 50, 10, 100, 20), gameController.playersRemainingBalls[1].ToString());
                                GUI.DrawTexture(new Rect(Screen.width *0.75f - 50, 40, calculatePercent(1) / 2, 10), blue);
                            }
                            if (P3)
                            {
                                GUI.Label(new Rect(Screen.width / 2 - 200, Screen.height / 2, 100, 20), gameController.playersRemainingBalls[2].ToString());
                                GUI.DrawTexture(new Rect(Screen.width / 2 - 200, Screen.height / 2 + 40, calculatePercent(2) / 2, 10), blue);
                            }
                        }
                        if (mainCamera.GetComponent<InputManager>().numberOfPlayer() == 4)
                        {
                            GUI.Label(new Rect(Screen.width / 2 - 50, Screen.height / 2, 100, 20), visualizeTime().ToString(), textStyle);
                            GUI.Label(new Rect(Screen.width / 2 - 50, Screen.height / 2 + 30, 100, 20), SplitScreenModel.getMultiplier(gameController.currentTime()).ToString("F2"), textStyle);
                            GUI.DrawTexture(new Rect(Screen.width / 2 - 1, 0, 2, Screen.height), bg);
                            if (P1)
                            {
                                GUI.Label(new Rect(Screen.width / 4 - 50, 40, 100, 20), gameController.playersRemainingBalls[0].ToString());
                                GUI.DrawTexture(new Rect(Screen.width / 4 - 50, 40, calculatePercent(0) / 2, 10), blue);
                            }
                            if (P2)
                            {
                                GUI.Label(new Rect(Screen.width * 0.75f - 50, 40, 100, 20), gameController.playersRemainingBalls[1].ToString());
                                GUI.DrawTexture(new Rect(Screen.width * 0.75f - 50, 40, calculatePercent(1) / 2, 10), blue);
                            }
                            if (P3)
                            {
                                GUI.Label(new Rect(Screen.width / 4 - 50, Screen.height / 2, 100, 20), gameController.playersRemainingBalls[2].ToString());
                                GUI.DrawTexture(new Rect(Screen.width / 4 - 50, Screen.height / 2 + 40, calculatePercent(2) / 2, 10), blue);
                            }
                            if (P4)
                            {
                                GUI.Label(new Rect(Screen.width * 0.75f - 50, Screen.height / 2, 100, 20), gameController.playersRemainingBalls[3].ToString());
                                GUI.DrawTexture(new Rect(Screen.width *0.75f - 50, Screen.height / 2 + 40, calculatePercent(2) / 2, 10), blue);
                            }
                        }

                    }
                }
                else
                {
                    if (gameController.currentTime() > gameController.totalGameTime + 1.5)
                    {
                        GUI.DrawTexture(new Rect(Screen.width / 2 - 350f, Screen.height / 2 - 250, 700, 500), endGameBG);
                        string text = "WINNER PLAYER " + (gameController.winner + 1) + " Balls remaining: " + gameController.playersRemainingBalls[gameController.winner] + "\n";
                        for (int i = gameController.scoreboard.Count - 1; i >= 0; i--)
                        {
                            text = text + " Player: " + (gameController.scoreboard[i].player + 1) + " Time: " + gameController.scoreboard[i].time + "\n";
                        }
                        GUI.Label(new Rect(Screen.width / 2 - 350f, Screen.height / 2 - 250, 700, 450), text);
                        if (GUI.Button(new Rect(Screen.width / 2 - 350f, Screen.height / 2 + 140, 700, 50), "Restart"))
                        {
                            gameController.resetMap();
                            mainCamera.GetComponent<MenuGUI>().instantiateSplit();
                            Time.timeScale = 1;
                            mainCamera.GetComponent<MenuGUI>().setScreenCameras();
                        }
                        if (GUI.Button(new Rect(Screen.width / 2 - 350f, Screen.height / 2 + 200, 700, 50), "Main Menu"))
                        {
                            mainCamera.GetComponent<MenuGUI>().isMenuActive = true;
                            if (!mainCamera.GetComponent<MenuGUI>().logged)
                            {
                                mainCamera.GetComponent<MenuGUI>().changeMenuState(1);
                            }
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
                if (!GameObject.Find("Main Camera").GetComponent<MenuGUI>().logged)
                {
                    mainCamera.GetComponent<MenuGUI>().changeMenuState(1);
                }
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
                            if (!GameObject.Find("Main Camera").GetComponent<MenuGUI>().logged)
                            {
                                mainCamera.GetComponent<MenuGUI>().changeMenuState(1);
                            }
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
