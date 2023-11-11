using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using AssetKits.ParticleImage;
using KoreanTyper;
using Unity.VisualScripting;

public class BattleUIManager : MonoBehaviour
{
    [Header("PC로 진행중인지")]
    public bool isPC;

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

    [Header("게임 매니저")]
    public GameManager gameManager;

    [Header("체력 바 슬라이더")]
    public Slider bigHealthBar;
    [Header("체력 바 슬라이더의 파티클")]
    public GameObject bigHealthBarParticle;

    [Header("랭크 텍스트")]
    public Text bigRankText;
    [Header("포인트 텍스트")]
    public Text bigScoreText;
    [Header("설명 텍스트")]
    public Text bigDescText;
    string saveDescText = "";
    //설명하는 코루틴
    Coroutine descCor;

    [Header("싱글 정지 버튼")]
    public GameObject singleStopBtn;
    [Header("멀티 나가기 버튼")]
    public GameObject multyExitBtn;


    [Header("정지 패널")]
    public GameObject stopPanel;
    [Header("이어하기 버튼")]
    public GameObject btnContinue;
    [Header("최종점수 텍스트")]
    public Text hiddenText;

    [Header("다른 컴포넌트 정보")]
    public AudioManager audioManager;

    [Header("목표 점수들")]
    public int Sscore;
    public int Ascore;
    public int Bscore;
    public int Cscore;
    public int Dscore;
    public int Escore;
    [Header("현재 생존 점수")]
    public float curScore;

    //채팅용 대기시간 0.025초
    WaitForSeconds wait0_025 = new WaitForSeconds(0.025f);
    //채팅용 대기시간 3초
    WaitForSeconds wait3_00 = new WaitForSeconds(3);

    

    public enum RankType
    {
        S, A, B, C, D, E
    }
    [Header("현재 생존 랭크")]
    public RankType rankType;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        battleUI.SetActive(false);
    }

    public void typingControl(string _str) 
    {
        //if (descCor != null) 
        {
            //StopCoroutine(descCor);
        }
        if (saveDescText != _str)
        {
            if (descCor != null)
                StopCoroutine(descCor);                
            descCor = StartCoroutine(typingRoutine(_str));
        }
    }

    #region 대화
    public IEnumerator typingRoutine(string str)
    {

            saveDescText = str;
            bigDescText.text = "";

            yield return null;

            int typingLength = str.GetTypingLength();
            for (int index = 0; index <= typingLength; index++)
            {
                bigDescText.text = str.Typing(index);

                yield return wait0_025;
                yield return wait0_025;

                //효과음
                audioManager.PlaySfx(AudioManager.Sfx.Typing);
            }
            yield return wait3_00;
            for (int index = typingLength; index >= 0; index--)
            {
                bigDescText.text = str.Typing(index);
                yield return wait0_025;
            }
            yield return null;

            saveDescText = "";
            bigDescText.text = "";
        
    }
    #endregion

    //정지 버튼
    public void btnStop()
    {
        //종이 효과음
        audioManager.PlaySfx(AudioManager.Sfx.Paper);

        bigHealthBarParticle.SetActive(false);

        stopPanel.SetActive(true);

        Time.timeScale = 0;
    }

    public void realBtnContinue()//이어하기
    {
        //종이 효과음
        audioManager.PlaySfx(AudioManager.Sfx.Paper);

        stopPanel.SetActive(false);
        Time.timeScale = 1;

        //파티클 이미지(안하면 오류남)
        Invoke("activateParticle", 0.1f);
    }

    void activateParticle() 
    {
        bigHealthBarParticle.SetActive(true);
    }


    public void btnRetry()//재시도 버튼
    {
        //이어하기 버튼 다시 보이도록
        btnContinue.SetActive(true);

        //정지 패널 안보이도록
        realBtnContinue();

        // 현재 씬을 다시 로드합니다.
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    
    public void btnExit()//나가기 버튼
    {
        //선택창 고르기
        if (battleType == BattleType.Single)//싱글
        {
            //이어하기 버튼 다시 보이도록
            btnContinue.SetActive(true);

            //정지 패널 안보이도록
            realBtnContinue();

            SceneManager.LoadScene("SingleSelect");
            battleUI.SetActive(false);
        }
        else if (battleType == BattleType.Multy)//멀티
        {
            //나가기
            gameManager.LeaveRoom();
        }
        else
        {
            Debug.Log("SceneError");
        }
        
    }

   

    void destroyBattleUiManager() { }
}
