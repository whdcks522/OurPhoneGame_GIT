using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public Sprite[] SoundIcon;
    BattleUIManager battleUIManager;

    public Toggle BgmToggle;
    public Toggle SfxToggle;
    public Image BgmIcon;
    public Image SfxIcon;
    void Awake() 
    {
        battleUIManager = BattleUIManager.Instance;

        if (!battleUIManager.audioManager.isPlayBgm) 
        {
            BgmToggle.isOn = false;
            BgmIcon.sprite = SoundIcon[1];
        }
        if (!battleUIManager.audioManager.isPlaySfx)
        {
            SfxToggle.isOn = false;
            SfxIcon.sprite = SoundIcon[1];
        }
    }

    public void BgmControl()
    {
        //수정 효과음
        battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Paper);
        //상태 변화
        battleUIManager.audioManager.isPlayBgm = BgmToggle.isOn;
        //활성화 시킨 경우
        if (BgmToggle.isOn)
        {
            BgmIcon.sprite = SoundIcon[0];
        }
        else 
        {
            BgmIcon.sprite = SoundIcon[1];
        }
    }

    public void SfxControl() 
    {
        //수정 효과음
        battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Paper);
        //상태 변화
        battleUIManager.audioManager.isPlaySfx = SfxToggle.isOn;
        //활성화 시킨 경우
        if (BgmToggle.isOn)
        {
            SfxIcon.sprite = SoundIcon[0];
        }
        else
        {
            SfxIcon.sprite = SoundIcon[1];
        }
    }

    public void goHome()
    {
        //입장 효과음
        battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Door);
        //입장
        SceneManager.LoadScene("Home");
    }
}
