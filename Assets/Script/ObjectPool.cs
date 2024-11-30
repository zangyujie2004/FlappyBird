using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public GameObject pipePrefab;  // 管道预制件
    public int poolSize = 5;  // 池的大小
    private Queue<GameObject> availablePipes = new Queue<GameObject>();  // 存储可用管道

    void Start()
    {
        // 初始化池
        for (int i = 0; i < poolSize; i++)
        {
            GameObject pipe = Instantiate(pipePrefab);
            pipe.SetActive(false);  // 初始时不激活
            availablePipes.Enqueue(pipe);  // 添加到池中
        }
    }

    // 从池中获取一个管道对象
    public GameObject GetObjectFromPool()
    {
        if (availablePipes.Count > 0)
        {
            GameObject pipe = availablePipes.Dequeue();
            pipe.SetActive(true);  // 激活管道对象
            return pipe;
        }
        else
        {
            // 如果池中没有对象，创建一个新管道
            GameObject pipe = Instantiate(pipePrefab);
            return pipe;
        }
    }

    // 将管道对象归还到池中
    public void ReturnObjectToPool(GameObject pipe)
    {
        pipe.SetActive(false);  // 禁用管道对象
        availablePipes.Enqueue(pipe);  // 放回池中
    }
}
