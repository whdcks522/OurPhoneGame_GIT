using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Egg : MonoBehaviourPunCallbacks
{
    //Rigidbody2D rigid;
    //public float speed;

    private void Awake()
    {
        //rigid = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        //speed = rigid.velocity.magnitude;
    }

    [PunRPC]
    public void eggOnRPC()
    {
        //���ӿ�����Ʈ Ȱ��ȭ
        gameObject.SetActive(true);
    }

    [PunRPC]
    public void eggOffRPC()
    {
        //���ӿ�����Ʈ Ȱ��ȭ
        gameObject.SetActive(false);
    }
}
