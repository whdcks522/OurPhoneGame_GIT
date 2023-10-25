using Assets.PixelHeroes.Scripts.ExampleScripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

public class FollowSword : MonoBehaviour
{
    public GameObject child;//�ڽ� ����
    private int followDelay = 5;//���󰡴� �����ð�
    //���� �θ� Į�� ����
    private FollowSwordInfo childSwordInfo = new FollowSwordInfo(Vector3.zero, Quaternion.identity);
    public GameObject player;
    private CharacterControls characterControls;
    private TrailRenderer trailRenderer;
    public float swordDir;
    public int maxSwordIndex;
    public int curSwordIndex;//

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
        maxSwordIndex = transform.parent.childCount - 1;
        curSwordIndex = transform.GetSiblingIndex();

        if (curSwordIndex != maxSwordIndex) 
        {
            child = transform.parent.GetChild(curSwordIndex+1).gameObject;
        }

        if (curSwordIndex == 0)
        {
            characterControls = player.GetComponent<CharacterControls>();
            trailRenderer = GetComponentInChildren<TrailRenderer>();
        }
    }

    private void OnEnable()
    {
        if(maxSwordIndex != curSwordIndex)
        childSwordInfo = new FollowSwordInfo(Vector3.zero, Quaternion.identity);
    }



    [Serializable]
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
    public Queue<FollowSwordInfo> followSwordQueue = new Queue<FollowSwordInfo>();



    void FixedUpdate()
    {
        if (maxSwordIndex == curSwordIndex)
            return;

        //ť�� ���� ����
        followSwordQueue.Enqueue(new FollowSwordInfo(transform.position, transform.rotation));

        //���
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
        if (other.transform.CompareTag("PlayerSwordArea") && curSwordIndex == 0)//���� ���� �浹 �ߴٸ�
        {
            //���� Į Ȱ��ȭ
            characterControls.backSwords.SetActive(true);
            //Ʈ���� ������ ����
            trailRenderer.Clear();
            //�Ÿ� ���� �ʱ�ȭ
            swordDir = 0;
            int tmpChildSize = characterControls.swordParent.transform.childCount;

            for (int i = 0; i < tmpChildSize; i++)
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
    }

    private void OnTriggerExit(Collider2D collision)
    {
        if (collision.transform.CompareTag("PlayerSwordArea") && curSwordIndex == 0)//���� ���� �浹 �ߴٸ�
        {
            //���� Į Ȱ��ȭ
            characterControls.backSwords.SetActive(true);
            //Ʈ���� ������ ����
            trailRenderer.Clear();
            //�Ÿ� ���� �ʱ�ȭ
            swordDir = 0;
            int tmpChildSize = characterControls.swordParent.transform.childCount;

            for (int i = 0; i < tmpChildSize; i++) 
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
    }
}
