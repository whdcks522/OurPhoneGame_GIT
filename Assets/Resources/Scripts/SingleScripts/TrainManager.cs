using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Assets.PixelHeroes.Scripts.ExampleScripts;
using KoreanTyper;
using Photon.Pun.Demo.PunBasics;
using Photon.Realtime;
using Unity.VisualScripting.Antlr3.Runtime;

public class TrainManager : MonoBehaviour
{
    //�ִ� �߻� �ð�
    float maxTime = 0.5f;
    //���� �߻� �ð�
    float curTime = 0f;

    [Header("�߻� ����")]
    public GameObject[] trainPoints;

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
        //Į ���� ����
        characterControls.curSwordCount = 1;
        

        if (scenelevel == 0) 
        {
            //�÷��̾� ü�� ����
            characterControls.curHealth = 20;

            characterControls.changeStateRPC(CharacterControls.PlayerStateType.LeftControl, false);
            characterControls.changeStateRPC(CharacterControls.PlayerStateType.IsCanJump, false);
            characterControls.changeStateRPC(CharacterControls.PlayerStateType.RightControl, false);
            //Į�� �浹 ����
            characterControls.changeStateRPC(CharacterControls.PlayerStateType.SwordCollision, false);
            characterControls.backSwords.SetActive(false);

            battleUIManager.typingControl("�Ʒ��忡 �������!");
            StartCoroutine(moveBoxRoutine());

            //��� �����ֱ�
            foreach (Block block in blockArr)
            {
                block.healthControl(80);
            }
        }
    }

    private void OnDisable()
    {
        if (scenelevel == 1) 
        {
            battleUIManager.JoySizeControl(true);

            Debug.Log("ũ�� �ʱ�ȭ");
        }
    }

    private void Update()
    {
        if (scenelevel == 0) 
        {
            curTime += Time.deltaTime;


            if (curTime > maxTime)
            {
                //�ð� �ʱ�ȭ
                curTime = 0f;

                BulletShotter powerUpBulletShotter = trainPoints[1].GetComponent<BulletShotter>();
                powerUpBulletShotter.sortShot(BulletShotter.BulletShotType.Direction, Bullet.BulletEffectType.PowerUp,
                        trainPoints[1], trainPoints[2], 0);

                BulletShotter normalBulletShotter = trainPoints[2].GetComponent<BulletShotter>();
                powerUpBulletShotter.sortShot(BulletShotter.BulletShotType.Direction, Bullet.BulletEffectType.Normal,
                        trainPoints[3], trainPoints[4], 0);
            }
        }
    }

    #region �̵� ���� ����
    IEnumerator moveBoxRoutine()
    {
        yield return wait3_00;
        yield return wait3_00;

        moveBox.transform.position = trainPoints[0].transform.position;
        
    }
    #endregion
}
