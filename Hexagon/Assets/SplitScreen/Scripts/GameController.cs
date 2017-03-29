using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    public List<Scoreboard> scoreboard;
    public int[] playersRemainingBalls;
    int numberOfPlayer;
    public int ballsInGame = 0;
    public Object ball;
    public Object navPrefab;
    public Object SBomb;
    public Object DBomb;
    public Object Shield;
    public Object Repulsion;
    public Material Red;
    public Material Blue;
    public Material Green;
    public Material LedRed;
    public Material LedBlue;
    public Material LedOff;
    public List<GameObject> inGameObjects = new List<GameObject>();
    public GameObject[] navs;
    public float[] destination;
    public bool isGameFinished = false;
    public int lastSPort=-1;
    public List<Bomb> bombTimes;
    public float totalGameTime;
    public bool isCentralCannonUp = false;
    public int centralCannonDegrees = -1;
    public bool centralCannonStop = false;
    public int leftRotations = 3;
    public int nextBallChangeTime = -1;
    public bool areBallsRest = false;
    public List<Vector3> ballsSpeed;
    public float gameTime;
    public Abilities[] cooldowns;
    GameObject mainCamera;
    public int winner;

    void Start () {
        totalGameTime = -1;
        GameObject.Find("BackPlane").GetComponent<Transform>().position = new Vector3(0, -51.86f, -4.71f);
        GameObject.Find("FrontPlane").GetComponent<Transform>().position = new Vector3(0, -51.86f, 4.71f);
        GameObject.Find("FrontBase").GetComponent<Renderer>().material = Blue;
        GameObject.Find("BackBase").GetComponent<Renderer>().material = Blue;
        destination = new float[4];
        playersRemainingBalls = new int[4];
        playersRemainingBalls[0] = SplitScreenModel.maxStartBalls;
        playersRemainingBalls[1] = SplitScreenModel.maxStartBalls;
        playersRemainingBalls[2] = -1;
        playersRemainingBalls[3] = -1;
        gameTime = -SplitScreenModel.preGameSeconds;
        mainCamera = GameObject.Find("Main Camera");
        numberOfPlayer = mainCamera.GetComponent<InputManager>().numberOfPlayer();
        navs = new GameObject[4];
        navs[0] = (GameObject)Instantiate(navPrefab, new Vector3(0, 0, -6), Quaternion.Euler(0,180,0));
        navs[0].name = "P0";
        navs[1] = (GameObject)Instantiate(navPrefab, new Vector3(0, 0, 6), Quaternion.Euler(0, 0, 0));
        navs[1].name = "P1";
        nextBallChangeTime = Random.Range(190, 221);
        cooldowns = new Abilities[numberOfPlayer];
        cooldowns[0] = new Abilities();
        cooldowns[1] = new Abilities();
        scoreboard = new List<Scoreboard>();
        if (numberOfPlayer >= 3)
        {
            playersRemainingBalls[2] = SplitScreenModel.maxStartBalls;
            navs[2] = (GameObject)Instantiate(navPrefab, new Vector3(-6, 0, 0), Quaternion.Euler(0, 270, 0));
            navs[2].name = "P2";
            GameObject.Find("LeftPlane").GetComponent<Transform>().position = new Vector3(-4.72f, -51.86f, 0);
            GameObject.Find("LeftBase").GetComponent<Renderer>().material = Blue;
            cooldowns[2] = new Abilities();
        }
        if (numberOfPlayer == 4)
        {
            playersRemainingBalls[3] = SplitScreenModel.maxStartBalls;
            navs[3] = (GameObject)Instantiate(navPrefab, new Vector3(6, 0, 0), Quaternion.Euler(0, 90, 0));
            navs[3].name = "P3";
            GameObject.Find("RightPlane").GetComponent<Transform>().position = new Vector3(4.72f, -51.86f, 0);
            GameObject.Find("RightBase").GetComponent<Renderer>().material = Blue;
            cooldowns[3] = new Abilities();
        }
        bombTimes = SplitScreenModel.mapExplosions();
        GameObject.Find("Reflection Probe").GetComponent<ReflectionProbe>().RenderProbe();
    }

    public void decreaseBall(int port)
    {
        playersRemainingBalls[port]--;
    }

    public void resetMap()
    {
        for(int i=0;i<4;i++)
        {
            if(navs[i]!=null)
            {
                Destroy(navs[i]);
            }
        }
        
        GameObject.Find("LeftPlane").GetComponent<Transform>().position = new Vector3(-6f, 2.375f, 0);
        GameObject.Find("RightPlane").GetComponent<Transform>().position = new Vector3(6f, 2.375f, 0);
        GameObject.Find("Turret").GetComponent<Transform>().position = new Vector3(0, -1f, 0);
        GameObject.Find("Turret").GetComponent<Transform>().rotation = Quaternion.Euler(0, 0, 0);
        GameObject.Find("Cannon 1").GetComponent<Transform>().rotation = Quaternion.Euler(0, 45, 0);
        GameObject.Find("Cannon 2").GetComponent<Transform>().rotation = Quaternion.Euler(0, -45, 0);
        GameObject.Find("Cannon 3").GetComponent<Transform>().rotation = Quaternion.Euler(0, 45, 0);
        GameObject.Find("Cannon 4").GetComponent<Transform>().rotation = Quaternion.Euler(0, -45, 0);
        GameObject.Find("FrontBase").GetComponent<Renderer>().material = Red;
        GameObject.Find("BackBase").GetComponent<Renderer>().material = Red;
        GameObject.Find("LeftBase").GetComponent<Renderer>().material = Red;
        GameObject.Find("RightBase").GetComponent<Renderer>().material = Red;
        GameObject.Find("SO Down Led").transform.GetChild(0).GetComponent<Renderer>().material = LedOff;
        GameObject.Find("NO Down Led").transform.GetChild(0).GetComponent<Renderer>().material = LedOff;
        GameObject.Find("NE Down Led").transform.GetChild(0).GetComponent<Renderer>().material = LedOff;
        GameObject.Find("SE Down Led").transform.GetChild(0).GetComponent<Renderer>().material = LedOff;
        GameObject.Find("Circle").transform.GetChild(0).GetComponent<Renderer>().material = LedOff;
        GameObject.Find("Main Camera").GetComponent<Camera>().fieldOfView = 64f;
        GameObject.Find("AuxFront").GetComponent<Camera>().fieldOfView = 64f;
        GameObject.Find("AuxLeft").GetComponent<Camera>().fieldOfView = 64f;

        for (int i = 0; i < inGameObjects.Count; i++)
        {
            Destroy(inGameObjects[i]);
        }
        //DOPO TUTTE LE OPERAZIONI
        Destroy(gameObject);
        GameObject.Find("Reflection Probe").GetComponent<ReflectionProbe>().RenderProbe();

    }

	// Update is called once per frame
	void Update () {
        gameTime = gameTime + Time.deltaTime;
        if (!isGameFinished)
        {
            manageEvents();
        }
	}

    void manageExplosion()
    {
        for(int i=0;i<bombTimes.Count;i++)
        {
            if((int)currentTime() == bombTimes[i].spawnTime)
            {
                //Spawna la bomba
                GameObject bomb;
                if (bombTimes[i].isStatic)
                    bomb = (GameObject)Instantiate(SBomb, new Vector3(Random.Range(-3.5f,3.5f), 5f, Random.Range(-3.51f, 3.51f)), Quaternion.identity);
                else
                    bomb = (GameObject)Instantiate(DBomb, new Vector3(Random.Range(-3.5f, 3.5f), 5f, Random.Range(-3.51f, 3.51f)), Quaternion.identity);
                bombTimes.RemoveAt(i);
                inGameObjects.Add(bomb);
                if (currentTime() > 300)
                {
                    bombTimes.Add(new Bomb(Random.Range((int)currentTime() + SplitScreenModel.endGameBombIntervalMin, (int)currentTime() + SplitScreenModel.endGameBombIntervalMax),false));
                }
            }
        }
    }

    

    public void useAbility(int player, int codAbility)
    {
        if (codAbility == 0 && cooldowns[player].repulsionTime <= currentTime())    //Repulsion
        {
            //Do Repulsion Ability
            GameObject repulsion;
            repulsion = (GameObject)Instantiate(Repulsion);
            repulsion.transform.parent = navs[player].transform;
            repulsion.GetComponent<Transform>().position = navs[player].transform.position;
            cooldowns[player].repulsionTime = currentTime() + SplitScreenModel.repulsionCooldown;
            //QUALSIASI COSA AGGIUNGI IN GAME METTILA NELLA LISTA inGameObjects. SERVE A PREVENIRE CHE QUANDO SI RESETTA IL GAME LE COSE CHE SPAWNI RIMANGONO
        }
        if (codAbility == 1 && cooldowns[player].shieldTime <= currentTime())   //Shield
        {
            GameObject shield;
            shield = (GameObject)Instantiate(Shield);
            shield.transform.parent = navs[player].transform;
            shield.GetComponent<Transform>().position = navs[player].transform.position;
            shield.GetComponent<ShieldScript>().endTime = currentTime() + SplitScreenModel.shieldDuration;
            cooldowns[player].shieldTime = currentTime() + SplitScreenModel.shieldCooldown;
        }
    }

    void manageEvents()
    {
        if (nextBallChangeTime == (int)currentTime())
        {
            startBallChangeDirection();
        }
        if (ballsInGame < SplitScreenModel.maxBalls((int)currentTime()) && !isCentralCannonUp && !areBallsRest && nextBallChangeTime - currentTime() > 2)
        {
            spawnBall();
        }
        if (playersRemainingBalls[0] == 0)
        {
            Destroy(navs[0]);
            navs[0] = null;
            GameObject.Find("BackPlane").GetComponent<Transform>().position = new Vector3(0, 2.375f, -6f);
            GameObject.Find("BackBase").GetComponent<Renderer>().material = Red;
            playersRemainingBalls[0] = -1;
            numberOfPlayer--;
            GameObject.Find("Reflection Probe").GetComponent<ReflectionProbe>().RenderProbe();
            scoreboard.Add(new Scoreboard(0, currentTime()));
            GetComponent<SplitScreenView>().inGamePlayerChange(0);
        }
        if (playersRemainingBalls[1] == 0)
        {
            Destroy(navs[1]);
            navs[1] = null;
            GameObject.Find("FrontPlane").GetComponent<Transform>().position = new Vector3(0, 2.375f, 6f);
            GameObject.Find("FrontBase").GetComponent<Renderer>().material = Red;
            playersRemainingBalls[1] = -1;
            numberOfPlayer--;
            GameObject.Find("Reflection Probe").GetComponent<ReflectionProbe>().RenderProbe();
            scoreboard.Add(new Scoreboard(1, currentTime()));
            GetComponent<SplitScreenView>().inGamePlayerChange(1);
        }
        if (playersRemainingBalls[2] == 0)
        {
            Destroy(navs[2]);
            navs[2] = null;
            GameObject.Find("LeftPlane").GetComponent<Transform>().position = new Vector3(-6f, 2.375f, 0);
            GameObject.Find("LeftBase").GetComponent<Renderer>().material = Red;
            playersRemainingBalls[2] = -1;
            numberOfPlayer--;
            GameObject.Find("Reflection Probe").GetComponent<ReflectionProbe>().RenderProbe();
            scoreboard.Add(new Scoreboard(2, currentTime()));
            GetComponent<SplitScreenView>().inGamePlayerChange(2);
        }
        if (playersRemainingBalls[3] == 0)
        {
            Destroy(navs[3]);
            navs[3] = null;
            GameObject.Find("RightPlane").GetComponent<Transform>().position = new Vector3(6f, 2.375f, 0);
            GameObject.Find("RightBase").GetComponent<Renderer>().material = Red;
            playersRemainingBalls[3] = -1;
            numberOfPlayer--;
            GameObject.Find("Reflection Probe").GetComponent<ReflectionProbe>().RenderProbe();
            scoreboard.Add(new Scoreboard(3, currentTime()));
            GetComponent<SplitScreenView>().inGamePlayerChange(3);
        }
        if(numberOfPlayer == 1)
        {
            endGame();
        }
        manageExplosion();
    }

    void manageCannons()
    {
        for (int i = 0; i < 4; i++)
        {
            GameObject currentCannon = GameObject.Find("Cannon " + (i + 1));
            if (destination[i] == 0)
            {
                calculateDestination(i);
            }
            if (currentCannon.GetComponent<Transform>().rotation.eulerAngles.y < destination[i])
                currentCannon.GetComponent<Transform>().rotation = Quaternion.Euler(0, currentCannon.transform.rotation.eulerAngles.y + SplitScreenModel.degreesSpeedLateralCannonRotation, 0);
            else
                currentCannon.GetComponent<Transform>().rotation = Quaternion.Euler(0, currentCannon.transform.rotation.eulerAngles.y - SplitScreenModel.degreesSpeedLateralCannonRotation, 0);

            if (Mathf.Abs(currentCannon.transform.rotation.eulerAngles.y - destination[i]) < SplitScreenModel.degreesSpeedLateralCannonRotation*4)
                calculateDestination(i);

        }
    }

    void endGame()
    {
        isGameFinished = true;
        totalGameTime = currentTime();
        GameObject.Find("Main Camera").GetComponent<Camera>().rect = new Rect(0, 0, 1, 1);
        if (playersRemainingBalls[0] != -1)
        {
            GameObject.Find("BackBase").GetComponent<Renderer>().material = Green;
            winner = 0;
        }
        if (playersRemainingBalls[1] != -1)
        {
            GameObject.Find("FrontBase").GetComponent<Renderer>().material = Green;
            winner = 1;
        }
        if (playersRemainingBalls[2] != -1)
        {
            GameObject.Find("LeftBase").GetComponent<Renderer>().material = Green;
            winner = 2;
        }
        if (playersRemainingBalls[3] != -1)
        {
            GameObject.Find("RightBase").GetComponent<Renderer>().material = Green;
            winner = 3;
        }
    }

    void startBallChangeDirection()
    {
        if (!areBallsRest)
        {
            ballsSpeed = new List<Vector3>();
            for (int i = 0; i < inGameObjects.Count; i++)
            {
                if (inGameObjects[i].name.Equals("Ball(Clone)") && inGameObjects[i].GetComponent<BallScript>().ground)
                {
                    ballsSpeed.Add(inGameObjects[i].GetComponent<Rigidbody>().velocity);
                    inGameObjects[i].GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
                    inGameObjects[i].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
                    inGameObjects[i].GetComponent<BallScript>().rest = true;
                }
            }
            areBallsRest = true;
            nextBallChangeTime = nextBallChangeTime + SplitScreenModel.changeDirectionBallInterval;
            GameObject.Find("SO Down Led").transform.GetChild(0).GetComponent<Renderer>().material = LedRed;
            GameObject.Find("NO Down Led").transform.GetChild(0).GetComponent<Renderer>().material = LedRed;
            GameObject.Find("NE Down Led").transform.GetChild(0).GetComponent<Renderer>().material = LedRed;
            GameObject.Find("SE Down Led").transform.GetChild(0).GetComponent<Renderer>().material = LedRed;
            GameObject.Find("Circle").transform.GetChild(0).GetComponent<Renderer>().material = LedRed;
        }
        else
        {
            int j = 0;
            for (int i = 0; i < inGameObjects.Count; i++)
            {
                if (inGameObjects[i].name.Equals("Ball(Clone)") && inGameObjects[i].GetComponent<BallScript>().rest)
                {
                    inGameObjects[i].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionY;
                    ballsSpeed[j] = new Vector3(ballsSpeed[j].x * -1, 0, ballsSpeed[j].z * -1);
                    inGameObjects[i].GetComponent<Rigidbody>().velocity = ballsSpeed[j];
                    inGameObjects[i].GetComponent<BallScript>().rest = false;
                    j++;
                }
            }
            areBallsRest = false;
            nextBallChangeTime = Random.Range(((int)Mathf.Floor(currentTime() / SplitScreenModel.centralCannonInterval)+1)* SplitScreenModel.centralCannonInterval + (int)SplitScreenModel.centralCannonDuration, ((int)Mathf.Floor(currentTime() / SplitScreenModel.centralCannonInterval) + 1) * SplitScreenModel.centralCannonInterval + SplitScreenModel.centralCannonInterval-SplitScreenModel.changeDirectionBallInterval-1);
            GameObject.Find("SO Down Led").transform.GetChild(0).GetComponent<Renderer>().material = LedOff;
            GameObject.Find("NO Down Led").transform.GetChild(0).GetComponent<Renderer>().material = LedOff;
            GameObject.Find("NE Down Led").transform.GetChild(0).GetComponent<Renderer>().material = LedOff;
            GameObject.Find("SE Down Led").transform.GetChild(0).GetComponent<Renderer>().material = LedOff;
            GameObject.Find("Circle").transform.GetChild(0).GetComponent<Renderer>().material = LedOff;
        }

    }

    private void FixedUpdate()
    {
        if (!isGameFinished)
        {
            manageCannons();
            manageCentralCannonMovement();
        }
    }

    private void manageCentralCannonMovement()
    {
        if ((int)currentTime() % SplitScreenModel.centralCannonInterval == 0 && (int)currentTime() != 0 && !isCentralCannonUp)
        {
            isCentralCannonUp = true;
            centralCannonDegrees = Random.Range(50, 130);
            GameObject.Find("Circle").transform.GetChild(0).GetComponent<Renderer>().material = LedBlue;
        }
        if (isCentralCannonUp)
        {
            GameObject turret = GameObject.Find("Turret");
            if (turret.transform.position.y < 0 && currentTime() % SplitScreenModel.centralCannonInterval < SplitScreenModel.centralCannonDuration)
            {
                turret.GetComponent<Transform>().position = new Vector3(turret.transform.position.x, turret.transform.position.y + SplitScreenModel.elevationSpeedCentralCannon, turret.transform.position.z);
                return;
            }
            if (turret.transform.position.y != 0 && currentTime() % SplitScreenModel.centralCannonInterval < SplitScreenModel.centralCannonDuration)
            {
                turret.GetComponent<Transform>().position = new Vector3(0, 0, 0);
                GameObject.Find("SO Down Led").transform.GetChild(0).GetComponent<Renderer>().material = LedBlue;
                GameObject.Find("NO Down Led").transform.GetChild(0).GetComponent<Renderer>().material = LedBlue;
                return;
            }
            if (currentTime() % SplitScreenModel.centralCannonInterval < SplitScreenModel.centralCannonDuration && !centralCannonStop)
            {
                turret.GetComponent<Transform>().rotation = Quaternion.Euler(new Vector3(turret.transform.rotation.eulerAngles.x, turret.transform.rotation.eulerAngles.y + SplitScreenModel.degreesSpeedCentralCannonRotation, turret.transform.rotation.eulerAngles.z));
                if (Mathf.Abs(turret.transform.eulerAngles.y - centralCannonDegrees) < SplitScreenModel.degreesSpeedCentralCannonRotation && leftRotations == 0)
                    {
                        GameObject spawnedBall;
                        spawnedBall = (GameObject)Instantiate(ball, new Vector3(0, 0.4f, 0), Quaternion.identity);
                        spawnedBall.GetComponent<Rigidbody>().velocity = new Vector3(-Mathf.Sin(turret.transform.rotation.eulerAngles.y * Mathf.PI / 180) * (SplitScreenModel.getMultiplier(currentTime()) + SplitScreenModel.baseCentralCannonPower), 0, -Mathf.Cos(turret.transform.rotation.eulerAngles.y * Mathf.PI / 180) * (SplitScreenModel.getMultiplier(currentTime()) + SplitScreenModel.baseCentralCannonPower));
                        ballsInGame++;
                        inGameObjects.Add(spawnedBall);
                        centralCannonStop = true;
                        leftRotations = 3;
                        if (centralCannonDegrees < 45 || centralCannonDegrees > 315)
                        {
                            centralCannonDegrees = Random.Range(50, 130);
                            GameObject.Find("SO Down Led").transform.GetChild(0).GetComponent<Renderer>().material = LedBlue;
                            GameObject.Find("NO Down Led").transform.GetChild(0).GetComponent<Renderer>().material = LedBlue;
                            GameObject.Find("NE Down Led").transform.GetChild(0).GetComponent<Renderer>().material = LedOff;
                            GameObject.Find("SE Down Led").transform.GetChild(0).GetComponent<Renderer>().material = LedOff;
                        return;
                        }
                        if (centralCannonDegrees > 45 && centralCannonDegrees < 135)
                        {
                            centralCannonDegrees = Random.Range(140, 220);
                        GameObject.Find("SO Down Led").transform.GetChild(0).GetComponent<Renderer>().material = LedOff;
                        GameObject.Find("NO Down Led").transform.GetChild(0).GetComponent<Renderer>().material = LedBlue;
                        GameObject.Find("NE Down Led").transform.GetChild(0).GetComponent<Renderer>().material = LedBlue;
                        GameObject.Find("SE Down Led").transform.GetChild(0).GetComponent<Renderer>().material = LedOff;
                        return;
                        }
                        if (centralCannonDegrees > 135 && centralCannonDegrees < 225)
                        {
                            centralCannonDegrees = Random.Range(230, 310);
                        GameObject.Find("SO Down Led").transform.GetChild(0).GetComponent<Renderer>().material = LedOff;
                        GameObject.Find("NO Down Led").transform.GetChild(0).GetComponent<Renderer>().material = LedOff;
                        GameObject.Find("NE Down Led").transform.GetChild(0).GetComponent<Renderer>().material = LedBlue;
                        GameObject.Find("SE Down Led").transform.GetChild(0).GetComponent<Renderer>().material = LedBlue;
                        return;
                        }
                        if (centralCannonDegrees > 225 && centralCannonDegrees < 315)
                        {
                            centralCannonDegrees = Random.Range(320, 400) % 360;
                        GameObject.Find("SO Down Led").transform.GetChild(0).GetComponent<Renderer>().material = LedBlue;
                        GameObject.Find("NO Down Led").transform.GetChild(0).GetComponent<Renderer>().material = LedOff;
                        GameObject.Find("NE Down Led").transform.GetChild(0).GetComponent<Renderer>().material = LedOff;
                        GameObject.Find("SE Down Led").transform.GetChild(0).GetComponent<Renderer>().material = LedBlue;
                        return;
                        }
                    return;
                    }
                if(Mathf.Abs(turret.transform.eulerAngles.y - centralCannonDegrees) < SplitScreenModel.degreesSpeedCentralCannonRotation && leftRotations != 0)
                {
                    leftRotations--;
                }
                return;
             }
             if (!centralCannonStop)
             {
                turret.GetComponent<Transform>().position = new Vector3(turret.transform.position.x, turret.transform.position.y - SplitScreenModel.elevationSpeedCentralCannon, turret.transform.position.z);
                if (turret.transform.position.y <= -1f)
                {
                    isCentralCannonUp = false;
                    turret.GetComponent<Transform>().rotation = Quaternion.Euler(0, 0, 0);
                    turret.GetComponent<Transform>().position = new Vector3(0, -1, 0);
                    GameObject.Find("SO Down Led").transform.GetChild(0).GetComponent<Renderer>().material = LedOff;
                    GameObject.Find("NO Down Led").transform.GetChild(0).GetComponent<Renderer>().material = LedOff;
                    GameObject.Find("NE Down Led").transform.GetChild(0).GetComponent<Renderer>().material = LedOff;
                    GameObject.Find("SE Down Led").transform.GetChild(0).GetComponent<Renderer>().material = LedOff;
                    GameObject.Find("Circle").transform.GetChild(0).GetComponent<Renderer>().material = LedOff;
                }
             }
        }
    }

    void calculateDestination(int i)
    {
        destination[i] = Random.Range(0f, 61f);
        if (i == 0 || i == 2)
        {
            destination[i] = 15 + destination[i];
        }
        else
        {
            destination[i] = 345 - destination[i];
        }
    }

    void spawnBall()
    {
        int port = Random.Range(1, 5);
        while (port == lastSPort)
        {
            port = Random.Range(1, 5);
        }
        lastSPort = port;
        float speed =  Random.Range(SplitScreenModel.getMultiplier(currentTime()) + SplitScreenModel.launchBallMinSpeed, SplitScreenModel.getMultiplier(currentTime()) + SplitScreenModel.launchBallMinSpeed);
        GameObject spawnedBall=null;
        switch (port)
        {
            case 1:
                {
                    spawnedBall = (GameObject)Instantiate(ball, new Vector3(6f, 2.35f, -6f), Quaternion.identity);
                    spawnedBall.GetComponent<Rigidbody>().velocity = new Vector3(-Mathf.Cos(GameObject.Find("Cannon 1").transform.rotation.eulerAngles.y * Mathf.PI/180) * speed,0, Mathf.Sin(GameObject.Find("Cannon 1").transform.rotation.eulerAngles.y * Mathf.PI/180) * speed);
                    ballsInGame++;
                }
            break;
            case 2:
                {
                    spawnedBall = (GameObject)Instantiate(ball, new Vector3(6f, 2.35f, 6f), Quaternion.identity);
                    spawnedBall.GetComponent<Rigidbody>().velocity = new Vector3(-Mathf.Cos(GameObject.Find("Cannon 2").transform.rotation.eulerAngles.y * Mathf.PI / 180) * speed,0, Mathf.Sin(GameObject.Find("Cannon 2").transform.rotation.eulerAngles.y * Mathf.PI / 180) * speed);
                    ballsInGame++;
                }
                break;
            case 3:
                {
                    spawnedBall = (GameObject)Instantiate(ball, new Vector3(-6f, 2.35f, 6f), Quaternion.identity);
                    spawnedBall.GetComponent<Rigidbody>().velocity = new Vector3(Mathf.Cos(GameObject.Find("Cannon 3").transform.rotation.eulerAngles.y * Mathf.PI / 180) *speed,0,-Mathf.Sin(GameObject.Find("Cannon 3").transform.rotation.eulerAngles.y * Mathf.PI / 180) *speed);
                    ballsInGame++;
                }
                break;
            case 4:
                {
                    spawnedBall = (GameObject)Instantiate(ball, new Vector3(-6f, 2.35f, -6f), Quaternion.identity);
                    spawnedBall.GetComponent<Rigidbody>().velocity = new Vector3(Mathf.Cos(GameObject.Find("Cannon 4").transform.rotation.eulerAngles.y * Mathf.PI / 180) * speed,  0, -Mathf.Sin(GameObject.Find("Cannon 4").transform.rotation.eulerAngles.y * Mathf.PI / 180) * speed);
                    ballsInGame++;
                    
                }
                break;
        }
        inGameObjects.Add(spawnedBall);
    }

    public float currentTime()
    {
        return gameTime;
    }

}