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

    public void loadSingleSel()//싱글 씬으로 이동
    {
        //종이 효과음
        battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Paper);
        battleUIManager.battleType = BattleUIManager.BattleType.Single;

        SceneManager.LoadScene("SingleSelect");
    }

    public void loadSingleMul()//멀티 씬으로 이동
    {
        //종이 효과음
        battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Paper);
        battleUIManager.battleType = BattleUIManager.BattleType.Multy;

        SceneManager.LoadScene("Lobby");
    }

    public void loadSettings()//세팅 씬으로 이동
    {
        //입장 효과음
        battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Door);
        //씬 변환
        SceneManager.LoadScene("Settings");
    }

    public void JsonControl(int _value)
    {
        //입장 효과음
        battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Paper);
        //씬 변환
        battleUIManager.jsonManager.DataClear(_value);
    }
}
