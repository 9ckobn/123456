using System.Collections;
using UnityEngine;

public class MoveableObstacle : MonoBehaviour, IObstacle
{
    [SerializeField] private float delayBeforeStartMove = 1f;
    [SerializeField] private float distanceToCheck = 50;
    private Player player = null;

    [SerializeField] private float speed = 10f;

    private bool isMoving = false;

    private void OnEnable()
    {
        player = Player.instance;
        StartCoroutine(CheckDistance());
    }

    IEnumerator CheckDistance()
    {
        while (player.blockRunning)
            yield return null;

        while (Vector3.Distance(transform.position, player.transform.position) > distanceToCheck)
        {
            for (int i = 0; i < 5; i++)
                yield return null;
        }

        yield return Move();
    }

    IEnumerator Move()
    {
        yield return new WaitForSeconds(delayBeforeStartMove);
        isMoving = true;

        Vector3 velocity = new Vector3(0, 0, speed);

        while (isMoving)
        {
            transform.Translate(velocity * Time.deltaTime);
            yield return null;
        }

        gameObject.SetActive(false);
    }

    public int OnCollideGetDamage()
    {
        throw new System.NotImplementedException();
    }

    public void DisableObstacle()
    {
        gameObject.SetActive(false);
    }
}