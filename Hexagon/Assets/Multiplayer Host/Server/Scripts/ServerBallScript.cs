using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerBallScript : MonoBehaviour {

    public bool ground = false;
    GameObject gm;
    public bool rest = false;
    public PhysicMaterial bounceMat;
    // Use this for initialization
    void Start()
    {
        gm = GameObject.Find("HostGameManager(Clone)");
    }

    // Update is called once per frame
    void Update()
    {
        if (ground)
        {
            float velocity = Mathf.Sqrt(Mathf.Pow(GetComponent<Rigidbody>().velocity.x, 2) + Mathf.Pow(GetComponent<Rigidbody>().velocity.z, 2));
            if (velocity < (SplitScreenModel.getMultiplier(gm.GetComponent<ServerGameModel>().gameTime)) + SplitScreenModel.ballMinSpeed)
            {
                GetComponent<Rigidbody>().velocity = new Vector3(GetComponent<Rigidbody>().velocity.x * SplitScreenModel.ballAccellerationValue, 0, GetComponent<Rigidbody>().velocity.z * SplitScreenModel.ballAccellerationValue);
            }
            if (velocity > (SplitScreenModel.getMultiplier(gm.GetComponent<ServerGameModel>().gameTime)) + SplitScreenModel.ballMaxSpeed)
            {
                GetComponent<Rigidbody>().velocity = new Vector3(GetComponent<Rigidbody>().velocity.x * SplitScreenModel.ballDecellerationValue, 0, GetComponent<Rigidbody>().velocity.z * SplitScreenModel.ballDecellerationValue);
            }
        }
    }

    private void FixedUpdate()
    {
        if (transform.position.y < 0.30f && !ground)
        {
            GetComponent<Transform>().position = new Vector3(transform.position.x, 0.21f, transform.position.z);
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionY;
            ground = true;
            gm.GetComponent<ServerGameController>().centralCannonStop = false;
            GetComponent<SphereCollider>().material = bounceMat;
        }
    }
}