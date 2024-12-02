using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class PoseMap
{
    public Transform IK_target;
    public Vector3 tracking_pos_offset;
    public Vector3 tracking_rotation_offset;
    public void Map(Vector3 pose_target, Transform character_transform)
    {   
        Vector3 character_pos = character_transform.position;
        character_pos.y = 0.0f;

        Vector3 world_pose_target = character_pos + character_transform.rotation * pose_target;


        IK_target.position = world_pose_target + tracking_pos_offset;
        IK_target.rotation = character_transform.rotation * Quaternion.Euler(tracking_rotation_offset);
    }

}

public class FootMapping : MonoBehaviour
{
    // pyhton kps var
    private Vector3[] python_model_keypoints;

    //Tactile, Keypoint mapping class
    public PoseMap left_foot;
    public PoseMap right_foot;


    private void Start()
    {
        python_model_keypoints = new Vector3[2];

    }

    private void LateUpdate()
    {
        ApplyPythonKeypoints();
    }


    //Socket communication evnent listener functions
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

    // mapping keypoints to IK target
    private void ApplyPythonKeypoints()
    {
        left_foot.Map(python_model_keypoints[0], this.transform);
        right_foot.Map(python_model_keypoints[1], this.transform);
    }
}
