using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothMoveRPC : MonoBehaviourPunCallbacks, IPunObservable
{
    //�ӵ� ������ ����
    Vector3 rpcPos;
    bool isRoom;
    void Awake() 
    {
        isRoom = PhotonNetwork.InRoom;
    }

    private void Update()
    {
        if (!photonView.IsMine && isRoom)
        {
            if ((transform.position - rpcPos).sqrMagnitude >= 1.5f)//�ʹ� �ָ� �����̵� 
            {
                Debug.Log("SmoothMove_QuickMove");
                transform.position = rpcPos;
            }
            else
            {
                Debug.Log("SmoothMove_SlowMove");
                transform.position = Vector3.Lerp(transform.position, rpcPos, Time.deltaTime * 20);//�ƴϸ� �ε巴��
            }
        }
    }

    #region �÷��̾��� ��ġ ���� ����ȭ
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)//�� �ȿ��� ���� ����ȭ�� �̷����(IPunObservable)
    {
        if (stream.IsWriting)//����.isMine�̶� ������
        {
            stream.SendNext(transform.position);
        }
        else//���� �Ÿ� �޳���
        {
            rpcPos = (Vector3)stream.ReceiveNext();//1��° ���� 1��° �ٷ� ����
        }
    }
    #endregion
}
