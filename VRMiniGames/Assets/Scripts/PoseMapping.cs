using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

[System.Serializable]
public class VRMap
{
    public Transform vr_target;
    public Transform ik_target;
    public Vector3 tracking_pos_offset;
    public Vector3 tracking_rotation_offset;
    public void Map()
    {
        ik_target.position = vr_target.TransformPoint(tracking_pos_offset);
        ik_target.rotation = vr_target.rotation * Quaternion.Euler(tracking_rotation_offset);
    }
}

[System.Serializable]
public class PoseMap
{
    //public Vector3 pose_target;
    public Transform IK_target;
    //public Transform IK_Child;
    public Vector3 tracking_pos_offset;
    //public Vector3 tracking_rotation_offset;
    public void Map(Vector3 pose_target)
    {
        IK_target.localPosition = pose_target + tracking_pos_offset;
    }

}



public class PoseMapping : MonoBehaviour
{
    /*

]*/
    private Vector3[] python_model_keypoints;


    private PoseMap[] pose_maps;


    [Range(0, 1)]
    public float turn_smoothness = 0.1f; 
    public float move_smoothness = 0.1f;

    public Vector3 head_body_pos_offset;
    public float head_body_yaw_offset;


    public VRMap head;
    public VRMap left_hand;
    public VRMap right_hand;

    public PoseMap left_shoulder;
    public PoseMap right_shoulder;
    public PoseMap left_hip;
    public PoseMap right_hip;
    public PoseMap left_knee;
    public PoseMap right_knee;
    public PoseMap left_ankle;
    public PoseMap right_ankle;
    public PoseMap left_toe;
    public PoseMap right_toe;


    private bool is_calibrate_done = false;
    private Vector3 accumulatedVRPosition; // VR 기기의 누적 위치
    private Vector3 accumulatedCenterPosition;
    private int sampleCount; // 수집된 샘플의 개수

    private Vector3 initialCharacterPosition; // 캐릭터 초기 위치
    private Vector3 initialVRPosition; // VR 초기 위치


    public Animator animator;


    private void Start()
    {

        python_model_keypoints = new Vector3[10];

        pose_maps = new PoseMap[10] {
            //nose,
            left_shoulder, right_shoulder,
            //left_elbow, right_elbow,
            //left_wrist, right_wrist,
            left_hip, right_hip,
            left_knee, right_knee,
            left_ankle, right_ankle,
            left_toe, right_toe
        };

        StartCoroutine(StartCalibration(2.0f));

    }
    IEnumerator StartCalibration(float duration)
    {
        accumulatedVRPosition = Vector3.zero;
        sampleCount = 0;

        Debug.Log("Calibration started...");

        // Calibration 동안 VR 기기의 위치를 누적
        float elapsedTime = 0.0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            // VR 기기의 위치 누적
            accumulatedVRPosition += head.vr_target.position;
            accumulatedCenterPosition += (head.vr_target.position + left_hand.vr_target.position + right_hand.vr_target.position) / 3.0f;
            sampleCount++;

            // 매 프레임 실행
            yield return null;
        }

        // Calibration 종료 처리
        CompleteCalibration();
    }
    void CompleteCalibration()
    {
        // VR 초기 위치를 평균값으로 설정
        initialVRPosition = accumulatedVRPosition / sampleCount;
        initialCharacterPosition = accumulatedCenterPosition / sampleCount;
        initialCharacterPosition.y = 0f;

        transform.position = initialCharacterPosition;
        is_calibrate_done = true;

        Debug.Log("Calibration completed.");
        Debug.Log($"Initial VR Position: {initialVRPosition}");
        Debug.Log($"Initial Character Position: {initialCharacterPosition}");
    }
    private void OnEnable()
    {
        PythonSocketClient.OnDataReceived += UpdatePythonKeypoints;
    }

    private void OnDisable()
    {
        PythonSocketClient.OnDataReceived -= UpdatePythonKeypoints;
    }


    private Vector3[] ConvertArrayToVec3(float[][] array)
    {
        Vector3[] vectors = new Vector3[array.Length];
        for (int i = 0; i < array.Length; i++)
            vectors[i] = new Vector3(array[i][0], array[i][2], array[i][1]);

        return vectors;
    }

    void UpdatePythonKeypoints(ModelOutputData data)
    {
        python_model_keypoints = data.GetKeypoints();

        //Debug.Log($"현재 오른 발 위치:{python_model_keypoints[14]}");


    }

    private void LateUpdate()
    {
        UpdateVRKeypoints();
        if (is_calibrate_done)
        {
            ApplyPythonKeypoints();
            //UpdateVRKeypoints();
            UpdateBodyTransform();
        }
    }

    private void ApplyPythonKeypoints()
    {
        //Vector3 character_pos = new Vector3(
        //    (python_model_keypoints[11].x + python_model_keypoints[12].x) / 2,
        //    (python_model_keypoints[11].y + python_model_keypoints[12].y) / 2,
        //    (python_model_keypoints[11].z + python_model_keypoints[12].z) / 2);
        //transform.position = character_pos;

        //float yaw = head.vr_target.eulerAngles.y;
        //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(transform.eulerAngles.x, yaw, transform.eulerAngles.z), turn_smoothness);

        //nose.Map(python_model_keypoints[0]);
        //left_shoulder.Map(python_model_keypoints[1]);
        //right_shoulder.Map(python_model_keypoints[2]);
        //left_wrist.Map(python_model_keypoints[5]);
        //right_wrist.Map(python_model_keypoints[6]);
        //left_hip.Map(python_model_keypoints[7]);
        //right_hip.Map(python_model_keypoints[8]);
        //left_ankle.Map(python_model_keypoints[11]);
        //right_ankle.Map(python_model_keypoints[12]);

        for (int i = 0; i < python_model_keypoints.Length; i++)
        {   
            pose_maps[i].Map(python_model_keypoints[i]);
        }
    }

    void UpdateVRKeypoints()
    {
        head.Map();
        left_hand.Map();
        right_hand.Map();
    }

    void UpdateBodyTransform()
    {
        Vector3 character_center = (python_model_keypoints[2] + python_model_keypoints[3]) / 2.0f;
        character_center.y = 0f;

        Vector3 vr_move_offset = head.vr_target.position - initialVRPosition;
        vr_move_offset.y = 0f;
        Vector3 final_body_pos = initialCharacterPosition + vr_move_offset + character_center;


        //transform.position = Vector3.Lerp(transform.position, final_body_pos, Time.deltaTime * move_smoothness);
        transform.position = final_body_pos;

        float yaw = head.vr_target.eulerAngles.y;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(transform.eulerAngles.x, yaw, transform.eulerAngles.z), turn_smoothness);
    }

    void CalibratePose()
    {
        Vector3 center = (head.vr_target.position + left_hand.vr_target.position + right_hand.vr_target.position) / 3.0f;
        center.y = 0f;

        transform.position = center;
    }


}
