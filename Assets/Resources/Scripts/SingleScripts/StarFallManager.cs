using Photon.Pun.Demo.Procedural;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using static BulletShotter;

public class StarFallManager : MonoBehaviour
{
    

    [Header("운석 발생 지점")]
    public GameObject[] starFallPoints;
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

            BulletShotter bulletShotter = starFallPoints[ranPos].GetComponent<BulletShotter>();

            if (scenelevel == 0) //강화 운석 1개 발사
            {
                if (curPowerUpIndex >= maxPowerUpIndex)//강화 운석
                {
                    bulletShotter.sortShot(BulletShotter.BulletShotType.Direction, Bullet.BulletEffectType.PowerUp,
                        starFallPoints[ranPos], player, 0);
                    curPowerUpIndex = 0;
                }
                else //기본 운석 2개 발사
                {
                    bulletShotter.sortShot(BulletShotter.BulletShotType.Direction, Bullet.BulletEffectType.Normal,
                        starFallPoints[ranPos], player, 1);
                    bulletShotter.sortShot(BulletShotter.BulletShotType.Direction, Bullet.BulletEffectType.Normal,
                        starFallPoints[ranPos], player, 1);

                    curPowerUpIndex++;
                }
            }
            else if (scenelevel == 1)
            {

                if (curPowerUpIndex >= maxPowerUpIndex)//강화 운석을 쏴야하는 경우
                {
                    curPowerUpIndex = 0;

                    bulletShotter.sortShot(BulletShotter.BulletShotType.Direction, Bullet.BulletEffectType.PowerUp,
                        starFallPoints[ranPos], player, 0);  
                }
                else //그냥 사격의 경우
                {
                    //투사체 종류 설정
                    Bullet.BulletEffectType bulletEffectType = Bullet.BulletEffectType.Normal;//기본 투사체
                    if (curPowerUpIndex == 0)
                        bulletEffectType = Bullet.BulletEffectType.UnBreakable;//0일때는, 깨지지 않는 투사체

                    //공격 패턴 설정
                    int ranPattern = Random.Range(0, 3);

                    switch (ranPattern)
                    {
                        case 0://일괄 비틀어서 발사
                            for (int i = 0; i < starFallPoints.Length; i++) 
                            {
                                BulletShotter allBulletShotter = starFallPoints[i].GetComponent<BulletShotter>();

                                allBulletShotter.sortShot(BulletShotter.BulletShotType.Direction, bulletEffectType,
                                starFallPoints[i], player, 1);
                            }
                            break;
                        case 1://작게 산탄
                            bulletShotter.sortShot(BulletShotter.BulletShotType.Big, bulletEffectType,
                            starFallPoints[ranPos], player, 0);
                            break;
                        case 2://흩뿌리기
                            bulletShotter.sortShot(BulletShotter.BulletShotType.Scatter, bulletEffectType,
                            starFallPoints[ranPos], player, 1);
                            break;
                    }
                    //여기에 안하면 붉은 별 패턴 안나옴
                    curPowerUpIndex++;
                }
            }//Scene == 1
            else if (scenelevel == 2) 
            {
                if (true)//강화 운석 curPowerUpIndex >= maxPowerUpIndex)
                {
                    bulletShotter.sortShot(BulletShotter.BulletShotType.Direction, Bullet.BulletEffectType.PowerUp,
                        starFallPoints[4], starFallPoints[12], 0);

                    bulletShotter.sortShot(BulletShotter.BulletShotType.Direction, Bullet.BulletEffectType.PowerUp,
                        starFallPoints[4], starFallPoints[12], 0);

                    curPowerUpIndex = 0;
                }
                else //기본 운석 2개 발사
                {
                    bulletShotter.sortShot(BulletShotter.BulletShotType.Direction, Bullet.BulletEffectType.Normal,
                        starFallPoints[4], starFallPoints[12], 0);

                    curPowerUpIndex++;
                }
            }//Scene == 2
        }
    }
}
