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

    [Header("전투 UI 정보")]
    public GameObject battleUI;
    //플레이어 이동
    public VariableJoystick moveJoy;
    //플레이어 칼
    public VariableJoystick swordJoy;
    public Image bigHealthBar;
    [Header("다른 컴포넌트 정보")]
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
