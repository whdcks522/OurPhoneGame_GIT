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
        if (version == 1)
        {
            bulletVec += 0.1f * Random.insideUnitCircle;//반지름이 1인 원 안에서 랜덤 벡터2 좌표 찍어줌
            bulletVec = bulletVec.normalized;
        }

        //최종 속도 조정
        bulletRigid.velocity = bulletVec * bulletComponent.bulletSpeed;

        //회전 조정
        bullet.transform.rotation = Quaternion.identity;
        float zValue = Mathf.Atan2(bulletRigid.velocity.x, bulletRigid.velocity.y) * 180 / Mathf.PI;
        Vector3 rotVec = Vector3.back * zValue + Vector3.back * 45.0f;
        bullet.transform.Rotate(rotVec);
    }
}
