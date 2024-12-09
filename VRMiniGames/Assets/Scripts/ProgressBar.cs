using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class ProgressBar : MonoBehaviour
{
    private RectTransform _progressBar;
    public RectTransform playerHandle;
    public TextMeshProUGUI playerTime;
    private List<RectTransform> _rulebotHandles;

    private Transform _player;
    private List<Transform> _rulebots; 
    private Transform _goal;
    private bool isgoal = false;

    private float _totalDistance;
    private float _playerProgress;
    private float[] _rulebotsProgress;

    void Start()
    {
        _progressBar = GetComponent<RectTransform>();
        
        _player = GameObject.Find("XR_OVRCameraRig").transform;
        _rulebots = new List<Transform>();
        foreach (var rulebot in GameObject.FindGameObjectsWithTag("Player"))
        {
            _rulebots.Add(rulebot.transform);
        }
        _rulebotsProgress = new float[_rulebots.Count];
        
        playerHandle.anchoredPosition = new Vector2(0, 20);
        _rulebotHandles = new List<RectTransform>();
        
        foreach (var rulebot in _rulebots)
        {
            RectTransform handle = Instantiate(playerHandle, _progressBar);
            _rulebotHandles.Add(handle);
        }
        _goal = GameObject.FindGameObjectWithTag("Goal").transform;
        isgoal = false;
        
        var playerImage = playerHandle.GetComponent<Image>();
        playerImage.color = Color.red;
        playerHandle.SetAsLastSibling();
        
        _totalDistance = Mathf.Abs(_goal.position.z - _player.position.z);
    }

    void Update()
    {   
        _playerProgress = Mathf.Clamp01(1 - (Mathf.Abs(_player.position.z - _goal.position.z) / _totalDistance));
        
        for (int i = 0; i < _rulebots.Count; i++)
        {
            _rulebotsProgress[i] = Mathf.Clamp01(1 - (Mathf.Abs(_rulebots[i].position.z - _goal.position.z) / _totalDistance));
        }  
        
        UpdateHandlePosition(playerHandle, _playerProgress);
        for (int i = 0; i < _rulebotHandles.Count; i++)
        {
            UpdateHandlePosition(_rulebotHandles[i], _rulebotsProgress[i]);
            
        }

        if (_player.position.z > _goal.position.z)
        {
            SetPlayerTime();
        }

        else
        {
            if (!isgoal)
            {
                saveScore();
                isgoal = true;
                StartCoroutine(GoToLobbyAfterDelay(3f));
            }
        }
        
        foreach (Transform _rulebot in _rulebots.ToArray())
        {
            if (_rulebot.position.z <= _goal.position.z)
            {
                _rulebot.gameObject.SetActive(false);
                
                int index = _rulebots.IndexOf(_rulebot);

                if (index >= 0)
                {
                    _rulebots.RemoveAt(index);
                    Destroy(_rulebotHandles[index].gameObject);
                    _rulebotHandles.RemoveAt(index);
                    
                    List<float> tempProgress = new List<float>(_rulebotsProgress);
                    tempProgress.RemoveAt(index);
                    _rulebotsProgress = tempProgress.ToArray();
                }
            }
        }
    }
    
    private IEnumerator GoToLobbyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("Lobby");
    }
    
    private void UpdateHandlePosition(RectTransform handle, float progress)
    {
        float barHeight = _progressBar.rect.height;
        float yPosition = Mathf.Lerp(0, barHeight, progress);
        
        handle.anchoredPosition = new Vector2(handle.anchoredPosition.x, yPosition);
    }

    private void SetPlayerTime()
    {
        float time = Time.timeSinceLevelLoad;
        
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        int milliseconds = Mathf.FloorToInt((time * 100) % 100);
        
        playerTime.text = $"Time : {minutes:00}:{seconds:00}:{milliseconds:00}";
    }
    
    private void saveScore()
    {
        string[] timeParts = playerTime.text.Replace("Time : ", "").Split(':');
        
        int minutes = int.Parse(timeParts[0]);
        int seconds = int.Parse(timeParts[1]);
        int milliseconds = int.Parse(timeParts[2]);
        
        float totalTimeInSeconds = minutes * 60 + seconds + milliseconds / 100f;
        
        GameManager.Instance.miniGameScore = totalTimeInSeconds;
        GameManager.Instance.SaveScore("GRLightBestScore", false);
        Debug.Log("savetime : " + totalTimeInSeconds);
    }
}