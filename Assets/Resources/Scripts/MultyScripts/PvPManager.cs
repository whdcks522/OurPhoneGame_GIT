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
    [Header("� �߻� ����")]
    public Transform[] starPoints;

    //�� ��ģ �߻� �ð�
    float sumTime;
    //�ִ� �߻� �ð�
    public float maxTime;
    //���� �߻� �ð�
    float curTime;

    public int loser = -2;
    public int maxPlayer;

    BattleUIManager battleUIManager;
    [Header("���ӸŴ���")]
    public GameManager gameManager;

    public bool isMasterCilentLocal => PhotonNetwork.IsMasterClient && photonView.IsMine;
                         //->���� �� ��ǻ�Ͱ� ȣ��Ʈ�鼭, �� ���ӿ�����Ʈ�� ȣ��Ʈ ������ ������

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
                        //�ؽ�Ʈ
                        cc.photonView.RPC("TypingRPC", RpcTarget.AllBuffered, CharacterControls.TypingType.None, "Fight!");
                    }
                }
                else if (loser != -2)
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
        }//���� �� ��,

        //������ �ƴϿ���
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
        //�����ϰ� ���� Ż���ϴ� ���
        else if (loser != -2 && !(PhotonNetwork.PlayerList.Length >= maxPlayer && gameManager.playerGroup.childCount >= maxPlayer))
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
    //PhotonNetwork.PlayerList[]:�迭�� �ϳ� �ϳ� ����
    //PhotonNetwork.CurrentRoom.Name: ���� �� �̸�
    //PhotonNetwork.CurrentRoom.PlayerCount: �濡 �ִ� ��� ��
    //PhotonNetwork.CurrentRoom.MaxPlayers: �� �ִ� ��� ��
}


