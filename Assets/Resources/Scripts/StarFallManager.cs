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
            //시간 초기화
            curTime = 0f;


            //사출 위치 정하기
            int ranPos = Random.Range(0, maxAttackIndex);
            //같은 곳 연속으로 안되도록 설정
            if (ranPos == recentPos) 
                ranPos = (ranPos + 1) / maxAttackIndex;
            recentPos = ranPos;


            //운석 오브젝트 생성
            GameObject bullet = gameManager.CreateObj("YellowStarBullet", GameManager.PoolTypes.BulletType);
            //컴포넌트 정의
            Rigidbody bulletRigid = bullet.GetComponent<Rigidbody>();
            Bullet bulletComponent = bullet.GetComponent<Bullet>();

            //위치 조정
            bullet.transform.position = starFallPoints[ranPos].position;
            bullet.transform.parent = this.transform;

            //운석 활성화
            bulletComponent.bulletOnRPC();

            //속도 조정
            Vector2 bulletVec = (player.transform.position - bullet.transform.position).normalized;

            //약간의 궤도 변화
            bulletVec += 0.1f * Random.insideUnitCircle;//반지름이 1인 원 안에서 랜덤 벡터2 좌표 찍어줌
            bulletVec = bulletVec.normalized;

            //최종 속도 조정
            bulletRigid.velocity = bulletVec * bulletComponent.bulletSpeed;

            //회전 조정
            float zValue = Mathf.Atan2(bulletRigid.velocity.x, bulletRigid.velocity.y) * 180 / Mathf.PI;
            Vector3 rotVec = Vector3.back * zValue + Vector3.back * 45.0f;
            bullet.transform.Rotate(rotVec); 
        }
    }
}
