using System;
using System.IO;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using AiTankArenaServer;
using Google.Protobuf;
using UnityEngine;

namespace Script
{
    public class NetworkSerivce
    {
        private byte[] resBuffer;
        private ClientWebSocket ws;

        public NetworkSerivce()
        {
            resBuffer = new byte[2048];
            ws = new ClientWebSocket();
        }

        public async Task ConnectGameServer(string wsUrl)
        {
            Uri url = new Uri("ws://127.0.0.1:8081/");
            await ws.ConnectAsync(url, CancellationToken.None);
            if (ws.State == WebSocketState.Open) {
                Debug.Log("Connected");
            }
            else {
                Debug.LogError("Error when connecting");
                Debug.Log(ws);
            }
        }

        public async Task Send(Operation operation)
        {
            using (var outputStream = new System.IO.MemoryStream())
            {
                operation.WriteTo(outputStream);
                await ws.SendAsync(new ArraySegment<byte>(outputStream.GetBuffer(),0,(int)outputStream.Length), WebSocketMessageType.Binary,true,
                    CancellationToken.None);
            }
        }

        public async Task<Response> Recv()
        {
            ArraySegment<byte> buffer = new ArraySegment<byte>(resBuffer);
            var result = await ws.ReceiveAsync(buffer, CancellationToken.None);
            var res =Response.Parser.ParseFrom(buffer.Array,0,result.Count);
            return res;
        }
        public async Task<ServerInfo> GetServerInfo()
        {
            var opr = new Operation
            {
                Type = OperationType.ServerInfo
            };
            await Send(opr);
            var res = await Recv();
            return res.ServerInfo;
        }

        public async Task<bool> JoinGameSession(
            int roomId,
            string myPlayerName = "Unnamed Observer",
            PlayerType type = PlayerType.Observer
                )
        {
            var opr = new Operation
            {
                Type = OperationType.Join,
                JoinRoomInfo = new JoinRoomInfo
                {
                    MyPlayerName = myPlayerName,
                    RoomID = roomId,
                    Type = type
                }
            };
            await Send(opr);
            var res = await Recv();
            return res.Status == Status.Ok;
        }

    }
}