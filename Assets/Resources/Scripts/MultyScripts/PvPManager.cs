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
    [Header("���ӸŴ���")]
    public GameManager gameManager;

    bool isMasterCilentLocal => PhotonNetwork.IsMasterClient && photonView.IsMine;
                         //->���� �� ��ǻ�Ͱ� ȣ��Ʈ�鼭, �� ���ӿ�����Ʈ�� ȣ��Ʈ ������ ������

    private void Awake()
    {
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;

        maxPlayer = PhotonNetwork.CurrentRoom.MaxPlayers;
    }


    private void Update()
    {
        if (isMasterCilentLocal)
        {
            if (PhotonNetwork.PlayerList.Length >= maxPlayer &&
                gameManager.playerGroup.childCount >= maxPlayer)//������ �÷��̾ ���� ���� �� �����ؾ���
            {
                //���� ����

                if (loser == -2)//-2: ������ �� ������ ���
                {
                    photonView.RPC("alreadyStartRPC", RpcTarget.AllBuffered);
                    for (int i = 0; i < gameManager.playerGroup.childCount; i++)
                    {
                        //���� Ȱ��ȭ
                        CharacterControls cc = gameManager.playerGroup.GetChild(i).GetComponent<CharacterControls>();
                        cc.photonView.RPC("changeStateRPC", RpcTarget.AllBuffered, CharacterControls.PlayerStateType.LeftControl, true);
                        cc.photonView.RPC("changeStateRPC", RpcTarget.AllBuffered, CharacterControls.PlayerStateType.RightControl, true);
                        //���� ���� 1����
                        cc.photonView.RPC("changeStateRPC", RpcTarget.AllBuffered, CharacterControls.PlayerStateType.SwordCount, false);
                        //���� �㰡
                        cc.photonView.RPC("changeStateRPC", RpcTarget.AllBuffered, CharacterControls.PlayerStateType.SwordFight, true);

                        //�ؽ�Ʈ
                        cc.GetComponent<PhotonView>().RPC("loopTypingRPC", RpcTarget.AllBuffered, CharacterControls.TypingType.None, "Fight!");

                    }
                }
                else if (loser != -2 && loser != 2)
                {
                    for (int i = 0; i < gameManager.playerGroup.childCount; i++)
                    {
                        CharacterControls cc = gameManager.playerGroup.GetChild(i).GetComponent<CharacterControls>();

                        if (loser == -1) //�й��� ����
                        {
                            if (cc.curHealth <= 0)
                            {
                                loser = i;
                            }
                        }
                        else if (loser != -1)
                        {
                            
                            if (i == loser) //�й������� �й� �޽��� ����
                            {
                                //�ؽ�Ʈ
                                cc.GetComponent<PhotonView>().RPC("loopTypingRPC", RpcTarget.AllBuffered, CharacterControls.TypingType.Lose, "Lose");
                            }
                            else
                            {
                                //�ؽ�Ʈ
                                cc.GetComponent<PhotonView>().RPC("loopTypingRPC", RpcTarget.AllBuffered, CharacterControls.TypingType.Win, "WIN");
                            }
                            loser = 2;
                        }
                    }
                }
            }
        }//���� �� ��,

        //������ �ƴϿ���
        if (loser == -2 && !isStart)
        {
            isStart = true;

            string str = PhotonNetwork.CurrentRoom.Name + '\n' +
                    PhotonNetwork.CurrentRoom.PlayerCount + '/' + PhotonNetwork.CurrentRoom.MaxPlayers;


            CharacterControls cc = gameManager.playerGroup.GetChild(0).GetComponent<CharacterControls>();
            cc.loopTypingRPC(CharacterControls.TypingType.None, str);
        }

        //�����ϰ� ���� Ż���ϴ� ���
        if (loser != -2 && !(PhotonNetwork.PlayerList.Length >= maxPlayer && gameManager.playerGroup.childCount >= maxPlayer))
        {
            gameManager.allLeaveRoomStart();
        }

    }//Update��
    [PunRPC]
    public void alreadyStartRPC() //���� ����
    {
        loser = -1;
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        gameManager.allLeaveRoomStart();
    }
}


