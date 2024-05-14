using UnityEngine;
using TMPro;
using System.Net.Sockets;
using System.Text;
using System.Net;

public class UDPListener : MonoBehaviour
{
    public int listenPort = 8888; // UDP port to listen on
    public UserAlert userAlert; // Reference to the UserAlert script
    private UdpClient udpClient;
    private IPEndPoint remoteEndPoint;

    void Start()
    {
        // Initialize UDP client and remote endpoint
        udpClient = new UdpClient(listenPort);
        remoteEndPoint = new IPEndPoint(System.Net.IPAddress.Any, 0);
        userAlert.displayMessage("established endpoint connection");

        // Start listening for UDP packets in a separate thread
        udpClient.BeginReceive(ReceiveCallback, null);
    }

    // Callback function to handle received UDP packets
    private void ReceiveCallback(System.IAsyncResult ar)
    {
        // Get the received data and remote endpoint
        byte[] receivedBytes = udpClient.EndReceive(ar, ref remoteEndPoint);
        string receivedString = System.Text.Encoding.ASCII.GetString(receivedBytes);
        userAlert.displayMessage(receivedString);

        float temperature;
        if (float.TryParse(receivedString, out temperature))
        {
            // Display temperature data using UserAlert script
            string message = "Temperature: " + temperature.ToString("F2") + " °C";
            userAlert.displayMessage(message);
        }

        // Continue listening for more UDP packets
        udpClient.BeginReceive(ReceiveCallback, null);
    }

    private void OnDestroy()
    {
        if (udpClient != null)
        {
            udpClient.Close();
        }
    }

    private void OnApplicationQuit()
    {
        // Close UDP client when the application quits
        if (udpClient != null)
        {
            udpClient.Close();
        }
    }
}
