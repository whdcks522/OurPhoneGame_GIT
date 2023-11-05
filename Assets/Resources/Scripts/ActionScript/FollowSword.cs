using Assets.PixelHeroes.Scripts.ExampleScripts;
using Photon.Pun;
using Photon.Pun.Demo.Asteroids;
using Photon.Pun.Demo.PunBasics;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;

public class FollowSword : MonoBehaviourPunCallbacks
{
    [Serializable]//Ŭ���� ����
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

    //Ŭ���� ���� ť
    Queue<FollowSwordInfo> followSwordQueue = new Queue<FollowSwordInfo>();

    public GameObject child;//�ڽ� ����
    int followDelay = 5;//���󰡴� �����ð�

    //���� Į�� ����
    private FollowSwordInfo childSwordInfo = new FollowSwordInfo(Vector3.zero, Quaternion.identity);

    [Header("�÷��̾� ���ӿ�����Ʈ(��Ȱ��ȭ ��, ������ ����)")]
    public GameObject player;
    //�÷��̾��� �Լ�
    public CharacterControls characterControls;

    [Header("Į�� �÷��̾� ������ �Ÿ� ���")]
    public float swordDir;


    [Header("������ �ӵ�")]
    public Vector3 leaderSwordVec = Vector3.zero;
    //���� Į�� �ӵ�
    int leaderSwordSpeed = 10;


    //���
    public TrailRenderer trailRenderer;
    //��� ������ ��ü Į�� ��
    int maxSwordIndex;
    //���� Į�� ���°����
    int curSwordIndex;
    //Į�� ������
    int swordDamage = 5;

    //��Ʋ �Ŵ���
    public BattleUIManager battleUIManager;
    Rigidbody rigid;

    


    #region �� ���� Ŭ���� ����
    /*
    [Serializable]//�ʿ��ϴ���
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

    public EnemySpawnInfoArray[] enemySpawnInfoArray;//é�� ��ü���� ��ȯ�� ���� ���
    private List<EnemySpawnInfo> enemySpawnList;//�̹� ������������ ��ȯ�� ���� ���
    */
    #endregion

    private void Awake()
    {
        battleUIManager = BattleUIManager.Instance;
        player = transform.root.gameObject;
        characterControls = player.GetComponent<CharacterControls>();

        maxSwordIndex = transform.parent.childCount;
        curSwordIndex = transform.GetSiblingIndex() + 1;//���� �ڽ��� �� ��°����

        rigid = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        trailRenderer.Clear();

        childSwordInfo = new FollowSwordInfo(Vector3.zero, Quaternion.identity);
    }

    void FixedUpdate()
    {
        //���� �� ���� ��, �÷��̾� ���� �ʱ�ȭ�� �ʿ���
        rigid.angularVelocity = Vector3.zero;

        //��� ���� ������ �����ֱ� ����
        if (curSwordIndex == 1)
        {
            rigid.velocity = leaderSwordVec * leaderSwordSpeed;

            //ȸ�� ����
            transform.rotation = Quaternion.identity;
            float zValue = Mathf.Atan2(rigid.velocity.x, rigid.velocity.y) * 180 / Mathf.PI;
            Vector3 rotVec = Vector3.back * zValue + Vector3.back * 45;
            transform.Rotate(rotVec);


            // �� ��ġ ���� �Ÿ��� ����մϴ�.
            Vector3 swordPos = transform.position;
            Vector3 playerPos = player.transform.position;


            swordDir = Vector3.Distance(swordPos, playerPos) / 500;
        }

        bool isRecentActive = false;

        //ť�� ���� ����
        followSwordQueue.Enqueue(new FollowSwordInfo(transform.position, transform.rotation));

        //���� ����, Ŭ���� ���� ���
        if (followSwordQueue.Count > followDelay)
        {
            childSwordInfo = followSwordQueue.Dequeue();

            //���� �ִٸ� ���� 
            if (!child.activeSelf && curSwordIndex < characterControls.curSwordCount) //���� Į�� ��ȣ x�� ĳ������ Į ������ 
            {
                child.SetActive(true);
                isRecentActive = true;
                //child.GetComponent<FollowSword>().trailRenderer.Clear();
            }
        }

        if (curSwordIndex >= characterControls.curSwordCount)//�� �� Į�� ���� ����
            return;


        //���� �̵�
        child.transform.rotation = childSwordInfo.swordRot;
        child.transform.position = childSwordInfo.swordVec;

        if(isRecentActive)//awake�ϱ� ���� �θ����  
            child.GetComponent<FollowSword>().trailRenderer.Clear();

        
    }

    
    private void OnTriggerExit(Collider other)
    {
        if (PhotonNetwork.InRoom)//��Ƽ ���̶��
        {
            if (other.transform.CompareTag("PlayerSwordArea") && curSwordIndex == 1 && photonView.IsMine)//���� ���� �浹 �ߴٸ�
            {
                PhotonView tmpPhotonView = other.gameObject.GetComponent<PhotonView>();
                if (tmpPhotonView.IsMine) //�ڽ��� �������� ���� ���� ����
                {
                    //�÷��̾�� ���� Į�� �Ÿ� ���� �ʱ�ȭ
                    swordDir = 0;
                    photonView.RPC("leaderSwordExitRPC", RpcTarget.AllBuffered, 1);
                }
            }
        }
        else //1���̶��
        {
            if (other.transform.CompareTag("PlayerSwordArea") && curSwordIndex == 1)//���� ���� �浹 �ߴٸ�
            {
                //�÷��̾�� ���� Į�� �Ÿ� ���� �ʱ�ȭ
                swordDir = 0;
                leaderSwordExitRPC(1);
            }
        }
    }

    #region Į�� ���� ������ ��Ż ��
    [PunRPC]
    public void leaderSwordExitRPC(int level)
    {
        //0: �׳� ����
        //1: ���⸸ ����(Į ���� ������ ���� ���)
        //2: ���ΰ� ���� ����
        if (level == 1 && characterControls.curSwordCount > 1)
        {
            createBomb(transform.position);
        }
        else if (level == 2) 
        {
            //�÷��̾� ����
            createBomb(player.transform.position);
            //���� ����
            if(gameObject.activeSelf)
                createBomb(transform.position);
        }
        
        //���� Į Ȱ��ȭ
        characterControls.backSwords.SetActive(true);
        //�ٽ� Į ��Ȱ��ȭ
        for (int i = 0; i <= maxSwordIndex - 1; i++)
        {
            GameObject tmpSword = characterControls.swordParent.transform.GetChild(i).gameObject;
            FollowSword tmpSwordComponent = tmpSword.GetComponent<FollowSword>();

            //Į Ȱ��ȭ
            tmpSword.SetActive(false);


            if (tmpSwordComponent != null)
            {
                tmpSwordComponent.followSwordQueue.Clear();//ť �ʱ�ȭ
            }
        }
    }
    #endregion

    #region ��ź ����
    void createBomb(Vector3 bombPos) 
    {
        //���� �� 1 ����
        characterControls.swordCountRPC(false);
        //��ź ����
        GameObject bomb = null;

        if (PhotonNetwork.InRoom)
        {
            if (photonView.IsMine)
            {
                bomb = battleUIManager.gameManager.CreateObj("Broken Phantasm", GameManager.PoolTypes.BombType);
            }
        }
        else if (!PhotonNetwork.InRoom)
        {
            bomb = battleUIManager.gameManager.CreateObj("Broken Phantasm", GameManager.PoolTypes.BombType);
        }
        //���� ���� ��� ���� �� ���� ����
        Bomb bombComponent = bomb.GetComponent<Bomb>();

        //��ź ��ġ ����
        bomb.transform.parent = battleUIManager.gameManager.transform;
        bomb.transform.position = bombPos;
        //��ź Ȱ��ȭ
        bombComponent.bombOnRPC();
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
                    //�Ѿ� �ı�
                    bullet.photonView.RPC("bulletOffRPC", RpcTarget.AllBuffered);
                    //ȸ��
                    characterControls.photonView.RPC("healOffRPC", RpcTarget.AllBuffered, bullet.bulletHeal);
                }
            }
            else if (!PhotonNetwork.InRoom)
            {
                //�Ѿ� �ı�
                bullet.bulletOffRPC();
                //ȸ��
                characterControls.healControlRPC(bullet.bulletHeal);
            }

            if (bullet.bulletEffectType == Bullet.BulletEffectType.PowerUp)//�Ŀ����� ���
            {
                if (PhotonNetwork.InRoom)
                {
                    if (photonView.IsMine)
                    {
                        //���� �� 1 ����
                        characterControls.photonView.RPC("swordCountRPC", RpcTarget.AllBuffered, true);
                    }
                }
                else if (!PhotonNetwork.InRoom)
                {
                    //���� �� 1 ����
                    characterControls.swordCountRPC(true);
                }

            }
            else if (bullet.bulletEffectType == Bullet.BulletEffectType.Normal)
            {
                //�Ϲ� ȿ����
                battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Heal);
            }
        }
       
    }

    private void OnCollisionStay(Collision other)
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
