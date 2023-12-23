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
        public Image miniUI;

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
        }

        void Update()
        {
            KeyInput();
        }

        [PunRPC]
        void changeClothRPC(string[] arr = null)
        {
            
            CharacterBuilder.Head = arr[0];//���
            CharacterBuilder.Ears = arr[1];//��
            CharacterBuilder.Eyes = arr[2];//��
            CharacterBuilder.Body = arr[3];//��
            CharacterBuilder.Hair = arr[4];//�Ӹ�ī��

            CharacterBuilder.Armor = arr[5];//����
            CharacterBuilder.Helmet = arr[6];//����
            CharacterBuilder.Weapon = arr[7];//����
            CharacterBuilder.Shield = arr[8];//����

            CharacterBuilder.Cape = arr[9];//����
            CharacterBuilder.Back = arr[10];//��
            CharacterBuilder.Mask = arr[11];//����ũ
            CharacterBuilder.Horns = arr[12];//��

            CharacterBuilder.Rebuild();

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
            //������ ���� �ƴ� ���� ��ȯ���� ��
            if ((isJumpAni && Character.GetState() != AnimationState.Jumping) || (!isJumpAni && Character.GetState() == AnimationState.Jumping)) //�������ٰ� �ȴ� ���
            {
                JumpDust.Play();
            }
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
        void deadRPC()
        {
            //��� ó��
            isDead = true;
            //�ִϸ��̼�
            Character.SetState(AnimationState.Dead);
            //ȿ����
            battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Heal);
            //�̴� UI �ݱ�
            miniUI.fillAmount = 0;
            //���� ����
            MoveDust.Stop();
            JumpDust.Stop();
            //�ӵ� ����ȭ(���ϸ� ��ü�� ��Ƽ� ���ư��� ��� ����)
            rigid.velocity = Vector2.zero;


            //�� ����
            if (!isRoom)
                Invoke("SoonDie", 1.5f);
        }

        [PunRPC]//�׾��� �� ���ӵ� ����ȭ�� ����
        void changeVelocity(Vector2 _tmpVec)
        {
            rigid.velocity = _tmpVec;
        }


        //�׾��� ���� ��, ������ ���� ó��
        void SoonDie()
        {
            
        }

       

        private void LateUpdate()
        {
            miniUI.fillAmount = curHealth / maxHealth;
        }
        

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

                            textEffectComponent.photonView.RPC("effectNameRPC", RpcTarget.All, _dmg.ToString());
                            textEffectComponent.photonView.RPC("effectOnRPC", RpcTarget.All, transform.position);
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
            }

            if (curHealth > 0)//�ǰ�
            {
                //ȿ����
                battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Block);
                //��½
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
                    //�� �÷��̾ ���� Į�� ������, ���� �㰡�� ���� ��
                    if (photonView.IsMine)
                    {
                        //�ǰ� ó��
                        photonView.RPC("damageControlRPC", RpcTarget.All, 20, true);
                    }
                }
                else if (!isRoom) 
                {
                    //�ǰ� ó��
                    damageControlRPC(20, true);
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.transform.CompareTag("Outline")) //�� ������ �������� ����
            {
                if (isRoom)
                {
                    if (photonView.IsMine && !isDead)
                    {
                        //�ǰ� ó��
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