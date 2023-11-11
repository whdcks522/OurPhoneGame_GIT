using Photon.Pun.Demo.PunBasics;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.PixelHeroes.Scripts.ExampleScripts;


public class Bomb : MonoBehaviourPunCallbacks
{
    [Header("폭탄의 최대 수명")]

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

    #region 폭탄 활성 동기화
    [PunRPC]
    public void bombOnRPC(Vector3 bombPos)
    {
        //게임오브젝트 활성화
        gameObject.SetActive(true);
        gameObject.transform.position = bombPos;
        particleSystem.Play();
    }
    #endregion

    #region 폭탄 비활성 동기화
    [PunRPC]
    public void bombOffRPC()
    {
        //시간 동기화
        curTime = 0f;
        //게임오브젝트 비활성화
        gameObject.SetActive(false);
    }
    #endregion

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.CompareTag("EnemyBullet"))
        {
            Bullet bullet = other.GetComponent<Bullet>();

            if (PhotonNetwork.InRoom)//멀티의 경우
            {
                if (photonView.IsMine)
                {
                    //총알 파괴
                    bullet.photonView.RPC("bulletOffRPC", RpcTarget.AllBuffered);
                    //회복
                    characterControls.photonView.RPC("healOffRPC", RpcTarget.AllBuffered, bullet.bulletHeal);
                }
            }
            else if (!PhotonNetwork.InRoom)//싱글의 경우
            {
                //총알 파괴
                bullet.bulletOffRPC();
                //회복
                characterControls.healControlRPC(bullet.bulletHeal);
            }

            //파워업의 경우
            if (bullet.bulletEffectType == Bullet.BulletEffectType.PowerUp)
            {
                if (PhotonNetwork.InRoom)
                {
                    if (photonView.IsMine)
                    {
                        //무기 수 1 증가
                        characterControls.photonView.RPC("swordCountRPC", RpcTarget.AllBuffered, 1);
                    }
                }
                else if (!PhotonNetwork.InRoom)
                {
                    //무기 수 1 증가
                    characterControls.swordCountRPC(1);
                }
            }
        }
        else if (other.transform.CompareTag("Block"))
        {
            Block block = other.GetComponent<Block>();

            if (PhotonNetwork.InRoom)//멀티의 경우
            {
                if (photonView.IsMine)
                {
                    //총알 파괴
                    block.photonView.RPC("blockOffRPC", RpcTarget.AllBuffered, true);
                    //회복
                    characterControls.photonView.RPC("healOffRPC", RpcTarget.AllBuffered, block.blockHeal);
                }
            }
            else if (!PhotonNetwork.InRoom)//싱글의 경우
            {
                //총알 파괴
                block.blockOffRPC(true);
                //회복
                characterControls.healControlRPC(block.blockHeal);
            }
        }
    }
}
