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
        public float curHealth;
        float maxHealth;

        [Header("캐릭터 위의 미니 UI")]
        public Image miniUI;

        //좌우 반전을 위해 필요함
        private Vector3 dirVec = new Vector3(1, 1, 1);

        BattleUIManager battleUIManager;
        public GameManager gameManager;
        Rigidbody2D rigid;
        [Header("Rigidbody 점프력")]
        public int jumpForce;

        [Header("레이의 반지름")]
        public float rayRadius;
        [Header("레이의 사거리")]
        public float raySize;
        [Header("레이의 시작점")]
        public Vector3 rayVec = Vector3.zero;
        //현재 바닥인지
        bool isGround = false;
        Collider2D hitCol = null;

        //멀티인지 솔로인지
        bool isRoom;
        //죽었는지
        bool isDead;
        public GameObject player;
        

        private void Awake()
        {
            battleUIManager = BattleUIManager.Instance;

            PhotonNetwork.SendRate = 60;
            PhotonNetwork.SerializationRate = 30;

            rigid = GetComponent<Rigidbody2D>();
            
            isRoom = PhotonNetwork.InRoom;
            //체력 동기화
            maxHealth = curHealth;
        }

        void Start()
        {
            //기존에 있던 것
            Character.SetState(AnimationState.Idle);
        }

        void Update()
        {
            KeyInput();
        }

        [PunRPC]
        void changeClothRPC(string[] arr = null)
        {
            
            CharacterBuilder.Head = arr[0];//헤드
            CharacterBuilder.Ears = arr[1];//귀
            CharacterBuilder.Eyes = arr[2];//눈
            CharacterBuilder.Body = arr[3];//눈
            CharacterBuilder.Hair = arr[4];//머리카락

            CharacterBuilder.Armor = arr[5];//갑옷
            CharacterBuilder.Helmet = arr[6];//모자
            CharacterBuilder.Weapon = arr[7];//무기
            CharacterBuilder.Shield = arr[8];//방패

            CharacterBuilder.Cape = arr[9];//망토
            CharacterBuilder.Back = arr[10];//등
            CharacterBuilder.Mask = arr[11];//마스크
            CharacterBuilder.Horns = arr[12];//뿔

            CharacterBuilder.Rebuild();

        }

        #region 키 입력
        void KeyInput()
        {
            if (curHealth <= 0 ) return;

            #region 안쓰는 애니메이션
            /*
            if (Input.GetKeyDown(KeyCode.A)) Character.Animator.SetTrigger("Attack");
            else if (Input.GetKeyDown(KeyCode.J)) Character.Animator.SetTrigger("Jab");
            else if (Input.GetKeyDown(KeyCode.P)) Character.Animator.SetTrigger("Push");
            else if (Input.GetKeyDown(KeyCode.H)) Character.Animator.SetTrigger("Hit");
            else if (Input.GetKeyDown(KeyCode.I)) { Character.SetState(AnimationState.Idle); _activityTime = 0; }
            else if (Input.GetKeyDown(KeyCode.R)) { Character.SetState(AnimationState.Ready); _activityTime = Time.time; }
            else if (Input.GetKeyDown(KeyCode.B)) Character.SetState(AnimationState.Blocking);
            else if (Input.GetKeyUp(KeyCode.B)) Character.SetState(AnimationState.Ready);
            else if (Input.GetKeyDown(KeyCode.D)) Character.SetState(AnimationState.Dead);

            
            else if (Input.GetKeyDown(KeyCode.Alpha1)) Character.Animator.SetTrigger("Slash");
            else if (Input.GetKeyDown(KeyCode.O)) Character.Animator.SetTrigger("Shot");
            else if (Input.GetKeyDown(KeyCode.F)) Character.Animator.SetTrigger("Fire1H");
            else if (Input.GetKeyDown(KeyCode.E)) Character.Animator.SetTrigger("Fire2H");
            else if (Input.GetKeyDown(KeyCode.Alpha2)) Character.SetState(AnimationState.Climbing);
            else if (Input.GetKeyUp(KeyCode.Alpha2)) Character.SetState(AnimationState.Ready);
            else if (Input.GetKeyUp(KeyCode.L)) Character.Blink();
            */
            #endregion
        }
        #endregion

        #region xy 동기화
        [PunRPC]
        void xyRPC(int x, int y)
        {
            _inputX = x;
            _inputY = y;
        }
        #endregion


        public void FixedUpdate()
        {
            if (isDead)
                return;

            //이동
            Move();
        }


        #region 이동
        private void Move()
        {
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
                   hitObj.transform.gameObject.layer.Equals(LayerMask.NameToLayer("Player")))
                {
                    hitCol = hitObj.collider;

                    if (!hitCol.isTrigger && hitObj.transform.gameObject != gameObject)//자신은 무시해야됨 
                    {
                        isGround = true;
                        break;
                    }
                }
            }

            if (isGround)//바닥에 있을 때
            {
                if (_inputY > 0)//점프
                {
                    Character.SetState(AnimationState.Jumping);
                    rigid.velocity = new Vector3(rigid.velocity.x, jumpForce);

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
        void TurnRPC(int direction)
        {
            var scale = Character.transform.localScale;

            scale.x = Mathf.Sign(direction) * Mathf.Abs(scale.x);

            Character.transform.localScale = scale;
            dirVec.x = direction;
            //UI와 검도 x축 전환
            miniUI.transform.localScale = dirVec;
        }
        #endregion

        [PunRPC]
        void deadRPC()
        {
            //사망 처리
            isDead = true;
            //애니메이션
            Character.SetState(AnimationState.Dead);
            //효과음
            battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Heal);
            //미니 UI 닫기
            miniUI.fillAmount = 0;
            //먼지 종료
            MoveDust.Stop();
            JumpDust.Stop();
            //속도 동기화(안하면 본체만 닿아서 날아가는 경우 있음)
            rigid.velocity = Vector2.zero;


            //곧 죽음
            if (!isRoom)
                Invoke("SoonDie", 1.5f);
        }

        [PunRPC]//죽었을 때 가속도 동기화를 위함
        void changeVelocity(Vector2 _tmpVec)
        {
            rigid.velocity = _tmpVec;
        }


        //죽었고 조금 뒤, 죽음에 대한 처리
        void SoonDie()
        {
            
        }

       

        private void LateUpdate()
        {
            miniUI.fillAmount = curHealth / maxHealth;
        }
        

        #region 피격 처리
        [PunRPC]
        public void damageControlRPC(int _dmg, bool isEffect)
        {
            //피해량 계산
            curHealth -= _dmg;
            if (curHealth <= 0) curHealth = 0;
            else if (curHealth > maxHealth) curHealth = maxHealth;

            //충격 초기화
            if (_dmg != 0)//피해가 있을 때
            {
                if (isEffect) 
                {
                    

                    if (PhotonNetwork.InRoom)
                    {
                        if (photonView.IsMine)
                        {
                            //피격 텍스트 이펙트
                            GameObject textEffect = gameManager.CreateObj("Text 52", GameManager.PoolTypes.EffectType);
                            Effect textEffectComponent = textEffect.GetComponent<Effect>();

                            textEffectComponent.photonView.RPC("effectNameRPC", RpcTarget.All, _dmg.ToString());
                            textEffectComponent.photonView.RPC("effectOnRPC", RpcTarget.All, transform.position);
                        }
                    }
                    else if (!PhotonNetwork.InRoom)
                    {
                        //피격 텍스트 이펙트
                        GameObject textEffect = gameManager.CreateObj("Text 52", GameManager.PoolTypes.EffectType);
                        Effect textEffectComponent = textEffect.GetComponent<Effect>();

                        textEffectComponent.effectNameRPC(_dmg.ToString());
                        textEffectComponent.effectOnRPC(transform.position);
                    }    
                }
            }

            if (curHealth > 0)//피격
            {
                //효과음
                battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Block);
                //번쩍
                Character.Blink();
            }
            else if (curHealth <= 0)
            {
                if (!isDead)
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
        }
        #endregion

        private void OnCollisionStay2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("playerSword"))
            {
                if (isRoom)
                {
                    //내 플레이어에 남의 칼이 왔으며, 전투 허가가 났을 때
                    if (photonView.IsMine)
                    {
                        //피격 처리
                        photonView.RPC("damageControlRPC", RpcTarget.All, 20, true);
                    }
                }
                else if (!isRoom) 
                {
                    //피격 처리
                    damageControlRPC(20, true);
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.transform.CompareTag("Outline")) //맵 밖으로 나가지면 종료
            {
                if (isRoom)
                {
                    if (photonView.IsMine && !isDead)
                    {
                        //피격 처리
                        photonView.RPC("transformRPC", RpcTarget.All, true, Vector3.up * 10);
                    }
                }
                else if (!isRoom && !isDead)
                {
                    transformRPC(true, Vector3.up * 10);
                }
            }
        }
        [PunRPC]

        void transformRPC(bool isOut, Vector2 tmpVec) 
        {
            if (isOut) //맵 밖으로 나간 경우
            {
                transform.position = player.transform.position + Vector3.up * 10;
            }
            else if (!isOut) //포탈 사용
            {
                transform.position = tmpVec;
            }
        }   
    }
}