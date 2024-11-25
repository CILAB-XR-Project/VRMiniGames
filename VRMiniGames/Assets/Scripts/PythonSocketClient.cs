using Meta.WitAi.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class PythonSocketClient : MonoBehaviour
{

    // 싱글턴 인스턴스
    private static PythonSocketClient _instance;
    public static PythonSocketClient Instance
    {
        get
        {
            if (_instance == null)
            {
                // 현재 씬에서 PythonSocketClient를 찾습니다.
                _instance = FindObjectOfType<PythonSocketClient>();

                // 없다면 새로운 GameObject를 생성하여 추가합니다.
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject("PythonSocketClient");
                    _instance = singletonObject.AddComponent<PythonSocketClient>();
                }
            }
            return _instance;
        }
    }
    // 이미 초기화되었는지 확인하기 위한 변수
    private bool _initialized = false;

    //이벤트 핸들러
    public static event Action<ModelOutputData> OnDataReceived;

    //tcp socket 관련 변수
    TcpClient client;
    NetworkStream stream;


    // 좌표를 받아오기 위한 오브젝트 들
    public GameObject hmd;
    public GameObject left_controller;
    public GameObject right_controller;

    void Awake()
    {
        // 싱글턴 설정
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject); // 이미 인스턴스가 존재하면 이 객체를 파괴합니다.
            return;
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject); // 씬 전환 시 객체 유지
        }

        // 초기화가 한 번만 이루어지도록 설정
        if (!_initialized)
        {
            Initialize();
            _initialized = true;
        }
    }

    private void Initialize()
    {
        // TCP 클라이언트 초기화
        client = new TcpClient("127.0.0.1", 12345);
        stream = client.GetStream();

        // 데이터 송수신 시작
        InvokeRepeating("SendData", 0.0666f, 0.0666f);
        InvokeRepeating("ReceiveData", 0.0666f, 0.0666f);
    }

    private float[] ConvertVec3ToArray(Vector3 pos_vec3)
    {
        return new float[] { pos_vec3.x, pos_vec3.y, pos_vec3.z };
    }

    //데이터 송신 함수
    private void SendData()
    {
        //VR 좌표 데이터
        var vr_pos_data = new
        {
            hmd_pos = ConvertVec3ToArray(hmd.transform.position),
            left_hand_pos = ConvertVec3ToArray(left_controller.transform.position),
            right_hand_pos = ConvertVec3ToArray(right_controller.transform.position)
        };

        var tmp_pos_data = new
        {
            hmd_pos = ConvertVec3ToArray(new Vector3(0.0f, 1.7f, 0.0f)),
            left_hand_pos = ConvertVec3ToArray(new Vector3(-0.5f, 1.5f, 0.2f)),
            right_hand_pos = ConvertVec3ToArray(new Vector3(0.5f, 1.5f, 0.2f)),
        };

        //VR 좌표 데이터 송신
        string json_vr_pos_data = JsonConvert.SerializeObject(vr_pos_data);
        byte[] byte_vr_pos_data = Encoding.UTF8.GetBytes(json_vr_pos_data);
        stream.Write(byte_vr_pos_data, 0, byte_vr_pos_data.Length);

    }

    
    //데이터 수신 함수
    private void ReceiveData()
    {
        if (stream.DataAvailable)
        {
            //python model 결과 수신
            byte[] model_output_buffer = new byte[4096];
            int byte_model_output_data = stream.Read(model_output_buffer, 0, model_output_buffer.Length);
            string json_model_output_data = Encoding.UTF8.GetString(model_output_buffer, 0, byte_model_output_data);

            var model_output_data = JsonConvert.DeserializeObject<ModelOutputData>(json_model_output_data);
            OnDataReceived?.Invoke(model_output_data);
        }
    }



    // Update is called once per frame
    private void OnApplicationQuit()
    {
        // 애플리케이션 종료 시 소켓 닫기
        if (stream != null)
            stream.Close();
        if (client != null)
            client.Close();
    }
}


[Serializable]
public class ModelOutputData
{
    public float[][] keypoints; //19개의 kps 3d좌표
    public int action_class; //현재 action class

    public Vector3[] GetKeypoints()
    {
        Vector3[] vectors = new Vector3[keypoints.Length];
        for (int i = 0; i < keypoints.Length; i++)
            vectors[i] = new Vector3(keypoints[i][0], keypoints[i][2], keypoints[i][1]);

        return vectors;
    }
}