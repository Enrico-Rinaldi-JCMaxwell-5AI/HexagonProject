using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerGameModel : MonoBehaviour {
    public ServerGameController controller;
    public int startBalls;
    public bool isGameStarted = false;
    public bool isGameFinished = false;
    public Vector3[] startingVectorNavs;
    public Vector3[] navRots;
    public string port = "30000";
    public string user = "";
    public int updatesPerSecond;
    public float gameTime = 0;
    public int winner;

    void Start()
    {
        startBalls = 60;
        controller = GetComponent<ServerGameController>();
        startingVectorNavs = new Vector3[4];
        navRots = new Vector3[4];
        startingVectorNavs[0] = new Vector3(0, 0, -6);
        startingVectorNavs[1] = new Vector3(0, 0, 6);
        startingVectorNavs[2] = new Vector3(-6, 0, 0);
        startingVectorNavs[3] = new Vector3(6, 0, 0);
        navRots[0] = new Vector3(0, 180, 0);
        navRots[1] = new Vector3(0, 0, 0);
        navRots[2] = new Vector3(0, 270, 0);
        navRots[3] = new Vector3(0, 90, 0);
    }
}
