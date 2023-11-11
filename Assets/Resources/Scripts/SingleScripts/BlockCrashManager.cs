using Photon.Pun.Demo.Asteroids;
using Photon.Pun.Demo.PunBasics;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockCrashManager : MonoBehaviour
{
    [Header("�߻� ����")]
    public Transform[] blockPoints;
    //�߻� �ε���
    int startIndex = -1;
    [Header("�ִ� �߻� �ð�")]
    public float maxTime;
    //���� �߻� �ð�
    float curTime;

    [Header("�Ŀ��� ��� �ִ� �ֱ�")]
    public int[] PowerUpIndexArr;
    //�ִ� �Ŀ��� ���� 
    int maxPowerUpIndex;
    //���� �Ŀ��� ���� 
    int curPowerUpIndex = 0;

    [Header("ȸ�� ��� �ִ� �ֱ�")]
    public int maxCureIndex;
    //���� ȸ�� ��� 
    int curCureIndex = 0;

    [Header("�߰� ���� �ӵ� �迭")]
    public float[] createBlockArr;
    //��ũ�� ���� �߰� ����
    float rankSpeed = 1;


    
    [Header("���� ����")]
    public int scenelevel;
    [Header("���� �Ŵ���")]
    public GameManager gameManager;
    
    BattleUIManager battleUIManager;

    private void Awake()
    {
        battleUIManager = BattleUIManager.Instance;
        curTime = maxTime;

        //�÷��̾� ���� ���� ���� ����
        gameManager.characterControl.scorePlus = 1;
    }

    private void Update()
    {
        curTime += Time.deltaTime * rankSpeed;

        if (curTime > maxTime)
        {
            switch (battleUIManager.rankType)
            {
                case BattleUIManager.RankType.S:
                    rankSpeed = createBlockArr[0];
                    gameManager.characterControl.healthMinus = createBlockArr[0];

                    maxPowerUpIndex = PowerUpIndexArr[0];
                    break;
                case BattleUIManager.RankType.A:
                    rankSpeed = createBlockArr[1];
                    gameManager.characterControl.healthMinus = createBlockArr[1];

                    maxPowerUpIndex = PowerUpIndexArr[1];
                    break;
                case BattleUIManager.RankType.B:
                    rankSpeed = createBlockArr[2];
                    gameManager.characterControl.healthMinus = createBlockArr[2];

                    maxPowerUpIndex = PowerUpIndexArr[2];
                    break;
                case BattleUIManager.RankType.C:
                    rankSpeed = createBlockArr[3];
                    gameManager.characterControl.healthMinus = createBlockArr[3];

                    maxPowerUpIndex = PowerUpIndexArr[3];
                    break;
                case BattleUIManager.RankType.D:
                    rankSpeed = createBlockArr[4];
                    gameManager.characterControl.healthMinus = createBlockArr[4];

                    maxPowerUpIndex = PowerUpIndexArr[4];
                    break;
                case BattleUIManager.RankType.E:
                    rankSpeed = createBlockArr[5];
                    gameManager.characterControl.healthMinus = createBlockArr[5];

                    maxPowerUpIndex = PowerUpIndexArr[5];
                    break;
            }

            startIndex++;
            if (startIndex >= blockPoints.Length) 
                startIndex = 0;
            //�ð� �ʱ�ȭ
            curTime = 0f;
            //��ȭ ����ü�� ��ȭ �� ����
            curPowerUpIndex++;
            curCureIndex++;
            //���� ȿ����
            battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Summon);
            //������ ����� �̸�
            string blockName = "NormalBlock";
            if (curCureIndex >= maxCureIndex)
            {
                //���� �ʱ�ȭ
                curCureIndex = 0;
                //������ ��� ����
                blockName = "CureBlock";
            }
            else if (curPowerUpIndex >= maxPowerUpIndex)
            {
                //���� �ʱ�ȭ
                curPowerUpIndex = 0;
                //������ ��� ����
                blockName = "PowerUpBlock";
            }


            GameObject block = gameManager.CreateObj(blockName, GameManager.PoolTypes.BlockType);
            Block blockComponent = block.GetComponent<Block>();


            //��� �θ� ����
            block.transform.parent = this.transform;
            blockComponent.blockPoints = blockPoints;

            //��� Ȱ��ȭ
            blockComponent.blockOnRPC();
        }
    }
}
