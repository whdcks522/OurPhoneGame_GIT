using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TrainManager : MonoBehaviour
{
    [Header("�߻� ����")]
    public Transform[] blockPoints;





    [Header("���� ����")]
    public int scenelevel;

    GameManager gameManager;
    GameObject player;

    BattleUIManager battleUIManager;

    private void Awake()
    {
        battleUIManager = BattleUIManager.Instance;
        gameManager = battleUIManager.gameManager;

        player = gameManager.player;

        //�÷��̾� ü�� ���� ���� ����
        gameManager.characterControl.healthMinus = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        
    }

    private void Update()
    {


            
        /*

            GameObject block = gameManager.CreateObj("����", GameManager.PoolTypes.BlockType);
            Block blockComponent = block.GetComponent<Block>();

            //��� �θ� ����
            block.transform.parent = this.transform;
            blockComponent.blockPoints = blockPoints;

            //��� Ȱ��ȭ
            blockComponent.blockOnRPC();
        */
    }
}
