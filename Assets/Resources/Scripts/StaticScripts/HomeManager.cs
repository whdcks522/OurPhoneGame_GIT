using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeManager : MonoBehaviour
{
    BattleUIManager battleUIManager;
    private void Start()
    {
        battleUIManager = BattleUIManager.Instance;
        battleUIManager.battleType = BattleUIManager.BattleType.Rest;
    }

    public void loadSingleSel()//�̱� ������ �̵�
    {
        //���� ȿ����
        battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Paper);
        battleUIManager.battleType = BattleUIManager.BattleType.Single;

        SceneManager.LoadScene("SingleSelect");
    }

    public void loadSingleMul()//��Ƽ ������ �̵�
    {
        //���� ȿ����
        battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Paper);
        battleUIManager.battleType = BattleUIManager.BattleType.Multy;

        SceneManager.LoadScene("Lobby");
    }

    public void loadSettings()//���� ������ �̵�
    {
        //���� ȿ����
        battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Door);
        //�� ��ȯ
        SceneManager.LoadScene("Settings");
    }

    public void JsonControl(int _value)
    {
        //���� ȿ����
        battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Paper);
        //�� ��ȯ
        battleUIManager.jsonManager.DataClear(_value);
    }
}
