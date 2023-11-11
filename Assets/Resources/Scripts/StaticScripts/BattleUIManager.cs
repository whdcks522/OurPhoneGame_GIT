using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using AssetKits.ParticleImage;
using KoreanTyper;

public class BattleUIManager : MonoBehaviour
{
    [Header("PC�� ����������")]
    public bool isPC;

    #region �̱���
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

    [Header("���� UI ����")]
    public GameObject battleUI;

    [Header("�÷��̾� �̵�")]
    public VariableJoystick moveJoy;
    [Header("�÷��̾� Į")]
    public VariableJoystick swordJoy;

    [Header("���� �Ŵ���")]
    public GameManager gameManager;

    [Header("ü�� �� �����̴�")]
    public Slider bigHealthBar;
    [Header("ü�� �� �����̴��� ��ƼŬ")]
    public GameObject bigHealthBarParticle;

    [Header("��ũ �ؽ�Ʈ")]
    public Text bigRankText;
    [Header("����Ʈ �ؽ�Ʈ")]
    public Text bigScoreText;
    [Header("���� �ؽ�Ʈ")]
    public Text bigDescText;
    //�����ϴ� �ڷ�ƾ
    Coroutine descCor;

    [Header("�̱� ���� ��ư")]
    public GameObject singleStopBtn;
    [Header("��Ƽ ������ ��ư")]
    public GameObject multyExitBtn;


    [Header("���� �г�")]
    public GameObject stopPanel;
    [Header("�̾��ϱ� ��ư")]
    public GameObject btnContinue;
    [Header("�������� �ؽ�Ʈ")]
    public Text hiddenText;

    [Header("�ٸ� ������Ʈ ����")]
    public AudioManager audioManager;

    [Header("��ǥ ������")]
    public int Sscore;
    public int Ascore;
    public int Bscore;
    public int Cscore;
    public int Dscore;
    public int Escore;
    [Header("���� ���� ����")]
    public float curScore;

    //ä�ÿ� ���ð� 0.05��
    WaitForSeconds wait0_05 = new WaitForSeconds(0.05f);
    //ä�ÿ� ���ð� 2��
    WaitForSeconds wait2_00 = new WaitForSeconds(2);

    

    public enum RankType
    {
        S, A, B, C, D, E
    }
    [Header("���� ���� ��ũ")]
    public RankType rankType;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        battleUI.SetActive(false);
    }

    public void typingControl(string _str) 
    {
        if (descCor != null) 
        {
            StopCoroutine(descCor);
        }
        descCor = StartCoroutine(typingRoutine(_str));
    }

    #region ��ȭ
    public IEnumerator typingRoutine(string str)
    {
        //ȿ����
        audioManager.PlaySfx(AudioManager.Sfx.RankUp);

        bigDescText.text = "";
        yield return null;

        int typingLength = str.GetTypingLength();

        for (int index = 0; index <= typingLength; index++)
        {
            bigDescText.text = str.Typing(index);
            yield return wait0_05;
        }
        yield return wait2_00;
        for (int index = typingLength; index >= 0; index--)
        {
            bigDescText.text = str.Typing(index);
            yield return wait0_05;
        }
        yield return null;
        bigDescText.text = "";

    }
    #endregion

    //���� ��ư
    public void btnStop()
    {
        //���� ȿ����
        audioManager.PlaySfx(AudioManager.Sfx.Paper);

        bigHealthBarParticle.SetActive(false);

        stopPanel.SetActive(true);

        Time.timeScale = 0;
    }

    public void realBtnContinue()//�̾��ϱ�
    {
        //���� ȿ����
        audioManager.PlaySfx(AudioManager.Sfx.Paper);

        stopPanel.SetActive(false);
        Time.timeScale = 1;

        //��ƼŬ �̹���(���ϸ� ������)
        Invoke("activateParticle", 0.1f);
    }

    void activateParticle() 
    {
        bigHealthBarParticle.SetActive(true);
    }


    public void btnRetry()//��õ� ��ư
    {
        //�̾��ϱ� ��ư �ٽ� ���̵���
        btnContinue.SetActive(true);

        //���� �г� �Ⱥ��̵���
        realBtnContinue();

        // ���� ���� �ٽ� �ε��մϴ�.
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    
    public void btnExit()//������ ��ư
    {
        //����â ����
        if (battleType == BattleType.Single)//�̱�
        {
            //�̾��ϱ� ��ư �ٽ� ���̵���
            btnContinue.SetActive(true);

            //���� �г� �Ⱥ��̵���
            realBtnContinue();

            SceneManager.LoadScene("SingleSelect");
            battleUI.SetActive(false);
        }
        else if (battleType == BattleType.Multy)//��Ƽ
        {
            //������
            gameManager.LeaveRoom();
        }
        else
        {
            Debug.Log("SceneError");
        }
        
    }

   

    void destroyBattleUiManager() { }
}
