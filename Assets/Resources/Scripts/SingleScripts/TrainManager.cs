using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TrainManager : MonoBehaviour
{
    [Header("발생 지점")]
    public Transform[] blockPoints;





    [Header("씬의 레벨")]
    public int scenelevel;

    GameManager gameManager;
    GameObject player;

    BattleUIManager battleUIManager;

    private void Awake()
    {
        battleUIManager = BattleUIManager.Instance;
        gameManager = battleUIManager.gameManager;

        player = gameManager.player;

        //플레이어 체력 감소 비율 설정
        gameManager.characterControl.healthMinus = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        
    }

    private void Update()
    {


            
        /*

            GameObject block = gameManager.CreateObj("ㄹㄹ", GameManager.PoolTypes.BlockType);
            Block blockComponent = block.GetComponent<Block>();

            //블록 부모 조정
            block.transform.parent = this.transform;
            blockComponent.blockPoints = blockPoints;

            //블록 활성화
            blockComponent.blockOnRPC();
        */
    }
}
