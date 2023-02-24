using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneControlManager : MonoBehaviour
{

    static SceneControlManager _uniqueInstance;

    DefineHelper._eSceneIndex _currScene;


    public static SceneControlManager _instance
    {
        get { return _uniqueInstance; }
    }
    // Start is called before the first frame update
    void Awake()
    {
        _uniqueInstance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        StartMainScene();
    }

    public void StartMainScene()
    {
        _currScene = DefineHelper._eSceneIndex.MainScene;
        StartCoroutine(LoaddingScene(DefineHelper._eSceneIndex.MainScene.ToString()));
        SoundManager.instance.PlayBgm(DefineHelper.eBgmType.Main);
    }
    public void StartIngameScene()
    {
        _currScene = DefineHelper._eSceneIndex.IngameScene;
        StartCoroutine(LoaddingScene(DefineHelper._eSceneIndex.IngameScene.ToString()));
        SoundManager.instance.PlayBgm(DefineHelper.eBgmType.Ingame);
    }

    IEnumerator LoaddingScene(string sceneName)
    {
        GameObject go = Instantiate(ResourcePoolManager.instance.GetWindowPrefabFromType(DefineHelper.eUIwindowtype.LoaddingWnd), transform);
        LoaddingWnd wnd = go.GetComponent<LoaddingWnd>();
        wnd.OpenWindow();
        yield return new WaitForSeconds(2);
        AsyncOperation aOper = SceneManager.LoadSceneAsync(sceneName);
        while (!aOper.isDone)
        {
            wnd.SetLoaddingProgress(aOper.progress);
            yield return null;
        }
        yield return new WaitForSeconds(2);
        wnd.SetLoaddingProgress(aOper.progress);
        while (go != null)
        {
            yield return null;
            go = null;
            wnd.Close();
        }

        if (_currScene == DefineHelper._eSceneIndex.IngameScene)
        {
            InGameManager._instance.InitializeSettings();
        }
    }
}
