using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class TCP : MonoBehaviour
{
    DateTimeOffset _baseDT = new(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);

    private string _data = default;

    private void OnDisable()
    {
        Debug.Log(_data);
    }

    private void Start()
    {
        ConnectPython();
    }

    /// <summary> 送信側に接続する </summary>
    private void ConnectPython()
    {
        Debug.Log("try to connect");

        // TcpListenerを作成する
        TcpListener Listener = new(IPAddress.Any, 50007);

        // TCP接続を待ち受ける
        Listener.Start();
        var Client = Listener.AcceptTcpClient();

        // 接続ができれば、データをやり取りするストリームを保存する
        NetworkStream Stream = Client.GetStream();

        GetPID(Stream);

        // 接続を切る
        if (Input.GetKeyDown(KeyCode.Space)) Client.Close();
    }

    private void GetPID(NetworkStream Stream)
    {
        Byte[] data = new Byte[256];

        // 接続先からデータを読み込む
        Int32 bytes = Stream.Read(data, 0, data.Length);

        // 読み込んだデータを文字列に変換する
        string responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
        Debug.Log("PID: " + responseData);

        var unixTime = (DateTimeOffset.Now - _baseDT).Ticks;

        //Debug.Log(Calculation(unixTime, long.Parse(responseData)));
        //_data += Calculation(unixTime, long.Parse(responseData)).ToString() + "\n";

        // 受け取った文字列に文字を付け足して戻す
        Byte[] buffer = System.Text.Encoding.ASCII.GetBytes("responce: " + responseData);
        Stream.Write(buffer, 0, buffer.Length);
    }

    private long Calculation(long unix, long time)
    {
        return unix - time;
    }
}
