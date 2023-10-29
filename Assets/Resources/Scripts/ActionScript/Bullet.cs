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

    #region 총알 활성 동기화
    [PunRPC]
    public void bulletOnRPC()
    {
        //게임오브젝트 활성화
        gameObject.SetActive(true);

        // 파티클 시스템을 다시 시작
        //particleSystem.Stop();
        //particleSystem.Play();

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
            else
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
        //시간 동기화
        curTime = 0f;
        //게임오브젝트 비활성화
        gameObject.SetActive(false);
        // 파티클 시스템을 다시 시작
        //particleSystem.Stop();
        //particleSystem.Play();

        if (isHit)
        {
            if (PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient)
            {
                GameObject hit = gameManager.CreateObj(hitStr, GameManager.PoolTypes.BulletType);
                Effect hitEffect = hit.GetComponent<Effect>();

                //위치 조정
                hit.transform.position = transform.position;
                hit.transform.forward = gameObject.transform.forward;
                hit.transform.parent = transform.parent;
                hitEffect.photonView.RPC("effectOnRPC", RpcTarget.AllBuffered);

                //회전 조정
                hit.transform.rotation = transform.rotation;
            }
            else
            {
                GameObject hit = gameManager.CreateObj(hitStr, GameManager.PoolTypes.BulletType);
                Effect hitEffect = hit.GetComponent<Effect>();
                hit.transform.position = transform.position;
                hit.transform.forward = gameObject.transform.forward;
                hit.transform.parent = transform.parent;
                hitEffect.effectOnRPC();

                //회전 조정
                hit.transform.rotation = transform.rotation;
            }
        }
    }
    #endregion
}
