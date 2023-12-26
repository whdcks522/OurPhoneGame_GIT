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

    [Header("Į�� ���������� ���� �浹 �� ���� �ִ� ��(���� ���� �� ��)")]
    public bool isAlreadyHit = false;

    BattleUIManager battleUIManager;
    GameManager gameManager;
    Rigidbody2D rigid;

    
    public enum BulletEffectType
    {
        Normal, PowerUp, UnBreakable, Chase
    }
    [Header("�Ѿ��� Ư��ȿ��")]
    public BulletEffectType bulletEffectType;

    [Header("���� ��, �÷��� ����Ʈ ��� ����")]
    public bool isFlash;
    public string flashStr;
    [Header("���� ��, ��Ʈ ����Ʈ ��� ����")]
    public bool isHit;
    public string hitStr;
    [Header("�÷��̾� ��ũ��Ʈ")]
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
            //�ӵ� ����
            Vector2 bulletVec = (bulletTarget.position - transform.position).normalized;

            //���� �ӵ� ����
            rigid.velocity = bulletVec * bulletSpeed;
        }
    }

    #region �Ѿ� Ȱ�� ����ȭ
    [PunRPC]
    public void bulletOnRPC()
    {
        //���ӿ�����Ʈ Ȱ��ȭ
        gameObject.SetActive(true);
        //ȸ�� �ʱ�ȭ
        transform.rotation = Quaternion.identity;
        //���� �ǰ� ó�� �� ���� ����
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

                    //ȸ�� ����
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

                //ȸ�� ����
                flash.transform.rotation = transform.rotation;
            }
        }
    }
    #endregion

    #region �Ѿ� ��Ȱ�� ����ȭ
    [PunRPC]
    public void bulletOffRPC(int type)//Į�� �¾Ұų�, �ڿ��Ҹ��߰ų�
    {
        //�ð� ����ȭ
        curTime = 0f;
        //���ӿ�����Ʈ ��Ȱ��ȭ
        gameObject.SetActive(false);
        //-1: �÷��̾ �Ѿ˿� ���� ���
        // 0: �ڿ� �Ҹ��� ���
        //+1: �÷��̾ Į�� �Ѿ��� �ı��� ���

        if (isHit)//���� ����Ʈ ���
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

        //1: Į, ��ź���� �ı��ؼ� ȸ��
        if (type == 1)
        {
            characterControls.healControlRPC(bulletHeal);//ȸ�� ȿ������ ȸ�� �ڵ忡�� ����

            if (bulletEffectType == Bullet.BulletEffectType.Normal)//�Ϲ� �Ѿ��� ���
            {
                //�Ϲ� ȿ����
                battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Heal);
            }
            else if (bulletEffectType == Bullet.BulletEffectType.PowerUp)//�Ŀ����� ���(ȿ������ �ȿ��� �����)
            {
                if (PhotonNetwork.InRoom)
                {
                    if (photonView.IsMine)
                    {
                        //���� �� 1 ����
                        characterControls.photonView.RPC("swordCountRPC", RpcTarget.AllBuffered, 1);
                    }
                }
                else if (!PhotonNetwork.InRoom)
                {
                    //���� �� 1 ����
                    characterControls.swordCountRPC(1);
                }
            }
        }
        else if (type == -1)//�Ѿ��� �÷��̾ ���� ���
        {
            if (!isAlreadyHit)
            {
                if (PhotonNetwork.InRoom)
                {
                    if (photonView.IsMine)
                    {
                        //�÷��̾� ü�� ����
                        characterControls.photonView.RPC("damageControlRPC", RpcTarget.All, bulletDamage, true);
                    }
                }
                else if (!PhotonNetwork.InRoom)
                {
                    //�ǰ� ó��
                    characterControls.damageControlRPC(bulletDamage, true);
                }
            }
        }
        //���� �ǰ� ���Ͼ����
        isAlreadyHit = true;
    }
    #endregion

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.CompareTag("playerSword")|| other.transform.CompareTag("Bomb"))//��ź�̳� Į�� �浹�� ���
        {
            if (bulletEffectType == Bullet.BulletEffectType.UnBreakable)
                return;

            //�Ѿ� �ı��� ȸ��
            if (PhotonNetwork.InRoom)
            {
                if (photonView.IsMine)
                {
                    if (!isAlreadyHit) 
                    {
                        //�Ѿ� �ı�
                        photonView.RPC("bulletOffRPC", RpcTarget.All, 1);
                    }
                    
                }
            }
            else if (!PhotonNetwork.InRoom)
            {
                if (!isAlreadyHit) 
                {
                    //�Ѿ� �ı�
                    bulletOffRPC(1);
                }   
            }
        }
        else if (other.transform.CompareTag("Player"))//�Ѿ��� �÷��̾ ����
        {
            //�Ѿ� �ı��� ȸ��
            if (PhotonNetwork.InRoom)
            {
                if (photonView.IsMine)
                {
                    //�Ѿ� �ı�
                    photonView.RPC("bulletOffRPC", RpcTarget.All, -1);
                }
            }
            else if (!PhotonNetwork.InRoom)
            {
                //�Ѿ� �ı�
                bulletOffRPC(-1);
            }
        }
        else if (other.transform.CompareTag("Outline")) //�� ������ �������� ����
        {
            if (PhotonNetwork.InRoom)
            {
                if (photonView.IsMine)
                    //�Ѿ� �ı�
                    photonView.RPC("bulletOffRPC", RpcTarget.AllBuffered);

            }
            else if (!PhotonNetwork.InRoom)
            {
                bulletOffRPC(0);
            }
        }
        
    }
}
