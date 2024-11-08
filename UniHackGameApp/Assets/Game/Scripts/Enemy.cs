using UnityEngine;

public class Enemy : MonoBehaviour
{
    private void CheckPlayerInDirection(Vector3 direction)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, 1f, LayerMask.GetMask("Player")))
        {
            if (hit.transform.TryGetComponent(out Player player))
            {
                player.Die();
            }
        }
    }

    private void Update()
    {
        Attack();
    }

    public void Attack()
    {
        CheckPlayerInDirection(transform.forward);
        CheckPlayerInDirection(-transform.forward);
        CheckPlayerInDirection(transform.right);
        CheckPlayerInDirection(-transform.right);
    }
}
