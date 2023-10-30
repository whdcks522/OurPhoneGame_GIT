using Assets.PixelHeroes.Scripts.ExampleScripts;
using Photon.Pun;
using Photon.Pun.Demo.Asteroids;
using Photon.Pun.Demo.PunBasics;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
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

    public GameObject child;//자식 설정
    int followDelay = 5;//따라가는 지연시간
    //현재 칼의 정보
    private FollowSwordInfo childSwordInfo = new FollowSwordInfo(Vector3.zero, Quaternion.identity);
    //[Header("플레이어 게임오브젝트")]
    GameObject player;
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
    //배틀 매니저
    BattleUIManager battleUIManager;


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
        battleUIManager = BattleUIManager.Instance;
        player = transform.root.gameObject;
        characterControls = player.GetComponent<CharacterControls>();

        maxSwordIndex = transform.parent.childCount - 1;
        curSwordIndex = transform.GetSiblingIndex();//현재 자신이 몇 번째인지

        trailRenderer = GetComponentInChildren<TrailRenderer>();
        
    }

    private void OnEnable()
    {
        if(maxSwordIndex != curSwordIndex)//클래스 객체 정보 초기화
            childSwordInfo = new FollowSwordInfo(Vector3.zero, Quaternion.identity);
    }

    void FixedUpdate()
    {
        //큐에 정보 삽입
        followSwordQueue.Enqueue(new FollowSwordInfo(transform.position, transform.rotation));

        //가득 차면, 클래스 정보 뱉기
        if (followSwordQueue.Count > followDelay)
        {
            childSwordInfo = followSwordQueue.Dequeue();

            if (!child.activeSelf && curSwordIndex < characterControls.curSwordCount) //꺼져 있다면 켜줌 //child != null && 
            {
                
                child.SetActive(true);
                trailRenderer.Clear();
                //child.transform.position = player.transform.position + Vector3.up * 0.5f;
            }
        }

        if (curSwordIndex > characterControls.curSwordCount)//맨 끝 칼은 수행 안함
            return;


        //최종 이동
        child.transform.rotation = childSwordInfo.swordRot;
        child.transform.position = childSwordInfo.swordVec;

        //영역을 보여주기 위함
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
        if (PhotonNetwork.InRoom)//멀티 중이라면
        {
            if (other.transform.CompareTag("PlayerSwordArea") && curSwordIndex == 0 && photonView.IsMine)//리더 검이 충돌 했다면
            {
                PhotonView tmpPhotonView = other.gameObject.GetComponent<PhotonView>();
                if (tmpPhotonView.IsMine) //자신의 영역에서 벗어 났을 때만
                {
                    //플레이어와 리더 칼의 거리 연산 초기화
                    swordDir = 0;
                    photonView.RPC("leaderSwordExitRPC", RpcTarget.AllBuffered);
                }
            }
        }
        else //1인이라면
        {
            if (other.transform.CompareTag("PlayerSwordArea") && curSwordIndex == 0)//리더 검이 충돌 했다면
            {
                //플레이어와 리더 칼의 거리 연산 초기화
                swordDir = 0;
                leaderSwordExitRPC();
            }
        }
    }

    #region 칼이 범위 밖으로 이탈 시
    [PunRPC]
    void leaderSwordExitRPC()
    {
        //등의 칼 활성화
        characterControls.backSwords.SetActive(true);
        //무기 수 1 감소
        characterControls.swordCountRPC(false);
        //폭탄 생성
        GameObject bomb = null;

        if (PhotonNetwork.InRoom)
        {
            if (photonView.IsMine)
                bomb = battleUIManager.gameManager.CreateObj("Broken Phantasm", GameManager.PoolTypes.BombType);
        }
        else if (!PhotonNetwork.InRoom) 
        {
            bomb = battleUIManager.gameManager.CreateObj("Broken Phantasm", GameManager.PoolTypes.BombType);
        }
        //여기 없는 경우 오류 날 수도 있음
        Bomb bombComponent = bomb.GetComponent<Bomb>();
        
        //폭탄 위치 조정
        bomb.transform.parent = battleUIManager.gameManager.transform;
        bomb.transform.position = transform.position;
        //폭탄 활성화
        bombComponent.bombOnRPC();


        


        for (int i = 0; i <= maxSwordIndex; i++)
        {
            GameObject tmpSword = characterControls.swordParent.transform.GetChild(i).gameObject;
            FollowSword tmpSwordComponent = tmpSword.GetComponent<FollowSword>();

            tmpSword.SetActive(false);
            //트레일 렌더러 초기화
            trailRenderer.Clear();

            if (tmpSwordComponent != null)
            {
                tmpSwordComponent.followSwordQueue.Clear();//큐 초기화
            }
        }
    }
    #endregion

    

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("EnemyBullet"))
        {
            Bullet bullet = other.GetComponent<Bullet>();

            if (bullet.bulletEffectType == Bullet.BulletEffectType.UnBreakable)
                return;

            if (PhotonNetwork.InRoom)
            {
                if (photonView.IsMine)
                {
                    //총알 파괴
                    bullet.photonView.RPC("bulletOffRPC", RpcTarget.AllBuffered);
                    //회복
                    characterControls.photonView.RPC("healOffRPC", RpcTarget.AllBuffered, bullet.bulletHeal);
                }
            }
            else if (!PhotonNetwork.InRoom)
            {
                //총알 파괴
                bullet.bulletOffRPC();
                //회복
                characterControls.healControlRPC(bullet.bulletHeal);
            }

            if (bullet.bulletEffectType == Bullet.BulletEffectType.PowerUp)//파워업의 경우
            {
                if (PhotonNetwork.InRoom)
                {
                    if (photonView.IsMine)
                    {
                        //무기 수 1 증가
                        characterControls.photonView.RPC("swordCountRPC", RpcTarget.AllBuffered, true);
                    }
                }
                else if (!PhotonNetwork.InRoom) 
                {
                    //무기 수 1 증가
                    characterControls.swordCountRPC(true);
                }
                    
            }
            else if (bullet.bulletEffectType == Bullet.BulletEffectType.Normal)
            {
                //일반 효과음
                battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Heal);
            }  
        }

        
    }

}
