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
        //�Ұ� ��ȭ
        battleUIManager.jsonManager.singleScore.isPlayBgm = BgmToggle.isOn;
        battleUIManager.audioManager.isPlayBgm = BgmToggle.isOn;

        //Ȱ��ȭ ��Ų ���
        if (BgmToggle.isOn)
        {
            BgmIcon.sprite = SoundIcon[0];
        }
        else //���� ��Ų ���
        {
            BgmIcon.sprite = SoundIcon[1];
        }
        //JSON ����
        battleUIManager.jsonManager.SaveData();

        //��� ����
        battleUIManager.audioManager.StopBgm();
        //���� ȿ����
        battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Paper);
    }

    public void SfxControl() 
    {
        //�Ұ� ��ȭ
        battleUIManager.jsonManager.singleScore.isPlaySfx = SfxToggle.isOn;
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
        //JSON ����
        battleUIManager.jsonManager.SaveData();

        //���� ȿ����
        battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Paper);
    }

    public void goHome()
    {
        //���� ȿ����
        battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Door);
        //����
        SceneManager.LoadScene("Home");
    }
}
