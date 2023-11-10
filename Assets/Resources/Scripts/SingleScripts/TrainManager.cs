using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Assets.PixelHeroes.Scripts.ExampleScripts;
using KoreanTyper;

public class TrainManager : MonoBehaviour
{
    [Header("�߻� ����")]
    public Transform[] trainPoints;

    [Header("���� ����")]
    public int scenelevel;

    [Header("�̵��� ���� �ڽ�")]
    public GameObject moveBox;

    [Header("�÷��̾� ��ũ��Ʈ")]
    public CharacterControls characterControls;

    //ä�ÿ� ���ð� 3��
    WaitForSeconds wait3_00 = new WaitForSeconds(3);

    BattleUIManager battleUIManager;

    //���̾ ����
    int playerLayer, playerSwordLayer;

    private void Awake()
    {
        battleUIManager = BattleUIManager.Instance;

        
    }

    private void Start()
    {
        //�÷��̾� ü�� ���� ���� 0���� ����
        characterControls.healthMinus = 0;
        //�÷��̾� ü�� ����
        characterControls.curHealth = 20;

        characterControls.changeStateRPC(CharacterControls.PlayerStateType.LeftControl, false);
        characterControls.changeStateRPC(CharacterControls.PlayerStateType.IsCanJump, false);
        characterControls.changeStateRPC(CharacterControls.PlayerStateType.RightControl, false);
        //Į�� �浹 ����
        characterControls.changeStateRPC(CharacterControls.PlayerStateType.SwordCollision, false);

        battleUIManager.typingControl("�Ʒüҿ� �������!");
        StartCoroutine(moveBoxRoutine());
    }

    #region �̵� ���� ����
    IEnumerator moveBoxRoutine()
    {
        yield return wait3_00;
        yield return wait3_00;

        moveBox.transform.position = trainPoints[0].position;
        
    }
    #endregion
}
