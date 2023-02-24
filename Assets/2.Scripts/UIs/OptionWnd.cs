using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionWnd : MonoBehaviour
{
    static OptionWnd _uniqueInstance;
    [SerializeField] Slider _bgmBar;
    [SerializeField] Slider _sfxBar;
    [SerializeField] Image _bgmButton;
    [SerializeField] Image _sfxButton;
    Dictionary<string, string> optionInfo = new Dictionary<string, string>();

    Color _bgmMuteColor;
    Color _sfxMuteColor;

    bool Mute;
    bool SfxMute;
    private void Awake()
    {
        _uniqueInstance = this;
    }
    private void Start()
    {
        _bgmMuteColor = Color.white;
        _sfxMuteColor = Color.white;
        _bgmButton.color = _bgmMuteColor;
        _sfxButton.color = _sfxMuteColor;
        SoundSeting();
    }
    private void Update()
    {
        SoundManager.instance.GetSetBgmVolume = _bgmBar.value;
        SoundManager.instance.GetSetSfxVolume = _sfxBar.value;
    }
    public static OptionWnd instance
    {
        get { return _uniqueInstance; }
    }
    public float GetBgmBar
    {
        get { return _bgmBar.value; }
    }
    public void SoundSeting()
    {
        _bgmBar.value = SoundManager.instance.GetSetBgmVolume;
        _sfxBar.value = SoundManager.instance.GetSetSfxVolume;
        Mute = SoundManager.instance.GetSetBgmMult;
        SfxMute = SoundManager.instance.GetSetSfxMute;

        if (!Mute)
        {
            _bgmMuteColor.a = 1f;
            _bgmButton.color = _bgmMuteColor;
        }
            
        else
        {
            _bgmMuteColor.a = 0.6f;
            _bgmButton.color = _bgmMuteColor;
        }

        if (!SfxMute)
        {
            _sfxMuteColor.a = 1f;
            _sfxButton.color = _sfxMuteColor;
        }
        else
        {

            _sfxMuteColor.a = 0.6f;
            _sfxButton.color = _sfxMuteColor;
        }
        PlayerPrefs.SetFloat("BGM", _bgmBar.value);
        PlayerPrefs.SetFloat("Sfx", _sfxBar.value);
        PlayerPrefs.SetString("BgmMute", Mute.ToString());
        PlayerPrefs.SetString("SfxMute", SfxMute.ToString());
    }

    public void BgmMultButton()
    {
        SoundManager.instance.PlaySfxSoundOneShot(DefineHelper.eFxType.Click_tock);
        if (!Mute)
        {
            Mute = true;
            _bgmMuteColor.a = 0.6f;
            _bgmButton.color = _bgmMuteColor;
            SoundManager.instance.GetSetBgmMult = Mute;
        }
        else
        {
            Mute = false;
            _bgmMuteColor.a = 1f;
            _bgmButton.color = _bgmMuteColor;
            SoundManager.instance.GetSetBgmMult = Mute;
        }
    }
    public void SfxMultButton()
    {
        SoundManager.instance.PlaySfxSoundOneShot(DefineHelper.eFxType.Click_tock);
        if (!SfxMute)
        {
            SfxMute = true;
            _sfxMuteColor.a = 0.6f;
            _sfxButton.color = _sfxMuteColor;
            SoundManager.instance.GetSetSfxMute = SfxMute;
        }
        else
        {
            SfxMute = false;
            _sfxMuteColor.a = 1f;
            _sfxButton.color = _sfxMuteColor;
            SoundManager.instance.GetSetSfxMute = SfxMute;
        }
    }

    public void CommonDownButton(RectTransform rtf)
    {
        rtf.anchoredPosition = new Vector2(rtf.anchoredPosition.x, rtf.anchoredPosition.y - 25);
    }
    public void CommonUpButton(RectTransform rtf)
    {
        rtf.anchoredPosition = new Vector2(rtf.anchoredPosition.x, rtf.anchoredPosition.y + 25);
    }
    public void ClilckOkButton()
    {
        SoundManager.instance.PlaySfxSoundOneShot(DefineHelper.eFxType.Click_tock);
        PlayerPrefs.SetFloat("BGM", _bgmBar.value);
        PlayerPrefs.SetFloat("Sfx", _sfxBar.value);
        PlayerPrefs.SetString("BgmMute", Mute.ToString());
        PlayerPrefs.SetString("SfxMute", SfxMute.ToString());
        gameObject.SetActive(false);
    }
}
