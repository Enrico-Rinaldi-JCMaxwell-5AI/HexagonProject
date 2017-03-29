using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldScript : MonoBehaviour {

    public float endTime;
    public GameObject mainController;

	void Start () {
        mainController = GameObject.Find("SplitScreenGameManager");
	}
	
	// Update is called once per frame
	void Update () {
		if(endTime<=mainController.GetComponent<GameController>().currentTime())
        {
            Destroy(gameObject);
        }
	}
}
