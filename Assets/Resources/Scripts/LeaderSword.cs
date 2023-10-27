using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderSword : MonoBehaviourPunCallbacks, IPunObservable
{
    //�ӵ� ������ ����
    Vector3 rpcPos;

    private void Update()
    {
        if (!photonView.IsMine)
        {
            if ((transform.position - rpcPos).sqrMagnitude >= 2)//�ʹ� �ָ� �����̵� 
            {
                Debug.Log("SwordQuickMove");
                transform.position = rpcPos;
            }
            else
            {
                Debug.Log("SwordSlowMove");
                transform.position = Vector3.Lerp(transform.position, rpcPos, Time.deltaTime * 10);//�ƴϸ� �ε巴��
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
