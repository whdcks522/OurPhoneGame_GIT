using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FollowSword : MonoBehaviour
{
    public Transform parent;//�θ� ����
    public int followDelay;//���󰡴� �����ð�
    public SpriteRenderer spriteRenderer;
    //���� �θ� Į�� ����
    private FollowSwordInfo curSwordInfo = new FollowSwordInfo(Vector3.zero, Quaternion.identity);

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
        //if (!parentPos.Contains(parent.position))

        followSwordQueue.Enqueue(new FollowSwordInfo(parent.position, parent.rotation));

        //���
        if (followSwordQueue.Count > followDelay) 
        {
            curSwordInfo = followSwordQueue.Dequeue();
            spriteRenderer.color = Color.white;
        }
        //�� ��á�ٸ�
        else if (followSwordQueue.Count < followDelay)
            curSwordInfo.swordVec = parent.position;//���� ���� Į�� ��ġ�� ����


        transform.position = curSwordInfo.swordVec;
        transform.rotation = curSwordInfo.swordRot;
    }
}
