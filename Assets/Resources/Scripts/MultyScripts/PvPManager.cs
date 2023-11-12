using Assets.PixelHeroes.Scripts.ExampleScripts;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PvPManager : MonoBehaviourPunCallbacks
{
    [Header("운석 발생 지점")]
    public Transform[] starPoints;

    //총 합친 발사 시간
    float sumTime;
    //최대 발사 시간
    public float maxTime;
    //현재 발사 시간
    float curTime;

    bool isFirst = true;

    BattleUIManager battleUIManager;
    [Header("게임매니저")]
    public GameManager gameManager;
    GameObject player;

    public bool isMasterCilentLocal => PhotonNetwork.IsMasterClient && photonView.IsMine;
                         //->현재 이 컴퓨터가 호스트면서, 이 게임오브젝트가 호스트 측에서 생성됨

    private void Awake()
    {
        battleUIManager = BattleUIManager.Instance;
        player = gameManager.player;

        //플레이어 점수 증가 비율 설정
        //gameManager.characterControl.scorePlus = 0;
        //플레이어 체력 감소 비율 설정
        //gameManager.characterControl.healthMinus = 0;
    }

    private void Update()
    {
        if (isMasterCilentLocal && PhotonNetwork.PlayerList.Length >= 2)
        {
            if (isFirst) 
            {
                isFirst = false;
                for (int i = 0; i < gameManager.playerGroup.childCount; i++)
                {
                    CharacterControls cc = gameManager.playerGroup.GetChild(i).GetComponent<CharacterControls>();

                    cc.photonView.RPC("changeStateRPC", RpcTarget.AllBuffered, CharacterControls.PlayerStateType.LeftControl, true);
                    cc.photonView.RPC("changeStateRPC", RpcTarget.AllBuffered, CharacterControls.PlayerStateType.IsCanJump, true);
                    cc.photonView.RPC("changeStateRPC", RpcTarget.AllBuffered, CharacterControls.PlayerStateType.RightControl, true);
                }
            }
            

            return;

            curTime += Time.deltaTime;


            if (curTime > maxTime)
            {
                sumTime += curTime;
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

                    //운석 속도 조정
                    bulletRigid.velocity = Vector3.down * bulletComponent.bulletSpeed;

                    //운석 회전 조정
                    bullet.transform.rotation = Quaternion.identity;
                    float zValue = Mathf.Atan2(bulletRigid.velocity.x, bulletRigid.velocity.y) * 180 / Mathf.PI;
                    Vector3 rotVec = Vector3.back * zValue + Vector3.back * 45.0f;
                    bullet.transform.Rotate(rotVec);
                }
            }
        }
    }
}

