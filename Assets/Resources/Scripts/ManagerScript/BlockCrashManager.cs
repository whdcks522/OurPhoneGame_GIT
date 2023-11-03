using Photon.Pun.Demo.Asteroids;
using Photon.Pun.Demo.PunBasics;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockCrashManager : MonoBehaviour
{
    GameManager gameManager;
    GameObject player;
    [Header("� �߻� ����")]
    public Transform[] blockPoints;

    [Header("�ִ� �߻� �ð�")]
    public float maxTime;
    //���� �߻� �ð�
    float curTime;

    [Header("�Ŀ��� ����ü ���")]
    public Transform PowerUpPos;
    [Header("�Ŀ��� �ֱ�")]
    public int maxPowerUpindex;
    //���� �Ŀ��� ����
    int curPowerUpindex = 0;

    private void Awake()
    {
        gameManager = GameManager.Instance;
        player = gameManager.player;
    }

    private void Update()
    {
        curTime += Time.deltaTime;

        if (curTime > maxTime)
        {
            //�ð� �ʱ�ȭ
            curTime = 0f;

            //���� ��ġ ���ϱ�
            int ranPos = Random.Range(0, blockPoints.Length);


            GameObject block = gameManager.CreateObj("NormalBlock", GameManager.PoolTypes.BlockType);
            Block blockComponent = block.GetComponent<Block>();

            //��� �θ� ����
            block.transform.parent = this.transform;
            blockComponent.createPos = blockPoints[ranPos].position;

            //��� Ȱ��ȭ
            blockComponent.blockOnRPC();

            curPowerUpindex++;
            if (curPowerUpindex >= maxPowerUpindex) 
            {
                curPowerUpindex = 0;

                //����ü ����
                GameObject bullet = gameManager.CreateObj("GreenStarBullet", GameManager.PoolTypes.BulletType);

                //������Ʈ ����
                Rigidbody bulletRigid = bullet.GetComponent<Rigidbody>();
                Bullet bulletComponent = bullet.GetComponent<Bullet>();

                //��ġ ����
                bullet.transform.parent = this.transform;
                bullet.transform.position = PowerUpPos.position;

                //� Ȱ��ȭ
                bulletComponent.bulletOnRPC();

                //�ӵ� ����
                Vector2 bulletVec = (player.transform.position - bullet.transform.position).normalized;

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


}
