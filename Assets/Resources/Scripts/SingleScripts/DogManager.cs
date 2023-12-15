using Assets.PixelHeroes.Scripts.ExampleScripts;
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
    float curTime = 0;

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

        //�÷��̾� ü�� ���� ���� 0���� ����
        characterControls.healthMinus = 0;

        characterControls.scorePlus = 1;

        //�ٶ� �߻��� �迭 ����
        dogPoints = new GameObject[dogPointsParent.childCount];
        for (int i = 0; i < dogPointsParent.childCount; i++)
        {
            dogPoints[i] = dogPointsParent.GetChild(i).gameObject;
        }
        //���� ���� ��,  ����� �ӵ� ���
        curDogSpeed = dogSpeedArr[5];

        //����� ���
        battleUIManager.audioManager.PlayBgm(AudioManager.BgmSingle.Fly);
    }


    private void Update()
    {
        return;
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

            //�Ѿ� ����
            //BulletShotter bulletShotter = windPoints[ran].GetComponent<BulletShotter>();
            //bulletShotter.sortShot(BulletShotter.BulletShotType.Direction, Bullet.BulletEffectType.PowerUp,
            //        windPoints[ran], player, 1);
        }
    }
}
