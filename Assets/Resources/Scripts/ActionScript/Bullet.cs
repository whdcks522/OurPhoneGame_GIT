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
    [Header("�Ѿ��� Ư��ȿ��")]
    public BulletEffectType bulletEffectType;

    [Header("���� �� �÷���")]
    public bool isFlash;
    public string flashStr;
    [Header("���� �� ��Ʈ")]
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

    [PunRPC]
    public void bulletOnRPC()
    {
        //���ӿ�����Ʈ Ȱ��ȭ
        gameObject.SetActive(true);

        // ��ƼŬ �ý����� �ٽ� ����
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

                //ȸ�� ����
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

                //ȸ�� ����
                flash.transform.rotation = transform.rotation;
            }
        }
    }


    [PunRPC]
    public void bulletOffRPC()
    {
        //�ð� ����ȭ
        curTime = 0f;
        //���ӿ�����Ʈ ��Ȱ��ȭ
        gameObject.SetActive(false);
        // ��ƼŬ �ý����� �ٽ� ����
        particleSystem.Stop();
        particleSystem.Play();

        if (isHit)
        {
            if (PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient)
            {
                GameObject hit = gameManager.CreateObj(hitStr, GameManager.PoolTypes.BulletType);
                Bullet hitBullet = hit.GetComponent<Bullet>();

                //��ġ ����
                hit.transform.position = transform.position;
                hit.transform.forward = gameObject.transform.forward;
                hit.transform.parent = transform.parent;
                hitBullet.photonView.RPC("bulletOnRPC", RpcTarget.AllBuffered);

                //ȸ�� ����
                hit.transform.rotation = transform.rotation;
            }
            else
            {
                GameObject hit = gameManager.CreateObj(hitStr, GameManager.PoolTypes.BulletType);
                Bullet hitBullet = hit.GetComponent<Bullet>();
                hit.transform.position = transform.position;
                hit.transform.forward = gameObject.transform.forward;
                hit.transform.parent = transform.parent;
                hitBullet.bulletOnRPC();

                //ȸ�� ����
                hit.transform.rotation = transform.rotation;
            }
        }
    }
}
