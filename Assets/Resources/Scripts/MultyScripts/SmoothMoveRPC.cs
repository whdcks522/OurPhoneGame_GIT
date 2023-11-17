using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothMoveRPC : MonoBehaviourPunCallbacks, IPunObservable
{
    //속도 공유를 위함
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
            if ((transform.position - rpcPos).sqrMagnitude >= 1.5f)//너무 멀면 순간이동 
            {
                Debug.LogError("이름: "+gameObject.name+" 현재 위치:"+transform.position + " 목표 위치: "+ rpcPos );
                transform.position = rpcPos;
            }
            else
            {
                Debug.LogWarning("SlowMove");
                transform.position = Vector3.Lerp(transform.position, rpcPos, Time.deltaTime * 20);//아니면 부드럽게
            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)//이 안에서 변수 동기화가 이루어짐(IPunObservable)
    {
        if (stream.IsWriting)//포톤.isMine이랑 같나봄
        {
            stream.SendNext(transform.position);
            //stream.SendNext(HealthImage.fillAmount);
        }
        else//남의 거면 받나봄
        {
            rpcPos = (Vector3)stream.ReceiveNext();//1번째 줄을 1번째 줄로 받음
            //HealthImage.fillAmount = (float)stream.ReceiveNext();//2번째 줄을 1번째 줄로 받음
        }
    }
}
