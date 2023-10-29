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
            if (PhotonNetwork.InRoom && photonView.IsMine)
            {
                photonView.RPC("effectOffRPC", RpcTarget.AllBuffered);
            }
            else
            {
                effectOffRPC();
            }
        }
    }

    #region �Ѿ� Ȱ�� ����ȭ
    [PunRPC]
    public void effectOnRPC()
    {
        //���ӿ�����Ʈ Ȱ��ȭ
        gameObject.SetActive(true);

        // ��ƼŬ �ý����� �ٽ� ����
        //particleSystem.Stop();
        //particleSystem.Play();
    }
    #endregion

    #region �Ѿ� ��Ȱ�� ����ȭ
    [PunRPC]
    public void effectOffRPC()
    {
        //�ð� ����ȭ
        curTime = 0f;
        //���ӿ�����Ʈ ��Ȱ��ȭ
        gameObject.SetActive(false);
        // ��ƼŬ �ý����� �ٽ� ����
        //particleSystem.Stop();
        //particleSystem.Play();
    }
    #endregion
}
