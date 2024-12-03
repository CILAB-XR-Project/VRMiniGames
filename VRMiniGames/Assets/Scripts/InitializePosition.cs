using Oculus.Platform;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitializePosition : MonoBehaviour
{
    public Transform character_transform;
    public Transform head_transform;
    public Transform vr_camera_transform;

    public LayerMask ground_layer;


    private void AdjustCharacterPos()
    {
        RaycastHit ground_hit;
        
        if (Physics.Raycast(character_transform.position, Vector3.down, out ground_hit, Mathf.Infinity, ground_layer))
        {
            Vector3 character_pos = character_transform.position;
            character_pos.y = ground_hit.point.y;
            character_transform.position = character_pos;

            Debug.Log($"Character adjusted to ground: {ground_hit.point}");
        }
        else
        {
            Debug.LogWarning("No ground detected below character's foot.");
        }
    }

    private void AdjustVRCamPos()
    {
        Vector3 vr_camera_pos = vr_camera_transform.position;
        vr_camera_pos.y = head_transform.position.y;
        vr_camera_transform.position = vr_camera_pos;

        Debug.Log($"VR Camera adjusted to head position: {head_transform.position}");
    }

    private void Start()
    {
        AdjustCharacterPos();
        AdjustVRCamPos();
    }
}
