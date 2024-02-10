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
        //���ӿ�����Ʈ Ȱ��ȭ
        gameObject.SetActive(true);
    }

    [PunRPC]
    public void eggOffRPC()
    {
        //���ӿ�����Ʈ Ȱ��ȭ
        gameObject.SetActive(false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Construction") || collision.transform.CompareTag("Player"))
        {
            eggJumpRPC();
            return;
            Debug.Log("�浹 ���� 1");
            if (photonView.IsMine)
            {
                Debug.Log("�浹 ���� 2");
                photonView.RPC("eggJumpRPC", RpcTarget.All);
            }
        }
    }

    [PunRPC]
    public void eggJumpRPC()
    {
        rigid.velocity = new Vector2(rigid.velocity.x, 15);
    }
    Vector2 jumpVec = new Vector2(0, 50f);
}
