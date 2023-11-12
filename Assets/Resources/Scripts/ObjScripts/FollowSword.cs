using Assets.PixelHeroes.Scripts.ExampleScripts;
using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;


public class FollowSword : MonoBehaviourPunCallbacks
{
    [Serializable]//클래스 정보
    public class FollowSwordInfo
    {
        public Vector2 swordPos { get; set; }//위치
        public Vector2 swordVec { get; set; }//속도

        public FollowSwordInfo(Vector2 tmpPos, Vector2 tmpVec)
        {
            swordPos = tmpPos;
            swordVec = tmpVec;
        }
    }

    //클래스 정보 큐
    Queue<FollowSwordInfo> swordQueue = new Queue<FollowSwordInfo>();

    public FollowSword upperSword;
    public GameObject lowerSword;//자식 설정
    int followDelay = 5;//따라가는 지연시간

    //현재 칼의 정보
    private FollowSwordInfo swordQueueInfo = new FollowSwordInfo(Vector2.zero, Vector2.zero);

    [Header("플레이어 게임오브젝트(비활성화 시, 죽으면 오류)")]
    public GameObject player;
    //플레이어의 함수
    public CharacterControls characterControls;

    [Header("칼과 플레이어 사이의 거리 계산")]
    public float swordDir;


    [Header("리더의 속도")]
    public Vector2 saveSwordVec = Vector3.zero;


    //경로
    public TrailRenderer trailRenderer;
    //사용 가능한 전체 칼의 수
    public int maxSwordIndex;
    //현재 칼이 몇번째인지
    public int curSwordIndex;
    //칼의 데미지
    int swordDamage = 5;

    //배틀 매니저
    public BattleUIManager battleUIManager;
    Rigidbody2D rigid;


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
        rigid = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        trailRenderer.Clear();

        swordQueueInfo = new FollowSwordInfo(transform.position, Vector2.zero);//transform이 나을듯?
    }

    void FixedUpdate()
    {
        if (curSwordIndex == 1)
        {
            // 두 위치 간의 거리를 계산합니다.
            Vector2 swordPos = transform.position;
            Vector2 playerPos = player.transform.position;

            swordDir = Vector3.Distance(swordPos, playerPos) / 400;  
        }


        //큐에 정보 삽입
        swordQueue.Enqueue(new FollowSwordInfo(transform.position, saveSwordVec));

        //가득 차면, 클래스 정보 뱉기
        if (swordQueue.Count > followDelay)
        {
            swordQueueInfo = swordQueue.Dequeue();

            //꺼져 있다면 켜줌 
            if (curSwordIndex < characterControls.curSwordCount) //현재 칼의 번호 x가 캐릭터의 칼 수보다 
            {
                if (!lowerSword.activeSelf) 
                {
                    lowerSword.SetActive(true);
                    lowerSword.transform.position = swordQueueInfo.swordPos;

                    lowerSword.GetComponent<FollowSword>().trailRenderer.Clear();
                } 
            }
        }

        //밟을 수 있을 때, 플레이어 점프 초기화도 필요함
        rigid.angularVelocity = 0f;
        rigid.velocity = saveSwordVec * 10;
      
        //회전 조작
        transform.rotation = Quaternion.identity;
        float zValue = Mathf.Atan2(rigid.velocity.x, rigid.velocity.y) * 180 / Mathf.PI;
        Vector3 rotVec = Vector3.back * zValue + Vector3.back * 45;
        transform.Rotate(rotVec);

        if (curSwordIndex < characterControls.curSwordCount)//맨 끝 칼은 수행 안함
        {
            lowerSword.GetComponent<FollowSword>().saveSwordVec = swordQueueInfo.swordVec;
        }
    }

    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (PhotonNetwork.InRoom)//멀티 중이라면
        {
            if (other.transform.CompareTag("PlayerSwordArea") && photonView.IsMine)//리더 검이 충돌 했다면
            {
                PhotonView tmpPhotonView = other.gameObject.GetComponent<PhotonView>();
                if (tmpPhotonView.IsMine) //자신의 영역에서 벗어 났을 때만
                {
                    //플레이어와 리더 칼의 거리 연산 초기화
                    swordDir = 0;
                    photonView.RPC("leaderSwordExitRPC", RpcTarget.AllBuffered, 1);
                }
            }
        }
        else //1인이라면
        {
            if (other.transform.CompareTag("PlayerSwordArea"))//리더 검이 충돌 했다면  && curSwordIndex == 1
            {
                //플레이어와 리더 칼의 거리 연산 초기화
                swordDir = 0;
                leaderSwordExitRPC(1);
            }
        }
    }

    #region 칼이 범위 밖으로 이탈 시
    [PunRPC]
    public void leaderSwordExitRPC(int level)
    {
        //0: 그냥 수납
        //1: 무기만 폭파(칼 범위 밖으로 나간 경우)
        //2: 본인과 무기 폭파

        //1: 무기만 폭파(칼 범위 밖으로 나간 경우)
        if (curSwordIndex == 1) //리더 칼이 나간 경우
        {
            if (level == 1 && characterControls.curSwordCount > 1)
            {
                if (PhotonNetwork.InRoom)
                {
                    if (photonView.IsMine)
                        createBomb(transform.position);
                }
                else if (!PhotonNetwork.InRoom)
                    createBomb(transform.position);
            }
            //2: 본인과 무기 폭파
            else if (level == 2)
            {
                if (PhotonNetwork.InRoom)
                {
                    if (photonView.IsMine)
                    {
                        //플레이어 폭파
                        createBomb(player.transform.position);
                        //무기 폭파
                        if (gameObject.activeSelf)
                            createBomb(transform.position);
                    }
                }
                else if (!PhotonNetwork.InRoom)
                {
                    //플레이어 폭파
                    createBomb(player.transform.position);
                    //무기 폭파
                    if (gameObject.activeSelf)
                        createBomb(transform.position);
                }
            }

            //등의 칼 활성화
            characterControls.backSwords.SetActive(true);
            //다시 칼 비활성화
            for (int i = 0; i <= maxSwordIndex - 1; i++)
            {
                GameObject tmpSword = characterControls.swordParent.transform.GetChild(i).gameObject;
                FollowSword tmpSwordComponent = tmpSword.GetComponent<FollowSword>();


                tmpSword.transform.position = player.transform.position;
                //칼 활성화
                tmpSword.SetActive(false);

                if (tmpSwordComponent != null)
                {
                    tmpSwordComponent.swordQueue.Clear();//큐 초기화
                }
            }
        }
        //---------- 죽을 때 오류
        else if (curSwordIndex != 1)
        {
            transform.position = upperSword.swordQueueInfo.swordPos;
        }
    }
    #endregion

    #region 폭탄 생성
    void createBomb(Vector3 bombPos) 
    {
       
        //폭탄 생성
        GameObject bomb = null;

        if (PhotonNetwork.InRoom)
        {
            if (photonView.IsMine)
            {
                //무기 수 1 감소
                characterControls.photonView.RPC("swordCountRPC", RpcTarget.AllBuffered, -1);

                bomb = battleUIManager.gameManager.CreateObj("Broken Phantasm", GameManager.PoolTypes.BombType);

                //여기 없는 경우 오류 날 수도 있음
                Bomb bombComponent = bomb.GetComponent<Bomb>();
                //폭탄 해당 위치에 활성화
                bombComponent.bombOnRPC(bombPos);
                bombComponent.photonView.RPC("bombOnRPC", RpcTarget.AllBuffered, bombPos);
            }
        }
        else if (!PhotonNetwork.InRoom)
        {
            //무기 수 1 감소
            characterControls.swordCountRPC(-1);

            bomb = battleUIManager.gameManager.CreateObj("Broken Phantasm", GameManager.PoolTypes.BombType);

            //여기 없는 경우 오류 날 수도 있음
            Bomb bombComponent = bomb.GetComponent<Bomb>();
            //폭탄 해당 위치에 활성화
            bombComponent.bombOnRPC(bombPos);
        }  
    }
    #endregion

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.CompareTag("EnemyBullet"))
        {
            Bullet bullet = other.GetComponent<Bullet>();

            if (bullet.bulletEffectType == Bullet.BulletEffectType.UnBreakable)
                return;

            //총알 파괴와 회복
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
                        characterControls.photonView.RPC("swordCountRPC", RpcTarget.AllBuffered, 1);
                    }
                }
                else if (!PhotonNetwork.InRoom)
                {
                    //무기 수 1 증가
                    characterControls.swordCountRPC(1);
                }

            }
            else if (bullet.bulletEffectType == Bullet.BulletEffectType.Normal)
            {
                //일반 효과음
                battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Heal);
            }
        }
       
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.transform.CompareTag("Block"))
        {
            Block block = other.gameObject.GetComponent<Block>();
            if (PhotonNetwork.InRoom)
            {
                if (photonView.IsMine)
                {
                    block.photonView.RPC("healthControl", RpcTarget.AllBuffered, Time.deltaTime * swordDamage);
                }
            }
            else if (!PhotonNetwork.InRoom)
            {
                block.healthControl(Time.deltaTime * swordDamage);
            }
        }
    }
}
