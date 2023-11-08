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

    [Header("발생 지점")]
    public Transform[] blockPoints;

    [Header("파괴 시, 회복량")]
    public int blockHeal;

    //기본 금간 정도
    float crackValue = 5;
    [Header("추가로 금간 정도")]
    public int crackMul;

    public enum BlockHealthType
    {
       Zero, Quarter, Half, Full
    }
    [Header("블록의 체력 상태")]
    public BlockHealthType blockHealthType;

    public enum BlockEffectType
    {
        Normal, PowerUp
    }
    [Header("블록의 효과")]
    public BlockEffectType blockEffectType;

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
        //게임오브젝트 활성화
        gameObject.SetActive(true);
        //체력 관리
        curHealth = maxHealth;

        //매터리얼 관리
        crackValue = 5;
        if (blockEffectType == BlockEffectType.Normal)
        {
            crackColor = new Color(0.5f, 0.5f, 0.5f, 1);
            //크기 랜덤화
            sizeVec = new Vector3(Random.Range(1, 4), Random.Range(2, 4), 1);
            transform.localScale = sizeVec;
        }
        else if (blockEffectType == BlockEffectType.PowerUp)
        {
            //매터리얼 관리
            crackColor = new Color(0.5f, 1, 0.5f, 1);
        }
        

        spriteRenderer.material.SetColor("_customColor" , crackColor);
        spriteRenderer.material.SetFloat("_customFloat", crackValue * crackMul);
        //가속 초기화
        rigid.velocity = Vector3.zero;
        //위치 초기화
        int r = Random.Range(0, blockPoints.Length);
        transform.position = blockPoints[r].position;
        //등급 초기화
        blockHealthType = BlockHealthType.Full;
    }

    [PunRPC]
    public void blockOffRPC(bool isBreak)//true: 깨부셨을 경우
    {
        //게임오브젝트 활성화
        gameObject.SetActive(false);
        if (isBreak) //부순경우
        {
            characterControls.healControlRPC(blockHeal);

            if (blockEffectType == BlockEffectType.Normal) 
            {
                //파괴 효과음
                battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Heal);
                //파괴 이펙트
                GameObject effect = gameManager.CreateObj("Explosion 2", GameManager.PoolTypes.EffectType);
                effect.SetActive(true);
                effect.transform.position = transform.position;
                effect.transform.parent = transform.parent.transform;
            }
            else if (blockEffectType == BlockEffectType.PowerUp)
            {
                if (PhotonNetwork.InRoom)
                {
                    if (photonView.IsMine)
                    {
                        //무기 수 1 증가
                        characterControls.photonView.RPC("swordCountRPC", RpcTarget.AllBuffered, 1);
                    }
                }
                else if (!PhotonNetwork.InRoom)
                {
                    //무기 수 1 증가
                    characterControls.swordCountRPC(1);
                }

                //파괴 이펙트
                GameObject effect = gameManager.CreateObj("Explosion 2_PowerUp", GameManager.PoolTypes.EffectType);
                effect.SetActive(true);
                effect.transform.position = transform.position;
                effect.transform.parent = transform.parent.transform;
            }

        }
        else if (!isBreak)//떨어진 경우
        {
            //경고 효과음
            battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Warn);
            

            //강한 블록 생성
            GameObject block = gameManager.CreateObj("HardBlock", GameManager.PoolTypes.BlockType);
            Block blockComponent = block.GetComponent<Block>();

            //블록 부모 조정
            block.transform.parent = transform.parent.transform;
            blockComponent.blockPoints = blockPoints;

            //블록 활성화
            blockComponent.blockOnRPC();
        }
    }


    [PunRPC]
    public void healthControl(float damage) 
    {
        curHealth -= damage;
        //매터리얼 관리
        
        if (blockEffectType == BlockEffectType.Normal) 
        {
            if (curHealth >= maxHealth * 3f / 4f) { }
            else if (curHealth >= maxHealth / 2f)
            {
                if (blockHealthType != BlockHealthType.Half)
                {
                    //상태 변화
                    blockHealthType = BlockHealthType.Half;
                    //효과음
                    battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Block);
                    //매터리얼 관리
                    crackColor = new Color(1, 1, 1, 1);
                    //매터리얼 관리
                    crackValue = 7;
                }
            }
            else if (curHealth >= maxHealth * 1f / 4f)
            {
                if (blockHealthType != BlockHealthType.Quarter)
                {
                    //상태 변화
                    blockHealthType = BlockHealthType.Quarter;
                    //효과음
                    battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Block);
                    //이펙트
                    GameObject effect = gameManager.CreateObj("Explosion 6", GameManager.PoolTypes.EffectType);
                    effect.SetActive(true);
                    effect.transform.position = transform.position;
                    effect.transform.parent = transform.parent.transform;

                    //매터리얼 관리
                    crackColor = new Color(1, 0.5f, 0.5f, 1);
                    //매터리얼 관리
                    crackValue = 9;
                }
            }
            else if (curHealth > 0)
            {
                if (blockHealthType != BlockHealthType.Zero)
                {
                    //상태 변화
                    blockHealthType = BlockHealthType.Zero;
                    //효과음
                    battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Block);

                    //매터리얼 관리
                    
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
                    //상태 변화
                    blockHealthType = BlockHealthType.Zero;
                    //효과음
                    battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Block);

                    //매터리얼 관리
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
