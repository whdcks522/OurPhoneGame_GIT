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
    float maxTime = 0.25f;
    //현재 발사 시간
    float curTime = 0f;

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

    [Header("블록 배열")]
    public Block[] blockArr;


    private void Start()
    {
        battleUIManager = BattleUIManager.Instance;
        gameManager = battleUIManager.gameManager;

        //배경음 재생
        battleUIManager.audioManager.PlayBgm(AudioManager.BgmSingle.Train);

        //플레이어 체력 감소 비율 0으로 설정
        characterControls.healthMinus = 0;
        //플레이어 체력 설정
        characterControls.curHealth = 20;

        characterControls.changeStateRPC(CharacterControls.PlayerStateType.LeftControl, false);
        characterControls.changeStateRPC(CharacterControls.PlayerStateType.IsCanJump, false);
        characterControls.changeStateRPC(CharacterControls.PlayerStateType.RightControl, false);
        //칼과 충돌 무시
        characterControls.changeStateRPC(CharacterControls.PlayerStateType.SwordCollision, false);
        characterControls.backSwords.SetActive(true);

        battleUIManager.typingControl("훈련장에 어서오세요!");
        StartCoroutine(moveBoxRoutine());

        //블록 피해주기
        foreach(Block block in blockArr)
        {
            block.healthControl(35);
        }
    }



    private void Update()
    {
        curTime += Time.deltaTime;
        

        if (curTime > maxTime)
        {
            //파워 업 스타

            //시간 초기화
            curTime = 0f;
            GameObject bullet  = gameManager.CreateObj("PowerUpBullet", GameManager.PoolTypes.BulletType);

            //컴포넌트 정의
            Rigidbody2D bulletRigid = bullet.GetComponent<Rigidbody2D>();
            Bullet bulletComponent = bullet.GetComponent<Bullet>();

            //운석 발사
            bullet.transform.parent = this.transform;
            bullet.transform.position = trainPoints[1].position;

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

            //파워 업 스타

            //시간 초기화
            GameObject bullet2 = gameManager.CreateObj("NormalBullet", GameManager.PoolTypes.BulletType);

            //컴포넌트 정의
            Rigidbody2D bullet2Rigid = bullet2.GetComponent<Rigidbody2D>();
            Bullet bullet2Component = bullet2.GetComponent<Bullet>();

            //운석 발사
            bullet2.transform.parent = this.transform;
            bullet2.transform.position = trainPoints[2].position;

            //운석 활성화
            bullet2Component.bulletOnRPC();

            //속도 조정
            Vector2 bullet2Vec = Vector2.down;

            //최종 속도 조정
            bullet2Rigid.velocity = bullet2Vec * bullet2Component.bulletSpeed;

            //회전 조정
            bullet2.transform.rotation = Quaternion.identity;
            float zValue2 = Mathf.Atan2(bullet2Rigid.velocity.x, bullet2Rigid.velocity.y) * 180 / Mathf.PI;
            Vector3 rotVec2 = Vector3.back * zValue2 + Vector3.back * 45.0f;
            bullet2.transform.Rotate(rotVec2);
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
