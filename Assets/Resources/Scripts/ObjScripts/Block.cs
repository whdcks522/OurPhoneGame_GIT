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

    [Header("�߻� ����")]
    public Transform[] blockPoints;

    [Header("�ı� ��, ȸ����")]
    public int blockHeal;

    [Header("�߰��� �ݰ� ����")]
    public int crackMul;

    public enum BlockHealthType
    {
       Zero, Quarter, Half, Full
    }
    [Header("����� ü�� ����")]
    public BlockHealthType blockHealthType;

    //�ݰ� ��
    Color crackColor;
    //���� ���� ���� ũ��
    Vector3 sizeVec = Vector3.one;
    //���� �Ŵ���
    GameManager gameManager;
    //�÷��̾� ��ũ��Ʈ
    CharacterControls characterControls;
    BattleUIManager battleUIManager;
    Rigidbody rigid;
    SpriteRenderer spriteRenderer;

    private void Awake()
    {
        battleUIManager = BattleUIManager.Instance;
        gameManager = battleUIManager.gameManager;
        characterControls = gameManager.player.GetComponent<CharacterControls>();

        maxHealth = curHealth;
        rigid = GetComponent<Rigidbody>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    [PunRPC]
    public void blockOnRPC()//False: ���μż� ��ȭ�� ���
    {
        //ũ�� ����ȭ
        sizeVec = new Vector3 (Random.Range(1,4), Random.Range(2, 4), 1);
        transform.localScale = sizeVec;

        //���ӿ�����Ʈ Ȱ��ȭ
        gameObject.SetActive(true);
        //ü�� ����
        curHealth = maxHealth;
        //���͸��� ����
        crackColor = new Color(0.5f, 0.5f, 0.5f, 1);
        spriteRenderer.material.SetColor("_customColor" , crackColor);
        spriteRenderer.material.SetFloat("_customFloat", 5 * crackMul);
        //���� �ʱ�ȭ
        rigid.velocity = Vector3.zero;
        //��ġ �ʱ�ȭ
        int r = Random.Range(0, blockPoints.Length);
        transform.position = blockPoints[r].position;
        //��� �ʱ�ȭ
        blockHealthType = BlockHealthType.Full;
    }

    [PunRPC]
    public void blockOffRPC(bool isBreak)//true: ���μ��� ���
    {
        //���ӿ�����Ʈ Ȱ��ȭ
        gameObject.SetActive(false);
        if (isBreak) 
        {
            //�ı� ȿ����
            battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Heal);
            //�ı� ����Ʈ
            GameObject effect = gameManager.CreateObj("Explosion 3", GameManager.PoolTypes.EffectType);
            effect.SetActive(true);
            effect.transform.position = transform.position;
            effect.transform.parent = transform.parent.transform;

            characterControls.healControlRPC(blockHeal);
        }
        else if (!isBreak)
        {
            //��� ȿ����
            battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Warn);
            

            //���� ��� ����
            GameObject block = gameManager.CreateObj("HardBlock", GameManager.PoolTypes.BlockType);
            Block blockComponent = block.GetComponent<Block>();

            //��� �θ� ����
            block.transform.parent = transform.parent.transform;
            blockComponent.blockPoints = blockPoints;

            //��� Ȱ��ȭ
            blockComponent.blockOnRPC();
        }
    }


    [PunRPC]
    public void healthControl(float damage) 
    {
        curHealth -= damage;
        //���͸��� ����
        float crackValue = 5;

        if (curHealth >= maxHealth * 3f / 4f) 
        {

        }
        else if (curHealth >= maxHealth / 2f)
        {
            if (blockHealthType != BlockHealthType.Half)
            {
                //���� ��ȭ
                blockHealthType = BlockHealthType.Half;
                //ȿ����
                battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Block);
            }
            crackColor = new Color(1, 1, 1, 1);
            crackValue = 7;
        }
        else if (curHealth >= maxHealth * 1f / 4f)
        {
            if (blockHealthType != BlockHealthType.Quarter)
            {
                //���� ��ȭ
                blockHealthType = BlockHealthType.Quarter;
                //ȿ����
                battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Block);
                //����Ʈ
                GameObject effect = gameManager.CreateObj("Explosion 6", GameManager.PoolTypes.EffectType);
                effect.SetActive(true);
                effect.transform.position = transform.position;
                effect.transform.parent = transform.parent.transform;
            }
            crackColor = new Color(1, 0.5f, 0.5f, 1);
            crackValue = 9;
        }
        else if (curHealth > 0)
        {
            if (blockHealthType != BlockHealthType.Zero)
            {
                //���� ��ȭ
                blockHealthType = BlockHealthType.Zero;
                //ȿ����
                battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Block);
            }
            crackColor = new Color(1, 0, 0, 1);
            crackValue = 11;
        }

        //_customFloat
        spriteRenderer.material.SetColor("_customColor", crackColor);
        spriteRenderer.material.SetFloat("_customFloat", crackValue * crackMul);

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
                blockOffRPC(true);
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
                {
                    //��� �ı�
                    photonView.RPC("blockOffRPC", RpcTarget.AllBuffered, false);

                }

            }
            else if (!PhotonNetwork.InRoom)
            {
                //��� �ı� ����
                blockOffRPC(false);
            }
        }
    }
}
