using Assets.PixelHeroes.Scripts.ExampleScripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

public class FollowSword : MonoBehaviour
{
    public GameObject child;//자식 설정
    private int followDelay = 5;//따라가는 지연시간
    //현재 부모 칼의 정보
    private FollowSwordInfo childSwordInfo = new FollowSwordInfo(Vector3.zero, Quaternion.identity);
    public GameObject player;
    private CharacterControls characterControls;
    private TrailRenderer trailRenderer;
    public float swordDir;
    public int maxSwordIndex;
    public int curSwordIndex;//

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

    private void Awake()
    {
        maxSwordIndex = transform.parent.childCount - 1;
        curSwordIndex = transform.GetSiblingIndex();

        if (curSwordIndex != maxSwordIndex) 
        {
            child = transform.parent.GetChild(curSwordIndex+1).gameObject;
        }

        if (curSwordIndex == 0)
        {
            characterControls = player.GetComponent<CharacterControls>();
            trailRenderer = GetComponentInChildren<TrailRenderer>();
        }
    }

    private void OnEnable()
    {
        if(maxSwordIndex != curSwordIndex)
        childSwordInfo = new FollowSwordInfo(Vector3.zero, Quaternion.identity);
    }



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



    void FixedUpdate()
    {
        if (maxSwordIndex == curSwordIndex)
            return;

        //큐에 정보 삽입
        followSwordQueue.Enqueue(new FollowSwordInfo(transform.position, transform.rotation));

        //뱉기
        if (followSwordQueue.Count > followDelay)
        {
            childSwordInfo = followSwordQueue.Dequeue();

            if (!child.activeSelf) //꺼져 있다면 켜줌
            { 
                child.SetActive(true);
                child.transform.position = player.transform.position + Vector3.up * 0.5f;
            }
        }

        //최종 이동
        child.transform.rotation = childSwordInfo.swordRot;
        child.transform.position = childSwordInfo.swordVec;

        if (curSwordIndex == 0) 
        {
            Vector3 swordPos = transform.position;
            Vector3 playerPos = player.transform.position;

            // 두 위치 간의 거리를 계산합니다.
            swordDir = Vector3.Distance(swordPos, playerPos)/ 500;
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag("PlayerSwordArea") && curSwordIndex == 0)//리더 검이 충돌 했다면
        {
            //등의 칼 활성화
            characterControls.backSwords.SetActive(true);
            //트레일 렌더러 삭제
            trailRenderer.Clear();
            //거리 연산 초기화
            swordDir = 0;
            int tmpChildSize = characterControls.swordParent.transform.childCount;

            for (int i = 0; i < tmpChildSize; i++)
            {
                GameObject tmpSword = characterControls.swordParent.transform.GetChild(i).gameObject;
                FollowSword tmpSwordComponent = tmpSword.GetComponent<FollowSword>();

                tmpSword.transform.position = player.transform.position;
                tmpSword.SetActive(false);


                if (tmpSwordComponent != null)
                {
                    tmpSwordComponent.followSwordQueue.Clear();
                }
            }
        }
    }

    private void OnTriggerExit(Collider2D collision)
    {
        if (collision.transform.CompareTag("PlayerSwordArea") && curSwordIndex == 0)//리더 검이 충돌 했다면
        {
            //등의 칼 활성화
            characterControls.backSwords.SetActive(true);
            //트레일 렌더러 삭제
            trailRenderer.Clear();
            //거리 연산 초기화
            swordDir = 0;
            int tmpChildSize = characterControls.swordParent.transform.childCount;

            for (int i = 0; i < tmpChildSize; i++) 
            {
                GameObject tmpSword = characterControls.swordParent.transform.GetChild(i).gameObject;
                FollowSword tmpSwordComponent = tmpSword.GetComponent<FollowSword>();

                tmpSword.transform.position = player.transform.position;
                tmpSword.SetActive(false);

                
                if (tmpSwordComponent != null)
                {
                    tmpSwordComponent.followSwordQueue.Clear();
                }    
            }
        }    
    }
}
