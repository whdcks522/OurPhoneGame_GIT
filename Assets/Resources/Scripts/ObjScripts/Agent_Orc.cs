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
    public CharacterControls characterControls;
    AudioManager audioManager;
    public float maxRange;
    void Start()
    {
        if (!enemy.isML) 
        {
            player = enemy.player;
            characterControls = enemy.characterControls;

            bulletShotter.gameManager = enemy.gameManager;

            audioManager = enemy.battleUIManager.audioManager;
        }
    }
    //�ƽ� ������ �ø�ä�� ���� �̻� ����
    
    Coroutine bigCor;
    WaitForSeconds wait = new WaitForSeconds(0.12f);
    IEnumerator bigSlash()
    {
        //����
        yield return wait;

        audioManager.PlaySfx(AudioManager.Sfx.Slash);
        bulletShotter.sortShot(BulletShotter.BulletShotType.Big, Bullet.BulletEffectType.UnBreakable, gameObject, player, 1);//�۰� ��ź
    }

    private void OnDisable()
    {
        if (bigCor != null)
            StopCoroutine(bigCor);
    }


    public override void OnActionReceived(ActionBuffers actions)//�׼� ����(������ ����), �� �� ȣ�� 
    {
        float curRange = (player.transform.position - transform.position).magnitude;
        AddReward(-0.0005f);

        if (!enemy.isML) 
        {
            if (enemy.maxTime <= enemy.curTime && curRange <= maxRange && gameObject.activeSelf)
            {
                //����
                enemy.reloadRPC(1f, "Slash");
                //���� ���
                bigCor = StartCoroutine(bigSlash());
            }
        }

        //  Discrete Action(������ ��ȯ��, Ư�� �ൿ�� ����ϱ� ����(AllBuffered�� ���� ����?))
        //  mlagents-learn --force

        int X = actions.DiscreteActions[0] - 1;
        int Y = actions.DiscreteActions[1];
        //Debug.Log("X: "+ X + "/ Y: " + Y);

        if (Y == 1) 
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
                sensor.AddObservation(player.transform.position.x - transform.position.x);
                sensor.AddObservation(player.transform.position.y - transform.position.y);
            }

            sensor.AddObservation(StepCount / (float)MaxStep);//������ ���� ����    //state size = 1
        }
        
    }

    

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ( enemy.isML) 
        {
            if (other.transform.CompareTag("Outline")) //�� ������ �������� ���
            {
                SetReward(-1f);
                EndEpisode();
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Finish") && enemy.isML)
        {

            SetReward(1f);
            EndEpisode();//�̰͸����� �ʱ�ȭ�� ���� ����
        }
    }

    [Header("���������")]
    public Transform[] points;

    public override void OnEpisodeBegin()//EndEpisode�� ȣ����� �� ����(���� ȣ���� ���� ��°�� ����)
    {
        if (enemy.isML) 
        {
            int r1 = Random.Range(0, points.Length);

            while (true)
            {
                int r2 = Random.Range(0, points.Length);
                if (r1 != r2)
                {
                    transform.position = points[r1].position;
                    rigid.velocity = Vector2.zero;

                    player.transform.position = points[r2].position;
                    break;
                }
            }
        }
    }
}
