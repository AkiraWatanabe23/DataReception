using System.Net.Sockets;
using System.Net;
using System.Text;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary> クリックした位置のVector3を送信してみる </summary>
public class PointSend : MonoBehaviour
{
    private TcpListener _listener = default;
    private TcpClient _client = default;

    private void Start()
    {
        StartServer();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var pos = Input.mousePosition;
            SendClickPosData(pos);
        }
    }

    /// <summary> 接続を開始する </summary>
    private void StartServer()
    {
        try
        {
            _listener = new(IPAddress.Any, 50007);
            //クライアントからの接続待機開始
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
    private void OnClientConnected(IAsyncResult ar)
    {
        //BeginAcceptTcpClient()の結果を取得する
        _client = _listener.EndAcceptTcpClient(ar);

        //Debug.Log($"client connected. IP : {((IPEndPoint)_client.Client.RemoteEndPoint).Address}");

        byte[] buffer = new byte[1024];
        _client.GetStream().BeginRead(buffer, 0, buffer.Length, OnDataReceived, buffer);
    }

    /// <summary> データ受信時に実行される </summary>
    private void OnDataReceived(IAsyncResult ar)
    {
        //読み込まれたデータのbyte数
        int bytesRead = _client.GetStream().EndRead(ar);
        byte[] buffer = (byte[])ar.AsyncState;

        //データが空だったら
        if (bytesRead <= 0)
        {
            //Debug.Log("client disconnected");
            _client.Close();

            //再接続
            _listener.BeginAcceptTcpClient(OnClientConnected, null);
            return;
        }

        //クライアントに送るデータ
        string receivedData = Encoding.ASCII.GetString(buffer, 0, bytesRead);
        //Debug.Log($"received data : {receivedData}");

        SendDataToClient(receivedData);

        //取得したデータの読み込み（受信）
        //完了したら、コールバック関数を実行する
        _client.GetStream().BeginRead(buffer, 0, buffer.Length, OnDataReceived, buffer);
    }

    /// <summary> クライアントにデータを送信する </summary>
    private void SendDataToClient(string data)
    {
        //文字列をbyte[]に変換して、データを送る
        byte[] buffer = Encoding.ASCII.GetBytes(data);
        _client.GetStream().Write(buffer, 0, buffer.Length);
    }

    private void SendClickPosData(Vector3 pos)
    {
        Debug.Log(pos);
        string data = pos.ToString();

        byte[] buffer = Encoding.ASCII.GetBytes(data);
        _client.GetStream().Write(buffer, 0, buffer.Length);
    }

    private void OnDestroy()
    {
        //接続を終了し、結果を出力
        _client?.Close();
        _listener?.Stop();
    }
}
