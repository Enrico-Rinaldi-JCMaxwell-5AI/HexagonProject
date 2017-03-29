using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {
    public int[] Joys;
    int presentJoy;
	// Use this for initialization
	void Start () {
        Joys = new int[4];
        backReset();
        presentJoy = 1;
    }

    public int numberOfPlayer()
    {
        int num=2;
        if (Joys[2] != -1)
        {
            num = 3;
        }
        if (Joys[3] != -1)
        {
            num = 4;
        }
        return num;
    }

    public void backReset()
    {
        Joys[0] = -1;
        Joys[1] = -1;
        Joys[2] = -1;
        Joys[3] = -1;
        presentJoy = 1;
    }

    public bool isReady()
    {
        return true;
        if (Joys[0] != -1 && Joys[1] != -1)
            return true;
        else
            return false;
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<MenuGUI>().menuState == 2 && GetComponent<MenuGUI>().isMenuActive)
        {
            for(int i = 1; i < 12; i++)
            {
                if(Input.GetKeyDown("joystick " + i + " button "+InputPreferences.getInput(presentJoy,0)))
                {
                    setJoy(i);
                }
            }
        }
    }

    void setJoy(int i)
    {
        for (int j = 0; j < 4; j++)
        {
            if(Joys[j]==-1 && Joys[0]!=i && Joys[1] != i && Joys[2] != i && Joys[3] != i)
            {
                Joys[j] = i;
                presentJoy++;
                return;
            }
        }
    }
}
