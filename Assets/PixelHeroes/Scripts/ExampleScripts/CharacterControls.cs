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
using UnityEngine.InputSystem;

namespace Assets.PixelHeroes.Scripts.ExampleScripts
{
    public class CharacterControls : MonoBehaviourPunCallbacks
    {
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
        private int _inputX, _inputY;
        private float _activityTime;
        //새로 추가
        PhotonView photonView;
        
        private int maxHealth;
        public int curHealth;
        public GameObject miniUI;
        public Image miniHealthGauge;
        public Text miniName;
        private Vector3 dirVec = new Vector3(1,1,1);
        private Vector3 swordParentVec = new Vector3(0, 0, 0);
        private Color redColor = new Color(197, 44, 28);
        private Color greenColor = new Color(1F,98,21,255);
        private Color blueColor = new Color(37, 97, 192, 255);
        public GameObject swordParent;
        public GameObject leaderSword;
        private FollowSword leaderSwordComponent;
        Rigidbody leaderSwordRigid;
        private Vector2 tmpLeaderSwordVec = new Vector2(0, 0);
        public Vector2 curLeaderSwordVec = new Vector2(0, 0);
        public int leaderSwordSpeed = 8;
        public GameObject backSwords;
        public SpriteRenderer playerSwordArea;
        Color swordAreaColor;

        private void Awake()
        {
            photonView = GetComponent<PhotonView>();
            leaderSwordRigid = leaderSword.GetComponent<Rigidbody>();
            leaderSwordComponent = leaderSword.GetComponent<FollowSword>();
        }

        public void Start()
        {
            Character.SetState(AnimationState.Idle);
            
            //복장 커스텀
            CharacterBuilder.Head = "Lizard#FFFFFF/0:0:0";
            CharacterBuilder.Rebuild();
            //최대 체력 설정
            curHealth = 100;
            maxHealth = curHealth;

            miniName.text = photonView.IsMine ? PhotonNetwork.NickName : photonView.Owner.NickName;//나라면 내이름, 다른 사람이면 다른 사람 이름
            miniName.color = photonView.IsMine ? greenColor : redColor;
        }

        public void Update()
        {
            if (!photonView.IsMine)//타인의 것이면 안건듬
                return;

            KeyInput();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.CompareTag("EnemyBullet"))
            {
                //Debug.Log("PlayerHit!");
            }
        }


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
            else if (Input.GetKeyDown(KeyCode.Alpha1)) Character.Animator.SetTrigger("Slash");
            else if (Input.GetKeyDown(KeyCode.O)) Character.Animator.SetTrigger("Shot");
            else if (Input.GetKeyDown(KeyCode.F)) Character.Animator.SetTrigger("Fire1H");
            else if (Input.GetKeyDown(KeyCode.E)) Character.Animator.SetTrigger("Fire2H");
            else if (Input.GetKeyDown(KeyCode.Alpha2)) Character.SetState(AnimationState.Climbing);
            else if (Input.GetKeyUp(KeyCode.Alpha2)) Character.SetState(AnimationState.Ready);
            else if (Input.GetKeyUp(KeyCode.L)) Character.Blink();

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

            if (Input.GetKey(KeyCode.Z))
            {
                _inputX = -1;
            }
            else if (Input.GetKey(KeyCode.C))
            {
                _inputX = 1;
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                _inputY = 1;

                if (Controller.isGrounded)
                {
                    JumpDust.Play(true);
                }
            }
        }
        #endregion

        public void FixedUpdate()
        {
            if (!photonView.IsMine)//타인의 것이면 안건듬
                return;

            Move();
            //검 조작
            SwordMove();
            //검 사거리 표시
            SwordDirCheck();

        }

        void SwordDirCheck() //검 사거리 표시
        {
            //칼 부모의 위치 자동 조정
            swordParent.transform.position = swordParentVec;

            swordAreaColor = new Color(1,1,1, leaderSwordComponent.swordDir);
            playerSwordArea.color = swordAreaColor;
        }

        public void OnMove(InputValue value) //인풋 시스템용이므로 이름 바꾸면 안됨, 정해진 함수임
        {
            tmpLeaderSwordVec = value.Get<Vector2>();
            if (tmpLeaderSwordVec.magnitude != 0) //만지고 있는 경우
            {
                curLeaderSwordVec = tmpLeaderSwordVec;
            }
        }

        private void SwordMove() //칼이 움직임
        {
            if (leaderSword.activeSelf) //칼이 활성화돼있을 때
            {
                {
                    //리더 칼의 위치 조정 

                    leaderSwordRigid.velocity = curLeaderSwordVec * leaderSwordSpeed;

                    //회전 조작
                    leaderSword.transform.rotation = Quaternion.identity;
                    float zValue = Mathf.Atan2(leaderSwordRigid.velocity.x, leaderSwordRigid.velocity.y) * 180 / Mathf.PI;
                    Vector3 rotVec = Vector3.back * zValue + Vector3.back * 45;
                    leaderSword.transform.Rotate(rotVec);
                }
            }
            else if (!leaderSword.activeSelf)//칼이 비활성화 돼있을 시
            {
                if ((curLeaderSwordVec.x != 0 || curLeaderSwordVec.y != 0)) //처음으로 조작 시, 칼 활성화
                {
                    leaderSword.transform.position = transform.position + Vector3.up * 0.5f;
                    leaderSword.SetActive(true);
                    backSwords.SetActive(false);
                }
            }
            
        }

        #region 이동
        private void Move()
        {
            if (Time.frameCount <= 1)
            {
                Controller.Move(new Vector3(0, Gravity) * Time.fixedDeltaTime);
                return;
            }

            var state = Character.GetState();

            if (state == AnimationState.Dead)
            {
                if (_inputX == 0) return;

                Character.SetState(AnimationState.Running);
            }

            if (_inputX != 0)
            {
                Turn(_inputX);
            }

            if (Controller.isGrounded)
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
            }
            else
            {
                _motion = new Vector3(RunSpeed * _inputX, _motion.y);
                Character.SetState(AnimationState.Jumping);
            }

            _motion.y += Gravity;

            Controller.Move(_motion * Time.fixedDeltaTime);

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


    }
}