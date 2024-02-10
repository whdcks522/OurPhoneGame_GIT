using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Assets.PixelHeroes.Scripts.ExampleScripts.CharacterControls;

public class Egg : MonoBehaviourPunCallbacks
{
    Rigidbody2D rigid;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Construction") || collision.transform.CompareTag("Player"))
        {
            if (photonView.IsMine)
            {
                photonView.RPC("eggJumpRPC", RpcTarget.All);
            }
        }
    }

    [PunRPC]
    public void eggJumpRPC()
    {
        rigid.AddForce(jumpVec);
    }
    Vector2 jumpVec = new Vector2(0, 15f);
}
