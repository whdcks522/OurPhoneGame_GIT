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
    [Header("조이스틱의 정보를 받아들임")]
    //플레이어 이동
    public VariableJoystick moveJoy;
    //플레이어 칼
    public VariableJoystick swordJoy;
    [Header("인스턴스에서 공유하기 위한 플레이어 정보")]
    public GameObject player;
    //카메라
    CinemachineVirtualCamera cinemachineVirtualCamera;

    //오브젝트 풀링
    //임시로 게임오브젝트 저장할 곳


    //총알 리스트
    string[] bulletNames = { "YellowStarBullet", "YellowStarFlash", "YellowStarHit"};

    List<GameObject>[] bulletPools;//실제로 주소가 저장될 곳
    

    //moveJoy = joySticks.transform.GetChild(0).GetComponent<VariableJoystick>();
    //swordJoy = joySticks.transform.GetChild(1).GetComponent<VariableJoystick>();

    public enum PoolTypes
    {
        EnemyType, BulletType
    }

    private void Awake()
    {
        //총알 풀 초기화
        bulletPools = new List<GameObject>[bulletNames.Length];
        for (int index = 0; index < bulletNames.Length; index++)//풀 하나하나 초기화
            bulletPools[index] = new List<GameObject>();


        //플레이어 생성
        //if ((PhotonNetwork.InRoom) && SceneManager.GetActiveScene().name == "Training")//네트워크 중에 있다면
        //    player = PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity);
        //else//싱글 플레이라면
        {
            player = Instantiate(Resources.Load<GameObject>("Player"), Vector3.zero, Quaternion.identity);
        }

        //카메라 관리
        cinemachineVirtualCamera = transform.GetChild(0).GetComponent<CinemachineVirtualCamera>();
        cinemachineVirtualCamera.Follow = player.transform;
        cinemachineVirtualCamera.LookAt = player.transform;
    }

    #region 싱글턴
    private static GameManager instance;
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
            //더하기
            tmpPools[index].Add(tmpGameObject);
            
            switch (poolTypes)
            {
                case PoolTypes.BulletType:
                    bulletPools = tmpPools;
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
