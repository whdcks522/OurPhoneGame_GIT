using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FollowSword : MonoBehaviour
{
    public GameObject child;//�ڽ� ����
    private int followDelay = 6;//���󰡴� �����ð�
    //���� �θ� Į�� ����
    private FollowSwordInfo childSwordInfo = new FollowSwordInfo(Vector3.zero, Quaternion.identity);

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

   
    void Update()
    {
        //�÷��̾ ���߸� ����
        if (!followSwordQueue.Contains(childSwordInfo))
            followSwordQueue.Enqueue(new FollowSwordInfo(transform.position, transform.rotation));

        //���
        if (followSwordQueue.Count > followDelay)
        {
            childSwordInfo = followSwordQueue.Dequeue();

            if (!child.activeSelf) //���� �ִٸ� ����
            { 
                child.SetActive(true);
            }
        }

        //���� �̵�
        child.transform.rotation = childSwordInfo.swordRot;
        child.transform.position = childSwordInfo.swordVec;
    }
}
