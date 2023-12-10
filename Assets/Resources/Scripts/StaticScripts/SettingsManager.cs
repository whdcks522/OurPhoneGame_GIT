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

        if (!battleUIManager.jsonManager.singleScore.isPlayBgm) 
        {
            BgmToggle.isOn = false;
            BgmIcon.sprite = SoundIcon[1];
        }
        if (!battleUIManager.jsonManager.singleScore.isPlaySfx)
        {
            SfxToggle.isOn = false;
            SfxIcon.sprite = SoundIcon[1];
        }
    }

    public void BgmControl()
    {
        //불값 변화
        battleUIManager.jsonManager.singleScore.isPlayBgm = BgmToggle.isOn;
        battleUIManager.audioManager.isPlayBgm = BgmToggle.isOn;

        //활성화 시킨 경우
        if (BgmToggle.isOn)
        {
            BgmIcon.sprite = SoundIcon[0];
        }
        else //종료 시킨 경우
        {
            BgmIcon.sprite = SoundIcon[1];
        }
        //JSON 저장
        battleUIManager.jsonManager.SaveData();

        //브금 정지
        battleUIManager.audioManager.StopBgm();
        //수정 효과음
        battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Paper);
    }

    public void SfxControl() 
    {
        //불값 변화
        battleUIManager.jsonManager.singleScore.isPlaySfx = SfxToggle.isOn;
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
        //JSON 저장
        battleUIManager.jsonManager.SaveData();

        //수정 효과음
        battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Paper);
    }

    public void goHome()
    {
        //입장 효과음
        battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Door);
        //입장
        SceneManager.LoadScene("Home");
    }
}
