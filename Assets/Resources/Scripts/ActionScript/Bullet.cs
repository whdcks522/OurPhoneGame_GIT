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
        //���� Į Ȱ��ȭ
        gameObject.SetActive(true);
        //Debug.Log("��Ȱ��ȭ��");
        //Ʈ���� ������ ����
        //trailRenderer.Clear();
    }

    [PunRPC]
    void bulletOffRPC()
    {
        //���� Į Ȱ��ȭ
        gameObject.SetActive(false);
        //Debug.Log("��Ȱ��ȭ��");
        //Ʈ���� ������ ����
        //trailRenderer.Clear();
    }
}
