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
    float curTime = 0f;//------------------

    [Header("�� �߻� ����")]
    public Transform dogPointsParent;
    GameObject[] dogPoints;

    [Header("��ȭ �� �߻� ����")]
    public Transform powerUpPointsParent;
    GameObject[] powerUpPoints;
    int powerUpCurIndex = 0;
    public int powerUpMaxIndex = 0;

    [Header("���� ����")]
    public int scenelevel;

    [Header("�� �ֱ� �迭")]
    public float[] dogSpeedArr;
    //����� �߰� �ӵ�
    float curDogSpeed;

    [Header("�÷��̾� ��ũ��Ʈ")]
    public CharacterControls characterControls;
    public GameObject player;

    BattleUIManager battleUIManager;
    public GameManager gameManager;
    public BulletShotter bulletShotter;


    private void Start()
    {
        battleUIManager = BattleUIManager.Instance;
        curTime = maxTime - 2;

        //�� �߻��� �迭 ����
        dogPoints = new GameObject[dogPointsParent.childCount];
        for (int i = 0; i < dogPointsParent.childCount; i++)
        {
            dogPoints[i] = dogPointsParent.GetChild(i).gameObject;
        }
        //��ȭ �� �߻��� �迭 ����
        powerUpPoints = new GameObject[powerUpPointsParent.childCount];
        for (int i = 0; i < powerUpPointsParent.childCount; i++)
        {
            powerUpPoints[i] = powerUpPointsParent.GetChild(i).gameObject;
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
            //curTime = -100f;

            //���� ȿ����
            battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Summon);

            if (++powerUpCurIndex >= powerUpMaxIndex) 
            {
                powerUpCurIndex = 0;
                int powerUpPos = Random.Range(0, powerUpMaxIndex);
                bulletShotter.sortShot(BulletShotter.BulletShotType.Direction, Bullet.BulletEffectType.PowerUp, 
                    powerUpPoints[powerUpPos], player, 0);
            }

            //�� ����
            if (scenelevel == 0)
            {
                //��ҿ� Ÿ�� ����
                int posR = Random.Range(0, dogPoints.Length);

                //int typeR = 2;
                int typeR = Random.Range(0, 4);
                string type = "";

                switch (typeR) 
                {
                    case 0:
                        type = "Enemy_Goblin";
                        break;
                    case 1:
                    case 2:
                        type = "Enemy_Orc";
                        break;
                    case 3:
                        type = "Enemy_Lizard";
                        break;
                }
                createEnemy(posR, type);
            }
        }
    }

    //���� ���� �� ��ġ
    Vector3 createVec;

    #region �� ����
    void createEnemy(int pos, string type) 
    {
        GameObject enemyGameObject = gameManager.CreateObj(type, GameManager.PoolTypes.EnemyType);
        Enemy enemyComponent = enemyGameObject.GetComponent<Enemy>();
        enemyComponent.gameManager = gameManager;

        //�� ��ġ ����
        float x = Random.Range(-1f, 1f);
        float y = Random.Range(-1f, 1f);
        createVec = new Vector3(x, y, 0);
        enemyGameObject.transform.position = dogPoints[pos].transform.position + createVec;

        //�� Ȱ��ȭ
        enemyComponent.activateRPC();
    }
    #endregion
}
