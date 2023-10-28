using Assets.PixelHeroes.Scripts.ExampleScripts;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviourPunCallbacks
{
    
    public float maxTime;
    float curTime = 0f;
    public float bulletSpeed;

    GameManager gameManager;
    Rigidbody rigidBody;
    ParticleSystem particleSystem;

    
    public enum BulletEffectType
    {
        Normal, UnBreakable, Chase
    }
    [Header("총알의 특수효과")]
    public BulletEffectType bulletEffectType;

    [Header("시작 시 플래시")]
    public bool isFlash;
    public string flashStr;
    [Header("종료 시 히트")]
    public bool isHit;
    public string hitStr;

    

    private void Awake()
    {
        gameManager = GameManager.Instance;
        rigidBody = GetComponent<Rigidbody>();
        particleSystem = GetComponent<ParticleSystem>();
    }

    void Start()
    {
        /*
        if (flash != null)
        {
            //사출 이펙트 생성
            var flashInstance = Instantiate(flash, transform.position, Quaternion.identity);
            //사출 이펙트 방향 설정
            flashInstance.transform.forward = gameObject.transform.forward;


            //사출 이펙트 삭제
            var flashPs = flashInstance.GetComponent<ParticleSystem>();
            if (flashPs != null)
            {
                Destroy(flashInstance, flashPs.main.duration);//게임 오브젝트도 삭제
            }
            else
            {
                var flashPsParts = flashInstance.transform.GetChild(0).GetComponent<ParticleSystem>();
                Destroy(flashInstance, flashPsParts.main.duration);//게임 오브젝트도 삭제
            }
        }
        */
        //Destroy(gameObject, 5);
    }

    private void Update()
    {
        curTime += Time.deltaTime;
        if (curTime > maxTime)
        {
            if (PhotonNetwork.InRoom && photonView.IsMine) 
            {
                photonView.RPC("bulletOffRPC", RpcTarget.AllBuffered);
            }
            else
            {
                bulletOffRPC();
            }
        }
    }
    /*
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("playerSword"))//칼과 충돌
        {
            if (PhotonNetwork.InRoom && photonView.IsMine)
            {
                photonView.RPC("bulletOffRPC", RpcTarget.AllBuffered);
            }
            else
            {
                bulletOffRPC();
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("player")) //플레이어와 충돌
        {
            if (PhotonNetwork.InRoom && photonView.IsMine)
            {
                photonView.RPC("bulletOffRPC", RpcTarget.AllBuffered);
            }
            else
            {
                bulletOffRPC();
            }
        }
    }
    */

    [PunRPC]
    public void bulletOnRPC()
    {
        //게임오브젝트 활성화
        gameObject.SetActive(true);

        // 파티클 시스템을 다시 시작
        particleSystem.Stop();
        particleSystem.Play();

        if (isFlash)
        {
            if (PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient)
            {
                GameObject flash = gameManager.CreateObj(flashStr, GameManager.PoolTypes.BulletType);
                Bullet flashBullet = flash.GetComponent<Bullet>();
                flash.transform.position = transform.position;
                flash.transform.forward = gameObject.transform.forward;
                flash.transform.parent = transform.parent;
                flashBullet.photonView.RPC("bulletOnRPC", RpcTarget.AllBuffered);
            }
            else
            {
                GameObject flash = gameManager.CreateObj(flashStr, GameManager.PoolTypes.BulletType);
                Bullet flashBullet = flash.GetComponent<Bullet>();
                flash.transform.position = transform.position;
                flash.transform.forward = gameObject.transform.forward;
                flashBullet.bulletOnRPC();
            }
        }
    }


    [PunRPC]
    public void bulletOffRPC()
    {
        //시간 동기화
        curTime = 0f;
        //게임오브젝트 비활성화
        gameObject.SetActive(false);

        if (isHit)
        {
            if (PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient)
            {
                GameObject hit = gameManager.CreateObj(hitStr, GameManager.PoolTypes.BulletType);
                Bullet hitBullet = hit.GetComponent<Bullet>();
                hit.transform.position = transform.position;
                hit.transform.forward = gameObject.transform.forward;
                hit.transform.parent = transform.parent;
                hitBullet.photonView.RPC("bulletOnRPC", RpcTarget.AllBuffered);
            }
            else
            {
                GameObject hit = gameManager.CreateObj(hitStr, GameManager.PoolTypes.BulletType);
                Bullet hitBullet = hit.GetComponent<Bullet>();
                hit.transform.position = transform.position;
                hit.transform.forward = gameObject.transform.forward;
                hit.transform.parent = transform.parent;
                hitBullet.bulletOnRPC();
            }
        }
    }
}
