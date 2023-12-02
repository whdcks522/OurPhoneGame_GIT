using Assets.PixelHeroes.Scripts.ExampleScripts;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PvPManager : MonoBehaviourPunCallbacks
{
    int loser = -2;
    int maxPlayer;
    bool isStart = false;
    [Header("게임매니저")]
    public GameManager gameManager;

    bool isMasterCilentLocal => PhotonNetwork.IsMasterClient && photonView.IsMine;
                         //->현재 이 컴퓨터가 호스트면서, 이 게임오브젝트가 호스트 측에서 생성됨

    BattleUIManager battleUIManager;
    private void Awake()
    {
        PhotonNetwork.SendRate = 10;
        PhotonNetwork.SerializationRate = 5;

        battleUIManager = BattleUIManager.Instance;

        maxPlayer = PhotonNetwork.CurrentRoom.MaxPlayers;
    }

    [PunRPC]
    void textRPC() 
    {
        battleUIManager.typingControl("Fight");
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
                        //전투 허가
                        cc.photonView.RPC("changeStateRPC", RpcTarget.AllBuffered, CharacterControls.PlayerStateType.SwordFight, true);

                        //텍스트
                        //cc.GetComponent<PhotonView>().RPC("loopTypingRPC", RpcTarget.AllBuffered, CharacterControls.TypingType.None, "Fight!");
                        //photonView.RPC("textRPC", RpcTarget.AllBuffered);
                        cc.GetComponent<PhotonView>().RPC("loopTypingRPC", RpcTarget.AllBuffered, CharacterControls.TypingType.None, "Fight!", false);

                    }
                }
                else if (loser != -2 && loser != 2)
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
                        else if (loser != -1 && loser != 2)
                        {
                            
                            if (i == loser) //패배자한테 패배 메시지 전송
                            {
                                //텍스트
                                //cc.GetComponent<PhotonView>().RPC("loopTypingRPC", RpcTarget.AllBuffered, CharacterControls.TypingType.Lose, "Lose", true);
                                cc.GetComponent<PhotonView>().RPC("loopTypingRPC", RpcTarget.AllBuffered, CharacterControls.TypingType.Win, "WIN", true);
                            }
                            else
                            {
                                //텍스트
                                cc.GetComponent<PhotonView>().RPC("loopTypingRPC", RpcTarget.AllBuffered, CharacterControls.TypingType.Win, "WIN", true);
                            }
                            loser = 2;
                        }
                    }
                }
            }
        }//방장 일 때,

        //방장이 아니여도
        if (loser == -2 && !isStart)
        {
            isStart = true;

            string str = PhotonNetwork.CurrentRoom.Name + '\n' +
                    PhotonNetwork.CurrentRoom.PlayerCount + '/' + PhotonNetwork.CurrentRoom.MaxPlayers;

            CharacterControls cc = gameManager.playerGroup.GetChild(0).GetComponent<CharacterControls>();
            cc.loopTypingRPC(CharacterControls.TypingType.None, str, true);
        }

        //시작하고 나서 탈주하는 경우
        if (loser != -2 && !(PhotonNetwork.PlayerList.Length >= maxPlayer))
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
}


