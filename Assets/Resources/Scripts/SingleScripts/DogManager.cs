using Assets.PixelHeroes.Scripts.ExampleScripts;
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
    float curTime = 0;

    [Header("적 발생 지점")]
    public Transform dogPointsParent;
    GameObject[] dogPoints;

    [Header("씬의 레벨")]
    public int scenelevel;

    [Header("적 주기 배열")]
    public float[] dogSpeedArr;
    //재생산 추가 속도
    float curDogSpeed;

    [Header("플레이어 스크립트")]
    public CharacterControls characterControls;

    BattleUIManager battleUIManager;
    [Header("게임 매니저")]
    public GameManager gameManager;

    private void Start()
    {
        battleUIManager = BattleUIManager.Instance;

        //플레이어 체력 감소 비율 0으로 설정
        characterControls.healthMinus = 0;

        characterControls.scorePlus = 1;

        //바람 발생지 배열 적용
        dogPoints = new GameObject[dogPointsParent.childCount];
        for (int i = 0; i < dogPointsParent.childCount; i++)
        {
            dogPoints[i] = dogPointsParent.GetChild(i).gameObject;
        }
        //게임 시작 시,  재생산 속도 목록
        curDogSpeed = dogSpeedArr[5];

        //배경음 재생
        battleUIManager.audioManager.PlayBgm(AudioManager.BgmSingle.Fly);
    }


    private void Update()
    {
        return;
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

            //생성 효과음
            battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Summon);

            //총알 생성
            //BulletShotter bulletShotter = windPoints[ran].GetComponent<BulletShotter>();
            //bulletShotter.sortShot(BulletShotter.BulletShotType.Direction, Bullet.BulletEffectType.PowerUp,
            //        windPoints[ran], player, 1);
        }
    }
}
