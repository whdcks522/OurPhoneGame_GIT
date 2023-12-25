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

        public BattleUIManager battleUIManager;
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
        public bool isGround = false;
        Collider2D hitCol = null;

        //��Ƽ���� �ַ�����
        bool isRoom;
        //�׾�����
        bool isDead;
        [Header("�÷��̾� ��ü")]
        public GameObject player;
        //[Header("�ӽŷ��� ������")]
        //public bool isML;

        public enum EnemyType { Goblin, Orc }
        //public EnemyType enemyType;

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

        #region xy ����ȭ
        [PunRPC]
        void xyRPC(int x, int y)
        {
            _inputX = x;
            _inputY = y;
        }
        #endregion

        #region �̵�
        public void FixedUpdate()
        {
            if (isDead)
                return;

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

        #region Į�� �浹�� ü�� ����
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("playerSword"))
            {
                int damage = collision.gameObject.GetComponent<Sword>().swordDamage;
                if (isRoom)
                {
                    //�� �÷��̾ ���� Į�� ������, ���� �㰡�� ���� ��
                    if (photonView.IsMine)
                    {
                        //�ǰ� ó��
                        photonView.RPC("damageControlRPC", RpcTarget.All, damage, true);
                    }
                }
                else if (!isRoom)
                {
                    //�ǰ� ó��
                    damageControlRPC(damage, true);
                }
            }
        }
        #endregion

        #region ���� ��迡 �浹 ��, �����̵�
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
        #endregion

        #region �����̵�
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
        #endregion

        #region �ǰ� ó��
        [PunRPC]
        public void damageControlRPC(int _dmg, bool isEffect)
        {
            //���ط� ���
            curHealth -= _dmg;
            if (curHealth <= 0) curHealth = 0;
            else if (curHealth > maxHealth) curHealth = maxHealth;
            //UI����
            miniUI.fillAmount = curHealth / maxHealth;

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

                    if (curHealth > 0)//�ǰ�
                    {
                        //ȿ����
                        battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Block);
                        //��½
                        Character.Blink();
                    }
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

        #region ���ó��
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
            Invoke("SoonDieRPC", 1.5f);
        }
        #endregion

        #region ��� ��, �Ҹ�
        void SoonDieRPC()//�׾��� ���� ��, ������ ���� ó��
        {
            //���ӿ�����Ʈ Ȱ��ȭ
            gameObject.SetActive(false);

            //�ı� ȿ����
            battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Heal);

            if (isRoom)
            {
                if (photonView.IsMine)
                {
                    //�ı� ����Ʈ
                    GameObject effect = gameManager.CreateObj("Explosion 2", GameManager.PoolTypes.EffectType);
                    effect.SetActive(true);
                    effect.transform.position = transform.position;
                }
            }
            //�ı� ����Ʈ
            GameObject effect2 = gameManager.CreateObj("Explosion 2", GameManager.PoolTypes.EffectType);
            effect2.SetActive(true);
            effect2.transform.position = transform.position;
        }
        #endregion
    }
}