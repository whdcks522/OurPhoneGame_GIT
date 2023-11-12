using Assets.PixelHeroes.Scripts.ExampleScripts;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FlyManager : MonoBehaviour
{
    //최대 발사 시간
    float maxTime = 3f;
    //현재 발사 시간
    float curTime = 0;

    //최근에 발사한 위치
    int recentPos;

    [Header("바람 발생 지점")]
    public Transform[] windPoints;

    [Header("씬의 레벨")]
    public int scenelevel;

    [Header("바람 최대 주기")]
    public float[] windSpeedArr;
    float curWindSpeed;

    //최대 파워업 유성 
    public int maxPowerUpIndex;
    //현재 파워업 유성 
    int curPowerUpIndex = 0;


    [Header("플레이어 스크립트")]
    public CharacterControls characterControls;

    BattleUIManager battleUIManager;
    [Header("게임 매니저")]
    public GameManager gameManager;
    public GameObject player;

    private void Start()
    {
        battleUIManager = BattleUIManager.Instance;

        //플레이어 체력 감소 비율 0으로 설정
        characterControls.healthMinus = 0;
        characterControls.curHealth = 100;
        characterControls.scorePlus = 1;

        curWindSpeed = windSpeedArr[5];
    }


    private void Update()
    {
        curTime += Time.deltaTime * curWindSpeed;

        if (curTime > maxTime)
        {
            switch (battleUIManager.rankType)
            {
                case BattleUIManager.RankType.S:
                    curWindSpeed = windSpeedArr[0];
                    break;
                case BattleUIManager.RankType.A:
                    curWindSpeed = windSpeedArr[1];
                    break;
                case BattleUIManager.RankType.B:
                    curWindSpeed = windSpeedArr[2];
                    break;
                case BattleUIManager.RankType.C:
                    curWindSpeed = windSpeedArr[3];
                    break;

                case BattleUIManager.RankType.D:
                    curWindSpeed = windSpeedArr[4];
                    break;
                case BattleUIManager.RankType.E:
                    curWindSpeed = windSpeedArr[5];
                    break;

            }

            //시간 초기화
            curTime = 0f;
            curPowerUpIndex++;

            //생성 효과음
            battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Summon);

            //사출 위치 정하기
            int ranPos = Random.Range(0, windPoints.Length);
            //같은 곳 연속으로 안되도록 설정
            if (ranPos == recentPos)
                ranPos = (ranPos + 1) / windPoints.Length;
            recentPos = ranPos;


            GameObject wind = gameManager.CreateObj("NormalWind", GameManager.PoolTypes.WindType);

            //컴포넌트 정의
            Wind windComponent = wind.GetComponent<Wind>();

            wind.transform.parent = this.transform;
            wind.transform.position = windPoints[ranPos].position;

            //운석 활성화
            windComponent.windOnRPC();

            //회전 조정
            int isTrue = Random.Range(0, 2);

            if (isTrue == 0) 
            {
                Vector3 direction = (player.transform.position - wind.transform.position).normalized;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                wind.transform.eulerAngles = new Vector3(0, 0, angle + Random.Range(-15, 16));
            }
            else if (isTrue == 1)
            {
                Vector3 direction = (wind.transform.position - player.transform.position).normalized;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                wind.transform.eulerAngles = new Vector3(0, 0, angle + Random.Range(-15, 16));
            }



            if (curPowerUpIndex >= maxPowerUpIndex) 
            {
                curPowerUpIndex = 0;

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
}
