using Photon.Pun.Demo.PunBasics;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.PixelHeroes.Scripts.ExampleScripts;


public class Bomb : MonoBehaviourPunCallbacks
{
    [Header("폭탄의 최대 수명")]
    public float maxTime;
    float curTime = 0f;
    [Header("폭탄의 최대 수명")]
    public int bombDmg;
    ParticleSystem particleSystem;
    

    private void Awake()
    {
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

    #region 폭탄 활성 동기화
    [PunRPC]
    public void bombOnRPC(Vector3 bombPos)
    {
        //게임오브젝트 활성화
        gameObject.SetActive(true);
        gameObject.transform.position = bombPos;
        particleSystem.Play();
    }
    #endregion

    #region 폭탄 비활성 동기화
    [PunRPC]
    public void bombOffRPC()
    {
        //시간 동기화
        curTime = 0f;
        //게임오브젝트 비활성화
        gameObject.SetActive(false);
    }
    #endregion
}
