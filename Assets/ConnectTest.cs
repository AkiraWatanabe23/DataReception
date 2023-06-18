using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class ConnectTest : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            ConnectPython();
        }
    }


    public void ConnectPython()
    {
        // エンドポイントを設定する
        IPEndPoint RemoteEP = new(IPAddress.Any, 50007);

        // TcpListenerを作成する
        TcpListener Listener = new(RemoteEP);

        // TCP接続を待ち受ける
        Listener.Start();
        TcpClient Client = Listener.AcceptTcpClient();

        // 接続ができれば、データをやり取りするストリームを保存する
        NetworkStream Stream = Client.GetStream();

        GetPID(Stream);

        // 接続を切る
        Client.Close();
    }


    void GetPID(NetworkStream Stream)
    {
        Byte[] data = new Byte[256];
        String responseData = String.Empty;

        // 接続先からデータを読み込む
        Int32 bytes = Stream.Read(data, 0, data.Length);

        // 読み込んだデータを文字列に変換する
        responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
        UnityEngine.Debug.Log("PID: " + responseData);

        // 受け取った文字列に文字を付け足して戻す
        Byte[] buffer = System.Text.Encoding.ASCII.GetBytes("responce: " + responseData);
        Stream.Write(buffer, 0, buffer.Length);
    }
}
