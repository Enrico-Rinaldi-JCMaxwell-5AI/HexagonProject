using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerShieldScript : MonoBehaviour {

    public float endTime;
    public GameObject mainController;

    void Start()
    {
        mainController = GameObject.Find("HostGameManager(Clone)");
    }

    // Update is called once per frame
    void Update()
    {
        if (endTime <= mainController.GetComponent<ServerGameController>().model.gameTime)
        {
            Destroy(gameObject);
        }
    }
}
