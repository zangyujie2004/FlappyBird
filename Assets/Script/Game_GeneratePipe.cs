using UnityEngine;

public class Game_GeneratePipe : MonoBehaviour
{
    public GameObject Pipe;  // 管道预制件
    public float generate_rate = 1.5f;  // 管道生成频率
    private float timer = 0.0f;  // 用于计时的变量
    public float offset = 2.25f;  // 管道的偏移量

    private bool generating = true;  // 管道生成控制标志

    private ObjectPool pipePool;  // 引用对象池

    void Start()
    {
        // 获取场景中的对象池
        pipePool = GameObject.FindObjectOfType<ObjectPool>();
    }

    void Update()
    {
        if (generating && generate_rate > 0) 
        {
            timer += Time.deltaTime;  // 累加时间
            if (timer >= generate_rate) 
            {
                GeneratePipe();  // 生成管道
                timer = 0.0f;  // 重置计时器
            }
        }
    }

    void GeneratePipe()
    {
        float highestPoint = transform.position.y + offset;
        float lowestPoint = transform.position.y - offset;
        float randomY = Random.Range(lowestPoint, highestPoint);

        // 从对象池中获取管道
        GameObject newPipe = pipePool.GetObjectFromPool();
        newPipe.transform.position = new Vector3(transform.position.x, randomY, 0);
        newPipe.transform.rotation = transform.rotation;

        // 激活管道
        newPipe.SetActive(true);

        // 重置管道状态（例如速度和位置）
        PipeMove pipeMove = newPipe.GetComponent<PipeMove>();
        if (pipeMove != null)
        {
            pipeMove.ResetPipe(7.0f, new Vector3(10f, randomY, 0));  // 设置管道的初始位置和速度
        }
    }

    public void StopGenerating() 
    {
        generating = false;  // 停止生成管道
    }

    public void ResumeGenerating() 
    {
        generating = true;  // 恢复生成管道
    }

    public void ResetGenerator() 
    {
        generating = true;  // 重新开始生成管道
        timer = 0.0f;  // 重置计时器
    }
}