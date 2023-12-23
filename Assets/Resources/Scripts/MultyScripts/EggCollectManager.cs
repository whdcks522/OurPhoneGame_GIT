using Assets.PixelHeroes.Scripts.ExampleScripts;
using KoreanTyper;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Assets.PixelHeroes.Scripts.ExampleScripts.CharacterControls;

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
    int targetScore = 7;

    [Header("���ӸŴ���")]
    public GameManager gameManager;

    bool isMasterCilentLocal => PhotonNetwork.IsMasterClient && photonView.IsMine;
    //->���� �� ��ǻ�Ͱ� ȣ��Ʈ�鼭, �� ���ӿ�����Ʈ�� ȣ��Ʈ ������ ������

    BattleUIManager battleUIManager;
    

    private void Awake()
    {
        PhotonNetwork.SendRate = 6;
        PhotonNetwork.SerializationRate = 3;

        battleUIManager = BattleUIManager.Instance;
        
        maxPlayer = PhotonNetwork.CurrentRoom.MaxPlayers;
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
            egg.GetComponent<PhotonView>().RPC("eggOnRPC", RpcTarget.All);
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

                        //�ؽ�Ʈ
                        cc.GetComponent<PhotonView>().RPC("loopTypingRPC", RpcTarget.AllBuffered, CharacterControls.TypingType.None, "Egg!", false);
                    }
                }
                else if (loser != -2)
                {
                    if (loser == -1)
                    {
                        if (LeftScore >= targetScore)
                        {
                            loser = 1;
                        }
                        else if (RightScore >= targetScore)
                        {
                            loser = 0;
                        }
                    }
                    else if (loser == 0 || loser == 1) 
                    {
                        for (int i = 0; i < gameManager.playerGroup.childCount; i++)
                        {
                            CharacterControls cc = gameManager.playerGroup.GetChild(i).GetComponent<CharacterControls>();

                            if (i == loser) //�й������� �й� �޽��� ����
                            {
                                cc.GetComponent<PhotonView>().RPC("loopTypingRPC", RpcTarget.AllBuffered,CharacterControls.TypingType.Lose, "Lose", true);
                                cc.GetComponent<PhotonView>().RPC("changeStateRPC", RpcTarget.AllBuffered, PlayerStateType.Dead, true);
                            }
                            else
                            {
                                cc.GetComponent<PhotonView>().RPC("loopTypingRPC", RpcTarget.AllBuffered, CharacterControls.TypingType.Win, "Win", true);
                            }
                        }
                        loser = 2;//update�� ���� ����
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
            cc.loopTypingRPC(CharacterControls.TypingType.None, str, true);
        }

        //�����ϰ� ���� Ż���ϴ� ���
        if (loser != -2 && !(PhotonNetwork.PlayerList.Length >= maxPlayer))
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
                                CharacterControls.TypingType.None, LeftScore + " : " + RightScore, true);
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
                                CharacterControls.TypingType.None, LeftScore + " : " + RightScore, true);
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
