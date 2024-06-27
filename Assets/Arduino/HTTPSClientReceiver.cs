using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class HTTPSClientReceiver : MonoBehaviour
{
    public string serverUrl = "https://your.quest3.server/data"; // Server URL
    public UserAlert userAlert;
    public Transform targetTransform; // Target object

    private HttpClient httpClient;
    private Vector3 lastData = Vector3.zero;

    private void Start()
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() => userAlert.displayMessage("Starting HTTPSClientReceiver..."));
        httpClient = new HttpClient();
        StartReceivingData();
    }

    private async void StartReceivingData()
    {
        while (true)
        {
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(serverUrl);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                UnityMainThreadDispatcher.Instance().Enqueue(() => userAlert.displayMessage(responseBody));
                UpdateTransform(responseBody);
            }
            catch (Exception e)
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() => userAlert.displayMessage("Error while receiving data: " + e.Message));
            }
            await Task.Delay(1000); // Adjust delay as needed
        }
    }

    private void UpdateTransform(string data)
    {
        // Parse data string and update transform
        // Assuming data is in format "AccelX=value&AccelY=value&AccelZ=value&GyroX=value&GyroY=value&GyroZ=value"
        var values = data.Split('&');
        float accelX = float.Parse(values[0].Split('=')[1]);
        float accelY = float.Parse(values[1].Split('=')[1]);
        float accelZ = float.Parse(values[2].Split('=')[1]);
        float gyroX = float.Parse(values[3].Split('=')[1]);
        float gyroY = float.Parse(values[4].Split('=')[1]);
        float gyroZ = float.Parse(values[5].Split('=')[1]);

        // Update rotation transform
        lastData = new Vector3(accelX, accelY, accelZ);
        targetTransform.rotation = Quaternion.Slerp(targetTransform.rotation, Quaternion.Euler(lastData), Time.deltaTime * 2f);
    }

    private void OnApplicationQuit()
    {
        httpClient.Dispose();
        UnityMainThreadDispatcher.Instance().Enqueue(() => userAlert.displayMessage("HTTPS Client closed"));
    }

    private void OnDestroy()
    {
        httpClient.Dispose();
    }
}
