using Assets.PixelHeroes.Scripts.ExampleScripts;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;

public class FollowSword : MonoBehaviourPunCallbacks
{
    [Serializable]//클래스 정보
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

    //클래스 정보 큐
    Queue<FollowSwordInfo> followSwordQueue = new Queue<FollowSwordInfo>();

    GameObject child;//자식 설정
    int followDelay = 5;//따라가는 지연시간
    //현재 칼의 정보
    private FollowSwordInfo childSwordInfo = new FollowSwordInfo(Vector3.zero, Quaternion.identity);
    [Header("플레이어 게임오브젝트")]
    public GameObject player;
    //플레이어의 함수
    private CharacterControls characterControls;
    //경로
    private TrailRenderer trailRenderer;

    [Header("칼과 플레이어 사이의 거리 계산")]
    public float swordDir;

    //있는 칼의 수
    int maxSwordIndex;
    //현재 칼이 몇번째인지
    int curSwordIndex;

    



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
        curSwordIndex = transform.GetSiblingIndex();//현재 자신이 몇 번째인지

        if (curSwordIndex != maxSwordIndex) //번호 설정
        {
            child = transform.parent.GetChild(curSwordIndex+1).gameObject;
        }

        if (curSwordIndex == 0)//리더 칼의 경우
        {
            characterControls = player.GetComponent<CharacterControls>();
            trailRenderer = GetComponentInChildren<TrailRenderer>();
        }
    }

    private void OnEnable()
    {
        if(maxSwordIndex != curSwordIndex)//클래스 객체 정보 초기화
            childSwordInfo = new FollowSwordInfo(Vector3.zero, Quaternion.identity);
    }

    

    void FixedUpdate()
    {
        if (maxSwordIndex == curSwordIndex)//맨 끝 칼은 수행 안함
            return;

        //큐에 정보 삽입
        followSwordQueue.Enqueue(new FollowSwordInfo(transform.position, transform.rotation));

        //가득 차면, 클래스 정보 뱉기
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
        if (other.transform.CompareTag("PlayerSwordArea") && curSwordIndex == 0 && photonView.IsMine)//리더 검이 충돌 했다면
        {
            //플레이어와 리더 칼의 거리 연산 초기화
            swordDir = 0;

            if (PhotonNetwork.InRoom)
                photonView.RPC("leaderSwordExit", RpcTarget.AllBuffered);
            else
                leaderSwordExit();
        }
    }

    #region 칼이 범위 밖으로 이탈 시
    [PunRPC]
    void leaderSwordExit()
    {
        //등의 칼 활성화
        characterControls.backSwords.SetActive(true);
        //트레일 렌더러 삭제
        trailRenderer.Clear();

        for (int i = 0; i <= maxSwordIndex; i++)
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
    #endregion
}
