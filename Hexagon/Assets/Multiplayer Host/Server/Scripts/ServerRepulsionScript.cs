using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerRepulsionScript : MonoBehaviour {

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

    private void OnCollisionEnter(Collision collision)
    {
        try
        {
            if (collision.gameObject.GetComponent<ServerBallScript>().ground)
            {
                collision.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(collision.gameObject.GetComponent<Rigidbody>().velocity.x * SplitScreenModel.repulsionBallSpeed, 0, collision.gameObject.GetComponent<Rigidbody>().velocity.z * SplitScreenModel.repulsionBallSpeed);
                if (System.Int32.Parse(transform.parent.name.ToCharArray()[0].ToString()) == 0)
                    GameObject.Find("HostGameManager(Clone)").GetComponent<NetworkManager>().server.ownerData.repulsionCoolDown = SplitScreenModel.repulsionCooldownReduced;
                else
                    GameObject.Find("HostGameManager(Clone)").GetComponent<NetworkManager>().server.clientSocketList.Get(System.Int32.Parse(transform.parent.name.ToCharArray()[0].ToString()) -1).clientData.repulsionCoolDown = SplitScreenModel.repulsionCooldownReduced;
                Destroy(gameObject);
            }
        }
        catch (System.Exception e) { Debug.Log(e.ToString()); }
    }
}
