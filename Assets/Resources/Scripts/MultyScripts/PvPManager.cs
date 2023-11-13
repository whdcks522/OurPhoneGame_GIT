using Assets.PixelHeroes.Scripts.ExampleScripts;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PvPManager : MonoBehaviourPunCallbacks
{
    [Header("� �߻� ����")]
    public Transform[] starPoints;

    //�� ��ģ �߻� �ð�
    float sumTime;
    //�ִ� �߻� �ð�
    public float maxTime;
    //���� �߻� �ð�
    float curTime;

    public int loser = -2;
    

    BattleUIManager battleUIManager;
    [Header("���ӸŴ���")]
    public GameManager gameManager;

    public bool isMasterCilentLocal => PhotonNetwork.IsMasterClient && photonView.IsMine;
                         //->���� �� ��ǻ�Ͱ� ȣ��Ʈ�鼭, �� ���ӿ�����Ʈ�� ȣ��Ʈ ������ ������

    private void Awake()
    {
        battleUIManager = BattleUIManager.Instance;
    }


    private void Update()
    {
        if (isMasterCilentLocal)
        {
            if (PhotonNetwork.PlayerList.Length >= PhotonNetwork.CurrentRoom.MaxPlayers &&
                gameManager.playerGroup.childCount >= PhotonNetwork.CurrentRoom.MaxPlayers)//������ �÷��̾ ���� ���� �� �����ؾ���
            {
                //���� ����

                if (loser == -2)//-2: ������ �� ������ ���
                {
                    photonView.RPC("alreadyStartRPC", RpcTarget.AllBuffered);
                    for (int i = 0; i < gameManager.playerGroup.childCount; i++)
                    {
                        CharacterControls cc = gameManager.playerGroup.GetChild(i).GetComponent<CharacterControls>();
                        cc.photonView.RPC("changeStateRPC", RpcTarget.AllBuffered, CharacterControls.PlayerStateType.LeftControl, true);
                        cc.photonView.RPC("changeStateRPC", RpcTarget.AllBuffered, CharacterControls.PlayerStateType.RightControl, true);
                    }
                }
                else if (loser != -2)
                {
                    for (int i = 0; i < gameManager.playerGroup.childCount; i++)
                    {
                        CharacterControls cc = gameManager.playerGroup.GetChild(i).GetComponent<CharacterControls>();

                        if (loser == -1) //�й��� ����
                        {
                            if (cc.curHealth <= 0)
                            {
                                loser = i;
                            }
                        }
                        else if (loser != -1)
                        {
                            if (i == loser) //�й������� �й� �޽��� ����
                            {
                                cc.photonView.RPC("TypingRPC", RpcTarget.AllBuffered, CharacterControls.TypingType.Lose, "lose");
                            }
                            else
                            {
                                cc.photonView.RPC("TypingRPC", RpcTarget.AllBuffered, CharacterControls.TypingType.Win, "win");
                            }

                        }
                    }
                }

                /*

                curTime += Time.deltaTime;


                if (curTime > maxTime)
                {
                    sumTime += curTime;
                    //�ð� �ʱ�ȭ
                    curTime = 0f;

                    foreach (Transform tmpTrans in starPoints)
                    {
                        GameObject bullet = gameManager.CreateObj("YellowStarBullet", GameManager.PoolTypes.BulletType);

                        //������Ʈ ����
                        Rigidbody bulletRigid = bullet.GetComponent<Rigidbody>();
                        Bullet bulletComponent = bullet.GetComponent<Bullet>();

                        bullet.transform.position = tmpTrans.position;

                        //� Ȱ��ȭ
                        bulletComponent.bulletOnRPC();

                        //� �ӵ� ����
                        bulletRigid.velocity = Vector3.down * bulletComponent.bulletSpeed;

                        //� ȸ�� ����
                        bullet.transform.rotation = Quaternion.identity;
                        float zValue = Mathf.Atan2(bulletRigid.velocity.x, bulletRigid.velocity.y) * 180 / Mathf.PI;
                        Vector3 rotVec = Vector3.back * zValue + Vector3.back * 45.0f;
                        bullet.transform.Rotate(rotVec);
                    }
                
                }
                */
            }

            else //���� ��� ���� ��
            {
                for (int i = 0; i < gameManager.playerGroup.childCount; i++)
                {
                    string str = PhotonNetwork.CurrentRoom.Name + '\n' +
                        PhotonNetwork.CurrentRoom.PlayerCount + '/' + PhotonNetwork.CurrentRoom.MaxPlayers;

                    CharacterControls cc = gameManager.playerGroup.GetChild(i).GetComponent<CharacterControls>();
                    cc.photonView.RPC("TypingRPC", RpcTarget.AllBuffered, CharacterControls.TypingType.None, str);
                }
            }


        }//���� �� ��,

    }
    [PunRPC]
    public void alreadyStartRPC() //���� ����
    {
        loser = -1;
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        gameManager.allLeaveRoomStart();
    }

    //PhotonNetwork.PlayerList[]:�迭�� �ϳ� �ϳ� ����
    //PhotonNetwork.CurrentRoom.Name: ���� �� �̸�
    //PhotonNetwork.CurrentRoom.PlayerCount: �濡 �ִ� ��� ��
    //PhotonNetwork.CurrentRoom.MaxPlayers: �� �ִ� ��� ��
}


