using Photon.Pun.Demo.Asteroids;
using Photon.Pun.Demo.PunBasics;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockCrashManager : MonoBehaviour
{
    [Header("발생 지점")]
    public Transform[] blockPoints;
    //발사 인덱스
    int startIndex = -1;
    [Header("최대 발사 시간")]
    public float maxTime;
    //현재 발사 시간
    float curTime;

    [Header("파워업 블록 최대 주기")]
    public int[] PowerUpIndexArr;
    //최대 파워업 유성 
    int maxPowerUpIndex;
    //현재 파워업 유성 
    int curPowerUpIndex = 0;

    [Header("회복 블록 최대 주기")]
    public int maxCureIndex;
    //현재 회복 블록 
    int curCureIndex = 0;

    [Header("추가 생산 속도 배열")]
    public float[] createBlockArr;
    //랭크에 따른 추가 생산
    float rankSpeed = 1;


    
    [Header("씬의 레벨")]
    public int scenelevel;
    [Header("게임 매니저")]
    public GameManager gameManager;
    
    BattleUIManager battleUIManager;

    private void Awake()
    {
        battleUIManager = BattleUIManager.Instance;
        curTime = maxTime;

        //플레이어 점수 증가 비율 설정
        gameManager.characterControl.scorePlus = 1;
    }

    private void Update()
    {
        curTime += Time.deltaTime * rankSpeed;

        if (curTime > maxTime)
        {
            switch (battleUIManager.rankType)
            {
                case BattleUIManager.RankType.S:
                    rankSpeed = createBlockArr[0];
                    gameManager.characterControl.healthMinus = createBlockArr[0];

                    maxPowerUpIndex = PowerUpIndexArr[0];
                    break;
                case BattleUIManager.RankType.A:
                    rankSpeed = createBlockArr[1];
                    gameManager.characterControl.healthMinus = createBlockArr[1];

                    maxPowerUpIndex = PowerUpIndexArr[1];
                    break;
                case BattleUIManager.RankType.B:
                    rankSpeed = createBlockArr[2];
                    gameManager.characterControl.healthMinus = createBlockArr[2];

                    maxPowerUpIndex = PowerUpIndexArr[2];
                    break;
                case BattleUIManager.RankType.C:
                    rankSpeed = createBlockArr[3];
                    gameManager.characterControl.healthMinus = createBlockArr[3];

                    maxPowerUpIndex = PowerUpIndexArr[3];
                    break;
                case BattleUIManager.RankType.D:
                    rankSpeed = createBlockArr[4];
                    gameManager.characterControl.healthMinus = createBlockArr[4];

                    maxPowerUpIndex = PowerUpIndexArr[4];
                    break;
                case BattleUIManager.RankType.E:
                    rankSpeed = createBlockArr[5];
                    gameManager.characterControl.healthMinus = createBlockArr[5];

                    maxPowerUpIndex = PowerUpIndexArr[5];
                    break;
            }

            startIndex++;
            if (startIndex >= blockPoints.Length) 
                startIndex = 0;
            //시간 초기화
            curTime = 0f;
            //강화 투사체와 강화 벽 생성
            curPowerUpIndex++;
            curCureIndex++;
            //생성 효과음
            battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Summon);
            //생성할 블록의 이름
            string blockName = "NormalBlock";
            if (curCureIndex >= maxCureIndex)
            {
                //변수 초기화
                curCureIndex = 0;
                //생성할 블록 변경
                blockName = "CureBlock";
            }
            else if (curPowerUpIndex >= maxPowerUpIndex)
            {
                //변수 초기화
                curPowerUpIndex = 0;
                //생성할 블록 변경
                blockName = "PowerUpBlock";
            }


            GameObject block = gameManager.CreateObj(blockName, GameManager.PoolTypes.BlockType);
            Block blockComponent = block.GetComponent<Block>();


            //블록 부모 조정
            block.transform.parent = this.transform;
            blockComponent.blockPoints = blockPoints;

            //블록 활성화
            blockComponent.blockOnRPC();
        }
    }
}
