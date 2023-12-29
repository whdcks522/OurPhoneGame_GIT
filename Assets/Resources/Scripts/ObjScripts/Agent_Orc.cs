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
    //맥스 스텝은 늘린채로 놔도 이상 없음
    //평면에서 학습하다가 환경 2로 옮김
    //<Enemy_Orc>
    //mlagents-learn "D:\gitHubDeskTop\ML_EX_GIT\config\ppo\Enemy_Orc.yaml" --run-id=Enemy_Orc_K --resum(2시간즈음부터 성능 향상 시작됨)

    Coroutine bigCor;
    WaitForSeconds wait = new WaitForSeconds(0.12f);
    IEnumerator bigSlash()
    {
        //저격
        yield return wait;

        audioManager.PlaySfx(AudioManager.Sfx.Slash);
        bulletShotter.sortShot(BulletShotter.BulletShotType.Big, Bullet.BulletEffectType.UnBreakable, gameObject, player, 1);//크게 산탄
    }

    private void OnDisable()
    {
        if (bigCor != null)
            StopCoroutine(bigCor);
    }

    //대상과의 거리
    float curRange;
    public override void OnActionReceived(ActionBuffers actions)//액션 기입(가능한 동작), 매 번 호출 
    {
        curRange = (player.transform.position - transform.position).magnitude;
        AddReward(-0.0005f);

        if (!enemy.isML) 
        {
            if (enemy.maxTime <= enemy.curTime && curRange <= maxRange && gameObject.activeSelf)
            {
                //장전
                enemy.reloadRPC(1f, "Slash");
                //실제 사격
                bigCor = StartCoroutine(bigSlash());
            }
        }

        //  Discrete Action(정수를 반환함, 특정 행동에 사용하기 좋음(AllBuffered와 같은 느낌?))
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

    public override void Heuristic(in ActionBuffers actionsOut)//잘 작동하는지 확인하는 법 1. 휴리스틱: 키보드를 통해 에이전트를 조정
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

    public override void CollectObservations(VectorSensor sensor)//관찰할 정보, 5번 당 한번 호출
    {
        
        //1. 수치형, 받아오는 데이터가 적을 수록 좋음
        //자신의 정보
        if (gameObject.activeSelf) 
        {
            sensor.AddObservation(transform.position.x);//state size = 1     x,y,z를 모두 받아오면 size가 3이 됨
            sensor.AddObservation(transform.position.y);

            //가속을 더하기도 함
            sensor.AddObservation(rigid.velocity.x);//state size = 1
            sensor.AddObservation(rigid.velocity.y);

            if (player != null) //시작 한 순간, 빈 취급됨
            {
                //플레이어의 정보
                sensor.AddObservation(player.transform.position.x);
                sensor.AddObservation(player.transform.position.y);
                //각각의 거리
                sensor.AddObservation(curRange);
            }

            sensor.AddObservation(StepCount / (float)MaxStep);//진행한 스텝 비율    //state size = 1
        }
        
    }

    

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ( enemy.isML) 
        {
            if (other.transform.CompareTag("Outline")) //맵 밖으로 나가지면 사망
            {
                AddReward(-1f);
                EndEpisode();
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == player && enemy.isML)
        {
            AddReward(10f);//점프였나 거기
            EndEpisode();//이것만으로 초기화가 되진 않음
        }
    }

    [Header("재시작점들")]
    public Transform[] points;

    public override void OnEpisodeBegin()//EndEpisode가 호출됐을 때 사용됨(씬을 호출할 때는 통째로 삭제)
    {
        if (enemy.isML) //자신에 의해서만 발동
        {
            int enemyIndex = Random.Range(0, points.Length);
            

            while (true)
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
