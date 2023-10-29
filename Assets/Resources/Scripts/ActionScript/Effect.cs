using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviourPunCallbacks
{
    [Header("이펙트의 최대 수명")]
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

    #region 총알 활성 동기화
    [PunRPC]
    public void effectOnRPC()
    {
        //게임오브젝트 활성화
        gameObject.SetActive(true);

        // 파티클 시스템을 다시 시작
        //particleSystem.Stop();
        //particleSystem.Play();
    }
    #endregion

    #region 총알 비활성 동기화
    [PunRPC]
    public void effectOffRPC()
    {
        //시간 동기화
        curTime = 0f;
        //게임오브젝트 비활성화
        gameObject.SetActive(false);
        // 파티클 시스템을 다시 시작
        //particleSystem.Stop();
        //particleSystem.Play();
    }
    #endregion
}
