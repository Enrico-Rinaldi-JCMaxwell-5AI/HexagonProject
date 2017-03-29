using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HostClientRepulsion : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
    void FixedUpdate()
    {
        if (transform.localScale.x < SplitScreenModel.maxRepulsionScale)
        {
            transform.localScale = new Vector3(transform.localScale.x + SplitScreenModel.speedRepulsionGrow, transform.localScale.y + SplitScreenModel.speedRepulsionGrow, transform.localScale.z + SplitScreenModel.speedRepulsionGrow);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // Update is called once per frame
    void Update () {
		
	}
}
