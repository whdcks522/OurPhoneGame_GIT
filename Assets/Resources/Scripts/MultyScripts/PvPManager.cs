using Assets.PixelHeroes.Scripts.ExampleScripts;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    public int loser = -2;
    public int maxPlayer;

    BattleUIManager battleUIManager;
    [Header("게임매니저")]
    public GameManager gameManager;

    public bool isMasterCilentLocal => PhotonNetwork.IsMasterClient && photonView.IsMine;
                         //->현재 이 컴퓨터가 호스트면서, 이 게임오브젝트가 호스트 측에서 생성됨

    private void Awake()
    {
        maxPlayer = PhotonNetwork.CurrentRoom.MaxPlayers;
        battleUIManager = BattleUIManager.Instance;
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
                        //무기 갯수 1개로
                        cc.photonView.RPC("changeStateRPC", RpcTarget.AllBuffered, CharacterControls.PlayerStateType.SwordCount, false);
                        //텍스트
                        cc.photonView.RPC("TypingRPC", RpcTarget.AllBuffered, CharacterControls.TypingType.None, "Fight!");
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
    //PhotonNetwork.PlayerList[]:배열로 하나 하나 접근
    //PhotonNetwork.CurrentRoom.Name: 현재 방 이름
    //PhotonNetwork.CurrentRoom.PlayerCount: 방에 있는 사람 수
    //PhotonNetwork.CurrentRoom.MaxPlayers: 방 최대 사람 수
}


