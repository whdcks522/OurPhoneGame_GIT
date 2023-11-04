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

    [Header("파워업 유성 랭크 별 주기")]
    public int[] PowerUpIndexArr;
    //최대 파워업 유성 
    int maxPowerUpIndex;
    //현재 파워업 유성 
    int curPowerUpIndex = 0;

    [Header("추가 생산 속도 배열")]
    public float[] createBlockArr;
    //랭크에 따른 추가 생산
    float rankSpeed = 1;


    
    [Header("씬의 레벨")]
    public int scenelevel;

    GameManager gameManager;
    GameObject player;
    
    BattleUIManager battleUIManager;

    private void Awake()
    {
        battleUIManager = BattleUIManager.Instance;
        gameManager = battleUIManager.gameManager;
        
        player = gameManager.player;
        curTime = maxTime;
    }

    private void Update()
    {
        switch (battleUIManager.rankType) 
        {
            case BattleUIManager.RankType.S:
                rankSpeed = createBlockArr[0];
                maxPowerUpIndex = PowerUpIndexArr[0];
                break;
            case BattleUIManager.RankType.A:
                rankSpeed = createBlockArr[1];
                maxPowerUpIndex = PowerUpIndexArr[1];
                break;
            case BattleUIManager.RankType.B:
                rankSpeed = createBlockArr[2];
                maxPowerUpIndex = PowerUpIndexArr[2];
                break;
            case BattleUIManager.RankType.C:
                rankSpeed = createBlockArr[3];
                maxPowerUpIndex = PowerUpIndexArr[3];
                break;
            case BattleUIManager.RankType.D:
                rankSpeed = createBlockArr[4];
                maxPowerUpIndex = PowerUpIndexArr[4];
                break;
            case BattleUIManager.RankType.E:
                rankSpeed = createBlockArr[5];
                maxPowerUpIndex = PowerUpIndexArr[5];
                break;
        }

        curTime += Time.deltaTime * rankSpeed;

        if (curTime > maxTime)
        {
            //사출 위치 정하기
            //int ranPos = Random.Range(0, blockPoints.Length);
            startIndex++;
            if (startIndex >= blockPoints.Length) 
                startIndex = 0;
            //시간 초기화
            curTime = 0f;
            //강화 투사체와 강화 벽 생성
            curPowerUpIndex++;
            //생성 효과음
            battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Summon);
            //생성할 블록의 이름
            string blockName = "NormalBlock";

            if (curPowerUpIndex >= maxPowerUpIndex)
            {
                //변수 초기화
                curPowerUpIndex = 0;
                //생성할 블록 변경
                blockName = "HardBlock";
                

                //강화 투사체 생성
                GameObject bullet = gameManager.CreateObj("GreenStarBullet", GameManager.PoolTypes.BulletType);

                //컴포넌트 정의
                Rigidbody bulletRigid = bullet.GetComponent<Rigidbody>();
                Bullet bulletComponent = bullet.GetComponent<Bullet>();

                //위치 조정
                bullet.transform.parent = this.transform;
                bullet.transform.position = blockPoints[startIndex].position;

                //운석 활성화
                bulletComponent.bulletOnRPC();

                //속도 조정
                Vector2 bulletVec = (player.transform.position - bullet.transform.position).normalized;

                //최종 속도 조정
                bulletRigid.velocity = bulletVec * bulletComponent.bulletSpeed;

                //회전 조정
                bullet.transform.rotation = Quaternion.identity;
                float zValue = Mathf.Atan2(bulletRigid.velocity.x, bulletRigid.velocity.y) * 180 / Mathf.PI;
                Vector3 rotVec = Vector3.back * zValue + Vector3.back * 45.0f;
                bullet.transform.Rotate(rotVec);
                
            }

            

            GameObject block = gameManager.CreateObj(blockName, GameManager.PoolTypes.BlockType);
            Block blockComponent = block.GetComponent<Block>();

            //블록 부모 조정
            block.transform.parent = this.transform;
            blockComponent.createPos = blockPoints[startIndex].position;

            //블록 활성화
            blockComponent.blockOnRPC();
        }
    }
}
