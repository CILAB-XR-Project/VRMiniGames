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

    public class VRMapping : MonoBehaviour
{
    [Range(0, 1)]
    public float turn_smoothness = 0.1f;
    public VRMap head;
    public VRMap left_hand;
    public VRMap right_hand;

    public Vector3 head_body_pos_offset;
    public float head_body_yaw_offset;



    void LateUpdate()
    {
        UpdateVRKeypoints();
    }


    void UpdateVRKeypoints()
    {
        //Update body Transform
        Vector3 character_pos = head.ik_target.position + head_body_pos_offset;
        //character_pos.y = 0.0f;
        transform.position = character_pos;
        float yaw = head.vr_target.eulerAngles.y;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(transform.eulerAngles.x, yaw, transform.eulerAngles.z), turn_smoothness);


        head.Map();
        left_hand.Map();
        right_hand.Map();
    }
}
