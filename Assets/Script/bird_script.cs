using UnityEngine;

public class bird_script : MonoBehaviour
{
    //Cite Regidbody
    public Rigidbody2D myRigidbody;
    public float fly_strength;
    public LogicScript logic;
    public bool birdisAlive = true;
    public Animator myAnimator;

    // Screen boundaries
    public float topBoundary = 5f;   // Set your top boundary (adjust based on your scene)
    public float bottomBoundary = -5f; // Set your bottom boundary (adjust based on your scene)
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript>();
        myAnimator = GetComponent<Animator>(); // automatic way to get component
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) && birdisAlive) {
            myRigidbody.linearVelocity = Vector2.up * fly_strength;
            myAnimator.SetTrigger("WingTrigger"); //trigger 
        }
        if (transform.position.y > topBoundary || transform.position.y < bottomBoundary) {
            logic.GameOver();
            birdisAlive = false;
        }
        
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        logic.GameOver();
        birdisAlive = false;
    }
}
