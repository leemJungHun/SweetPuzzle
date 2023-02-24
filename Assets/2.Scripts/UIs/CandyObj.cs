using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CandyObj : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    [SerializeField] Image _candy;
    RectTransform _myRect;
    Vector3 _inputVector;
    bool _touch = false;
    float _touchTime = 1f;
    float _checkTime = 0f;
    float _dropTime = 5f;
    int _dropCount = 0;
    bool _rollCookie = false;

    public int _typeNum
    {
        get; set;
    }

    //블럭의 열 위치
    public int row
    {
        get; set;
    }

    //블럭의 행 위치
    public int col
    {
        get; set;
    }
    public float _horizVal
    {
        get { return _inputVector.normalized.x > 0.9f ? 1 : _inputVector.normalized.x < -0.9f ? -1 : 0; }
    }

    public float _vetizVal
    {
        get { return _inputVector.normalized.y > 0.9f ? 1 : _inputVector.normalized.y < -0.9f ? -1 : 0; }
    }
    public Vector3 _roundVector
    {
        get { return new Vector2(_horizVal, _vetizVal); }
    }

    void Start()
    {
        _myRect = GetComponent<RectTransform>();
    }

    private void Update()
    {
        //터치를 한 시간이 1초 이상 지났을 경우 터치가 풀리도록
        if (_touch)
        {
            _checkTime += Time.deltaTime;
            if (_checkTime >= _touchTime)
            {
                _touch = false;
                _checkTime = 0;
            }
        }

        _dropTime += Time.deltaTime;
        if (_dropTime >= 0.2f && !GameBoardObj.instance._downStop)
        {
            if (GameBoardObj.instance.UnderBlockCheck(row + _dropCount + 1, col))
            {
                _dropCount++;
            }
            else
            {
                if (_dropCount != 0)
                {
                    GameBoardObj.instance.BlockDrop(row, col, _dropCount);
                    _dropCount = 0;
                    _dropTime = 0f;
                }
            }
        }

        if (_rollCookie)
        {
            _myRect.Rotate(new Vector3(0, 0, 360f) * Time.deltaTime);
        }
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        Vector2 pos;
        _touch = true;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_candy.rectTransform, eventData.position, eventData.pressEventCamera, out pos))
        {
            pos.x = (pos.x / _candy.rectTransform.sizeDelta.x);
            pos.y = (pos.y / _candy.rectTransform.sizeDelta.y);

            _inputVector = new Vector3(pos.x, pos.y, 0);
            _inputVector = (_inputVector.magnitude > 1) ? _inputVector.normalized : _inputVector;

        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Vector2 pos;
        if (_touch && GameBoardObj.instance._stopTime >= 0.5f && InGameManager._instance._ingameState == DefineHelper.eIngameState.PLAY)
        {
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_candy.rectTransform, eventData.position, eventData.pressEventCamera, out pos))
            {
                pos.x = (pos.x / _candy.rectTransform.sizeDelta.x);
                pos.y = (pos.y / _candy.rectTransform.sizeDelta.y);

                _inputVector = new Vector3(pos.x, pos.y, 0);
                _inputVector = (_inputVector.magnitude > 1) ? _inputVector.normalized : _inputVector;
            }

            //특수 블럭 체크
            switch (gameObject.tag.ToString())
            {
                case "Cookie":
                    //스왑 함수 불러오기
                    switch (_roundVector.x)
                    {
                        case 1://오른쪽
                            GameBoardObj.instance.CookieRoll(row, col, DefineHelper.eDirection.Right);
                            _rollCookie = true;
                            break;
                        case -1://왼쪽
                            GameBoardObj.instance.CookieRoll(row, col, DefineHelper.eDirection.Left);
                            _rollCookie = true;
                            break;
                    }
                    switch (_roundVector.y)
                    {
                        case 1://윗쪽
                            GameBoardObj.instance.CookieRoll(row, col, DefineHelper.eDirection.Up);
                            _rollCookie = true;
                            break;
                        case -1://아랫쪽
                            GameBoardObj.instance.CookieRoll(row, col, DefineHelper.eDirection.Down);
                            _rollCookie = true;
                            break;
                    }
                    break;
                case "SweetStar":
                    //스왑 함수 불러오기
                    switch (_roundVector.x)
                    {
                        case 1://오른쪽
                            if(col+1 != 8)
                            {
                                GameBoardObj.instance.SweetStar(row, col, DefineHelper.eDirection.Right);
                            }
                            break;
                        case -1://왼쪽
                            if(col - 1 != 0)
                            {
                                GameBoardObj.instance.SweetStar(row, col, DefineHelper.eDirection.Left);
                            }
                            break;
                    }
                    switch (_roundVector.y)
                    {
                        case 1://윗쪽
                            if(row - 1 != 0)
                            {
                                GameBoardObj.instance.SweetStar(row, col, DefineHelper.eDirection.Up);
                            }
                            break;
                        case -1://아랫쪽
                            if(row + 1 != 8)
                            {
                                GameBoardObj.instance.SweetStar(row, col, DefineHelper.eDirection.Down);
                            }
                            break;
                    }
                    break;
                default:
                    //스왑 함수 불러오기
                    switch (_roundVector.x)
                    {
                        case 1://오른쪽
                            GameBoardObj.instance.BlockSwap(row, col, DefineHelper.eDirection.Right);
                            break;
                        case -1://왼쪽
                            GameBoardObj.instance.BlockSwap(row, col, DefineHelper.eDirection.Left);
                            break;
                    }
                    switch (_roundVector.y)
                    {
                        case 1://윗쪽
                            GameBoardObj.instance.BlockSwap(row, col, DefineHelper.eDirection.Up);
                            break;
                        case -1://아랫쪽
                            GameBoardObj.instance.BlockSwap(row, col, DefineHelper.eDirection.Down);
                            break;
                    }
                    break;
            }
        }
        _touch = false;
    }

}
