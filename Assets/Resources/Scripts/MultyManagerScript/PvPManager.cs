using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PvPManager : MonoBehaviour
{
    [Header("� �߻� ����")]
    public Transform[] starPoints;

    //��ģ �߻� �ð�
    public float sumTime;
    //�ִ� �߻� �ð�
    public float maxTime;
    //���� �߻� �ð�
    float curTime;


    BattleUIManager battleUIManager;
    GameManager gameManager;
    GameObject player;

    private void Awake()
    {
        battleUIManager = BattleUIManager.Instance;
        gameManager = battleUIManager.gameManager;
        player = gameManager.player;

        //�÷��̾� ���� ���� ���� ����
        gameManager.characterControl.scorePlus = 1;
        gameManager.characterControl.healthMinus = 0;
    }

    private void Update()
    {
        curTime += Time.deltaTime;


        if (curTime > maxTime)
        {
            //�ð� �ʱ�ȭ
            curTime = 0f;

            foreach (Transform tmpTrans in starPoints) 
            {
                GameObject bullet = gameManager.CreateObj("YellowStarBullet", GameManager.PoolTypes.BulletType);

                //������Ʈ ����
                Rigidbody bulletRigid = bullet.GetComponent<Rigidbody>();
                Bullet bulletComponent = bullet.GetComponent<Bullet>();

                bullet.transform.position = tmpTrans.position;

                //� Ȱ��ȭ
                bulletComponent.bulletOnRPC();

                //�ӵ� ����
                Vector2 bulletVec = ;

                //���� �ӵ� ����
                bulletRigid.velocity = Vector3.down * bulletComponent.bulletSpeed;

                //ȸ�� ����
                bullet.transform.rotation = Quaternion.identity;
                float zValue = Mathf.Atan2(bulletRigid.velocity.x, bulletRigid.velocity.y) * 180 / Mathf.PI;
                Vector3 rotVec = Vector3.back * zValue + Vector3.back * 45.0f;
                bullet.transform.Rotate(rotVec);
            }
        }
    }
}

