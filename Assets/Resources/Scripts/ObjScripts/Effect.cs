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

    #region 이펙트 활성 동기화
    [PunRPC]
    public void effectOnRPC(Transform _Pos)
    {
        //게임오브젝트 활성화
        gameObject.SetActive(true);
        gameObject.transform.position = _Pos.position;

        //회전 조정
        gameObject.transform.forward = gameObject.transform.forward;
        //회전 조정
        //gameObject.transform.rotation = transform.rotation;
    }
    #endregion

    #region 이펙트 비활성 동기화
    [PunRPC]
    public void effectOffRPC()
    {
        //시간 동기화
        curTime = 0f;
        //게임오브젝트 비활성화
        gameObject.SetActive(false);
    }
    #endregion

    #region 이펙트 이름 동기화
    [PunRPC]
    public void effectNameRPC(string _str)
    {
        gameObject.name = _str;
    }
    #endregion
}
