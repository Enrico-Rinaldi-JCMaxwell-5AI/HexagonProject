using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerGameController : MonoBehaviour {

    public Material Red;
    public Material Blue;
    public Material Green;
    public Material LedRed;
    public Material LedBlue;
    public Material LedOff;
    public ServerGameModel model;
    ServerGameView view;
    public NetworkManager nmanager;
    public Object ball;
    public List<GameObject> allInGameObjects = new List<GameObject>();
    public Object navPrefab;
    public Object SBomb;
    public Object DBomb;
    public Object Repulsion;
    public Object Shield;
    public float[] destination;
    public int ballsInGame;
    public int lastSPort;
    public bool isCentralCannonUp=false;
    public float centralCannonDegrees;
    public int leftRotations;
    public bool centralCannonStop=false;
    public int ballid=4;
    public int nextBallChangeTime;
    public List<Bomb> bombTimes;
    public bool areBallsRest = false;
    public List<Vector3> ballsSpeed;

    private void Start()
    {
        model = GetComponent<ServerGameModel>();
        view = GetComponent<ServerGameView>();
        nmanager = GetComponent<NetworkManager>();
    }

    public void resetMap()
    {
        for (int i = 0; i < allInGameObjects.Count; i++)
        {
            Destroy(allInGameObjects[i]);
        }
        allInGameObjects = new List<GameObject>();
        GameObject.Find("FrontPlane").GetComponent<Transform>().position = new Vector3(0, -51.86f, 6);
        GameObject.Find("BackPlane").GetComponent<Transform>().position = new Vector3(0, -51.86f, -6);
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
        GameObject.Find("Reflection Probe").GetComponent<ReflectionProbe>().RenderProbe();
        model.gameTime = 0;
        model.isGameFinished = false;
        model.winner = -1;
        //DOPO TUTTE LE OPERAZIONI
    }

    public void startGame()
    {
        areBallsRest = false;
        nextBallChangeTime = Random.Range(190, 221);
        ballsInGame = 0;
        lastSPort = -1;
        isCentralCannonUp = false;
        centralCannonStop = false;
        destination = new float[4];
        model.gameTime = 0;
        model.isGameFinished = false;
        GameObject.Find("BackPlane").GetComponent<Transform>().position = new Vector3(-6, -51.86f, 0);
        GameObject.Find("FrontPlane").GetComponent<Transform>().position = new Vector3(-6, -51.86f, 0);
        GameObject.Find("BackBase").GetComponent<Renderer>().material = Blue;
        GameObject.Find("FrontBase").GetComponent<Renderer>().material = Blue;
        if (nmanager.server.clientSocketList.Count() >= 2)
        {
            GameObject.Find("LeftBase").GetComponent<Renderer>().material = Blue;
            GameObject.Find("LeftPlane").GetComponent<Transform>().position = new Vector3(-6, -51.86f, 0);
        }
        if (nmanager.server.clientSocketList.Count() == 3)
        {
            GameObject.Find("RightBase").GetComponent<Renderer>().material = Blue;
            GameObject.Find("RightPlane").GetComponent<Transform>().position = new Vector3(6, -51.86f, 0);
        }
        nmanager.sendStartGame();
        GameObject instantiated;
        GameObject[] navs = new GameObject[nmanager.server.clientSocketList.Count()+1];
        //OWNER DATA
        instantiated = (GameObject)Instantiate(navPrefab, model.startingVectorNavs[0], Quaternion.Euler(model.navRots[0]));
        instantiated.name = allInGameObjects.Count.ToString();
        nmanager.server.ownerData.resetData();
        navs[0] = instantiated;
        allInGameObjects.Add(instantiated);
        nmanager.sendInstantiate(System.Int32.Parse(instantiated.name), 0, instantiated.transform.position);
        //ALL OTHER CLIENT DATA
        for (int i = 1; i < nmanager.server.clientSocketList.Count()+1; i++) {
            instantiated = (GameObject)Instantiate(navPrefab, model.startingVectorNavs[i], Quaternion.Euler(model.navRots[i]));
            instantiated.name = allInGameObjects.Count.ToString();
            nmanager.server.clientSocketList.Get(i - 1).clientData.resetData();
            navs[i] = instantiated;
            allInGameObjects.Add(instantiated);
            nmanager.sendInstantiate(System.Int32.Parse(instantiated.name), 0, instantiated.transform.position);
        }
        view.navs = navs;
        bombTimes = SplitScreenModel.mapExplosions();
    }

    private void Update()
    {
        if(model.isGameStarted && !model.isGameFinished)
        {
            model.gameTime = model.gameTime + Time.deltaTime;
            if (checkEnd())
            {
                nmanager.sendFinishGame(model.winner);
                gameFinished(model.winner);
                model.isGameFinished = true;
            }
            checkBalls();
            if(ballsInGame<SplitScreenModel.maxBalls((int)model.gameTime) && !areBallsRest &&nextBallChangeTime-model.gameTime>2 && !isCentralCannonUp)
            {
                spawnBall();
            }
            if (nextBallChangeTime == (int)model.gameTime)
            {
                startBallChangeDirection();
            }
            manageExplosion();
            nmanager.server.ownerData.reduceCooldown(Time.deltaTime);
            for(int i=0;i<3;i++)
            {
                if(nmanager.server.clientSocketList.Exist(i))
                {
                    nmanager.server.clientSocketList.Get(i).clientData.reduceCooldown(Time.deltaTime);
                }
            }
        }
    }

    void startBallChangeDirection()
    {
        if (!areBallsRest)
        {
            ballsSpeed = new List<Vector3>();
            for (int i = 0; i < allInGameObjects.Count; i++)
            {
                try
                {
                    if (allInGameObjects[i].GetComponent<ServerBallScript>().ground)
                    {
                        ballsSpeed.Add(allInGameObjects[i].GetComponent<Rigidbody>().velocity);
                        allInGameObjects[i].GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
                        allInGameObjects[i].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
                        allInGameObjects[i].GetComponent<ServerBallScript>().rest = true;
                    }
                }catch(System.Exception) { }
            }
            areBallsRest = true;
            nmanager.sendCentralBallRest(true);
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
            for (int i = 0; i < allInGameObjects.Count; i++)
            {
                try
                {
                    if (allInGameObjects[i].GetComponent<ServerBallScript>().rest)
                    {
                        allInGameObjects[i].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionY;
                        ballsSpeed[j] = new Vector3(ballsSpeed[j].x * -1, 0, ballsSpeed[j].z * -1);
                        allInGameObjects[i].GetComponent<Rigidbody>().velocity = ballsSpeed[j];
                        allInGameObjects[i].GetComponent<ServerBallScript>().rest = false;
                        j++;
                    }
                }
                catch (System.Exception) { }
            }
            areBallsRest = false;
            nmanager.sendCentralBallRest(false);
            nextBallChangeTime = Random.Range(((int)Mathf.Floor(model.gameTime / SplitScreenModel.centralCannonInterval) + 1) * SplitScreenModel.centralCannonInterval + (int)SplitScreenModel.centralCannonDuration, ((int)Mathf.Floor(model.gameTime / SplitScreenModel.centralCannonInterval) + 1) * SplitScreenModel.centralCannonInterval + SplitScreenModel.centralCannonInterval - SplitScreenModel.changeDirectionBallInterval - 1);
            GameObject.Find("SO Down Led").transform.GetChild(0).GetComponent<Renderer>().material = LedOff;
            GameObject.Find("NO Down Led").transform.GetChild(0).GetComponent<Renderer>().material = LedOff;
            GameObject.Find("NE Down Led").transform.GetChild(0).GetComponent<Renderer>().material = LedOff;
            GameObject.Find("SE Down Led").transform.GetChild(0).GetComponent<Renderer>().material = LedOff;
            GameObject.Find("Circle").transform.GetChild(0).GetComponent<Renderer>().material = LedOff;
        }

    }

    private void FixedUpdate()
    {
        if (!model.isGameFinished && model.isGameStarted)
        {
            manageCannons();
            manageCentralCannonMovement();
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

    private void manageCentralCannonMovement()
    {
        if ((int)model.gameTime % SplitScreenModel.centralCannonInterval == 0 && (int)model.gameTime != 0 && !isCentralCannonUp)
        {
            isCentralCannonUp = true;
            nmanager.sendCentralCannonUp();
            centralCannonDegrees = Random.Range(50, 130);
            GameObject.Find("Circle").transform.GetChild(0).GetComponent<Renderer>().material = LedBlue;
        }
        if (isCentralCannonUp)
        {
            GameObject turret = GameObject.Find("Turret");
            if (turret.transform.position.y < 0 && model.gameTime % SplitScreenModel.centralCannonInterval < SplitScreenModel.centralCannonDuration)
            {
                turret.GetComponent<Transform>().position = new Vector3(turret.transform.position.x, turret.transform.position.y + SplitScreenModel.elevationSpeedCentralCannon, turret.transform.position.z);
                nmanager.sendCentralCannonPos(turret.transform.rotation.eulerAngles.y, turret.transform.position.y);
                return;
            }
            if (turret.transform.position.y != 0 && model.gameTime % SplitScreenModel.centralCannonInterval < SplitScreenModel.centralCannonDuration)
            {
                turret.GetComponent<Transform>().position = new Vector3(0, 0, 0);
                GameObject.Find("SO Down Led").transform.GetChild(0).GetComponent<Renderer>().material = LedBlue;
                GameObject.Find("NO Down Led").transform.GetChild(0).GetComponent<Renderer>().material = LedBlue;
                nmanager.sendCentralCannonPos(turret.transform.rotation.eulerAngles.y, turret.transform.position.y);
                return;
            }
            if (model.gameTime % SplitScreenModel.centralCannonInterval < SplitScreenModel.centralCannonDuration && !centralCannonStop)
            {
                turret.GetComponent<Transform>().rotation = Quaternion.Euler(new Vector3(turret.transform.rotation.eulerAngles.x, turret.transform.rotation.eulerAngles.y + SplitScreenModel.degreesSpeedCentralCannonRotation, turret.transform.rotation.eulerAngles.z));
                nmanager.sendCentralCannonPos(turret.transform.rotation.eulerAngles.y, turret.transform.position.y);
                if (Mathf.Abs(turret.transform.eulerAngles.y - centralCannonDegrees) < SplitScreenModel.degreesSpeedCentralCannonRotation && leftRotations == 0)
                {
                    GameObject spawnedBall;
                    spawnedBall = (GameObject)Instantiate(ball, new Vector3(0, 0.4f, 0), Quaternion.identity);
                    spawnedBall.GetComponent<Rigidbody>().velocity = new Vector3(-Mathf.Sin(turret.transform.rotation.eulerAngles.y * Mathf.PI / 180) * (SplitScreenModel.getMultiplier(model.gameTime) + SplitScreenModel.baseCentralCannonPower), 0, -Mathf.Cos(turret.transform.rotation.eulerAngles.y * Mathf.PI / 180) * (SplitScreenModel.getMultiplier(model.gameTime) + SplitScreenModel.baseCentralCannonPower));
                    ballsInGame++;
                    spawnedBall.name = ballid.ToString();
                    nmanager.sendInstantiate(ballid, 1, spawnedBall.transform.position);
                    increaseBallId();
                    allInGameObjects.Add(spawnedBall);
                    centralCannonStop = true;
                    leftRotations = 3;
                    if (centralCannonDegrees < 45 || centralCannonDegrees > 315)
                    {
                        centralCannonDegrees = Random.Range(50, 130);
                        GameObject.Find("SO Down Led").transform.GetChild(0).GetComponent<Renderer>().material = LedBlue;
                        GameObject.Find("NO Down Led").transform.GetChild(0).GetComponent<Renderer>().material = LedBlue;
                        GameObject.Find("NE Down Led").transform.GetChild(0).GetComponent<Renderer>().material = LedOff;
                        GameObject.Find("SE Down Led").transform.GetChild(0).GetComponent<Renderer>().material = LedOff;
                        nmanager.sendCentralCannonLeds(0);
                        return;
                    }
                    if (centralCannonDegrees > 45 && centralCannonDegrees < 135)
                    {
                        centralCannonDegrees = Random.Range(140, 220);
                        GameObject.Find("SO Down Led").transform.GetChild(0).GetComponent<Renderer>().material = LedOff;
                        GameObject.Find("NO Down Led").transform.GetChild(0).GetComponent<Renderer>().material = LedBlue;
                        GameObject.Find("NE Down Led").transform.GetChild(0).GetComponent<Renderer>().material = LedBlue;
                        GameObject.Find("SE Down Led").transform.GetChild(0).GetComponent<Renderer>().material = LedOff;
                        nmanager.sendCentralCannonLeds(1);
                        return;
                    }
                    if (centralCannonDegrees > 135 && centralCannonDegrees < 225)
                    {
                        centralCannonDegrees = Random.Range(230, 310);
                        GameObject.Find("SO Down Led").transform.GetChild(0).GetComponent<Renderer>().material = LedOff;
                        GameObject.Find("NO Down Led").transform.GetChild(0).GetComponent<Renderer>().material = LedOff;
                        GameObject.Find("NE Down Led").transform.GetChild(0).GetComponent<Renderer>().material = LedBlue;
                        GameObject.Find("SE Down Led").transform.GetChild(0).GetComponent<Renderer>().material = LedBlue;
                        nmanager.sendCentralCannonLeds(2);
                        return;
                    }
                    if (centralCannonDegrees > 225 && centralCannonDegrees < 315)
                    {
                        centralCannonDegrees = Random.Range(320, 400) % 360;
                        GameObject.Find("SO Down Led").transform.GetChild(0).GetComponent<Renderer>().material = LedBlue;
                        GameObject.Find("NO Down Led").transform.GetChild(0).GetComponent<Renderer>().material = LedOff;
                        GameObject.Find("NE Down Led").transform.GetChild(0).GetComponent<Renderer>().material = LedOff;
                        GameObject.Find("SE Down Led").transform.GetChild(0).GetComponent<Renderer>().material = LedBlue;
                        nmanager.sendCentralCannonLeds(3);
                        return;
                    }
                    return;
                }
                if (Mathf.Abs(turret.transform.eulerAngles.y - centralCannonDegrees) < SplitScreenModel.degreesSpeedCentralCannonRotation && leftRotations != 0)
                {
                    leftRotations--;
                }
                return;
            }
            if (!centralCannonStop)
            {
                turret.GetComponent<Transform>().position = new Vector3(turret.transform.position.x, turret.transform.position.y - SplitScreenModel.elevationSpeedCentralCannon, turret.transform.position.z);
                nmanager.sendCentralCannonPos(turret.transform.rotation.eulerAngles.y, turret.transform.position.y);
                if (turret.transform.position.y <= -1f)
                {
                    isCentralCannonUp = false;
                    nmanager.sendCentralCannonDown();
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

            if (Mathf.Abs(currentCannon.transform.rotation.eulerAngles.y - destination[i]) < SplitScreenModel.degreesSpeedLateralCannonRotation * 4)
                calculateDestination(i);

        }
    }

    void manageExplosion()
    {
        for (int i = 0; i < bombTimes.Count; i++)
        {
            if ((int)model.gameTime == bombTimes[i].spawnTime)
            {
                //Spawna la bomba
                GameObject bomb;
                if (bombTimes[i].isStatic) {
                    bomb = (GameObject)Instantiate(SBomb, new Vector3(Random.Range(-3.5f, 3.5f), 5f, Random.Range(-3.51f, 3.51f)), Quaternion.identity);
                    nmanager.sendInstantiate(ballid, 2, bomb.transform.position);
                }
                else {
                    bomb = (GameObject)Instantiate(DBomb, new Vector3(Random.Range(-3.5f, 3.5f), 5f, Random.Range(-3.51f, 3.51f)), Quaternion.identity);
                    nmanager.sendInstantiate(ballid, 3, bomb.transform.position);
                }
                bombTimes.RemoveAt(i);
                allInGameObjects.Add(bomb);
                bomb.name = ballid.ToString();
                increaseBallId();
                if (model.gameTime > 300)
                {
                    bombTimes.Add(new Bomb(Random.Range((int)model.gameTime + SplitScreenModel.endGameBombIntervalMin, (int)model.gameTime + SplitScreenModel.endGameBombIntervalMax), false));
                }
            }
        }
    }

    private void increaseBallId()
    {
        ballid++;
        if (ballid == 256)
            ballid = 4;
    }

    public bool checkEnd()
    {
        if(nmanager.server.clientSocketList.Count()==0)
        {
            model.winner = 0;
            return true;
        }else
        {
            int players = 0;
            if (nmanager.server.ownerData.isAlive)
                players++;
            for(int i=0;i<3;i++)
            {
                if (nmanager.server.clientSocketList.Exist(i) && nmanager.server.clientSocketList.Get(i).clientData.isAlive)
                    players++;
            }
            if (players == 1)
            {
                if (nmanager.server.ownerData.isAlive)
                {
                    model.winner = 0;
                    return true;
                }
                for(int i = 0; i < 3; i++)
                {
                    if(nmanager.server.clientSocketList.Exist(i) && nmanager.server.clientSocketList.Get(i).clientData.isAlive)
                    {
                        model.winner = i+1;
                        return true;
                    }
                }
            }
            return false;
        }
    }

    public void checkBalls()
    {
        if (nmanager.server.ownerData.balls <= 0 && nmanager.server.ownerData.isAlive)
        {
            nmanager.server.ownerData.balls = 0;
            nmanager.server.ownerData.isAlive = false;
            nmanager.sendDeadPort(0);
            changePortState(0);
            for(int i=0;i<allInGameObjects.Count;i++)
            {
                if(allInGameObjects[i].name.Equals("0"))
                {
                    Destroy(allInGameObjects[i]);
                    allInGameObjects.RemoveAt(i);
                }
            }
            view.navs[0] = null;
        }
        for(int i=0;i<3;i++)
        {
            if(nmanager.server.clientSocketList.Exist(i) && nmanager.server.clientSocketList.Get(i).clientData.balls<=0&& nmanager.server.clientSocketList.Get(i).clientData.isAlive)
            {
                nmanager.server.clientSocketList.Get(i).clientData.balls = 0;
                nmanager.server.clientSocketList.Get(i).clientData.isAlive = false;
                nmanager.sendDeadPort(i+1);
                changePortState(i+1);
                for (int j = 0; j < allInGameObjects.Count; j++)
                {
                    if (allInGameObjects[j].name.Equals((i+1).ToString()))
                    {
                        Destroy(allInGameObjects[j]);
                        allInGameObjects.RemoveAt(j);
                    }
                }
                view.navs[i + 1] = null;
            }
        }
    }

    public void changePortState(int port)
    {
        for (int i = 0; i < allInGameObjects.Count; i++)
        {
            if (allInGameObjects[i].name.Equals(port.ToString()))
            {
                Destroy(allInGameObjects[i]);
                allInGameObjects.RemoveAt(i);
            }
        }
        switch (port)
        {
            case 0:
                {
                    GameObject.Find("BackPlane").GetComponent<Transform>().position = new Vector3(0f, 2.375f, -6f);
                    GameObject.Find("BackBase").GetComponent<Transform>().GetComponent<Renderer>().material = Red;
                }
                break;
            case 1:
                {
                    GameObject.Find("FrontPlane").GetComponent<Transform>().position = new Vector3(0f, 2.375f, 6f);
                    GameObject.Find("FrontBase").GetComponent<Transform>().GetComponent<Renderer>().material = Red;
                }
                break;
            case 2:
                {
                    GameObject.Find("LeftPlane").GetComponent<Transform>().position = new Vector3(-6f, 2.375f, 0);
                    GameObject.Find("LeftBase").GetComponent<Transform>().GetComponent<Renderer>().material = Red;
                }
                break;
            case 3:
                {
                    GameObject.Find("RightPlane").GetComponent<Transform>().position = new Vector3(6f, 2.375f, 0);
                    GameObject.Find("RightBase").GetComponent<Transform>().GetComponent<Renderer>().material = Red;
                }
                break;
        }
    }

    public void gameFinished(int winner)
    {
        switch (winner)
        {
            case 0:
                {
                    GameObject.Find("BackBase").GetComponent<Transform>().GetComponent<Renderer>().material = Green;
                }
                break;
            case 1:
                {
                    GameObject.Find("FrontBase").GetComponent<Transform>().GetComponent<Renderer>().material = Green;
                }
                break;
            case 2:
                {
                    GameObject.Find("LeftBase").GetComponent<Transform>().GetComponent<Renderer>().material = Green;
                }
                break;
            case 3:
                {
                    GameObject.Find("RightBase").GetComponent<Transform>().GetComponent<Renderer>().material = Green;
                }
                break;
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
        float speed = Random.Range(SplitScreenModel.getMultiplier(model.gameTime) + SplitScreenModel.launchBallMinSpeed, SplitScreenModel.getMultiplier(model.gameTime) + SplitScreenModel.launchBallMinSpeed);
        GameObject spawnedBall = null;
        switch (port)
        {
            case 1:
                {
                    spawnedBall = (GameObject)Instantiate(ball, new Vector3(6f, 2.35f, -6f), Quaternion.identity);
                    spawnedBall.GetComponent<Rigidbody>().velocity = new Vector3(-Mathf.Cos(GameObject.Find("Cannon 1").transform.rotation.eulerAngles.y * Mathf.PI / 180) * speed, 0, Mathf.Sin(GameObject.Find("Cannon 1").transform.rotation.eulerAngles.y * Mathf.PI / 180) * speed);
                    ballsInGame++;
                }
                break;
            case 2:
                {
                    spawnedBall = (GameObject)Instantiate(ball, new Vector3(6f, 2.35f, 6f), Quaternion.identity);
                    spawnedBall.GetComponent<Rigidbody>().velocity = new Vector3(-Mathf.Cos(GameObject.Find("Cannon 2").transform.rotation.eulerAngles.y * Mathf.PI / 180) * speed, 0, Mathf.Sin(GameObject.Find("Cannon 2").transform.rotation.eulerAngles.y * Mathf.PI / 180) * speed);
                    ballsInGame++;
                }
                break;
            case 3:
                {
                    spawnedBall = (GameObject)Instantiate(ball, new Vector3(-6f, 2.35f, 6f), Quaternion.identity);
                    spawnedBall.GetComponent<Rigidbody>().velocity = new Vector3(Mathf.Cos(GameObject.Find("Cannon 3").transform.rotation.eulerAngles.y * Mathf.PI / 180) * speed, 0, -Mathf.Sin(GameObject.Find("Cannon 3").transform.rotation.eulerAngles.y * Mathf.PI / 180) * speed);
                    ballsInGame++;
                }
                break;
            case 4:
                {
                    spawnedBall = (GameObject)Instantiate(ball, new Vector3(-6f, 2.35f, -6f), Quaternion.identity);
                    spawnedBall.GetComponent<Rigidbody>().velocity = new Vector3(Mathf.Cos(GameObject.Find("Cannon 4").transform.rotation.eulerAngles.y * Mathf.PI / 180) * speed, 0, -Mathf.Sin(GameObject.Find("Cannon 4").transform.rotation.eulerAngles.y * Mathf.PI / 180) * speed);
                    ballsInGame++;
                }
                break;
        }
        spawnedBall.name = ballid.ToString();
        nmanager.sendInstantiate(ballid, 1, spawnedBall.transform.position);
        increaseBallId();  
        allInGameObjects.Add(spawnedBall);
    }

    public void moveNav(float pos,int id)
    {
        if (pos < 4 && pos > -4)
        {
            if ((id + 1) == 1)
            {
                view.navs[1].GetComponent<Transform>().position = new Vector3(pos, view.navs[1].transform.position.y, view.navs[1].transform.position.z);
            }
            if ((id + 1) == 2 || (id + 1) == 3)
            {
                view.navs[1].GetComponent<Transform>().position = new Vector3(view.navs[1].transform.position.x, view.navs[1].transform.position.y, pos);
            }
            nmanager.sendUpdateNav(id + 1, pos);
        }
    }

    public void useAbility(int player, int codAbility)
    {
        if (codAbility == 0)    //Repulsion
        {
            if (player == 0)
            {
                if (nmanager.server.ownerData.repulsionCoolDown <= 0)
                {
                    GameObject repulsion;
                    repulsion = (GameObject)Instantiate(Repulsion);
                    repulsion.transform.parent = view.navs[player].transform;
                    repulsion.GetComponent<Transform>().position = view.navs[player].transform.position;
                    nmanager.server.ownerData.repulsionCoolDown = SplitScreenModel.repulsionCooldown;
                    nmanager.sendUsedAbility(0, 0);
                }
                return;
            }
            if (nmanager.server.clientSocketList.Get(player - 1).clientData.repulsionCoolDown <= 0)
            {
                GameObject repulsion;
                repulsion = (GameObject)Instantiate(Repulsion);
                repulsion.transform.parent = view.navs[player].transform;
                repulsion.GetComponent<Transform>().position = view.navs[player].transform.position;
                nmanager.server.clientSocketList.Get(player - 1).clientData.repulsionCoolDown = SplitScreenModel.repulsionCooldown;
                nmanager.sendUsedAbility(player, 0);
            }
            //Do Repulsion Ability
            //QUALSIASI COSA AGGIUNGI IN GAME METTILA NELLA LISTA inGameObjects. SERVE A PREVENIRE CHE QUANDO SI RESETTA IL GAME LE COSE CHE SPAWNI RIMANGONO
        }
        if (codAbility == 1)   //Shield
        {
            if (player == 0)
            {
                if (nmanager.server.ownerData.shieldCoolDown <= 0)
                {
                    GameObject shield;
                    shield = (GameObject)Instantiate(Shield);
                    shield.transform.parent = view.navs[player].transform;
                    shield.GetComponent<Transform>().position = view.navs[player].transform.position;
                    shield.GetComponent<ServerShieldScript>().endTime = model.gameTime + SplitScreenModel.shieldDuration;
                    nmanager.server.ownerData.shieldCoolDown = SplitScreenModel.shieldCooldown;
                    nmanager.sendUsedAbility(0, 1);
                }
                return;
            }
            if (nmanager.server.clientSocketList.Get(player - 1).clientData.shieldCoolDown <= 0)
            {
                GameObject shield;
                shield = (GameObject)Instantiate(Shield);
                shield.transform.parent = view.navs[player].transform;
                shield.GetComponent<Transform>().position = view.navs[player].transform.position;
                shield.GetComponent<ServerShieldScript>().endTime = model.gameTime + SplitScreenModel.shieldDuration;
                nmanager.server.clientSocketList.Get(player - 1).clientData.shieldCoolDown = SplitScreenModel.shieldCooldown;
                nmanager.sendUsedAbility(player, 1);
            }
        }
    }
}