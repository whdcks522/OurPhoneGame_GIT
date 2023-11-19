using Assets.PixelHeroes.Scripts.ExampleScripts;
using KoreanTyper;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggCollectManager : MonoBehaviourPunCallbacks
{
    [Header("��� �߻� ����")]
    public Transform[] eggPoints;

    int LeftScore = 0;
    int RightScore = 0;

    int loser = -2;
    int maxPlayer;

    [Header("���ӸŴ���")]
    public GameManager gameManager;

    bool isMasterCilentLocal => PhotonNetwork.IsMasterClient && photonView.IsMine;
    //->���� �� ��ǻ�Ͱ� ȣ��Ʈ�鼭, �� ���ӿ�����Ʈ�� ȣ��Ʈ ������ ������

    BattleUIManager battleUIManager;
    //�����ϴ� �ڷ�ƾ
    Coroutine textCor;

    private void Awake()
    {
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;

        battleUIManager = BattleUIManager.Instance;
        
        maxPlayer = PhotonNetwork.CurrentRoom.MaxPlayers;

        //StartCoroutine(CreateEgg());
    }

    IEnumerator CreateEgg() //��� ����
    {
        float curTime = 0, maxTime = 2.5f;
        while (curTime <= maxTime) 
        {
            curTime += Time.deltaTime;
            yield return null;
        }

        //��� ����
        GameObject egg = gameManager.CreateObj("Egg", GameManager.PoolTypes.ObjType);
        egg.GetComponent<PhotonView>().RPC("eggOnRPC", RpcTarget.AllBuffered);
        egg.transform.position = eggPoints[0].position;
    }

    void typingControl(CharacterControls _cc, string _str)
    {
        if (textCor != null)
            StopCoroutine(textCor);

        textCor = StartCoroutine(typingRoutine(_cc, _str));
    }


    public IEnumerator typingRoutine(CharacterControls _cc, string _str)
    {
        _cc.photonView.RPC("TypingRPC", RpcTarget.AllBuffered, CharacterControls.TypingType.None, _str);
        yield return new WaitForSeconds(3.5f + 0.075f * _str.Length);
        textCor = StartCoroutine(typingRoutine(_cc, _str));

        Debug.Log("Typing");
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
                    StartCoroutine(CreateEgg());

                    photonView.RPC("alreadyStartRPC", RpcTarget.AllBuffered);
                    for (int i = 0; i < gameManager.playerGroup.childCount; i++)
                    {
                        //���� Ȱ��ȭ
                        CharacterControls cc = gameManager.playerGroup.GetChild(i).GetComponent<CharacterControls>();
                        cc.photonView.RPC("changeStateRPC", RpcTarget.AllBuffered, CharacterControls.PlayerStateType.LeftControl, true);
                        cc.photonView.RPC("changeStateRPC", RpcTarget.AllBuffered, CharacterControls.PlayerStateType.RightControl, true);
                        //���� ���� 8����
                        cc.photonView.RPC("changeStateRPC", RpcTarget.AllBuffered, CharacterControls.PlayerStateType.SwordCount, false);

                        //�ؽ�Ʈ
                        cc.photonView.RPC("TypingRPC", RpcTarget.AllBuffered, CharacterControls.TypingType.None, "Egg!");
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
                                cc.photonView.RPC("TypingRPC", RpcTarget.AllBuffered, CharacterControls.TypingType.Lose, "Lose");
                                //typingControl(cc, "Lose");
                            }
                            else
                            {
                                //typingControl(cc, "Win");
                                cc.photonView.RPC("TypingRPC", RpcTarget.AllBuffered, CharacterControls.TypingType.Win, "Win");
                            }
                        }
                        loser = 2;
                    }
                }
            }
        }//���� �� ��,

        //������ �ƴϿ���
        
        if (loser == -2)
        {
            string str = PhotonNetwork.CurrentRoom.Name + '\n' +
                    PhotonNetwork.CurrentRoom.PlayerCount + '/' + PhotonNetwork.CurrentRoom.MaxPlayers;


            CharacterControls cc = gameManager.playerGroup.GetChild(0).GetComponent<CharacterControls>();
            cc.TypingRPC(CharacterControls.TypingType.None, str);

            /*
            for (int i = 0; i < gameManager.playerGroup.childCount; i++)
            {
                CharacterControls cc = gameManager.playerGroup.GetChild(i).GetComponent<CharacterControls>();
                typingControl(cc, LeftScore + " : " + RightScore);
            }
            */

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
                if (collision.gameObject.transform.position.x < 0) 
                {
                    //���� ���� ����
                    LeftScore += 1;
                    //Ÿ���� �۵�
                    for(int i = 0; i < gameManager.playerGroup.childCount; i++)
                    {
                        CharacterControls cc = gameManager.playerGroup.GetChild(i).GetComponent<CharacterControls>();
                        typingControl(cc, LeftScore + " : " + RightScore);
                    }

                    //�� ����Ʈ
                    photonView.RPC("createEffect", RpcTarget.AllBuffered, true);
                }
                else if (collision.gameObject.transform.position.x > 0)
                {
                    //�������� ����
                    RightScore += 1;
                    //Ÿ���� �۵�
                    for (int i = 0; i < gameManager.playerGroup.childCount; i++)
                    {
                        CharacterControls cc = gameManager.playerGroup.GetChild(i).GetComponent<CharacterControls>();
                        typingControl(cc, LeftScore + " : " + RightScore);
                    }

                    //�� ����Ʈ
                    photonView.RPC("createEffect", RpcTarget.AllBuffered, false);
                }
                

                //��� ��Ȱ��ȭ
                collision.gameObject.GetComponent<PhotonView>().RPC("eggOffRPC", RpcTarget.AllBuffered);

                //��� ����
                StartCoroutine(CreateEgg());
            }
        }
    }

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
