using Assets.PixelHeroes.Scripts.CharacterScripts;
using Photon.Pun;
using Photon.Pun.Demo.Asteroids;
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
        
        private int maxHealth;
        [Header("----새로 추가----")]
        public int curHealth;

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
        [Header("리더 칼의 속도")]
        public int leaderSwordSpeed = 8;

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
        //중간으로 이동 시 사용하는 그 벡터
        Vector3 tmpRpcPos;
        //그냥 순간이동은 못씀!!
        bool isTeleporting = false;

        [Header("PC로 진행중인지 확인")]
        public bool isPC;


        

        private void Awake()
        {
            leaderSword = swordParent.transform.GetChild(0).gameObject;
            leaderSwordRigid = leaderSword.GetComponent<Rigidbody>();
            leaderSwordComponent = leaderSword.GetComponent<FollowSword>();
        }

        void Start()
        {
            //기존에 있던 것
            Character.SetState(AnimationState.Idle);
            
            //복장 커스텀
            CharacterBuilder.Head = "Lizard#FFFFFF/0:0:0";
            CharacterBuilder.Rebuild();
            //최대 체력 설정
            curHealth = 100;
            maxHealth = curHealth;

            //미니 ui 설정
            miniName.text = photonView.IsMine ? PhotonNetwork.NickName : photonView.Owner.NickName;//나라면 내이름, 다른 사람이면 다른 사람 이름
            miniName.color = photonView.IsMine ? greenColor : redColor;
        }

        


        void Update()
        {
            if (photonView.IsMine)//타인의 것이면 안건듬
            {
                //키 입력
                KeyInput();
            }
            else if (!isTeleporting) 
            {
                if ((transform.position - rpcPos).sqrMagnitude >= 2)//너무 멀면 순간이동 
                {
                    Debug.Log("PlayerQuickMove");
                    TeleportToDestination(rpcPos);
                }
                else
                {
                    Debug.Log("PlayerSlowMove");
                    tmpRpcPos = Vector3.Lerp(transform.position, rpcPos, Time.deltaTime * 10);//아니면 부드럽게
                    TeleportToDestination(tmpRpcPos);
                }
            }
        }

        #region 특수 순간이동      
        private void TeleportToDestination(Vector3 targetVec)
        {
            isTeleporting = true;

            // 임시로 충돌 처리 비활성화
            Controller.enabled = false;

            // 순간 이동
            transform.position = targetVec;

            // 충돌 처리 다시 활성화
            Controller.enabled = true;

            isTeleporting = false;
        }
        #endregion


        #region 키 입력
        void KeyInput() 
        {
            if (Input.GetKeyDown(KeyCode.A)) Character.Animator.SetTrigger("Attack");
            else if (Input.GetKeyDown(KeyCode.J)) Character.Animator.SetTrigger("Jab");
            else if (Input.GetKeyDown(KeyCode.P)) Character.Animator.SetTrigger("Push");
            else if (Input.GetKeyDown(KeyCode.H)) Character.Animator.SetTrigger("Hit");
            else if (Input.GetKeyDown(KeyCode.I)) { Character.SetState(AnimationState.Idle); _activityTime = 0; }
            else if (Input.GetKeyDown(KeyCode.R)) { Character.SetState(AnimationState.Ready); _activityTime = Time.time; }
            else if (Input.GetKeyDown(KeyCode.B)) Character.SetState(AnimationState.Blocking);
            else if (Input.GetKeyUp(KeyCode.B)) Character.SetState(AnimationState.Ready);
            else if (Input.GetKeyDown(KeyCode.D)) Character.SetState(AnimationState.Dead);

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
                    _inputX = -1;
                }
                else if (Input.GetKey(KeyCode.C))
                {
                    _inputX = 1;
                }

                if (Input.GetKey(KeyCode.S))//Down 지워버림
                {
                    _inputY = 1;

                    if (Controller.isGrounded)
                    {
                        JumpDust.Play(true);
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

                    if (Controller.isGrounded)
                    {
                        JumpDust.Play(true);
                    }
                }
            }

            if (PhotonNetwork.InRoom)
            {
                photonView.RPC("xyRPC", RpcTarget.AllBuffered, _inputX, _inputY);
            }
        }
        #endregion

        #region 벡터 동기화
        [PunRPC]
        void xyRPC(int x, int y)
        {
            _inputX = x;
            _inputY = y;

            if (Controller.isGrounded && y == 1)
            {
                JumpDust.Play(true);
            }
        }
        #endregion

        public void FixedUpdate()
        {
            //이동
            Move();

            if (photonView.IsMine)//타인의 것이면 안건듬
            {
                //칼 부모 위치 초기호와 검 사거리 표시
                SwordDirCheck();
                //PC, 조이스틱에 따른 칼의 움직임 조정
                SwordInput();
            }
        }

        

        #region 칼 부모 위치 초기화와 검 사거리 표시
        void SwordDirCheck()
        {
            //칼 부모의 위치 자동 조정
            swordParent.transform.position = swordParentVec;//0,0,0 고정시킴

            swordAreaColor = new Color(1,1,1, leaderSwordComponent.swordDir);
            playerSwordArea.color = swordAreaColor;
        }
        #endregion

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.CompareTag("EnemyBullet"))
            {
                //Debug.Log("PlayerHit!");
            }
        }

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
            if (leaderSword.activeSelf && !isSame) 
            {
                Debug.Log("Spin");

                if (PhotonNetwork.InRoom) //2인 이상이라면
                    photonView.RPC("SwordSpinRPC", RpcTarget.AllBuffered);
                else
                    SwordSpinRPC();//1인이라면
            }
            else if (!leaderSword.activeSelf)//칼이 비활성화 돼있을 시
            {
                if (PhotonNetwork.InRoom) //2인 이상이라면
                    photonView.RPC("SwordActiveRPC", RpcTarget.AllBuffered);
                else
                    SwordActiveRPC();//1인이라면
            }
        }
        #endregion

        #region 칼 회전 동기화
        [PunRPC]
        void SwordSpinRPC()
        {
            //리더 칼의 속도 조정 
            leaderSwordRigid.velocity = swordJoyVec * leaderSwordSpeed;

            //회전 조작
            leaderSword.transform.rotation = Quaternion.identity;
            float zValue = Mathf.Atan2(leaderSwordRigid.velocity.x, leaderSwordRigid.velocity.y) * 180 / Mathf.PI;
            Vector3 rotVec = Vector3.back * zValue + Vector3.back * 45;
            leaderSword.transform.Rotate(rotVec);
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


        #region 이동
        private void Move()
        {
            if (Time.frameCount <= 1)
            {
                Controller.Move(new Vector3(0, Gravity) * Time.fixedDeltaTime);
                return;
            }

            var state = Character.GetState();

            if (state == AnimationState.Dead)//죽었는데 방향키 누르면 움직이도록
            {
                if (_inputX == 0) return;

                Character.SetState(AnimationState.Running);
            }

            if (_inputX != 0)//좌우 방향 전환
            {   
                if(PhotonNetwork.InRoom)
                    photonView.RPC("Turn", RpcTarget.AllBuffered, _inputX);
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
            else
            {
                _motion = new Vector3(RunSpeed * _inputX, _motion.y);
                Character.SetState(AnimationState.Jumping);
            }

            _motion.y += Gravity;

            Controller.Move(_motion * Time.fixedDeltaTime);//!!!!!!!!!!!!!!
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

        private void GetDown()
        {
            Character.Animator.SetTrigger("GetDown");
        }

        private void GetUp()
        {
            Character.Animator.SetTrigger("GetUp");
        }

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
