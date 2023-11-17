using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Assets.PixelHeroes.Scripts.ExampleScripts;
using KoreanTyper;
using Photon.Pun.Demo.PunBasics;
using Photon.Realtime;

public class TrainManager : MonoBehaviour
{
    //�ִ� �߻� �ð�
    float maxTime = 0.25f;
    //���� �߻� �ð�
    float curTime = 0f;

    [Header("�߻� ����")]
    public Transform[] trainPoints;

    [Header("���� ����")]
    public int scenelevel;

    [Header("�̵��� ���� �ڽ�")]
    public GameObject moveBox;

    [Header("�÷��̾� ��ũ��Ʈ")]
    public CharacterControls characterControls;

    //ä�ÿ� ���ð� 3��
    WaitForSeconds wait3_00 = new WaitForSeconds(3);

    BattleUIManager battleUIManager;
    [Header("���� �Ŵ���")]
    public GameManager gameManager;

    [Header("��� �迭")]
    public Block[] blockArr;


    private void Start()
    {
        battleUIManager = BattleUIManager.Instance;
        gameManager = battleUIManager.gameManager;

        //����� ���
        battleUIManager.audioManager.PlayBgm(AudioManager.BgmSingle.Train);

        //�÷��̾� ü�� ���� ���� 0���� ����
        characterControls.healthMinus = 0;
        //�÷��̾� ü�� ����
        characterControls.curHealth = 20;

        characterControls.changeStateRPC(CharacterControls.PlayerStateType.LeftControl, false);
        characterControls.changeStateRPC(CharacterControls.PlayerStateType.IsCanJump, false);
        characterControls.changeStateRPC(CharacterControls.PlayerStateType.RightControl, false);
        //Į�� �浹 ����
        characterControls.changeStateRPC(CharacterControls.PlayerStateType.SwordCollision, false);
        characterControls.backSwords.SetActive(true);

        battleUIManager.typingControl("�Ʒ��忡 �������!");
        StartCoroutine(moveBoxRoutine());

        //��� �����ֱ�
        foreach(Block block in blockArr)
        {
            block.healthControl(35);
        }
    }



    private void Update()
    {
        curTime += Time.deltaTime;
        

        if (curTime > maxTime)
        {
            //�Ŀ� �� ��Ÿ

            //�ð� �ʱ�ȭ
            curTime = 0f;
            GameObject bullet  = gameManager.CreateObj("PowerUpBullet", GameManager.PoolTypes.BulletType);

            //������Ʈ ����
            Rigidbody2D bulletRigid = bullet.GetComponent<Rigidbody2D>();
            Bullet bulletComponent = bullet.GetComponent<Bullet>();

            //� �߻�
            bullet.transform.parent = this.transform;
            bullet.transform.position = trainPoints[1].position;

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

            //�Ŀ� �� ��Ÿ

            //�ð� �ʱ�ȭ
            GameObject bullet2 = gameManager.CreateObj("NormalBullet", GameManager.PoolTypes.BulletType);

            //������Ʈ ����
            Rigidbody2D bullet2Rigid = bullet2.GetComponent<Rigidbody2D>();
            Bullet bullet2Component = bullet2.GetComponent<Bullet>();

            //� �߻�
            bullet2.transform.parent = this.transform;
            bullet2.transform.position = trainPoints[2].position;

            //� Ȱ��ȭ
            bullet2Component.bulletOnRPC();

            //�ӵ� ����
            Vector2 bullet2Vec = Vector2.down;

            //���� �ӵ� ����
            bullet2Rigid.velocity = bullet2Vec * bullet2Component.bulletSpeed;

            //ȸ�� ����
            bullet2.transform.rotation = Quaternion.identity;
            float zValue2 = Mathf.Atan2(bullet2Rigid.velocity.x, bullet2Rigid.velocity.y) * 180 / Mathf.PI;
            Vector3 rotVec2 = Vector3.back * zValue2 + Vector3.back * 45.0f;
            bullet2.transform.Rotate(rotVec2);
        }
    }

    #region �̵� ���� ����
    IEnumerator moveBoxRoutine()
    {
        yield return wait3_00;
        yield return wait3_00;

        moveBox.transform.position = trainPoints[0].position;
        
    }
    #endregion
}
