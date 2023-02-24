using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcePoolManager : MonoBehaviour
{
    [SerializeField] GameObject[] _blockPrefabs;
    [SerializeField] GameObject[] _colBoomPrefabs;
    [SerializeField] GameObject[] _rowBoomPrefabs;
    [SerializeField] GameObject[] _boomPrefabs;
    [SerializeField] GameObject[] _windowPrefabs;
    [SerializeField] AudioClip[] _BgmList;
    [SerializeField] AudioClip[] _FxList;
    [SerializeField] string[] _tiostrings;
    static ResourcePoolManager _unique;
    private void Awake()
    {
        _unique = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public static ResourcePoolManager instance
    {
        get { return _unique; }
    }

    public AudioClip GetBgmClipFormType(DefineHelper.eBgmType type)
    {
        return _BgmList[(int)type];
    }

    public AudioClip GetFxClipFormType(DefineHelper.eFxType type)
    {
        return _FxList[(int)type];
    }
    public string GetRandomTip()
    {
        int idx = Random.Range(0, _tiostrings.Length);

        return _tiostrings[idx];
    }

    public GameObject GetUIPrefabFromType(DefineHelper.eUIblocktype blocktype)
    {
        return _blockPrefabs[(int)blocktype];
    }
    public GameObject GetRowBlockPrefabFromType(DefineHelper.eUIRowblocktype rowblocktype)
    {
        return _rowBoomPrefabs[(int)rowblocktype];
    }
    public GameObject GetColBlockFromType(DefineHelper.eUIColblocktype colblocktype)
    {
        return _colBoomPrefabs[(int)colblocktype];
    }
    public GameObject GetBoomBlockFromType(DefineHelper.eUIBoomblocktype boomblocktype)
    {
        return _boomPrefabs[(int)boomblocktype];
    }
    public GameObject GetWindowPrefabFromType(DefineHelper.eUIwindowtype windowtype)
    {
        return _windowPrefabs[(int)windowtype];
    }
}
