using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Egg : MonoBehaviourPunCallbacks
{
    [PunRPC]
    public void eggOnRPC()
    {
        //���ӿ�����Ʈ Ȱ��ȭ
        gameObject.SetActive(true);
    }

    [PunRPC]
    public void eggOffRPC()
    {
        //���ӿ�����Ʈ Ȱ��ȭ
        gameObject.SetActive(false);
    }
}
