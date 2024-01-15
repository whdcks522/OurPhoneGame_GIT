using Assets.PixelHeroes.Scripts.ExampleScripts;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class DogManager : MonoBehaviour
{
    [Header("최대 생성 시간")]
    public float maxTime;
    //현재 생성 시간
    float curTime = 0f;//------------------

    [Header("적 발생 지점")]
    public Transform dogPointsParent;
    GameObject[] dogPoints;

    [Header("강화 별 발생 지점")]
    public Transform powerUpPointsParent;
    GameObject[] powerUpPoints;
    int powerUpCurIndex = 0;
    public int powerUpMaxIndex = 0;

    [Header("씬의 레벨")]
    public int scenelevel;

    [Header("적 주기 배열")]
    public float[] dogSpeedArr;
    //재생산 추가 속도
    float curDogSpeed;

    [Header("플레이어 스크립트")]
    public CharacterControls characterControls;
    public GameObject player;

    BattleUIManager battleUIManager;
    public GameManager gameManager;
    public BulletShotter bulletShotter;


    private void Start()
    {
        battleUIManager = BattleUIManager.Instance;
        curTime = maxTime - 2;

        //적 발생지 배열 적용
        dogPoints = new GameObject[dogPointsParent.childCount];
        for (int i = 0; i < dogPointsParent.childCount; i++)
        {
            dogPoints[i] = dogPointsParent.GetChild(i).gameObject;
        }
        //강화 별 발생지 배열 적용
        powerUpPoints = new GameObject[powerUpPointsParent.childCount];
        for (int i = 0; i < powerUpPointsParent.childCount; i++)
        {
            powerUpPoints[i] = powerUpPointsParent.GetChild(i).gameObject;
        }
        //게임 시작 시,  재생산 속도 목록
        curDogSpeed = dogSpeedArr[5];
        //플레이어 체력 감소 비율 0으로 설정
        characterControls.healthMinus = curDogSpeed;
        characterControls.scorePlus = 1;

        //배경음 재생
        battleUIManager.audioManager.PlayBgm(AudioManager.BgmSingle.Fly);

    }


    private void Update()
    {
        curTime += Time.deltaTime * curDogSpeed;

        if (curTime > maxTime)
        {
            switch (battleUIManager.rankType)
            {
                case BattleUIManager.RankType.S:
                    curDogSpeed = dogSpeedArr[0];
                    break;
                case BattleUIManager.RankType.A:
                    curDogSpeed = dogSpeedArr[1];
                    break;
                case BattleUIManager.RankType.B:
                    curDogSpeed = dogSpeedArr[2];
                    break;
                case BattleUIManager.RankType.C:
                    curDogSpeed = dogSpeedArr[3];
                    break;
                case BattleUIManager.RankType.D:
                    curDogSpeed = dogSpeedArr[4];
                    break;
                case BattleUIManager.RankType.E:
                    curDogSpeed = dogSpeedArr[5];
                    break;
            }

            //시간 초기화
            curTime = 0f;
            //curTime = -100f;

            //생성 효과음
            battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Summon);

            if (++powerUpCurIndex >= powerUpMaxIndex) 
            {
                powerUpCurIndex = 0;
                int powerUpPos = Random.Range(0, powerUpMaxIndex);
                bulletShotter.sortShot(BulletShotter.BulletShotType.Direction, Bullet.BulletEffectType.PowerUp, 
                    powerUpPoints[powerUpPos], player, 0);
            }

            //적 생성
            if (scenelevel == 0)
            {
                //장소와 타입 설정
                int posR = Random.Range(0, dogPoints.Length);

                //int typeR = 2;
                int typeR = Random.Range(0, 4);
                string type = "";

                switch (typeR) 
                {
                    case 0:
                        type = "Enemy_Goblin";
                        break;
                    case 1:
                    case 2:
                        type = "Enemy_Orc";
                        break;
                    case 3:
                        type = "Enemy_Lizard";
                        break;
                }
                createEnemy(posR, type);
            }
        }
    }

    //적이 생성 될 위치
    Vector3 createVec;

    #region 적 생성
    void createEnemy(int pos, string type) 
    {
        GameObject enemyGameObject = gameManager.CreateObj(type, GameManager.PoolTypes.EnemyType);
        Enemy enemyComponent = enemyGameObject.GetComponent<Enemy>();
        enemyComponent.gameManager = gameManager;

        //적 위치 조정
        float x = Random.Range(-1f, 1f);
        float y = Random.Range(-1f, 1f);
        createVec = new Vector3(x, y, 0);
        enemyGameObject.transform.position = dogPoints[pos].transform.position + createVec;

        //적 활성화
        enemyComponent.activateRPC();
    }
    #endregion
}
