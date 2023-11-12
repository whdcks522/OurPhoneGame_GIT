using Assets.PixelHeroes.Scripts.ExampleScripts;
using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;


public class FollowSword : MonoBehaviourPunCallbacks
{
    [Serializable]//Ŭ���� ����
    public class FollowSwordInfo
    {
        public Vector2 swordPos { get; set; }//��ġ
        public Vector2 swordVec { get; set; }//�ӵ�

        public FollowSwordInfo(Vector2 tmpPos, Vector2 tmpVec)
        {
            swordPos = tmpPos;
            swordVec = tmpVec;
        }
    }

    //Ŭ���� ���� ť
    Queue<FollowSwordInfo> swordQueue = new Queue<FollowSwordInfo>();

    public FollowSword upperSword;
    public GameObject lowerSword;//�ڽ� ����
    int followDelay = 5;//���󰡴� �����ð�

    //���� Į�� ����
    private FollowSwordInfo swordQueueInfo = new FollowSwordInfo(Vector2.zero, Vector2.zero);

    [Header("�÷��̾� ���ӿ�����Ʈ(��Ȱ��ȭ ��, ������ ����)")]
    public GameObject player;
    //�÷��̾��� �Լ�
    public CharacterControls characterControls;

    [Header("Į�� �÷��̾� ������ �Ÿ� ���")]
    public float swordDir;


    [Header("������ �ӵ�")]
    public Vector2 saveSwordVec = Vector3.zero;


    //���
    public TrailRenderer trailRenderer;
    //��� ������ ��ü Į�� ��
    public int maxSwordIndex;
    //���� Į�� ���°����
    public int curSwordIndex;
    //Į�� ������
    int swordDamage = 5;

    //��Ʋ �Ŵ���
    public BattleUIManager battleUIManager;
    Rigidbody2D rigid;


    #region �� ���� Ŭ���� ����
    /*
    [Serializable]//�ʿ��ϴ���
    public class EnemySpawnInfo
    {
        public EnemyType enemyType;
        public int generateIndex;
    }

    [Serializable]
    public class EnemySpawnInfoArray
    {
        public EnemySpawnInfo[] enemySpawnInfo;
    }

    public EnemySpawnInfoArray[] enemySpawnInfoArray;//é�� ��ü���� ��ȯ�� ���� ���
    private List<EnemySpawnInfo> enemySpawnList;//�̹� ������������ ��ȯ�� ���� ���
    */
    #endregion

    private void Awake()
    {
        battleUIManager = BattleUIManager.Instance;
        player = transform.root.gameObject;
        characterControls = player.GetComponent<CharacterControls>();
        rigid = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        trailRenderer.Clear();

        swordQueueInfo = new FollowSwordInfo(transform.position, Vector2.zero);//transform�� ������?
    }

    void FixedUpdate()
    {
        if (curSwordIndex == 1)
        {
            // �� ��ġ ���� �Ÿ��� ����մϴ�.
            Vector2 swordPos = transform.position;
            Vector2 playerPos = player.transform.position;

            swordDir = Vector3.Distance(swordPos, playerPos) / 400;  
        }


        //ť�� ���� ����
        swordQueue.Enqueue(new FollowSwordInfo(transform.position, saveSwordVec));

        //���� ����, Ŭ���� ���� ���
        if (swordQueue.Count > followDelay)
        {
            swordQueueInfo = swordQueue.Dequeue();

            //���� �ִٸ� ���� 
            if (curSwordIndex < characterControls.curSwordCount) //���� Į�� ��ȣ x�� ĳ������ Į ������ 
            {
                if (!lowerSword.activeSelf) 
                {
                    lowerSword.SetActive(true);
                    lowerSword.transform.position = swordQueueInfo.swordPos;

                    lowerSword.GetComponent<FollowSword>().trailRenderer.Clear();
                } 
            }
        }

        //���� �� ���� ��, �÷��̾� ���� �ʱ�ȭ�� �ʿ���
        rigid.angularVelocity = 0f;
        rigid.velocity = saveSwordVec * 10;
      
        //ȸ�� ����
        transform.rotation = Quaternion.identity;
        float zValue = Mathf.Atan2(rigid.velocity.x, rigid.velocity.y) * 180 / Mathf.PI;
        Vector3 rotVec = Vector3.back * zValue + Vector3.back * 45;
        transform.Rotate(rotVec);

        if (curSwordIndex < characterControls.curSwordCount)//�� �� Į�� ���� ����
        {
            lowerSword.GetComponent<FollowSword>().saveSwordVec = swordQueueInfo.swordVec;
        }
    }

    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (PhotonNetwork.InRoom)//��Ƽ ���̶��
        {
            if (other.transform.CompareTag("PlayerSwordArea") && photonView.IsMine)//���� ���� �浹 �ߴٸ�
            {
                PhotonView tmpPhotonView = other.gameObject.GetComponent<PhotonView>();
                if (tmpPhotonView.IsMine) //�ڽ��� �������� ���� ���� ����
                {
                    //�÷��̾�� ���� Į�� �Ÿ� ���� �ʱ�ȭ
                    swordDir = 0;
                    photonView.RPC("leaderSwordExitRPC", RpcTarget.AllBuffered, 1);
                }
            }
        }
        else //1���̶��
        {
            if (other.transform.CompareTag("PlayerSwordArea"))//���� ���� �浹 �ߴٸ�  && curSwordIndex == 1
            {
                //�÷��̾�� ���� Į�� �Ÿ� ���� �ʱ�ȭ
                swordDir = 0;
                leaderSwordExitRPC(1);
            }
        }
    }

    #region Į�� ���� ������ ��Ż ��
    [PunRPC]
    public void leaderSwordExitRPC(int level)
    {
        //0: �׳� ����
        //1: ���⸸ ����(Į ���� ������ ���� ���)
        //2: ���ΰ� ���� ����

        //1: ���⸸ ����(Į ���� ������ ���� ���)
        if (curSwordIndex == 1) //���� Į�� ���� ���
        {
            if (level == 1 && characterControls.curSwordCount > 1)
            {
                if (PhotonNetwork.InRoom)
                {
                    if (photonView.IsMine)
                        createBomb(transform.position);
                }
                else if (!PhotonNetwork.InRoom)
                    createBomb(transform.position);
            }
            //2: ���ΰ� ���� ����
            else if (level == 2)
            {
                if (PhotonNetwork.InRoom)
                {
                    if (photonView.IsMine)
                    {
                        //�÷��̾� ����
                        createBomb(player.transform.position);
                        //���� ����
                        if (gameObject.activeSelf)
                            createBomb(transform.position);
                    }
                }
                else if (!PhotonNetwork.InRoom)
                {
                    //�÷��̾� ����
                    createBomb(player.transform.position);
                    //���� ����
                    if (gameObject.activeSelf)
                        createBomb(transform.position);
                }
            }

            //���� Į Ȱ��ȭ
            characterControls.backSwords.SetActive(true);
            //�ٽ� Į ��Ȱ��ȭ
            for (int i = 0; i <= maxSwordIndex - 1; i++)
            {
                GameObject tmpSword = characterControls.swordParent.transform.GetChild(i).gameObject;
                FollowSword tmpSwordComponent = tmpSword.GetComponent<FollowSword>();


                tmpSword.transform.position = player.transform.position;
                //Į Ȱ��ȭ
                tmpSword.SetActive(false);

                if (tmpSwordComponent != null)
                {
                    tmpSwordComponent.swordQueue.Clear();//ť �ʱ�ȭ
                }
            }
        }
        //---------- ���� �� ����
        else if (curSwordIndex != 1)
        {
            transform.position = upperSword.swordQueueInfo.swordPos;
        }
    }
    #endregion

    #region ��ź ����
    void createBomb(Vector3 bombPos) 
    {
       
        //��ź ����
        GameObject bomb = null;

        if (PhotonNetwork.InRoom)
        {
            if (photonView.IsMine)
            {
                //���� �� 1 ����
                characterControls.photonView.RPC("swordCountRPC", RpcTarget.AllBuffered, -1);

                bomb = battleUIManager.gameManager.CreateObj("Broken Phantasm", GameManager.PoolTypes.BombType);

                //���� ���� ��� ���� �� ���� ����
                Bomb bombComponent = bomb.GetComponent<Bomb>();
                //��ź �ش� ��ġ�� Ȱ��ȭ
                bombComponent.bombOnRPC(bombPos);
                bombComponent.photonView.RPC("bombOnRPC", RpcTarget.AllBuffered, bombPos);
            }
        }
        else if (!PhotonNetwork.InRoom)
        {
            //���� �� 1 ����
            characterControls.swordCountRPC(-1);

            bomb = battleUIManager.gameManager.CreateObj("Broken Phantasm", GameManager.PoolTypes.BombType);

            //���� ���� ��� ���� �� ���� ����
            Bomb bombComponent = bomb.GetComponent<Bomb>();
            //��ź �ش� ��ġ�� Ȱ��ȭ
            bombComponent.bombOnRPC(bombPos);
        }  
    }
    #endregion

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.CompareTag("EnemyBullet"))
        {
            Bullet bullet = other.GetComponent<Bullet>();

            if (bullet.bulletEffectType == Bullet.BulletEffectType.UnBreakable)
                return;

            //�Ѿ� �ı��� ȸ��
            if (PhotonNetwork.InRoom)
            {
                if (photonView.IsMine)
                {
                    //�Ѿ� �ı�
                    bullet.photonView.RPC("bulletOffRPC", RpcTarget.AllBuffered);
                    //ȸ��
                    characterControls.photonView.RPC("healOffRPC", RpcTarget.AllBuffered, bullet.bulletHeal);
                }
            }
            else if (!PhotonNetwork.InRoom)
            {
                //�Ѿ� �ı�
                bullet.bulletOffRPC();
                //ȸ��
                characterControls.healControlRPC(bullet.bulletHeal);
            }

            if (bullet.bulletEffectType == Bullet.BulletEffectType.PowerUp)//�Ŀ����� ���
            {
                if (PhotonNetwork.InRoom)
                {
                    if (photonView.IsMine)
                    {
                        //���� �� 1 ����
                        characterControls.photonView.RPC("swordCountRPC", RpcTarget.AllBuffered, 1);
                    }
                }
                else if (!PhotonNetwork.InRoom)
                {
                    //���� �� 1 ����
                    characterControls.swordCountRPC(1);
                }

            }
            else if (bullet.bulletEffectType == Bullet.BulletEffectType.Normal)
            {
                //�Ϲ� ȿ����
                battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Heal);
            }
        }
       
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.transform.CompareTag("Block"))
        {
            Block block = other.gameObject.GetComponent<Block>();
            if (PhotonNetwork.InRoom)
            {
                if (photonView.IsMine)
                {
                    block.photonView.RPC("healthControl", RpcTarget.AllBuffered, Time.deltaTime * swordDamage);
                }
            }
            else if (!PhotonNetwork.InRoom)
            {
                block.healthControl(Time.deltaTime * swordDamage);
            }
        }
    }
}
