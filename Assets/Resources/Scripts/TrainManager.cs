using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrainManager : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab;

    private void Awake()
    {
        Screen.SetResolution(960, 540, false);//해상도 조정
        PhotonNetwork.SendRate = 480;//60, 동기화가 빨리 된다나 뭐라나
        PhotonNetwork.SerializationRate = 240;//30
    }

    public void Connect() => PhotonNetwork.ConnectUsingSettings();//바로 서버에 접속

    public override void OnConnectedToMaster()//온라인 상태일경우
    {
        PhotonNetwork.LocalPlayer.NickName = "Guest"+Random.Range(0,1000);
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions { MaxPlayers = 6 }, null);//최대 6명인 Room이라는 이름의 룸 생성************8
    }

    public override void OnJoinedRoom()//방에 들어가면 자동 실행
    {
        PhotonNetwork.Instantiate("Player", new Vector3(Random.Range(-6f, 6f), 4, 0), Quaternion.identity);
    }


    void Update() 
    { 
        if (Input.GetKeyDown(KeyCode.Escape) && PhotonNetwork.IsConnected) 
            PhotonNetwork.Disconnect(); 
    }//esacpe누르면서 연결중이면 단절

    public override void OnDisconnected(DisconnectCause cause)//단절됐을 때
    {
        //DisconnectPanel.SetActive(true);
        //RespawnPanel.SetActive(false);
    }
}
