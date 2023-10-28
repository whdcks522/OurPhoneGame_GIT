using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class GameManager : MonoBehaviour
{
    [Header("조이스틱의 정보를 받아들임")]
    //플레이어 이동
    public VariableJoystick moveJoy;
    //플레이어 칼
    public VariableJoystick swordJoy;
    public GameObject player;
    public CinemachineVirtualCamera cinemachineVirtualCamera;
    //moveJoy = joySticks.transform.GetChild(0).GetComponent<VariableJoystick>();
    //swordJoy = joySticks.transform.GetChild(1).GetComponent<VariableJoystick>();

    private void Awake()
    {
        cinemachineVirtualCamera.Follow = player.transform;
        cinemachineVirtualCamera.LookAt = player.transform;
    }

    #region 싱글턴
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<GameManager>();
            return instance;
        }
    }
    #endregion
}
