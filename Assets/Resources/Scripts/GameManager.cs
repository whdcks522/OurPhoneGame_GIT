using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class GameManager : MonoBehaviour
{
    [Header("���̽�ƽ�� ������ �޾Ƶ���")]
    //�÷��̾� �̵�
    public VariableJoystick moveJoy;
    //�÷��̾� Į
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

    #region �̱���
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
