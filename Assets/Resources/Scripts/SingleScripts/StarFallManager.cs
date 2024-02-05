using Photon.Pun.Demo.Procedural;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using static BulletShotter;

public class StarFallManager : MonoBehaviour
{
    

    [Header("� �߻� ����")]
    public GameObject[] starFallPoints;
    //� �߻� ������ ����
    int starFallPointsSize;

    //�ִ� �߻� �ð�
    public float maxTime;
    //���� �߻� �ð�
    float curTime;
    //�ֱٿ� �߻��� ��ġ
    int recentPos;

    [Header("�Ŀ��� ���� ��ũ �� �ֱ�")]
    public int[] PowerUpIndexArr;
    //�ִ� �Ŀ��� ���� 
    int maxPowerUpIndex;
    //���� �Ŀ��� ���� 
    int curPowerUpIndex = 0;

    [Header("�߰� ���� �ӵ� �迭")]
    public float[] createBulletArr;
    //��ũ�� ���� �߰� ����
    float rankSpeed = 1;

    [Header("���� ����")]
    public int scenelevel;

    BattleUIManager battleUIManager;
    [Header("���� �Ŵ���")]
    public GameManager gameManager;
    GameObject player;

    private void Awake()
    {
        battleUIManager = BattleUIManager.Instance;
        
        player = gameManager.player;
        starFallPointsSize = starFallPoints.Length;

        //�÷��̾� ���� ���� ���� ����
        gameManager.characterControl.scorePlus = 1;

        //����� ���
        battleUIManager.audioManager.PlayBgm(AudioManager.BgmSingle.StarFall);
    }

    private void Update()
    {
        curTime += Time.deltaTime * rankSpeed;

        if (curTime > maxTime)
        {
            switch (battleUIManager.rankType)
            {
                case BattleUIManager.RankType.S:
                    rankSpeed = createBulletArr[0];
                    gameManager.characterControl.healthMinus = createBulletArr[0];

                    maxPowerUpIndex = PowerUpIndexArr[0];
                    break;
                case BattleUIManager.RankType.A:
                    rankSpeed = createBulletArr[1];
                    gameManager.characterControl.healthMinus = createBulletArr[1];

                    maxPowerUpIndex = PowerUpIndexArr[1];
                    break;
                case BattleUIManager.RankType.B:
                    rankSpeed = createBulletArr[2];
                    gameManager.characterControl.healthMinus = createBulletArr[2];

                    maxPowerUpIndex = PowerUpIndexArr[2];
                    break;
                case BattleUIManager.RankType.C:
                    rankSpeed = createBulletArr[3];
                    gameManager.characterControl.healthMinus = createBulletArr[3];

                    maxPowerUpIndex = PowerUpIndexArr[3];
                    break;
                case BattleUIManager.RankType.D:
                    rankSpeed = createBulletArr[4];
                    gameManager.characterControl.healthMinus = createBulletArr[4];

                    maxPowerUpIndex = PowerUpIndexArr[4];
                    break;
                case BattleUIManager.RankType.E:
                    rankSpeed = createBulletArr[5];
                    gameManager.characterControl.healthMinus = createBulletArr[5];

                    maxPowerUpIndex = PowerUpIndexArr[5];
                    break;
            }

            //�ð� �ʱ�ȭ
            curTime = 0f;

            //���� ȿ����
            battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Summon);

            //���� ��ġ ���ϱ�
            int ranPos = Random.Range(0, starFallPointsSize);
            //���� �� �������� �ȵǵ��� ����
            if (ranPos == recentPos) 
                ranPos = (ranPos + 1) / starFallPointsSize;
            recentPos = ranPos;

            BulletShotter bulletShotter = starFallPoints[ranPos].GetComponent<BulletShotter>();

            if (scenelevel == 0) //��ȭ � 1�� �߻�
            {
                if (curPowerUpIndex >= maxPowerUpIndex)//��ȭ �
                {
                    bulletShotter.sortShot(BulletShotter.BulletShotType.Direction, Bullet.BulletEffectType.PowerUp,
                        starFallPoints[ranPos], player, 0);
                    curPowerUpIndex = 0;
                }
                else //�⺻ � 2�� �߻�
                {
                    bulletShotter.sortShot(BulletShotter.BulletShotType.Direction, Bullet.BulletEffectType.Normal,
                        starFallPoints[ranPos], player, 1);
                    bulletShotter.sortShot(BulletShotter.BulletShotType.Direction, Bullet.BulletEffectType.Normal,
                        starFallPoints[ranPos], player, 1);

                    curPowerUpIndex++;
                }
            }
            else if (scenelevel == 1)
            {

                if (curPowerUpIndex >= maxPowerUpIndex)//��ȭ ��� �����ϴ� ���
                {
                    curPowerUpIndex = 0;

                    bulletShotter.sortShot(BulletShotter.BulletShotType.Direction, Bullet.BulletEffectType.PowerUp,
                        starFallPoints[ranPos], player, 0);  
                }
                else //�׳� ����� ���
                {
                    //����ü ���� ����
                    Bullet.BulletEffectType bulletEffectType = Bullet.BulletEffectType.Normal;//�⺻ ����ü
                    if (curPowerUpIndex == 0)
                        bulletEffectType = Bullet.BulletEffectType.UnBreakable;//0�϶���, ������ �ʴ� ����ü

                    //���� ���� ����
                    int ranPattern = Random.Range(0, 3);

                    switch (ranPattern)
                    {
                        case 0://�ϰ� ��Ʋ� �߻�
                            for (int i = 0; i < starFallPoints.Length; i++) 
                            {
                                BulletShotter allBulletShotter = starFallPoints[i].GetComponent<BulletShotter>();

                                allBulletShotter.sortShot(BulletShotter.BulletShotType.Direction, bulletEffectType,
                                starFallPoints[i], player, 1);
                            }
                            break;
                        case 1://�۰� ��ź
                            bulletShotter.sortShot(BulletShotter.BulletShotType.Big, bulletEffectType,
                            starFallPoints[ranPos], player, 0);
                            break;
                        case 2://��Ѹ���
                            bulletShotter.sortShot(BulletShotter.BulletShotType.Scatter, bulletEffectType,
                            starFallPoints[ranPos], player, 1);
                            break;
                    }
                    //���⿡ ���ϸ� ���� �� ���� �ȳ���
                    curPowerUpIndex++;
                }
            }//Scene == 1
            else if (scenelevel == 2) 
            {
                if (true)//��ȭ � curPowerUpIndex >= maxPowerUpIndex)
                {
                    bulletShotter.sortShot(BulletShotter.BulletShotType.Direction, Bullet.BulletEffectType.PowerUp,
                        starFallPoints[4], starFallPoints[12], 0);

                    bulletShotter.sortShot(BulletShotter.BulletShotType.Direction, Bullet.BulletEffectType.PowerUp,
                        starFallPoints[4], starFallPoints[12], 0);

                    curPowerUpIndex = 0;
                }
                else //�⺻ � 2�� �߻�
                {
                    bulletShotter.sortShot(BulletShotter.BulletShotType.Direction, Bullet.BulletEffectType.Normal,
                        starFallPoints[4], starFallPoints[12], 0);

                    curPowerUpIndex++;
                }
            }//Scene == 2
        }
    }
}
