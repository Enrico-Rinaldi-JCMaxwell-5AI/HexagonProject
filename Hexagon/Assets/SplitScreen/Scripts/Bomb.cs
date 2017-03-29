using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Bomb
{
    public int spawnTime;
    public bool isStatic;

    public Bomb(int spawnTime,bool isStatic)
    {
        this.spawnTime = spawnTime;
        this.isStatic = isStatic;
    }
}

