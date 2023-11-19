using Assets.PixelHeroes.Scripts.ExampleScripts;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggCollectManager : MonoBehaviourPunCallbacks
{
    [Header("계란 발생 지점")]
    public Transform eggPoint;

    //최대 발사 시간
    float maxTime;
    //현재 발사 시간
    float curTime;

    int LeftScore = 0;
    int RightScore = 0;

    int loser = -2;
    int maxPlayer;

    [Header("게임매니저")]
    public GameManager gameManager;

    public bool isMasterCilentLocal => PhotonNetwork.IsMasterClient && photonView.IsMine;
    //->현재 이 컴퓨터가 호스트면서, 이 게임오브젝트가 호스트 측에서 생성됨

    private void Awake()
    {
        maxPlayer = PhotonNetwork.CurrentRoom.MaxPlayers;

        Invoke("CreateEgg", 1.5f);
    }

    void CreateEgg() //계란 생성
    {
        //계란 생성
        GameObject egg = gameManager.CreateObj("Egg", GameManager.PoolTypes.ObjType);
        egg.transform.position = eggPoint.position;

        Debug.Log("Create");
    }


    private void Update()
    {
        if (isMasterCilentLocal)
        {
            if (PhotonNetwork.PlayerList.Length >= maxPlayer &&
                gameManager.playerGroup.childCount >= maxPlayer)//실제로 플레이어도 생성 됐을 때 시작해야함
            {
                //게임 시작

                if (loser == -2)//-2: 게임을 막 시작한 경우
                {
                    photonView.RPC("alreadyStartRPC", RpcTarget.AllBuffered);
                    for (int i = 0; i < gameManager.playerGroup.childCount; i++)
                    {
                        //조작 활성화
                        CharacterControls cc = gameManager.playerGroup.GetChild(i).GetComponent<CharacterControls>();
                        cc.photonView.RPC("changeStateRPC", RpcTarget.AllBuffered, CharacterControls.PlayerStateType.LeftControl, true);
                        cc.photonView.RPC("changeStateRPC", RpcTarget.AllBuffered, CharacterControls.PlayerStateType.RightControl, true);
                        //무기 갯수 8개로
                        cc.photonView.RPC("changeStateRPC", RpcTarget.AllBuffered, CharacterControls.PlayerStateType.SwordCount, false);

                        //텍스트
                        cc.photonView.RPC("TypingRPC", RpcTarget.AllBuffered, CharacterControls.TypingType.None, "Egg!");
                    }
                }
                else if (loser != -2)
                {
                    for (int i = 0; i < gameManager.playerGroup.childCount; i++)
                    {
                        CharacterControls cc = gameManager.playerGroup.GetChild(i).GetComponent<CharacterControls>();

                        if (loser == -1) //패배자 결정
                        {
                            if (cc.curHealth <= 0)
                            {
                                loser = i;
                            }
                        }
                        else if (loser != -1)
                        {
                            if (i == loser) //패배자한테 패배 메시지 전송
                            {
                                cc.photonView.RPC("TypingRPC", RpcTarget.AllBuffered, CharacterControls.TypingType.Lose, "lose");
                            }
                            else
                            {
                                cc.photonView.RPC("TypingRPC", RpcTarget.AllBuffered, CharacterControls.TypingType.Win, "win");
                            }
                        }
                    }
                }
            }
        }//방장 일 때,

        //방장이 아니여도
        if (loser == -2)
        {
            for (int i = 0; i < gameManager.playerGroup.childCount; i++)
            {
                string str = PhotonNetwork.CurrentRoom.Name + '\n' +
                    PhotonNetwork.CurrentRoom.PlayerCount + '/' + PhotonNetwork.CurrentRoom.MaxPlayers;

                CharacterControls cc = gameManager.playerGroup.GetChild(i).GetComponent<CharacterControls>();
                cc.TypingRPC(CharacterControls.TypingType.None, str);
            }
        }
        //시작하고 나서 탈주하는 경우
        else if (loser != -2 && !(PhotonNetwork.PlayerList.Length >= maxPlayer && gameManager.playerGroup.childCount >= maxPlayer))
        {
            gameManager.allLeaveRoomStart();
        }

    }//Update문
    [PunRPC]
    public void alreadyStartRPC() //시작 선언
    {
        loser = -1;
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        gameManager.allLeaveRoomStart();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isMasterCilentLocal) //계란 삽입
        {
            if (collision.gameObject.CompareTag("Egg")) 
            {
                Debug.Log(gameObject.name);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isMasterCilentLocal) //계란 삽입
        {
            if (collision.gameObject.CompareTag("Egg"))
            {
                Debug.Log(gameObject.name);
            }
        }
    }
}
