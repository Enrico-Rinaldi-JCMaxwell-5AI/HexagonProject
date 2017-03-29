using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LimitController : MonoBehaviour {
    public int port;
    public GameObject particles;
	// Use this for initialization
	void Start () {
        port = Int32.Parse(name.ToCharArray()[1].ToString());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter(Collision collision)
    {
        try
        {
            if (collision.gameObject.name.Equals("Ball(Clone)"))
            {
                if(!GameObject.Find("SplitScreenGameManager").GetComponent<GameController>().isGameFinished)
                GameObject.Find("SplitScreenGameManager").GetComponent<GameController>().decreaseBall(port);
                GameObject.Find("SplitScreenGameManager").GetComponent<GameController>().inGameObjects.Remove(collision.gameObject);
                GameObject.Find("SplitScreenGameManager").GetComponent<GameController>().inGameObjects.Add((GameObject)Instantiate(particles, collision.gameObject.transform.position,Quaternion.Euler(-90,0,0)));
                Destroy(collision.gameObject);
                GameObject.Find("SplitScreenGameManager").GetComponent<GameController>().ballsInGame--;
            }
        } catch(Exception)
        {
            
        }
        try
        {
            if (!GameObject.Find("HostGameManager(Clone)").GetComponent<ServerGameModel>().isGameFinished)
            {
                if (port == 0)
                {
                    GameObject.Find("HostGameManager(Clone)").GetComponent<NetworkManager>().server.ownerData.balls--;
                    GameObject.Find("HostGameManager(Clone)").GetComponent<NetworkManager>().sendRefreshPortBalls(0, GameObject.Find("HostGameManager(Clone)").GetComponent<NetworkManager>().server.ownerData.balls);
                }
                else
                {
                    GameObject.Find("HostGameManager(Clone)").GetComponent<NetworkManager>().server.clientSocketList.Get(port - 1).clientData.balls--;
                    GameObject.Find("HostGameManager(Clone)").GetComponent<NetworkManager>().sendRefreshPortBalls(port, GameObject.Find("HostGameManager(Clone)").GetComponent<NetworkManager>().server.clientSocketList.Get(port - 1).clientData.balls);
                }
            }
            GameObject.Find("HostGameManager(Clone)").GetComponent<ServerGameController>().allInGameObjects.Remove(collision.gameObject);
            GameObject.Find("HostGameManager(Clone)").GetComponent<NetworkManager>().sendDestroyObject(Int32.Parse(collision.gameObject.name));
            Destroy(collision.gameObject);
            GameObject.Find("HostGameManager(Clone)").GetComponent<ServerGameController>().ballsInGame--;
            
        }
        catch (Exception)
        {
            
        }
    }
}
