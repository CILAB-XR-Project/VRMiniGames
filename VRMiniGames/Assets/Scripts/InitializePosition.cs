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
    private Vector3 ground_pos;

    private void UpdateGroundY()
    {
        RaycastHit ground_hit;

        if (Physics.Raycast(character_transform.position, Vector3.down, out ground_hit, Mathf.Infinity, ground_layer))
        {
            ground_pos = ground_hit.point;

            Debug.Log($"Character adjusted to ground: {ground_hit.point}");
        }
        else
        {
            Debug.LogWarning("No ground detected below character's foot.");
        }

    }
    private void AdjustCharacterPos()
    {
        Vector3 character_pos = character_transform.position;
        character_pos.y = ground_pos.y;
        character_transform.position = character_pos;
    }

    private void AdjustVRCamPos()
    {
        Vector3 vr_camera_pos = vr_camera_transform.position;
        vr_camera_pos.y = head_transform.position.y;
        vr_camera_transform.position = vr_camera_pos;

        Debug.Log($"VR Camera adjusted to head position: {head_transform.position}");
    }

    public float GetGroundY() { return ground_pos.y; }

    private void Start()
    {
        UpdateGroundY();
        AdjustCharacterPos();
        AdjustVRCamPos();
    }
}
