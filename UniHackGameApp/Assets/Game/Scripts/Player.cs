using DG.Tweening;
using UnityEngine;

public class Player : MonoBehaviour
{
    public enum Direction
    {
        Front = 0,
        Right = 1,
        Back = 2,
        Left = 3
    }

    public Vector2 mazePosition;
    public Direction direction;

    private CircuitEvaluator evaluator;
    private Animator anim;

    [SerializeField] private int ticksSinceCheeseWasEaten = 0;
    [SerializeField] AudioSource audioMove;
    [SerializeField] AudioSource audioDie;
    [SerializeField] AudioSource audioBearTrap;
    [SerializeField] AudioSource audioEat;

    [SerializeField] private bool ateCheese = false;
    private bool isDead = false;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        evaluator = CircuitEvaluator.Instance;
        evaluator.onBeforeTick += OnBeforeTick;
        evaluator.onAfterTick += OnAfterTick;
    }

    private void OnDestroy()
    {
        if (evaluator)
        {
            evaluator.onBeforeTick -= OnBeforeTick;
            evaluator.onAfterTick -= OnAfterTick;
        }
    }

    private void OnBeforeTick()
    {
        evaluator.SetInput("wall_front", IsWallFront());
        evaluator.SetInput("wall_right", IsWallRight());
        evaluator.SetInput("wall_back", IsWallBack());
        evaluator.SetInput("wall_left", IsWallLeft());
        evaluator.SetInput("cheese_near", IsCheeseAround());
        evaluator.SetInput("cheese_front", IsCheeseFront());
        evaluator.SetInput("ate_cheese", ateCheese);
        evaluator.SetInput("trap_front", IsTrapFront());
    }

    private void OnAfterTick()
    {
        if (evaluator.ReadOutput("rotate_right")) { Rotate(1); }
        if (evaluator.ReadOutput("rotate_left")) { Rotate(-1); }
        if (evaluator.ReadOutput("move")) { Move(); audioMove.Play(); }

        if (ateCheese)
        {
            ticksSinceCheeseWasEaten++;
            if (ticksSinceCheeseWasEaten >= 1)
            {
                ticksSinceCheeseWasEaten = 0;
                ateCheese = false;
            }
        }
    }

    private void Update()
    {
        if (isDead) return;
        if (Input.GetKeyDown(KeyCode.W)) Move();
        if (Input.GetKeyDown(KeyCode.R)) Rotate(1);
        if (Input.GetKeyDown(KeyCode.Z)) Attack();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Trap"))
        {
            Die();
        }

        if (other.gameObject.CompareTag("Cheese"))
        {
            audioEat.Play();
            ateCheese = true;
            LevelManager.Instance.mazeManager.CollectCheese();
            Destroy(other.gameObject);
        }
    }

    public void Move()
    {
        if (IsWallFront()) { return; }

        Vector2 newMazePos = Vector2.zero;
        switch (direction)
        {
            case Direction.Front:
                newMazePos = new Vector2(mazePosition.x, mazePosition.y + 1);
                break;
            case Direction.Right:
                newMazePos = new Vector2(mazePosition.x + 1, mazePosition.y);
                break;
            case Direction.Left:
                newMazePos = new Vector2(mazePosition.x - 1, mazePosition.y);
                break;
            case Direction.Back:
                newMazePos = new Vector2(mazePosition.x, mazePosition.y - 1);
                break;

        }
        mazePosition = newMazePos;
        transform.DOMove(new Vector3(mazePosition.x, transform.position.y, mazePosition.y), 0.5f);
        anim.Play("Walk");
    }

    public void Rotate(int x)
    {
        direction += x;
        if ((int)direction == 4) { direction = 0; }
        else if((int)direction == -1) { direction = (Direction)3; }
        transform.Rotate(0, 90*x, 0);
    }

    public void Attack()
    {
        Transform enemyInFront = GetObstacleInDirection(LayerMask.GetMask("Enemy", "Wall"), LayerMask.GetMask("Enemy"), transform.forward);
        Destroy(enemyInFront.gameObject);
    }

    public void Die()
    {
        if (isDead) { return; }
        audioDie.Play();
        audioBearTrap.Play();
        isDead = true;
        anim.Play("Die");
        LevelManager.Instance.Lose();
    }

    private bool IsWallFront()
    {
        if (GetObstacleInDirection(LayerMask.GetMask("Wall"), LayerMask.GetMask("Wall"), transform.forward)) { return true; }
        return false;
    }

    private bool IsWallRight()
    {
        if (GetObstacleInDirection(LayerMask.GetMask("Wall"), LayerMask.GetMask("Wall"), transform.right)) { return true; }
        return false;
    }

    private bool IsWallBack()
    {
        if (GetObstacleInDirection(LayerMask.GetMask("Wall"), LayerMask.GetMask("Wall"), -transform.forward)) { return true; }
        return false;
    }

    private bool IsWallLeft()
    {
        if (GetObstacleInDirection(LayerMask.GetMask("Wall"), LayerMask.GetMask("Wall"), -transform.right)) { return true; }
        return false;
    }

    private bool IsCheeseAround()
    {
        if (GetObstacleInDirection(LayerMask.GetMask("Cheese", "Wall"), LayerMask.GetMask("Cheese"), transform.right)
            || GetObstacleInDirection(LayerMask.GetMask("Cheese", "Wall"), LayerMask.GetMask("Cheese"), -transform.forward)
            || GetObstacleInDirection(LayerMask.GetMask("Cheese", "Wall"), LayerMask.GetMask("Cheese"), -transform.right))
        { 
            return true; 
        }
        return false;
    }

    private bool IsCheeseFront()
    {
        if (GetObstacleInDirection(LayerMask.GetMask("Cheese", "Wall"), LayerMask.GetMask("Cheese"), transform.forward))
        {
            return true;
        }
        return false;
    }

    private bool IsTrapAround()
    {
        if (GetObstacleInDirection(LayerMask.GetMask("Trap", "Wall"), LayerMask.GetMask("Trap"), transform.right)
            || GetObstacleInDirection(LayerMask.GetMask("Trap", "Wall"), LayerMask.GetMask("Trap"), -transform.forward)
            || GetObstacleInDirection(LayerMask.GetMask("Trap", "Wall"), LayerMask.GetMask("Trap"), -transform.right))
        {
            return true;
        }
        return false;
    }

    private bool IsTrapFront()
    {
        if (GetObstacleInDirection(LayerMask.GetMask("Trap", "Wall"), LayerMask.GetMask("Trap"), transform.forward)) { return true; }
        return false;
    }

    private Transform GetObstacleInDirection(LayerMask raycastLayer, LayerMask targetLayer, Vector3 direction)
    {
        RaycastHit hit;
        if (Physics.Raycast(new Vector3(mazePosition.x, transform.position.y, mazePosition.y),
            direction, out hit, 1f, raycastLayer))
        {
            if ((1<<hit.transform.gameObject.layer) == targetLayer)
                return hit.transform;
        }
        return null;
    }
}
