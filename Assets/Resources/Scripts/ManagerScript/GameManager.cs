using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using Photon.Pun;
using System;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
    [Header("인스턴스에서 공유하기 위한 플레이어 정보")]
    public GameObject player;
    public CharacterController characterController;
    //카메라
    CinemachineVirtualCamera cinemachineVirtualCamera;
    BattleUIManager battleUIManager;

    //총알 리스트
    string[] bulletNames = { "YellowStarBullet", "YellowStarHit",
                                "BlackStarBullet", "BlackStarHit",
                                "GreenStarBullet", "GreenStarHit" };
    //총알 주소가 저장될 곳
    List<GameObject>[] bulletPools;

    //폭탄 리스트
    string[] bombNames = {"Broken Phantasm" };
    //폭탄 주소가 저장될 곳
    List<GameObject>[] bombPools;

    //블록 리스트
    string[] blockNames = { "NormalBlock" };
    //블록 주소가 저장될 곳
    List<GameObject>[] blockPools;

    public enum PoolTypes
    {
        BulletType, BombType, BlockType //EnemyType, 
    }

    private void Awake()
    {
        battleUIManager = BattleUIManager.Instance;
        battleUIManager.gameManager = this;
        //가져오기
        battleUIManager.battleUI.SetActive(true);
        battleUIManager.curScore = 0;
        battleUIManager.bigRankText.text = "E";
        battleUIManager.bigScoreText.text = 0 +" / "+ battleUIManager.Dscore;


        //총알 풀 초기화
        bulletPools = new List<GameObject>[bulletNames.Length];
        for (int index = 0; index < bulletNames.Length; index++)//풀 하나하나 초기화
            bulletPools[index] = new List<GameObject>();

        //폭탄 풀 초기화(4개씩 수정)
        bombPools = new List<GameObject>[bombNames.Length];
        for (int index = 0; index < bombNames.Length; index++)//풀 하나하나 초기화
            bombPools[index] = new List<GameObject>();

        //폭탄 풀 초기화(4개씩 수정)
        blockPools = new List<GameObject>[blockNames.Length];
        for (int index = 0; index < blockNames.Length; index++)//풀 하나하나 초기화
            blockPools[index] = new List<GameObject>();


        //플레이어 생성
        //if ((PhotonNetwork.InRoom) && SceneManager.GetActiveScene().name == "Training")//네트워크 중에 있다면
        //    player = PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity);
        //else//싱글 플레이라면
        if (!PhotonNetwork.InRoom)
        {
            player = Instantiate(Resources.Load<GameObject>("Player"), Vector3.zero, Quaternion.identity);
        }
        characterController = player.GetComponent<CharacterController>();

        //카메라 관리
        cinemachineVirtualCamera = transform.GetChild(0).GetComponent<CinemachineVirtualCamera>();
        cinemachineVirtualCamera.Follow = player.transform;
        cinemachineVirtualCamera.LookAt = player.transform;
    }

    #region 싱글턴
    private static GameManager instance;//따로 작성 없음
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<GameManager>();
            return instance;
        }
    }
    #endregion

    
    public GameObject CreateObj(string name, PoolTypes poolTypes) //있으면 적 부르고, 없으면 생성
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
                 //Debug.Log("총알 타입 선택됨");
                 break;
            case PoolTypes.BombType:
                tmpPools = bombPools;
                tmpNames = bombNames;//awake에서 선언햇니
                break;
            case PoolTypes.BlockType:
                tmpPools = blockPools;
                tmpNames = blockNames;//awake에서 선언햇니
                break;
        }

        int index = NametoIndex(tmpNames, name);
       //있다면 찾아봄
        foreach (GameObject item in tmpPools[index])
        {
            if (!item.activeSelf)
            {
                //Debug.Log("재사용함");
                tmpGameObject = item;
                break;
            }
        }

        //없으면 생성하고 select에 할당
        if (!tmpGameObject)
        {
            //Debug.Log("생성함");

            //네트워크 중에 있다면
            if (PhotonNetwork.InRoom)
                tmpGameObject = PhotonNetwork.Instantiate(tmpNames[index], Vector3.zero, Quaternion.identity);
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
            }
        }   
        return tmpGameObject;
    }

    int NametoIndex(string[] tmpNames, string _name) //오브젝트풀링에서 생성하는 문자열을 순서로 변환
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

}
