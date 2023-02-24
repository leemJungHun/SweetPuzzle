using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoaddingWnd : MonoBehaviour
{
    [SerializeField] Slider _bar;
    [SerializeField] Text _txtStaticLoadding;
    [SerializeField] Text _txtTipStr;

    float _checkTime = 0f;
    int _limitCount = 3;
    int _dotCnt = 0;


    void Update()
    {
        _checkTime += Time.deltaTime;
        if (_checkTime >= 0.5f)
        {
            _checkTime = 0;
            _txtStaticLoadding.text = "Loadding";
            for (int n = 0; n < _dotCnt; n++)
            {
                _txtStaticLoadding.text += ".";
            }
            if (++_dotCnt > _limitCount)
            {
                _dotCnt = 0;
            }
        }

    }

    public void SetLoaddingProgress(float pro)
    {
        _bar.value = pro;
    }

    public void OpenWindow()
    {

        _txtTipStr.text = ResourcePoolManager.instance.GetRandomTip();
        SetLoaddingProgress(0);
        //CancelInvoke("LoaddingTextChange");
        _txtStaticLoadding.text = "Loadding";
        _dotCnt++;

    }

   

    public void Close()
    {
        Destroy(gameObject);
    }
}
