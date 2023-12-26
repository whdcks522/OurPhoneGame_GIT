using Assets.PixelHeroes.Scripts.ExampleScripts;
using Photon.Pun;
using Photon.Pun.Demo.Asteroids;
using Photon.Pun.Demo.PunBasics;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviourPunCallbacks
{
    [Header("총알의 최대 수명")]
    public float maxTime;
    float curTime = 0f;

    [Header("총알의 속도")]
    public float bulletSpeed;

    [Header("총알의 피해량")]
    public int bulletDamage;

    [Header("파괴 시, 총알의 회복량")]
    public int bulletHeal;

    [Header("총알의 목표")]
    public Transform bulletTarget;

    [Header("칼이 여러개여서 동시 충돌 할 때가 있는 듯(삭제 하지 말 것)")]
    public bool isAlreadyHit = false;

    BattleUIManager battleUIManager;
    GameManager gameManager;
    Rigidbody2D rigid;

    
    public enum BulletEffectType
    {
        Normal, PowerUp, UnBreakable, Chase
    }
    [Header("총알의 특수효과")]
    public BulletEffectType bulletEffectType;

    [Header("시작 시, 플래시 이펙트 사용 여부")]
    public bool isFlash;
    public string flashStr;
    [Header("종료 시, 히트 이펙트 사용 여부")]
    public bool isHit;
    public string hitStr;
    [Header("플레이어 스크립트")]
    public CharacterControls characterControls;

    private void Awake()
    {
        battleUIManager = BattleUIManager.Instance;
        gameManager = battleUIManager.gameManager;
        characterControls = gameManager.characterControl;
        rigid = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        curTime += Time.deltaTime;
        if (curTime > maxTime)
        {
            if (PhotonNetwork.InRoom) 
            {
                if (photonView.IsMine)
                    photonView.RPC("bulletOffRPC", RpcTarget.AllBuffered);
            }
            else if(!PhotonNetwork.InRoom)
            {
                bulletOffRPC(0);
            }
        }

        if (bulletEffectType == BulletEffectType.Chase) 
        {
            //속도 조정
            Vector2 bulletVec = (bulletTarget.position - transform.position).normalized;

            //최종 속도 조정
            rigid.velocity = bulletVec * bulletSpeed;
        }
    }

    #region 총알 활성 동기화
    [PunRPC]
    public void bulletOnRPC()
    {
        //게임오브젝트 활성화
        gameObject.SetActive(true);
        //회전 초기화
        transform.rotation = Quaternion.identity;
        //연속 피격 처리 될 때가 있음
        isAlreadyHit = false;

        if (isFlash)
        {
            if (PhotonNetwork.InRoom)
            {
                if (photonView.IsMine) 
                {
                    GameObject flash = gameManager.CreateObj(flashStr, GameManager.PoolTypes.BulletType);
                    Bullet flashBullet = flash.GetComponent<Bullet>();
                    flash.transform.position = transform.position;
                    flash.transform.forward = gameObject.transform.forward;
                    flash.transform.parent = transform.parent;
                    flashBullet.photonView.RPC("effectOnRPC", RpcTarget.AllBuffered);

                    //회전 조정
                    flash.transform.rotation = transform.rotation;
                } 
            }
            else if (!PhotonNetwork.InRoom)
            {
                GameObject flash = gameManager.CreateObj(flashStr, GameManager.PoolTypes.BulletType);
                Bullet flashBullet = flash.GetComponent<Bullet>();
                flash.transform.position = transform.position;
                flash.transform.forward = gameObject.transform.forward;
                flash.transform.parent = transform.parent;
                flashBullet.bulletOnRPC();

                //회전 조정
                flash.transform.rotation = transform.rotation;
            }
        }
    }
    #endregion

    #region 총알 비활성 동기화
    [PunRPC]
    public void bulletOffRPC(int type)//칼에 맞았거나, 자연소멸했거나
    {
        //시간 동기화
        curTime = 0f;
        //게임오브젝트 비활성화
        gameObject.SetActive(false);
        //-1: 플레이어가 총알에 맞은 경우
        // 0: 자연 소멸한 경우
        //+1: 플레이어가 칼로 총알을 파괴한 경우

        if (isHit)//종료 이펙트 출력
        {
            GameObject hit = gameManager.CreateObj(hitStr, GameManager.PoolTypes.BulletType);
            Effect hitEffect = hit.GetComponent<Effect>();

            if (PhotonNetwork.InRoom)
            {
                if (PhotonNetwork.IsMasterClient)
                    hitEffect.photonView.RPC("effectOnRPC", RpcTarget.AllBuffered, transform.position);
            }
            else if (!PhotonNetwork.InRoom)
            {
                hitEffect.effectOnRPC(transform.position);
            }
        }

        //1: 칼, 폭탄으로 파괴해서 회복
        if (type == 1)
        {
            characterControls.healControlRPC(bulletHeal);//회복 효과음은 회복 코드에서 관리

            if (bulletEffectType == Bullet.BulletEffectType.Normal)//일반 총알의 경우
            {
                //일반 효과음
                battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Heal);
            }
            else if (bulletEffectType == Bullet.BulletEffectType.PowerUp)//파워업의 경우(효과음은 안에서 재생됨)
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
        }
        else if (type == -1)//총알이 플레이어를 맞춘 경우
        {
            if (!isAlreadyHit)
            {
                if (PhotonNetwork.InRoom)
                {
                    if (photonView.IsMine)
                    {
                        //플레이어 체력 감소
                        characterControls.photonView.RPC("damageControlRPC", RpcTarget.All, bulletDamage, true);
                    }
                }
                else if (!PhotonNetwork.InRoom)
                {
                    //피격 처리
                    characterControls.damageControlRPC(bulletDamage, true);
                }
            }
        }
        //재차 피격 안일어나도록
        isAlreadyHit = true;
    }
    #endregion

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.CompareTag("playerSword")|| other.transform.CompareTag("Bomb"))//폭탄이나 칼과 충돌한 경우
        {
            if (bulletEffectType == Bullet.BulletEffectType.UnBreakable)
                return;

            //총알 파괴와 회복
            if (PhotonNetwork.InRoom)
            {
                if (photonView.IsMine)
                {
                    if (!isAlreadyHit) 
                    {
                        //총알 파괴
                        photonView.RPC("bulletOffRPC", RpcTarget.All, 1);
                    }
                    
                }
            }
            else if (!PhotonNetwork.InRoom)
            {
                if (!isAlreadyHit) 
                {
                    //총알 파괴
                    bulletOffRPC(1);
                }   
            }
        }
        else if (other.transform.CompareTag("Player"))//총알이 플레이어를 맞춤
        {
            //총알 파괴와 회복
            if (PhotonNetwork.InRoom)
            {
                if (photonView.IsMine)
                {
                    //총알 파괴
                    photonView.RPC("bulletOffRPC", RpcTarget.All, -1);
                }
            }
            else if (!PhotonNetwork.InRoom)
            {
                //총알 파괴
                bulletOffRPC(-1);
            }
        }
        else if (other.transform.CompareTag("Outline")) //맵 밖으로 나가지면 종료
        {
            if (PhotonNetwork.InRoom)
            {
                if (photonView.IsMine)
                    //총알 파괴
                    photonView.RPC("bulletOffRPC", RpcTarget.AllBuffered);

            }
            else if (!PhotonNetwork.InRoom)
            {
                bulletOffRPC(0);
            }
        }
        
    }
}
