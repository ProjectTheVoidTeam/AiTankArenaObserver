using AiTankArenaServer;
using UnityEngine;

namespace Script
{
    public static class Util
    {
        public static Vector3 toVector3(Vector2D vector)
        {
            return new Vector3(vector.X,vector.Y,0);
        }
        public static Vector3 toVector3(Vector3D vector)
        {
            return new Vector3(vector.X,vector.Y,vector.Z);
        }
        
    }
}