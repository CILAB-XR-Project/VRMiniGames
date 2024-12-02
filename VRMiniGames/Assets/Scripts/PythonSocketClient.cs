using Meta.WitAi.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PythonSocketClient : MonoBehaviour
{
    //Singleton pattern
    private static PythonSocketClient _instance;
    private bool _initialized = false;
    public static PythonSocketClient Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<PythonSocketClient>();

                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject("PythonSocketClient");
                    _instance = singletonObject.AddComponent<PythonSocketClient>();
                }
            }
            return _instance;
        }
    }

    void Awake()
    {

        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject); 
        }


        if (!_initialized)
        {
            Initialize();
            _initialized = true;
        }
    }

    //Socket Communication Event Handler
    public static event Action<ModelOutputData> OnDataReceived;

    //Socket Commnuicaters
    TcpClient client;
    NetworkStream stream;
    private  byte[] model_output_buffer = new byte[4096];

    //public PoseMapping character_pose;

    // Start cleint for socket communication
    //private void Initialize()
    //{

    //    client = new TcpClient("127.0.0.1", 12345);
    //    stream = client.GetStream();


    //    //InvokeRepeating("SendData", 0.0666f, 0.0666f);
    //    InvokeRepeating("ReceiveData", 0.0666f, 0.0666f);
    //}

    private void Initialize()
    {
        try
        {
            client = new TcpClient("127.0.0.1", 12345);
            stream = client.GetStream();
            Debug.Log("Python Server Connected");

            Task.Run(() => ReceiveDataAsync());
        }
        catch (Exception e)
        {
            Debug.LogError($"Initialize Socket Connection Err: {e.Message}");
        }
    }

    //private async Task ReceiveDataAsync()
    //{
    //    while(client.Connected)
    //    {
    //        try
    //        {
    //            if (stream.DataAvailable)
    //            {
    //                int byte_model_output_data = await stream.ReadAsync(model_output_buffer, 0, model_output_buffer.Length);
    //                if (byte_model_output_data > 0)
    //                {
    //                    string json_model_output_data = Encoding.UTF8.GetString(model_output_buffer, 0, byte_model_output_data);
    //                    Debug.Log($"Received: {json_model_output_data}");

    //                    var model_output_data = JsonConvert.DeserializeObject<ModelOutputData>(json_model_output_data);

    //                    OnDataReceived?.Invoke(model_output_data);
    //                }
    //            }
    //        }
    //        catch (Exception e)
    //        {
    //            Debug.LogError($"Error in receiving data: {e.Message}");
    //            break;
    //        }
    //    }
    //}
    private async Task ReceiveDataAsync()
    {
        while (client.Connected)
        {
            try
            {
                if (stream == null)
                {
                    Debug.LogError("Stream is null.");
                    break;
                }

                if (stream.DataAvailable)
                {
                    int bytesRead = await stream.ReadAsync(model_output_buffer, 0, model_output_buffer.Length);
                    if (bytesRead > 0)
                    {
                        string json_model_output_data = Encoding.UTF8.GetString(model_output_buffer, 0, bytesRead);
                        //Debug.Log($"Received: {json_model_output_data}");

                        try
                        {
                            var model_output_data = JsonConvert.DeserializeObject<ModelOutputData>(json_model_output_data);
                            if (model_output_data == null)
                            {
                                Debug.LogError("Deserialized data is null.");
                                continue;
                            }

                            OnDataReceived?.Invoke(model_output_data);
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError($"JSON deserialization error: {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error in receiving data: {e.Message}");
                break;
            }
        }
    }

    //Vec3[] to float[][]
    private float[] ConvertVec3ToArray(Vector3 pos_vec3)
    {
        return new float[] { pos_vec3.x, pos_vec3.y, pos_vec3.z };
    }

    //Send VR kps data to Python(Not Using now)
    //private void SendData()
    //{

    //    float[][] vr_pos_data = character_pose.GetVRPosition();
        
    //    var dummy_pos_data = new
    //    {
    //        hmd_pos = ConvertVec3ToArray(new Vector3(0.0f, 1.7f, 0.0f)),
    //        left_hand_pos = ConvertVec3ToArray(new Vector3(-0.5f, 1.5f, 0.2f)),
    //        right_hand_pos = ConvertVec3ToArray(new Vector3(0.5f, 1.5f, 0.2f)),
    //    };

    //    Debug.Log($"VR kps :{vr_pos_data.ToString()}");

    //    string json_vr_pos_data = JsonConvert.SerializeObject(vr_pos_data);
    //    json_vr_pos_data += "\n";
    //    byte[] byte_vr_pos_data = Encoding.UTF8.GetBytes(json_vr_pos_data);
    //    stream.Write(byte_vr_pos_data, 0, byte_vr_pos_data.Length);

    //}


    // Recieve Data from python (Not Using now)
    //private void ReceiveData()
    //{
    //    if (stream.DataAvailable)
    //    {

    //        byte[] model_output_buffer = new byte[1024];
    //        int byte_model_output_data = stream.Read(model_output_buffer, 0, model_output_buffer.Length);
    //        string json_model_output_data = Encoding.UTF8.GetString(model_output_buffer, 0, byte_model_output_data);

    //        var model_output_data = JsonConvert.DeserializeObject<ModelOutputData>(json_model_output_data);

    //        Debug.Log($"Keypoint:{model_output_data.GetKeypoints()[0]}");
    //        Debug.Log($"Action: {model_output_data.GetAction()}");
    //        Debug.Log("");

    //        OnDataReceived?.Invoke(model_output_data);
    //    }
    //}



    // End communication
    private void OnApplicationQuit()
    {
        if (stream != null)
            stream.Close();
        if (client != null)
            client.Close();
    }
}


[Serializable]
public class ModelOutputData
{
    public float[][] keypoints; 
    public int action_class; 

    public Vector3[] GetKeypoints()
    {
        Vector3[] vectors = new Vector3[keypoints.Length];
        for (int i = 0; i < keypoints.Length; i++)
            vectors[i] = new Vector3(keypoints[i][0], keypoints[i][2], keypoints[i][1]);

        return vectors;
    }


    public string GetAction()
    /**
     * "Squat", 0
     * "Lunge", 1
     * "Jump", 2
     * "Stepper", 3
     * "Walking", 4
     * "InPlaceWalking", 5
     * "SideWalking", 6
     * "BackwardWalking", 7
    **/
    {
        if (action_class == 0)
            return "squat";
        else if (action_class == 1)
            return "lunge";
        else if (action_class == 4 || action_class == 5)
            return "walk";
        else
            return "others";
    }
}