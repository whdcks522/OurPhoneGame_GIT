using Assets.PixelHeroes.Scripts.ExampleScripts;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class Agent_Orc : Agent
{
    public Rigidbody2D rigid;
    public Enemy enemy;
    public BulletShotter bulletShotter;
    public GameObject player;
    AudioManager audioManager;
    public float maxRange;
    void Start()
    {
        if (!enemy.isML) 
        {
            player = enemy.player;

            bulletShotter.gameManager = enemy.gameManager;

            audioManager = enemy.battleUIManager.audioManager;
        }
    }
    //�ƽ� ������ �ø�ä�� ���� �̻� ����
    //��鿡�� �н��ϴٰ� ȯ�� 2�� �ű�
    //<Enemy_Orc>
    //mlagents-learn "D:\gitHubDeskTop\ML_EX_GIT\config\ppo\Enemy_Orc.yaml" --run-id=Enemy_Orc_K --resum(2�ð��������� ���� ��� ���۵�)

    Coroutine bigCor;
    WaitForSeconds wait = new WaitForSeconds(0.12f);
    IEnumerator directSlash()
    {
        if (!enemy.isPrison)//�Ʒ� 1������ ���� ���ϵ���
        {
            //����
            yield return wait;

            audioManager.PlaySfx(AudioManager.Sfx.Slash);
            bulletShotter.sortShot(BulletShotter.BulletShotType.Direction, Bullet.BulletEffectType.Normal, gameObject, player, 1);
            bulletShotter.sortShot(BulletShotter.BulletShotType.Direction, Bullet.BulletEffectType.Normal, gameObject, player, 1);
        }  
    }

    private void OnDisable()
    {
        if (bigCor != null)
            StopCoroutine(bigCor);
    }

    //������ �Ÿ�
    float curRange;

    public override void OnActionReceived(ActionBuffers actions)//�׼� ����(������ ����), �� �� ȣ�� 
    {
        curRange = (player.transform.position - transform.position).magnitude;
        AddReward(-0.0005f);

        if (!enemy.isML) 
        {
            if (enemy.maxTime <= enemy.curTime && curRange <= maxRange && gameObject.activeSelf)
            {
                //����
                enemy.reloadRPC(enemy.intervalTime, "Slash");
                //���� ���
                bigCor = StartCoroutine(directSlash());
            }
        }

        //  Discrete Action(������ ��ȯ��, Ư�� �ൿ�� ����ϱ� ����(AllBuffered�� ���� ����?))
        //  mlagents-learn --force

        int X = actions.DiscreteActions[0] - 1;
        int Y = actions.DiscreteActions[1];
        //Debug.Log("X: "+ X + "/ Y: " + Y);

        if (Y == 1 && enemy.isGround) 
        {
            AddReward(-0.0050f);
        }

        enemy.xyRPC(X, Y);
    }

    public override void Heuristic(in ActionBuffers actionsOut)//�� �۵��ϴ��� Ȯ���ϴ� �� 1. �޸���ƽ: Ű���带 ���� ������Ʈ�� ����
    {
        var disCreteActionOut = actionsOut.DiscreteActions;

        int x = 1;
        int y = 0;

        if (Input.GetKey(KeyCode.Z))
            x = 0;
        if (Input.GetKey(KeyCode.C))
            x = 2;

        if (Input.GetKey(KeyCode.S))
            y = 1;

        disCreteActionOut[0] = x;
        disCreteActionOut[1] = y;
    }

    public override void CollectObservations(VectorSensor sensor)//������ ����, 5�� �� �ѹ� ȣ��
    {
        //1. ��ġ��, �޾ƿ��� �����Ͱ� ���� ���� ����
        if (gameObject.activeSelf) //������ ���� �ν���  �ʿ� ����
        {
            sensor.AddObservation(transform.position.x);//state size = 1     x,y,z�� ��� �޾ƿ��� size�� 3�� ��
            sensor.AddObservation(transform.position.y);

            //������ ���ϱ⵵ ��
            sensor.AddObservation(rigid.velocity.x);//state size = 1
            sensor.AddObservation(rigid.velocity.y);

            if (player != null) //���� �� ����, �� ��޵�
            {
                //�÷��̾��� ����
                sensor.AddObservation(player.transform.position.x);
                sensor.AddObservation(player.transform.position.y);
                //������ �Ÿ�
                sensor.AddObservation(curRange);
            }

            sensor.AddObservation(StepCount / (float)MaxStep);//������ ���� ����    //state size = 1
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ( enemy.isML) 
        {
            if (other.transform.CompareTag("Outline")) //�� ������ ������ ����
            {
                AddReward(-1f);
                EndEpisode();
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)//�÷��̾�� �浹�ϸ� ����
    {
        if (collision.gameObject == player && enemy.isML)
        {
            AddReward(10f);//�������� �ű�
            EndEpisode();//�̰͸����� �ʱ�ȭ�� ���� ����
        }
    }

    [Header("���������")]
    public Transform[] points;

    public override void OnEpisodeBegin()//EndEpisode�� ȣ����� �� ����(���� ȣ���� ���� ��°�� ����)
    {
        //���� �н��� ���� �ʱ�ȭ�ϴ� �Լ�

        if (enemy.isML) //�ڽſ� ���ؼ��� �ߵ�
        {
            //Debug.Log("Orc");
            //�ڽ��� �ڵ忡 ���ؼ��� �ߵ���(�÷��̾ �ʱ�ȭ�ص� �� �ڵ�� ȣ�� x, �浹�� ��� �Ѵ� �����)

            int enemyIndex = Random.Range(0, points.Length);
            
            while (true)//������ ��ġ���� �ٽ� ��ȯ��(�پ��� ��ġ���� �н��� ����)
            {
                int playerIndex = Random.Range(0, points.Length);
                if (enemyIndex != playerIndex)
                {
                    transform.position = points[enemyIndex].position;
                    rigid.velocity = Vector2.zero;

                    player.transform.position = points[playerIndex].position;
                    break;
                }
            }    
        }
    }
}
