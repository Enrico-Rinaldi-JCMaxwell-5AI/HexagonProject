using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepulsionScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (transform.localScale.x < SplitScreenModel.maxRepulsionScale)
        {
            transform.localScale = new Vector3(transform.localScale.x + SplitScreenModel.speedRepulsionGrow, transform.localScale.y + SplitScreenModel.speedRepulsionGrow, transform.localScale.z + SplitScreenModel.speedRepulsionGrow);
        }
        else
        {
            GameObject.Find("SplitScreenGameManager").GetComponent<GameController>().inGameObjects.Remove(gameObject);
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name.Equals("Ball(Clone)"))
        {
            collision.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(collision.gameObject.GetComponent<Rigidbody>().velocity.x * SplitScreenModel.repulsionBallSpeed, 0, collision.gameObject.GetComponent<Rigidbody>().velocity.z * SplitScreenModel.repulsionBallSpeed);
            GameObject.Find("SplitScreenGameManager").GetComponent<GameController>().cooldowns[System.Int32.Parse(transform.parent.name.ToCharArray()[1].ToString())].repulsionTime = GameObject.Find("SplitScreenGameManager").GetComponent<GameController>().currentTime() + SplitScreenModel.repulsionCooldownReduced;
            Destroy(gameObject);
        }
    }
}
