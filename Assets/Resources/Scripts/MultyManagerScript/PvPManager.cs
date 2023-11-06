using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PvPManager : MonoBehaviour
{
    [Header("운석 발생 지점")]
    public Transform[] starPoints;

    //합친 발사 시간
    public float sumTime;
    //최대 발사 시간
    public float maxTime;
    //현재 발사 시간
    float curTime;


    BattleUIManager battleUIManager;
    GameManager gameManager;
    GameObject player;

    private void Awake()
    {
        battleUIManager = BattleUIManager.Instance;
        gameManager = battleUIManager.gameManager;
        player = gameManager.player;

        //플레이어 점수 증가 비율 설정
        gameManager.characterControl.scorePlus = 1;
        gameManager.characterControl.healthMinus = 0;
    }

    private void Update()
    {
        curTime += Time.deltaTime;


        if (curTime > maxTime)
        {
            //시간 초기화
            curTime = 0f;

            foreach (Transform tmpTrans in starPoints) 
            {
                GameObject bullet = gameManager.CreateObj("YellowStarBullet", GameManager.PoolTypes.BulletType);

                //컴포넌트 정의
                Rigidbody bulletRigid = bullet.GetComponent<Rigidbody>();
                Bullet bulletComponent = bullet.GetComponent<Bullet>();

                bullet.transform.position = tmpTrans.position;

                //운석 활성화
                bulletComponent.bulletOnRPC();

                //속도 조정
                Vector2 bulletVec = ;

                //최종 속도 조정
                bulletRigid.velocity = Vector3.down * bulletComponent.bulletSpeed;

                //회전 조정
                bullet.transform.rotation = Quaternion.identity;
                float zValue = Mathf.Atan2(bulletRigid.velocity.x, bulletRigid.velocity.y) * 180 / Mathf.PI;
                Vector3 rotVec = Vector3.back * zValue + Vector3.back * 45.0f;
                bullet.transform.Rotate(rotVec);
            }
        }
    }
}

