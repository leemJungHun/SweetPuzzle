using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameManager : MonoBehaviour
{
    [SerializeField] GameObject _questObj;
    [SerializeField] RectTransform _centerPoint;
    [SerializeField] Text _moveCountText;
    [SerializeField] Text _targetCountText;
    [SerializeField] Text _scoreText;
    [SerializeField] Text _clearText;
    Vector3 _originPoint;
    static InGameManager _uniqueInstance;

    // 정보 변수
    float _questMoveSpeed = 1f;
    float _questViewTime = 1.5f;
    int _moveCount = 20;
    int _targetCount = 3;
    int _totalScore = 0;
    float _drawScore = 0;
    float _countingTime = 1f;

    DefineHelper.eIngameState _currentState;
    public static InGameManager _instance
    {
        get { return _uniqueInstance; }
    }

    public int totalScore { 
        get { return _totalScore; }
    }
    public DefineHelper.eIngameState _ingameState
    {
        get { return _currentState; }
    }

    public int MoveCount
    {
        get { return _moveCount; }
    }

    public int TargetCount
    {
        get { return _targetCount; }
    }

    void Awake()
    {
        _uniqueInstance = this;
    }

    private void Start()
    {
        _originPoint = _questObj.transform.position;
        _clearText.gameObject.SetActive(false);
    }

    void LateUpdate()
    {
        if (_totalScore <= _drawScore)
        {
            _scoreText.text = string.Format("{0:#,0}", _totalScore);
        }
        else
        {
            _drawScore += _totalScore * (Time.deltaTime / _countingTime);
            _scoreText.text = string.Format("{0:#,0}", _drawScore);

        }
    }

    public void InitializeSettings()
    {
        _moveCountText.text = _moveCount.ToString();
        _targetCountText.text = _targetCount.ToString();
        StartGame();
    }

    
    void StartGame()
    {
        _currentState = DefineHelper.eIngameState.COUNT;
        iTween.MoveTo(_questObj, iTween.Hash("y", _centerPoint.transform.position.y, "time", _questMoveSpeed, "easetype", iTween.EaseType.easeInOutSine, "onComplete", "EndQuestView"
            , "oncompletetarget", this.gameObject));
    }

    public void EndQuestView()
    {
        StartCoroutine(HideView());
    }

    IEnumerator HideView()
    {
        yield return new WaitForSeconds(_questViewTime);
        iTween.MoveTo(_questObj, iTween.Hash("y", _originPoint.y, "time", _questMoveSpeed, "easetype", iTween.EaseType.easeInOutSine, "onComplete", "PlayGame"
            , "oncompletetarget", this.gameObject));
    }
    void PlayGame()
    {
        _currentState = DefineHelper.eIngameState.PLAY;
    }

    public void EndGame()
    {
        bool gameClear = false;
        if (_moveCount == 0 && _targetCount != 0)
        {
            gameClear = false;
        }
        else
        {
            gameClear = true;
        }

        _currentState = DefineHelper.eIngameState.END;
        StartCoroutine(ClearTextOpen(gameClear));
        if (gameClear)
        {
            StartCoroutine(RemainMove());
        }
        else
        {
            ResultGame(gameClear);
        }
    }

    void ResultGame(bool gameClear)
    {
        _currentState = DefineHelper.eIngameState.RESULT;
        // 종료창을 생성
        GameObject go = Instantiate(ResourcePoolManager.instance.GetWindowPrefabFromType(DefineHelper.eUIwindowtype.ResultWnd));
        ResultWnd resultWnd = go.GetComponent<ResultWnd>();
        resultWnd.OpenResultWindow(_totalScore, gameClear);
    }

    IEnumerator ClearTextOpen(bool clear)
    {
        _clearText.gameObject.SetActive(true);
        if (clear)
        {
            _clearText.color = Color.green;
            _clearText.text = "Clear!!";
        }
        else
        {
            _clearText.color = Color.red;
            _clearText.text = "Failed";
        }
        yield return new WaitForSeconds(1f);
    }

    IEnumerator RemainMove()
    {
        yield return new WaitForSeconds(1f);
        while (_moveCount>0)
        {
            _moveCount--;
            _moveCountText.text = _moveCount.ToString();
            AddScore(3000);
            yield return new WaitForSeconds(0.1f);
        }

        

        ResultGame(true);
    }

    public void CandyMove(bool isCookie)
    {
        if(_moveCount != 0)
            _moveCount--;
        
        _moveCountText.text = _moveCount.ToString();

    }

    public void TargetSuccess()
    {
        if(_targetCount != 0)
            _targetCount--;

        _targetCountText.text = _targetCount.ToString();
    }

    public void AddScore(int addScore)
    {
        _totalScore += addScore;
    }

}
