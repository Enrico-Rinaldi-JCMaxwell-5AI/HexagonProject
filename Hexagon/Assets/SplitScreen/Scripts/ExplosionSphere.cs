using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionSphere : MonoBehaviour {

	// Use this for initialization
	
	// Update is called once per frame
	void FixedUpdate () {
        if (transform.localScale.x < 5)
        {
            transform.localScale = new Vector3(transform.localScale.x + 0.4f, transform.localScale.y + 0.4f, transform.localScale.z + 0.4f);
        }else
        {
            GameObject.Find("SplitScreenGameManager").GetComponent<GameController>().inGameObjects.Remove(gameObject);
            Destroy(gameObject);
        }
	}
}
