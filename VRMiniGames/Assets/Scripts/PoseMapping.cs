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


/*
1. T-pose calibration(done)
 - t-pose를 수행하여 최초 캐릭터, VR 중심 좌표 저장 
 - 캐릭터 중심 좌표로 이동(transform)
2. 캐릭터 중심을 기준으로 VR에서 얻은 머리, 손 좌표 normailze (done)
    - transform 기준으로 머리, 손 normalize
    - 이거는 어디서 수행? pose mapping에서 함수 만들고 불러오는 식으로
3. 센서에서 얻은 압력데이터+normalized 머리, 손 좌표를 사용한 전신 키포인트 추론 (함수 형태만 만들어 놓음. 실제 tactile 연동 코드 작성 필요)
    -normalize 좌표를 파이썬 모델에 전달하여 10개의 키포인트 추론
4. 현재 골반 기준으로 전신 키포인트 캐릭터에 적용 (done)
    - 현재 캐릭터를 기준으로 localPosition에 키포인트 적용
5. VR의 위치 이동량+모델에서 추론한 골반 좌표로 캐릭터 좌표 중심 최신화 (done?)
    - VR의 위치 이동량 + 모델에서 추론한 골반 좌표를 사용해 캐릭터 중심 좌표 업데이트
    - VR의 위치 이동은 다른 곳에서 수행하여도 
6. 2~5 반복.

 */


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
    private Vector3 accumulatedVRPosition; // VR 기기의 누적 위치
    private Vector3 accumulatedCenterPosition;
    private Vector3 initialCharacterPosition; // 캐릭터 초기 위치
    private Vector3 initialVRPosition; // VR 초기 위치
    private int sampleCount; // 수집된 샘플의 개수

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

        //캐릭터 중심 및 시작 좌표 계산용 캘리브레이션
        //씬 바뀔때마다...? 특정 위치에서 매번 시작할거면 상관 없을 수도
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

    //calibration 수행
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
            //머리, 양손의 중심을 캐릭터의 중심으로 계산
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

        //캐릭터 최초 중심 위치 저장
        initialCharacterPosition = accumulatedCenterPosition / sampleCount;
        initialCharacterPosition.y = 0f;

        transform.position = initialCharacterPosition;
        is_calibrate_done = true;

        Debug.Log("Calibration completed.");
        Debug.Log($"Initial VR Position: {initialVRPosition}");
        Debug.Log($"Initial Character Position: {initialCharacterPosition}");
    }

    //이벤트 핸들러 등록 및 삭제
    private void OnEnable()
    {
        PythonSocketClient.OnDataReceived += UpdatePythonKeypoints;
    }

    private void OnDisable()
    {
        PythonSocketClient.OnDataReceived -= UpdatePythonKeypoints;
    }


    // Python model에서 생성된 키포인트를 받기위한 이벤트 리스너
    void UpdatePythonKeypoints(ModelOutputData data)
    {
        python_model_keypoints = data.GetKeypoints();
    }

    // Python model 입력을 위한 Normalized 머리, 손 키포인트 계산
    public float[][] GetVRPosition()
    {
        Transform character_center = transform;

        Vector3 head_normalize = character_center.InverseTransformPoint(head.vr_target.position);
        Vector3 left_hand_normalize = character_center.InverseTransformPoint(left_hand.vr_target.position);
        Vector3 right_hand_normalize = character_center.InverseTransformPoint(right_hand.vr_target.position);

        float[] head_normalize_f = new float[3] { head_normalize.x, head_normalize.z, head_normalize.y };
        float[] right_normalize_f = new float[3] { left_hand_normalize.x, left_hand_normalize.z, left_hand_normalize.y };
        float[] left_normailze_f = new float[3] { right_hand_normalize.x, right_hand_normalize.z, right_hand_normalize.y };
        float[][] vr_pos_data = new float[][] {head_normalize_f, left_normailze_f, right_normalize_f};

        return vr_pos_data;
    }

    // Tactile에서 얻은 keypoint를 캐릭터에 적용
    private void ApplyPythonKeypoints()
    {
        for (int i = 0; i < python_model_keypoints.Length; i++)
        {   
            pose_maps[i].Map(python_model_keypoints[i]);
        }
    }

    //VR에서 얻은 keypoint 머리, 손에 적용
    void UpdateVRKeypoints()
    {
        head.Map();
        left_hand.Map();
        right_hand.Map();
    }

    //캐릭터 중심 좌표 업데이트 및 이동
    void UpdateBodyTransform()
    {
        Vector3 character_center = (python_model_keypoints[2] + python_model_keypoints[3]) / 2.0f;
        character_center.y = 0f;

        Vector3 vr_move_offset = head.vr_target.position - initialVRPosition;
        vr_move_offset.y = 0f;
        Vector3 final_body_pos = initialCharacterPosition + vr_move_offset + character_center;


        //transform.position = Vector3.Lerp(transform.position, final_body_pos, Time.deltaTime * move_smoothness);
        transform.position = final_body_pos; //일단 즉시 업데이트

        float yaw = head.vr_target.eulerAngles.y;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(transform.eulerAngles.x, yaw, transform.eulerAngles.z), turn_smoothness);
    }
}
