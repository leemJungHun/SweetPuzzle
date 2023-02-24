using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainManager : MonoBehaviour
{
    static MainManager _uniqueInstance;
    GameObject _option;

    void Awake()
    {
        _uniqueInstance = this;
    }


    public static MainManager _instance
    {
        get { return _uniqueInstance; }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OptionOpen()
    {
        SoundManager.instance.PlaySfxSoundOneShot(DefineHelper.eFxType.Click_tock);
        if (_option == null)
        {
            _option = Instantiate(ResourcePoolManager.instance.GetWindowPrefabFromType(DefineHelper.eUIwindowtype.OptionWnd));
        }
        else
        {
            _option.SetActive(true);
        }


    }
    public void StartButton()
    {
        SoundManager.instance.PlaySfxSoundOneShot(DefineHelper.eFxType.Click_tock);
        SceneControlManager._instance.StartIngameScene();
    }

    public void ExitButton()
    {
        SoundManager.instance.PlaySfxSoundOneShot(DefineHelper.eFxType.Click_tock);

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
