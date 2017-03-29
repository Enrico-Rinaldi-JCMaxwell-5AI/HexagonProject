using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HostClientController : MonoBehaviour {
    public List<GameObject> allInGameObject = new List<GameObject>();
    public HostClientModel model;
    public HostClientView view;
    public ClientNManager nmanager;
    public Material Red;
    public Material Blue;
    public Material Green;
    public Material LedRed;
    public Material LedBlue;
    public Material LedOff;

    // Use this for initialization
    void Start () {
        model = GetComponent<HostClientModel>();
        view = GetComponent<HostClientView>();
        nmanager = GetComponent<ClientNManager>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void setCamera(int port)
    {
        switch(port)
        {
            case 1:
                {
                    GameObject.Find("Main Camera").GetComponent<Transform>().position = new Vector3(0, 4.28f, 9.49f);
                    GameObject.Find("Main Camera").GetComponent<Transform>().rotation = Quaternion.Euler(22.853f, -180, 0);
                }
                break;
            case 2:
                {
                    GameObject.Find("Main Camera").GetComponent<Transform>().position = new Vector3(-9.49f, 4.28f, 0);
                    GameObject.Find("Main Camera").GetComponent<Transform>().rotation = Quaternion.Euler(22.853f, 90, 0);
                }
                break;
            case 3:
                {
                    GameObject.Find("Main Camera").GetComponent<Transform>().position = new Vector3(9.49f, 4.28f, 0);
                    GameObject.Find("Main Camera").GetComponent<Transform>().rotation = Quaternion.Euler(22.853f, -90, 0);
                }
                break;
        }
    }

    public void startGame()
    {
        GameObject.Find("FrontBase").GetComponent<Renderer>().material = Blue;
        GameObject.Find("BackBase").GetComponent<Renderer>().material = Blue;
        if (model.clientData[2]!=null)
        {
            GameObject.Find("LeftPlane").GetComponent<Transform>().position = new Vector3(-4.72f, -51.86f, 0);
            GameObject.Find("LeftBase").GetComponent<Renderer>().material = Blue;
        }
        if (model.clientData[3] != null)
        {
            GameObject.Find("RightPlane").GetComponent<Transform>().position = new Vector3(4.72f, -51.86f, 0);
            GameObject.Find("RightBase").GetComponent<Renderer>().material = Blue;
        }
        GameObject.Find("Reflection Probe").GetComponent<ReflectionProbe>().RenderProbe();
        setStartingValue();
        setCamera(model.getMyPort());
    }

    public void setStartingValue()
    {
        for(int i=0;i<4;i++)
        {
            if(model.clientData[i]!=null)
            {
                model.clientData[i].balls = 60;
                model.clientData[i].isAlive = true;
            }
        }
    }

    public void resetMap()
    {
        for (int i = 0; i < allInGameObject.Count; i++)
        {
            Destroy(allInGameObject[i]);
        }
        allInGameObject = new List<GameObject>();
        GameObject.Find("FrontPlane").GetComponent<Transform>().position = new Vector3(0, -51.86f, 6);
        GameObject.Find("BackPlane").GetComponent<Transform>().position = new Vector3(0, -51.86f, -6);
        GameObject.Find("LeftPlane").GetComponent<Transform>().position = new Vector3(-6f, 2.375f, 0);
        GameObject.Find("RightPlane").GetComponent<Transform>().position = new Vector3(6f, 2.375f, 0);
        GameObject.Find("Turret").GetComponent<Transform>().position = new Vector3(0, -1f, 0);
        GameObject.Find("Turret").GetComponent<Transform>().rotation = Quaternion.Euler(0, 0, 0);
        GameObject.Find("Cannon 1").GetComponent<Transform>().rotation = Quaternion.Euler(0, 45, 0);
        GameObject.Find("Cannon 2").GetComponent<Transform>().rotation = Quaternion.Euler(0, -45, 0);
        GameObject.Find("Cannon 3").GetComponent<Transform>().rotation = Quaternion.Euler(0, 45, 0);
        GameObject.Find("Cannon 4").GetComponent<Transform>().rotation = Quaternion.Euler(0, -45, 0);
        GameObject.Find("FrontBase").GetComponent<Renderer>().material = Red;
        GameObject.Find("BackBase").GetComponent<Renderer>().material = Red;
        GameObject.Find("LeftBase").GetComponent<Renderer>().material = Red;
        GameObject.Find("RightBase").GetComponent<Renderer>().material = Red;
        GameObject.Find("SO Down Led").transform.GetChild(0).GetComponent<Renderer>().material = LedOff;
        GameObject.Find("NO Down Led").transform.GetChild(0).GetComponent<Renderer>().material = LedOff;
        GameObject.Find("NE Down Led").transform.GetChild(0).GetComponent<Renderer>().material = LedOff;
        GameObject.Find("SE Down Led").transform.GetChild(0).GetComponent<Renderer>().material = LedOff;
        GameObject.Find("Circle").transform.GetChild(0).GetComponent<Renderer>().material = LedOff;
        GameObject.Find("Main Camera").GetComponent<Transform>().position = new Vector3(0, 4.28f, -9.49f);
        GameObject.Find("Main Camera").GetComponent<Transform>().rotation = Quaternion.Euler(22.853f, 0, 0);
        GameObject.Find("Reflection Probe").GetComponent<ReflectionProbe>().RenderProbe();
        view.auxObject.GetComponent<Transform>().position = new Vector3(0, 0, 0);
        //DOPO TUTTE LE OPERAZIONI
        
    }

    public void changePortState(int port)
    {
        for (int i = 0; i < allInGameObject.Count; i++)
        {
            if (allInGameObject[i].name.Equals(port.ToString()))
            {
                Destroy(allInGameObject[i]);
                allInGameObject.RemoveAt(i);
            }
        }
        switch (port)
        {
            case 0:
                {
                    GameObject.Find("BackPlane").GetComponent<Transform>().position = new Vector3(0f, 2.375f, -6f);
                    GameObject.Find("BackBase").GetComponent<Transform>().GetComponent<Renderer>().material = Red;
                }
                break;
            case 1:
                {
                    GameObject.Find("FrontPlane").GetComponent<Transform>().position = new Vector3(0f, 2.375f, 6f);
                    GameObject.Find("FrontBase").GetComponent<Transform>().GetComponent<Renderer>().material = Red;
                }
                break;
            case 2:
                {
                    GameObject.Find("LeftPlane").GetComponent<Transform>().position = new Vector3(-6f, 2.375f, 0);
                    GameObject.Find("LeftBase").GetComponent<Transform>().GetComponent<Renderer>().material = Red;
                }
                break;
            case 3:
                {
                    GameObject.Find("RightPlane").GetComponent<Transform>().position = new Vector3(6f, 2.375f, 0);
                    GameObject.Find("RightBase").GetComponent<Transform>().GetComponent<Renderer>().material = Red;
                }
                break;
        }
        GameObject.Find("Reflection Probe").GetComponent<ReflectionProbe>().RenderProbe();
    }

    public void gameFinished(int winner)
    {
        switch (winner)
        {
            case 0:
                {
                    GameObject.Find("BackBase").GetComponent<Transform>().GetComponent<Renderer>().material = Green;
                }
                break;
            case 1:
                {
                    GameObject.Find("FrontBase").GetComponent<Transform>().GetComponent<Renderer>().material = Green;
                }
                break;
            case 2:
                {
                    GameObject.Find("LeftBase").GetComponent<Transform>().GetComponent<Renderer>().material = Green;
                }
                break;
            case 3:
                {
                    GameObject.Find("RightBase").GetComponent<Transform>().GetComponent<Renderer>().material = Green;
                }
                break;
        }
        GameObject.Find("Reflection Probe").GetComponent<ReflectionProbe>().RenderProbe();
        model.winner = winner;
        model.isGameFinished = true;
    }

    public void portDead(int portdead)
    {
        changePortState(portdead);
        model.clientData[portdead].isAlive = false;
    }
}
