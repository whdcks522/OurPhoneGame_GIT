using Assets.PixelHeroes.Scripts.ExampleScripts;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class Agent_Lizard : Agent
{
    public Rigidbody2D rigid;
    public Enemy enemy;
    public BulletShotter bulletShotter;
    public GameObject player;
    AudioManager audioManager;
    //������ �ִ� �Ÿ�(�Ÿ� ���̸� ���)
    public float maxRange;
    //������ �Ÿ�
    float curRange;
    void Start()
    {
        if (!enemy.isML)//�ӽŷ������� �ƴ϶��
        {
            player = enemy.player;

            bulletShotter.gameManager = enemy.gameManager;

            audioManager = enemy.battleUIManager.audioManager;
        }
    }
    //�ƽ� ������ �ø�ä�� ���� �̻� ����
    //��鿡�� �н��ϴٰ� ȯ�� 2�� �ű�
    

    Coroutine bigCor;
    WaitForSeconds wait = new WaitForSeconds(0.12f);
    IEnumerator bigShot()
    {
        //����
        yield return wait;

        audioManager.PlaySfx(AudioManager.Sfx.Arrow);
        bulletShotter.sortShot(BulletShotter.BulletShotType.Big, Bullet.BulletEffectType.Normal, gameObject, player, 0);//�븻�� �۰� ��ź
    }

    private void OnDisable()
    {
        if (bigCor != null)
            StopCoroutine(bigCor);
    }

    
    public override void OnActionReceived(ActionBuffers actions)//�׼� ����(������ ����), �� �� ȣ�� 
    {
        curRange = (player.transform.position - transform.position).magnitude;
        AddReward(0.005f);//* 10    (0~15)

        //Debug.Log(GetCumulativeReward());

        if (!enemy.isML)
        {
            if (enemy.maxTime <= enemy.curTime && curRange >= maxRange && gameObject.activeSelf)
            {
                //����
                enemy.reloadRPC(1f, "Shot");
                //���� ���
                bigCor = StartCoroutine(bigShot());
            }
        }

        //  Discrete Action(������ ��ȯ��, Ư�� �ൿ�� ����ϱ� ����(AllBuffered�� ���� ����?))
        //  mlagents-learn --force(�׳� �����ϱ�)

        int X = actions.DiscreteActions[0] - 1;
        int Y = actions.DiscreteActions[1];
        //Debug.Log("X: "+ X + "/ Y: " + Y);

        if (Y == 1 && enemy.isGround)
        {
            AddReward(-0.0100f);
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
        //�ڽ��� ����
        if (gameObject.activeSelf)
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
        if (enemy.isML)
        {
            if (other.transform.CompareTag("Outline")) //�� ������ �������� ���
            {
                AddReward(-6f);
                EndEpisode();
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == player && enemy.isML)
        {
            AddReward(-3f);//�������� �ű�
            EndEpisode();//�̰͸����� �ʱ�ȭ�� ���� ����
        }
    }

    [Header("���������")]
    public Transform[] points;

    public override void OnEpisodeBegin()//EndEpisode�� ȣ����� �� ����(���� ȣ���� ���� ��°�� ����)
    {
        if (enemy.isML)
        {
            //Debug.Log("Lizard");
            //�ڽ��� �ڵ忡 ���ؼ��� �ߵ���(���� �� �ϵ� ��� x, �浹�� ��� �Ѵ� �����)

            //��ũ�� �״´ٰ� ���ڵ� �ð��� �ʱ�ȭ���� ����

            int enemyIndex = Random.Range(0, points.Length);//����(��)�� ��ġ


            while (true)
            {
                int playerIndex = Random.Range(0, points.Length);//�÷��̾��� ��ġ
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
