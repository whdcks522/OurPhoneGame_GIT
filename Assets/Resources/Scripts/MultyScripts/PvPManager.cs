using Assets.PixelHeroes.Scripts.ExampleScripts;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    

    BattleUIManager battleUIManager;
    [Header("게임매니저")]
    public GameManager gameManager;

    public bool isMasterCilentLocal => PhotonNetwork.IsMasterClient && photonView.IsMine;
                         //->현재 이 컴퓨터가 호스트면서, 이 게임오브젝트가 호스트 측에서 생성됨

    private void Awake()
    {
        battleUIManager = BattleUIManager.Instance;
    }

    private void Update()
    {
        if (isMasterCilentLocal &&
            PhotonNetwork.PlayerList.Length >= PhotonNetwork.CurrentRoom.MaxPlayers && 
            gameManager.playerGroup.childCount >= PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            if (!gameManager.alreadyStart)
            {
                Debug.Log("Activate");
                gameManager.alreadyStart = true;
                for (int i = 0; i < gameManager.playerGroup.childCount; i++)
                {
                    CharacterControls cc = gameManager.playerGroup.GetChild(i).GetComponent<CharacterControls>();
                    Debug.Log("gg:+ " + i);
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
        else //현재 대기 중일 때
        {
            if (!gameManager.alreadyStart) //맨 처음 시작하고 대기하고 있을 때
            {
                string str = PhotonNetwork.CurrentRoom.Name + '\n' +
                    PhotonNetwork.CurrentRoom.PlayerCount + '/' + PhotonNetwork.CurrentRoom.MaxPlayers;
                battleUIManager.typingControl(str);
            }
        }

        
        //PhotonNetwork.PlayerList[]:배열로 하나 하나 접근
        //PhotonNetwork.CurrentRoom.Name: 현재 방 이름
        //PhotonNetwork.CurrentRoom.PlayerCount: 방에 있는 사람 수
        //PhotonNetwork.CurrentRoom.MaxPlayers: 방 최대 사람 수
    }
}

