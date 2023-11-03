using Assets.PixelHeroes.Scripts.ExampleScripts;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Block : MonoBehaviourPunCallbacks
{
    [Header("����� ü��")]
    float maxHealth;
    public float curHealth;
    Rigidbody rigid;
    SpriteRenderer spriteRenderer;
    [Header("������� ��ġ")]
    public Vector3 createPos;

    [Header("�̵� ����")]
    public Vector3 dirVec = Vector3.right;

    //���� �Ŵ���
    GameManager gameManager;
    //�÷��̾� ��ũ��Ʈ
    CharacterControls characterControls;

    //�������ΰ�
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
        //���ӿ�����Ʈ Ȱ��ȭ
        gameObject.SetActive(true);
        //���� �ʱ�ȭ
        //isFalling = true;
        //ü�� ����
        curHealth = maxHealth;
        //���͸��� ����
        spriteRenderer.material.SetColor("_customColor" ,new Color(0, 0, 0, 1));
        //���� �ʱ�ȭ
        rigid.velocity = Vector3.zero;
        //��ġ �ʱ�ȭ
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
        //���ӿ�����Ʈ Ȱ��ȭ
        gameObject.SetActive(false);
    }

    [PunRPC]
    public void healthControl(float damage) 
    {
        curHealth -= damage;
        //���͸��� ����
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
        if (other.transform.CompareTag("Outline")) //�� ������ �������� ����
        {
            if (PhotonNetwork.InRoom)
            {
                if (photonView.IsMine)
                    //�Ѿ� �ı�
                    photonView.RPC("blockOnRPC", RpcTarget.AllBuffered);

            }
            else if (!PhotonNetwork.InRoom)
            {
                //�ٽ� �ҷ���
                blockOnRPC();
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //
        if (collision.transform.CompareTag("ConveyorIsland") || collision.transform.CompareTag("Block")) //�� ������ �������� ����
        {
            if (PhotonNetwork.InRoom)
            {
                //isFalling = false;
            }
            else if (!PhotonNetwork.InRoom)
            {
                //�ٽ� �ҷ���
                //isFalling = false;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.transform.CompareTag("ConveyorIsland") || collision.transform.CompareTag("Block")) //�� ������ �������� ����
        {
            if (PhotonNetwork.InRoom)
            {
                //isFalling = true;
            }
            else if (!PhotonNetwork.InRoom)
            {
                //�ٽ� �ҷ���
                //isFalling = true;
            }
        }
    }

}
