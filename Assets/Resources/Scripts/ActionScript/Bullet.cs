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

    void Start()
    {
        /*
        if (flash != null)
        {
            //���� ����Ʈ ����
            var flashInstance = Instantiate(flash, transform.position, Quaternion.identity);
            //���� ����Ʈ ���� ����
            flashInstance.transform.forward = gameObject.transform.forward;


            //���� ����Ʈ ����
            var flashPs = flashInstance.GetComponent<ParticleSystem>();
            if (flashPs != null)
            {
                Destroy(flashInstance, flashPs.main.duration);//���� ������Ʈ�� ����
            }
            else
            {
                var flashPsParts = flashInstance.transform.GetChild(0).GetComponent<ParticleSystem>();
                Destroy(flashInstance, flashPsParts.main.duration);//���� ������Ʈ�� ����
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
        if (other.transform.CompareTag("playerSword"))//Į�� �浹
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
        if (collision.transform.CompareTag("player")) //�÷��̾�� �浹
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
        //�ð� ����ȭ
        curTime = 0f;
        //���ӿ�����Ʈ ��Ȱ��ȭ
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
