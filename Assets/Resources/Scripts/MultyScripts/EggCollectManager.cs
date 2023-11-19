using Assets.PixelHeroes.Scripts.ExampleScripts;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggCollectManager : MonoBehaviourPunCallbacks
{
    [Header("계란 발생 지점")]
    public Transform[] eggPoints;

    int LeftScore = 0;
    int RightScore = 0;

    int loser = -2;
    int maxPlayer;

    [Header("게임매니저")]
    public GameManager gameManager;

    bool isMasterCilentLocal => PhotonNetwork.IsMasterClient && photonView.IsMine;
    //->현재 이 컴퓨터가 호스트면서, 이 게임오브젝트가 호스트 측에서 생성됨

    BattleUIManager battleUIManager;

    private void Awake()
    {
        //아래 2개는 동기화에 도움 준다는데 맞는지 몰루
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;

        battleUIManager = BattleUIManager.Instance;
        
        maxPlayer = PhotonNetwork.CurrentRoom.MaxPlayers;

        if(isMasterCilentLocal)
        {
            //계란 생성
            StartCoroutine(CreateEgg());
        }
    }

    int count = 0;
    IEnumerator CreateEgg() //계란 생성
    {
        yield return null;

        float curTime = 0, maxTime = 2.5f;
        while (curTime <= maxTime) 
        {
            curTime += Time.deltaTime;
            yield return null;
        }
        count++;

        //계란 생성
        GameObject egg = gameManager.CreateObj("Egg", GameManager.PoolTypes.ObjType);
        egg.GetComponent<PhotonView>().RPC("eggOnRPC", RpcTarget.AllBuffered);
        egg.transform.position = eggPoints[0].position;
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
                    if (LeftScore >= 2) 
                    {
                        loser = 1;
                    }
                    else if (RightScore >= 2)
                    {
                        loser = 0;
                    }

                    for (int i = 0; i < gameManager.playerGroup.childCount; i++)
                    {
                        CharacterControls cc = gameManager.playerGroup.GetChild(i).GetComponent<CharacterControls>();

                        if (loser == -1)//경쟁중이면 점수 출력
                        {
                            cc.photonView.RPC("TypingRPC", RpcTarget.AllBuffered, CharacterControls.TypingType.None, LeftScore + " : " + RightScore);
                        }
                        else if (i == loser) //패배자한테 패배 메시지 전송
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
    void alreadyStartRPC() //시작 선언
    {
        loser = -1;
    }
    public override void OnDisconnected(DisconnectCause cause)//모두 나가도록
    {
        gameManager.allLeaveRoomStart();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isMasterCilentLocal) //계란 삽입
        {
            if (collision.gameObject.CompareTag("Egg")) 
            {
                if (collision.gameObject.transform.position.x < 0) 
                {
                    LeftScore += 1;
                    //골 이펙트
                    photonView.RPC("createEffect", RpcTarget.AllBuffered, true);
                }
                else if (collision.gameObject.transform.position.x > 0)
                {
                    RightScore += 1;
                    //골 이펙트
                    photonView.RPC("createEffect", RpcTarget.AllBuffered, false);
                }
                

                //계란 비활성화
                collision.gameObject.GetComponent<PhotonView>().RPC("eggOffRPC", RpcTarget.AllBuffered);

                //계란 생성
                StartCoroutine(CreateEgg());
            }
        }
    }

    [PunRPC]
    public void createEffect(bool isLeft) 
    {
        //효과음
        battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Heal);

        //골 이펙트
        GameObject effect = gameManager.CreateObj("congratulation 9", GameManager.PoolTypes.EffectType);//"Explosion 2"
        effect.SetActive(true);
        if(isLeft)
            effect.transform.position = eggPoints[1].position;
        if (!isLeft)
            effect.transform.position = eggPoints[2].position;
    }


}
