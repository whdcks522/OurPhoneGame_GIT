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

    [Header("�Ŀ��� ���� ��ũ �� �ֱ�")]
    public int[] PowerUpIndexArr;
    //�ִ� �Ŀ��� ���� 
    int maxPowerUpIndex;
    //���� �Ŀ��� ���� 
    int curPowerUpIndex = 0;

    [Header("�߰� ���� �ӵ� �迭")]
    public float[] createBlockArr;
    //��ũ�� ���� �߰� ����
    float rankSpeed = 1;


    
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
        curTime = maxTime;
    }

    private void Update()
    {
        switch (battleUIManager.rankType) 
        {
            case BattleUIManager.RankType.S:
                rankSpeed = createBlockArr[0];
                maxPowerUpIndex = PowerUpIndexArr[0];
                break;
            case BattleUIManager.RankType.A:
                rankSpeed = createBlockArr[1];
                maxPowerUpIndex = PowerUpIndexArr[1];
                break;
            case BattleUIManager.RankType.B:
                rankSpeed = createBlockArr[2];
                maxPowerUpIndex = PowerUpIndexArr[2];
                break;
            case BattleUIManager.RankType.C:
                rankSpeed = createBlockArr[3];
                maxPowerUpIndex = PowerUpIndexArr[3];
                break;
            case BattleUIManager.RankType.D:
                rankSpeed = createBlockArr[4];
                maxPowerUpIndex = PowerUpIndexArr[4];
                break;
            case BattleUIManager.RankType.E:
                rankSpeed = createBlockArr[5];
                maxPowerUpIndex = PowerUpIndexArr[5];
                break;
        }

        curTime += Time.deltaTime * rankSpeed;

        if (curTime > maxTime)
        {
            //���� ��ġ ���ϱ�
            //int ranPos = Random.Range(0, blockPoints.Length);
            startIndex++;
            if (startIndex >= blockPoints.Length) 
                startIndex = 0;
            //�ð� �ʱ�ȭ
            curTime = 0f;
            //��ȭ ����ü�� ��ȭ �� ����
            curPowerUpIndex++;
            //���� ȿ����
            battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Summon);
            //������ ����� �̸�
            string blockName = "NormalBlock";

            if (curPowerUpIndex >= maxPowerUpIndex)
            {
                //���� �ʱ�ȭ
                curPowerUpIndex = 0;
                //������ ��� ����
                blockName = "HardBlock";
                

                //��ȭ ����ü ����
                GameObject bullet = gameManager.CreateObj("GreenStarBullet", GameManager.PoolTypes.BulletType);

                //������Ʈ ����
                Rigidbody bulletRigid = bullet.GetComponent<Rigidbody>();
                Bullet bulletComponent = bullet.GetComponent<Bullet>();

                //��ġ ����
                bullet.transform.parent = this.transform;
                bullet.transform.position = blockPoints[startIndex].position;

                //� Ȱ��ȭ
                bulletComponent.bulletOnRPC();

                //�ӵ� ����
                Vector2 bulletVec = (player.transform.position - bullet.transform.position).normalized;

                //���� �ӵ� ����
                bulletRigid.velocity = bulletVec * bulletComponent.bulletSpeed;

                //ȸ�� ����
                bullet.transform.rotation = Quaternion.identity;
                float zValue = Mathf.Atan2(bulletRigid.velocity.x, bulletRigid.velocity.y) * 180 / Mathf.PI;
                Vector3 rotVec = Vector3.back * zValue + Vector3.back * 45.0f;
                bullet.transform.Rotate(rotVec);
                
            }

            

            GameObject block = gameManager.CreateObj(blockName, GameManager.PoolTypes.BlockType);
            Block blockComponent = block.GetComponent<Block>();

            //��� �θ� ����
            block.transform.parent = this.transform;
            blockComponent.createPos = blockPoints[startIndex].position;

            //��� Ȱ��ȭ
            blockComponent.blockOnRPC();
        }
    }
}
