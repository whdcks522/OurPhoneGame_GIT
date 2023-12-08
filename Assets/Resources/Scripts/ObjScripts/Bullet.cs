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

    [Header("총알의 목표")]
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

    private void Awake()
    {
        battleUIManager = BattleUIManager.Instance;
        gameManager = battleUIManager.gameManager;
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
                bulletOffRPC();
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
            if (PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient)
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
    public void bulletOffRPC()
    {
        //재차 피격 안일어나도록
        isAlreadyHit = true;
        //시간 동기화
        curTime = 0f;
        //게임오브젝트 비활성화
        gameObject.SetActive(false);
        

        if (isHit)
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
    }
    #endregion

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.CompareTag("Outline")) //맵 밖으로 나가지면 종료
        {
            if (PhotonNetwork.InRoom) 
            {
                if(photonView.IsMine)
                    //총알 파괴
                    photonView.RPC("bulletOffRPC", RpcTarget.AllBuffered);

            }
            else if (!PhotonNetwork.InRoom)
            {
                bulletOffRPC();
            }
        }
    }
}
