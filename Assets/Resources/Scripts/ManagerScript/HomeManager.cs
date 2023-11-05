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
    }

    public void loadSingleSel()
    {
        //종이 효과음
        battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Paper);

        SceneManager.LoadScene("SingleSelect");
    }

    public void loadSingleMul()
    {
        //종이 효과음
        battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Paper);

        SceneManager.LoadScene("Lobby");
    }
}
