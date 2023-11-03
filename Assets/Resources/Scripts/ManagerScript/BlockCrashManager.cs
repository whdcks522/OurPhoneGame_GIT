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
    [Header("운석 발생 지점")]
    public Transform[] blockPoints;

    [Header("최대 발사 시간")]
    public float maxTime;
    //현재 발사 시간
    float curTime;

    [Header("파워업 투사체 장소")]
    public Transform PowerUpPos;
    [Header("파워업 주기")]
    public int maxPowerUpindex;
    //현재 파워업 변수
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
            //시간 초기화
            curTime = 0f;

            //사출 위치 정하기
            int ranPos = Random.Range(0, blockPoints.Length);


            GameObject block = gameManager.CreateObj("NormalBlock", GameManager.PoolTypes.BlockType);
            Block blockComponent = block.GetComponent<Block>();

            //블록 부모 조정
            block.transform.parent = this.transform;
            blockComponent.createPos = blockPoints[ranPos].position;

            //블록 활성화
            blockComponent.blockOnRPC();

            curPowerUpindex++;
            if (curPowerUpindex >= maxPowerUpindex) 
            {
                curPowerUpindex = 0;

                //투사체 생성
                GameObject bullet = gameManager.CreateObj("GreenStarBullet", GameManager.PoolTypes.BulletType);

                //컴포넌트 정의
                Rigidbody bulletRigid = bullet.GetComponent<Rigidbody>();
                Bullet bulletComponent = bullet.GetComponent<Bullet>();

                //위치 조정
                bullet.transform.parent = this.transform;
                bullet.transform.position = PowerUpPos.position;

                //운석 활성화
                bulletComponent.bulletOnRPC();

                //속도 조정
                Vector2 bulletVec = (player.transform.position - bullet.transform.position).normalized;

                //최종 속도 조정
                bulletRigid.velocity = bulletVec * bulletComponent.bulletSpeed;

                //회전 조정
                bullet.transform.rotation = Quaternion.identity;
                float zValue = Mathf.Atan2(bulletRigid.velocity.x, bulletRigid.velocity.y) * 180 / Mathf.PI;
                Vector3 rotVec = Vector3.back * zValue + Vector3.back * 45.0f;
                bullet.transform.Rotate(rotVec);
            }
        }
    }


}
