using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MovingObject {

    public int wallDamage = 1;
    public int pointsPerFood = 10;
    public int pointsPerSoda = 20;
    public float restartLevelDelay = 1f;

    public Text foodText;

    private Animator animator;
    private int food;

    // Use this for initialization
    protected override void Start()
    {
        animator = GetComponent<Animator>();

        food = GameManager.instance.playerFoodPoints;

        foodText.text = "Food " + food;

        base.Start();
    }

    // Update is called once per frame
    public void Update()
    {
        if (!GameManager.instance.playersTurn) return;

        // store direction of movement
        int horizontal = 0;
        int vertical = 0;

        // take values from keyboard
        horizontal = (int)Input.GetAxisRaw("Horizontal");
        vertical = (int)Input.GetAxisRaw("Vertical");

        // prevent diagonal
        if (horizontal != 0)
            vertical = 0;

        // we tried to move
        if (horizontal != 0 || vertical != 0)
            AttemptMove<Wall>(horizontal, vertical);
    }

    public void LoseFood(int loss)
    {
        animator.SetTrigger("playerHit");

        food -= loss;

        foodText.text = " - " + loss + " Food: " + food;

        CheckIfGameOver();
    }

    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        food--;

        foodText.text = "Food " + food;

        base.AttemptMove<T>(xDir, yDir);

        RaycastHit2D hit;

        CheckIfGameOver();

        GameManager.instance.playersTurn = false;
    }

    protected override void OnCantMove<T>(T component)
    {
        Wall hitWall = component as Wall;
        hitWall.DamageWall(wallDamage);

        animator.SetTrigger("playerAttack");
    }

    private void OnDisable()
    {
        GameManager.instance.playerFoodPoints = food;
    }

    private void CheckIfGameOver()
    {
        if (food <= 0)
            GameManager.instance.GameOver();
    }

    // if we hit exit
    private void Restart()
    {
        SceneManager.LoadScene(0);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Exit")
        {
            Invoke("Restart", restartLevelDelay);
            enabled = false;
        }
        else if (other.tag == "Food")
        {
            food += pointsPerFood;

            foodText.text = "+ " + pointsPerFood + " Food: " + food;

            //remove food
            other.gameObject.SetActive(false);
        }
        else if (other.tag == "Soda") {
            food += pointsPerSoda;

            foodText.text = "+ " + pointsPerFood + " Food: " + food;

            //remove soda
            other.gameObject.SetActive(false);
        }
            
    }
}
