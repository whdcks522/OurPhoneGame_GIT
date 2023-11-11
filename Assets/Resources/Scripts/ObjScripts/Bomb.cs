using Photon.Pun.Demo.PunBasics;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.PixelHeroes.Scripts.ExampleScripts;


public class Bomb : MonoBehaviourPunCallbacks
{
    [Header("��ź�� �ִ� ����")]

    public float maxTime;
    float curTime = 0f;
    BattleUIManager battleUIManager;
    GameManager gameManager;
    CharacterControls characterControls;
    ParticleSystem particleSystem;
    

    private void Awake()
    {
        battleUIManager = BattleUIManager.Instance.GetComponent<BattleUIManager>();
        gameManager = battleUIManager.gameManager;
        characterControls = gameManager.player.GetComponent<CharacterControls>();

        particleSystem = GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        curTime += Time.deltaTime;
        if (curTime > maxTime)
        {
            if (PhotonNetwork.InRoom && photonView.IsMine)
            {
                photonView.RPC("bombOffRPC", RpcTarget.AllBuffered);
            }
            else
            {
                bombOffRPC();
            }
        }
    }

    #region ��ź Ȱ�� ����ȭ
    [PunRPC]
    public void bombOnRPC(Vector3 bombPos)
    {
        //���ӿ�����Ʈ Ȱ��ȭ
        gameObject.SetActive(true);
        gameObject.transform.position = bombPos;
        particleSystem.Play();
    }
    #endregion

    #region ��ź ��Ȱ�� ����ȭ
    [PunRPC]
    public void bombOffRPC()
    {
        //�ð� ����ȭ
        curTime = 0f;
        //���ӿ�����Ʈ ��Ȱ��ȭ
        gameObject.SetActive(false);
    }
    #endregion

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.CompareTag("EnemyBullet"))
        {
            Bullet bullet = other.GetComponent<Bullet>();

            if (PhotonNetwork.InRoom)//��Ƽ�� ���
            {
                if (photonView.IsMine)
                {
                    //�Ѿ� �ı�
                    bullet.photonView.RPC("bulletOffRPC", RpcTarget.AllBuffered);
                    //ȸ��
                    characterControls.photonView.RPC("healOffRPC", RpcTarget.AllBuffered, bullet.bulletHeal);
                }
            }
            else if (!PhotonNetwork.InRoom)//�̱��� ���
            {
                //�Ѿ� �ı�
                bullet.bulletOffRPC();
                //ȸ��
                characterControls.healControlRPC(bullet.bulletHeal);
            }

            //�Ŀ����� ���
            if (bullet.bulletEffectType == Bullet.BulletEffectType.PowerUp)
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
        }
        else if (other.transform.CompareTag("Block"))
        {
            Block block = other.GetComponent<Block>();

            if (PhotonNetwork.InRoom)//��Ƽ�� ���
            {
                if (photonView.IsMine)
                {
                    //�Ѿ� �ı�
                    block.photonView.RPC("blockOffRPC", RpcTarget.AllBuffered, true);
                    //ȸ��
                    characterControls.photonView.RPC("healOffRPC", RpcTarget.AllBuffered, block.blockHeal);
                }
            }
            else if (!PhotonNetwork.InRoom)//�̱��� ���
            {
                //�Ѿ� �ı�
                block.blockOffRPC(true);
                //ȸ��
                characterControls.healControlRPC(block.blockHeal);
            }
        }
    }
}
