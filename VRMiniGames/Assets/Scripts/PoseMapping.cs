using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
     KEYPOINTS = [
    'nose', #0
    'left_shoulder', #1
    'right_shoulder', #2
    'left_elbow', #3
    'right_elbow',#4
    'left_wrist',#5
    'right_wrist',#6
    'left_hip_extra',#7
    'right_hip_extra',#8
    'left_knee',#9
    'right_knee',#10
    'left_ankle',#11
    'right_ankle',#12
    'left_bigtoe',#13
    'right_bigtoe',#14
]*/
    private Vector3[] python_model_keypoints;


    private PoseMap[] pose_maps;


    [Range(0, 1)]
    public float turn_smoothness = 0.1f;

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
        ApplyPythonKeypoints();
        UpdateVRKeypoints();
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
        transform.position = head.ik_target.position + head_body_pos_offset;
        float yaw = head.vr_target.eulerAngles.y;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(transform.eulerAngles.x, yaw, transform.eulerAngles.z), turn_smoothness);

        head.Map();
        left_hand.Map();
        right_hand.Map();
    }

}
