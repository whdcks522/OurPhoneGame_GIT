using Assets.PixelHeroes.Scripts.ExampleScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyManager : MonoBehaviour
{
    //�ִ� �߻� �ð�
    float maxTime = 2f;
    //���� �߻� �ð�
    float curTime = 0f;


    [Header("�ٶ� �߻� ����")]
    public Transform[] windPoints;

    [Header("���� ����")]
    public int scenelevel;

    [Header("�ٶ� �ִ� �ֱ�")]
    public int[] windIndexArr;
    //�ִ� �Ŀ��� ���� 
    //int maxPowerUpIndex;
    //���� �Ŀ��� ���� 
    //int curPowerUpIndex = 0;


    [Header("�÷��̾� ��ũ��Ʈ")]
    public CharacterControls characterControls;

    BattleUIManager battleUIManager;
    [Header("���� �Ŵ���")]
    public GameManager gameManager;

    private void Start()
    {
        battleUIManager = BattleUIManager.Instance;

        //�÷��̾� ü�� ���� ���� 0���� ����
        characterControls.healthMinus = 1;
        characterControls.curHealth = 20;
    }


    private void Update()
    {
        curTime += Time.deltaTime;


        if (curTime > maxTime)
        {
            //�ð� �ʱ�ȭ
            curTime = 0f;
            GameObject bullet = gameManager.CreateObj("PowerUpBullet", GameManager.PoolTypes.BulletType);

            //������Ʈ ����
            Rigidbody2D bulletRigid = bullet.GetComponent<Rigidbody2D>();
            Bullet bulletComponent = bullet.GetComponent<Bullet>();


            //� �߽�

            bullet.transform.parent = this.transform;
            bullet.transform.position = windPoints[0].position;

            //� Ȱ��ȭ
            bulletComponent.bulletOnRPC();

            //�ӵ� ����
            Vector2 bulletVec = Vector2.down;

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
