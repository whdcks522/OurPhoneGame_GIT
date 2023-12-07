using Assets.PixelHeroes.Scripts.ExampleScripts;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FlyManager : MonoBehaviour
{
    [Header("�ִ� �߻� �ð�")]
    public float maxTime;
    //���� �߻� �ð�
    float curTime = 0;

    //�ֱٿ� �߻��� ��ġ
    int recentPos;

    [Header("�ٶ� �߻� ����")]
    public Transform windPointsParent;
    GameObject[] windPoints;

    [Header("���� ����")]
    public int scenelevel;

    [Header("�ٶ� �ִ� �ֱ�")]
    public float[] windSpeedArr;
    //����� �ֱ�
    float curWindSpeed;

    //�ִ� �ٶ� 
    public int maxWindIndex;
    //���� �ٶ� 
    int curWindIndex = 0;


    [Header("�÷��̾� ��ũ��Ʈ")]
    public CharacterControls characterControls;

    BattleUIManager battleUIManager;
    [Header("���� �Ŵ���")]
    public GameManager gameManager;
    public GameObject player;



    [Header("���� ī�޶��� ���� ���̴� ��(�Ʒ��� ���� 1����)")]
    float errorDir = -35;
    [Header("��踦 ��Ȱ��ȭ�ϱ� ����")]
    public GameObject outLines;
    [Header("�����̴� ī�޶� ����")]
    public GameObject cmRange;
    [Header("�����̴� ��̵��� ����")]
    public GameObject redRoses;
    [Header("�����̴� ����� ����")]
    public GameObject backGround;


    private void Start()
    {
        battleUIManager = BattleUIManager.Instance;

        //�÷��̾� ü�� ���� ���� 0���� ����
        characterControls.healthMinus = 0;
        characterControls.curHealth = 100;

        if (scenelevel == 0)
            characterControls.scorePlus = 1;
        else if (scenelevel == 1)
        {
            characterControls.scorePlus = 0;
            outLines.SetActive(false);
        }

        //�ٶ� �߻��� �迭 ����
        windPoints = new GameObject[windPointsParent.childCount];
        for (int i = 0; i < windPointsParent.childCount; i++) 
        {
            windPoints[i] = windPointsParent.GetChild(i).gameObject;
        }
        //���� ���� ��,  ����� �ӵ� ���
        curWindSpeed = windSpeedArr[5];
        
        //����� ���
        battleUIManager.audioManager.PlayBgm(AudioManager.BgmSingle.Fly);
    }

    Vector2 maxHighVec = Vector2.zero;
    float maxHigh = 10f;
    private void Update()
    {
        curTime += Time.deltaTime * curWindSpeed;
        
        if (scenelevel == 1) 
        {
            //�ٶ� ��ġ �ִ밪 ����
            battleUIManager.curScore  = Mathf.Max(battleUIManager.curScore, player.transform.position.y);
            maxHigh = Mathf.Max(maxHigh, battleUIManager.curScore);

            //�ٶ� ���� ��ġ ����
            windPointsParent.transform.position = player.transform.position;

            //��� ��ġ ����
            maxHighVec = new Vector2(0, maxHigh);
            cmRange.transform.position = maxHighVec;

            //��� ��ġ ����
            maxHighVec = new Vector2(0, maxHigh + errorDir);
            backGround.transform.position = maxHighVec;

            //�Ʒ� �ƿ����� ����
            redRoses.transform.position = maxHighVec;
        }

        if (curTime > maxTime)
        {
            switch (battleUIManager.rankType)
            {
                case BattleUIManager.RankType.S:
                    curWindSpeed = windSpeedArr[0];
                    break;
                case BattleUIManager.RankType.A:
                    curWindSpeed = windSpeedArr[1];
                    break;
                case BattleUIManager.RankType.B:
                    curWindSpeed = windSpeedArr[2];
                    break;
                case BattleUIManager.RankType.C:
                    curWindSpeed = windSpeedArr[3];
                    break;
                case BattleUIManager.RankType.D:
                    curWindSpeed = windSpeedArr[4];
                    break;
                case BattleUIManager.RankType.E:
                    curWindSpeed = windSpeedArr[5];
                    break;
            }

            //�ð� �ʱ�ȭ
            curTime = 0f;
            curWindIndex++;

            //���� ȿ����
            battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Summon);

            //���� ��ġ ���ϱ�
            int ran = Random.Range(0, windPoints.Length);
            //���� �� �������� �ȵǵ��� ����
            if (ran == recentPos)
                ran = (ran + 1) / windPoints.Length;
            recentPos = ran;

            //�Ѿ� ����
            BulletShotter bulletShotter = windPoints[ran].GetComponent<BulletShotter>();
            bulletShotter.sortShot(BulletShotter.BulletShotType.Direction, Bullet.BulletEffectType.PowerUp,
                    windPoints[ran], player, 1);

            if (curWindIndex >= maxWindIndex)
            {
                curWindIndex = 0;
                //�ٶ� ����
                createWind(); 
            }
        }
    }


    #region �ٶ� ����
    void createWind() 
    {
        //���� ��ġ ���ϱ�
        int ranPos = Random.Range(0, windPoints.Length);
        //���� �� �������� �ȵǵ��� ����
        if (ranPos == recentPos)
            ranPos = (ranPos + 1) / windPoints.Length;
        recentPos = ranPos;


        GameObject wind = gameManager.CreateObj("NormalWind", GameManager.PoolTypes.WindType);

        //������Ʈ ����
        Wind windComponent = wind.GetComponent<Wind>();
        wind.transform.position = windPoints[ranPos].transform.position;

        //� Ȱ��ȭ
        windComponent.windOnRPC();

        //ȸ�� ����
        int isTrue = Random.Range(0, 2);//������ ���� ���ؼ�

        if (isTrue == 0)
        {
            Vector3 direction = (player.transform.position - wind.transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            wind.transform.eulerAngles = new Vector3(0, 0, angle + Random.Range(-15, 16));
        }
        else if (isTrue == 1)
        {
            Vector3 direction = (wind.transform.position - player.transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            wind.transform.eulerAngles = new Vector3(0, 0, angle + Random.Range(-15, 16));
        }
    }
    #endregion
}
