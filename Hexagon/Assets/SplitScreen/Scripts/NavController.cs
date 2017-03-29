using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class NavController : MonoBehaviour { 
    int joyNum;
    public int player;
    GameObject mainController;
	// Use this for initialization
	void Start () {
        player = Int32.Parse(name.ToCharArray()[1].ToString());
        joyNum = GameObject.Find("SplitScreenGameManager").GetComponent<SplitScreenView>().joyNums[player];
        mainController = GameObject.Find("SplitScreenGameManager");
	}
	
	// Update is called once per frame
	void Update () {
        if(mainController==null)
            mainController = GameObject.Find("SplitScreenGameManager");
        if (!mainController.GetComponent<SplitScreenView>().paused && !mainController.GetComponent<GameController>().isGameFinished)
        {
            float power = SplitScreenModel.joySensibility * Time.deltaTime;
            if (player == 0 && Input.GetAxis("MovJoy" + joyNum) * power + transform.position.x < 4f && Input.GetAxis("MovJoy" + joyNum) * power + transform.position.x > -4f)
                GetComponent<Transform>().position = new Vector3(Input.GetAxis("MovJoy" + joyNum) * power + transform.position.x, transform.position.y, transform.position.z);
            if (player == 1 && -Input.GetAxis("MovJoy" + joyNum) * power + transform.position.x < 4f && -Input.GetAxis("MovJoy" + joyNum) * power + transform.position.x > -4f)
               GetComponent<Transform>().position = new Vector3(-Input.GetAxis("MovJoy" + joyNum)* power + transform.position.x, transform.position.y, transform.position.z);
            if (player == 2 && -Input.GetAxis("MovJoy" + joyNum) * power + transform.position.z < 4f && -Input.GetAxis("MovJoy" + joyNum) * power + transform.position.z > -4f)
                GetComponent<Transform>().position = new Vector3(transform.position.x, transform.position.y, -Input.GetAxis("MovJoy" + joyNum) * power + transform.position.z);
            if (player == 3 && Input.GetAxis("MovJoy" + joyNum) * power + transform.position.z < 4f && Input.GetAxis("MovJoy" + joyNum) * power + transform.position.z > -4f)
                GetComponent<Transform>().position = new Vector3(transform.position.x, transform.position.y, Input.GetAxis("MovJoy" + joyNum) * power + transform.position.z);
        }
    }
}
