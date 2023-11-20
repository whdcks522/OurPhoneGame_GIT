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
    [Header("�Ѿ��� �ִ� ����")]
    public float maxTime;
    float curTime = 0f;

    [Header("�Ѿ��� �ӵ�")]
    public float bulletSpeed;

    [Header("�Ѿ��� ���ط�")]
    public int bulletDamage;

    [Header("�ı� ��, �Ѿ��� ȸ����")]
    public int bulletHeal;

    [Header("�Ѿ��� ��ǥ")]
    public Transform bulletTarget;

    [Header("�Ѿ��� ��ǥ")]
    public bool isAlreadyHit = false;

    BattleUIManager battleUIManager;
    GameManager gameManager;
    Rigidbody2D rigid;
    ParticleSystem particleSystem;

    
    public enum BulletEffectType
    {
        Normal, PowerUp, UnBreakable, Chase
    }
    [Header("�Ѿ��� Ư��ȿ��")]
    public BulletEffectType bulletEffectType;

    [Header("���� �� �÷���")]
    public bool isFlash;
    public string flashStr;
    [Header("���� �� ��Ʈ")]
    public bool isHit;
    [Header("��Ʈ ��, �����ϴ� ������Ʈ")]
    public string hitStr;

    private void Awake()
    {
        battleUIManager = BattleUIManager.Instance;
        gameManager = battleUIManager.gameManager;
        rigid = GetComponent<Rigidbody2D>();
        particleSystem = GetComponent<ParticleSystem>();
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
            //�ӵ� ����
            Vector2 bulletVec = (bulletTarget.position - transform.position).normalized;

            //���� �ӵ� ����
            rigid.velocity = bulletVec * bulletSpeed;

            //ȸ�� ����
            transform.rotation = Quaternion.identity;
            float zValue = Mathf.Atan2(rigid.velocity.x, rigid.velocity.y) * 180 / Mathf.PI;
            Vector3 rotVec = Vector3.back * zValue + Vector3.back * 45.0f;
            transform.Rotate(rotVec);
        }
    }

    #region �Ѿ� Ȱ�� ����ȭ
    [PunRPC]
    public void bulletOnRPC()
    {
        //���ӿ�����Ʈ Ȱ��ȭ
        gameObject.SetActive(true);
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

                //ȸ�� ����
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

                //ȸ�� ����
                flash.transform.rotation = transform.rotation;
            }
        }
    }
    #endregion

    #region �Ѿ� ��Ȱ�� ����ȭ
    [PunRPC]
    public void bulletOffRPC()
    {
        //���� �ǰ� ���Ͼ����
        isAlreadyHit = true;
        //�ð� ����ȭ
        curTime = 0f;
        //���ӿ�����Ʈ ��Ȱ��ȭ
        gameObject.SetActive(false);
        

        if (isHit)
        {
            GameObject hit = gameManager.CreateObj(hitStr, GameManager.PoolTypes.BulletType);
            Effect hitEffect = hit.GetComponent<Effect>();

            if (PhotonNetwork.InRoom)
            {
                if (PhotonNetwork.IsMasterClient)
                    hitEffect.photonView.RPC("effectOnRPC", RpcTarget.AllBuffered, transform);
            }
            else if (!PhotonNetwork.InRoom)
            {
                hitEffect.effectOnRPC(transform);
            }
        }
    }
    #endregion

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.CompareTag("Outline")) //�� ������ �������� ����
        {
            if (PhotonNetwork.InRoom) 
            {
                if(photonView.IsMine)
                    //�Ѿ� �ı�
                    photonView.RPC("bulletOffRPC", RpcTarget.AllBuffered);

            }
            else if (!PhotonNetwork.InRoom)
            {
                bulletOffRPC();
            }
        }
    }
}
