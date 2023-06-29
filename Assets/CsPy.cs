using System.Diagnostics;
using System.IO;
using UnityEngine;

/// <summary> C#スクリプトからPythonスクリプトを呼び出し、実行する </summary>
public class CsPy : MonoBehaviour
{
    //相対パスで実行ファイルのディレクトリを指定
    private string _exePath = "\"..\\AppData\\Local\\Programs\\Python\\Python311\\python.exe\"";
    private string _pyCodePath = "\"..\\Documents\\SocketCommunication\\output_test.py\"";

    private void Start()
    {
        ProcessStartInfo info = new()
        {
            FileName = _exePath,
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,

            Arguments = _pyCodePath,
        };

        Process process = Process.Start(info);

        StreamReader streamReader = process.StandardOutput;
        var data = streamReader.ReadLine();

        process.WaitForExit();
        process.Close();

        //UnityEngine.Debug.Log(Encoding.UTF8.GetBytes(data).ToString());
        UnityEngine.Debug.Log(data);
    }
}
