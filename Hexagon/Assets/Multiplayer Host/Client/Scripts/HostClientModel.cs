using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HostClientModel : MonoBehaviour {
    public ClientSideData[] clientData;
    public ClientNManager nmanager;
    public Object[] objectPool = new Object[4];
    public string remoteIP = "127.0.0.1";
    public string port = "30000";
    public string username;
    public bool isGameStarted = false;
    public bool isGameFinished = false;
    public int winner;
    public bool usernameAk = false;
    public int confirmedPort;
    // Use this for initialization
    void Start () {
        nmanager = GetComponent<ClientNManager>();
        clientData = new ClientSideData[4];
    }

    public void resetClientData()
    {
        clientData[0] = null;
        clientData[1] = null;
        clientData[2] = null;
        clientData[3] = null;
    }

    public void shiftClientDataAt(int n)
    {
        for(int i = n; i < 3; i++)
        {
            clientData[i] = clientData[i + 1];
        }
        clientData[3] = new ClientSideData();
    }

    public int getMyPort()
    {
        for(int i=0;i<4;i++)
        {
            if(clientData[i]!=null && username.Equals(clientData[i].username))
            {
                return i;
            }
        }
        return -1;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
