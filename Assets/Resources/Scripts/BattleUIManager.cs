using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
    [Header("ü�� �� �����̴�")]
    public Slider bigHealthBar;
    [Header("��ũ �ؽ�Ʈ")]
    public Text bigRankText;
    [Header("����Ʈ �ؽ�Ʈ")]
    public Text bigPointText;
    [Header("���� �г�")]
    public GameObject stopPanel;

    [Header("�ٸ� ������Ʈ ����")]
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

    //���� ��ư
    public void btnStop()
    {
        stopPanel.SetActive(true);
        Time.timeScale = 0;
    }

    //Ȱ��ȭ ��ư
    public void btnActivate()
    {
        stopPanel.SetActive(false);
        Time.timeScale = 1;
    }

    //��õ� ��ư
    public void btnRetry()
    {
        stopPanel.SetActive(false);
        Time.timeScale = 1;

        // ���� ���� �ٽ� �ε��մϴ�.
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    //������ ��ư
    public void btnExit()
    {
        stopPanel.SetActive(false);
        Time.timeScale = 1;

        //����â ����
        if (battleType == BattleType.Single)
            SceneManager.LoadScene("SingleSelect");
        else 
        {
            Debug.Log("SceneError");
        }
        battleUI.SetActive(false);
    }
}
