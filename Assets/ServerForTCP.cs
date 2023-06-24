using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class ServerForTCP : MonoBehaviour
{
    private TcpListener _listener = default;
    private TcpClient _client = default;

    private void Start()
    {
        StartServer();
    }

    /// <summary> 接続を開始する </summary>
    private void StartServer()
    {
        try
        {
            _listener = new(IPAddress.Any, 50007);
            _listener.Start();

            Debug.Log("server started...");

            _listener.BeginAcceptTcpClient(OnClientConnected, null);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    /// <summary> 接続時に実行される </summary>
    private void OnClientConnected(IAsyncResult result)
    {
        _client = _listener.EndAcceptTcpClient(result);

        Debug.Log($"client connected. IP : {((IPEndPoint)_client.Client.RemoteEndPoint).Address}");

        byte[] buffer = new byte[1024];
        _client.GetStream().BeginRead(buffer, 0, buffer.Length, OnDataReceived, buffer);
    }

    /// <summary> データ受信時に実行される </summary>
    private void OnDataReceived(IAsyncResult result)
    {
        int bytesRead = _client.GetStream().EndRead(result);
        byte[] buffer = (byte[])result.AsyncState;

        if (bytesRead <= 0)
        {
            Debug.Log("client disconnected");
            _client.Close();

            _listener.BeginAcceptTcpClient(OnClientConnected, null);
            return;
        }

        string receivedData = Encoding.ASCII.GetString(buffer, 0, bytesRead);
        Debug.Log($"received data : {receivedData}");

        SendDataToClient(receivedData);

        _client.GetStream().BeginRead(buffer, 0, buffer.Length, OnDataReceived, buffer);
    }

    /// <summary> クライアントにデータを送信する </summary>
    private void SendDataToClient(string data)
    {
        byte[] buffer = Encoding.ASCII.GetBytes(data);
        _client.GetStream().Write(buffer, 0, buffer.Length);
    }

    private void OnDestroy()
    {
        _client?.Close();
        _listener?.Stop();
    }
}
