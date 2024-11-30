using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LogicStartScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame(){
        SceneManager.LoadScene("GameScene");
        Debug.Log("Load GameScene successfully");
    }

    public void AgentGame(){
        SceneManager.LoadScene("AgentScene");
        Debug.Log("Load AgentScene successfully");
    }
    public void quit(){
        Application.Quit();
        Debug.Log("Quit");
    }
}
