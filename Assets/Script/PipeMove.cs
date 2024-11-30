using UnityEngine;

public class PipeMove : MonoBehaviour
{
    public float PipeVelocity = 7.0f;
    public float deadPlace = -12.0f;
    
    private ObjectPool pipePool; // 引入对象池

    void Start()
    {
        // 获取对象池实例
        pipePool = FindObjectOfType<ObjectPool>();
    }

    void Update()
    {
        transform.position += Vector3.left * PipeVelocity * Time.deltaTime;
        if (transform.position.x < deadPlace)
        {
            // 回收到对象池
            pipePool.ReturnObjectToPool(gameObject);
        }
    }

    public void ResetPipe(float newVelocity = 7.0f, Vector3? newPosition = null)
    {
        // 设置管道的初始速度
        PipeVelocity = newVelocity;

        // 如果提供了新的位置，设置管道的新位置
        if (newPosition.HasValue)
        {
            transform.position = newPosition.Value;
        }
        else
        {
            // 如果没有提供新的位置，设置为默认生成位置
            transform.position = new Vector3(10f, Random.Range(-2f, 2f), 0f);
        }
    }
}
