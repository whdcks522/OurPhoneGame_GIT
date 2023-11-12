using Assets.PixelHeroes.Scripts.ExampleScripts;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        if (isMasterCilentLocal &&
            PhotonNetwork.PlayerList.Length >= PhotonNetwork.CurrentRoom.MaxPlayers && 
            gameManager.playerGroup.childCount >= PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            if (!gameManager.alreadyStart)
            {
                Debug.Log("Activate");
                gameManager.alreadyStart = true;
                for (int i = 0; i < gameManager.playerGroup.childCount; i++)
                {
                    CharacterControls cc = gameManager.playerGroup.GetChild(i).GetComponent<CharacterControls>();
                    Debug.Log("gg:+ " + i);
                    cc.photonView.RPC("changeStateRPC", RpcTarget.AllBuffered, CharacterControls.PlayerStateType.LeftControl, true);
                    cc.photonView.RPC("changeStateRPC", RpcTarget.AllBuffered, CharacterControls.PlayerStateType.IsCanJump, true);
                    cc.photonView.RPC("changeStateRPC", RpcTarget.AllBuffered, CharacterControls.PlayerStateType.RightControl, true);
                }
            }


            return;

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
        }
        else //���� ��� ���� ��
        {
            if (!gameManager.alreadyStart) //�� ó�� �����ϰ� ����ϰ� ���� ��
            {
                string str = PhotonNetwork.CurrentRoom.Name + '\n' +
                    PhotonNetwork.CurrentRoom.PlayerCount + '/' + PhotonNetwork.CurrentRoom.MaxPlayers;
                battleUIManager.typingControl(str);
            }
        }

        
        //PhotonNetwork.PlayerList[]:�迭�� �ϳ� �ϳ� ����
        //PhotonNetwork.CurrentRoom.Name: ���� �� �̸�
        //PhotonNetwork.CurrentRoom.PlayerCount: �濡 �ִ� ��� ��
        //PhotonNetwork.CurrentRoom.MaxPlayers: �� �ִ� ��� ��
    }
}

