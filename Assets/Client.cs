using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class Client : MonoBehaviour
{
    private TcpClient _client = default;
    private NetworkStream _stream = default;

    private const string SERVER_IP = "127.0.0.1";
    private const int SERVER_PORT = 50007;

    private void Start()
    {
        _client = new(SERVER_IP, SERVER_PORT);
        _stream = _client.GetStream();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var inputPos = Input.mousePosition;
            Debug.Log(inputPos);

            byte[] bytes = Encoding.UTF8.GetBytes(inputPos.ToString());
            _stream.Write(bytes, 0, bytes.Length);
        }
    }

    private void OnDestroy()
    {
        Debug.Log("終了します");
        _client.Close();
        _stream.Close();
    }
}
