using Photon.Pun.Demo.PunBasics;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.PixelHeroes.Scripts.ExampleScripts;


public class Bomb : MonoBehaviourPunCallbacks
{
    [Header("��ź�� �ִ� ����")]

    public float maxTime;
    float curTime = 0f;
    BattleUIManager battleUIManager;
    CharacterController characterController;
    ParticleSystem particleSystem;
    

    private void Awake()
    {
        battleUIManager = BattleUIManager.Instance.GetComponent<BattleUIManager>();
        characterController = battleUIManager.GetComponent<CharacterController>();
        particleSystem = GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        curTime += Time.deltaTime;
        if (curTime > maxTime)
        {
            if (PhotonNetwork.InRoom && photonView.IsMine)
            {
                photonView.RPC("bombOffRPC", RpcTarget.AllBuffered);
            }
            else
            {
                bombOffRPC();
            }
        }
    }

    #region ��ź Ȱ�� ����ȭ
    [PunRPC]
    public void bombOnRPC()
    {
        //���ӿ�����Ʈ Ȱ��ȭ
        gameObject.SetActive(true);
        particleSystem.Play();
    }
    #endregion

    #region ��ź ��Ȱ�� ����ȭ
    [PunRPC]
    public void bombOffRPC()
    {
        //�ð� ����ȭ
        curTime = 0f;
        //���ӿ�����Ʈ ��Ȱ��ȭ
        gameObject.SetActive(false);
    }
    #endregion

    private void OnTriggerEnter(Collider other)
    {
    }
}
