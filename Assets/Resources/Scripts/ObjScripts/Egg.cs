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
        //게임오브젝트 활성화
        gameObject.SetActive(true);
    }

    [PunRPC]
    public void eggOffRPC()
    {
        //게임오브젝트 활성화
        gameObject.SetActive(false);
    }
}
