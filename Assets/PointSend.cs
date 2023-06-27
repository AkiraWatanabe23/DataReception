using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

/// <summary> クリックした位置のVector3を送信してみる </summary>
public class PointSend : MonoBehaviour
{
    private TcpListener _listener = default;
    private TcpClient _client = default;

    private Vector3 _inputPos = Vector3.zero;
    private bool _isConnected = false;

    private void Start()
    {
        //StartServer();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !_isConnected)
        {
            StartServer();
            _inputPos = Input.mousePosition;
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
        _isConnected = true;

        //BeginAcceptTcpClient()の結果を取得する
        _client = _listener.EndAcceptTcpClient(ar);

        Debug.Log($"client connected. IP : {((IPEndPoint)_client.Client.RemoteEndPoint).Address}");

        //配列を定義し、読み込み完了したら OnDataReceived() を実行する
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
            Debug.Log("client disconnected");
            _client.Close();
            _isConnected = false;

            //再接続
            _listener.BeginAcceptTcpClient(OnClientConnected, null);
            return;
        }

        //クライアントに送るデータ
        string receivedData = Encoding.ASCII.GetString(buffer, 0, bytesRead);
        //Debug.Log($"received data : {receivedData}");

        SendClickPosData(_inputPos);

        //ここで再帰呼び出ししてる
        //→ TCPではデータの受信を行った際、「次のデータを受信する準備」をする必要があるため
        //   再帰的に呼び出してデータの受信を非同期で行う
        _client.GetStream().BeginRead(buffer, 0, buffer.Length, OnDataReceived, buffer);
    }

    /// <summary> クライアントにデータを送信する </summary>
    private void SendClickPosData(Vector3 pos)
    {
        Debug.Log(pos);
        string data = pos.ToString();

        //文字列をbyte[]に変換して、データを送る
        byte[] buffer = Encoding.ASCII.GetBytes(data);
        _client.GetStream().Write(buffer, 0, buffer.Length);

        _isConnected = false;
    }

    private void OnDestroy()
    {
        //接続を終了し、結果を出力
        _client?.Close();
        _listener?.Stop();
    }
}
