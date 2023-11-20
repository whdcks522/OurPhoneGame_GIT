using Assets.PixelHeroes.Scripts.ExampleScripts;
using KoreanTyper;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EggCollectManager : MonoBehaviourPunCallbacks
{
    [Header("��� �߻� ����")]
    public Transform[] eggPoints;

    int LeftScore = 0;
    int RightScore = 0;

    int loser = -2;
    int maxPlayer;

    bool isStart = false;

    public Image whiteEgg;


    [Header("���ӸŴ���")]
    public GameManager gameManager;

    bool isMasterCilentLocal => PhotonNetwork.IsMasterClient && photonView.IsMine;
    //->���� �� ��ǻ�Ͱ� ȣ��Ʈ�鼭, �� ���ӿ�����Ʈ�� ȣ��Ʈ ������ ������

    BattleUIManager battleUIManager;
    

    private void Awake()
    {
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;

        battleUIManager = BattleUIManager.Instance;
        
        maxPlayer = PhotonNetwork.CurrentRoom.MaxPlayers;

        //��� ����
        photonView.RPC("createEgg", RpcTarget.AllBuffered);
    }

    #region ��� ����
    [PunRPC]
    public void createEgg()
    {
        StartCoroutine(createEggCor());
    }
    #endregion

    IEnumerator createEggCor() //��� ����
    {
        float curTime = 0, maxTime = 2.5f;
        while (curTime <= maxTime) 
        {
            whiteEgg.fillAmount = curTime / maxTime;
            curTime += Time.deltaTime;
            yield return null;
        }
        whiteEgg.fillAmount = 0;

        //��� ����
        if (isMasterCilentLocal) 
        {
            GameObject egg = gameManager.CreateObj("Egg", GameManager.PoolTypes.ObjType);
            egg.GetComponent<PhotonView>().RPC("eggOnRPC", RpcTarget.AllBuffered);
            egg.transform.position = eggPoints[0].position;
        }
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
                    //��� ����
                    photonView.RPC("createEgg", RpcTarget.AllBuffered);

                    //���� ����
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
                        cc.GetComponent<PhotonView>().RPC("loopTypingRPC", RpcTarget.AllBuffered, CharacterControls.TypingType.None, "Egg!");
                    }
                }
                else if (loser != -2)
                {
                    if (loser == -1)
                    {
                        if (LeftScore >= 2)
                        {
                            loser = 1;
                        }
                        else if (RightScore >= 2)
                        {
                            loser = 0;
                        }
                    }
                    else if (loser != -1) 
                    {
                        for (int i = 0; i < gameManager.playerGroup.childCount; i++)
                        {
                            CharacterControls cc = gameManager.playerGroup.GetChild(i).GetComponent<CharacterControls>();

                            if (i == loser) //�й������� �й� �޽��� ����
                            {
                                Debug.Log("UP");
                                cc.GetComponent<PhotonView>().RPC("loopTypingRPC", RpcTarget.AllBuffered,CharacterControls.TypingType.Lose, "Lose");
                            }
                            else
                            {
                                Debug.Log("DOWN");
                                cc.GetComponent<PhotonView>().RPC("loopTypingRPC", RpcTarget.AllBuffered, CharacterControls.TypingType.Win, "WIN");
                            }
                        }
                        loser = 2;
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
    void alreadyStartRPC() //���� ����
    {
        loser = -1;
    }
    public override void OnDisconnected(DisconnectCause cause)//��� ��������
    {
        gameManager.allLeaveRoomStart();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isMasterCilentLocal) //��� ����
        {
            if (collision.gameObject.CompareTag("Egg")) 
            {
                if (collision.gameObject.transform.position.x < 0) //���� ����
                {
                    if (loser < 0) 
                    {
                        //���� ���� ����
                        LeftScore += 1;
                        //Ÿ���� �۵�
                        for (int i = 0; i < gameManager.playerGroup.childCount; i++)
                        {
                            CharacterControls cc = gameManager.playerGroup.GetChild(i).GetComponent<CharacterControls>();
                            cc.GetComponent<PhotonView>().RPC("loopTypingRPC", RpcTarget.AllBuffered,
                                CharacterControls.TypingType.None, LeftScore + " : " + RightScore);
                        }
                    }

                    

                    //�� ����Ʈ
                    photonView.RPC("createEffect", RpcTarget.AllBuffered, true);
                }
                else if (collision.gameObject.transform.position.x > 0)//������ ����
                {
                    if (loser < 0)
                    {
                        //�������� ����
                        RightScore += 1;
                        //Ÿ���� �۵�
                        for (int i = 0; i < gameManager.playerGroup.childCount; i++)
                        {
                            CharacterControls cc = gameManager.playerGroup.GetChild(i).GetComponent<CharacterControls>();
                            cc.GetComponent<PhotonView>().RPC("loopTypingRPC", RpcTarget.AllBuffered,
                                CharacterControls.TypingType.None, LeftScore + " : " + RightScore);
                        }
                    }
                    //�� ����Ʈ
                    photonView.RPC("createEffect", RpcTarget.AllBuffered, false);
                }
                

                //��� ��Ȱ��ȭ
                collision.gameObject.GetComponent<PhotonView>().RPC("eggOffRPC", RpcTarget.AllBuffered);

                //��� ����
                photonView.RPC("createEgg", RpcTarget.AllBuffered);
            }
        }
    }

    //����Ʈ ����
    [PunRPC]
    public void createEffect(bool isLeft) 
    {
        //ȿ����
        battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Heal);

        //�� ����Ʈ
        GameObject effect = gameManager.CreateObj("congratulation 9", GameManager.PoolTypes.EffectType);//"Explosion 2"
        effect.SetActive(true);
        if(isLeft)
            effect.transform.position = eggPoints[1].position;
        if (!isLeft)
            effect.transform.position = eggPoints[2].position;
    }

    
}
