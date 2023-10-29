using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BattleUIManager : MonoBehaviour
{
    #region 싱글턴
    private static BattleUIManager instance;
    public static BattleUIManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<BattleUIManager>();
            return instance;
        }
    }
    #endregion

    public enum BattleType 
    {
        Rest, Single, Multy
    }
    public BattleType battleType;

    [Header("전투 UI 정보")]
    public GameObject battleUI;

    [Header("플레이어 이동")]
    public VariableJoystick moveJoy;
    [Header("플레이어 칼")]
    public VariableJoystick swordJoy;
    [Header("체력 바 슬라이더")]
    public Slider bigHealthBar;
    [Header("랭크 텍스트")]
    public Text bigRankText;
    [Header("포인트 텍스트")]
    public Text bigPointText;
    [Header("정지 패널")]
    public GameObject stopPanel;

    [Header("다른 컴포넌트 정보")]
    public AudioManager audioManager;


    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        battleUI.SetActive(false);
    }

    public void loadSingleSel()
    {
        audioManager.PlaySfx(AudioManager.Sfx.PowerUp);
        SceneManager.LoadScene("SingleSelect");
    }

    //정지 버튼
    public void btnStop()
    {
        stopPanel.SetActive(true);
        Time.timeScale = 0;
    }

    //활성화 버튼
    public void btnActivate()
    {
        stopPanel.SetActive(false);
        Time.timeScale = 1;
    }

    //재시도 버튼
    public void btnRetry()
    {
        stopPanel.SetActive(false);
        Time.timeScale = 1;

        // 현재 씬을 다시 로드합니다.
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    //나가기 버튼
    public void btnExit()
    {
        stopPanel.SetActive(false);
        Time.timeScale = 1;

        //선택창 고르기
        if (battleType == BattleType.Single)
            SceneManager.LoadScene("SingleSelect");
        else 
        {
            Debug.Log("SceneError");
        }
        battleUI.SetActive(false);
    }
}
