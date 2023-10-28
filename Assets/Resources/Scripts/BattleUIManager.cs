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

    [Header("���� UI ����")]
    public GameObject battleUI;
    //�÷��̾� �̵�
    public VariableJoystick moveJoy;
    //�÷��̾� Į
    public VariableJoystick swordJoy;
    public Image bigHealthBar;
    [Header("�ٸ� ������Ʈ ����")]
    public AudioManager audioManager;

    private void Awake()
    {
        audioManager = GetComponent<AudioManager>();
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        moveJoy = transform.GetChild(0).GetComponent<VariableJoystick>();
        swordJoy = transform.GetChild(1).GetComponent<VariableJoystick>();
    }

    public void loadSingleSel()
    {
        SceneManager.LoadScene("SingleSelect");
    }
}
