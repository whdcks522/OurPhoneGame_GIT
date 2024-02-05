using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Assets.PixelHeroes.Scripts.ExampleScripts;
using KoreanTyper;
using Photon.Pun.Demo.PunBasics;
using Photon.Realtime;
using Unity.VisualScripting.Antlr3.Runtime;

public class TrainManager : MonoBehaviour
{
    //최대 발사 시간
    float maxTime = 0.5f;
    //현재 발사 시간
    float curTime = 0f;

    [Header("씬의 레벨")]
    public int scenelevel;

    [Header("플레이어 스크립트")]
    public CharacterControls characterControls;

    [Header("총알 시작점들(훈련 0)------------------------ ")]
    public GameObject[] trainPoints;

    [Header("시작 시, 이동을 위한 박스")]
    public GameObject moveBox;

    [Header("블록 배열")]
    public Block[] blockArr;

    [Header("매니저들")]
    BattleUIManager battleUIManager;
    public GameManager gameManager;


    private void Start()
    {
        battleUIManager = BattleUIManager.Instance;
        gameManager = battleUIManager.gameManager;

        //배경음 재생
        battleUIManager.audioManager.PlayBgm(AudioManager.BgmSingle.Train);
        //칼 갯수 조정
        characterControls.curSwordCount = 1;
        //무기 조작 금지
        characterControls.changeStateRPC(CharacterControls.PlayerStateType.RightControl, false);

        if (scenelevel == 0)
        {
            //플레이어 체력 설정
            characterControls.curHealth = 20;

            characterControls.changeStateRPC(CharacterControls.PlayerStateType.LeftControl, false);
            characterControls.changeStateRPC(CharacterControls.PlayerStateType.IsCanJump, false);
            //칼과 충돌 무시
            characterControls.changeStateRPC(CharacterControls.PlayerStateType.SwordCollision, false);
            characterControls.backSwords.SetActive(false);

            battleUIManager.typingControl("훈련장에 어서오세요!");
            StartCoroutine(moveBoxRoutine());

            //블록 깨부수기
            float dmg = blockArr[0].GetComponent<Block>().curHealth * 0.4f;
            foreach (Block block in blockArr)
            {
                block.healthControl(dmg);
            }
        }
    }

    private void OnDisable()
    {
        if (scenelevel == 1) 
        {
            battleUIManager.JoySizeControl(true);

            Debug.Log("크기 초기화");
        }
    }

    private void Update()
    {
        if (scenelevel == 0) 
        {
            curTime += Time.deltaTime;

            if (curTime > maxTime)
            {
                //시간 초기화
                curTime = 0f;

                BulletShotter powerUpBulletShotter = trainPoints[1].GetComponent<BulletShotter>();
                powerUpBulletShotter.sortShot(BulletShotter.BulletShotType.Direction, Bullet.BulletEffectType.PowerUp,
                        trainPoints[1], trainPoints[2], 0);

                BulletShotter normalBulletShotter = trainPoints[2].GetComponent<BulletShotter>();
                powerUpBulletShotter.sortShot(BulletShotter.BulletShotType.Direction, Bullet.BulletEffectType.Normal,
                        trainPoints[3], trainPoints[4], 0);
            }
        }
    }


    //채팅용 대기시간 3초
    WaitForSeconds wait3_00 = new WaitForSeconds(3);
    #region 이동 상자 생성
    IEnumerator moveBoxRoutine()
    {
        yield return wait3_00;
        yield return wait3_00;

        moveBox.transform.position = trainPoints[0].transform.position;
    }
    #endregion
}
