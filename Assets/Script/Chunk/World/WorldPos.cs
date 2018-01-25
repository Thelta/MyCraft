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

    public override string ToString()
    {
        return string.Format("{0} {1} {2}", x, y, z);
    }

    public static float EuclideanDistance(WorldPos pos1, WorldPos pos2)
    {
        //I don't use - operator to avoid allocation
        int xDist = pos1.x - pos2.x;
        int yDist = pos1.y - pos2.y;
        int zDist = pos1.z - pos2.z;
        return Mathf.Sqrt(xDist * xDist + yDist * yDist + zDist * zDist);
    }

    public static int ManhattanDistance(WorldPos pos1, WorldPos pos2)
    {
        //I don't use - operator to avoid allocation
        int xDist = pos1.x - pos2.x;
        int yDist = pos1.y - pos2.y;
        int zDist = pos1.z - pos2.z;
        return xDist + yDist + zDist;
    }


    public static WorldPos operator -(WorldPos pos1, WorldPos pos2)
    {
        return new WorldPos(pos1.x - pos2.x, pos1.y - pos2.y, pos1.z - pos2.z);
    }

    public static WorldPos operator +(WorldPos pos1, WorldPos pos2)
    {
        
        return new WorldPos(pos1.x + pos2.x, pos1.y + pos2.y, pos1.z + pos2.z);
    }

    public static bool operator ==(WorldPos pos1, WorldPos pos2)
    {
        return (pos1.x == pos2.x) && (pos1.y == pos2.y) && (pos1.z == pos2.z);
    }

    public static bool operator !=(WorldPos pos1, WorldPos pos2)
    {
        return !(pos1 == pos2);
    }

}
