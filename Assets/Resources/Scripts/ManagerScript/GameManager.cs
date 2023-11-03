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
    [Header("�ν��Ͻ����� �����ϱ� ���� �÷��̾� ����")]
    public GameObject player;
    public CharacterController characterController;
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
    string[] blockNames = { "NormalBlock" };
    //��� �ּҰ� ����� ��
    List<GameObject>[] blockPools;

    public enum PoolTypes
    {
        BulletType, BombType, BlockType //EnemyType, 
    }

    private void Awake()
    {
        battleUIManager = BattleUIManager.Instance;
        battleUIManager.gameManager = this;
        //��������
        battleUIManager.battleUI.SetActive(true);
        battleUIManager.curScore = 0;
        battleUIManager.bigRankText.text = "E";
        battleUIManager.bigScoreText.text = 0 +" / "+ battleUIManager.Dscore;


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


        //�÷��̾� ����
        //if ((PhotonNetwork.InRoom) && SceneManager.GetActiveScene().name == "Training")//��Ʈ��ũ �߿� �ִٸ�
        //    player = PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity);
        //else//�̱� �÷��̶��
        if (!PhotonNetwork.InRoom)
        {
            player = Instantiate(Resources.Load<GameObject>("Player"), Vector3.zero, Quaternion.identity);
        }
        characterController = player.GetComponent<CharacterController>();

        //ī�޶� ����
        cinemachineVirtualCamera = transform.GetChild(0).GetComponent<CinemachineVirtualCamera>();
        cinemachineVirtualCamera.Follow = player.transform;
        cinemachineVirtualCamera.LookAt = player.transform;
    }

    #region �̱���
    private static GameManager instance;//���� �ۼ� ����
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

    
    public GameObject CreateObj(string name, PoolTypes poolTypes) //������ �� �θ���, ������ ����
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
                 //Debug.Log("�Ѿ� Ÿ�� ���õ�");
                 break;
            case PoolTypes.BombType:
                tmpPools = bombPools;
                tmpNames = bombNames;//awake���� �����޴�
                break;
            case PoolTypes.BlockType:
                tmpPools = blockPools;
                tmpNames = blockNames;//awake���� �����޴�
                break;
        }

        int index = NametoIndex(tmpNames, name);
       //�ִٸ� ã�ƺ�
        foreach (GameObject item in tmpPools[index])
        {
            if (!item.activeSelf)
            {
                //Debug.Log("������");
                tmpGameObject = item;
                break;
            }
        }

        //������ �����ϰ� select�� �Ҵ�
        if (!tmpGameObject)
        {
            //Debug.Log("������");

            //��Ʈ��ũ �߿� �ִٸ�
            if (PhotonNetwork.InRoom)
                tmpGameObject = PhotonNetwork.Instantiate(tmpNames[index], Vector3.zero, Quaternion.identity);
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

}
