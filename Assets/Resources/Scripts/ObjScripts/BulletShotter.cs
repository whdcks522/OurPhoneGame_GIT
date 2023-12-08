using Photon.Pun.Demo.PunBasics;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class BulletShotter : MonoBehaviour
{
    public GameManager gameManager;

    public enum BulletShotType 
    {
        Direction, Big, Genocide, Scatter, Spin
    }

    public void sortShot(BulletShotType bulletShotType, Bullet.BulletEffectType bulletEffectType, GameObject host, GameObject target, int index)
    {
        string bulletName = "";
        switch (bulletEffectType) 
        {
            case Bullet.BulletEffectType.Normal:
                bulletName = "NormalBullet";//���
                break;
            case Bullet.BulletEffectType.PowerUp:
                bulletName = "PowerUpBullet";//�Ķ�
                break;
            case Bullet.BulletEffectType.UnBreakable:
                bulletName = "UnBreakableBullet";//����
                break;
        }

        switch (bulletShotType) 
        {
            case BulletShotType.Direction:
                StartCoroutine(directionShot(bulletName, host, target, index));
                break;
            case BulletShotType.Big:
                StartCoroutine(bigShot(bulletName, host, target, index));
                break;
            case BulletShotType.Scatter:
                StartCoroutine(scatterShot(bulletName, host, target, index));
                break;
            case BulletShotType.Genocide:
                StartCoroutine(genocideShot(bulletName, host, target, index));
                break;
            case BulletShotType.Spin:
                StartCoroutine(spinShot(bulletName, host, target, index));
                break;
        }
    }

    #region ���� �߻�
    IEnumerator directionShot(string bulletName, GameObject host, GameObject target, int index)
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
        if (index == 1)
        {
            bulletVec += 0.1f * Random.insideUnitCircle;//�������� 1�� �� �ȿ��� ���� ����2 ��ǥ �����
            bulletVec = bulletVec.normalized;
        }

        //���� �ӵ� ����
        bulletRigid.velocity = bulletVec * bulletComponent.bulletSpeed;

        //ȸ�� ����
        //bullet.transform.rotation = Quaternion.identity;
        //float zValue = Mathf.Atan2(bulletRigid.velocity.x, bulletRigid.velocity.y) * 180 / Mathf.PI;
        //Vector3 rotVec = Vector3.back * zValue + Vector3.back * 45.0f;
        //bullet.transform.Rotate(rotVec);
    }
    #endregion

    #region ��ź �߻�
    IEnumerator bigShot(string bulletName, GameObject host, GameObject target, int index)
    {
        //0�̸� �۰�
        //1�̸� ũ��

        yield return null;

        float interval = 0.03f;
        if (index == 0)
            interval = 0.03f;
        if (index == 1)
            interval = 0.200f;

        Vector2 mainVec = (target.transform.position - host.transform.position).normalized;
        Vector2 tmpVec = new Vector2(-mainVec.y, mainVec.x);

        for (float i = 0f; i <= interval * 5; i += interval)
        {
            Vector2 bulletVec = Vector2.Lerp(mainVec, tmpVec, i).normalized;

            //� ������Ʈ ����
            GameObject bullet = gameManager.CreateObj(bulletName, GameManager.PoolTypes.BulletType);

            //������Ʈ ����
            Rigidbody2D bulletRigid = bullet.GetComponent<Rigidbody2D>();
            Bullet bulletComponent = bullet.GetComponent<Bullet>();

            //��ġ ����
            bullet.transform.position = host.transform.position;

            //� Ȱ��ȭ
            bulletComponent.bulletOnRPC();

            //���� �ӵ� ����
            bulletRigid.velocity = bulletVec * bulletComponent.bulletSpeed;
        }

        tmpVec = new Vector2(mainVec.y, -mainVec.x);
        for (float i = interval * 5; i > 0.0f; i -= interval)
        {
            Vector2 bulletVec = Vector2.Lerp(mainVec, tmpVec, i).normalized;

            //� ������Ʈ ����
            GameObject bullet = gameManager.CreateObj(bulletName, GameManager.PoolTypes.BulletType);

            //������Ʈ ����
            Rigidbody2D bulletRigid = bullet.GetComponent<Rigidbody2D>();
            Bullet bulletComponent = bullet.GetComponent<Bullet>();

            //��ġ ����
            bullet.transform.position = host.transform.position;

            //� Ȱ��ȭ
            bulletComponent.bulletOnRPC();

            //���� �ӵ� ����
            bulletRigid.velocity = bulletVec * bulletComponent.bulletSpeed;
        }
    }
    #endregion

    #region ��Ѹ��� �߻�
    IEnumerator scatterShot(string bulletName, GameObject host, GameObject target, int index)
    {
        //0�̸� ����
        //1�̸� �˵� ��ȭ �߰�
        yield return null;

        Vector2 mainVec = (target.transform.position - host.transform.position).normalized;
        int interval = 13;
        float spinLate = 25;//ȸ����, ���ʸ�
        WaitForSeconds wait_01 = new WaitForSeconds(0.1f);

        while (index-- > 0)
        {
            for (int i = 0; i < interval; i++)
            {
                //� ������Ʈ ����
                GameObject bullet = gameManager.CreateObj(bulletName, GameManager.PoolTypes.BulletType);

                //������Ʈ ����
                Rigidbody2D bulletRigid = bullet.GetComponent<Rigidbody2D>();
                Bullet bulletComponent = bullet.GetComponent<Bullet>();

                //��ġ ����
                bullet.transform.position = host.transform.position;

                //� Ȱ��ȭ
                bulletComponent.bulletOnRPC();

                // ���� ����
                float angle = spinLate * Mathf.Sin(Mathf.PI * 2f * i / interval); // ������ ���� ���
                Vector2 bulletVec = Quaternion.Euler(0, 0, angle) * mainVec;

                //���� �ӵ� ����
                bulletRigid.velocity = bulletVec * bulletComponent.bulletSpeed;

                yield return wait_01;
            }
        }
    }
    #endregion

    #region ���� �߻�
    IEnumerator genocideShot(string bulletName, GameObject host, GameObject target, int index)
    {
        yield return null;

        for (int i = 0; i < index; i++)
        {
            //� ������Ʈ ����
            GameObject bullet = gameManager.CreateObj(bulletName, GameManager.PoolTypes.BulletType);

            //������Ʈ ����
            Rigidbody2D bulletRigid = bullet.GetComponent<Rigidbody2D>();
            Bullet bulletComponent = bullet.GetComponent<Bullet>();

            //��ġ ����
            bullet.transform.position = host.transform.position;

            //� Ȱ��ȭ
            bulletComponent.bulletOnRPC();

            Vector2 tmpVec = new Vector2(Mathf.Sin(Mathf.PI * i / index * 2.0f), Mathf.Cos(Mathf.PI * i / index * 2.0f));

            //���� �ӵ� ����
            bulletRigid.velocity = tmpVec.normalized * bulletComponent.bulletSpeed;
        }
    }
    #endregion

    

    #region ȸ�� �߻�
    IEnumerator spinShot(string bulletName, GameObject host, GameObject target, int index)
    {
        //index�� �ݺ� Ƚ��
        yield return null;

        int interval = 21;
        Vector2 mainVec = (target.transform.position - host.transform.position).normalized;
        WaitForSeconds wait_01 = new WaitForSeconds(0.1f);

        while (index-- > 0) 
        {
            for (int i = 0; i < interval; i++)
            {
                //� ������Ʈ ����
                GameObject bullet = gameManager.CreateObj(bulletName, GameManager.PoolTypes.BulletType);

                //������Ʈ ����
                Rigidbody2D bulletRigid = bullet.GetComponent<Rigidbody2D>();
                Bullet bulletComponent = bullet.GetComponent<Bullet>();

                //��ġ ����
                bullet.transform.position = host.transform.position;

                //� Ȱ��ȭ
                bulletComponent.bulletOnRPC();

                // ���� ����: mainVec �������� ��Ѹ���
                float angle = i * (360f / interval); // ������ ���� ���
                Vector2 bulletVec = Quaternion.Euler(0, 0, angle) * mainVec;

                //���� �ӵ� ����
                bulletRigid.velocity = bulletVec * bulletComponent.bulletSpeed;

                yield return wait_01;
            }
        }   
    }
    #endregion
}


