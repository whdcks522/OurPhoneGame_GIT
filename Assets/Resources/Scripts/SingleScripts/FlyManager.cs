using Assets.PixelHeroes.Scripts.ExampleScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyManager : MonoBehaviour
{
    //최대 발사 시간
    float maxTime = 2f;
    //현재 발사 시간
    float curTime = 0f;


    [Header("바람 발생 지점")]
    public Transform[] windPoints;

    [Header("씬의 레벨")]
    public int scenelevel;

    [Header("바람 최대 주기")]
    public int[] windIndexArr;
    //최대 파워업 유성 
    //int maxPowerUpIndex;
    //현재 파워업 유성 
    //int curPowerUpIndex = 0;


    [Header("플레이어 스크립트")]
    public CharacterControls characterControls;

    BattleUIManager battleUIManager;
    [Header("게임 매니저")]
    public GameManager gameManager;

    private void Start()
    {
        battleUIManager = BattleUIManager.Instance;

        //플레이어 체력 감소 비율 0으로 설정
        characterControls.healthMinus = 1;
        characterControls.curHealth = 20;
    }


    private void Update()
    {
        curTime += Time.deltaTime;


        if (curTime > maxTime)
        {
            //시간 초기화
            curTime = 0f;
            GameObject bullet = gameManager.CreateObj("PowerUpBullet", GameManager.PoolTypes.BulletType);

            //컴포넌트 정의
            Rigidbody2D bulletRigid = bullet.GetComponent<Rigidbody2D>();
            Bullet bulletComponent = bullet.GetComponent<Bullet>();


            //운석 발싸

            bullet.transform.parent = this.transform;
            bullet.transform.position = windPoints[0].position;

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

}
