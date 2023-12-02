using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeManager : MonoBehaviour
{
    public GameObject multyEnterBtn;
    BattleUIManager battleUIManager;
    private void Start()
    {
        battleUIManager = BattleUIManager.Instance;
        battleUIManager.battleType = BattleUIManager.BattleType.Rest;
    }

    bool isOnline = false;
    private void LateUpdate()//인터넷이 작동하는지 확인
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            // 인터넷 연결이 안되었을때
            isOnline = false;
        }
        else if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
        {
            // 데이터로 인터넷 연결이 되었을때
            isOnline = true;
        }
        else
        {
            // 와이파이로 연결이 되었을때
            isOnline = true;
        }
        if (isOnline != multyEnterBtn.activeSelf)//멀티 버튼 비활성화
            multyEnterBtn.SetActive(isOnline);
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
