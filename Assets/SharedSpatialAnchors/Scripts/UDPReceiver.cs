using System;
using System.Net;
using System.Net.Sockets;
using TMPro;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class UDPReceiver : MonoBehaviour
{
    Thread receiveThread;
    UdpClient client;
    public int port = 8888;  // Ensure this matches the Arduino sending port

    private string lastReceived = "Waiting for data...";  // Store the last received temperature data

    public TMP_Text temperatureDisplayText;  // Assign this in the inspector if you're using a UI Text element

    void Start()
    {
        InitUDP();
    }

    private void InitUDP()
    {
        client = new UdpClient(port);
        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    private void ReceiveData()
    {
        IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
        while (true)
        {
            try
            {
                byte[] data = client.Receive(ref anyIP);
                lastReceived = Encoding.UTF8.GetString(data);
                Debug.Log(">> " + lastReceived);
                UpdateTemperatureDisplay(lastReceived);
            }
            catch (Exception err)
            {
                Debug.LogError(err.ToString());
                break;  // Exit the loop on exception to avoid an infinite logging of exceptions
            }
        }
    }

    // Call this method to update the UI text
    void UpdateTemperatureDisplay(string temperature)
    {
        if (temperatureDisplayText != null)
        {
            temperatureDisplayText.text = "Temperature: " + temperature + "°C";
        }

        Debug.Log(temperature);
    }

    void OnApplicationQuit()
    {
        CloseUDP();
    }

    private void CloseUDP()
    {
        receiveThread?.Abort();
        client?.Close();
        Debug.Log("UDP Receiver closed");
    }

    void OnDestroy()
    {
        CloseUDP();  // Ensure resources are released when the GameObject is destroyed
    }
}
