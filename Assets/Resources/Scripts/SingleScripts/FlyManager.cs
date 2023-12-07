using Assets.PixelHeroes.Scripts.ExampleScripts;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FlyManager : MonoBehaviour
{
    [Header("최대 발사 시간")]
    public float maxTime;
    //현재 발사 시간
    float curTime = 0;

    //최근에 발사한 위치
    int recentPos;

    [Header("바람 발생 지점")]
    public Transform windPointsParent;
    GameObject[] windPoints;

    [Header("씬의 레벨")]
    public int scenelevel;

    [Header("바람 최대 주기")]
    public float[] windSpeedArr;
    //재생산 주기
    float curWindSpeed;

    //최대 바람 
    public int maxWindIndex;
    //현재 바람 
    int curWindIndex = 0;


    [Header("플레이어 스크립트")]
    public CharacterControls characterControls;

    BattleUIManager battleUIManager;
    [Header("게임 매니저")]
    public GameManager gameManager;
    public GameObject player;



    [Header("배경과 카메라의 오차 줄이는 용(아래는 레벨 1부터)")]
    public float errorDir;
    [Header("경계를 비활성화하기 위함")]
    public GameObject outLines;
    [Header("움직이는 카메라를 위함")]
    public GameObject cmRange;
    [Header("움직이는 장미들을 위함")]
    public GameObject redRoses;
    [Header("움직이는 배경을 위함")]
    public GameObject backGround;


    private void Start()
    {
        battleUIManager = BattleUIManager.Instance;

        //플레이어 체력 감소 비율 0으로 설정
        characterControls.healthMinus = 0;
        characterControls.curHealth = 100;
        characterControls.scorePlus = 1;

        windPoints = new GameObject[windPointsParent.childCount];
        for (int i = 0; i < windPointsParent.childCount; i++) 
        {
            windPoints[i] = windPointsParent.GetChild(i).gameObject;
        }
        curWindSpeed = windSpeedArr[5];
        

        //배경음 재생
        battleUIManager.audioManager.PlayBgm(AudioManager.BgmSingle.Fly);

        if (scenelevel == 1)
        {
            outLines.SetActive(false);
        }
    }

    Vector2 maxHighVec = Vector2.zero;
    float maxHigh = 10f;//10
    private void Update()
    {
        curTime += Time.deltaTime * curWindSpeed;
        
        if (scenelevel == 1) 
        {
            //아무것도 안해도 증가하도록
            maxHigh += Time.deltaTime * curWindSpeed;
            //바람 위치 최대값 갱신
            maxHigh = Mathf.Max(maxHigh, player.transform.position.y);
            //바람 생성 위치 조절
            windPointsParent.transform.position = player.transform.position;

            //배경 위치 조정
            maxHighVec = new Vector2(0, maxHigh);
            cmRange.transform.position = maxHighVec;
            //배경 위치 조정
            maxHighVec = new Vector2(0, maxHigh + errorDir);
            backGround.transform.position = maxHighVec;
            //아래 아웃라인 조정
            redRoses.transform.position = maxHighVec;
            
        }

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
            curWindIndex++;

            //생성 효과음
            battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Summon);


            //사출 위치 정하기
            int ran = Random.Range(0, windPoints.Length);
            //같은 곳 연속으로 안되도록 설정
            if (ran == recentPos)
                ran = (ran + 1) / windPoints.Length;
            recentPos = ran;

            //총알 생성
            BulletShotter bulletShotter = windPoints[ran].GetComponent<BulletShotter>();
            bulletShotter.sortShot(BulletShotter.BulletShotType.Direction, Bullet.BulletEffectType.PowerUp,
                    windPoints[ran], player, 1);

            if (curWindIndex >= maxWindIndex)
            {
                curWindIndex = 0;
                createWind(); 
            }
        }
    }


    #region 바람 생성
    void createWind() 
    {
        //사출 위치 정하기
        int ranPos = Random.Range(0, windPoints.Length);
        //같은 곳 연속으로 안되도록 설정
        if (ranPos == recentPos)
            ranPos = (ranPos + 1) / windPoints.Length;
        recentPos = ranPos;


        GameObject wind = gameManager.CreateObj("NormalWind", GameManager.PoolTypes.WindType);

        //컴포넌트 정의
        Wind windComponent = wind.GetComponent<Wind>();
        wind.transform.position = windPoints[ranPos].transform.position;

        //운석 활성화
        windComponent.windOnRPC();

        //회전 조정
        int isTrue = Random.Range(0, 2);//방향을 섞기 위해서

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
    }
    #endregion
}
