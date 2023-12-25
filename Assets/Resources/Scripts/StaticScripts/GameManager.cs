using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using Photon.Pun;
using System;
using UnityEngine.SceneManagement;
using Assets.PixelHeroes.Scripts.ExampleScripts;
using Photon.Pun.Demo.PunBasics;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks
{
    [Header("인스턴스에서 공유하기 위한 플레이어 정보")]
    public GameObject player;
    public CharacterControls characterControl;

    [Header("멀티에서 플레이어가 생성될 위치")]
    public Transform[] spawnPositions;
    [Header("멀티에서 플레이어의 리스트")]
    public Transform playerGroup;

    //카메라
    CinemachineVirtualCamera cinemachineVirtualCamera;
    BattleUIManager battleUIManager;


    //1. 총알 리스트
    string[] bulletNames = { "NormalBullet", "NormalBulletHit","PowerUpBullet","PowerUpBulletHit",
                                "UnBreakableBullet", "UnBreakableBulletHit"};
    //총알 주소가 저장될 곳
    List<GameObject>[] bulletPools;

    //2. 폭탄 리스트
    string[] bombNames = {"Broken Phantasm" };
    //폭탄 주소가 저장될 곳
    List<GameObject>[] bombPools;

    //3. 블록 리스트
    string[] blockNames = { "NormalBlock", "HardBlock", "PowerUpBlock", "CureBlock" };
    //블록 주소가 저장될 곳
    List<GameObject>[] blockPools;

    //4. 이펙트 리스트(2가 강한 폭발, 6이 약한 폭발),3은 안씀
    string[] effectNames = { "Explosion 2", "Explosion 3", "Explosion 6", "Explosion 2_Cure", "Explosion 2_PowerUp",
                                "Text 52_Enemy", "Text 52_Player", "congratulation 9"};
    //이펙트 주소가 저장될 곳
    List<GameObject>[] effectPools;

    //5. 바람 리스트
    string[] windNames = { "NormalWind" };
    //이펙트 주소가 저장될 곳
    List<GameObject>[] windPools;

    //6. 오브젝트 리스트
    string[] objNames = { "Egg" };
    //이펙트 주소가 저장될 곳
    List<GameObject>[] objPools;

    public enum PoolTypes
    {
        BulletType, BombType, BlockType, EffectType, WindType, ObjType //EnemyType, 
    }

    private void Awake()
    {
        battleUIManager = BattleUIManager.Instance;
        battleUIManager.gameManager = this;

        //1. 총알 풀 초기화
        bulletPools = new List<GameObject>[bulletNames.Length];
        for (int index = 0; index < bulletNames.Length; index++)//풀 하나하나 초기화
            bulletPools[index] = new List<GameObject>();

        //2. 폭탄 풀 초기화(4개씩 수정)
        bombPools = new List<GameObject>[bombNames.Length];
        for (int index = 0; index < bombNames.Length; index++)//풀 하나하나 초기화
            bombPools[index] = new List<GameObject>();

        //3. 블록 풀 초기화(4개씩 수정)
        blockPools = new List<GameObject>[blockNames.Length];
        for (int index = 0; index < blockNames.Length; index++)//풀 하나하나 초기화
            blockPools[index] = new List<GameObject>();

        //4. 이펙트 풀 초기화(4개씩 수정)
        effectPools = new List<GameObject>[effectNames.Length];
        for (int index = 0; index < effectNames.Length; index++)//풀 하나하나 초기화
            effectPools[index] = new List<GameObject>();

        //5. 바람 풀 초기화(4개씩 수정)
        windPools = new List<GameObject>[windNames.Length];
        for (int index = 0; index < windNames.Length; index++)//풀 하나하나 초기화
            windPools[index] = new List<GameObject>();

        //6. 오브젝트 풀 초기화(4개씩 수정)
        objPools = new List<GameObject>[objNames.Length];
        for (int index = 0; index < objNames.Length; index++)//풀 하나하나 초기화
            objPools[index] = new List<GameObject>();
    }

    private void Start()
    {
        if (battleUIManager.battleType == BattleUIManager.BattleType.Multy)//Room에 있는지 물어보면 작동 안하더라
        {
            var localPlayerIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1;//현재 방에 들어온 플레이어의 번호(1부터 시작, 배열을 이용함)
            var spawnPosition = spawnPositions[localPlayerIndex % spawnPositions.Length];//혹시 몰라서 나눔

            player = PhotonNetwork.Instantiate("Player", spawnPosition.position, Quaternion.identity);
            characterControl = player.GetComponent<CharacterControls>();
            //방향 전환
            characterControl.GetComponent<PhotonView>().RPC("TurnRPC", RpcTarget.AllBuffered, (int)spawnPosition.localScale.x);
            characterControl.gameManager = this;
            player.transform.parent = playerGroup;
            

            //UI 가져오기
            battleUIManager.battleUI.SetActive(true);
            battleUIManager.bigRankText.gameObject.SetActive(false);
            battleUIManager.bigScoreText.gameObject.SetActive(false);
            battleUIManager.multyExitBtn.SetActive(true);
            battleUIManager.singleStopBtn.SetActive(false);
        }
        else if (battleUIManager.battleType == BattleUIManager.BattleType.Single)//싱글의 경우
        {
            //UI 가져오기
            battleUIManager.battleUI.SetActive(true);
            battleUIManager.bigRankText.gameObject.SetActive(true);
            battleUIManager.bigScoreText.gameObject.SetActive(true);
            battleUIManager.curScore = 0;
            battleUIManager.rankType = BattleUIManager.RankType.E;
            battleUIManager.bigRankText.text = "<color=#AA00FF> E </color>";
            battleUIManager.bigScoreText.text = 0 + " / " + battleUIManager.Dscore;
            battleUIManager.multyExitBtn.SetActive(false);
            battleUIManager.singleStopBtn.SetActive(true);
        }

        //카메라 관리
        cinemachineVirtualCamera = transform.GetChild(0).GetComponent<CinemachineVirtualCamera>();
        cinemachineVirtualCamera.Follow = player.transform;
        cinemachineVirtualCamera.LookAt = player.transform;
    }

    public GameObject CreateObj(string _name, PoolTypes poolTypes) //있으면 적 부르고, 없으면 생성
    {
        //반드시 매번 초기화
        GameObject tmpGameObject = null;

        List<GameObject>[] tmpPools = null;
        string[] tmpNames = null;

        switch (poolTypes) 
        {
             case PoolTypes.BulletType:
                 tmpPools = bulletPools;
                 tmpNames = bulletNames;
                 break;
            case PoolTypes.BombType:
                tmpPools = bombPools;
                tmpNames = bombNames;//awake에서 선언햇니
                break;
            case PoolTypes.BlockType:
                tmpPools = blockPools;
                tmpNames = blockNames;//awake에서 선언햇니
                break;
            case PoolTypes.EffectType:
                tmpPools = effectPools;
                tmpNames = effectNames;//awake에서 선언햇니
                break;
            case PoolTypes.WindType:
                tmpPools = windPools;
                tmpNames = windNames;//awake에서 선언햇니
                break;
            case PoolTypes.ObjType:
                tmpPools = objPools;
                tmpNames = objNames;//awake에서 선언햇니
                break;
        }

        int index = NametoIndex(tmpNames, _name);
       //있다면 찾아봄
        foreach (GameObject item in tmpPools[index])
        {
            if (!item.activeSelf)
            {
                tmpGameObject = item;
                break;
            }
        }

        //없으면 생성하고 select에 할당
        if (!tmpGameObject)
        {
            //네트워크 중에 있다면
            if (PhotonNetwork.InRoom)
            {
                tmpGameObject = PhotonNetwork.Instantiate(tmpNames[index], Vector3.zero, Quaternion.identity);
                
            }
            //싱글 플레이라면
            else
            {
                tmpGameObject = Instantiate(Resources.Load<GameObject>(tmpNames[index]), Vector3.zero, Quaternion.identity);
                
            }
            //임시 리스트에 더하기
            tmpPools[index].Add(tmpGameObject);
            
            //동기화
            switch (poolTypes)
            {
                case PoolTypes.BulletType:
                    bulletPools = tmpPools;
                    break;
                case PoolTypes.BombType:
                    bombPools = tmpPools;
                    break;
                case PoolTypes.BlockType:
                    blockPools = tmpPools;
                    break;
                case PoolTypes.EffectType:
                    effectPools = tmpPools;
                    break;
                case PoolTypes.WindType:
                    windPools = tmpPools;
                    break;
                case PoolTypes.ObjType:
                    objPools = tmpPools;
                    break;
            }
        }   
        return tmpGameObject;
    }

    #region 오브젝트 풀링에서 순서 출력
    int NametoIndex(string[] tmpNames, string _name) 
    {
        for (int i = 0; i < tmpNames.Length; i++)
        {
            if (string.Equals(tmpNames[i], _name))
            {
                return i;
            }
        }
        Debug.Log("Error: -1");
        return -1;
    }
    #endregion

    #region 멀티 플레이 모두 퇴장


    public void allLeaveRoomStart() //모두 방에서 나가기
    {
        photonView.RPC("LeaveRoomRPC", RpcTarget.All);
    }

    [PunRPC]
    void LeaveRoomRPC()
    {
        battleUIManager.battleUI.SetActive(false);
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("Lobby");
        
        /*
        bool canEnterRoom = (bool)PhotonNetwork.CurrentRoom.CustomProperties["IsAllowedToEnter"];
        bool canExitRoom = (bool)PhotonNetwork.CurrentRoom.CustomProperties["IsAllowedToExit"];

        if (photonView.IsMine )
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                battleUIManager.battleUI.SetActive(false);
                PhotonNetwork.LeaveRoom();
                SceneManager.LoadScene("Lobby");
            }
            else Debug.Log("못나감");
        }
        //방장이 아닌 플레이어는, 전투 중일 때만 못나감
        else if (!canExitRoom)//전투중이 아닐 경우, 방장만 못나감
        {
            Debug.Log("못나감");
        }
        else
        {
            battleUIManager.battleUI.SetActive(false);
            PhotonNetwork.LeaveRoom();
            SceneManager.LoadScene("Lobby");

        }
        */
    }
    #endregion


    /*
     //룸 설정
        ExitGames.Client.Photon.Hashtable roomProperties = new ExitGames.Client.Photon.Hashtable();
        roomProperties.Add("IsAllowedToEnter", false);//입장은 맨 처음 빼고 더이상 못들어옴
        roomProperties.Add("IsAllowedToExit", true);//대기 중에는 나갈 수 있도록
        PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperties);//모두에게 적용됨-------
     */
}
