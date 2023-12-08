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
                bulletName = "NormalBullet";//노랑
                break;
            case Bullet.BulletEffectType.PowerUp:
                bulletName = "PowerUpBullet";//파랑
                break;
            case Bullet.BulletEffectType.UnBreakable:
                bulletName = "UnBreakableBullet";//빨강
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

    #region 직선 발사
    IEnumerator directionShot(string bulletName, GameObject host, GameObject target, int index)
    {
        //0이면 직선
        //1이면 궤도 변화 추가
        yield return null;

        //운석 오브젝트 생성
        GameObject bullet = gameManager.CreateObj(bulletName, GameManager.PoolTypes.BulletType);

        //컴포넌트 정의
        Rigidbody2D bulletRigid = bullet.GetComponent<Rigidbody2D>();
        Bullet bulletComponent = bullet.GetComponent<Bullet>();

        //위치 조정
        bullet.transform.position = host.transform.position;

        //운석 활성화
        bulletComponent.bulletOnRPC();

        //방향 조정
        Vector2 bulletVec = (target.transform.position - bullet.transform.position).normalized;

        //약간의 궤도 변화
        if (index == 1)
        {
            bulletVec += 0.1f * Random.insideUnitCircle;//반지름이 1인 원 안에서 랜덤 벡터2 좌표 찍어줌
            bulletVec = bulletVec.normalized;
        }

        //최종 속도 조정
        bulletRigid.velocity = bulletVec * bulletComponent.bulletSpeed;

        //회전 조정
        //bullet.transform.rotation = Quaternion.identity;
        //float zValue = Mathf.Atan2(bulletRigid.velocity.x, bulletRigid.velocity.y) * 180 / Mathf.PI;
        //Vector3 rotVec = Vector3.back * zValue + Vector3.back * 45.0f;
        //bullet.transform.Rotate(rotVec);
    }
    #endregion

    #region 산탄 발사
    IEnumerator bigShot(string bulletName, GameObject host, GameObject target, int index)
    {
        //0이면 작게
        //1이면 크게

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

            //운석 오브젝트 생성
            GameObject bullet = gameManager.CreateObj(bulletName, GameManager.PoolTypes.BulletType);

            //컴포넌트 정의
            Rigidbody2D bulletRigid = bullet.GetComponent<Rigidbody2D>();
            Bullet bulletComponent = bullet.GetComponent<Bullet>();

            //위치 조정
            bullet.transform.position = host.transform.position;

            //운석 활성화
            bulletComponent.bulletOnRPC();

            //최종 속도 조정
            bulletRigid.velocity = bulletVec * bulletComponent.bulletSpeed;
        }

        tmpVec = new Vector2(mainVec.y, -mainVec.x);
        for (float i = interval * 5; i > 0.0f; i -= interval)
        {
            Vector2 bulletVec = Vector2.Lerp(mainVec, tmpVec, i).normalized;

            //운석 오브젝트 생성
            GameObject bullet = gameManager.CreateObj(bulletName, GameManager.PoolTypes.BulletType);

            //컴포넌트 정의
            Rigidbody2D bulletRigid = bullet.GetComponent<Rigidbody2D>();
            Bullet bulletComponent = bullet.GetComponent<Bullet>();

            //위치 조정
            bullet.transform.position = host.transform.position;

            //운석 활성화
            bulletComponent.bulletOnRPC();

            //최종 속도 조정
            bulletRigid.velocity = bulletVec * bulletComponent.bulletSpeed;
        }
    }
    #endregion

    #region 흩뿌리기 발사
    IEnumerator scatterShot(string bulletName, GameObject host, GameObject target, int index)
    {
        //0이면 직선
        //1이면 궤도 변화 추가
        yield return null;

        Vector2 mainVec = (target.transform.position - host.transform.position).normalized;
        int interval = 13;
        float spinLate = 25;//회전률, 한쪽만
        WaitForSeconds wait_01 = new WaitForSeconds(0.1f);

        while (index-- > 0)
        {
            for (int i = 0; i < interval; i++)
            {
                //운석 오브젝트 생성
                GameObject bullet = gameManager.CreateObj(bulletName, GameManager.PoolTypes.BulletType);

                //컴포넌트 정의
                Rigidbody2D bulletRigid = bullet.GetComponent<Rigidbody2D>();
                Bullet bulletComponent = bullet.GetComponent<Bullet>();

                //위치 조정
                bullet.transform.position = host.transform.position;

                //운석 활성화
                bulletComponent.bulletOnRPC();

                // 방향 조정
                float angle = spinLate * Mathf.Sin(Mathf.PI * 2f * i / interval); // 균일한 각도 계산
                Vector2 bulletVec = Quaternion.Euler(0, 0, angle) * mainVec;

                //최종 속도 조정
                bulletRigid.velocity = bulletVec * bulletComponent.bulletSpeed;

                yield return wait_01;
            }
        }
    }
    #endregion

    #region 난사 발사
    IEnumerator genocideShot(string bulletName, GameObject host, GameObject target, int index)
    {
        yield return null;

        for (int i = 0; i < index; i++)
        {
            //운석 오브젝트 생성
            GameObject bullet = gameManager.CreateObj(bulletName, GameManager.PoolTypes.BulletType);

            //컴포넌트 정의
            Rigidbody2D bulletRigid = bullet.GetComponent<Rigidbody2D>();
            Bullet bulletComponent = bullet.GetComponent<Bullet>();

            //위치 조정
            bullet.transform.position = host.transform.position;

            //운석 활성화
            bulletComponent.bulletOnRPC();

            Vector2 tmpVec = new Vector2(Mathf.Sin(Mathf.PI * i / index * 2.0f), Mathf.Cos(Mathf.PI * i / index * 2.0f));

            //최종 속도 조정
            bulletRigid.velocity = tmpVec.normalized * bulletComponent.bulletSpeed;
        }
    }
    #endregion

    

    #region 회전 발사
    IEnumerator spinShot(string bulletName, GameObject host, GameObject target, int index)
    {
        //index는 반복 횟수
        yield return null;

        int interval = 21;
        Vector2 mainVec = (target.transform.position - host.transform.position).normalized;
        WaitForSeconds wait_01 = new WaitForSeconds(0.1f);

        while (index-- > 0) 
        {
            for (int i = 0; i < interval; i++)
            {
                //운석 오브젝트 생성
                GameObject bullet = gameManager.CreateObj(bulletName, GameManager.PoolTypes.BulletType);

                //컴포넌트 정의
                Rigidbody2D bulletRigid = bullet.GetComponent<Rigidbody2D>();
                Bullet bulletComponent = bullet.GetComponent<Bullet>();

                //위치 조정
                bullet.transform.position = host.transform.position;

                //운석 활성화
                bulletComponent.bulletOnRPC();

                // 방향 조정: mainVec 방향으로 흩뿌리기
                float angle = i * (360f / interval); // 균일한 각도 계산
                Vector2 bulletVec = Quaternion.Euler(0, 0, angle) * mainVec;

                //최종 속도 조정
                bulletRigid.velocity = bulletVec * bulletComponent.bulletSpeed;

                yield return wait_01;
            }
        }   
    }
    #endregion
}


