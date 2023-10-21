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
        Screen.SetResolution(960, 540, false);//�ػ� ����
        PhotonNetwork.SendRate = 480;//60, ����ȭ�� ���� �ȴٳ� ����
        PhotonNetwork.SerializationRate = 240;//30
    }

    public void Connect() => PhotonNetwork.ConnectUsingSettings();//�ٷ� ������ ����

    public override void OnConnectedToMaster()//�¶��� �����ϰ��
    {
        PhotonNetwork.LocalPlayer.NickName = "Guest"+Random.Range(0,1000);
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions { MaxPlayers = 6 }, null);//�ִ� 6���� Room�̶�� �̸��� �� ����************8
    }

    public override void OnJoinedRoom()//�濡 ���� �ڵ� ����
    {
        PhotonNetwork.Instantiate("Player", new Vector3(Random.Range(-6f, 6f), 4, 0), Quaternion.identity);
    }


    void Update() 
    { 
        if (Input.GetKeyDown(KeyCode.Escape) && PhotonNetwork.IsConnected) 
            PhotonNetwork.Disconnect(); 
    }//esacpe�����鼭 �������̸� ����

    public override void OnDisconnected(DisconnectCause cause)//�������� ��
    {
        //DisconnectPanel.SetActive(true);
        //RespawnPanel.SetActive(false);
    }
}
