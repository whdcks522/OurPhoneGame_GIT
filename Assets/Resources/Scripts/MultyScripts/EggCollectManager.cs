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
    [Header("계란 발생 지점")]
    public Transform[] eggPoints;

    int LeftScore = 0;
    int RightScore = 0;

    int loser = -2;
    int maxPlayer;

    bool isStart = false;

    public Image whiteEgg;


    [Header("게임매니저")]
    public GameManager gameManager;

    bool isMasterCilentLocal => PhotonNetwork.IsMasterClient && photonView.IsMine;
    //->현재 이 컴퓨터가 호스트면서, 이 게임오브젝트가 호스트 측에서 생성됨

    BattleUIManager battleUIManager;
    

    private void Awake()
    {
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;

        battleUIManager = BattleUIManager.Instance;
        
        maxPlayer = PhotonNetwork.CurrentRoom.MaxPlayers;

        //계란 생성
        photonView.RPC("createEgg", RpcTarget.AllBuffered);
    }

    #region 계란 생성
    [PunRPC]
    public void createEgg()
    {
        StartCoroutine(createEggCor());
    }
    #endregion

    IEnumerator createEggCor() //계란 생성
    {
        float curTime = 0, maxTime = 2.5f;
        while (curTime <= maxTime) 
        {
            whiteEgg.fillAmount = curTime / maxTime;
            curTime += Time.deltaTime;
            yield return null;
        }
        whiteEgg.fillAmount = 0;

        //계란 생성
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
                gameManager.playerGroup.childCount >= maxPlayer)//실제로 플레이어도 생성 됐을 때 시작해야함
            {
                //게임 시작

                if (loser == -2)//-2: 게임을 막 시작한 경우
                {
                    //계란 생성
                    photonView.RPC("createEgg", RpcTarget.AllBuffered);

                    //시작 선언
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

                            if (i == loser) //패배자한테 패배 메시지 전송
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
        }//방장 일 때,

        //방장이 아니여도
        
        if (loser == -2 && !isStart)
        {
            isStart = true;

            string str = PhotonNetwork.CurrentRoom.Name + '\n' +
                    PhotonNetwork.CurrentRoom.PlayerCount + '/' + PhotonNetwork.CurrentRoom.MaxPlayers;


            CharacterControls cc = gameManager.playerGroup.GetChild(0).GetComponent<CharacterControls>();
            cc.loopTypingRPC(CharacterControls.TypingType.None, str);
        }
        
        //시작하고 나서 탈주하는 경우
        if (loser != -2 && !(PhotonNetwork.PlayerList.Length >= maxPlayer && gameManager.playerGroup.childCount >= maxPlayer))
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
                if (collision.gameObject.transform.position.x < 0) //왼쪽 골인
                {
                    if (loser < 0) 
                    {
                        //좌측 점수 증가
                        LeftScore += 1;
                        //타이핑 작동
                        for (int i = 0; i < gameManager.playerGroup.childCount; i++)
                        {
                            CharacterControls cc = gameManager.playerGroup.GetChild(i).GetComponent<CharacterControls>();
                            cc.GetComponent<PhotonView>().RPC("loopTypingRPC", RpcTarget.AllBuffered,
                                CharacterControls.TypingType.None, LeftScore + " : " + RightScore);
                        }
                    }

                    

                    //골 이펙트
                    photonView.RPC("createEffect", RpcTarget.AllBuffered, true);
                }
                else if (collision.gameObject.transform.position.x > 0)//오른쪽 골인
                {
                    if (loser < 0)
                    {
                        //우측점수 증가
                        RightScore += 1;
                        //타이핑 작동
                        for (int i = 0; i < gameManager.playerGroup.childCount; i++)
                        {
                            CharacterControls cc = gameManager.playerGroup.GetChild(i).GetComponent<CharacterControls>();
                            cc.GetComponent<PhotonView>().RPC("loopTypingRPC", RpcTarget.AllBuffered,
                                CharacterControls.TypingType.None, LeftScore + " : " + RightScore);
                        }
                    }
                    //골 이펙트
                    photonView.RPC("createEffect", RpcTarget.AllBuffered, false);
                }
                

                //계란 비활성화
                collision.gameObject.GetComponent<PhotonView>().RPC("eggOffRPC", RpcTarget.AllBuffered);

                //계란 생성
                photonView.RPC("createEgg", RpcTarget.AllBuffered);
            }
        }
    }

    //이펙트 생성
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
