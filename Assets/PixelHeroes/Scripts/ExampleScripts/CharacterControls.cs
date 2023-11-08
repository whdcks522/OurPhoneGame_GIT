using Assets.PixelHeroes.Scripts.CharacterScripts;
using CartoonFX;
using Photon.Pun;
using Photon.Pun.Demo.Asteroids;
using Photon.Pun.Demo.PunBasics;
using System;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;
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
        //칼의 부모 위치 고정
        private Vector3 swordParentVec = new Vector3(0, 0, 0);
        
        //첫 번재 칼 게임 오브젝트
        GameObject leaderSword;
        private FollowSword leaderSwordComponent;
        Rigidbody leaderSwordRigid;

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
        GameManager gameManager;
        Rigidbody rigid;
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

        //플레이어의 상태--------------
        public enum PlayerStateType
        {
            Dead, LeftControl, RightControl, CanHeal
        }
        public PlayerStateType playerStateType;
        //이미 죽음
        public bool isDead = false;
        //칼을 던질 수 있는 상태인지
        bool isLeftControl = false;
        bool isRightControl = false;
        //회복 가능한 상태인지
        bool isCanHeal = false;

        [Header("PC로 진행중인지 확인")]
        public bool isPC;

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
                    }
                    else if (!isCheck)
                    {
                        Debug.Log("isDead?!");
                    }
                    break;
                case PlayerStateType.LeftControl:
                    isLeftControl = isCheck;
                    break;
                case PlayerStateType.RightControl:
                    isRightControl = isCheck;
                    break;
                case PlayerStateType.CanHeal:
                    isCanHeal = isCheck;
                    break;
            }
        }

        private void Awake()
        {
            rigid = GetComponent<Rigidbody>();
            
            battleUIManager = BattleUIManager.Instance;
            
            //칼 관리
            for (int i = 0; i < swordParent.transform.childCount; i++)
            {
                GameObject tmpSword = swordParent.transform.GetChild(i).gameObject;
                tmpSword.GetComponent<FollowSword>().player = gameObject;
                tmpSword.GetComponent<FollowSword>().characterControls = this;
                tmpSword.GetComponent<FollowSword>().battleUIManager = battleUIManager;
            }

            leaderSword = swordParent.transform.GetChild(0).gameObject;
            leaderSwordRigid = leaderSword.GetComponent<Rigidbody>();
            leaderSwordComponent = leaderSword.GetComponent<FollowSword>();

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
            //자신의 미니 UI 안보이게
            if (PhotonNetwork.InRoom) 
            {
                if (photonView.IsMine)
                    miniUI.SetActive(false);
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
                else if((transform.position - rpcPos).sqrMagnitude >= 4)//너무 멀면 순간이동 
                {
                    Debug.Log("PlayerQuickMove");
                    transform.position = rpcPos;
                }
                else
                {
                    Debug.Log("PlayerSlowMove");
                    Vector3.Lerp(transform.position, rpcPos, Time.deltaTime * 10);

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

                _inputX = 0;
                if (Input.GetKey(KeyCode.Z))
                {
                    _inputX = -1;
                }
                else if (Input.GetKey(KeyCode.C))
                {
                    _inputX = 1;
                }

                _inputY = 0;
                if (Input.GetKey(KeyCode.S))//Down 지워버림
                {
                    _inputY = 1;

                    //if (Controller.isGrounded)
                    {
                        //JumpDust.Play(true);
                    }
                }
            }
            else if (!isPC)//조이스틱
            {
                moveJoyVec.x = moveJoy.Horizontal;
                moveJoyVec.y = moveJoy.Vertical;
                _inputX = (int)moveJoyVec.x;

                
                if (moveJoyVec.y >= 0.7f)
                {
                    _inputY = 1;

                    //if (Controller.isGrounded)
                    {
                        //JumpDust.Play(true);
                    }
                }
            }

            if (PhotonNetwork.InRoom)
            {
                photonView.RPC("xyRPC", RpcTarget.AllBuffered, _inputX, _inputY);
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
                Turn(_inputX);
            }

            bool isJumpAni = false;

            if (Character.GetState() == AnimationState.Jumping)
            {
                isJumpAni = true;
            }

            if (rigid.velocity.y <= 2) //jumpForce/4
            {

                isGround = false;
                RaycastHit[] rayHits = Physics.SphereCastAll(transform.position + rayVec, rayRadius, Vector3.down, raySize);
                foreach (RaycastHit hitObj in rayHits) 
                {
                    if(hitObj.transform.gameObject.layer.Equals(LayerMask.NameToLayer("Construction")) ||
                        hitObj.transform.gameObject.layer.Equals(LayerMask.NameToLayer("Block"))||
                        hitObj.transform.gameObject.layer.Equals(LayerMask.NameToLayer("PlayerSword")))
                    {
                        isGround = true;
                        break;
                    }
                }


                if (isGround)//바닥에 있을 때 isConst || isBlock
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

                    else if (_inputX != 0)//좌우 방향 전환
                    {
                        Character.SetState(AnimationState.Running);
                        //런닝 먼지 시작
                        if (!MoveDust.isPlaying && Character.GetState() == AnimationState.Running)
                        {
                            MoveDust.Play();
                        }
                    }
                    else if (_inputX == 0)//좌우 방향 전환
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

            if (PhotonNetwork.InRoom) 
            {
                if (photonView.IsMine)
                    _inputX = _inputY = 0;
            }
            else if (!PhotonNetwork.InRoom)
                _inputX = _inputY = 0;


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
        private void Turn(int direction)
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
            //칼 부모의 위치 자동 조정
            //swordParent.transform.position = swordParentVec;//0,0,0 고정시킴  //이거 빼니까 고쳐지네 뭐징..

            swordAreaColor = new Color(1,1,1, leaderSwordComponent.swordDir);
            playerSwordArea.color = swordAreaColor;
        }
        #endregion

        #region PC, 조이스틱에 따른 칼의 벡터 입력
        private void SwordInput()
        {
            bool isMove = false;
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

                if (tmpX != 0 || tmpY != 0) isMove = true;
            }

            else if (!isPC) 
            {
                swordJoyVec.x = swordJoy.Horizontal;
                swordJoyVec.y = swordJoy.Vertical;

                if (swordJoyVec.x != 0 || swordJoyVec.y != 0) isMove = true;
            }
            //길이 감소
            swordJoyVec = swordJoyVec.normalized;

            //조작이 이전과 같은지
            bool isSame = (leaderSwordRigid.velocity.normalized.x == swordJoyVec.x) && (leaderSwordRigid.velocity.normalized.y == swordJoyVec.y);

            //검 이동
            if (isMove)
                SwordMove(isSame);
        }
        #endregion

        #region 이전과 조작이 다르면 칼을 움직임
        void SwordMove(bool isSame) 
        {
            //칼이 활성화돼있을 때, 방향 조작시
            if (leaderSword.activeSelf) // && !isSame
            {
                if (PhotonNetwork.InRoom) //2인 이상이라면
                    photonView.RPC("SwordSpinRPC", RpcTarget.AllBuffered, swordJoyVec);
                else
                    SwordSpinRPC(swordJoyVec);//1인이라면
            }
            else if (!leaderSword.activeSelf)//칼이 비활성화 돼있을 시
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
            FollowSword followSword = leaderSword.GetComponent<FollowSword>();
            followSword.leaderSwordVec = tmpVec;
        }
        #endregion

        #region 칼 활성 동기화
        [PunRPC]
        void SwordActiveRPC() 
        {
            leaderSword.transform.position = transform.position + Vector3.up * 0.5f + (Vector3)swordJoyVec ;
            leaderSword.SetActive(true);
            backSwords.SetActive(false);
        }
        #endregion


        private void LateUpdate()
        {
            if (isDead)
                return;

            //자연 체력 감소
            curHealth -= healthMinus * Time.deltaTime;

            //생존 파악
            if (PhotonNetwork.InRoom)
            {
                if (photonView.IsMine)
                {
                    //피격 처리
                    photonView.RPC("damageControlRPC", RpcTarget.AllBuffered, 0);
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
                    float secondValue = battleUIManager.bigHealthBar.value;
                    battleUIManager.bigHealthBar.value = Mathf.Lerp(firstValue, curHealth / maxHealth, 1f);
                }
            }
            else if (!PhotonNetwork.InRoom)
            {
                float firstValue = battleUIManager.bigHealthBar.value;
                float secondValue = battleUIManager.bigHealthBar.value;
                battleUIManager.bigHealthBar.value = Mathf.Lerp(firstValue, curHealth / maxHealth, 1f);
            }

            //시간에 따라 점수 증가
            battleUIManager.curScore +=  Time.deltaTime * scorePlus;

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
                battleUIManager.bigRankText.text = "<color=red> S </color>";
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
                battleUIManager.bigRankText.text = "<color=#FFAA00> A </color>";
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
                battleUIManager.bigRankText.text = "<color=#FFFF00> B </color>";
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
                battleUIManager.bigRankText.text = "<color=#00AA00> C </color>";
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
                battleUIManager.bigRankText.text = "<color=#00AAFF> D </color>";
                battleUIManager.bigScoreText.text += battleUIManager.Cscore;
            }
            else if (battleUIManager.curScore >= battleUIManager.Escore) //E급 이상의 경우
            {
                battleUIManager.bigRankText.text = "<color=#AA00FF> E </color>";
                battleUIManager.bigScoreText.text += battleUIManager.Dscore;
            }   
        }

        #region 무기 변화 동기화
        [PunRPC]
        public bool swordCountRPC(int value)
        {
            if (value > 0)
            {
                curSwordCount += value;
                if (curSwordCount > 9) curSwordCount = 10;
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

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.CompareTag("EnemyBullet"))
            {
                Bullet bullet = other.GetComponent<Bullet>();
                int dmg = bullet.bulletDamage;


                if (PhotonNetwork.InRoom)
                {
                    if (photonView.IsMine)
                    {
                        //피격 처리
                        photonView.RPC("damageControlRPC", RpcTarget.AllBuffered, dmg);
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
            else if (other.transform.CompareTag("Outline") || other.transform.CompareTag("RedRose")) //맵 밖으로 나가지면 종료
            {
                curHealth = 0;
            }
        }

        private void OnCollisionStay(Collision collision)
        {
            if (collision.gameObject.CompareTag("playerSword")) 
            {
                if (PhotonNetwork.InRoom) 
                {
                    if (!photonView.IsMine) 
                    {
                        //피격 처리
                        photonView.RPC("damageControlRPC", RpcTarget.AllBuffered, 1, false);
                    }
                }
                else if (!PhotonNetwork.InRoom)
                {
                    damageControlRPC(1, false);
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
                //이동 억제
                _motion = Vector3.zero;

                if (isEffect) 
                {
                    //피격 텍스트 이펙트
                    GameObject textEffect = gameManager.CreateObj("Text 52", GameManager.PoolTypes.EffectType);
                    //이름으로 설정
                    textEffect.name = _dmg.ToString();

                    textEffect.SetActive(true);
                    textEffect.transform.position = transform.position;
                }
                


                if (curHealth > 0)//피격
                {
                    //효과음
                    battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Damage);
                    //애니메이션 처리
                    Character.SetState(AnimationState.Jumping);
                }
            }

            if (curHealth <= 0)
            {
                if (Character.GetState() != AnimationState.Dead) 
                {
                    
                    if (PhotonNetwork.InRoom)
                    {
                        if (photonView.IsMine) 
                        {
                            photonView.RPC("changeStateRPC", RpcTarget.AllBuffered, PlayerStateType.Dead, true);
                            //칼 비활성화
                            leaderSword.GetComponent<FollowSword>().photonView.RPC("leaderSwordExitRPC", RpcTarget.AllBuffered, 2);
                            
                        }
                    }
                    else if (!PhotonNetwork.InRoom)
                    {
                        changeStateRPC(PlayerStateType.Dead, true);
                        //칼 비활성화
                        leaderSword.GetComponent<FollowSword>().leaderSwordExitRPC(2);
                        Invoke("SoonDie", 1.5f);
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


        #region 플레이어의 위치 정보 동기화
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)//이 안에서 변수 동기화가 이루어짐(IPunObservable)
        {
            if (stream.IsWriting)//포톤.isMine이랑 같나봄
            {
                stream.SendNext(transform.position);
                //stream.SendNext(HealthImage.fillAmount);
            }
            else//남의 거면 받나봄
            {
                rpcPos = (Vector3)stream.ReceiveNext();//1번째 줄을 1번째 줄로 받음
                //HealthImage.fillAmount = (float)stream.ReceiveNext();//2번째 줄을 1번째 줄로 받음
            }
        }
        #endregion
    }
}
