using System;
using AiTankArenaServer;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

namespace Script
{
    public abstract class DiffUpdateBehaviour : MonoBehaviour
    {
        private long lastTickId = -1;
        private void Update()
        {
            if (GameManager.TickId != lastTickId)
            {
                if (GameManager.GameState != null && GameManager.GameState.Stage == GameStage.InGame)
                {
                    lastTickId = GameManager.TickId;
                    DiffUpdate();
                }
            }
        }

        protected abstract void DiffUpdate();
    }
}