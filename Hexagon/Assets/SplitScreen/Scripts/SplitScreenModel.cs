using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class SplitScreenModel
{
    public static float ballMinSpeed = 4f;
    public static float ballMaxSpeed = 12f;
    public static float ballAccellerationValue = 1.02f;
    public static float ballDecellerationValue = 0.98f;
    public static float bombStartPower = 1000f;
    public static float bombExplosionRadius = 5f;
    public static float bombExplosionFuse = 5;
    public static float joySensibility = 12f;
    public static int maxStartBalls = 60;
    public static int preGameSeconds = 3;
    public static int changeDirectionBallInterval = 3;
    public static float launchBallMinSpeed = 4.5f;
    public static float launchBallMaxSpeed = 9.5f;
    public static float baseCentralCannonPower = 6f;
    public static int centralCannonInterval = 45;
    public static float centralCannonDuration = 10;
    public static float elevationSpeedCentralCannon = 0.02f; //For every fixed frame (20ms)
    public static float degreesSpeedCentralCannonRotation = 10.8f; //For every fixed frame (20ms)
    public static float degreesSpeedLateralCannonRotation = 0.5f; //For every fixed frame (20ms)
    public static float shieldDuration = 3f;
    public static float shieldCooldown = 15f;
    public static float repulsionCooldown = 2.3f;
    public static float repulsionCooldownReduced = 0.8f;
    public static int endGameBombIntervalMin = 3;
    public static int endGameBombIntervalMax = 20;
    public static float maxRepulsionScale = 2f;
    public static float speedRepulsionGrow = 0.2f;  //For every fixed frame (20ms)
    public static float repulsionBallSpeed = 2f;

    public static int maxBalls(int currentTime)
    {

        if (currentTime > 2)
        {
            if (currentTime < 12)
            {
                return 1;
            }
            if (currentTime < 30)
            {
                return 2;
            }
            if (currentTime < 60)
            {
                return 3;
            }
            if (currentTime < 150)
            {
                return 4;
            }
            if (currentTime < 300)
            {
                return 5;
            }
            if (currentTime > 300)
            {
                return 6;
            }
        }
        return 0;
    }

    public static float getMultiplier(float currentTime)
    {
        return 1 + Mathf.Pow(currentTime / 300, 2);
    }

    public static List<Bomb> mapExplosions()
    {
        List<Bomb> bombTimes = new List<Bomb>();
        bombTimes.Add(new Bomb(Random.Range(25, 50), true));
        bombTimes.Add(new Bomb(Random.Range(50, 80), true));
        bombTimes.Add(new Bomb(Random.Range(50, 80), true));
        for (int i = 3; i < 8; i++)
        {
            bombTimes.Add(new Bomb(Random.Range(80, 150), true));
        }
        int numberOfStatic = 5;
        int numberOfDynamic = 5;
        for (int i = 8; i < 18; i++)
        {
            int choose = Random.Range(0, 2);
            if (choose == 0 && numberOfStatic != 0)
            {
                bombTimes.Add(new Bomb(Random.Range(150 + ((i - 8) * 14), 180 + ((i - 8) * 14)), true));
                numberOfStatic--;
            }
            else
            {
                if (numberOfDynamic != 0)
                    bombTimes.Add(new Bomb(Random.Range(150 + ((i - 8) * 14), 180 + ((i - 8) * 14)), false));
                else
                    bombTimes.Add(new Bomb(Random.Range(150 + ((i - 8) * 14), 180 + ((i - 8) * 14)), true));
            }
        }
        return bombTimes;
    }
}

