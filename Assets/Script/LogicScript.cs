using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LogicScript : MonoBehaviour
{
    public Text scoreText;
    public int playerScore = 0;
    public GameObject gameOver;
    public AudioSource coinSound;

    void Start()
    {
        // 播放背景音乐
        BackgroundMusicManager bgMusicManager = GameObject.FindObjectOfType<BackgroundMusicManager>();
        if (bgMusicManager != null)
        {
            bgMusicManager.PlayMusic();  // 确保背景音乐播放
        }
    }

    void Update()
    {
        // 这里可以加入其他游戏逻辑
    }

    [ContextMenu("Increasing Score")]
    public void addScore() {
        playerScore = playerScore + 1;
        scoreText.text = playerScore.ToString();
        coinSound.Play();
    } 

    public void RestarteGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GameOver() {
        gameOver.SetActive(true);
    }

    public void ResetScore()
    {
        playerScore = 0;
        scoreText.text = playerScore.ToString();  // 重置分数显示
        coinSound.Stop();  // 停止播放音效（如果需要）
    }
    
    public void QuitToHome(){
        // 在退出时停止背景音乐
        BackgroundMusicManager.StopBackgroundMusic();

        SceneManager.LoadScene("HomeScene");  // 加载主菜单场景
    }
}
