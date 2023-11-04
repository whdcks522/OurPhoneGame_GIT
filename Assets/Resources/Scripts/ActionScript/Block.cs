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
    
    [Header("재생성될 위치")]
    public Vector3 createPos;

    [Header("파괴 시, 회복량")]
    public int blockHeal;

    [Header("추가로 금간 정도")]
    public int crackMul;

    public enum BlockType
    {
       Zero, Quarter, Half, Sum, Full
    }
    [Header("블록의 상태")]
    public BlockType blockType;

    //금간 색
    Color crackColor;
    //생성 됐을 때의 크기
    Vector3 sizeVec = Vector3.one;
    //게임 매니저
    GameManager gameManager;
    //플레이어 스크립트
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
    public void blockOnRPC()//False: 못부셔셔 진화한 경우
    {
        //크기 랜덤화
        sizeVec = new Vector3 (Random.Range(1,4), Random.Range(2, 4), 1);
        transform.localScale = sizeVec;

        //게임오브젝트 활성화
        gameObject.SetActive(true);
        //체력 관리
        curHealth = maxHealth;
        //매터리얼 관리
        spriteRenderer.material.SetColor("_customColor" ,new Color(0, 0, 0, 1));
        //가속 초기화
        rigid.velocity = Vector3.zero;
        //위치 초기화
        transform.position = createPos;
        //등급 초기화
        blockType = BlockType.Full;
    }

    [PunRPC]
    public void blockOffRPC(bool isBreak)//true: 깨부셨을 경우
    {
        //게임오브젝트 활성화
        gameObject.SetActive(false);
        if (isBreak) 
        {
            //파괴 효과음
            battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Heal);

            characterControls.healControlRPC(blockHeal);
        }
        else if (!isBreak)
        {
            //경고 효과음
            battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Warn);

            //강한 블록 생성
            GameObject block = gameManager.CreateObj("HardBlock", GameManager.PoolTypes.BlockType);
            Block blockComponent = block.GetComponent<Block>();

            //블록 부모 조정
            block.transform.parent = transform.parent.transform;
            blockComponent.createPos = createPos;

            //블록 활성화
            blockComponent.blockOnRPC();
        }
    }


    [PunRPC]
    public void healthControl(float damage) 
    {
        curHealth -= damage;
        //매터리얼 관리
        float crackValue = 5;
        if (curHealth >= maxHealth * 3f / 4f)//75
        {
            
            if (blockType != BlockType.Sum) 
            {
                blockType = BlockType.Sum;
                battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Block);
            }
            crackColor = new Color(0.5f, 0.5f, 0.5f, 1);
        }
        else if (curHealth >= maxHealth  / 2f) 
        {
            if (blockType != BlockType.Half)
            {
                blockType = BlockType.Half;
                battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Block);
            }
            crackColor = new Color(1, 1, 1, 1);
            crackValue = 7;
        }
        else if (curHealth >= maxHealth * 1f / 4f)
        {
            if (blockType != BlockType.Quarter)
            {
                blockType = BlockType.Quarter;
                battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Block);
            }
            crackColor = new Color(1, 0.5f, 0.5f, 1);
            crackValue = 9;
        }
        else if (curHealth > 0)
        {
            if (blockType != BlockType.Zero)
            {
                blockType = BlockType.Zero;
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
        if (other.transform.CompareTag("Outline")) //맵 밖으로 나가지면 종료
        {
            if (PhotonNetwork.InRoom)
            {
                if (photonView.IsMine)
                {
                    //블록 파괴
                    photonView.RPC("blockOffRPC", RpcTarget.AllBuffered, false);

                }

            }
            else if (!PhotonNetwork.InRoom)
            {
                //블록 파괴 실패
                blockOffRPC(false);
            }
        }
    }
}
