using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class ServerForTCP : MonoBehaviour
{
    /// <summary> TCPListener ... TCP接続の待機、受け入れを行う </summary>
    private TcpListener _listener = default;
    /// <summary> TCPClient ... 接続したクライアント側の操作を行う </summary>
    private TcpClient _client = default;

    private DateTimeOffset _baseDT = new(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);

    private string _data = default;

    private void Start()
    {
        StartServer();
    }

    /// <summary> 接続を開始する </summary>
    private void StartServer()
    {
        try
        {
            //コンストラクタ ... (IPAddress addr, int port)
            //addr ... ローカルIPアドレス
            //port ... 待ち受けするポート番号
            _listener = new(IPAddress.Any, 50007);
            //クライアントからの接続待機開始
            _listener.Start();

            Debug.Log("server started...");

            //クライアントの接続待機開始（非同期）
            //接続できたタイミングで、指定したコールバック関数が呼ばれる
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

        //取得したデータの読み込み（受信）
        //完了したら、コールバック関数を実行する
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

        //追記
        //=================================================
        if (long.TryParse(receivedData, out long time))
        {
            var unixTime = (DateTimeOffset.Now - _baseDT).Ticks * 100;
            var diff = Calculation(unixTime, time);

            Debug.Log(diff);
            _data += diff.ToString() + "\n";
        }
        //=================================================

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

    /// <summary> 誤差を計算 </summary>
    private long Calculation(long unix, long time)
    {
        return unix - time;
    }

    private void OnDestroy()
    {
        //接続を終了し、結果を出力
        _client?.Close();
        _listener?.Stop();

        Debug.Log(_data);
    }
}
