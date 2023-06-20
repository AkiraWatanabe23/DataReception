using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class TCP : MonoBehaviour
{
    private TcpClient _client = default;

    DateTimeOffset _baseDT = new(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ConnectPython();
        }

        if (Input.GetKeyDown(KeyCode.Return) && _client != null)
        {
            ClosePython();
        }
    }

    /// <summary> 送信側に接続する </summary>
    public void ConnectPython()
    {
        // エンドポイントを設定する
        IPEndPoint RemoteEP = new(IPAddress.Any, 50007);

        // TcpListenerを作成する
        TcpListener Listener = new(RemoteEP);

        // TCP接続を待ち受ける
        Listener.Start();
        _client = Listener.AcceptTcpClient();

        // 接続ができれば、データをやり取りするストリームを保存する
        NetworkStream Stream = _client.GetStream();

        GetPID(Stream);

        // 接続を切る
        //_client.Close();
    }

    /// <summary> 接続を切る </summary>
    private void ClosePython()
    {
        _client.Close();
    }

    private void GetPID(NetworkStream Stream)
    {
        Byte[] data = new Byte[256];
        String responseData = String.Empty;

        // 接続先からデータを読み込む
        Int32 bytes = Stream.Read(data, 0, data.Length);

        // 読み込んだデータを文字列に変換する
        responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
        Debug.Log("PID: " + responseData);

        var unixTime = (DateTimeOffset.Now - _baseDT).Ticks;

        //Debug.Log(DateTime.Now.Ticks);
        Debug.Log(unixTime);

        // 受け取った文字列に文字を付け足して戻す
        Byte[] buffer = System.Text.Encoding.ASCII.GetBytes("responce: " + responseData);
        Stream.Write(buffer, 0, buffer.Length);
    }
}
