using Meta.WitAi.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class PythonSocketClient : MonoBehaviour
{

    // �̱��� �ν��Ͻ�
    private static PythonSocketClient _instance;
    public static PythonSocketClient Instance
    {
        get
        {
            if (_instance == null)
            {
                // ���� ������ PythonSocketClient�� ã���ϴ�.
                _instance = FindObjectOfType<PythonSocketClient>();

                // ���ٸ� ���ο� GameObject�� �����Ͽ� �߰��մϴ�.
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject("PythonSocketClient");
                    _instance = singletonObject.AddComponent<PythonSocketClient>();
                }
            }
            return _instance;
        }
    }
    // �̹� �ʱ�ȭ�Ǿ����� Ȯ���ϱ� ���� ����
    private bool _initialized = false;

    //�̺�Ʈ �ڵ鷯
    public static event Action<ModelOutputData> OnDataReceived;

    //tcp socket ���� ����
    TcpClient client;
    NetworkStream stream;


    // ��ǥ�� �޾ƿ��� ���� ������
    public PoseMapping character_pose;

    void Awake()
    {
        // �̱��� ����
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject); // �̹� �ν��Ͻ��� �����ϸ� �� ��ü�� �ı��մϴ�.
            return;
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject); // �� ��ȯ �� ��ü ����
        }

        // �ʱ�ȭ�� �� ���� �̷�������� ����
        if (!_initialized)
        {
            Initialize();
            _initialized = true;
        }
    }

    private void Initialize()
    {
        // TCP Ŭ���̾�Ʈ �ʱ�ȭ
        client = new TcpClient("127.0.0.1", 12345);
        stream = client.GetStream();

        // ������ �ۼ��� ����
        InvokeRepeating("SendData", 0.0666f, 0.0666f);
        InvokeRepeating("ReceiveData", 0.0666f, 0.0666f);
    }

    //Vec3[] to float[][]
    private float[] ConvertVec3ToArray(Vector3 pos_vec3)
    {
        return new float[] { pos_vec3.x, pos_vec3.y, pos_vec3.z };
    }

    //������ �۽� �Լ�
    private void SendData()
    {
        //VR ��ǥ ������ �޾ƿ���
        float[][] vr_pos_data = character_pose.GetVRPosition();
        
        var dummy_pos_data = new
        {
            hmd_pos = ConvertVec3ToArray(new Vector3(0.0f, 1.7f, 0.0f)),
            left_hand_pos = ConvertVec3ToArray(new Vector3(-0.5f, 1.5f, 0.2f)),
            right_hand_pos = ConvertVec3ToArray(new Vector3(0.5f, 1.5f, 0.2f)),
        };

        Debug.Log($"VR �Ӹ� ��ġ:{vr_pos_data.ToString()}");
        //VR ��ǥ ������ �۽�
        string json_vr_pos_data = JsonConvert.SerializeObject(vr_pos_data);
        json_vr_pos_data += "\n";
        byte[] byte_vr_pos_data = Encoding.UTF8.GetBytes(json_vr_pos_data);
        stream.Write(byte_vr_pos_data, 0, byte_vr_pos_data.Length);

    }

    
    //������ ���� �Լ�
    private void ReceiveData()
    {
        if (stream.DataAvailable)
        {
            //python model ��� ����
            byte[] model_output_buffer = new byte[4096];
            int byte_model_output_data = stream.Read(model_output_buffer, 0, model_output_buffer.Length);
            string json_model_output_data = Encoding.UTF8.GetString(model_output_buffer, 0, byte_model_output_data);

            var model_output_data = JsonConvert.DeserializeObject<ModelOutputData>(json_model_output_data);

            Debug.Log($"python �Ӹ� ��ġ{model_output_data.GetKeypoints()[0]}");

            //�̺�Ʈ �߻�
            OnDataReceived?.Invoke(model_output_data);
        }
    }



    // Update is called once per frame
    private void OnApplicationQuit()
    {
        // ���ø����̼� ���� �� ���� �ݱ�
        if (stream != null)
            stream.Close();
        if (client != null)
            client.Close();
    }
}


[Serializable]
public class ModelOutputData
{
    public float[][] keypoints; //19���� kps 3d��ǥ
    public int action_class; //���� action class

    // Ű����Ʈ vec3�� ��ȯ �Ͽ� ����
    public Vector3[] GetKeypoints()
    {
        Vector3[] vectors = new Vector3[keypoints.Length];
        for (int i = 0; i < keypoints.Length; i++)
            vectors[i] = new Vector3(keypoints[i][0], keypoints[i][2], keypoints[i][1]);

        return vectors;
    }

//    ACTIVITY_LIST = [
//    "Squat", 0
//    "Lunge", 1
//    "Jump", 2
//    "Stepper", 3
//    "Walking", 4
//    "InPlaceWalking", 5
//    "SideWalking", 6
//    "BackwardWalking", 7
//]

    //action idx ��ȯ(�ǹ̴� �� ����Ʈ ����)
    public int GetAction() {  return action_class; }
}