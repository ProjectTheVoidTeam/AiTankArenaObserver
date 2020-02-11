using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using AiTankArenaServer;
using Google.Protobuf.Collections;
using UnityEngine;

namespace Script
{
    public class GameManager : MonoBehaviour
    {
        public String gameServerWsUrl;
        public GameObject tankPrefab;
        
        [HideInInspector]
        public static long TickId = 0;

        public static GameState GameState = new GameState()
        {
            Stage = GameStage.InGame
        };

        private NetworkSerivce network;
        private Task stateUpdateTask;
        private readonly CancellationTokenSource stateUpdateCancellationTokenSource = new CancellationTokenSource();
        private Dictionary<uint, Player> tankidPlayerMap = new Dictionary<uint, Player>();
        private bool isInit = false;


        private void Awake()
        {
            network = new NetworkSerivce();
        }

        private async void Start()
        {
            await network.ConnectGameServer(gameServerWsUrl);
            var serverInfo = await network.GetServerInfo();
            Debug.Log(serverInfo.WelcomeMessage);

            var result = await network.JoinGameSession(0);
            Debug.Log(result ? "加入成功" : "加入失败");

            if (result)
            {
                StartStateUpdate();
            }
        }

        private void Update()
        {
        }

        public void StartStateUpdate()
        {
            if (stateUpdateTask != null)
            {
                stateUpdateTask = new Task(() =>
                {
                    while (!stateUpdateCancellationTokenSource.Token.IsCancellationRequested)
                    {
                        StateUpdate();
                    }
                });
                stateUpdateTask.Start();
            }
        }

        public void StopStateUpdate()
        {
            stateUpdateCancellationTokenSource.Cancel();
        }

        private void InitGame(RepeatedField<Player> players)
        {
            foreach (var player in players)
            {
                tankidPlayerMap[player.TankId] = player;
                var tank = (
                    from it in GameState.Tanks
                    where it.Id == player.TankId
                    select it
                ).First();
                Instantiate(tankPrefab, Util.toVector3(tank.Pos), Quaternion.Euler(0, 0, 0));
            }

            isInit = true;
        }

        private async void StateUpdate()
        {
            var res = await network.Recv();
            GameState = res.GameState;
            switch (GameState.Stage)
            {
                case GameStage.Waiting:
                    foreach (var player in res.Players)
                        tankidPlayerMap[player.TankId] = player;
                    break;
                case GameStage.AllReady:
                case GameStage.InGame:
                    if(!isInit)
                        InitGame(res.Players);

                    break;
                case GameStage.End:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}