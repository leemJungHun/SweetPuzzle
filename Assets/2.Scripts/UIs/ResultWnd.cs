using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultWnd : MonoBehaviour
{

    [SerializeField] Text _txtTotalScore;
    [SerializeField] Image[] _starImages;
    [SerializeField] Sprite[] _starSprites;

    //정보변수
    int _totalScore = 0;
    float _drawScore = 0;
    float _countingTime = 2;
    bool _gameClear = false;

    void LateUpdate()
    {
        if (_gameClear)
        {
            _totalScore = InGameManager._instance.totalScore;
            if (_totalScore <= _drawScore)
            {
                _txtTotalScore.text = string.Format("{0:#,0}", _totalScore);
                _starImages[0].sprite = _totalScore >= 3000 ? _starSprites[0] : _starSprites[1];
                _starImages[1].sprite = _totalScore >= 10000 ? _starSprites[0] : _starSprites[1];
                _starImages[2].sprite = _totalScore >= 50000 ? _starSprites[0] : _starSprites[1];
            }
            else
            {
                _drawScore += _totalScore * (Time.deltaTime / _countingTime);
                _txtTotalScore.text = string.Format("{0:#,0}", _drawScore);
                _starImages[0].sprite = _drawScore >= 3000 ? _starSprites[0] : _starSprites[1];
                _starImages[1].sprite = _drawScore >= 10000 ? _starSprites[0] : _starSprites[1];
                _starImages[2].sprite = _drawScore >= 50000 ? _starSprites[0] : _starSprites[1];
            }
        }
       
    }

    public void ClickOkButton()
    {
        SoundManager.instance.PlaySfxSoundOneShot(DefineHelper.eFxType.Click_tock);
        SceneControlManager._instance.StartMainScene();
    }

    public void ClickExitButton()
    {
        SoundManager.instance.PlaySfxSoundOneShot(DefineHelper.eFxType.Click_tock);
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void OpenResultWindow(int totalScore, bool gameClear)
    {
        _gameClear = gameClear;
        if (gameClear)
        {
            _txtTotalScore.text = "0";
            _totalScore = totalScore;
            SoundManager.instance.PlaySfxSoundOneShot(DefineHelper.eFxType.Counting_Nor);

        }
        else
        {
            _txtTotalScore.text = "Failed";
            _txtTotalScore.color = Color.red;
            for(int i = 0; i< _starImages.Length; i++)
            {
                _starImages[i].gameObject.SetActive(false);
            }
        }
    }
}
