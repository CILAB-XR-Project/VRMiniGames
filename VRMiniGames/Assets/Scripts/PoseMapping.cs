using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;


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

    //VR keypoints mapping
    public VRMap head;
    public VRMap left_hand;
    public VRMap right_hand;


    //Tactile keypoints mapping
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

    //Calibration vars
    private bool is_calibrate_done = false;
    private Vector3 accumulatedVRPosition; 
    private Vector3 accumulatedCenterPosition;
    private Vector3 initialCharacterPosition; 
    private Vector3 initialVRPosition; 
    private int sampleCount;

    private void Start()
    {

        python_model_keypoints = new Vector3[10];

        pose_maps = new PoseMap[10] {
            left_shoulder, right_shoulder,
            left_hip, right_hip,
            left_knee, right_knee,
            left_ankle, right_ankle,
            left_toe, right_toe
        };


        StartCoroutine(StartCalibration(2.0f));

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


    IEnumerator StartCalibration(float duration)
    {
        accumulatedVRPosition = Vector3.zero;
        sampleCount = 0;

        Debug.Log("Calibration started...");


        float elapsedTime = 0.0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;


            accumulatedVRPosition += head.vr_target.position;

            accumulatedCenterPosition += (head.vr_target.position + left_hand.vr_target.position + right_hand.vr_target.position) / 3.0f;
            sampleCount++;


            yield return null;
        }


        CompleteCalibration();
    }

    void CompleteCalibration()
    {

        initialVRPosition = accumulatedVRPosition / sampleCount;


        //initialCharacterPosition = accumulatedCenterPosition / sampleCount;
        initialCharacterPosition = accumulatedVRPosition / sampleCount;
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



    private void UpdatePythonKeypoints(ModelOutputData data)
    {
        python_model_keypoints = data.GetKeypoints();
    }


    public float[][] GetVRPosition()
    {
        Transform character_center = this.transform;

        Vector3 head_normalize = character_center.InverseTransformPoint(head.vr_target.position);
        Vector3 left_hand_normalize = character_center.InverseTransformPoint(left_hand.vr_target.position);
        Vector3 right_hand_normalize = character_center.InverseTransformPoint(right_hand.vr_target.position);

        float[] head_normalize_f = new float[3] { head_normalize.x, head_normalize.z, head_normalize.y };
        float[] right_normalize_f = new float[3] { left_hand_normalize.x, left_hand_normalize.z, left_hand_normalize.y };
        float[] left_normailze_f = new float[3] { right_hand_normalize.x, right_hand_normalize.z, right_hand_normalize.y };
        float[][] vr_pos_data = new float[][] {head_normalize_f, left_normailze_f, right_normalize_f};

        return vr_pos_data;
    }


    private void ApplyPythonKeypoints()
    {
        for (int i = 0; i < python_model_keypoints.Length; i++)
        {   
            pose_maps[i].Map(python_model_keypoints[i], this.transform, 0.0f);
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


        Vector3 final_body_pos = initialCharacterPosition + vr_move_offset + character_center + head_body_pos_offset; ;


        //transform.position = Vector3.Lerp(transform.position, final_body_pos, Time.deltaTime * move_smoothness);
        transform.position = final_body_pos; 

        float yaw = head.vr_target.eulerAngles.y;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(transform.eulerAngles.x, yaw, transform.eulerAngles.z), turn_smoothness);
    }
}
