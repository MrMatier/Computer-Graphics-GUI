using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Globalization;

public class EyeTrackingReceiver : MonoBehaviour
{
    Thread receiveThread;
    UdpClient client;
    public int port = 5052;
    private Vector3 faceDeltas = Vector3.zero;
    private object lockObject = new object();
    private bool running;

    void Start()
    {
        receiveThread = new Thread(ReceiveData);
        receiveThread.IsBackground = true;
        running = true;
        receiveThread.Start();
    }

    void ReceiveData()
    {
        client = new UdpClient(port);
        while (running)
        {
            try
            {
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = client.Receive(ref anyIP);
                string text = Encoding.UTF8.GetString(data);

                string[] coords = text.Split(',');
                float deltaX = float.Parse(coords[0], CultureInfo.InvariantCulture);
                float deltaY = float.Parse(coords[1], CultureInfo.InvariantCulture);
                float deltaZ = float.Parse(coords[2], CultureInfo.InvariantCulture);

                Vector3 newFaceDeltas = new Vector3(deltaX, deltaY, deltaZ);

                lock (lockObject)
                {
                    faceDeltas = newFaceDeltas;
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Error in ReceiveData: " + ex);
            }
        }
    }

    public Vector3 GetFaceDeltas()
    {
        lock (lockObject)
        {
            return faceDeltas;
        }
    }

    public void ResetData()
    {
        lock (lockObject)
        {
            faceDeltas = Vector3.zero;
        }
    }

    void OnApplicationQuit()
    {
        running = false;
        receiveThread?.Abort();
        client?.Close();
    }
}
