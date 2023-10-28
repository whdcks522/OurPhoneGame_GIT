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
    [Header("���̽�ƽ�� ������ �޾Ƶ���")]
    //�÷��̾� �̵�
    public VariableJoystick moveJoy;
    //�÷��̾� Į
    public VariableJoystick swordJoy;
    [Header("�ν��Ͻ����� �����ϱ� ���� �÷��̾� ����")]
    public GameObject player;
    //ī�޶�
    CinemachineVirtualCamera cinemachineVirtualCamera;

    //������Ʈ Ǯ��
    //�ӽ÷� ���ӿ�����Ʈ ������ ��


    //�Ѿ� ����Ʈ
    string[] bulletNames = { "YellowStarBullet", "YellowStarFlash", "YellowStarHit"};

    List<GameObject>[] bulletPools;//������ �ּҰ� ����� ��
    

    //moveJoy = joySticks.transform.GetChild(0).GetComponent<VariableJoystick>();
    //swordJoy = joySticks.transform.GetChild(1).GetComponent<VariableJoystick>();

    public enum PoolTypes
    {
        EnemyType, BulletType
    }

    private void Awake()
    {
        //�Ѿ� Ǯ �ʱ�ȭ
        bulletPools = new List<GameObject>[bulletNames.Length];
        for (int index = 0; index < bulletNames.Length; index++)//Ǯ �ϳ��ϳ� �ʱ�ȭ
            bulletPools[index] = new List<GameObject>();


        //�÷��̾� ����
        //if ((PhotonNetwork.InRoom) && SceneManager.GetActiveScene().name == "Training")//��Ʈ��ũ �߿� �ִٸ�
        //    player = PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity);
        //else//�̱� �÷��̶��
        {
            player = Instantiate(Resources.Load<GameObject>("Player"), Vector3.zero, Quaternion.identity);
        }

        //ī�޶� ����
        cinemachineVirtualCamera = transform.GetChild(0).GetComponent<CinemachineVirtualCamera>();
        cinemachineVirtualCamera.Follow = player.transform;
        cinemachineVirtualCamera.LookAt = player.transform;
    }

    #region �̱���
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
            //���ϱ�
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
