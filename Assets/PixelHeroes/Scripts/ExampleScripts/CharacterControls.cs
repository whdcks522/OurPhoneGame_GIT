﻿using Assets.PixelHeroes.Scripts.CharacterScripts;
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
using AnimationState = Assets.PixelHeroes.Scripts.CharacterScripts.AnimationState;

namespace Assets.PixelHeroes.Scripts.ExampleScripts
{
    public class CharacterControls : MonoBehaviourPunCallbacks, IPunObservable
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
        private float maxHealth;

        [Header("캐릭터 위의 미니 UI")]
        public GameObject miniUI;
        public Image miniHealthGauge;
        public Text miniName;

        //닉네임 색
        private Color redColor = new Color(197, 44, 28);
        private Color greenColor = new Color(1F, 98, 21, 255);
        private Color blueColor = new Color(37, 97, 192, 255);

        //좌우 반전을 위해 필요함
        private Vector3 dirVec = new Vector3(1,1,1);

        [Header("칼의 부모 게임오브젝트")]
        public GameObject swordParent;
        
        //칼 게임 오브젝트 배열
        public GameObject[] playerSwords;
        //속도 공유를 위함
        Vector3[] swordsRpcPos = new Vector3[8];
        private Sword SwordComponent;

        [Header("현재 칼의 갯수")]
        public int curSwordCount;

        [Header("등의 있는 칼 게임오브젝트")]
        public GameObject backSwords;

        [Header("칼 영역 표시 게임오브젝트")]
        public SpriteRenderer playerSwordArea;
        Color swordAreaColor;
        
        [Header("조이스틱의 정보를 받아들임")]
        //플레이어 이동
        public VariableJoystick moveJoy;
        Vector2 moveJoyVec;
        //플레이어 칼
        public VariableJoystick swordJoy;
        Vector2 swordJoyVec;

        //속도 공유를 위함
        Vector3 rpcPos;

        //랭크 업 효과음을 위한 bool
        bool isSRank, isARank, isBRank, isCRank, isDRank;
        
        [Header("체력 감소율")]
        public float healthMinus = 0;
        [Header("점수 증가율")]
        public float scorePlus = 0;

        public BattleUIManager battleUIManager;
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
        [Header("현재 바닥인지")]
        public bool isGround = false;
        //튜토리얼에서 부양을 보여주기 위함
        int playerLayer;
        int playerSwordLayer; 

        //플레이어의 상태--------------
        public enum PlayerStateType
        {
           None, Dead, LeftControl, IsCanJump, RightControl, CanHeal, SwordCount, SwordCollision, SwordFight
        }
        //이미 죽음
        public bool isDead = false;
        //이동이 가능한 상태인지
        bool isLeftControl = false;
        //점프가 가능한 상태인지
        bool isCanJump = true;
        //칼을 던질 수 있는 상태인지
        bool isRightControl = false;
        //회복 가능한 상태인지
        bool isCanHeal = false;
        //칼로 전투할 것인지
        bool isSwordFight = false;
        //PC로 진행중인지 확인
        bool isPC;
        //텍스트를 위한 반복하는 문장 코루틴
        Coroutine loopTypingCor;

        [PunRPC]
        void changeVelocity(Vector2 _tmpVec) 
        {
            rigid.velocity = _tmpVec;
        }

        [PunRPC]
        public void changeStateRPC(PlayerStateType tmpPlayerStateType, bool isCheck)
        {
            switch (tmpPlayerStateType) 
            {
                case PlayerStateType.Dead:
                    if (isCheck)
                    {
                        //사망 처리
                        isDead = true;
                        //애니메이션
                        Character.SetState(AnimationState.Dead);
                        //체력 처리
                        curHealth = 0;
                        miniHealthGauge.fillAmount = 0;
                        battleUIManager.bigHealthBar.value = 0;
                        //효과음
                        battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.TimeOver);
                        //칼 비활성화
                        SwordComponent.leaderSwordExitRPC(2);
                        miniUI.SetActive(false);
                        //속도 동기화(안하면 본체만 닿아서 날아가는 경우 있음)
                        rigid.velocity = Vector2.zero;

                        if (PhotonNetwork.InRoom)
                            photonView.RPC("changeVelocity", RpcTarget.AllBuffered, rigid.velocity);

                        //곧 죽음
                        if (!PhotonNetwork.InRoom)
                            Invoke("SoonDie", 1.5f);
                    }
                    else if (!isCheck)
                    {
                        Debug.Log("isDead?!");
                    }
                    break;
                case PlayerStateType.LeftControl:
                    isLeftControl = isCheck;
                    break;
                case PlayerStateType.IsCanJump:
                    isCanJump = isCheck;
                    break;
                case PlayerStateType.RightControl:
                    isRightControl = isCheck;
                    break;
                case PlayerStateType.CanHeal:
                    isCanHeal = isCheck;
                    break;
                case PlayerStateType.SwordCount://true면 무기 꽉 채우기, false면 1로 만들기
                    if (isCheck) 
                    {
                        swordCountRPC(9);
                        backSwords.SetActive(true);
                    }
                    else if (!isCheck)
                    {
                        swordCountRPC(-9);
                    }
                    break;
                case PlayerStateType.SwordCollision://true면 충돌, false면 무시
                    //플레이어와 칼 레이어 관리
                    playerLayer = LayerMask.NameToLayer("Player");
                    playerSwordLayer = LayerMask.NameToLayer("PlayerSword");

                    Physics2D.IgnoreLayerCollision(playerLayer, playerSwordLayer, !isCheck);
                    break;
                case PlayerStateType.SwordFight:
                    isSwordFight = isCheck;
                    break;
            }
        }

        private void Awake()
        {
            PhotonNetwork.SendRate = 80;
            PhotonNetwork.SerializationRate = 40;

            rigid = GetComponent<Rigidbody2D>();
            
            battleUIManager = BattleUIManager.Instance;
            isPC = battleUIManager.isPC;
            //칼 관리
            for (int i = 0; i < swordParent.transform.childCount; i++)
            {
                GameObject tmpSword = swordParent.transform.GetChild(i).gameObject;
                tmpSword.GetComponent<Sword>().player = gameObject;
                tmpSword.GetComponent<Sword>().characterControls = this;
                tmpSword.GetComponent<Sword>().battleUIManager = battleUIManager;
            }

            SwordComponent = playerSwords[0].GetComponent<Sword>();

            moveJoy = BattleUIManager.Instance.moveJoy;
            swordJoy = BattleUIManager.Instance.swordJoy;
            //체력 동기화
            maxHealth = curHealth;

            

            //멀티의 변수 관리
            if (battleUIManager.battleType == BattleUIManager.BattleType.Single)
            {
                changeStateRPC(PlayerStateType.LeftControl, true);
                changeStateRPC(PlayerStateType.RightControl, true);
                changeStateRPC(PlayerStateType.CanHeal, true);
            }
            else if (battleUIManager.battleType == BattleUIManager.BattleType.Multy)
            {
                changeStateRPC(PlayerStateType.LeftControl, true);
                changeStateRPC(PlayerStateType.RightControl, true);


                //체력 감소율을 0으로
                healthMinus = 0;
                //점수 증가율을 0으로
                scorePlus = 0;
            }
            //자신의 미니 UI 안보이게
            if (PhotonNetwork.InRoom) 
            {
                if (photonView.IsMine)//자신의 것은
                    miniUI.SetActive(false);//미니 UI 비활성화
                else if (!photonView.IsMine)//남의 것은
                    playerSwordArea.gameObject.SetActive(false);//칼 영역 비활성화
            }
            else if (!PhotonNetwork.InRoom)
            {
                    miniUI.SetActive(false);
            }
        }

        void Start()
        {
            gameManager = battleUIManager.gameManager;

            //기존에 있던 것
            Character.SetState(AnimationState.Idle);
            
            //복장 커스텀
            //CharacterBuilder.Head = "Lizard#FFFFFF/0:0:0";
            //CharacterBuilder.Rebuild();
            
            if (PhotonNetwork.InRoom)
            {
                miniName.text = photonView.IsMine ? PhotonNetwork.NickName : photonView.Owner.NickName;//나라면 내이름, 다른 사람이면 다른 사람 이름
                miniName.color = photonView.IsMine ? Color.green : Color.red;
            }
            else if (!PhotonNetwork.InRoom)
            {
                //미니 ui 설정
                miniName.color = Color.green;
            }
            transform.parent = gameManager.playerGroup;
        }

        

        void Update()
        {
            if (PhotonNetwork.InRoom)
            {
                if (photonView.IsMine)//타인의 것이면 안건듬
                {
                    //키 입력
                    KeyInput();
                }
                else if (!photonView.IsMine) 
                {
                    //플레이어 위치 관리
                    if ((transform.position - rpcPos).sqrMagnitude >= 2)//너무 멀면 순간이동, 12
                    {
                        Debug.LogWarning("PlayerQuickMove");
                        transform.position = rpcPos;
                    }
                    else
                    {
                        transform.position = Vector3.Lerp(transform.position, rpcPos, Time.deltaTime * 40);
                    }

                    //칼 위치 조정
                    for (int i = 0; i < 8; i++) 
                    {
                        if ((playerSwords[i].transform.position - swordsRpcPos[i]).sqrMagnitude >= 1)//너무 멀면 순간이동 
                        {
                            //playerSwords[i].transform.position = swordsRpcPos[i];
                        }
                        else
                        {
                            //Vector3.Lerp(playerSwords[i].transform.position, swordsRpcPos[i], Time.deltaTime * 40);
                        }
                    }
                }
                
            }
            else if (!PhotonNetwork.InRoom)
            {
                //키 입력
                KeyInput();
            }
        }

        #region 키 입력
        void KeyInput() 
        {
            if (curHealth <= 0 || !isLeftControl) return;

            if (Input.GetKeyDown(KeyCode.A)) Character.Animator.SetTrigger("Attack");
            else if (Input.GetKeyDown(KeyCode.J)) Character.Animator.SetTrigger("Jab");
            else if (Input.GetKeyDown(KeyCode.P)) Character.Animator.SetTrigger("Push");
            else if (Input.GetKeyDown(KeyCode.H)) Character.Animator.SetTrigger("Hit");
            else if (Input.GetKeyDown(KeyCode.I)) { Character.SetState(AnimationState.Idle); _activityTime = 0; }
            else if (Input.GetKeyDown(KeyCode.R)) { Character.SetState(AnimationState.Ready); _activityTime = Time.time; }
            else if (Input.GetKeyDown(KeyCode.B)) Character.SetState(AnimationState.Blocking);
            else if (Input.GetKeyUp(KeyCode.B)) Character.SetState(AnimationState.Ready);
            else if (Input.GetKeyDown(KeyCode.D)) Character.SetState(AnimationState.Dead);

            #region 안쓰는 부분
            // Builder characters only.
            /*
            else if (Input.GetKeyDown(KeyCode.Alpha1)) Character.Animator.SetTrigger("Slash");
            else if (Input.GetKeyDown(KeyCode.O)) Character.Animator.SetTrigger("Shot");
            else if (Input.GetKeyDown(KeyCode.F)) Character.Animator.SetTrigger("Fire1H");
            else if (Input.GetKeyDown(KeyCode.E)) Character.Animator.SetTrigger("Fire2H");
            else if (Input.GetKeyDown(KeyCode.Alpha2)) Character.SetState(AnimationState.Climbing);
            else if (Input.GetKeyUp(KeyCode.Alpha2)) Character.SetState(AnimationState.Ready);
            else if (Input.GetKeyUp(KeyCode.L)) Character.Blink();
            */
            #endregion

            int tmpX = 0, tmpY = 0;

            if (isPC)
            {
                /*
                if (Controller.isGrounded)
                {
                    if (Input.GetKeyDown(KeyCode.X))
                    {
                        GetDown();
                    }
                    else if (Input.GetKeyUp(KeyCode.X))
                    {
                        GetUp();
                    }
                }
                */

                if (Input.GetKey(KeyCode.Z))
                {
                    tmpX = -1;
                }
                else if (Input.GetKey(KeyCode.C))
                {
                    tmpX = 1;
                }
                if (Input.GetKey(KeyCode.S))//Down 지워버림
                {
                    tmpY = 1;
                }
            }//PC
            else if (!isPC)//조이스틱
            {
                moveJoyVec.x = moveJoy.Horizontal;
                moveJoyVec.y = moveJoy.Vertical;
                tmpX = (int)moveJoyVec.x;

                if (moveJoyVec.y >= 0.65f)
                {
                    tmpY = 1;
                }

            }//조이스틱

            if(!isCanJump)
                tmpY = 0;

            if (PhotonNetwork.InRoom)//여기까진 애초에 본인만 올 수 있음
            {
                if(tmpX != _inputX || tmpY != _inputY)
                    photonView.RPC("xyRPC", RpcTarget.AllBuffered, tmpX, tmpY);
            }
            else if (!PhotonNetwork.InRoom)
            {
                if (tmpX != _inputX || tmpY != _inputY)
                    xyRPC(tmpX, tmpY);
            }
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

            if (isRightControl)
            {
                if (PhotonNetwork.InRoom)
                {
                    if (photonView.IsMine)//타인의 것이면 안건듬
                    {
                        //칼 부모 위치 초기호와 검 사거리 표시
                        SwordDirCheck();
                        //PC, 조이스틱에 따른 칼의 움직임 조정
                        SwordInput();
                    }
                }
                else if (!PhotonNetwork.InRoom)
                {
                    //칼 부모 위치 초기호와 검 사거리 표시
                    SwordDirCheck();
                    //PC, 조이스틱에 따른 칼의 움직임 조정
                    SwordInput();
                }
            }
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

            //if (rigid.velocity.y <= 4) 
            {
                isGround = false;
                RaycastHit2D[] rayHits = Physics2D.CircleCastAll(transform.position + rayVec, rayRadius, Vector3.down, raySize);
                foreach (RaycastHit2D hitObj in rayHits) 
                {
                    if(hitObj.transform.gameObject.layer.Equals(LayerMask.NameToLayer("Construction")) ||
                       hitObj.transform.gameObject.layer.Equals(LayerMask.NameToLayer("Block"))||
                       hitObj.transform.gameObject.layer.Equals(LayerMask.NameToLayer("PlayerSword"))||
                       hitObj.transform.gameObject.layer.Equals(LayerMask.NameToLayer("Obj")) ||
                       hitObj.transform.gameObject.layer.Equals(LayerMask.NameToLayer("Player")))
                    {
                        if (hitObj.transform.gameObject != gameObject) 
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
                    if(Character.GetState() != AnimationState.Jumping)
                        Character.SetState(AnimationState.Jumping);
                }
            }

            rigid.velocity = new Vector2(_inputX* RunSpeed, rigid.velocity.y);

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
            if ((isJumpAni && Character.GetState() != AnimationState.Jumping)|| (!isJumpAni && Character.GetState() == AnimationState.Jumping)) //점프였다가 걷는 경우
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
            swordParent.transform.localScale = dirVec;
        }
        #endregion

        #region 일어나고 엎어지기
        private void GetDown()
        {
            Character.Animator.SetTrigger("GetDown");
        }

        private void GetUp()
        {
            Character.Animator.SetTrigger("GetUp");
        }
        #endregion




        #region 칼 부모 위치 초기화와 검 사거리 표시
        void SwordDirCheck()
        {
            swordAreaColor = new Color(0.6f, 0, 0, SwordComponent.swordDir);
            playerSwordArea.color = swordAreaColor;
        }
        #endregion

        #region PC, 조이스틱에 따른 칼의 벡터 입력
        private void SwordInput()
        {

            if (isPC)
            {
                int tmpX = 0, tmpY = 0;
                if (Input.GetKey(KeyCode.RightArrow))
                {
                    tmpX = 1;
                }
                if (Input.GetKey(KeyCode.LeftArrow))
                {
                    tmpX = -1;
                }
                if (Input.GetKey(KeyCode.UpArrow))
                {
                    tmpY = 1;
                }
                if (Input.GetKey(KeyCode.DownArrow))
                {
                    tmpY = -1;
                }

                swordJoyVec.x = tmpX;
                swordJoyVec.y = tmpY;
            }
            else if (!isPC) //모바일의 경우
            {
                swordJoyVec.x = swordJoy.Horizontal;
                swordJoyVec.y = swordJoy.Vertical;
            }
            //길이 감소
            swordJoyVec = swordJoyVec.normalized; 

            //검 이동
            SwordMove(SwordComponent.saveSwordVec == swordJoyVec);//조작이 이전과 같은지
        }
        #endregion

        #region 이전과 조작이 다르면 칼을 움직임
        void SwordMove(bool isSame) 
        {
            //칼이 활성화돼있을 때, 방향 조작시
            if (playerSwords[0].activeSelf && !isSame) // 
            {
                if (PhotonNetwork.InRoom) //2인 이상이라면
                    photonView.RPC("SwordSpinRPC", RpcTarget.AllBuffered, swordJoyVec);
                else
                    SwordSpinRPC(swordJoyVec);//1인이라면
            }
            else if (!playerSwords[0].activeSelf && !isSame)//칼이 비활성화 돼있을 시
            {
                if (PhotonNetwork.InRoom) //2인 이상이라면
                {
                    photonView.RPC("SwordActiveRPC", RpcTarget.AllBuffered);
                    photonView.RPC("SwordSpinRPC", RpcTarget.AllBuffered, swordJoyVec);
                }
                else//1인이라면
                {
                    SwordActiveRPC();
                    SwordSpinRPC(swordJoyVec);
                }
            }
        }
        #endregion

        #region 칼 회전 동기화
        [PunRPC]
        void SwordSpinRPC(Vector2 tmpVec)
        {
            if (tmpVec.x == 0 && tmpVec.y == 0)//사용 안할 시, 다시 수납
            {
                SwordComponent.leaderSwordExitRPC(0);
            }

            SwordComponent.saveSwordVec = tmpVec;//방향 조정
        }
        #endregion

        #region 칼 활성 동기화
        [PunRPC]
        void SwordActiveRPC() 
        {
            Debug.LogWarning("칼 활성화");

            playerSwords[0].transform.position = transform.position + Vector3.up * 0.5f + (Vector3)swordJoyVec ;
            playerSwords[0].SetActive(true);
            SwordComponent.trailRenderer.Clear();
            //등의 검 비활성화
            backSwords.SetActive(false);
        }
        #endregion

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

        #region 무기 변화 동기화
        [PunRPC]
        public bool swordCountRPC(int value)
        {
            if (value > 0)
            {
                curSwordCount += value;
                if (curSwordCount > 7) curSwordCount = 8;
                //파워 업 효과음
                battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.PowerUp);
            }
            else if (value < 0) 
            {
                curSwordCount += value;
                if (curSwordCount < 1) 
                {
                    curSwordCount = 1;
                    return false;//무기가 충분하지 않음
                }
                //무기 파괴 효과음
                battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Broken);
            }
            return true;//남은 무기가 없는 경우
        }
        #endregion

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.transform.CompareTag("EnemyBullet"))
            {
                Bullet bullet = other.GetComponent<Bullet>();
                int dmg = bullet.bulletDamage;

                if (!bullet.isAlreadyHit) 
                {
                    if (PhotonNetwork.InRoom)
                    {
                        if (photonView.IsMine)
                        {
                            //피격 처리
                            photonView.RPC("damageControlRPC", RpcTarget.AllBuffered, dmg, true);
                            //투사체 파괴
                            bullet.photonView.RPC("bulletOffRPC", RpcTarget.AllBuffered);
                        }
                    }
                    else if (!PhotonNetwork.InRoom)
                    {
                        //피격 처리
                        damageControlRPC(dmg, true);
                        //투사체 파괴
                        bullet.bulletOffRPC();
                    }
                }
            }
            if (other.transform.CompareTag("Outline") || other.transform.CompareTag("RedRose")) //맵 밖으로 나가지면 종료
            {
                if (PhotonNetwork.InRoom)
                {
                    if (photonView.IsMine)
                    {
                        //피격 처리
                        photonView.RPC("changeStateRPC", RpcTarget.AllBuffered, PlayerStateType.Dead, true);
                    }
                }
                else if (!PhotonNetwork.InRoom)
                {
                    changeStateRPC(PlayerStateType.Dead, true);
                }
            }
            if (other.transform.CompareTag("Wind")) //맵 밖으로 나가지면 종료
            {
                //바람 효과음
                battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Wind);
            }          
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("playerSword"))
            {
                if (PhotonNetwork.InRoom)
                {
                    //내 플레이어에 남의 칼이 왔으며, 전투 허가가 났을 때
                    if (photonView.IsMine && !collision.gameObject.GetComponent<PhotonView>().IsMine && isSwordFight)
                    {
                        //피격 처리
                        photonView.RPC("damageControlRPC", RpcTarget.AllBuffered, 20, true);
                    }
                }
            }
        }

        #region 피격 처리
        [PunRPC]
        public void damageControlRPC(int _dmg, bool isEffect)
        {
            //피해량 계산
            curHealth -= _dmg;
            if (curHealth < 0) curHealth = 0;
            else if (curHealth > 100) curHealth = 100;

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
                            photonView.RPC("changeStateRPC", RpcTarget.AllBuffered, PlayerStateType.Dead, true);
                        }
                    }
                    else if (!PhotonNetwork.InRoom)
                    {
                        changeStateRPC(PlayerStateType.Dead, true);
                    }
                }    
            } 
        }

        //죽었고 조금 뒤, 죽음에 대한 처리
        void SoonDie() 
        {
            //최종 점수 보여주기
            battleUIManager.hiddenText.text =
                "최종 점수: " + Mathf.FloorToInt(battleUIManager.curScore)+" 랭크:" + battleUIManager.bigRankText.text;

            //이어하기 버튼은 못 나오도록
            battleUIManager.btnContinue.SetActive(false);
            //정지 패널 나오도록
            battleUIManager.btnStop();
        }
        
        #endregion

        #region 칼로 파괴시, 회복
        [PunRPC]
        public void healControlRPC(int _heal)
        {
            if (isDead || !isCanHeal) 
                return;

            //회복량 계산
            curHealth += _heal;

            //초과하지 않도록
            if (curHealth > maxHealth) 
                curHealth = maxHealth;
        }
        #endregion

        #region 멀티 게임 텍스트 관리
        public enum TypingType { None, Win, Lose, Draw }

        [PunRPC]
        public void loopTypingRPC(TypingType _typingType, string _str = "")//당사자에게 알리는 용
        {
            if (photonView.IsMine)
            {
                //실행하고 있다면 중지
                if (loopTypingCor != null)
                    StopCoroutine(loopTypingCor);

                string str = _str;

                switch (_typingType)
                {
                    case TypingType.Win:
                        str = "Win!";
                        break;
                    case TypingType.Lose:
                        str = "Lose..";
                        break;
                    case TypingType.Draw:
                        str = "Draw?";
                        break;
                }
                loopTypingCor = StartCoroutine(loopTyping(str));
            }
        }

        

        IEnumerator loopTyping(string _str)
        {
            Debug.Log("타이핑");

            battleUIManager.typingControl(_str);

            yield return new WaitForSeconds(3.5f + 0.075f * _str.Length);

            loopTypingCor = StartCoroutine(loopTyping(_str));
        }
        #endregion

        #region 플레이어의 위치 정보 동기화
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)//이 안에서 변수 동기화가 이루어짐(IPunObservable)
        {
            if (stream.IsWriting)//포톤.isMine이랑 같나봄
            {
                stream.SendNext(transform.position);

                stream.SendNext(playerSwords[0].transform.localPosition);
                stream.SendNext(playerSwords[1].transform.localPosition);
                stream.SendNext(playerSwords[2].transform.localPosition);
                stream.SendNext(playerSwords[3].transform.localPosition);
                stream.SendNext(playerSwords[4].transform.position);
                stream.SendNext(playerSwords[5].transform.position);
                stream.SendNext(playerSwords[6].transform.position);
                stream.SendNext(playerSwords[7].transform.position);


                //stream.SendNext(HealthImage.fillAmount);
            }
            else//남의 거면 받나봄
            {
                rpcPos = (Vector3)stream.ReceiveNext();//1번째 줄을 1번째 줄로 받음
                swordsRpcPos[0] = (Vector3)stream.ReceiveNext();
                swordsRpcPos[1] = (Vector3)stream.ReceiveNext();
                swordsRpcPos[2] = (Vector3)stream.ReceiveNext();
                swordsRpcPos[3] = (Vector3)stream.ReceiveNext();
                swordsRpcPos[4] = (Vector3)stream.ReceiveNext();
                swordsRpcPos[5] = (Vector3)stream.ReceiveNext();
                swordsRpcPos[6] = (Vector3)stream.ReceiveNext();
                swordsRpcPos[7] = (Vector3)stream.ReceiveNext();

                //HealthImage.fillAmount = (float)stream.ReceiveNext();//2번째 줄을 1번째 줄로 받음
            }
        }
        #endregion
    }
}
