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
    [Header("�ν��Ͻ����� �����ϱ� ���� �÷��̾� ����")]
    public GameObject player;
    public CharacterControls characterControl;

    [Header("��Ƽ���� �÷��̾ ������ ��ġ")]
    public Transform[] spawnPositions;
    [Header("��Ƽ���� �÷��̾��� ����Ʈ")]
    public Transform playerGroup;

    //ī�޶�
    CinemachineVirtualCamera cinemachineVirtualCamera;
    BattleUIManager battleUIManager;


    //1. �Ѿ� ����Ʈ
    string[] bulletNames = { "NormalBullet", "NormalBulletHit","PowerUpBullet","PowerUpBulletHit",
                                "UnBreakableBullet", "UnBreakableBulletHit"};
    //�Ѿ� �ּҰ� ����� ��
    List<GameObject>[] bulletPools;

    //2. ��ź ����Ʈ
    string[] bombNames = {"Broken Phantasm" };
    //��ź �ּҰ� ����� ��
    List<GameObject>[] bombPools;

    //3. ��� ����Ʈ
    string[] blockNames = { "NormalBlock", "HardBlock", "PowerUpBlock", "CureBlock" };
    //��� �ּҰ� ����� ��
    List<GameObject>[] blockPools;

    //4. ����Ʈ ����Ʈ(2�� ���� ����, 6�� ���� ����),3�� �Ⱦ�
    string[] effectNames = { "Explosion 2", "Explosion 3", "Explosion 6", "Explosion 2_Cure", "Explosion 2_PowerUp",
                                "Text 52_Enemy", "Text 52_Player", "congratulation 9"};
    //����Ʈ �ּҰ� ����� ��
    List<GameObject>[] effectPools;

    //5. �ٶ� ����Ʈ
    string[] windNames = { "NormalWind" };
    //����Ʈ �ּҰ� ����� ��
    List<GameObject>[] windPools;

    //6. ������Ʈ ����Ʈ
    string[] objNames = { "Egg" };
    //����Ʈ �ּҰ� ����� ��
    List<GameObject>[] objPools;

    public enum PoolTypes
    {
        BulletType, BombType, BlockType, EffectType, WindType, ObjType //EnemyType, 
    }

    private void Awake()
    {
        battleUIManager = BattleUIManager.Instance;
        battleUIManager.gameManager = this;

        //1. �Ѿ� Ǯ �ʱ�ȭ
        bulletPools = new List<GameObject>[bulletNames.Length];
        for (int index = 0; index < bulletNames.Length; index++)//Ǯ �ϳ��ϳ� �ʱ�ȭ
            bulletPools[index] = new List<GameObject>();

        //2. ��ź Ǯ �ʱ�ȭ(4���� ����)
        bombPools = new List<GameObject>[bombNames.Length];
        for (int index = 0; index < bombNames.Length; index++)//Ǯ �ϳ��ϳ� �ʱ�ȭ
            bombPools[index] = new List<GameObject>();

        //3. ��� Ǯ �ʱ�ȭ(4���� ����)
        blockPools = new List<GameObject>[blockNames.Length];
        for (int index = 0; index < blockNames.Length; index++)//Ǯ �ϳ��ϳ� �ʱ�ȭ
            blockPools[index] = new List<GameObject>();

        //4. ����Ʈ Ǯ �ʱ�ȭ(4���� ����)
        effectPools = new List<GameObject>[effectNames.Length];
        for (int index = 0; index < effectNames.Length; index++)//Ǯ �ϳ��ϳ� �ʱ�ȭ
            effectPools[index] = new List<GameObject>();

        //5. �ٶ� Ǯ �ʱ�ȭ(4���� ����)
        windPools = new List<GameObject>[windNames.Length];
        for (int index = 0; index < windNames.Length; index++)//Ǯ �ϳ��ϳ� �ʱ�ȭ
            windPools[index] = new List<GameObject>();

        //6. ������Ʈ Ǯ �ʱ�ȭ(4���� ����)
        objPools = new List<GameObject>[objNames.Length];
        for (int index = 0; index < objNames.Length; index++)//Ǯ �ϳ��ϳ� �ʱ�ȭ
            objPools[index] = new List<GameObject>();
    }

    private void Start()
    {
        if (battleUIManager.battleType == BattleUIManager.BattleType.Multy)//Room�� �ִ��� ����� �۵� ���ϴ���
        {
            var localPlayerIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1;//���� �濡 ���� �÷��̾��� ��ȣ(1���� ����, �迭�� �̿���)
            var spawnPosition = spawnPositions[localPlayerIndex % spawnPositions.Length];//Ȥ�� ���� ����

            player = PhotonNetwork.Instantiate("Player", spawnPosition.position, Quaternion.identity);
            characterControl = player.GetComponent<CharacterControls>();
            //���� ��ȯ
            characterControl.GetComponent<PhotonView>().RPC("TurnRPC", RpcTarget.AllBuffered, (int)spawnPosition.localScale.x);
            characterControl.gameManager = this;
            player.transform.parent = playerGroup;
            

            //UI ��������
            battleUIManager.battleUI.SetActive(true);
            battleUIManager.bigRankText.gameObject.SetActive(false);
            battleUIManager.bigScoreText.gameObject.SetActive(false);
            battleUIManager.multyExitBtn.SetActive(true);
            battleUIManager.singleStopBtn.SetActive(false);
        }
        else if (battleUIManager.battleType == BattleUIManager.BattleType.Single)//�̱��� ���
        {
            //UI ��������
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
            case PoolTypes.WindType:
                tmpPools = windPools;
                tmpNames = windNames;//awake���� �����޴�
                break;
            case PoolTypes.ObjType:
                tmpPools = objPools;
                tmpNames = objNames;//awake���� �����޴�
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
                
            }
            //�̱� �÷��̶��
            else
            {
                tmpGameObject = Instantiate(Resources.Load<GameObject>(tmpNames[index]), Vector3.zero, Quaternion.identity);
                
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

    #region ������Ʈ Ǯ������ ���� ���
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

    #region ��Ƽ �÷��� ��� ����


    public void allLeaveRoomStart() //��� �濡�� ������
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
        */
    }
    #endregion


    /*
     //�� ����
        ExitGames.Client.Photon.Hashtable roomProperties = new ExitGames.Client.Photon.Hashtable();
        roomProperties.Add("IsAllowedToEnter", false);//������ �� ó�� ���� ���̻� ������
        roomProperties.Add("IsAllowedToExit", true);//��� �߿��� ���� �� �ֵ���
        PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperties);//��ο��� �����-------
     */
}
