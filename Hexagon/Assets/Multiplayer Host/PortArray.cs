using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class HostPortArray
{
    ClientSocket[] ports;
    public HostPortArray()
    {
        ports = new ClientSocket[3];
    }

    public ClientSocket Get(int id)
    {
        return ports[id];
    }

    public bool Exist(int id)
    {
        if (ports[id] != null)
            return true;
        else
            return false;
    }

    public void Set(ClientSocket Object, int id)
    {
        ports[id] = Object;
    }

    public void Add(ClientSocket Object)
    {
        for (int i = 0; i < 3; i++)
        {
            if (ports[i] == null)
            {
                ports[i] = Object;
                return;
            }
        }
    }

    public bool isFull()
    {

        for (int i = 0; i < 3; i++)
        {
            if (ports[i] == null)
            {
                return false;
            }
        }
        return true;
    }

    public int Count()
    {
        int total = 0;
        for (int i = 0; i < 3; i++)
        {
            if (ports[i] != null)
            {
                total++;
            }
        }
        return total;
    }

    public void Remove(ClientSocket Object, bool shift)
    {
        for (int i = 0; i < 3; i++)
        {
            if (ports[i] == Object)
            {
                if (shift)
                {
                    for (int j = i; j < 2; j++)
                    {
                        ports[i] = ports[i + 1];
                    }
                    ports[2] = null;
                }else
                {
                    ports[i] = null;
                }
             }   
         } 
    }

    public void RemoveAt(int index, bool shift)
    {
        if (shift)
        {
            for (int i = index; i < 2; i++)
            {
                ports[i] = ports[i + 1];
            }
            ports[2] = null;
        }
        else
        {
            ports[index] = null;
        }
    }
}

