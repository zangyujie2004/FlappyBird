using UnityEngine;

public class PipeGenerate : MonoBehaviour
{
    public float generate_rate = 1.5f;  // 管道生成频率（秒）
    private float timer = 0.0f;  // 用于计时的变量
    public float offset = 2.25f;  // 管道的偏移量

    private bool generating = true;  // 管道生成控制标志

    private ObjectPool pipePool;  // 引用对象池

    void Start()
    {
        // 获取场景中的对象池
        pipePool = FindObjectOfType<ObjectPool>();
    }

    void Update()
    {
        if (generating)
        {
            timer += Time.deltaTime;  // 累加时间

            if (timer >= generate_rate)  // 如果超过生成间隔
            {
                generatePipe();  // 生成管道
                timer = 0.0f;  // 重置计时器
            }
        }
    }

    void generatePipe()
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

        // 将新生成的管道加入到鸟的管道列表
        BirdAgent birdAgent = GameObject.FindObjectOfType<BirdAgent>();
        birdAgent.activePipes.Add(newPipe);  
    }

    public void ResetGenerator()
    {
        generating = true;  // 重新开始生成管道
    }

    public void StopGenerating()
    {
        generating = false;  // 停止生成管道
    }
}
