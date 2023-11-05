using Photon.Pun.Demo.Procedural;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class StarFallManager : MonoBehaviour
{
    

    [Header("� �߻� ����")]
    public Transform[] starFallPoints;
    //� �߻� ������ ����
    int starFallPointsSize;

    //�ִ� �߻� �ð�
    public float maxTime;
    //���� �߻� �ð�
    float curTime;
    //�ֱٿ� �߻��� ��ġ
    int recentPos;

    [Header("�Ŀ��� ���� ��ũ �� �ֱ�")]
    public int[] PowerUpIndexArr;
    //�ִ� �Ŀ��� ���� 
    int maxPowerUpIndex;
    //���� �Ŀ��� ���� 
    int curPowerUpIndex = 0;

    [Header("�߰� ���� �ӵ� �迭")]
    public float[] createBlockArr;
    //��ũ�� ���� �߰� ����
    float rankSpeed = 1;

    [Header("���� ����")]
    public int scenelevel;

    BattleUIManager battleUIManager;
    GameManager gameManager;
    GameObject player;

    private void Awake()
    {
        battleUIManager = BattleUIManager.Instance;
        gameManager = battleUIManager.gameManager;
        player = gameManager.player;
        starFallPointsSize = starFallPoints.Length;

    }

    private void Update()
    {
        curTime += Time.deltaTime * rankSpeed;


        if (curTime > maxTime)
        {
            switch (battleUIManager.rankType)
            {
                case BattleUIManager.RankType.S:
                    rankSpeed = createBlockArr[0];
                    maxPowerUpIndex = PowerUpIndexArr[0];
                    break;
                case BattleUIManager.RankType.A:
                    rankSpeed = createBlockArr[1];
                    maxPowerUpIndex = PowerUpIndexArr[1];
                    break;
                case BattleUIManager.RankType.B:
                    rankSpeed = createBlockArr[2];
                    maxPowerUpIndex = PowerUpIndexArr[2];
                    break;
                case BattleUIManager.RankType.C:
                    rankSpeed = createBlockArr[3];
                    maxPowerUpIndex = PowerUpIndexArr[3];
                    break;
                case BattleUIManager.RankType.D:
                    rankSpeed = createBlockArr[4];
                    maxPowerUpIndex = PowerUpIndexArr[4];
                    break;
                case BattleUIManager.RankType.E:
                    rankSpeed = createBlockArr[5];
                    maxPowerUpIndex = PowerUpIndexArr[5];
                    break;
            }

            //�ð� �ʱ�ȭ
            curTime = 0f;

            //���� ��ġ ���ϱ�
            int ranPos = Random.Range(0, starFallPointsSize);
            //���� �� �������� �ȵǵ��� ����
            if (ranPos == recentPos) 
                ranPos = (ranPos + 1) / starFallPointsSize;
            recentPos = ranPos;


            GameObject bullet = null;
            //bullet = gameManager.CreateObj("GreenStarBullet", GameManager.PoolTypes.BulletType);
            //� ������Ʈ ����
            
            if (curPowerUpIndex >= maxPowerUpIndex)//��ȭ �
            {
                bullet = gameManager.CreateObj("GreenStarBullet", GameManager.PoolTypes.BulletType);
                curPowerUpIndex = 0;
            }
            else //�⺻ �
            {
                bullet = gameManager.CreateObj("YellowStarBullet", GameManager.PoolTypes.BulletType);
                curPowerUpIndex++;
            }
            
            //������Ʈ ����
            Rigidbody bulletRigid = bullet.GetComponent<Rigidbody>();
            Bullet bulletComponent = bullet.GetComponent<Bullet>();

            //��ġ ����
            bullet.transform.parent = this.transform;
            bullet.transform.position = starFallPoints[ranPos].position;

            //� Ȱ��ȭ
            bulletComponent.bulletOnRPC();

            //�ӵ� ����
            Vector2 bulletVec = (player.transform.position - bullet.transform.position).normalized;

            //�ణ�� �˵� ��ȭ
            bulletVec += 0.1f * Random.insideUnitCircle;//�������� 1�� �� �ȿ��� ���� ����2 ��ǥ �����
            bulletVec = bulletVec.normalized;

            //���� �ӵ� ����
            bulletRigid.velocity = bulletVec * bulletComponent.bulletSpeed;

            //ȸ�� ����
            bullet.transform.rotation = Quaternion.identity;
            float zValue = Mathf.Atan2(bulletRigid.velocity.x, bulletRigid.velocity.y) * 180 / Mathf.PI;
            Vector3 rotVec = Vector3.back * zValue + Vector3.back * 45.0f;
            bullet.transform.Rotate(rotVec); 
        }
    }
}
