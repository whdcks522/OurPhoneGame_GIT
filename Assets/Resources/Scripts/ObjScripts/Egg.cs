using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Egg : MonoBehaviourPunCallbacks
{
    [PunRPC]
    public void eggOnRPC()
    {
        //게임오브젝트 활성화
        gameObject.SetActive(true);
    }

    [PunRPC]
    public void eggOffRPC()
    {
        //게임오브젝트 활성화
        gameObject.SetActive(false);
    }
}
