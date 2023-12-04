using Assets.PixelHeroes.Scripts.ExampleScripts;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FlyManager : MonoBehaviour
{
    //�ִ� �߻� �ð�
    public float maxTime = 3f;
    //���� �߻� �ð�
    float curTime = 0;

    //�ֱٿ� �߻��� ��ġ
    int recentPos;

    [Header("�ٶ� �߻� ����")]
    public Transform[] windPoints;

    [Header("���� ����")]
    public int scenelevel;

    [Header("�ٶ� �ִ� �ֱ�")]
    public float[] windSpeedArr;
    float curWindSpeed;

    //�ִ� �Ŀ��� ���� 
    public int maxPowerUpIndex;
    //���� �Ŀ��� ���� 
    int curPowerUpIndex = 0;


    [Header("�÷��̾� ��ũ��Ʈ")]
    public CharacterControls characterControls;

    BattleUIManager battleUIManager;
    [Header("���� �Ŵ���")]
    public GameManager gameManager;
    public GameObject player;

    [Header("ī�޶� ��� ������ ����")]
    public int[] xArr = { 28, -28, -28, 28 };
    public int[] yArr = { 25, 25, -10, -10};
    Vector2[] vecArr = {Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero};//new Vector2(-28, -10), new Vector2(28, -10), new Vector2(-28, 25), new Vector2(28, 25)
    //ī�޶� ��踦 ����
    public PolygonCollider2D polyCol;

    private void Start()
    {
        battleUIManager = BattleUIManager.Instance;

        //�÷��̾� ü�� ���� ���� 0���� ����
        characterControls.healthMinus = 0;
        characterControls.curHealth = 100;
        characterControls.scorePlus = 1;

        curWindSpeed = windSpeedArr[5];

        //����� ���
        battleUIManager.audioManager.PlayBgm(AudioManager.BgmSingle.Fly);
    }

    Vector2 maxHighVec = Vector2.zero;
    public float maxHigh = 0f;
    private void Update()
    {
        //curTime += Time.deltaTime * curWindSpeed;
        maxHigh = Mathf.Max(maxHigh, player.transform.position.y);
        if (scenelevel == 1) 
        {
            for(int i = 0; i < vecArr.Length; i++) 
            {
                maxHighVec = new Vector2(xArr[i], yArr[i] + maxHigh);
                vecArr[i] = maxHighVec;
            }
            polyCol.points = vecArr;
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
            curPowerUpIndex++;

            //���� ȿ����
            battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Summon);

            //���� ��ġ ���ϱ�
            int ranPos = Random.Range(0, windPoints.Length);
            //���� �� �������� �ȵǵ��� ����
            if (ranPos == recentPos)
                ranPos = (ranPos + 1) / windPoints.Length;
            recentPos = ranPos;


            GameObject wind = gameManager.CreateObj("NormalWind", GameManager.PoolTypes.WindType);

            //������Ʈ ����
            Wind windComponent = wind.GetComponent<Wind>();

            wind.transform.parent = this.transform;
            wind.transform.position = windPoints[ranPos].position;

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

            if (scenelevel == 0) 
            {
                if (curPowerUpIndex >= maxPowerUpIndex)
                {
                    curPowerUpIndex = 0;

                    GameObject bullet = gameManager.CreateObj("PowerUpBullet", GameManager.PoolTypes.BulletType);

                    //������Ʈ ����
                    Rigidbody2D bulletRigid = bullet.GetComponent<Rigidbody2D>();
                    Bullet bulletComponent = bullet.GetComponent<Bullet>();


                    //������� �������� � �߻�
                    bullet.transform.parent = this.transform;
                    bullet.transform.position = windPoints[0].position;

                    //� Ȱ��ȭ
                    bulletComponent.bulletOnRPC();

                    //�ӵ� ����
                    Vector2 bulletVec = Vector2.down;

                    //���� �ӵ� ����
                    bulletRigid.velocity = bulletVec * bulletComponent.bulletSpeed;

                    //ȸ�� ����
                    bullet.transform.rotation = Quaternion.identity;
                    float zValue = Mathf.Atan2(bulletRigid.velocity.x, bulletRigid.velocity.y) * 180 / Mathf.PI;
                    Vector3 rotVec = Vector3.back * zValue + Vector3.back * 45.0f;
                    bullet.transform.Rotate(rotVec);
                }
            }

            
        }
    }
}
