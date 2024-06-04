using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using TMPro;

public class TCPClientReceiver : MonoBehaviour
{
    public string serverIp = "your_arduino_ip"; // Replace with Arduino's IP address
    public int serverPort = 8888; // Arduino server port
    public TextMeshProUGUI temperatureDisplayText; // Assign in inspector

    private TcpClient client;
    private NetworkStream stream;
    private Thread clientThread;

    private void Start()
    {
        ConnectToServer();
    }

    private void ConnectToServer()
    {
        try
        {
            client = new TcpClient(serverIp, serverPort);
            stream = client.GetStream();
            clientThread = new Thread(new ThreadStart(ListenForData));
            clientThread.IsBackground = true;
            clientThread.Start();
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to connect to server: " + e.Message);
        }
    }

    private void ListenForData()
    {
        byte[] buffer = new byte[1024];
        while (true)
        {
            if (stream.CanRead)
            {
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                if (bytesRead > 0)
                {
                    string dataReceived = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Debug.Log("Data received: " + dataReceived);
                    UpdateTemperatureDisplay(dataReceived);
                }
            }
            Thread.Sleep(1000); // To avoid busy-waiting, adjust as needed
        }
    }

    private void UpdateTemperatureDisplay(string data)
    {
        // Use Unity's main thread to update the UI
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            if (temperatureDisplayText != null)
            {
                temperatureDisplayText.text = "Temperature: " + data;
            }
        });
    }

    private void OnApplicationQuit()
    {
        CloseConnection();
    }

    private void OnDestroy()
    {
        CloseConnection();
    }

    private void CloseConnection()
    {
        if (clientThread != null)
        {
            clientThread.Abort();
        }

        if (stream != null)
        {
            stream.Close();
        }

        if (client != null)
        {
            client.Close();
        }

        Debug.Log("TCP Client closed");
    }
}
