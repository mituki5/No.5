using UnityEngine;

/// <summary>
/// 動く床にアタッチ。プレイヤーが床に乗ったら親子にして一緒に動かす。
/// </summary>
public class PlatformPassenger : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            collision.collider.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            collision.collider.transform.SetParent(null);
        }
    }
}
