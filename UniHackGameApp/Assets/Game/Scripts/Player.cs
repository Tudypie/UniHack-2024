using System;
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

    public Transform GetObstacleInFront(LayerMask layer)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 0.5f, layer))
        {
            return hit.transform;
        }
        return null;
    }

    public bool IsWallInFront()
    {
        if (GetObstacleInFront(LayerMask.GetMask("Wall"))) { return true; }
        return false;
    }

    public bool IsEnemyInFront()
    {
        if (GetObstacleInFront(LayerMask.GetMask("Enemy"))) { return true; }
        return false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W)) Move();
        if (Input.GetKeyDown(KeyCode.R)) Rotate();
        if (Input.GetKeyDown(KeyCode.Z)) Attack();
    }

    public void Move()
    {
        if (IsWallInFront() || IsEnemyInFront()) { return; }

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
        transform.position = new Vector3(mazePosition.x, transform.position.y, mazePosition.y);
    }

    public void Rotate()
    {
        direction++;
        if ((int)direction == 4) { direction = 0; }
        transform.Rotate(0, 90, 0);
    }

    public void Attack()
    {
        if (!IsEnemyInFront()) { return; }

        Transform enemyInFront = GetObstacleInFront(LayerMask.GetMask("Enemy"));
        Destroy(enemyInFront.gameObject);
    }
}
