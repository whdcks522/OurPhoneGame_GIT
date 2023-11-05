using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using AssetKits.ParticleImage;

public class BattleUIManager : MonoBehaviour
{
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
        //�̾��ϱ� ��ư �ٽ� ���̵���
        btnContinue.SetActive(true);

        //���� �г� �Ⱥ��̵���
        realBtnContinue();

        //����â ����
        if (battleType == BattleType.Single)
        {
            SceneManager.LoadScene("SingleSelect");
            battleType = BattleType.Rest;
        }
        else if (battleType == BattleType.Single)
        {
            // SceneManager.LoadScene("SingleSelect");
            battleType = BattleType.Rest;
        }
        else
        {
            Debug.Log("SceneError");
        }
        battleUI.SetActive(false);
    }
}
