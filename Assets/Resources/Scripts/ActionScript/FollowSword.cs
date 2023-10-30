using Assets.PixelHeroes.Scripts.ExampleScripts;
using Photon.Pun;
using Photon.Pun.Demo.Asteroids;
using Photon.Pun.Demo.PunBasics;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
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
    //[Header("�÷��̾� ���ӿ�����Ʈ")]
    GameObject player;
    //�÷��̾��� �Լ�
    private CharacterControls characterControls;
    //���
    private TrailRenderer trailRenderer;

    [Header("Į�� �÷��̾� ������ �Ÿ� ���")]
    public float swordDir;

    //�ִ� Į�� ��
    int maxSwordIndex;
    //���� Į�� ���°����
    int curSwordIndex;
    //��Ʋ �Ŵ���
    BattleUIManager battleUIManager;


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

        maxSwordIndex = transform.parent.childCount - 1;
        curSwordIndex = transform.GetSiblingIndex();//���� �ڽ��� �� ��°����

        trailRenderer = GetComponentInChildren<TrailRenderer>();
        
    }

    private void OnEnable()
    {
        if(maxSwordIndex != curSwordIndex)//Ŭ���� ��ü ���� �ʱ�ȭ
            childSwordInfo = new FollowSwordInfo(Vector3.zero, Quaternion.identity);
    }

    void FixedUpdate()
    {
        //ť�� ���� ����
        followSwordQueue.Enqueue(new FollowSwordInfo(transform.position, transform.rotation));

        //���� ����, Ŭ���� ���� ���
        if (followSwordQueue.Count > followDelay)
        {
            childSwordInfo = followSwordQueue.Dequeue();

            if (!child.activeSelf && curSwordIndex < characterControls.curSwordCount) //���� �ִٸ� ���� //child != null && 
            {
                
                child.SetActive(true);
                trailRenderer.Clear();
                //child.transform.position = player.transform.position + Vector3.up * 0.5f;
            }
        }

        if (curSwordIndex > characterControls.curSwordCount)//�� �� Į�� ���� ����
            return;


        //���� �̵�
        child.transform.rotation = childSwordInfo.swordRot;
        child.transform.position = childSwordInfo.swordVec;

        //������ �����ֱ� ����
        if (curSwordIndex == 0) 
        {
            Vector3 swordPos = transform.position;
            Vector3 playerPos = player.transform.position;

            // �� ��ġ ���� �Ÿ��� ����մϴ�.
            swordDir = Vector3.Distance(swordPos, playerPos)/ 500;
        }
    }

    
    private void OnTriggerExit(Collider other)
    {
        if (PhotonNetwork.InRoom)//��Ƽ ���̶��
        {
            if (other.transform.CompareTag("PlayerSwordArea") && curSwordIndex == 0 && photonView.IsMine)//���� ���� �浹 �ߴٸ�
            {
                PhotonView tmpPhotonView = other.gameObject.GetComponent<PhotonView>();
                if (tmpPhotonView.IsMine) //�ڽ��� �������� ���� ���� ����
                {
                    //�÷��̾�� ���� Į�� �Ÿ� ���� �ʱ�ȭ
                    swordDir = 0;
                    photonView.RPC("leaderSwordExitRPC", RpcTarget.AllBuffered);
                }
            }
        }
        else //1���̶��
        {
            if (other.transform.CompareTag("PlayerSwordArea") && curSwordIndex == 0)//���� ���� �浹 �ߴٸ�
            {
                //�÷��̾�� ���� Į�� �Ÿ� ���� �ʱ�ȭ
                swordDir = 0;
                leaderSwordExitRPC();
            }
        }
    }

    #region Į�� ���� ������ ��Ż ��
    [PunRPC]
    void leaderSwordExitRPC()
    {
        //���� Į Ȱ��ȭ
        characterControls.backSwords.SetActive(true);
        //���� �� 1 ����
        characterControls.swordCountRPC(false);
        //��ź ����
        GameObject bomb = null;

        if (PhotonNetwork.InRoom)
        {
            if (photonView.IsMine)
                bomb = battleUIManager.gameManager.CreateObj("Broken Phantasm", GameManager.PoolTypes.BombType);
        }
        else if (!PhotonNetwork.InRoom) 
        {
            bomb = battleUIManager.gameManager.CreateObj("Broken Phantasm", GameManager.PoolTypes.BombType);
        }
        //���� ���� ��� ���� �� ���� ����
        Bomb bombComponent = bomb.GetComponent<Bomb>();
        
        //��ź ��ġ ����
        bomb.transform.parent = battleUIManager.gameManager.transform;
        bomb.transform.position = transform.position;
        //��ź Ȱ��ȭ
        bombComponent.bombOnRPC();


        


        for (int i = 0; i <= maxSwordIndex; i++)
        {
            GameObject tmpSword = characterControls.swordParent.transform.GetChild(i).gameObject;
            FollowSword tmpSwordComponent = tmpSword.GetComponent<FollowSword>();

            tmpSword.SetActive(false);
            //Ʈ���� ������ �ʱ�ȭ
            trailRenderer.Clear();

            if (tmpSwordComponent != null)
            {
                tmpSwordComponent.followSwordQueue.Clear();//ť �ʱ�ȭ
            }
        }
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

}
