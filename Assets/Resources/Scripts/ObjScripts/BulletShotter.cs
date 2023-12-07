using Photon.Pun.Demo.PunBasics;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletShotter : MonoBehaviour
{
    public GameManager gameManager;

    public enum BulletShotType 
    {
        Direction, Scatter
    }

    public void sortShot(BulletShotType bulletShotType, Bullet.BulletEffectType bulletEffectType, GameObject host, GameObject target, int version)
    {
        string bulletName = "";
        switch (bulletEffectType) 
        {
            case Bullet.BulletEffectType.Normal:
                bulletName = "NormalBullet";
                break;
            case Bullet.BulletEffectType.PowerUp:
                bulletName = "PowerUpBullet";
                break;
        }

        switch (bulletShotType) 
        {
            case BulletShotType.Direction:
                StartCoroutine(directionShot(bulletName, host, target, version));
                break;
        }
    }

    IEnumerator directionShot(string bulletName, GameObject host, GameObject target, int version)
    {
        //0�̸� ����
        //1�̸� �˵� ��ȭ �߰�
        yield return null;

        //� ������Ʈ ����
        GameObject bullet = gameManager.CreateObj(bulletName, GameManager.PoolTypes.BulletType);

        //������Ʈ ����
        Rigidbody2D bulletRigid = bullet.GetComponent<Rigidbody2D>();
        Bullet bulletComponent = bullet.GetComponent<Bullet>();

        //��ġ ����
        bullet.transform.position = host.transform.position;

        //� Ȱ��ȭ
        bulletComponent.bulletOnRPC();

        //���� ����
        Vector2 bulletVec = (target.transform.position - bullet.transform.position).normalized;

        //�ణ�� �˵� ��ȭ
        if (version == 1)
        {
            bulletVec += 0.1f * Random.insideUnitCircle;//�������� 1�� �� �ȿ��� ���� ����2 ��ǥ �����
            bulletVec = bulletVec.normalized;
        }

        //���� �ӵ� ����
        bulletRigid.velocity = bulletVec * bulletComponent.bulletSpeed;

        //ȸ�� ����
        bullet.transform.rotation = Quaternion.identity;
        float zValue = Mathf.Atan2(bulletRigid.velocity.x, bulletRigid.velocity.y) * 180 / Mathf.PI;
        Vector3 rotVec = Vector3.back * zValue + Vector3.back * 45.0f;
        bullet.transform.Rotate(rotVec);
    }
}
