using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class InputPreferences
{
    public static void setDefault()
    {
        for (int i = 1; i < 5; i++)
        {
            for (int j = 0; j < 20; j++)
            {
                PlayerPrefs.SetInt("J" + i + j, j);
            }
        }
        PlayerPrefs.SetInt("TA", 97);
        PlayerPrefs.SetInt("TD", 100);
        PlayerPrefs.SetInt("TSH", 306);
        PlayerPrefs.SetInt("TRE", 304);
        PlayerPrefs.SetInt("TES", 27);
    }

    public static void setInput(int nJoyPad, int nButton, int buttonPressed)
    {
        for (int i = 0; i < 20; i++)
        {
            if (PlayerPrefs.GetInt("J" + nJoyPad + i) == buttonPressed)
            {
                PlayerPrefs.SetInt("J" + nJoyPad + i, PlayerPrefs.GetInt("J" + nJoyPad + nButton));
                PlayerPrefs.SetInt("J" + nJoyPad + nButton, buttonPressed);
            }
        }
        PlayerPrefs.SetInt("J" + nJoyPad + nButton, buttonPressed);
    }

    public static int getInput(int nJoyPad, int nButton)
    {

        return PlayerPrefs.GetInt("J" + nJoyPad + nButton);

    }

    public static int getKeyBoardInput(string buttonName)
    {
        return PlayerPrefs.GetInt("T" + buttonName);
    }

    public static int getKeyId()
    {
        int e = System.Enum.GetNames(typeof(KeyCode)).Length;
        for (int i = 0; i < e; i++)
        {
            if (Input.GetKey((KeyCode)i))
            {
                return i;
            }
        }
        return -1;
    }

    public static void setKeyBoardInput(string buttonName,int buttonPressed)
    {
        PlayerPrefs.SetInt("T" + buttonName,buttonPressed);
    }
}

