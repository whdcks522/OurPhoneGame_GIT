using Photon.Pun.Demo.Procedural;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class StarFallManager : MonoBehaviour
{
    GameManager gameManager;
    GameObject player;
    [Header("� �߻� ����")]
    public Transform[] starFallPoints;
    int maxAttackIndex;

    //�ִ� �߻� �ð�
    public float maxTime;
    //���� �߻� �ð�
    float curTime;
    //�ֱٿ� �߻��� ��ġ
    int recentPos;
    //�Ŀ��� �ֱ�
    public int maxPowerUpindex;
    //���� �Ŀ��� ����
    int curPowerUpindex = 0;

    private void Awake()
    {
        gameManager = GameManager.Instance;
        player = gameManager.player;
        maxAttackIndex = starFallPoints.Length;

    }

    private void Update()
    {
        curTime += Time.deltaTime;


        if (curTime > maxTime)
        {
            //�ð� �ʱ�ȭ
            curTime = 0f;


            //���� ��ġ ���ϱ�
            int ranPos = Random.Range(0, maxAttackIndex);
            //���� �� �������� �ȵǵ��� ����
            if (ranPos == recentPos) 
                ranPos = (ranPos + 1) / maxAttackIndex;
            recentPos = ranPos;


            GameObject bullet = null;
            //bullet = gameManager.CreateObj("GreenStarBullet", GameManager.PoolTypes.BulletType);
            //� ������Ʈ ����
            
            if (curPowerUpindex >= maxPowerUpindex)//��ȭ �
            {
                bullet = gameManager.CreateObj("GreenStarBullet", GameManager.PoolTypes.BulletType);
                curPowerUpindex = 0;
            }
            else //�⺻ �
            {
                bullet = gameManager.CreateObj("YellowStarBullet", GameManager.PoolTypes.BulletType);
                curPowerUpindex++;
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
