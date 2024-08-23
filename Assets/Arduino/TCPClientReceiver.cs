using System;
using System.Collections.Specialized;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class TCPClientReceiver : MonoBehaviour
{
    public string serverIp = "10.65.54.224"; // Replace with Arduino's IP address
    public int serverPort = 8888; // Arduino server port
    public UserAlert userAlert; // Reference to the UserAlert script
    public GameObject targetObject; // The object to be manipulated
    public Vector3 objPosition;
    public Quaternion objRotation;

    private TcpClient client;
    private NetworkStream stream;
    private Thread clientThread;
    private string currentData = "AccelX=0.00&AccelY=0.00&AccelZ=0.00&GyroX=0.00&GyroY=0.00&GyroZ=0.00";

    private void Start()
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() => userAlert.displayMessage("Starting TCPClientReceiver..."));
        objPosition = targetObject.transform.position;
        objRotation = targetObject.transform.rotation;
        ConnectToServer();
    }
    void Update()
    {
        targetObject.transform.position = objPosition;
        targetObject.transform.rotation = Quaternion.Slerp(targetObject.transform.rotation, objRotation, Time.deltaTime);
    }
    private void ConnectToServer()
    {
        try
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => userAlert.displayMessage($"Attempting to connect to server at {serverIp}:{serverPort}..."));
            client = new TcpClient(serverIp, serverPort);
            stream = client.GetStream();
            clientThread = new Thread(new ThreadStart(ListenForData));
            clientThread.IsBackground = true;
            clientThread.Start();
            UnityMainThreadDispatcher.Instance().Enqueue(() => userAlert.displayMessage("Connected to server."));

            // Send initial message to wake up the server
            SendInitialMessage();

        }
        catch (Exception e)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => userAlert.displayMessage("Failed to connect to server: " + e.Message));
        }
    }

    private void SendInitialMessage()
    {
        try
        {
            byte[] initialMessage = Encoding.UTF8.GetBytes("Hello Server");
            stream.Write(initialMessage, 0, initialMessage.Length);
            UnityMainThreadDispatcher.Instance().Enqueue(() => userAlert.displayMessage("Initial message sent to server."));
        }
        catch (Exception e)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => userAlert.displayMessage("Failed to send initial message: " + e.Message));
        }
    }

    void ListenForData()
    {
        byte[] buffer = new byte[1024];
        while (true)
        {
            try
            {
               
                if (stream.CanRead)
                {
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead > 0)
                    {
                        string dataReceived = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        currentData = dataReceived; // Update the currentData with the latest received data
                        UnityMainThreadDispatcher.Instance().Enqueue(() => UpdateDisplay(currentData));
                        UpdateTransform(dataReceived);
                    }
                }
            }
            catch (Exception e)
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() => userAlert.displayMessage("Error while reading data: " + e.Message));
                break;
            }
            Thread.Sleep(600); // To avoid busy-waiting, adjust as needed
        }
    }

    private void UpdateDisplay(string data)
    {
        // Use the same text format, but update only the numbers
        UnityMainThreadDispatcher.Instance().Enqueue(() => userAlert.displayMessage(data));
    }

    private void UpdateTransform(string data)
    {
        try
        {
            // Split the data by & and extract the values
            string[] values = data.Split('&');
            if (values.Length >= 6)
            {
                // Extract accelerometer data
                float accelX = float.Parse(values[0].Split('=')[1]);
                float accelY = float.Parse(values[1].Split('=')[1]);
                float accelZ = float.Parse(values[2].Split('=')[1]);

                // Extract gyroscope data
                float gyroX = float.Parse(values[3].Split('=')[1]);
                float gyroY = float.Parse(values[4].Split('=')[1]);
                float gyroZ = float.Parse(values[5].Split('=')[1]);

                Vector3 lastData = new Vector3(accelX, accelY, accelZ);
                UnityEngine.Debug.Log(lastData.ToString());

                // Update rotation based on gyroscope data
                objRotation = Quaternion.Euler(gyroX*180f/3.1415f, gyroY * 180f / 3.1415f, gyroZ * 180f / 3.1415f);
            }
        }
        catch (Exception e)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => userAlert.displayMessage("Error while parsing data: " + e.Message));
        }
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

        UnityMainThreadDispatcher.Instance().Enqueue(() => userAlert.displayMessage("TCP Client closed"));
    }
}