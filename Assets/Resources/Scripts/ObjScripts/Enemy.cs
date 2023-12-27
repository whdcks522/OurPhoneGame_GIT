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
        [Header("���� �ִ� ü��")]
        public float maxHealth;
        [Header("���� ���� ü��")]
        public float curHealth;
        [Header("�÷��̾��� ȸ����")]
        public int playerHeal;
        [Header("���� �ִ� ���� ��� �ð�")]
        public float maxTime;
        [Header("���� �ӽ� ���� ��� �ð�")]
        public float tmpTime;
        [Header("���� ���� ���� ��� �ð�")]
        public float curTime;

        [Header("ĳ���� ���� �̴� UI")]
        public GameObject miniUI;
        public Image miniHealth;

        //�¿� ������ ���� �ʿ���
        private Vector3 dirVec = new Vector3(1, 1, 1);

        public BattleUIManager battleUIManager;
        public GameManager gameManager;
        public Rigidbody2D rigid;
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
        public CharacterControls characterControls;
        //[Header("�ӽŷ��� ������")]
        public bool isML;

        public enum EnemyType { Goblin, Orc }//������?
        public EnemyType enemyType;

        private void Awake()
        {
            battleUIManager = BattleUIManager.Instance;

            PhotonNetwork.SendRate = 60;
            PhotonNetwork.SerializationRate = 30;
            

            isRoom = PhotonNetwork.InRoom;
            //ȯ��
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
            //���� �ʱ�ȭ
            rigid.velocity = Vector2.zero;
            //ü�� ȸ��
            curHealth = maxHealth;
            miniHealth.fillAmount = 1;
            //������ �ִ� ��
            Character.SetState(AnimationState.Idle);

            //������Ʈ Ȱ��ȭ
            isDead = false;
            
            gameObject.SetActive(true);
        }

        #region xy ����ȭ
        [PunRPC]
        public void xyRPC(int x, int y)
        {
            _inputX = x;
            _inputY = y;
        }
        #endregion

        #region �̵��� �ð� ����
        public void FixedUpdate()
        {
            if (isDead)
            {
                rigid.velocity = new Vector3(0, rigid.velocity.y);
                return;
            }
            //�ð� ���ϱ�
            curTime += Time.deltaTime;

            //if (Input.GetKeyDown(KeyCode.J)) Character.Animator.SetTrigger("Jab");

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
                   hitObj.transform.gameObject.layer.Equals(LayerMask.NameToLayer("Player")) ||
                   hitObj.transform.gameObject.layer.Equals(LayerMask.NameToLayer("Enemy")))
                {
                    hitCol = hitObj.collider;

                    if (!hitCol.isTrigger && hitObj.transform.gameObject != gameObject)//�ڽ��� �����ؾߵ� 
                    {
                        isGround = true;
                        break;
                    }
                    else //�ؿ� ���� ���� ���
                    {
                        if (MoveDust.isPlaying)
                            MoveDust.Stop();
                    }
                }
            }

            if (isGround)//�ٴڿ� ���� ��
            {
                if (_inputY > 0)//����
                {
                    Character.SetState(AnimationState.Jumping);
                    rigid.velocity = new Vector2(rigid.velocity.x, jumpForce);

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
        public void TurnRPC(int direction)
        {
            var scale = Character.transform.localScale;

            scale.x = Mathf.Sign(direction) * Mathf.Abs(scale.x);

            Character.transform.localScale = scale;
            dirVec.x = direction;
            //UI�� �˵� x�� ��ȯ
            miniUI.transform.localScale = dirVec;
        }
        #endregion

        #region �浹 ����
        private void OnCollisionStay2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("playerSword"))//Į�� �浹���� ��
            {
                int damage = collision.gameObject.GetComponent<Sword>().swordDamage;
                if (isRoom)
                {
                    //�� �÷��̾ ���� Į�� ������, ���� �㰡�� ���� ��
                    if (photonView.IsMine)
                    {
                        //�ǰ� ó��
                        photonView.RPC("damageControlRPC", RpcTarget.All, 25 * damage * Time.deltaTime);
                    }
                }
                else if (!isRoom)
                {
                    //�ǰ� ó��
                    damageControlRPC(50 * damage * Time.deltaTime);
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.transform.CompareTag("Outline")) //�� ������ �������� ���
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
            else if (other.gameObject.CompareTag("Bomb"))//��ź�� �浹���� ��
            {
                int damage = other.gameObject.GetComponent<Bomb>().bombDmg;
                if (isRoom)
                {
                    //�� �÷��̾ ���� Į�� ������, ���� �㰡�� ���� ��
                    if (photonView.IsMine)
                    {
                        //�ǰ� ó��
                        photonView.RPC("damageControlRPC", RpcTarget.All, damage * 3);
                    }
                }
                else if (!isRoom)
                {
                    //�ǰ� ó��
                    damageControlRPC(damage * 3);
                }
            }
        }

        #endregion

        //ȿ������ ���� �з��� ����
        int sfxLevel = 4;

        #region �ǰ� ó��
        [PunRPC]
        public void damageControlRPC(float _dmg)
        {
            if (isDead)
                return;

            //���ط� ���
            curHealth -= _dmg;
            if (curHealth <= 0) curHealth = 0;
            else if (curHealth > maxHealth) curHealth = maxHealth;
            //UI����
            miniHealth.fillAmount = curHealth / maxHealth;

            //��� �ʱ�ȭ
            if (curHealth > 0)//�ǰ�
            {
                //��½
                Character.Blink();

                if (curHealth <= maxHealth * 0.75f && sfxLevel > 3) 
                {
                    //ȿ����
                    battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Block);
                    sfxLevel = 3;
                }
                else if (curHealth <= maxHealth * 0.5f && sfxLevel > 2)
                {
                    //ȿ����
                    battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Block);
                    sfxLevel = 2;
                }
                else if (curHealth <= maxHealth * 0.25f && sfxLevel > 1)
                {
                    //ȿ����
                    battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Block);
                    sfxLevel = 1;
                }
                else if (curHealth <= 0 && sfxLevel > 0)
                {
                    //ȿ����
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

        #region ���ó��
        [PunRPC]
        void deadRPC()
        {
            if (!isML) 
            {
                //��� ó��
                isDead = true;
                rigid.velocity = new Vector3(rigid.velocity.x, -jumpForce / 2);
                //�ִϸ��̼�
                Character.SetState(AnimationState.Dead);
                //ȿ����
                battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Heal);
                //�̴� UI �ݱ�
                miniHealth.fillAmount = 0;
                //���� ����
                MoveDust.Stop();
                JumpDust.Stop();
                //�� ����
                Invoke("SoonDieRPC", 1.5f);
            } 
        }
        #endregion

        #region ��� ��, �Ҹ�
        void SoonDieRPC()//�׾��� ���� ��, ������ ���� ó��
        {
            //���ӿ�����Ʈ Ȱ��ȭ
            gameObject.SetActive(false);

            //�ı� ȿ����
            battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Heal);

            //ü�� ȸ��
            characterControls.healControlRPC(playerHeal);

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
            else if (!isRoom)
            {
                //�ı� ����Ʈ
                GameObject effect2 = gameManager.CreateObj("Explosion 2", GameManager.PoolTypes.EffectType);
                effect2.SetActive(true);
                effect2.transform.position = transform.position;
            } 
        }
        #endregion

        public void reloadRPC(float tmpF, string ani)//������
        {
            switch (ani) 
            {
                //��� �ִϸ��̼�
                case "Shot":
                    Character.Animator.SetTrigger("Shot");
                    break;
                //���� �ִϸ��̼�
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