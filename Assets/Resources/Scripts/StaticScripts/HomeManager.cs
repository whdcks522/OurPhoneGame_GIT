using System.Collections;
using System.Collections.Generic;
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

    public void loadSingleSel()
    {
        //���� ȿ����
        battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Paper);
        battleUIManager.battleType = BattleUIManager.BattleType.Single;

        SceneManager.LoadScene("SingleSelect");
    }

    public void loadSingleMul()
    {
        //���� ȿ����
        battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Paper);
        battleUIManager.battleType = BattleUIManager.BattleType.Multy;

        SceneManager.LoadScene("Lobby");
    }

    public void tmp()
    {
        SceneManager.LoadScene("Train_0");
    }
}
