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
        //���� ȿ����
        battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Paper);
        //���� ��ȭ
        battleUIManager.audioManager.isPlayBgm = BgmToggle.isOn;
        //Ȱ��ȭ ��Ų ���
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
        //���� ȿ����
        battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Paper);
        //���� ��ȭ
        battleUIManager.audioManager.isPlaySfx = SfxToggle.isOn;
        //Ȱ��ȭ ��Ų ���
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
        //���� ȿ����
        battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Door);
        //����
        SceneManager.LoadScene("Home");
    }
}
