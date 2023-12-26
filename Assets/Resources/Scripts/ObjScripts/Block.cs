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

    //�⺻ �ݰ� ����
    float crackValue = 5;
    [Header("�߰��� �ݰ� ����")]
    public int crackMul;

    public enum BlockHealthType
    {
       Zero, Quarter, Half, Full
    }
    [Header("����� ü�� ����")]
    public BlockHealthType blockHealthType;

    public enum BlockEffectType
    {
        Normal, PowerUp, Cure
    }
    [Header("����� ȿ��")]
    public BlockEffectType blockEffectType;

    //�ݰ� ��
    Color crackColor;
    //���� ���� ���� ũ��
    Vector3 sizeVec = Vector3.one;

    //�÷��̾� ��ũ��Ʈ
    public GameManager gameManager;//������ �� ��
    public CharacterControls characterControls;//������ �� ��
    BattleUIManager battleUIManager;
    
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;

    public BoxCollider2D []boxColliders;//���� �浹 �ذ��ҷ��� ������ٰ� �Ⱦ�

    private void Awake()
    {
        battleUIManager = BattleUIManager.Instance;
        if(gameManager == null)
            gameManager = battleUIManager.gameManager;
        if (characterControls == null)
            characterControls = gameManager.characterControl;

        maxHealth = curHealth;
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }


    [PunRPC]
    public void blockOnRPC()//False: ���μż� ��ȭ�� ���
    {
        //���ӿ�����Ʈ Ȱ��ȭ
        gameObject.SetActive(true);
        //ü�� ����
        curHealth = maxHealth;

        //���͸��� ����
        crackValue = 5;
        if (blockEffectType == BlockEffectType.Normal)
        {
            crackColor = new Color(0.5f, 0.5f, 0.5f, 1);
            //ũ�� ����ȭ
            sizeVec = new Vector3(Random.Range(1, 4), Random.Range(2, 4), 1);
            transform.localScale = sizeVec;
        }
        else if (blockEffectType == BlockEffectType.PowerUp)
        {
            //���͸��� ����
            crackColor = new Color(0.5f, 0.5f, 1, 1);
        }
        else if (blockEffectType == BlockEffectType.Cure)
        {
            //���͸��� ����
            crackColor = new Color(0.5f, 1, 0.5f, 1);
        }


        spriteRenderer.material.SetColor("_customColor" , crackColor);
        spriteRenderer.material.SetFloat("_customFloat", crackValue * crackMul);
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
        //�ݶ��̴� ��Ȱ��ȭ

        if (isBreak) //�μ����
        {
            characterControls.healControlRPC(blockHeal);

            if (blockEffectType == BlockEffectType.Normal) 
            {
                //�ı� ȿ����
                battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Heal);
                //�ı� ����Ʈ
                GameObject effect = gameManager.CreateObj("Explosion 2", GameManager.PoolTypes.EffectType);
                effect.SetActive(true);
                effect.transform.position = transform.position;
            }
            else if (blockEffectType == BlockEffectType.PowerUp)
            {
                if (PhotonNetwork.InRoom)
                {
                    if (photonView.IsMine)
                    {
                        //���� �� 1 ����
                        characterControls.photonView.RPC("swordCountRPC", RpcTarget.AllBuffered, 1);
                    }
                }
                else if (!PhotonNetwork.InRoom)
                {
                    //���� �� 1 ����
                    characterControls.swordCountRPC(1);
                }

                //�ı� ����Ʈ
                GameObject effect = gameManager.CreateObj("Explosion 2_PowerUp", GameManager.PoolTypes.EffectType);
                effect.SetActive(true);
                effect.transform.position = transform.position;
                //effect.transform.parent = transform.parent.transform;
            }
            else if (blockEffectType == BlockEffectType.Cure)
            {
                //�ı� ȿ����
                battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Heal);
                //�ı� ����Ʈ
                GameObject effect = gameManager.CreateObj("Explosion 2_Cure", GameManager.PoolTypes.EffectType);
                effect.SetActive(true);
                effect.transform.position = transform.position;
                //effect.transform.parent = transform.parent.transform;
            }
        }
        else if (!isBreak)//������ ���
        {
            //��� ȿ����
            battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Warn);
            
            //���� ��� ����
            GameObject block = gameManager.CreateObj("HardBlock", GameManager.PoolTypes.BlockType);
            Block blockComponent = block.GetComponent<Block>();

            //��� �θ� ����
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
        
        if (blockEffectType == BlockEffectType.Normal) 
        {
            if (curHealth >= maxHealth * 3f / 4f) { }
            else if (curHealth >= maxHealth / 2f)
            {
                if (blockHealthType != BlockHealthType.Half)
                {
                    //���� ��ȭ
                    blockHealthType = BlockHealthType.Half;
                    //ȿ����
                    battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Block);
                    //���͸��� ����
                    crackColor = new Color(1, 1, 1, 1);
                    //���͸��� ����
                    crackValue = 7;
                }
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

                    //���͸��� ����
                    crackColor = new Color(1, 0.5f, 0.5f, 1);
                    //���͸��� ����
                    crackValue = 9;
                }
            }
            else if (curHealth > 0)
            {
                if (blockHealthType != BlockHealthType.Zero)
                {
                    //���� ��ȭ
                    blockHealthType = BlockHealthType.Zero;
                    //ȿ����
                    battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Block);

                    //���͸��� ����
                    
                    crackColor = new Color(1, 0, 0, 1);
                    crackValue = 11;
                } 
            }
        }
        else if (blockEffectType == BlockEffectType.PowerUp) 
        {
            if (curHealth >= maxHealth / 2f) { }
            else if (curHealth > 0)
            {
                if (blockHealthType != BlockHealthType.Zero)
                {
                    //���� ��ȭ
                    blockHealthType = BlockHealthType.Zero;
                    //ȿ����
                    battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Block);

                    //���͸��� ����
                    crackColor = new Color(0, 0, 0.6f, 1);
                    crackValue = 11;
                } 
            }
        }
        else if (blockEffectType == BlockEffectType.Cure)
        {
            if (curHealth >= maxHealth / 2f) { }
            else if (curHealth > 0)
            {
                if (blockHealthType != BlockHealthType.Zero)
                {
                    //���� ��ȭ
                    blockHealthType = BlockHealthType.Zero;
                    //ȿ����
                    battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Block);

                    //���͸��� ����
                    crackColor = new Color(0, 0.6f, 0, 1);
                    crackValue = 11;
                }
            }
        }

        spriteRenderer.material.SetColor("_customColor", crackColor);
        spriteRenderer.material.SetFloat("_customFloat", crackValue * crackMul);

        if (curHealth <= 0)
        {
            if (PhotonNetwork.InRoom)
            {
                if (photonView.IsMine)
                {
                    photonView.RPC("blockOffRPC", RpcTarget.All);
                }
            }
            else if (!PhotonNetwork.InRoom) 
            {
                blockOffRPC(true);
            }
        }
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.transform.CompareTag("playerSword"))
        {
            int damage = other.gameObject.GetComponent<Sword>().swordDamage;
            if (PhotonNetwork.InRoom)
            {
                if (photonView.IsMine)
                {
                    photonView.RPC("healthControl", RpcTarget.AllBuffered, Time.deltaTime * damage);
                }
            }
            else if (!PhotonNetwork.InRoom)
            {
                healthControl(Time.deltaTime * damage);
            }
        }
    }

    //�ݶ��̴��� 3������ �浹 �����
    int threeCount = 0;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.CompareTag("Outline")) //�� ������ �������� ����
        {
            if (PhotonNetwork.InRoom)
            {
                if (photonView.IsMine)
                {
                    if (++threeCount >= 3)
                    {
                        threeCount = 0;
                        //��� �ı� ����
                        photonView.RPC("blockOffRPC", RpcTarget.All, false);
                    }
                }
            }
            else if (!PhotonNetwork.InRoom)
            {
                if (++threeCount >= 3) 
                {
                    threeCount = 0;
                    //��� �ı� ����
                    blockOffRPC(false);
                }
            }
        }
        else if (other.transform.CompareTag("Bomb"))//��ϰ� �浹
        {
            Bomb bomb = other.GetComponent<Bomb>();

            if (PhotonNetwork.InRoom)//��Ƽ�� ���
            {
                if (photonView.IsMine)
                {
                    if (++threeCount >= 3)
                    {
                        threeCount = 0;
                        //��� �μ���
                        photonView.RPC("healthControl", RpcTarget.All, bomb.bombDmg);
                    }
                }
            }
            else if (!PhotonNetwork.InRoom)//�̱��� ���
            {
                if (++threeCount >= 3)
                {
                    threeCount = 0;
                    //��� �μ���
                    healthControl(bomb.bombDmg);
                }
                
            }
        }
    }
}
