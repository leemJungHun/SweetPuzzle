using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoardObj : MonoBehaviour
{
    static GameBoardObj _unique;

    [SerializeField] GameObject _blockPos;
    [SerializeField] GameObject _holePos;
    [SerializeField] RectTransform _dummyPoint;
    Queue<CandyObj> _dropBlocks = new Queue<CandyObj>();
    GameObject _swapObject;
    GameObject _swapObject2;
    Vector3 _swapPosition;
    Vector3 _swapPosition2;

    float _swapTime = 0.15f;
    float _dropTime = 0f;
    float _addTime = 0f;


    // 첫번째 키는 row, 두번째 키는 col로 지정한다
    Dictionary<int, Dictionary<int, GameObject>> _gameBoard;

    int _holeCount = 7;
    int _candyCount = 7;
    int _squareCount = 1;

    bool _swap = false;

    public static GameBoardObj instance
    {
        get { return _unique; }
    }

    public float _stopTime
    {
        get { return _addTime; }
    }

    public bool _downStop
    {
        get; set;
    }

    
    private void Awake()
    {
        _unique = this;
        _gameBoard = new Dictionary<int, Dictionary<int, GameObject>>();
        _downStop = false;
    }

    void Start()
    {
        InitSetting();
    }

    void Update()
    {
        if(InGameManager._instance._ingameState == DefineHelper.eIngameState.PLAY)
        {
            _dropTime += Time.deltaTime;
            _addTime += Time.deltaTime;
            if (_dropTime >= 0.15f && !_downStop)
            {
                if (_gameBoard.ContainsKey(1))
                {
                    for (int i = 1; i <= _gameBoard[1].Count; i++)
                    {
                        if (_gameBoard[1][i] == null)
                        {
                            AddBlock(i);
                        }
                    }
                    _dropTime = 0f;
                }
            }

            if (_addTime >= 0.2f)
            {
                DropAddMatchingCheck();
            }

            if (_addTime >= 0.5f)
            {
                _squareCount = 1;
                if (InGameManager._instance.MoveCount <= 0 || InGameManager._instance.TargetCount <= 0)
                {
                    InGameManager._instance.EndGame();
                }
            }

        }
    }

    void InitSetting()
    {
        // 먼치킨 볼이 들어갈 구멍 셋팅
        HoleSetting();

        // 랜덤으로 초기 블럭 셋팅
        BlockSetting();
    }

    // 초기 구멍 셋팅
    void HoleSetting()
    {
        for (int i = 1; i <= _holeCount; i++)
        {
            //윗쪽 구멍 셋팅
            RectTransform rectTransform = Instantiate(ResourcePoolManager.instance.GetUIPrefabFromType(DefineHelper.eUIblocktype.Hole), _holePos.transform).GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x + i * 100, rectTransform.anchoredPosition.y);

            //왼쪽 구멍 셋팅
            rectTransform = Instantiate(ResourcePoolManager.instance.GetUIPrefabFromType(DefineHelper.eUIblocktype.Hole), _holePos.transform).GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y + i * -100);

            //아랫쪽 구멍 셋팅
            rectTransform = Instantiate(ResourcePoolManager.instance.GetUIPrefabFromType(DefineHelper.eUIblocktype.Hole), _holePos.transform).GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x + i * 100, rectTransform.anchoredPosition.y - 800);

            //오른쪽 구멍 셋팅
            rectTransform = Instantiate(ResourcePoolManager.instance.GetUIPrefabFromType(DefineHelper.eUIblocktype.Hole), _holePos.transform).GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x + 800, rectTransform.anchoredPosition.y + i * -100);
        }
    }

    // 초기 블럭 셋팅
    void BlockSetting()
    {
        for (int row = 1; row <= _candyCount; row++)
        {
            for (int col = 1; col <= _candyCount; col++)
            {
                DefineHelper.eUIblocktype randomBlock = (DefineHelper.eUIblocktype)Random.Range(0, 4);
                bool check = true;
                while (check)
                {
                    randomBlock = (DefineHelper.eUIblocktype)Random.Range(0, 4);
                    check = InitBlockCheck(row, col, randomBlock);
                }
                
                GameObject go = Instantiate(ResourcePoolManager.instance.GetUIPrefabFromType(randomBlock), _blockPos.transform);
               
                RectTransform rectTransform = go.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x + col * 100, rectTransform.anchoredPosition.y + row * -100);
                
                CandyObj candyobj = go.GetComponent<CandyObj>();
                candyobj.row = row;
                candyobj.col = col;
                candyobj._typeNum = (int)randomBlock;
                Dictionary<int, GameObject> colDic = _gameBoard.ContainsKey(row) ? _gameBoard[row] : new Dictionary<int, GameObject>();
                colDic.Add(col, go);

                if (_gameBoard.ContainsKey(row))
                {
                    _gameBoard[row] = colDic;
                }
                else
                {
                    _gameBoard.Add(row, colDic);
                }
            }
        }
    }


    /// <summary>
    /// 초기 3 매칭 블록이 있는지 체크하는 함수
    /// </summary>
    /// <param name="row">비교하는 열</param>
    /// <param name="col">비교하는 행</param>
    /// <returns></returns>
    bool InitBlockCheck(int row, int col, DefineHelper.eUIblocktype randomBlock)
    {
        bool rowCheck = true;
        bool colCheck = true;
        bool cookieCheck = false;

        // 윗쪽 체크
        if (_gameBoard.ContainsKey(row - 1)&& _gameBoard[row - 1].ContainsKey(col))
        {
            rowCheck = _gameBoard[row - 1][col].tag.Contains(randomBlock.ToString());
            if (rowCheck)
            {
                cookieCheck = true;
                if (_gameBoard.ContainsKey(row - 2) && _gameBoard[row - 2].ContainsKey(col))
                {
                    rowCheck = _gameBoard[row - 2][col].tag.Contains(randomBlock.ToString());
                }
            }
            else
                cookieCheck = false;
        }
        else
        {
            cookieCheck = false;
            rowCheck = false;
        }

        // 왼쪽 체크
        if (_gameBoard.ContainsKey(row) && _gameBoard[row].ContainsKey(col - 1))
        {
            colCheck = _gameBoard[row][col - 1].tag.Contains(randomBlock.ToString());
            if (colCheck)
            {
                cookieCheck = cookieCheck ? true : false;
                if (_gameBoard.ContainsKey(row) && _gameBoard[row].ContainsKey(col-2))
                {
                    colCheck = _gameBoard[row][col-2].tag.Contains(randomBlock.ToString());
                }
            }
        }
        else
        {
            cookieCheck = false;
            colCheck = false;
        }

        //왼쪽, 윗쪽 모두 한칸 이상씩 있을 경우 초반에 특수블럭이 생성되지 않도록 체크
        if (cookieCheck)
        {
            if (_gameBoard.ContainsKey(row-1) && _gameBoard[row-1].ContainsKey(col-1) && _gameBoard[row-1][col-1] != null)
            {
                if (_gameBoard[row - 1][col - 1].tag.Contains(randomBlock.ToString()))
                {
                    return true;
                }
            }

        }

        return rowCheck ? true : colCheck ? true : false;
    }

    public void BlockSwap(int row, int col, DefineHelper.eDirection direction)
    {
        switch (direction)
        {
            case DefineHelper.eDirection.Left://col -1
                if (col != 1)
                {
                    //스왑
                    ChangeObjectInfo(row, col, row, col - 1);
                    _swap = true;
                }
                else
                    Debug.Log("이동 실패");
                break;
            case DefineHelper.eDirection.Right://col +1
                if (col != 7)
                {
                    ChangeObjectInfo(row, col, row, col+1);
                    
                    _swap = true;
                }
                else
                    Debug.Log("이동 실패");
                break;
            case DefineHelper.eDirection.Up://row -1
                if (row != 1)
                {
                    ChangeObjectInfo(row, col, row - 1, col);
                    _swap = true;
                }
                else
                    Debug.Log("이동 실패");
                break;
            case DefineHelper.eDirection.Down://row +1
                if (row != 7)
                {

                    ChangeObjectInfo(row, col, row + 1, col);
                    _swap = true;
                }
                else
                    Debug.Log("이동 실패");
                break;
        }

        if (_swap)
        {
            _swapPosition = _swapObject.transform.position;
            _swapPosition2 = _swapObject2.transform.position;

            iTween.MoveTo(_swapObject, iTween.Hash("x", _swapPosition2.x, "y", _swapPosition2.y, "time", _swapTime, "easetype", iTween.EaseType.linear, "onComplete", "SwapEndEvent"
                , "oncompletetarget", gameObject));
            iTween.MoveTo(_swapObject2, iTween.Hash("x", _swapPosition.x, "y", _swapPosition.y, "time", _swapTime, "easetype", iTween.EaseType.linear));


            _swapPosition = _swapObject.transform.position;
            _swapPosition2 = _swapObject2.transform.position;

            _swap = false;
        }
    }

    /// <summary>
    /// 스왑후 매칭이 되는 블럭이 있는지 체크 후 특수 블럭 생성 및 블럭 삭제 함수
    /// </summary>
    bool SwapMatchingCheck(int row, int col)
    {
        bool blockMatching = false;
        bool cookieCheck = false;
        int destroyCount = 0;
        int maxCheck = 4;
        int leftCount = 0;
        int rightCount = 0;
        int upCount = 0;
        int downCount = 0;
        List<GameObject> rowRemoveList = new List<GameObject>();
        List<GameObject> colRemoveList = new List<GameObject>();

        if (_gameBoard[row][col] == null)
        {
            return false;
        }


        // 윗쪽 체크
        for (int i = 1; i <= maxCheck; i++)
        {
            if (_gameBoard.ContainsKey(row - i) && _gameBoard[row - i].ContainsKey(col) && _gameBoard[row - i][col] != null)
            {
                if (!_gameBoard[row - i][col].tag.Contains(_gameBoard[row][col].tag))
                    break;
                upCount++;
                rowRemoveList.Add(_gameBoard[row - i][col]);
            }
        }

        // 아랫쪽 체크
        for (int i = 1; i <= maxCheck; i++)
        {
            if (_gameBoard.ContainsKey(row + i) && _gameBoard[row + i].ContainsKey(col) && _gameBoard[row + i][col] != null)
            {
                if (!_gameBoard[row + i][col].tag.Contains(_gameBoard[row][col].tag))
                    break;
                downCount++;
                rowRemoveList.Add(_gameBoard[row + i][col]);
            }
        }

        // 왼쪽 체크
        for (int i = 1; i <= maxCheck; i++)
        {
            if (_gameBoard.ContainsKey(row) && _gameBoard[row].ContainsKey(col - i) && _gameBoard[row][col - i] != null)
            {
                if (!_gameBoard[row][col - i].tag.Contains(_gameBoard[row][col].tag))
                    break;
                leftCount++;
                colRemoveList.Add(_gameBoard[row][col - i]);
            }
        }

        // 오른쪽 체크
        for (int i = 1; i <= maxCheck; i++)
        {
            if (_gameBoard.ContainsKey(row) && _gameBoard[row].ContainsKey(col + i) && _gameBoard[row][col + i] != null)
            {
                if (!_gameBoard[row][col + i].tag.Contains(_gameBoard[row][col].tag))
                    break;
                rightCount++;
                colRemoveList.Add(_gameBoard[row][col + i]);
            }
        }

        // 북서 방향 블록 체크
        if (upCount >= 1 && leftCount >= 1)
        {
            cookieCheck = CookieCheck(row - 1, col - 1, _gameBoard[row][col]);
            destroyCount = cookieCheck ? destroyCount + 1 : destroyCount;
        }

        // 북동 방향 블록 체크
        if (upCount >= 1 && rightCount >= 1)
        {
            if (cookieCheck)
            {
                destroyCount = CookieCheck(row - 1, col + 1, _gameBoard[row][col]) ? destroyCount + 1 : destroyCount;
            }
            else
            {
                cookieCheck = CookieCheck(row - 1, col + 1, _gameBoard[row][col]);
                destroyCount = cookieCheck ? destroyCount + 1 : destroyCount;
            }
        }

        // 남서 방향 블록 체크
        if (downCount >= 1 && leftCount >= 1)
        {
            if (cookieCheck)
            {
                destroyCount = CookieCheck(row + 1, col - 1, _gameBoard[row][col]) ? destroyCount + 1 : destroyCount;
            }
            else
            {
                cookieCheck = CookieCheck(row + 1, col - 1, _gameBoard[row][col]);
                destroyCount = cookieCheck ? destroyCount + 1 : destroyCount;
            }
        }

        // 남동 방향 블록 체크
        if (downCount >= 1 && rightCount >= 1)
        {
            if (cookieCheck)
            {
                destroyCount = CookieCheck(row + 1, col + 1, _gameBoard[row][col]) ? destroyCount + 1 : destroyCount;
            }
            else
            {
                cookieCheck = CookieCheck(row + 1, col + 1, _gameBoard[row][col]);
                destroyCount = cookieCheck ? destroyCount + 1 : destroyCount;
            }
        }

        // 퍼즐 체크 후 특수블럭 생성 및 파괴 부분
        if (cookieCheck) // 쿠키 생성 부분
        {
            destroyCount++;
            Destroy(_gameBoard[row][col]);
            ColRowBoomCheck(_gameBoard[row][col]);
            GameObject go = Instantiate(ResourcePoolManager.instance.GetUIPrefabFromType(DefineHelper.eUIblocktype.Cookie), _blockPos.transform);
            BlockInfoSave(go, row, col, (int)DefineHelper.eUIblocktype.Cookie);
            blockMatching = true;

        }
        else if (rowRemoveList.Count >= 4 || colRemoveList.Count >= 4) // 지정 폭탄
        {
            destroyCount++;
            Destroy(_gameBoard[row][col]);
            ColRowBoomCheck(_gameBoard[row][col]);
            GameObject go = Instantiate(ResourcePoolManager.instance.GetUIPrefabFromType(DefineHelper.eUIblocktype.SweetStar), _blockPos.transform);
            BlockInfoSave(go, row, col, (int)DefineHelper.eUIblocktype.SweetStar);
            blockMatching = true;
        }
        else if (rowRemoveList.Count >= 2 && colRemoveList.Count >= 2) // 폭탄 생성 부분
        {
            destroyCount++;
            CandyObj originCandyobj = _gameBoard[row][col].GetComponent<CandyObj>();
            ColRowBoomCheck(_gameBoard[row][col]);
            int typeNum = originCandyobj._typeNum;
            Destroy(_gameBoard[row][col]);
            GameObject go = Instantiate(ResourcePoolManager.instance.GetBoomBlockFromType((DefineHelper.eUIBoomblocktype)typeNum), _blockPos.transform);
            BlockInfoSave(go, row, col, typeNum);
            blockMatching = true;
        }
        else if (rowRemoveList.Count >= 3) // 좌우 폭탄 생성 부분 col 폭탄
        {
            destroyCount++;
            CandyObj originCandyobj = _gameBoard[row][col].GetComponent<CandyObj>();
            ColRowBoomCheck(_gameBoard[row][col]);
            int typeNum = originCandyobj._typeNum;
            Destroy(_gameBoard[row][col]);
            GameObject go = Instantiate(ResourcePoolManager.instance.GetColBlockFromType((DefineHelper.eUIColblocktype)typeNum), _blockPos.transform);
            BlockInfoSave(go, row, col, typeNum);
            blockMatching = true;
        }
        else if (colRemoveList.Count >= 3) // 상하 폭탄 생성 부분 row 폭탄
        {
            destroyCount++;
            CandyObj originCandyobj = _gameBoard[row][col].GetComponent<CandyObj>();
            ColRowBoomCheck(_gameBoard[row][col]);
            int typeNum = originCandyobj._typeNum;
            Destroy(_gameBoard[row][col]);
            GameObject go = Instantiate(ResourcePoolManager.instance.GetRowBlockPrefabFromType((DefineHelper.eUIRowblocktype)typeNum), _blockPos.transform);
            BlockInfoSave(go, row, col, typeNum);
            blockMatching = true;
        }
        else if (rowRemoveList.Count >= 2 || colRemoveList.Count >= 2)
        {
            destroyCount++;
            ColRowBoomCheck(_gameBoard[row][col]);
            Destroy(_gameBoard[row][col]);
            _gameBoard[row][col] = null;
            blockMatching = true;
        }

        if (rowRemoveList.Count >= 2 || cookieCheck)
        {
            foreach (GameObject go in rowRemoveList)
            {
                CandyObj candy = go.GetComponent<CandyObj>();
                ColRowBoomCheck(go);
                destroyCount++;
                Destroy(go);
                _gameBoard[candy.row][candy.col] = null;
            }
        }

        if (colRemoveList.Count >= 2 || cookieCheck)
        {
            foreach (GameObject go in colRemoveList)
            {
                CandyObj candy = go.GetComponent<CandyObj>();
                ColRowBoomCheck(go);
                destroyCount++;
                Destroy(go);
                _gameBoard[candy.row][candy.col] = null;
            }
        }

        if (destroyCount != 0)
        {
            SoundManager.instance.PlaySfxSoundOneShot(DefineHelper.eFxType.Click_tock);
        }

        InGameManager._instance.AddScore(destroyCount * 20 * _squareCount);
        _squareCount++;
        return blockMatching;
    }

    // 먼치킨 볼 매칭 검사를 하는 함수
    bool CookieCheck(int row, int col, GameObject gameObject)
    {
        if (_gameBoard.ContainsKey(row) && _gameBoard[row].ContainsKey(col) && _gameBoard[row][col] != null)
        {
            if (_gameBoard[row][col].tag.Contains(gameObject.tag.ToString()))
            {
                Destroy(_gameBoard[row][col]);
                _gameBoard[row][col] = null;
                return true;
            }
        }
        return false;
    }

    // 스왑 후 이벤트 함수
    public void SwapEndEvent()
    {
        bool match1 = false;
        bool match2 = false;
        if (_swapObject != null)
        {
            match1 = SwapMatchingCheck(_swapObject.GetComponent<CandyObj>().row, _swapObject.GetComponent<CandyObj>().col);
        }

        if (_swapObject2 != null)
        {
            match2 = SwapMatchingCheck(_swapObject2.GetComponent<CandyObj>().row, _swapObject2.GetComponent<CandyObj>().col);
        }

        SoundManager.instance.PlaySfxSoundOneShot(DefineHelper.eFxType.Swap);
        if (!match1 && !match2)
        {
            SwapFail();
        }
        else
        {
            InGameManager._instance.CandyMove(false);
        }
    }

    // 스왑 실패시 원래 포지션으로 돌아가도록 하는 함수
    void SwapFail()
    {
        if (_swapObject != null && _swapObject2 != null)
        {
            ChangeObjectInfo(_swapObject.GetComponent<CandyObj>().row, _swapObject.GetComponent<CandyObj>().col, _swapObject2.GetComponent<CandyObj>().row, _swapObject2.GetComponent<CandyObj>().col);
            _swapPosition = _swapObject.transform.position;
            _swapPosition2 = _swapObject2.transform.position;
            iTween.MoveTo(_swapObject, iTween.Hash("x", _swapPosition2.x, "y", _swapPosition2.y, "time", _swapTime, "easetype", iTween.EaseType.linear, "onComplete", "SwapFailPositionSet"
            , "oncompletetarget", this.gameObject, "oncompleteparams", _swapObject));
            iTween.MoveTo(_swapObject2, iTween.Hash("x", _swapPosition.x, "y", _swapPosition.y, "time", _swapTime, "easetype", iTween.EaseType.linear, "onComplete", "SwapFailPositionSet"
            , "oncompletetarget", this.gameObject, "oncompleteparams", _swapObject2));
        }
    }

    // 스왑 실패시 포지션이 흐트러지지 않도록 고정하는 함수
    public void SwapFailPositionSet(object candyObject)
    {
        GameObject go = (GameObject)candyObject;
        CandyObj candy = go.GetComponent<CandyObj>();
        RectTransform rectTransform = go.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(10 + candy.col * 100, -10 + candy.row * -100);
    }

    //먼치킨 볼 사용 함수
    public void CookieRoll(int row, int col, DefineHelper.eDirection direction)
    {
        int moveCount = 0;
        _downStop = true;
        RectTransform rect = _gameBoard[row][col].GetComponent<RectTransform>();
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector2(rect.anchoredPosition.x + 40, rect.anchoredPosition.y - 40);
        switch (direction)
        {
            case DefineHelper.eDirection.Left:
                _dummyPoint.anchoredPosition = new Vector2(10, rect.anchoredPosition.y);
                moveCount = (int)(rect.anchoredPosition.x) / 100;
                break;
            case DefineHelper.eDirection.Right:
                _dummyPoint.anchoredPosition = new Vector2(810, rect.anchoredPosition.y);
                moveCount = (int)(810 - rect.anchoredPosition.x) / 100;
                moveCount++;
                break;
            case DefineHelper.eDirection.Up:
                _dummyPoint.anchoredPosition = new Vector2(rect.anchoredPosition.x, -10);
                moveCount = (int)(rect.anchoredPosition.y * -1) / 100;
                break;
            case DefineHelper.eDirection.Down:
                _dummyPoint.anchoredPosition = new Vector2(rect.anchoredPosition.x, -810);
                moveCount = (int)(810 + rect.anchoredPosition.y) / 100;
                moveCount++;
                break;
        }
        StartCoroutine(RollBreak(row, col, moveCount, direction));
        InGameManager._instance.CandyMove(true);
        iTween.MoveTo(_gameBoard[row][col], iTween.Hash("x", _dummyPoint.transform.position.x, "y", _dummyPoint.transform.position.y, "time", _swapTime * moveCount, "easetype", iTween.EaseType.linear, "onComplete", "RollEndEvent"
            , "oncompletetarget", this.gameObject, "oncompleteparams", _gameBoard[row][col]));
    }
    public void RollEndEvent(object rollObj)
    {
        _downStop = false; 
        GameObject go = (GameObject)rollObj;
        CandyObj candy = go.GetComponent<CandyObj>();
        _gameBoard[candy.row][candy.col] = null;
        Destroy(go);
        InGameManager._instance.TargetSuccess();
    }
    IEnumerator RollBreak(int row, int col, int moveCount, DefineHelper.eDirection dir)
    {
        InGameManager._instance.AddScore(60 * _squareCount);
        for (int i = 1; i < moveCount; i++)
        {
            switch (dir)
            {
                case DefineHelper.eDirection.Left:
                    if (_gameBoard[row][col - i] != null && _gameBoard[row][col - i].name.Contains("Row"))
                    {
                        RowBoom(col - i);
                    }
                    if (_gameBoard[row][col - i] != null &&_gameBoard[row][col - i].name.Contains("Sweet"))
                    {
                        CookieSweetCheck(row, col - i);
                    }
                    else
                    {
                        Destroy(_gameBoard[row][col - i]);
                        _gameBoard[row][col - i] = null;
                    }
                    break;
                case DefineHelper.eDirection.Right:
                    if (_gameBoard[row][col + i] != null && _gameBoard[row][col + i].name.Contains("Row"))
                    {
                        RowBoom(col + i);
                    }
                    if (_gameBoard[row][col + i] != null && _gameBoard[row][col + i].name.Contains("Sweet"))
                    {
                        CookieSweetCheck(row, col + i);
                    }
                    else
                    {
                        Destroy(_gameBoard[row][col + i]);
                        _gameBoard[row][col + i] = null;
                    }
                    break;
                case DefineHelper.eDirection.Up:
                    if (_gameBoard[row - i][col] != null && _gameBoard[row - i][col].name.Contains("Col"))
                    {
                        ColBoom(row - i);
                    }
                    if (_gameBoard[row - i][col] != null && _gameBoard[row - i][col].name.Contains("Sweet"))
                    {
                        CookieSweetCheck(row - i, col);
                    }
                    else
                    {
                        Destroy(_gameBoard[row - i][col]);
                        _gameBoard[row - i][col] = null;
                    }
                    break;
                case DefineHelper.eDirection.Down:
                    if (_gameBoard[row + i][col] != null && _gameBoard[row + i][col].name.Contains("Col"))
                    {
                        ColBoom(row + i);
                    }
                    if (_gameBoard[row + i][col] != null && _gameBoard[row + i][col].name.Contains("Sweet"))
                    {
                        CookieSweetCheck(row + i, col);
                    }
                    else
                    {
                        Destroy(_gameBoard[row + i][col]);
                        _gameBoard[row + i][col] = null;
                    }
                    break;
            }

            SoundManager.instance.PlaySfxSoundOneShot(DefineHelper.eFxType.StoneBreak);
            InGameManager._instance.AddScore(60 * _squareCount);
            yield return new WaitForSeconds(_swapTime);
        }

        
        _squareCount++;
    }

    //지정 폭탄 사용 함수
    public void SweetStar(int row, int col, DefineHelper.eDirection direction)
    {
        _downStop = true;
        GameObject containObject = _gameBoard[row][col];

        switch (direction)
        {
            case DefineHelper.eDirection.Left:
                containObject = _gameBoard[row][col - 1];
                break;
            case DefineHelper.eDirection.Right:
                containObject = _gameBoard[row][col + 1];
                break;
            case DefineHelper.eDirection.Up:
                containObject = _gameBoard[row - 1][col];
                break;
            case DefineHelper.eDirection.Down:
                containObject = _gameBoard[row + 1][col];
                break;
        }
        
        iTween.MoveTo(_gameBoard[row][col], iTween.Hash("x", containObject.transform.position.x, "y", containObject.transform.position.y, "time", _swapTime, "easetype", iTween.EaseType.linear, "onComplete", "SweetStarBreak"
            , "oncompletetarget", this.gameObject, "oncompleteparams", containObject));
        
    }

    public void SweetStarBreak(object tagGo)
    {
        GameObject tagObj = (GameObject)tagGo;
        string tagName = tagObj.tag;
        int destroyCount = 0;
        for (int i = 1; i <= _candyCount; i++)
        {
            for (int j = 1; j <= _candyCount; j++)
            {
                if (_gameBoard[i][j] != null)
                {
                    if (_gameBoard[i][j].tag.Contains(tagName) || _gameBoard[i][j].tag.Contains(DefineHelper.eUIblocktype.SweetStar.ToString()))
                    {
                        if (_gameBoard[i][j].tag.Contains(DefineHelper.eUIblocktype.Cookie.ToString()))
                        {
                            CookieRoll(i, j, (DefineHelper.eDirection)Random.Range(0, 4));
                        }
                        else
                        {
                            destroyCount++;
                            Destroy(_gameBoard[i][j]);
                            _gameBoard[i][j] = null;

                        }
                    }
                }
            }
        }
        SoundManager.instance.PlaySfxSoundOneShot(DefineHelper.eFxType.TreeBreak);
        InGameManager._instance.AddScore(destroyCount * 60 * _squareCount);
        _squareCount *= 2;
        _downStop = false;
    }

    // 상하, 좌우 일자 폭탄 체크 함수
    public void ColRowBoomCheck(GameObject go)
    {
        if(go != null)
        {
            CandyObj candy = go.GetComponent<CandyObj>();
            if (go.name.Contains("Col"))
            {
                ColBoom(candy.row);
            }

            if (go.name.Contains("Row"))
            {
                RowBoom(candy.col);
            }

            if (go.name.Contains("Boom"))
            {
                Boom(candy.row, candy.col);
            }
        }
        
    }

    // 상하 좌우 대각선 폭탄 체크 함수
    public void Boom(int row, int col)
    {
        if (_gameBoard[row][col] != null)
        {
            Destroy(_gameBoard[row][col]);
            _gameBoard[row][col] = null;
            InGameManager._instance.AddScore(20 * _squareCount);
        }
        for(int i = 0; i < 2; i++)
        {
            for(int j = 0; j< 2; j++)
            {
                if (row-i >= 1 && col - j >= 1 && _gameBoard[row - i][col - j] != null)
                {
                    ColRowBoomCheck(_gameBoard[row - i][col - j]);
                    CookieSweetCheck(row - i, col - j);
                }
                if (row + i <= 7 && col + j <= 7 && _gameBoard[row + i][col + j] != null)
                {
                    ColRowBoomCheck(_gameBoard[row + i][col + j]);
                    CookieSweetCheck(row + i, col + j);
                }
                if (row + i <= 7 && col - j >= 1 && _gameBoard[row + i][col - j] != null)
                {
                    ColRowBoomCheck(_gameBoard[row + i][col - j]);
                    CookieSweetCheck(row + i, col - j);
                }
                if (row - i >= 1 && col + j <= 7 && _gameBoard[row - i][col + j] != null)
                {
                    ColRowBoomCheck(_gameBoard[row - i][col + j]);
                    CookieSweetCheck(row - i, col + j);
                }
            }
        }

        _squareCount++;
    }

    // 특수블록으로 특수블럭에 영향을 줬을때 체크 함수
    void CookieSweetCheck(int row, int col)
    {
        if(_gameBoard[row][col] != null)
        {
            if (_gameBoard[row][col].name.Contains("Cookie"))
            {
                CookieRoll(row, col, (DefineHelper.eDirection)Random.Range(1, 5));
            }
            else if (_gameBoard[row][col].name.Contains("Sweet"))
            {
                if (row == 1)
                {
                    SweetStar(row, col, DefineHelper.eDirection.Down);
                } else if (row == 7)
                {
                    SweetStar(row, col, DefineHelper.eDirection.Up);
                } else if (col == 1)
                {
                    SweetStar(row, col, DefineHelper.eDirection.Right);
                }else if( col == 7)
                {
                    SweetStar(row, col, DefineHelper.eDirection.Left);
                }
                else
                {
                    SweetStar(row, col, (DefineHelper.eDirection)Random.Range(1, 5));
                }
            }
            else
            {
                Destroy(_gameBoard[row][col]);
                _gameBoard[row][col] = null;
                InGameManager._instance.AddScore(20 * _squareCount);
            }
        }
    }

    // 좌우 폭탄 함수
    public void ColBoom(int row)
    {
        int destroyCount = 0;
        for(int i = 1; i <= _candyCount; i++)
        {
            if (_gameBoard[row][i] != null)
            {
                if (_gameBoard[row][i].name.Contains("Row"))
                {
                    Destroy(_gameBoard[row][i]);
                    _gameBoard[row][i] = null;
                    destroyCount++;
                    RowBoom(i);
                }else if (_gameBoard[row][i].name.Contains("Cookie"))
                {
                    CookieRoll(row, i, (DefineHelper.eDirection)Random.Range(3,5));
                }
                else if (_gameBoard[row][i].name.Contains("Sweet"))
                {
                    SweetStar(row, i, (DefineHelper.eDirection)Random.Range(1, 5));
                }
                else
                {
                    Destroy(_gameBoard[row][i]);
                    _gameBoard[row][i] = null;
                    destroyCount++;
                }
            }
            
        }
        InGameManager._instance.AddScore(destroyCount * 20 * _squareCount);
        _squareCount++;
    }

    // 상하 폭탄 함수
    public void RowBoom(int col)
    {
        int destroyCount = 0;
        for (int i = 1; i <= _candyCount; i++)
        {
            if(_gameBoard[i][col] != null )
            {
                if (_gameBoard[i][col].name.Contains("Col"))
                {
                    Destroy(_gameBoard[i][col]);
                    _gameBoard[i][col] = null;
                    destroyCount++;
                    ColBoom(i);
                }
                else if (_gameBoard[i][col].name.Contains("Cookie"))
                {
                    CookieRoll(i, col, (DefineHelper.eDirection)Random.Range(1, 3));
                }
                else if (_gameBoard[i][col].name.Contains("Sweet"))
                {
                    SweetStar(i, col, (DefineHelper.eDirection)Random.Range(1, 5));
                }
                else
                {
                    Destroy(_gameBoard[i][col]);
                    _gameBoard[i][col] = null;
                    destroyCount++;
                }
            }
        }
        InGameManager._instance.AddScore(destroyCount * 20 * _squareCount);
        _squareCount++;
    }
    // 블럭 스왑 시 블럭 정보 변경 함수
    void ChangeObjectInfo(int row, int col, int changeRow, int changeCol)
    {
        _swapObject = _gameBoard[row][col];
        _swapObject2 = _gameBoard[changeRow][changeCol];
        _gameBoard[row][col] = _swapObject2;
        _gameBoard[changeRow][changeCol] = _swapObject;
        if (_swapObject != null)
        {
            CandyObj candyObj = _swapObject.GetComponent<CandyObj>();
            candyObj.row = changeRow;
            candyObj.col = changeCol;
        }
        if (_swapObject2 != null)
        {
            CandyObj candyObj2 = _swapObject2.GetComponent<CandyObj>();
            candyObj2.row = row;
            candyObj2.col = col;
        }
    }
    
    // 블록이 떨어진 후 이벤트
    public void DropEndEvent(object cmpParams)
    {
        GameObject go = (GameObject)cmpParams;
        CandyObj dropCandy = go.GetComponent<CandyObj>();
        if (!_dropBlocks.Contains(dropCandy))
        {
            _dropBlocks.Enqueue(dropCandy);
        }
        //SwapMatchingCheck(dropCandy.row, dropCandy.col);
    }
    //떨어진 블럭 매치 검사
    public void DropAddMatchingCheck()
    {
        if(_dropBlocks.Count > 0)
        {
            CandyObj dropCandy = _dropBlocks.Dequeue();
            if(dropCandy != null)
            {
                if (!dropCandy.gameObject.tag.Contains("Cookie"))
                {
                    SwapMatchingCheck(dropCandy.row, dropCandy.col);
                }
            }
        }
    }

    //아래 블럭 체크 함수
    public bool UnderBlockCheck(int row, int col)
    {
        if(_gameBoard.ContainsKey(row) && _gameBoard[row].ContainsKey(col))
        {
            return _gameBoard[row][col] == null ? true : false;
        }
        else
        {
            return false;
        }  
    }
    //블럭 떨어지는 부분
    public void BlockDrop(int row, int col, int dropCount)
    {
        _dummyPoint.anchoredPosition = new Vector2(_gameBoard[row][col].GetComponent<RectTransform>().anchoredPosition.x, _gameBoard[row][col].GetComponent<RectTransform>().anchoredPosition.y - 100 * dropCount);

        _addTime = 0f;
        iTween.MoveTo(_gameBoard[row][col], iTween.Hash("y", _dummyPoint.transform.position.y, "time", _swapTime * dropCount, "easetype", iTween.EaseType.linear, "onComplete", "DropEndEvent" 
            , "oncompletetarget", this.gameObject, "oncompleteparams", _gameBoard[row][col]));

        ChangeObjectInfo(row, col, row + dropCount, col);
    }
    //블럭 추가
    void AddBlock(int col)
    {
        DefineHelper.eUIblocktype randomBlock = (DefineHelper.eUIblocktype)Random.Range(0, 4);
        GameObject go = Instantiate(ResourcePoolManager.instance.GetUIPrefabFromType(randomBlock), _blockPos.transform);
        BlockInfoSave(go, 1, col, (int)randomBlock);
        CandyObj candyobj = go.GetComponent<CandyObj>();
        _dropBlocks.Enqueue(candyobj);
        _addTime = 0f;
    }

    //
    void BlockInfoSave(GameObject go, int row, int col, int typeNum)
    {
        RectTransform rectTransform = go.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(10 + col * 100, -10 + row * -100);
        CandyObj candyobj = go.GetComponent<CandyObj>();
        candyobj.row = row;
        candyobj.col = col;
        candyobj._typeNum = typeNum;
        _gameBoard[row][col] = go;
    }
}
