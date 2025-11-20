using UnityEngine;

public class ItemController : MonoBehaviour
{
    public int stageIndex = 0; // Inspector でどのステージ用かセット
    [HideInInspector] public Vector3 OriginalPosition;
    [HideInInspector] public bool IsCollected = false;

    private void Awake()
    {
        OriginalPosition = transform.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsCollected) return;

        if (collision.CompareTag("Player"))
        {
            IsCollected = true;
            // プレイヤーに拾ったことを教える（PlayerController の OnPickedItem を呼ぶ）
            PlayerController pc = collision.GetComponent<PlayerController>();
            if (pc != null) pc.OnPickedItem();

            // StageManager に通知（必要なら参照を取って呼ぶ）
            StageManager sm = FindObjectOfType<StageManager>();
            if (sm != null)
            {
                sm.NotifyItemCollected(stageIndex);
            }

            // 非表示（永久取得）
            gameObject.SetActive(false);
        }
    }
}