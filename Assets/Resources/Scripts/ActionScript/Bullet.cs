using Assets.PixelHeroes.Scripts.ExampleScripts;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviourPunCallbacks
{
    public float maxTime;
    float curTime = 0f;
    public float bulletSpeed;

    private void Update()
    {
        curTime += Time.deltaTime;
        if (curTime > maxTime)
        {
            curTime = 0f;
            if (PhotonNetwork.InRoom && photonView.IsMine) 
            {
                photonView.RPC("bulletOffRPC", RpcTarget.AllBuffered);
            }
            else
            {
                bulletOffRPC();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("playerSword"))
        {
            Destroy(gameObject);
        }
    }

    [PunRPC]
    public void bulletOnRPC()
    {
        //등의 칼 활성화
        gameObject.SetActive(true);
        //Debug.Log("비활성화됨");
        //트레일 렌더러 삭제
        //trailRenderer.Clear();
    }

    [PunRPC]
    void bulletOffRPC()
    {
        //등의 칼 활성화
        gameObject.SetActive(false);
        //Debug.Log("비활성화됨");
        //트레일 렌더러 삭제
        //trailRenderer.Clear();
    }
}
