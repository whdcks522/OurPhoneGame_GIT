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
    [Header("���� ��Ʋ Ÿ��")]
    public BattleType battleType;


    [Header("���� �̱� �� Ÿ��")]//������ JSON ���忡 ����
    public SingleInfoData.SingleInfoType singleInfoType = SingleInfoData.SingleInfoType.Train;
    [Header("���� �̱� �� ��ȣ")]
    public int singleIndex = 0;

    [Header("���� UI ����")]
    public GameObject battleUI;

    [Header("�̵� ���̽�ƽ")]
    public VariableJoystick moveJoy;
    [Header("Į ���̽�ƽ")]
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
    string saveDescText = "";
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
    public JSONManager jsonManager;

    [Header("��ǥ ������")]
    public int Sscore;
    public int Ascore;
    public int Bscore;
    public int Cscore;
    public int Dscore;
    public int Escore;
    [Header("���� ���� ����")]
    public float curScore;
    [Header("���� ���� ����")]
    public int curLevel;
    //ä�ÿ� ���ð� 0.025��
    WaitForSeconds wait0_025 = new WaitForSeconds(0.025f);
    //ä�ÿ� ���ð� 3��
    WaitForSeconds wait3_00 = new WaitForSeconds(3);

    public enum RankType
    {
        S, A, B, C, D, E
    }
    [Header("���� ���� ��ũ")]//�̱� ������ ���̵� ������ ����
    public RankType rankType;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        battleUI.SetActive(false);
    }

    #region ��ȭ
    public void typingControl(string _str) 
    {
        if (saveDescText != _str)
        {
            if (descCor != null)
                StopCoroutine(descCor);
            descCor = StartCoroutine(typingRoutine(_str));
        }
    }

    
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

                //ȿ����
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
        //��ȭ �ڷ�ƾ ����
        typingControl("");

        //������ ����
        jsonManager.SaveData(singleInfoType, curLevel, Mathf.FloorToInt(curScore));

        //�̾��ϱ� ��ư �ٽ� ���̵���
        btnContinue.SetActive(true);

        //���� �г� �Ⱥ��̵���
        realBtnContinue();

        // ���� ���� �ٽ� �ε��մϴ�.
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    
    public void btnExit()//������ ��ư
    {
        
        //��ȭ �ڷ�ƾ ����
        typingControl("");

        //����â ����
        if (battleType == BattleType.Single)//�̱�
        {
            //������ ����
            jsonManager.SaveData(singleInfoType, curLevel, Mathf.FloorToInt(curScore));

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
            gameManager.allLeaveRoomStart();
        }
        else
        {
            Debug.Log("SceneError");
        }
        
    }
    public void JoySizeControl(bool isLarge)//���̹�ư ��Ȱ��ȭ ���
    {
        if (isLarge)
        {
            moveJoy.GetComponent<Image>().rectTransform.localScale = Vector3.one;
            swordJoy.GetComponent<Image>().rectTransform.localScale = Vector3.one;
        }
        else if (!isLarge)
        {
            moveJoy.GetComponent<Image>().rectTransform.localScale = Vector3.zero;
            swordJoy.GetComponent<Image>().rectTransform.localScale = Vector3.zero;
        }
    }
}
