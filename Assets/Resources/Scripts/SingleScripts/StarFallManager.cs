using Photon.Pun.Demo.Procedural;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class StarFallManager : MonoBehaviour
{
    

    [Header("운석 발생 지점")]
    public Transform[] starFallPoints;
    //운석 발생 지점의 갯수
    int starFallPointsSize;

    //최대 발사 시간
    public float maxTime;
    //현재 발사 시간
    float curTime;
    //최근에 발사한 위치
    int recentPos;

    [Header("파워업 유성 랭크 별 주기")]
    public int[] PowerUpIndexArr;
    //최대 파워업 유성 
    int maxPowerUpIndex;
    //현재 파워업 유성 
    int curPowerUpIndex = 0;

    [Header("추가 생산 속도 배열")]
    public float[] createBulletArr;
    //랭크에 따른 추가 생산
    float rankSpeed = 1;

    [Header("씬의 레벨")]
    public int scenelevel;

    BattleUIManager battleUIManager;
    [Header("게임 매니저")]
    public GameManager gameManager;
    GameObject player;

    private void Awake()
    {
        battleUIManager = BattleUIManager.Instance;
        
        player = gameManager.player;
        starFallPointsSize = starFallPoints.Length;

        //플레이어 점수 증가 비율 설정
        gameManager.characterControl.scorePlus = 1;

        //배경음 재생
        battleUIManager.audioManager.PlayBgm(AudioManager.BgmSingle.StarFall);
    }

    private void Update()
    {
        curTime += Time.deltaTime * rankSpeed;


        if (curTime > maxTime)
        {
            switch (battleUIManager.rankType)
            {
                case BattleUIManager.RankType.S:
                    rankSpeed = createBulletArr[0];
                    gameManager.characterControl.healthMinus = createBulletArr[0];

                    maxPowerUpIndex = PowerUpIndexArr[0];
                    break;
                case BattleUIManager.RankType.A:
                    rankSpeed = createBulletArr[1];
                    gameManager.characterControl.healthMinus = createBulletArr[1];

                    maxPowerUpIndex = PowerUpIndexArr[1];
                    break;
                case BattleUIManager.RankType.B:
                    rankSpeed = createBulletArr[2];
                    gameManager.characterControl.healthMinus = createBulletArr[2];

                    maxPowerUpIndex = PowerUpIndexArr[2];
                    break;
                case BattleUIManager.RankType.C:
                    rankSpeed = createBulletArr[3];
                    gameManager.characterControl.healthMinus = createBulletArr[3];

                    maxPowerUpIndex = PowerUpIndexArr[3];
                    break;
                case BattleUIManager.RankType.D:
                    rankSpeed = createBulletArr[4];
                    gameManager.characterControl.healthMinus = createBulletArr[4];

                    maxPowerUpIndex = PowerUpIndexArr[4];
                    break;
                case BattleUIManager.RankType.E:
                    rankSpeed = createBulletArr[5];
                    gameManager.characterControl.healthMinus = createBulletArr[5];

                    maxPowerUpIndex = PowerUpIndexArr[5];
                    break;
            }

            //시간 초기화
            curTime = 0f;

            //생성 효과음
            battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Summon);

            //사출 위치 정하기
            int ranPos = Random.Range(0, starFallPointsSize);
            //같은 곳 연속으로 안되도록 설정
            if (ranPos == recentPos) 
                ranPos = (ranPos + 1) / starFallPointsSize;
            recentPos = ranPos;


            GameObject bullet = null;
            //운석 오브젝트 생성
            if (curPowerUpIndex >= maxPowerUpIndex)//강화 운석
            {
                bullet = gameManager.CreateObj("PowerUpBullet", GameManager.PoolTypes.BulletType);
                curPowerUpIndex = 0;
            }
            else //기본 운석
            {
                bullet = gameManager.CreateObj("NormalBullet", GameManager.PoolTypes.BulletType);
                curPowerUpIndex++;
            }

            //컴포넌트 정의
            Rigidbody2D bulletRigid = bullet.GetComponent<Rigidbody2D>();
            Bullet bulletComponent = bullet.GetComponent<Bullet>();

            //위치 조정
            bullet.transform.parent = this.transform;
            bullet.transform.position = starFallPoints[ranPos].position;

            //운석 활성화
            bulletComponent.bulletOnRPC();

            //방향 조정
            Vector2 bulletVec = (player.transform.position - bullet.transform.position).normalized;

            //약간의 궤도 변화
            bulletVec += 0.1f * Random.insideUnitCircle;//반지름이 1인 원 안에서 랜덤 벡터2 좌표 찍어줌
            bulletVec = bulletVec.normalized;

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
