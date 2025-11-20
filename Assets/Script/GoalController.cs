using UnityEngine;

public class GoalController : MonoBehaviour
{
    public int stageIndex = 0;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        PlayerController pc = collision.GetComponent<PlayerController>();
        if (pc == null) return;

        // アイテムを持っていなければゴール不可
        if (!pc.hasItem) return;

        // StageManager にゴール到達を通知
        StageManager sm = FindObjectOfType<StageManager>();
        if (sm != null)
        {
            sm.OnPlayerReachedGoal(stageIndex);
        }
    }
}