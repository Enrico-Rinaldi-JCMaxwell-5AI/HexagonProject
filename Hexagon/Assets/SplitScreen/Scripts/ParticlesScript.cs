using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesScript : MonoBehaviour {
    public float deadTime;
	// Use this for initialization
	void Start () {
        deadTime = GameObject.Find("SplitScreenGameManager").GetComponent<GameController>().gameTime + 4f;
	}
	
	// Update is called once per frame
	void Update () {
        if (GameObject.Find("SplitScreenGameManager").GetComponent<GameController>().gameTime >= deadTime)
        {
            GameObject.Find("SplitScreenGameManager").GetComponent<GameController>().inGameObjects.Remove(gameObject);
            Destroy(gameObject);
        }
	}
}
