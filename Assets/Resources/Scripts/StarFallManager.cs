using Photon.Pun.Demo.Procedural;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class StarFallManager : MonoBehaviour
{
    GameManager gameManager;
    GameObject player;
    public GameObject star;
    public Transform[] starFallPoints;
    int maxAttackIndex;
    

    float maxTime = 0.5f;
    float curTime = 0f;
    int recentPos;

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


            //� ������Ʈ ����
            GameObject bullet = gameManager.CreateObj("YellowStarBullet", GameManager.PoolTypes.BulletType);
            //������Ʈ ����
            Rigidbody bulletRigid = bullet.GetComponent<Rigidbody>();
            Bullet bulletComponent = bullet.GetComponent<Bullet>();

            //��ġ ����
            bullet.transform.position = starFallPoints[ranPos].position;
            bullet.transform.parent = this.transform;

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
            float zValue = Mathf.Atan2(bulletRigid.velocity.x, bulletRigid.velocity.y) * 180 / Mathf.PI;
            Vector3 rotVec = Vector3.back * zValue + Vector3.back * 45.0f;
            bullet.transform.Rotate(rotVec); 
        }
    }
}
