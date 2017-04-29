using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class PacketSize
{
    public static int getPacketLenght(byte[] header)
    {
        if(header[0] == 0x00 && header[1] == 0x00)
        {
            return 16;
        }
        if (header[0] == 0x00 && header[1] == 0x01)
        {
            return 0;
        }
        if (header[0] == 0x00 && header[1] == 0x02)
        {
            return 16;
        }
        if (header[0] == 0x00 && header[1] == 0x03)
        {
            return 17;
        }
        if (header[0] == 0x00 && header[1] == 0x04)
        {
            return 1;
        }
        if (header[0] == 0x00 && header[1] == 0x05)
        {
            return 0;
        }
        if (header[0] == 0x00 && header[1] == 0x06)
        {
            return 1;
        }
        if (header[0] == 0x00 && header[1] == 0x07)
        {
            return 1;
        }
        if (header[0] == 0x00 && header[1] == 0x08)
        {
            return 14;
        }
        if (header[0] == 0x00 && header[1] == 0x09)
        {
            return 13;
        }
        if (header[0] == 0x00 && header[1] == 0x0a)
        {
            return 1;
        }
        if (header[0] == 0x00 && header[1] == 0x0b)
        {
            return 1;
        }
        if (header[0] == 0x00 && header[1] == 0x0c)
        {
            return 4;
        }
        if (header[0] == 0x00 && header[1] == 0x0d)
        {
            return 8;
        }
        if (header[0] == 0x00 && header[1] == 0x0e)
        {
            return 0;
        }
        if (header[0] == 0x00 && header[1] == 0x0f)
        {
            return 0;
        }
        if (header[0] == 0x00 && header[1] == 0x10)
        {
            return 1;
        }
        if (header[0] == 0x00 && header[1] == 0x11)
        {
            return 1;
        }
        if (header[0] == 0x00 && header[1] == 0x12)
        {
            return 1;
        }
        if (header[0] == 0x00 && header[1] == 0x13)
        {
            return 2;
        }
        if (header[0] == 0x00 && header[1] == 0x14)
        {
            return 2;
        }
        if (header[0] == 0x00 && header[1] == 0x15)
        {
            return 1;
        }
        if (header[0] == 0x00 && header[1] == 0x16)
        {
            return 0;
        }
        if (header[0] == 0x00 && header[1] == 0x17)
        {
            return 0;
        }
        if (header[0] == 0x00 && header[1] == 0x18)
        {
            return 9;
        }
        if (header[0] == 0x00 && header[1] == 0x19)
        {
            return 5;
        }
        return 0;
    }

    public static string composeString(string user)
    {
        string basestring = "                ";
        return user + basestring.Substring(0, 16 - user.Length);
    }

    public static string decomposeString(string user)
    {
        char[] charuser = user.ToCharArray();
        string returnstring = "";
        for(int i=0;i<16;i++)
        {
            if(charuser[i]!=' ')
            {
                returnstring = returnstring + charuser[i];
            }else
            {
                return returnstring;
            }
        }
        return returnstring;
    }
}

