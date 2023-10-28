using Assets.PixelHeroes.Scripts.ExampleScripts;
using Photon.Pun;
using Photon.Pun.Demo.Asteroids;
using System;
using System.Collections;
using System.Collections.Generic;
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

    GameObject child;//�ڽ� ����
    int followDelay = 5;//���󰡴� �����ð�
    //���� Į�� ����
    private FollowSwordInfo childSwordInfo = new FollowSwordInfo(Vector3.zero, Quaternion.identity);
    [Header("�÷��̾� ���ӿ�����Ʈ")]
    public GameObject player;
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

        maxSwordIndex = transform.parent.childCount - 1;
        curSwordIndex = transform.GetSiblingIndex();//���� �ڽ��� �� ��°����

        if (curSwordIndex != maxSwordIndex) //��ȣ ����
        {
            child = transform.parent.GetChild(curSwordIndex+1).gameObject;
        }

        if (curSwordIndex == 0)//���� Į�� ���
        {
            characterControls = player.GetComponent<CharacterControls>();
            trailRenderer = GetComponentInChildren<TrailRenderer>();
        }
    }

    private void OnEnable()
    {
        if(maxSwordIndex != curSwordIndex)//Ŭ���� ��ü ���� �ʱ�ȭ
            childSwordInfo = new FollowSwordInfo(Vector3.zero, Quaternion.identity);
    }

    void FixedUpdate()
    {
        if (maxSwordIndex == curSwordIndex)//�� �� Į�� ���� ����
            return;

        //ť�� ���� ����
        followSwordQueue.Enqueue(new FollowSwordInfo(transform.position, transform.rotation));

        //���� ����, Ŭ���� ���� ���
        if (followSwordQueue.Count > followDelay)
        {
            childSwordInfo = followSwordQueue.Dequeue();

            if (!child.activeSelf) //���� �ִٸ� ����
            { 
                child.SetActive(true);
                child.transform.position = player.transform.position + Vector3.up * 0.5f;
            }
        }

        //���� �̵�
        child.transform.rotation = childSwordInfo.swordRot;
        child.transform.position = childSwordInfo.swordVec;

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
        //Ʈ���� ������ ����
        trailRenderer.Clear();

        for (int i = 0; i <= maxSwordIndex; i++)
        {
            GameObject tmpSword = characterControls.swordParent.transform.GetChild(i).gameObject;
            FollowSword tmpSwordComponent = tmpSword.GetComponent<FollowSword>();

            tmpSword.transform.position = player.transform.position;
            tmpSword.SetActive(false);


            if (tmpSwordComponent != null)
            {
                tmpSwordComponent.followSwordQueue.Clear();
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

            if (PhotonNetwork.InRoom && photonView.IsMine)
            {
                bullet.photonView.RPC("bulletOffRPC", RpcTarget.AllBuffered);
            }
            else
            {
                bullet.bulletOffRPC();
            }
            battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.PowerUp);
        }
    }

    #region Į�� ���� ������ ��Ż ��
    
    #endregion

}
