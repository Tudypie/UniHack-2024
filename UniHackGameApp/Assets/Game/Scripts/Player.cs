using DG.Tweening;
using System;
using UnityEditor.Experimental.GraphView;
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

    [SerializeField] private MazeManager mazeManager;

    private CircuitEvaluator evaluator;

    private Animator anim;

    public bool IsDead { get; private set; }

    public bool IsWallFront()
    {
        if (GetObstacleInDirection(LayerMask.GetMask("Wall"), transform.forward)) { return true; }
        return false;
    }

    public bool IsWallRight()
    {
        if (GetObstacleInDirection(LayerMask.GetMask("Wall"), transform.right)) { return true; }
        return false;
    }

    public bool IsWallBack()
    {
        if (GetObstacleInDirection(LayerMask.GetMask("Wall"), -transform.forward)) { return true; }
        return false;
    }

    public bool IsWallLeft()
    {
        if (GetObstacleInDirection(LayerMask.GetMask("Wall"), -transform.right)) { return true; }
        return false;
    }

    public bool IsTrapFront()
    {
        if (GetObstacleInDirection(LayerMask.GetMask("Trap"), transform.forward)) { return true; }
        return false;
    }

    public Transform GetObstacleInDirection(LayerMask layer, Vector3 direction)
    {
        RaycastHit hit;
        if (Physics.Raycast(new Vector3(mazePosition.x, transform.position.y, mazePosition.y),
            direction, out hit, 1f, layer))
        {
            return hit.transform;
        }
        return null;
    }

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

    private void OnBeforeTick()
    {
        evaluator.SetInput("wall_front", IsWallFront());
        evaluator.SetInput("wall_right", IsWallRight());
        evaluator.SetInput("wall_back", IsWallBack());
        evaluator.SetInput("wall_left", IsWallLeft());
        evaluator.SetInput("trap_front", IsTrapFront());
    }

    private void OnAfterTick()
    {
        if (evaluator.ReadOutput("move")) { Move(); }
        if (evaluator.ReadOutput("rotate_right")) { Rotate(1); }
        if (evaluator.ReadOutput("rotate_left")) { Rotate(-1); }
    }

    private void Update()
    {
        if (IsDead) return;
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
            mazeManager.CollectCheese();
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
        Transform enemyInFront = GetObstacleInDirection(LayerMask.GetMask("Enemy"), transform.forward);
        Destroy(enemyInFront.gameObject);
    }

    public void Die()
    {
        if (IsDead) { return; }
        IsDead = true;
        anim.Play("Die");
        GameManager.Instance.ShowLosePanel();
    }
}
