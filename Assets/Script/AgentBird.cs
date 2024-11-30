using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System.Collections.Generic;

public class BirdAgent : Agent
{
    // Components
    public Rigidbody2D myRigidbody;
    public Animator myAnimator;
    public LogicScript logic;

    // Game parameters
    public float fly_strength = 5f;
    public float topBoundary = 5f;
    public float bottomBoundary = -5f;

    // State
    public bool birdIsAlive = true;
    public float distanceToNextPipe;  // Horizontal distance to the next pipe
    public float heightOfNextPipe;    // Height of the next pipe (top or bottom)

    // 管道相关
    public List<GameObject> activePipes = new List<GameObject>();  // 存储当前活跃的管道

    private GameObject currentPipe;  // 当前正在被观察的管道

    private ObjectPool pipePool;  // 对象池

    public override void Initialize()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myRigidbody.freezeRotation = true;
        myAnimator = GetComponent<Animator>();
        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript>();
        pipePool = GameObject.FindObjectOfType<ObjectPool>();  // 获取对象池的引用
    }

    public override void OnEpisodeBegin()
    {
        // 重置 Bird 的状态和位置
        birdIsAlive = true;
        myRigidbody.linearVelocity = Vector2.zero;  // 清除之前的速度
        transform.position = new Vector3(0f, 0f, 0f);  // 重新设置鸟的位置
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);  // 保持不旋转
        
        // 重新绑定动画
        myAnimator.Rebind();

        // 禁用当前所有管道
        foreach (var pipe in activePipes)
        {
            pipe.SetActive(false);  // 禁用管道对象
            pipePool.ReturnObjectToPool(pipe);  // 将管道归还对象池
        }
        activePipes.Clear();  // 清空管道列表

        // 管道生成器重新开始生成管道
        PipeGenerate pipeGenerate = GameObject.FindObjectOfType<PipeGenerate>();
        pipeGenerate.ResetGenerator();

        logic.ResetScore();  // 重置分数
        
        currentPipe = null;  // 重置当前目标管道
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        if (!birdIsAlive) return;

        // 添加 Bird 的位置和速度作为观察数据
        sensor.AddObservation(transform.position.y);  // Bird 位置
        sensor.AddObservation(myRigidbody.linearVelocity.y); // Bird 速度

        // 获取最近的管道信息
        if (activePipes.Count > 0)
        {
            // 选择当前视野内的最近管道
            GameObject closestPipe = GetNextPipeInView();

            if (closestPipe != null)
            {
                // 更新当前管道
                currentPipe = closestPipe;

                // 获取管道的 x 和 y 位置
                float pipeX = closestPipe.transform.position.x;
                float pipeY = closestPipe.transform.position.y;

                // 计算与鸟之间的距离
                distanceToNextPipe = pipeX - transform.position.x;
                heightOfNextPipe = pipeY;

                // 添加管道信息作为观察数据
                sensor.AddObservation(distanceToNextPipe);  // Horizontal distance to next pipe
                sensor.AddObservation(heightOfNextPipe);    // Height of the next pipe
            }
        }
    }

    // 选择当前视野内的下一个管道
    private GameObject GetNextPipeInView()
    {
        GameObject nextPipe = null;

        foreach (var pipe in activePipes)
        {
            float distance = pipe.transform.position.x - transform.position.x;

            // 只关注在视野内且尚未跨越的管道
            if (distance > -2.5 && distance < 12)  // 确保管道在视野范围内
            {
                if (nextPipe == null || distance < nextPipe.transform.position.x - transform.position.x)
                {
                    nextPipe = pipe;  // 选择最接近的管道
                }
            }
        }

        return nextPipe;
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        if (!birdIsAlive) return;

        int action = actions.DiscreteActions[0];

        if (action == 1)  // Flap
        {
            myRigidbody.linearVelocity = Vector2.up * fly_strength;
            myAnimator.SetTrigger("WingTrigger");
            AddReward(-2.0f);
        }

        // 检查管道通过
        if (currentPipe != null && -2 > currentPipe.transform.position.x && birdIsAlive)
        {
            AddReward(10f); // 成功通过管道，奖励
            currentPipe = null;  // Reset the current pipe after passing
        }

        // 检查生存奖励
        if (birdIsAlive)
        {
            AddReward(0.5f); // 每帧存活奖励
        }

        // 检查撞墙或者撞到地面
        if (transform.position.y >= topBoundary || transform.position.y <= bottomBoundary)
        {
            AddReward(-10f); // 撞墙或地面，死亡惩罚
            birdIsAlive = false;
            EndEpisode(); // 游戏结束，结束回合
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!birdIsAlive) return;
        
        birdIsAlive = false;
        AddReward(-10f); // 撞墙或地面，死亡惩罚
        EndEpisode(); // 游戏结束，结束回合
    }
}
