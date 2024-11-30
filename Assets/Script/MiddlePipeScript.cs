using UnityEngine;

public class MiddlePipeScript : MonoBehaviour
{
    public LogicScript logic;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //When another 2D collider enter trigger zone, it will run.
    private void OnTriggerEnter2D(Collider2D collision) {
        logic.addScore();
    }
}
