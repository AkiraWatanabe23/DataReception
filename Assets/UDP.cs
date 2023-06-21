using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class UDP : MonoBehaviour
{
    private static UdpClient _udp = default;

    DateTimeOffset _baseDT = new(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);

    private string _data = default;

    private void Start()
    {
        int LOCA_PORT = 50007;

        _udp = new UdpClient(LOCA_PORT);
        _udp.Client.ReceiveTimeout = 2000;
    }

    private void OnDisable()
    {
        Debug.Log(_data);
    }

    private void Update()
    {
        IPEndPoint remoteEP = null;

        //UdpClientからデータを受け取る
        byte[] data = _udp.Receive(ref remoteEP);
        //stringに変換し、出力
        string text = Encoding.UTF8.GetString(data);

        var unixTime = (DateTimeOffset.Now - _baseDT).Ticks * 100;

        //Debug.Log(text);
        //Debug.Log(unixTime);

        Debug.Log(Calculation(unixTime, long.Parse(text)));
        _data += Calculation(unixTime, long.Parse(text)).ToString() + "\n";
    }

    private long Calculation(long unix, long time)
    {
        return unix - time;
    }
}
