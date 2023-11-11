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
    float maxTime = 1f;
    //���� �߻� �ð�
    float curTime = 0f;
    //�߻� ��ġ ����
    int count = 1;

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


    private void Awake()
    {
        
    }

    private void Start()
    {
        battleUIManager = BattleUIManager.Instance;
        gameManager = battleUIManager.gameManager;

        //�÷��̾� ü�� ���� ���� 0���� ����
        characterControls.healthMinus = 0;
        //�÷��̾� ü�� ����
        characterControls.curHealth = 20;

        characterControls.changeStateRPC(CharacterControls.PlayerStateType.LeftControl, false);
        characterControls.changeStateRPC(CharacterControls.PlayerStateType.IsCanJump, false);
        characterControls.changeStateRPC(CharacterControls.PlayerStateType.RightControl, false);
        //Į�� �浹 ����
        characterControls.changeStateRPC(CharacterControls.PlayerStateType.SwordCollision, false);

        battleUIManager.typingControl("�Ʒüҿ� �������!");
        StartCoroutine(moveBoxRoutine());
    }

    private void Update()
    {
        curTime += Time.deltaTime;
        

        if (curTime > maxTime)
        {
            //�ð� �ʱ�ȭ
            curTime = 0f;
            GameObject bullet  = gameManager.CreateObj("GreenStarBullet", GameManager.PoolTypes.BulletType);

            //������Ʈ ����
            Rigidbody2D bulletRigid = bullet.GetComponent<Rigidbody2D>();
            Bullet bulletComponent = bullet.GetComponent<Bullet>();

            //��ġ ����
            count++;
            if (count == 3) count = 1;

            //� �߽�

            bullet.transform.parent = this.transform;
            bullet.transform.position = trainPoints[count].position;

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

    #region �̵� ���� ����
    IEnumerator moveBoxRoutine()
    {
        yield return wait3_00;
        yield return wait3_00;

        moveBox.transform.position = trainPoints[0].position;
        
    }
    #endregion
}
