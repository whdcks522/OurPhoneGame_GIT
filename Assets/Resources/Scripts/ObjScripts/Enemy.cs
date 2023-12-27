using Assets.PixelHeroes.Scripts.CharacterScripts;
using CartoonFX;
using Photon.Pun;
using Photon.Pun.Demo.Asteroids;
using Photon.Pun.Demo.PunBasics;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static Assets.PixelHeroes.Scripts.ExampleScripts.CharacterControls;
using AnimationState = Assets.PixelHeroes.Scripts.CharacterScripts.AnimationState;

namespace Assets.PixelHeroes.Scripts.ExampleScripts
{
    public class Enemy : MonoBehaviourPunCallbacks
    {
        [Header("----원래 있던 것----")]
        public Character Character;
        public CharacterBuilder CharacterBuilder;
        public CharacterController Controller; // https://docs.unity3d.com/ScriptReference/CharacterController.html
        public float RunSpeed = 1f;
        public float JumpSpeed = 3f;
        public float CrawlSpeed = 0.25f;
        public float Gravity = -0.2f;
        public ParticleSystem MoveDust;
        public ParticleSystem JumpDust;

        private Vector3 _motion = Vector3.zero;
        public int _inputX, _inputY;
        private float _activityTime;


        [Header("----새로 추가----")]
        [Header("적의 최대 체력")]
        public float maxHealth;
        [Header("적의 현재 체력")]
        public float curHealth;
        [Header("플레이어의 회복량")]
        public int playerHeal;
        [Header("적의 최대 공격 대기 시간")]
        public float maxTime;
        [Header("적의 임시 공격 대기 시간")]
        public float tmpTime;
        [Header("적의 현재 공격 대기 시간")]
        public float curTime;

        [Header("캐릭터 위의 미니 UI")]
        public GameObject miniUI;
        public Image miniHealth;

        //좌우 반전을 위해 필요함
        private Vector3 dirVec = new Vector3(1, 1, 1);

        public BattleUIManager battleUIManager;
        public GameManager gameManager;
        public Rigidbody2D rigid;
        [Header("Rigidbody 점프력")]
        public int jumpForce;

        [Header("레이의 반지름")]
        public float rayRadius;
        [Header("레이의 사거리")]
        public float raySize;
        [Header("레이의 시작점")]
        public Vector3 rayVec = Vector3.zero;
        //현재 바닥인지
        public bool isGround = false;
        Collider2D hitCol = null;

        //멀티인지 솔로인지
        bool isRoom;
        //죽었는지
        bool isDead;
        [Header("플레이어 객체")]
        public GameObject player;
        public CharacterControls characterControls;
        //[Header("머신러닝 중인지")]
        public bool isML;

        public enum EnemyType { Goblin, Orc }//쓸려나?
        public EnemyType enemyType;

        private void Awake()
        {
            battleUIManager = BattleUIManager.Instance;

            PhotonNetwork.SendRate = 60;
            PhotonNetwork.SerializationRate = 30;
            

            isRoom = PhotonNetwork.InRoom;
            //환복
            CharacterBuilder.Rebuild();
        }

        private void Start()
        {
            if (!isML) 
            {
                player = gameManager.player;
                characterControls = gameManager.characterControl;
            }
        }

        [PunRPC]
        public void activateRPC() 
        {
            isGround = false;
            curTime = 0;
            sfxLevel = 4;
            //가속 초기화
            rigid.velocity = Vector2.zero;
            //체력 회복
            curHealth = maxHealth;
            miniHealth.fillAmount = 1;
            //기존에 있던 것
            Character.SetState(AnimationState.Idle);

            //오브젝트 활성화
            isDead = false;
            
            gameObject.SetActive(true);
        }

        #region xy 동기화
        [PunRPC]
        public void xyRPC(int x, int y)
        {
            _inputX = x;
            _inputY = y;
        }
        #endregion

        #region 이동과 시간 축적
        public void FixedUpdate()
        {
            if (isDead)
            {
                rigid.velocity = new Vector3(0, rigid.velocity.y);
                return;
            }
            //시간 더하기
            curTime += Time.deltaTime;

            //if (Input.GetKeyDown(KeyCode.J)) Character.Animator.SetTrigger("Jab");

            if (_inputX != 0)//좌우 방향 전환
            {
                TurnRPC(_inputX);
            }

            bool isJumpAni = false;

            if (Character.GetState() == AnimationState.Jumping)
            {
                isJumpAni = true;//이미 점프 중인지 확인
            }


            isGround = false;
            RaycastHit2D[] rayHits = Physics2D.CircleCastAll(transform.position + rayVec, rayRadius, Vector3.down, raySize);
            foreach (RaycastHit2D hitObj in rayHits)
            {
                if (hitObj.transform.gameObject.layer.Equals(LayerMask.NameToLayer("Construction")) ||
                   hitObj.transform.gameObject.layer.Equals(LayerMask.NameToLayer("Block")) ||
                   hitObj.transform.gameObject.layer.Equals(LayerMask.NameToLayer("PlayerSword")) ||
                   hitObj.transform.gameObject.layer.Equals(LayerMask.NameToLayer("Obj")) ||
                   hitObj.transform.gameObject.layer.Equals(LayerMask.NameToLayer("Player")) ||
                   hitObj.transform.gameObject.layer.Equals(LayerMask.NameToLayer("Enemy")))
                {
                    hitCol = hitObj.collider;

                    if (!hitCol.isTrigger && hitObj.transform.gameObject != gameObject)//자신은 무시해야됨 
                    {
                        isGround = true;
                        break;
                    }
                    else //밑에 뭐가 없는 경우
                    {
                        if (MoveDust.isPlaying)
                            MoveDust.Stop();
                    }
                }
            }

            if (isGround)//바닥에 있을 때
            {
                if (_inputY > 0)//점프
                {
                    Character.SetState(AnimationState.Jumping);
                    rigid.velocity = new Vector2(rigid.velocity.x, jumpForce);

                    //런닝 먼지 중지
                    if (MoveDust.isPlaying)
                    {
                        MoveDust.Stop();
                    }
                }

                else if (_inputX != 0)//걷기
                {
                    Character.SetState(AnimationState.Running);
                    //런닝 먼지 시작
                    if (!MoveDust.isPlaying && Character.GetState() == AnimationState.Running)
                    {
                        MoveDust.Play();
                    }
                }
                else if (_inputX == 0)//서있기
                {
                    //런닝 먼지 중지
                    Character.SetState(AnimationState.Idle);

                    if (MoveDust.isPlaying)
                    {
                        MoveDust.Stop();
                    }
                }
            }
            else if (!isGround) //공중에 있을 때
            {
                if (Character.GetState() != AnimationState.Jumping)
                    Character.SetState(AnimationState.Jumping);
            }

            rigid.velocity = new Vector2(_inputX * RunSpeed, rigid.velocity.y);
            //점프인 경우와 아닌 경우로 전환했을 때
            if ((isJumpAni && Character.GetState() != AnimationState.Jumping) || (!isJumpAni && Character.GetState() == AnimationState.Jumping)) //점프였다가 걷는 경우
            {
                JumpDust.Play();
            }
        }
        #endregion

        #region Scale을 돌려서 좌우 반전 적용
        [PunRPC]
        public void TurnRPC(int direction)
        {
            var scale = Character.transform.localScale;

            scale.x = Mathf.Sign(direction) * Mathf.Abs(scale.x);

            Character.transform.localScale = scale;
            dirVec.x = direction;
            //UI와 검도 x축 전환
            miniUI.transform.localScale = dirVec;
        }
        #endregion

        #region 충돌 관리
        private void OnCollisionStay2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("playerSword"))//칼과 충돌했을 때
            {
                int damage = collision.gameObject.GetComponent<Sword>().swordDamage;
                if (isRoom)
                {
                    //내 플레이어에 남의 칼이 왔으며, 전투 허가가 났을 때
                    if (photonView.IsMine)
                    {
                        //피격 처리
                        photonView.RPC("damageControlRPC", RpcTarget.All, 25 * damage * Time.deltaTime);
                    }
                }
                else if (!isRoom)
                {
                    //피격 처리
                    damageControlRPC(50 * damage * Time.deltaTime);
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.transform.CompareTag("Outline")) //맵 밖으로 나가지면 사망
            {
                if (PhotonNetwork.InRoom)
                {
                    if (photonView.IsMine)
                    {
                        photonView.RPC("deadRPC", RpcTarget.All);
                    }
                }
                else if (!PhotonNetwork.InRoom)
                {
                    deadRPC();
                }
            }
            else if (other.gameObject.CompareTag("Bomb"))//폭탄과 충돌했을 때
            {
                int damage = other.gameObject.GetComponent<Bomb>().bombDmg;
                if (isRoom)
                {
                    //내 플레이어에 남의 칼이 왔으며, 전투 허가가 났을 때
                    if (photonView.IsMine)
                    {
                        //피격 처리
                        photonView.RPC("damageControlRPC", RpcTarget.All, damage * 3);
                    }
                }
                else if (!isRoom)
                {
                    //피격 처리
                    damageControlRPC(damage * 3);
                }
            }
        }

        #endregion

        //효과음의 계층 분류를 위함
        int sfxLevel = 4;

        #region 피격 처리
        [PunRPC]
        public void damageControlRPC(float _dmg)
        {
            if (isDead)
                return;

            //피해량 계산
            curHealth -= _dmg;
            if (curHealth <= 0) curHealth = 0;
            else if (curHealth > maxHealth) curHealth = maxHealth;
            //UI관리
            miniHealth.fillAmount = curHealth / maxHealth;

            //충격 초기화
            if (curHealth > 0)//피격
            {
                //번쩍
                Character.Blink();

                if (curHealth <= maxHealth * 0.75f && sfxLevel > 3) 
                {
                    //효과음
                    battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Block);
                    sfxLevel = 3;
                }
                else if (curHealth <= maxHealth * 0.5f && sfxLevel > 2)
                {
                    //효과음
                    battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Block);
                    sfxLevel = 2;
                }
                else if (curHealth <= maxHealth * 0.25f && sfxLevel > 1)
                {
                    //효과음
                    battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Block);
                    sfxLevel = 1;
                }
                else if (curHealth <= 0 && sfxLevel > 0)
                {
                    //효과음
                    battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Block);
                    sfxLevel = 0;
                }


            }

            if (curHealth <= 0)
            {
                if (PhotonNetwork.InRoom)
                {
                    if (photonView.IsMine)
                    {
                        photonView.RPC("deadRPC", RpcTarget.All);
                    }
                }
                else if (!PhotonNetwork.InRoom)
                {
                    deadRPC();
                }
            }
        }
        #endregion

        #region 사망처리
        [PunRPC]
        void deadRPC()
        {
            if (!isML) 
            {
                //사망 처리
                isDead = true;
                rigid.velocity = new Vector3(rigid.velocity.x, -jumpForce / 2);
                //애니메이션
                Character.SetState(AnimationState.Dead);
                //효과음
                battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Heal);
                //미니 UI 닫기
                miniHealth.fillAmount = 0;
                //먼지 종료
                MoveDust.Stop();
                JumpDust.Stop();
                //곧 죽음
                Invoke("SoonDieRPC", 1.5f);
            } 
        }
        #endregion

        #region 사망 뒤, 소멸
        void SoonDieRPC()//죽었고 조금 뒤, 죽음에 대한 처리
        {
            //게임오브젝트 활성화
            gameObject.SetActive(false);

            //파괴 효과음
            battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Heal);

            //체력 회복
            characterControls.healControlRPC(playerHeal);

            if (isRoom)
            {
                if (photonView.IsMine)
                {
                    //파괴 이펙트
                    GameObject effect = gameManager.CreateObj("Explosion 2", GameManager.PoolTypes.EffectType);
                    effect.SetActive(true);
                    effect.transform.position = transform.position;
                }
            }
            else if (!isRoom)
            {
                //파괴 이펙트
                GameObject effect2 = gameManager.CreateObj("Explosion 2", GameManager.PoolTypes.EffectType);
                effect2.SetActive(true);
                effect2.transform.position = transform.position;
            } 
        }
        #endregion

        public void reloadRPC(float tmpF, string ani)//재장전
        {
            switch (ani) 
            {
                //사격 애니메이션
                case "Shot":
                    Character.Animator.SetTrigger("Shot");
                    break;
                //베기 애니메이션
                case "Slash":
                    Character.Animator.SetTrigger("Slash");
                    break;
                default:
                    break;
            }

            maxTime = tmpTime + UnityEngine.Random.Range(-1 * tmpF, tmpF);
            curTime = 0;
        }
    }
}