using System.Linq;
using AiTankArenaServer;
using UnityEngine;

namespace Script
{
    public class TankBehaviour : MonoBehaviour
    {
        public uint id;

        void Update()
        {
            var self =
            (
                from tank in GameManager.GameState.Tanks
                where (tank.Id == id)
                select tank
            ).First();


            transform.position = Vector3.Lerp(transform.position, Util.toVector3(self.Pos), 0.3f);
        }

    }
    
}