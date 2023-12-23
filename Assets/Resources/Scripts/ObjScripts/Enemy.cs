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
        [Header("----���� �ִ� ��----")]
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


        [Header("----���� �߰�----")]
        public float curHealth;
        float maxHealth;

        [Header("ĳ���� ���� �̴� UI")]
        public GameObject miniUI;
        public Image miniHealthGauge;
        public Text miniName;

        //�¿� ������ ���� �ʿ���
        private Vector3 dirVec = new Vector3(1, 1, 1);

        BattleUIManager battleUIManager;
        public GameManager gameManager;
        Rigidbody2D rigid;
        [Header("Rigidbody ������")]
        public int jumpForce;

        [Header("������ ������")]
        public float rayRadius;
        [Header("������ ��Ÿ�")]
        public float raySize;
        [Header("������ ������")]
        public Vector3 rayVec = Vector3.zero;
        //���� �ٴ�����
        bool isGround = false;
        Collider2D hitCol = null;

        //��Ƽ���� �ַ�����
        bool isRoom;
        //�׾�����
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
            //ü�� ����ȭ
            maxHealth = curHealth;
        }

        void Start()
        {
            //������ �ִ� ��
            Character.SetState(AnimationState.Idle);

            //���� Ŀ����
            //CharacterBuilder.Head = "Lizard#FFFFFF/0:0:0";
            CharacterBuilder.Rebuild();

            miniUI.SetActive(false);
            miniName.color = Color.red;
        }

        void Update()
        {
            KeyInput();
        }

        #region Ű �Է�
        void KeyInput()
        {
            if (curHealth <= 0 ) return;

            #region �Ⱦ��� �ִϸ��̼�
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

        #region xy ����ȭ
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

            //�̵�
            Move();
        }


        #region �̵�
        private void Move()
        {
            if (_inputX != 0)//�¿� ���� ��ȯ
            {
                TurnRPC(_inputX);
            }

            bool isJumpAni = false;

            if (Character.GetState() == AnimationState.Jumping)
            {
                isJumpAni = true;//�̹� ���� ������ Ȯ��
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

                    if (!hitCol.isTrigger && hitObj.transform.gameObject != gameObject)//�ڽ��� �����ؾߵ� 
                    {
                        isGround = true;
                        break;
                    }
                }
            }

            if (isGround)//�ٴڿ� ���� ��
            {
                if (_inputY > 0)//����
                {
                    Character.SetState(AnimationState.Jumping);
                    rigid.velocity = new Vector3(rigid.velocity.x, jumpForce);

                    //���� ���� ����
                    if (MoveDust.isPlaying)
                    {
                        MoveDust.Stop();
                    }
                }

                else if (_inputX != 0)//�ȱ�
                {
                    Character.SetState(AnimationState.Running);
                    //���� ���� ����
                    if (!MoveDust.isPlaying && Character.GetState() == AnimationState.Running)
                    {
                        MoveDust.Play();
                    }
                }
                else if (_inputX == 0)//���ֱ�
                {
                    //���� ���� ����
                    Character.SetState(AnimationState.Idle);

                    if (MoveDust.isPlaying)
                    {
                        MoveDust.Stop();
                    }
                }
            }
            else if (!isGround) //���߿� ���� ��
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

            //������ ���� �ƴ� ���� ��ȯ���� ��
            if ((isJumpAni && Character.GetState() != AnimationState.Jumping) || (!isJumpAni && Character.GetState() == AnimationState.Jumping)) //�������ٰ� �ȴ� ���
            {
                JumpDust.Play();
            }

            #region ���� �ٲ�

            /*

             if (Time.frameCount <= 1)
             {
                 Controller.Move(new Vector3(0, Gravity) * Time.fixedDeltaTime);
                 return;
             }

             var state = Character.GetState();

             if (state == AnimationState.Dead)//�׾��µ� ����Ű ������ �����̵���
             {
                 _motion.y += Gravity;
                 Controller.Move(_motion * Time.fixedDeltaTime);

                 //if (_inputX == 0)
                 return;

                 //Character.SetState(AnimationState.Running);
             }

             if (_inputX != 0)//�¿� ���� ��ȯ
             {
                 if (PhotonNetwork.InRoom)
                     photonView.RPC("Turn", RpcTarget.AllBuffered, _inputX);//-------------
                 else
                     Turn(_inputX);
             }

             if (Controller.isGrounded)//���� ���� ��
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
             }//���� ���� ��
             else//�ϴÿ� ���� ��
             {
                 _motion = new Vector3(RunSpeed * _inputX, _motion.y);
                 Character.SetState(AnimationState.Jumping);
             }

             _motion.y += Gravity;


             Controller.Move(_motion * Time.fixedDeltaTime);//!!!!!!!!!!!!!!�̺κ� ���� �ȿ�����
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

        #region Scale�� ������ �¿� ���� ����
        [PunRPC]
        void TurnRPC(int direction)
        {
            var scale = Character.transform.localScale;

            scale.x = Mathf.Sign(direction) * Mathf.Abs(scale.x);

            Character.transform.localScale = scale;
            dirVec.x = direction;
            //UI�� �˵� x�� ��ȯ
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
            if (battleUIManager.btnContinue.activeSelf)//�װ� 1.5�� �Ŀ� ��Ȱ��ȭ ��
            {
                if (!isDead) //���� ���� ����
                {
                    //�ڿ� ü�� ����
                    curHealth -= healthMinus * Time.deltaTime;

                    //���� �ľ�
                    if (PhotonNetwork.InRoom)
                    {
                        if (photonView.IsMine)
                        {
                            //�ǰ� ó��
                            damageControlRPC(0, false);
                        }
                    }
                    else if (!PhotonNetwork.InRoom)
                    {
                        damageControlRPC(0, false);
                    }
                    //UI ����-------------------------------------------------------

                    //�̴� ü�� �� ����
                    miniHealthGauge.fillAmount = curHealth / maxHealth;

                    //ū ü�¹� ����
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

                    //�ð��� ���� ���� ����
                    battleUIManager.curScore += Time.deltaTime * scorePlus;

                }//���� ���� ����

                //��ũ�� ���� �ؽ�Ʈ ����
                battleUIManager.bigScoreText.text = Mathf.FloorToInt(battleUIManager.curScore) + "/";
                if (battleUIManager.curScore >= battleUIManager.Sscore) //S�� �̻��� ���
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
                else if (battleUIManager.curScore >= battleUIManager.Ascore) //A�� �̻��� ���
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
                else if (battleUIManager.curScore >= battleUIManager.Bscore) //B�� �̻��� ���
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
                else if (battleUIManager.curScore >= battleUIManager.Cscore) //C�� �̻��� ���
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
                else if (battleUIManager.curScore >= battleUIManager.Dscore) //D�� �̻��� ���
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
                else if (battleUIManager.curScore >= battleUIManager.Escore) //E�� �̻��� ���
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

        #region �ǰ� ó��
        [PunRPC]
        public void damageControlRPC(int _dmg, bool isEffect)
        {
            //���ط� ���
            curHealth -= _dmg;
            if (curHealth <= 0) curHealth = 0;
            else if (curHealth > maxHealth) curHealth = maxHealth;

            //��� �ʱ�ȭ
            if (_dmg != 0)//���ذ� ���� ��
            {
                if (isEffect) 
                {
                    if (PhotonNetwork.InRoom)
                    {
                        if (photonView.IsMine)
                        {
                            //�ǰ� �ؽ�Ʈ ����Ʈ
                            GameObject textEffect = gameManager.CreateObj("Text 52", GameManager.PoolTypes.EffectType);
                            Effect textEffectComponent = textEffect.GetComponent<Effect>();

                            textEffectComponent.photonView.RPC("effectNameRPC", RpcTarget.AllBuffered, _dmg.ToString());
                            textEffectComponent.photonView.RPC("effectOnRPC", RpcTarget.AllBuffered, transform.position);
                        }
                    }
                    else if (!PhotonNetwork.InRoom)
                    {
                        //�ǰ� �ؽ�Ʈ ����Ʈ
                        GameObject textEffect = gameManager.CreateObj("Text 52", GameManager.PoolTypes.EffectType);
                        Effect textEffectComponent = textEffect.GetComponent<Effect>();

                        textEffectComponent.effectNameRPC(_dmg.ToString());
                        textEffectComponent.effectOnRPC(transform.position);
                    }    
                }
                if (curHealth > 0)//�ǰ�
                {
                    //ȿ����
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
            if (other.transform.CompareTag("Outline")) //�� ������ �������� ����
            {
                if (isRoom)
                {
                    if (photonView.IsMine && !isDead)
                    {
                        //�ǰ� ó��
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
            if (isOut) //�� ������ ���� ���
            {
                transform.position = player.transform.position + Vector3.up * 10;
            }
            else if (!isOut) //��Ż ���
            {
                transform.position = tmpVec;
            }
        }   
    }
}