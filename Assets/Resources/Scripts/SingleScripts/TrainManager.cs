using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Assets.PixelHeroes.Scripts.ExampleScripts;
using KoreanTyper;

public class TrainManager : MonoBehaviour
{
    [Header("발생 지점")]
    public Transform[] trainPoints;

    [Header("씬의 레벨")]
    public int scenelevel;

    [Header("이동을 위한 박스")]
    public GameObject moveBox;

    [Header("플레이어 스크립트")]
    public CharacterControls characterControls;

    //채팅용 대기시간 3초
    WaitForSeconds wait3_00 = new WaitForSeconds(3);

    BattleUIManager battleUIManager;

    //레이어를 위함
    int playerLayer, playerSwordLayer;

    private void Awake()
    {
        battleUIManager = BattleUIManager.Instance;

        
    }

    private void Start()
    {
        //플레이어 체력 감소 비율 0으로 설정
        characterControls.healthMinus = 0;
        //플레이어 체력 설정
        characterControls.curHealth = 20;

        characterControls.changeStateRPC(CharacterControls.PlayerStateType.LeftControl, false);
        characterControls.changeStateRPC(CharacterControls.PlayerStateType.IsCanJump, false);
        characterControls.changeStateRPC(CharacterControls.PlayerStateType.RightControl, false);
        //칼과 충돌 무시
        characterControls.changeStateRPC(CharacterControls.PlayerStateType.SwordCollision, false);

        battleUIManager.typingControl("훈련소에 어서오세요!");
        StartCoroutine(moveBoxRoutine());
    }

    #region 이동 상자 생성
    IEnumerator moveBoxRoutine()
    {
        yield return wait3_00;
        yield return wait3_00;

        moveBox.transform.position = trainPoints[0].position;
        
    }
    #endregion
}
