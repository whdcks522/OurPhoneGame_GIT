using Assets.PixelHeroes.Scripts.ExampleScripts;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class DogManager : MonoBehaviour
{
    [Header("�ִ� ���� �ð�")]
    public float maxTime;
    //���� ���� �ð�
    float curTime = 2;//------------------�����Ƽ�

    [Header("�� �߻� ����")]
    public Transform dogPointsParent;
    GameObject[] dogPoints;

    [Header("���� ����")]
    public int scenelevel;

    [Header("�� �ֱ� �迭")]
    public float[] dogSpeedArr;
    //����� �߰� �ӵ�
    float curDogSpeed;

    [Header("�÷��̾� ��ũ��Ʈ")]
    public CharacterControls characterControls;

    BattleUIManager battleUIManager;
    [Header("���� �Ŵ���")]
    public GameManager gameManager;

    private void Start()
    {
        battleUIManager = BattleUIManager.Instance;

        //�ٶ� �߻��� �迭 ����
        dogPoints = new GameObject[dogPointsParent.childCount];
        for (int i = 0; i < dogPointsParent.childCount; i++)
        {
            dogPoints[i] = dogPointsParent.GetChild(i).gameObject;
        }
        //���� ���� ��,  ����� �ӵ� ���
        curDogSpeed = dogSpeedArr[5];
        //�÷��̾� ü�� ���� ���� 0���� ����
        characterControls.healthMinus = curDogSpeed;
        characterControls.scorePlus = 1;

        //����� ���
        battleUIManager.audioManager.PlayBgm(AudioManager.BgmSingle.Fly);
    }


    private void Update()
    {
        curTime += Time.deltaTime * curDogSpeed;

        if (curTime > maxTime)
        {
            switch (battleUIManager.rankType)
            {
                case BattleUIManager.RankType.S:
                    curDogSpeed = dogSpeedArr[0];
                    break;
                case BattleUIManager.RankType.A:
                    curDogSpeed = dogSpeedArr[1];
                    break;
                case BattleUIManager.RankType.B:
                    curDogSpeed = dogSpeedArr[2];
                    break;
                case BattleUIManager.RankType.C:
                    curDogSpeed = dogSpeedArr[3];
                    break;
                case BattleUIManager.RankType.D:
                    curDogSpeed = dogSpeedArr[4];
                    break;
                case BattleUIManager.RankType.E:
                    curDogSpeed = dogSpeedArr[5];
                    break;
            }

            //�ð� �ʱ�ȭ
            curTime = 0f;

            //���� ȿ����
            battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Summon);

            //�� ����
            if (scenelevel == 0)
            {
                int r = Random.Range(0, dogPoints.Length);

                GameObject enemyGameObject = gameManager.CreateObj("Enemy_Goblin", GameManager.PoolTypes.EnemyType);
                Enemy enemyComponent = enemyGameObject.GetComponent<Enemy>();
                enemyComponent.gameManager = gameManager;
                //�� ��ġ ����
                enemyGameObject.transform.position = dogPoints[r].transform.position;
                //�� Ȱ��ȭ
                enemyComponent.activateRPC();
            }
        }
    }
}
