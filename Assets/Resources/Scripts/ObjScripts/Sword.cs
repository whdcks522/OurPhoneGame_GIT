using Assets.PixelHeroes.Scripts.ExampleScripts;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviourPunCallbacks
{
    [Serializable]//Ŭ���� ����
    public class SwordInfo
    {
        public Vector2 swordPos;//��ġ, �������� �ʱ�ȭ��
        public Vector2 swordVec;//�ӵ�

        public SwordInfo(Vector2 tmpPos, Vector2 tmpVec)
        {
            swordPos = tmpPos;
            swordVec = tmpVec;
        }
    }

    //Ŭ���� ���� ť
    Queue<SwordInfo> swordQueue = new Queue<SwordInfo>();

    public Sword upperSword;
    public GameObject lowerSword;//�ڽ� ����
    int followDelay = 5;//���󰡴� �����ð�

    //���� Į�� ����
    public SwordInfo swordQueueInfo = new SwordInfo(Vector2.zero, Vector2.zero);

    [Header("�÷��̾� ���ӿ�����Ʈ(��Ȱ��ȭ ��, ������ ����)")]
    public GameObject player;
    //�÷��̾��� �Լ�
    public CharacterControls characterControls;

    [Header("Į�� �÷��̾� ������ ���� �Ÿ�")]
    public float swordDir;
    [Header("Į�� �÷��̾� ������ �ִ� �Ÿ�")]
    public float maxDir;

    [Header("������ �ӵ�")]
    public Vector2 saveSwordVec = Vector3.zero;

    //������ �Ǵ���(���� �ȿ��� ������ ������ ��찡 ����)
    public bool isReadyExplode  = true;

    //���
    public TrailRenderer trailRenderer;
    //��� ������ ��ü Į�� ��
    public int maxSwordIndex;
    //���� Į�� ���°����
    public int curSwordIndex;
    //Į�� ������
    public int swordDamage = 10;

    //��Ʋ �Ŵ���
    public BattleUIManager battleUIManager;
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;

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
        rigid = GetComponent<Rigidbody2D>();

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (PhotonNetwork.InRoom) 
        {
            if (!photonView.IsMine) 
            {
                spriteRenderer.color = Color.red;
                trailRenderer.startColor = Color.red;
                trailRenderer.endColor = Color.red;
            }
        }
    }

    private void OnEnable()
    {
        trailRenderer.Clear();
        swordQueueInfo = new SwordInfo(transform.position, Vector2.zero);
        isReadyExplode = true;
    }

    void FixedUpdate()
    {
        Vector2 swordPos = transform.position;
        Vector2 playerPos = player.transform.position;
        if (PhotonNetwork.InRoom) 
        {
            if(photonView.IsMine) 
            {
                // �� ��ġ ���� �Ÿ��� ����մϴ�.
                swordDir = Vector3.Distance(swordPos, playerPos) / 20;

                if (swordDir > maxDir) //�ڽ��� �������� ���� ���� ����
                {
                    //�÷��̾�� ���� Į�� �Ÿ� ���� �ʱ�ȭ
                    swordDir = 0;
                    photonView.RPC("leaderSwordExitRPC", RpcTarget.AllBuffered, 1);
                }
            }
        }
        else if (!PhotonNetwork.InRoom)
        {
            // �� ��ġ ���� �Ÿ��� ����մϴ�
            swordDir = Vector3.Distance(swordPos, playerPos) / 20;

            if (swordDir > maxDir) //�ڽ��� �������� ���� ���� ����
            {
                //�÷��̾�� ���� Į�� �Ÿ� ���� �ʱ�ȭ
                Debug.Log("aaa");
                swordDir = 0;
                leaderSwordExitRPC(1);
            }
        }
        

        //-----

        //----

        //ť�� ���� ����
        swordQueue.Enqueue(new SwordInfo(transform.position, saveSwordVec));

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

                    lowerSword.GetComponent<Sword>().trailRenderer.Clear();
                }
            }
        }

        //���� �� ���� ��, �÷��̾� ���� �ʱ�ȭ�� �ʿ���
        rigid.angularVelocity = 0f;
        rigid.velocity = saveSwordVec * 8.5f;

        //ȸ�� ����
        transform.rotation = Quaternion.identity;
        float zValue = Mathf.Atan2(rigid.velocity.x, rigid.velocity.y) * 180 / Mathf.PI;
        Vector3 rotVec = Vector3.back * zValue + Vector3.back * 45;
        transform.Rotate(rotVec);

        if (curSwordIndex < characterControls.curSwordCount)
            lowerSword.GetComponent<Sword>().saveSwordVec = swordQueueInfo.swordVec;


    }

    /*
    [PunRPC]
    void saveSwordRPC(SwordInfo tmpSwordInfo) 
    {
        lowerSword.GetComponent<Sword>().saveSwordVec = swordQueueInfo.swordVec;
    }
    */

    /*
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
    */

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
                        createBomb(player.transform.position);//����� �ʿ��� RPC ����
                        //���� ����
                        if (gameObject.activeSelf)
                            createBomb(transform.position);
                    }
                }
                else if (!PhotonNetwork.InRoom)
                {
                    //���ϸ� ����������
                    isReadyExplode = true;
                    //�÷��̾� ����
                    createBomb(player.transform.position);

                    //���� ����
                    if (gameObject.activeSelf) 
                        createBomb(transform.position);
                }
            }

            //���� Į Ȱ��ȭ
            characterControls.backSwords.SetActive(true);
            //��ź ����(���ϸ� Į ����)
            isReadyExplode = false;

            //��� Į ��Ȱ��ȭ
            for (int i = 0; i <= maxSwordIndex - 1; i++)
            {
                GameObject tmpSword = characterControls.swordParent.transform.GetChild(i).gameObject;
                Sword tmpSwordComponent = tmpSword.GetComponent<Sword>();

                //Į ������Ʈ ��Ȱ��ȭ
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
            //Debug.Log("AA");
            //if (upperSword.gameObject.activeSelf)//���� ���� ��
            {
                //Debug.Log("BB");
                transform.position = upperSword.swordQueueInfo.swordPos;
            }
        }
    }
    #endregion

    #region ��ź ����
    void createBomb(Vector3 bombPos)
    {
        if (isReadyExplode)
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
    }
    #endregion
}
