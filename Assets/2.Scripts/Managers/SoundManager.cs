using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    static SoundManager _uniqueInstance;

    AudioSource bgPlayer;
    AudioSource SfxPlayer;
    AudioSource[] fxplayer;
    float bgmvolume;
    float fxvolume;
    bool bgmMute;
    bool fxMute;


    List<AudioSource> sfxPlayer = new List<AudioSource>();
    public float GetSetBgmVolume
    {
        get { return bgmvolume; }
        set { bgmvolume = value; }
    }
    public float GetSetSfxVolume
    {
        get { return fxvolume; }
        set { fxvolume = value; }
    }
    public bool GetSetBgmMult
    {
        get { return bgmMute; }
        set { bgmMute = value; }
    }
    public bool GetSetSfxMute
    {
        get { return fxMute; }
        set { fxMute = value; }
    }
    public static SoundManager instance
    {
        get { return _uniqueInstance; }
    }

    private void Awake()
    {
        _uniqueInstance = this;
        DontDestroyOnLoad(this.gameObject);

        bgPlayer = GetComponent<AudioSource>();
        fxplayer = transform.GetChild(0).GetComponents<AudioSource>();
        SfxPlayer = transform.GetChild(1).GetComponent<AudioSource>();
        IntielizeSet(1);
    }

    private void Start()
    {
        //옵션 불러오기
        if (PlayerPrefs.HasKey("BGM"))
            bgmvolume = PlayerPrefs.GetFloat("BGM");
        if (PlayerPrefs.HasKey("Sfx"))
            fxvolume = PlayerPrefs.GetFloat("Sfx");
        if (PlayerPrefs.HasKey("BgmMute"))
            bgmMute = PlayerPrefs.GetString("BgmMute").Contains("True") ? true : false;
        if (PlayerPrefs.HasKey("SfxMute"))
            fxMute = PlayerPrefs.GetString("SfxMute").Contains("True") ? true : false;
    }


    private void Update()
    {

        bgPlayer.volume = bgmvolume;
        bgPlayer.mute = bgmMute;
        SfxPlayer.volume = fxvolume;
        SfxPlayer.mute = fxMute;


    }

    public void IntielizeSet(float b, bool bm = false, float f = 1, bool fm = false)
    {
        bgmvolume = b;
        bgmMute = bm;
        fxvolume = f;
        fxMute = fm;

    }
    public void PlayBgm(DefineHelper.eBgmType type, bool isloop = true)
    {
        bgPlayer.clip = ResourcePoolManager.instance.GetBgmClipFormType(type);
        bgPlayer.volume = bgmvolume;
        bgPlayer.mute = bgmMute;
        bgPlayer.loop = isloop;
        bgPlayer.Play();
    }

    private void LateUpdate()
    {
        for (int n = 0; n < sfxPlayer.Count; n++)
        {
            if (!sfxPlayer[n].isPlaying)
            {
                Destroy(sfxPlayer[n].gameObject);
                sfxPlayer.RemoveAt(n);
                break;
            }
        }
    }
    public void PlayCountingFx(DefineHelper.eFxType type)
    {
        fxplayer[0].clip = ResourcePoolManager.instance.GetFxClipFormType(type);
        fxplayer[0].volume = fxvolume;
        fxplayer[0].mute = fxMute;
        fxplayer[0].Play();
    }

    public void PlaySfxSoundOneShot(DefineHelper.eFxType type, bool isloop = false)
    {
        SfxPlayer.volume = fxvolume;
        SfxPlayer.mute = fxMute;
        SfxPlayer.loop = isloop;
        SfxPlayer.PlayOneShot(ResourcePoolManager.instance.GetFxClipFormType(type));
    }
}
