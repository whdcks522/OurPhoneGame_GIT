using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviourPunCallbacks
{
    [Header("����Ʈ�� �ִ� ����")]
    public float maxTime;
    float curTime = 0f;

    private void Update()
    {
        curTime += Time.deltaTime;
        if (curTime > maxTime)
        {
            if (PhotonNetwork.InRoom)
            {
                if(photonView.IsMine)
                photonView.RPC("effectOffRPC", RpcTarget.AllBuffered);
            }
            else if(!PhotonNetwork.InRoom)
            {
                effectOffRPC();
            }
        }
    } 

    #region ����Ʈ Ȱ�� ����ȭ
    [PunRPC]
    public void effectOnRPC(Transform _Pos)
    {
        //���ӿ�����Ʈ Ȱ��ȭ
        gameObject.SetActive(true);
        gameObject.transform.position = _Pos.position;

        //ȸ�� ����
        gameObject.transform.forward = gameObject.transform.forward;
        //ȸ�� ����
        //gameObject.transform.rotation = transform.rotation;
    }
    #endregion

    #region ����Ʈ ��Ȱ�� ����ȭ
    [PunRPC]
    public void effectOffRPC()
    {
        //�ð� ����ȭ
        curTime = 0f;
        //���ӿ�����Ʈ ��Ȱ��ȭ
        gameObject.SetActive(false);
    }
    #endregion

    #region ����Ʈ �̸� ����ȭ
    [PunRPC]
    public void effectNameRPC(string _str)
    {
        gameObject.name = _str;
    }
    #endregion
}
