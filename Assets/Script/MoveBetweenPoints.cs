using UnityEngine;

public class MoveBetweenPoints : MonoBehaviour
{
    public Transform startPoint;
    public Transform endPoint;
    public float speed = 2f;
    public bool moveVertical = false;

    private Vector3 targetPos;
    private Vector3 localStart;
    private Vector3 localEnd;

    void Start()
    {
        if (startPoint == null || endPoint == null)
        {
            Debug.LogError("MoveBetweenPoints: startPoint Ç∆ endPoint Çê›íËÇµÇƒÇ≠ÇæÇ≥Ç¢");
            enabled = false;
            return;
        }
        targetPos = endPoint.position;
        localStart = startPoint.position;
        localEnd = endPoint.position;
    }

    void Update()
    {
        Vector3 next = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

        if (moveVertical)
        {
            // X Ç startPoint.x Ç…å≈íË
            next.x = localStart.x;
        }
        else
        {
            // Y Ç startPoint.y Ç…å≈íË
            next.y = localStart.y;
        }

        transform.position = next;

        if (Vector3.Distance(transform.position, targetPos) < 0.01f)
        {
            targetPos = (targetPos == localEnd) ? localStart : localEnd;
        }
    }
}