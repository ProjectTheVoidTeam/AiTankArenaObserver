using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using AiTankArenaServer;
using Google.Protobuf;
using UnityEngine;

public class WebSocketTest : MonoBehaviour
{
    // Start is called before the first frame update
    async void  Start()
    {
        var bytes = new byte[1024];
        ClientWebSocket ws = new ClientWebSocket();
        CancellationToken ct = new CancellationToken();
        Uri url = new Uri("ws://127.0.0.1:8081/");
        await ws.ConnectAsync(url,ct);
        
        
        Operation operation = new Operation();
        operation.Type = OperationType.ServerInfo;
        using (var outputStream= new System.IO.MemoryStream())
        {
            operation.WriteTo(outputStream);
            await ws.SendAsync(new ArraySegment<byte>(outputStream.GetBuffer(),0,(int)outputStream.Length), WebSocketMessageType.Binary,true,
                CancellationToken.None);
        }
        ArraySegment<byte> buffer = new ArraySegment<byte>(bytes);
        var result = await ws.ReceiveAsync(buffer, CancellationToken.None);
        var res =Response.Parser.ParseFrom(buffer.Array,0,result.Count);
        Debug.Log(res.ServerInfo.WelcomeMessage);
        
        
        Debug.Log(ws);
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
