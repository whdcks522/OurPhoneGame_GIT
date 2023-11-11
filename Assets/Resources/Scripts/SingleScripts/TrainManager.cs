using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Assets.PixelHeroes.Scripts.ExampleScripts;
using KoreanTyper;
using Photon.Pun.Demo.PunBasics;
using Photon.Realtime;

public class TrainManager : MonoBehaviour
{
    //최대 발사 시간
    float maxTime = 1f;
    //현재 발사 시간
    float curTime = 0f;
    //발사 위치 지수
    int count = 1;

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
    [Header("게임 매니저")]
    public GameManager gameManager;


    private void Awake()
    {
        
    }

    private void Start()
    {
        battleUIManager = BattleUIManager.Instance;
        gameManager = battleUIManager.gameManager;

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

    private void Update()
    {
        curTime += Time.deltaTime;
        

        if (curTime > maxTime)
        {
            //시간 초기화
            curTime = 0f;
            GameObject bullet  = gameManager.CreateObj("GreenStarBullet", GameManager.PoolTypes.BulletType);

            //컴포넌트 정의
            Rigidbody2D bulletRigid = bullet.GetComponent<Rigidbody2D>();
            Bullet bulletComponent = bullet.GetComponent<Bullet>();

            //위치 조정
            count++;
            if (count == 3) count = 1;

            //운석 발싸

            bullet.transform.parent = this.transform;
            bullet.transform.position = trainPoints[count].position;

            //운석 활성화
            bulletComponent.bulletOnRPC();

            //속도 조정
            Vector2 bulletVec = Vector2.down;

            //최종 속도 조정
            bulletRigid.velocity = bulletVec * bulletComponent.bulletSpeed;

            //회전 조정
            bullet.transform.rotation = Quaternion.identity;
            float zValue = Mathf.Atan2(bulletRigid.velocity.x, bulletRigid.velocity.y) * 180 / Mathf.PI;
            Vector3 rotVec = Vector3.back * zValue + Vector3.back * 45.0f;
            bullet.transform.Rotate(rotVec);
        }
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
