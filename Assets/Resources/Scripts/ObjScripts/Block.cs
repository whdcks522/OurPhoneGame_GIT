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
        Normal, PowerUp, Cure
    }
    [Header("블록의 효과")]
    public BlockEffectType blockEffectType;

    //금간 색
    Color crackColor;
    //생성 됐을 때의 크기
    Vector3 sizeVec = Vector3.one;

    //플레이어 스크립트
    public GameManager gameManager;//지우지 말 것
    public CharacterControls characterControls;//지우지 말 것
    BattleUIManager battleUIManager;
    
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;

    public BoxCollider2D []boxColliders;//동시 충돌 해결할려고 만들었다가 안씀

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
    public void blockOnRPC()//False: 못부셔셔 진화한 경우
    {
        //게임오브젝트 활성화
        gameObject.SetActive(true);
        //폭탄에 한 번 만 맞도록
        threeCount = 0;
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
            crackColor = new Color(0.5f, 0.5f, 1, 1);
            //이펙트 아이콘 사이즈 관리
        }
        else if (blockEffectType == BlockEffectType.Cure)
        {
            //매터리얼 관리
            crackColor = new Color(0.5f, 1, 0.5f, 1);
            //이펙트 아이콘 사이즈 관리
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
            }
            else if (blockEffectType == BlockEffectType.Cure)
            {
                //파괴 효과음
                battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Heal);
                //파괴 이펙트
                GameObject effect = gameManager.CreateObj("Explosion 2_Cure", GameManager.PoolTypes.EffectType);
                effect.SetActive(true);
                effect.transform.position = transform.position;
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
            int damage = 2 * other.gameObject.GetComponent<Sword>().swordDamage;
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

    //콜라이더가 3개여서 충돌 제어용
    int threeCount = 0;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.CompareTag("Outline")) //맵 밖으로 나가지면 종료
        {
            if (PhotonNetwork.InRoom)
            {
                if (photonView.IsMine)
                {
                    if (++threeCount >= 3)
                    {
                        threeCount = 0;
                        //블록 파괴 실패
                        photonView.RPC("blockOffRPC", RpcTarget.All, false);
                    }
                }
            }
            else if (!PhotonNetwork.InRoom)
            {
                if (++threeCount >= 3) 
                {
                    threeCount = 0;
                    //블록 파괴 실패
                    blockOffRPC(false);
                }
            }
        }
        else if (other.transform.CompareTag("Bomb"))//블록과 충돌
        {
            Bomb bomb = other.GetComponent<Bomb>();

            if (PhotonNetwork.InRoom)//멀티의 경우
            {
                if (photonView.IsMine)
                {
                    if (++threeCount >= 3)
                    {
                        threeCount = 0;
                        //블록 부수기
                        photonView.RPC("healthControl", RpcTarget.All, bomb.bombDmg);
                    }
                }
            }
            else if (!PhotonNetwork.InRoom)//싱글의 경우
            {
                if (++threeCount >= 3)
                {
                    threeCount = 0;
                    //블록 부수기
                    healthControl(bomb.bombDmg);
                }
                
            }
        }
    }
}
