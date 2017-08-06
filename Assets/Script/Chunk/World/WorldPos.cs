using UnityEngine;
using System.Collections;

public class WorldPos
{
    public int x, y, z;

    public WorldPos()
    {
        this.x = 0; this.y = 0; this.z = 0;
    }

    public WorldPos(int x, int y, int z)
    {
        this.x = x; this.y = y; this.z = z;
    }

    public override bool Equals(object obj)
    {
        if (GetHashCode() == obj.GetHashCode())
             return true;
         return false;
    }

    public void Set(int x, int y, int z)
    {
        this.x = x; this.y = y; this.z = z;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 47;

            hash = hash * 227 + x.GetHashCode();
            hash = hash * 227 + y.GetHashCode();
            hash = hash * 227 + z.GetHashCode();

            return hash;
        }
    }

    public static WorldPos operator -(WorldPos c1, WorldPos c2)
    {
        return new WorldPos(c1.x - c2.x, c1.y - c2.y, c1.z - c2.z);
    }

    public static WorldPos operator +(WorldPos c1, WorldPos c2)
    {
        
        return new WorldPos(c1.x + c2.x, c1.y + c2.y, c1.z + c2.z);
    }

    public void print()
    {
        Debug.Log(x + " " + y + " " + z + " ");
    }

}
