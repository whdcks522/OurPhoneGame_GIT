using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FollowSword : MonoBehaviour
{
    public Transform parent;//부모 설정
    public int followDelay;//따라가는 지연시간
    public SpriteRenderer spriteRenderer;
    //현재 부모 칼의 정보
    private FollowSwordInfo curSwordInfo = new FollowSwordInfo(Vector3.zero, Quaternion.identity);

    #region 적 정보 클래스 공백
    /*
    [Serializable]//필요하더라
    public class EnemySpawnInfo
    {
        public EnemyType enemyType;
        public int generateIndex;
    }

    [Serializable]
    public class EnemySpawnInfoArray
    {
        public EnemySpawnInfo[] enemySpawnInfo;
    }

    public EnemySpawnInfoArray[] enemySpawnInfoArray;//챕터 전체에서 소환할 적의 목록
    private List<EnemySpawnInfo> enemySpawnList;//이번 스테이지에서 소환할 적의 목록
    */
    #endregion 


    [Serializable]
    public class FollowSwordInfo
    {
        public Vector3 swordVec { get; set; }
        public Quaternion swordRot { get; set; }

        public FollowSwordInfo(Vector3 tmpVec, Quaternion tmpRot)
        {
            swordVec = tmpVec;
            swordRot = tmpRot;
        }
    }
    public Queue<FollowSwordInfo> followSwordQueue = new Queue<FollowSwordInfo>();

   
    void Update()
    {
        //플레이어가 멈추면 정지
        //if (!parentPos.Contains(parent.position))

        followSwordQueue.Enqueue(new FollowSwordInfo(parent.position, parent.rotation));

        //뱉기
        if (followSwordQueue.Count > followDelay) 
        {
            curSwordInfo = followSwordQueue.Dequeue();
            spriteRenderer.color = Color.white;
        }
        //다 안찼다면
        else if (followSwordQueue.Count < followDelay)
            curSwordInfo.swordVec = parent.position;//현재 리더 칼의 위치로 설정


        transform.position = curSwordInfo.swordVec;
        transform.rotation = curSwordInfo.swordRot;
    }
}
