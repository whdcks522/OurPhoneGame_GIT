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

public class GameManager : MonoBehaviourPunCallbacks
{
    [Header("�ν��Ͻ����� �����ϱ� ���� �÷��̾� ����")]
    public GameObject player;
    public CharacterControls characterControl;

    [Header("��Ƽ�� ���� �÷��̾� ����")]
    public Transform[] spawnPositions;
    public List<GameObject>list = new List<GameObject>();
    //ī�޶�
    CinemachineVirtualCamera cinemachineVirtualCamera;
    BattleUIManager battleUIManager;

    //�Ѿ� ����Ʈ
    string[] bulletNames = { "YellowStarBullet", "YellowStarHit",
                                "BlackStarBullet", "BlackStarHit",
                                "GreenStarBullet", "GreenStarHit" };
    //�Ѿ� �ּҰ� ����� ��
    List<GameObject>[] bulletPools;

    //��ź ����Ʈ
    string[] bombNames = {"Broken Phantasm" };
    //��ź �ּҰ� ����� ��
    List<GameObject>[] bombPools;

    //��� ����Ʈ
    string[] blockNames = { "NormalBlock", "HardBlock", "PowerUpBlock" };
    //��� �ּҰ� ����� ��
    List<GameObject>[] blockPools;

    //����Ʈ ����Ʈ(2�� ���� ����, 6�� ���� ����)
    string[] effectNames = { "Explosion 2", "Explosion 3", "Explosion 6", "Explosion 2_PowerUp", "Text 52" };//"congratulation 9"
    //��� �ּҰ� ����� ��
    List<GameObject>[] effectPools;

    public enum PoolTypes
    {
        BulletType, BombType, BlockType, EffectType //EnemyType, 
    }

    private void Awake()
    {
        battleUIManager = BattleUIManager.Instance;
        battleUIManager.gameManager = this;

        //�Ѿ� Ǯ �ʱ�ȭ
        bulletPools = new List<GameObject>[bulletNames.Length];
        for (int index = 0; index < bulletNames.Length; index++)//Ǯ �ϳ��ϳ� �ʱ�ȭ
            bulletPools[index] = new List<GameObject>();

        //��ź Ǯ �ʱ�ȭ(4���� ����)
        bombPools = new List<GameObject>[bombNames.Length];
        for (int index = 0; index < bombNames.Length; index++)//Ǯ �ϳ��ϳ� �ʱ�ȭ
            bombPools[index] = new List<GameObject>();

        //��ź Ǯ �ʱ�ȭ(4���� ����)
        blockPools = new List<GameObject>[blockNames.Length];
        for (int index = 0; index < blockNames.Length; index++)//Ǯ �ϳ��ϳ� �ʱ�ȭ
            blockPools[index] = new List<GameObject>();

        //��ź Ǯ �ʱ�ȭ(4���� ����)
        effectPools = new List<GameObject>[effectNames.Length];
        for (int index = 0; index < effectNames.Length; index++)//Ǯ �ϳ��ϳ� �ʱ�ȭ
            effectPools[index] = new List<GameObject>();
        
        if (battleUIManager.battleType == BattleUIManager.BattleType.Multy)//Room�� �ִ��� ����� �۵� ���ϴ���
        {
            var localPlayerIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1;//���� �濡 ���� �÷��̾��� ��ȣ(1���� ����, �迭�� �̿���)
            var spawnPosition = spawnPositions[localPlayerIndex % spawnPositions.Length];//Ȥ�� ���� ����

            player = PhotonNetwork.Instantiate("Player", spawnPosition.position, Quaternion.identity);
            player.transform.parent = transform;

            //UI ��������
            battleUIManager.battleUI.SetActive(true);
            battleUIManager.multyExitBtn.SetActive(true);
            battleUIManager.singleStopBtn.SetActive(false);
        }
        else if (battleUIManager.battleType == BattleUIManager.BattleType.Single)//�̱��� ���
        {
            //Į ���� �̸� ����

            //UI ��������
            battleUIManager.battleUI.SetActive(true);
            battleUIManager.curScore = 0;
            battleUIManager.rankType = BattleUIManager.RankType.E;
            battleUIManager.bigRankText.text = "<color=#AA00FF> E </color>";
            battleUIManager.bigScoreText.text = 0 + " / " + battleUIManager.Dscore;
            battleUIManager.multyExitBtn.SetActive(false);
            battleUIManager.singleStopBtn.SetActive(true);
        }
        list.Add(player);

        //characterControl = player.GetComponent<CharacterControls>();

        //ī�޶� ����
        cinemachineVirtualCamera = transform.GetChild(0).GetComponent<CinemachineVirtualCamera>();
        cinemachineVirtualCamera.Follow = player.transform;
        cinemachineVirtualCamera.LookAt = player.transform;


    }
    
    public GameObject CreateObj(string _name, PoolTypes poolTypes) //������ �� �θ���, ������ ����
    {
        //�ݵ�� �Ź� �ʱ�ȭ
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
                tmpNames = bombNames;//awake���� �����޴�
                break;
            case PoolTypes.BlockType:
                tmpPools = blockPools;
                tmpNames = blockNames;//awake���� �����޴�
                break;
            case PoolTypes.EffectType:
                tmpPools = effectPools;
                tmpNames = effectNames;//awake���� �����޴�
                break;
        }

        int index = NametoIndex(tmpNames, _name);
       //�ִٸ� ã�ƺ�
        foreach (GameObject item in tmpPools[index])
        {
            if (!item.activeSelf)
            {
                tmpGameObject = item;
                break;
            }
        }

        //������ �����ϰ� select�� �Ҵ�
        if (!tmpGameObject)
        {
            //��Ʈ��ũ �߿� �ִٸ�
            if (PhotonNetwork.InRoom)
            {
                tmpGameObject = PhotonNetwork.Instantiate(tmpNames[index], Vector3.zero, Quaternion.identity);
                tmpGameObject.name = _name;
            }
            //�̱� �÷��̶��
            else
            {
                tmpGameObject = Instantiate(Resources.Load<GameObject>(tmpNames[index]), Vector3.zero, Quaternion.identity);
                tmpGameObject.name = _name;
            }
            //�ӽ� ����Ʈ�� ���ϱ�
            tmpPools[index].Add(tmpGameObject);
            
            //����ȭ
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
            }
        }   
        return tmpGameObject;
    }

    int NametoIndex(string[] tmpNames, string _name) //������ƮǮ������ �����ϴ� ���ڿ��� ������ ��ȯ
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

    public void LeaveRoom()
    {
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
            else Debug.Log("������");
        }
        //������ �ƴ� �÷��̾��, ���� ���� ���� ������
        else if (!canExitRoom)//�������� �ƴ� ���, ���常 ������
        {
            Debug.Log("������");
        }
        else
        {
            battleUIManager.battleUI.SetActive(false);
            PhotonNetwork.LeaveRoom();
            SceneManager.LoadScene("Lobby");

        }
    }

    /*
     //�� ����
ExitGames.Client.Photon.Hashtable roomProperties = new ExitGames.Client.Photon.Hashtable();
//roomProperties.Add("IsAllowedToEnter", false);//������ �� ó�� ���� ���̻� ������
roomProperties.Add("IsAllowedToExit", true);//��� �߿��� ���� �� �ֵ���
PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperties);//��ο��� �����-------
     */
}
