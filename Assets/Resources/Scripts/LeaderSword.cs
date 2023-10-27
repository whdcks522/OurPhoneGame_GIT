using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderSword : MonoBehaviourPunCallbacks, IPunObservable
{
    //속도 공유를 위함
    Vector3 rpcPos;

    private void Update()
    {
        if (!photonView.IsMine)
        {
            if ((transform.position - rpcPos).sqrMagnitude >= 2)//너무 멀면 순간이동 
            {
                Debug.Log("SwordQuickMove");
                transform.position = rpcPos;
            }
            else
            {
                Debug.Log("SwordSlowMove");
                transform.position = Vector3.Lerp(transform.position, rpcPos, Time.deltaTime * 10);//아니면 부드럽게
            }
        }
    }

    #region 플레이어의 위치 정보 동기화
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)//이 안에서 변수 동기화가 이루어짐(IPunObservable)
    {
        if (stream.IsWriting)//포톤.isMine이랑 같나봄
        {
            stream.SendNext(transform.position);
        }
        else//남의 거면 받나봄
        {
            rpcPos = (Vector3)stream.ReceiveNext();//1번째 줄을 1번째 줄로 받음
        }
    }
    #endregion
}
