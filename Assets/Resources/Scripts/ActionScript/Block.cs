using Assets.PixelHeroes.Scripts.ExampleScripts;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Block : MonoBehaviourPunCallbacks
{
    [Header("블록의 체력")]
    float maxHealth;
    public float curHealth;
    Rigidbody rigid;
    SpriteRenderer spriteRenderer;
    [Header("재생성될 위치")]
    public Vector3 createPos;

    [Header("이동 방향")]
    public Vector3 dirVec = Vector3.right;

    //게임 매니저
    GameManager gameManager;
    //플레이어 스크립트
    CharacterControls characterControls;

    //낙하중인가
    //bool isFalling = true;

    public enum BlockType
    {
        Normal, UnBreakable
    }
    public BlockType blockType;

    private void Awake()
    {
        gameManager = GameManager.Instance;
        //characterControls = gameManager.cha

        maxHealth = curHealth;
        rigid = GetComponent<Rigidbody>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    [PunRPC]
    public void blockOnRPC()
    {
        //게임오브젝트 활성화
        gameObject.SetActive(true);
        //낙하 초기화
        //isFalling = true;
        //체력 관리
        curHealth = maxHealth;
        //매터리얼 관리
        spriteRenderer.material.SetColor("_customColor" ,new Color(0, 0, 0, 1));
        //가속 초기화
        rigid.velocity = Vector3.zero;
        //위치 초기화
        transform.position = createPos;
        
    }

    private void FixedUpdate()
    {
        //rigid.AddForce(Vector3.down * 3);

        //if (!isFalling) 
        {
            //rigid.velocity = dirVec * 1;
        }
    }

    [PunRPC]
    public void blockOffRPC()
    {
        //게임오브젝트 활성화
        gameObject.SetActive(false);
    }

    [PunRPC]
    public void healthControl(float damage) 
    {
        curHealth -= damage;
        //매터리얼 관리
        if (curHealth >= 80)
        {
            spriteRenderer.material.SetColor("_customColor", new Color(0.5f, 0.5f, 0.5f, 1));
        }
        else if (curHealth <= 60) 
        {
            spriteRenderer.material.SetColor("_customColor", new Color(1, 1, 1, 1));
        }
        else if (curHealth <= 40)
        {
            spriteRenderer.material.SetColor("_customColor", new Color(1, 0.5f, 0.5f, 1));
        }
        else if (curHealth <= 20)
        {
            spriteRenderer.material.SetColor("_customColor", new Color(1, 0, 0, 1));
        }

        if (curHealth <= 0)
        {
            if (PhotonNetwork.InRoom)
            {
                if (photonView.IsMine)
                {
                    photonView.RPC("blockOffRPC", RpcTarget.AllBuffered);
                }
            }
            else if (!PhotonNetwork.InRoom) 
            {
                blockOffRPC();
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Outline")) //맵 밖으로 나가지면 종료
        {
            if (PhotonNetwork.InRoom)
            {
                if (photonView.IsMine)
                    //총알 파괴
                    photonView.RPC("blockOnRPC", RpcTarget.AllBuffered);

            }
            else if (!PhotonNetwork.InRoom)
            {
                //다시 불러옴
                blockOnRPC();
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //
        if (collision.transform.CompareTag("ConveyorIsland") || collision.transform.CompareTag("Block")) //맵 밖으로 나가지면 종료
        {
            if (PhotonNetwork.InRoom)
            {
                //isFalling = false;
            }
            else if (!PhotonNetwork.InRoom)
            {
                //다시 불러옴
                //isFalling = false;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.transform.CompareTag("ConveyorIsland") || collision.transform.CompareTag("Block")) //맵 밖으로 나가지면 종료
        {
            if (PhotonNetwork.InRoom)
            {
                //isFalling = true;
            }
            else if (!PhotonNetwork.InRoom)
            {
                //다시 불러옴
                //isFalling = true;
            }
        }
    }

}
