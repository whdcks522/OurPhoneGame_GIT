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
        public GameObject miniUI;
        public Image miniHealthGauge;
        public Text miniName;

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
        public Vector2[] createVec;
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

            //복장 커스텀
            //CharacterBuilder.Head = "Lizard#FFFFFF/0:0:0";
            CharacterBuilder.Rebuild();

            miniUI.SetActive(false);
            miniName.color = Color.red;
        }

        void Update()
        {
            KeyInput();
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

            /*
            if (PhotonNetwork.InRoom) 
            {
                if (photonView.IsMine)
                    _inputX = _inputY = 0;
            }
            else if (!PhotonNetwork.InRoom)
                _inputX = _inputY = 0;
            */

            //점프인 경우와 아닌 경우로 전환했을 때
            if ((isJumpAni && Character.GetState() != AnimationState.Jumping) || (!isJumpAni && Character.GetState() == AnimationState.Jumping)) //점프였다가 걷는 경우
            {
                JumpDust.Play();
            }

            #region 물리 바꿈

            /*

             if (Time.frameCount <= 1)
             {
                 Controller.Move(new Vector3(0, Gravity) * Time.fixedDeltaTime);
                 return;
             }

             var state = Character.GetState();

             if (state == AnimationState.Dead)//죽었는데 방향키 누르면 움직이도록
             {
                 _motion.y += Gravity;
                 Controller.Move(_motion * Time.fixedDeltaTime);

                 //if (_inputX == 0)
                 return;

                 //Character.SetState(AnimationState.Running);
             }

             if (_inputX != 0)//좌우 방향 전환
             {
                 if (PhotonNetwork.InRoom)
                     photonView.RPC("Turn", RpcTarget.AllBuffered, _inputX);//-------------
                 else
                     Turn(_inputX);
             }

             if (Controller.isGrounded)//땅에 있을 시
             {
                 if (state == AnimationState.Jumping)
                 {
                     if (Input.GetKey(KeyCode.X))
                     {
                         GetDown();
                     }
                     else
                     {
                         Character.Animator.SetTrigger("Landed");
                         Character.SetState(AnimationState.Ready);
                         JumpDust.Play(true);
                     }
                 }

                 _motion = state == AnimationState.Crawling
                     ? new Vector3(CrawlSpeed * _inputX, 0)
                     : new Vector3(RunSpeed * _inputX, JumpSpeed * _inputY);

                 if (_inputX != 0 || _inputY != 0)
                 {
                     if (_inputY > 0)
                     {
                         Character.SetState(AnimationState.Jumping);
                     }
                     else
                     {
                         switch (state)
                         {
                             case AnimationState.Idle:
                             case AnimationState.Ready:
                                 Character.SetState(AnimationState.Running);
                                 break;
                         }
                     }
                 }
                 else
                 {
                     switch (state)
                     {
                         case AnimationState.Crawling:
                         case AnimationState.Climbing:
                         case AnimationState.Blocking:
                             break;
                         default:
                             var targetState = Time.time - _activityTime > 5 ? AnimationState.Idle : AnimationState.Ready;

                             if (state != targetState)
                             {
                                 Character.SetState(targetState);
                             }

                             break;
                     }
                 }
             }//땅에 있을 시
             else//하늘에 있을 시
             {
                 _motion = new Vector3(RunSpeed * _inputX, _motion.y);
                 Character.SetState(AnimationState.Jumping);
             }

             _motion.y += Gravity;


             Controller.Move(_motion * Time.fixedDeltaTime);//!!!!!!!!!!!!!!이부분 빼면 안움직임
             //Controller.
             Character.Animator.SetBool("Grounded", Controller.isGrounded);
             Character.Animator.SetBool("Moving", Controller.isGrounded && _inputX != 0);
             Character.Animator.SetBool("Falling", !Controller.isGrounded && Controller.velocity.y < 0);

             if (_inputX != 0 && _inputY != 0 || Character.Animator.GetBool("Action"))
             {
                 _activityTime = Time.time;
             }

             _inputX = _inputY = 0;


             if (Controller.isGrounded && !Mathf.Approximately(Controller.velocity.x, 0))
             {
                 var velocity = MoveDust.velocityOverLifetime;

                 velocity.xMultiplier = 0.2f * -Mathf.Sign(Controller.velocity.x);

                 if (!MoveDust.isPlaying)
                 {
                     MoveDust.Play();
                 }
             }
             else
             {
                 MoveDust.Stop();
             }
             */
            #endregion
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
        void activeRPC(bool isDead)
        {
            curHealth = maxHealth;
        }

        /*
        void lateUpdate()
        {
            if (battleUIManager.btnContinue.activeSelf)//죽고 1.5초 후에 비활성화 됨
            {
                if (!isDead) //죽자 마자 정지
                {
                    //자연 체력 감소
                    curHealth -= healthMinus * Time.deltaTime;

                    //생존 파악
                    if (PhotonNetwork.InRoom)
                    {
                        if (photonView.IsMine)
                        {
                            //피격 처리
                            damageControlRPC(0, false);
                        }
                    }
                    else if (!PhotonNetwork.InRoom)
                    {
                        damageControlRPC(0, false);
                    }
                    //UI 관리-------------------------------------------------------

                    //미니 체력 바 적용
                    miniHealthGauge.fillAmount = curHealth / maxHealth;

                    //큰 체력바 적용
                    if (PhotonNetwork.InRoom)
                    {
                        if (photonView.IsMine)
                        {
                            float firstValue = battleUIManager.bigHealthBar.value;
                            battleUIManager.bigHealthBar.value = Mathf.Lerp(firstValue, curHealth / maxHealth, 1f);
                        }
                    }
                    else if (!PhotonNetwork.InRoom)
                    {
                        float firstValue = battleUIManager.bigHealthBar.value;
                        battleUIManager.bigHealthBar.value = Mathf.Lerp(firstValue, curHealth / maxHealth, 1f);
                    }

                    //시간에 따라 점수 증가
                    battleUIManager.curScore += Time.deltaTime * scorePlus;

                }//죽자 마자 정지

                //랭크와 점수 텍스트 적용
                battleUIManager.bigScoreText.text = Mathf.FloorToInt(battleUIManager.curScore) + "/";
                if (battleUIManager.curScore >= battleUIManager.Sscore) //S급 이상의 경우
                {
                    if (!isSRank)
                    {
                        battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.RankUp);
                        battleUIManager.rankType = BattleUIManager.RankType.S;
                        isSRank = true;

                    }
                    battleUIManager.bigRankText.text = "<color=#AA00FF> S </color>";
                    battleUIManager.bigScoreText.text += '-';
                }
                else if (battleUIManager.curScore >= battleUIManager.Ascore) //A급 이상의 경우
                {
                    if (!isARank)
                    {
                        battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.RankUp);
                        battleUIManager.rankType = BattleUIManager.RankType.A;
                        isARank = true;
                    }
                    battleUIManager.bigRankText.text = "<color=#0000FF> A </color>";
                    battleUIManager.bigScoreText.text += battleUIManager.Sscore;
                }
                else if (battleUIManager.curScore >= battleUIManager.Bscore) //B급 이상의 경우
                {
                    if (!isBRank)
                    {
                        battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.RankUp);
                        battleUIManager.rankType = BattleUIManager.RankType.B;
                        isBRank = true;
                    }
                    battleUIManager.bigRankText.text = "<color=#00AA00> B </color>";
                    battleUIManager.bigScoreText.text += battleUIManager.Ascore;
                }
                else if (battleUIManager.curScore >= battleUIManager.Cscore) //C급 이상의 경우
                {
                    if (!isCRank)
                    {
                        battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.RankUp);
                        battleUIManager.rankType = BattleUIManager.RankType.C;
                        isCRank = true;
                    }
                    battleUIManager.bigRankText.text = "<color=#FF0000> C </color>";
                    battleUIManager.bigScoreText.text += battleUIManager.Bscore;
                }
                else if (battleUIManager.curScore >= battleUIManager.Dscore) //D급 이상의 경우
                {
                    if (!isDRank)
                    {
                        battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.RankUp);
                        battleUIManager.rankType = BattleUIManager.RankType.D;
                        isDRank = true;
                    }
                    battleUIManager.bigRankText.text = "<color=#FFFF00> D </color>";
                    battleUIManager.bigScoreText.text += battleUIManager.Cscore;
                }
                else if (battleUIManager.curScore >= battleUIManager.Escore) //E급 이상의 경우
                {
                    battleUIManager.bigRankText.text = "<color=#FFFFFF> E </color>";
                    battleUIManager.bigScoreText.text += battleUIManager.Dscore;
                }
            }
        }

        private void LateUpdate()
        {
            lateUpdate();
        }
        */

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

                            textEffectComponent.photonView.RPC("effectNameRPC", RpcTarget.AllBuffered, _dmg.ToString());
                            textEffectComponent.photonView.RPC("effectOnRPC", RpcTarget.AllBuffered, transform.position);
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
                if (curHealth > 0)//피격
                {
                    //효과음
                    battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Damage);
                }

            }

            if (curHealth <= 0)
            {
                if (!isDead)
                {
                    if (PhotonNetwork.InRoom)
                    {
                        if (photonView.IsMine)
                        {
                            photonView.RPC("activeRPC", RpcTarget.AllBuffered, true);
                        }
                    }
                    else if (!PhotonNetwork.InRoom)
                    {
                        activeRPC(true);
                    }
                }
            }
        }
        #endregion



        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.transform.CompareTag("Outline")) //맵 밖으로 나가지면 종료
            {
                if (isRoom)
                {
                    if (photonView.IsMine && !isDead)
                    {
                        //피격 처리
                        photonView.RPC("transformRPC", RpcTarget.AllBuffered, true, Vector3.up * 10);
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