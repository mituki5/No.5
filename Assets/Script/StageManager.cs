using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager : MonoBehaviour
{
    public Camera mainCamera;

    [Header("ステージ設定（同じ順序で）")]
    public Transform[] cameraPositions; // ステージごとにカメラ位置（Transform）
    public GameObject[] players;        // ステージごとのプレイヤー（Player GameObject）
    public Transform[] housePositions;  // ステージごとの家（スタート／ゴール）位置
    public ItemController[] items;      // ステージごとのアイテム（null可）

    [Header("その他")]
    public string clearSceneName = "ClearScene";

    private int currentStage = 0;

    void Awake()
    {
        if (mainCamera == null) mainCamera = Camera.main;
        // 全プレイヤーを非アクティブにしてから currentStage のみ有効化
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i] != null) players[i].SetActive(false);
        }
    }

    void Start()
    {
        // 最初のステージ開始
        StartStage(0);
    }

    /// <summary>
    /// 指定ステージを開始（カメラを移動、対応するPlayerを有効化）
    /// </summary>
    public void StartStage(int stageIndex)
    {
        if (stageIndex < 0 || stageIndex >= players.Length)
        {
            Debug.LogError("StartStage: index out of range");
            return;
        }

        // 既存のプレイヤー無効化
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i] != null) players[i].SetActive(false);
        }

        currentStage = stageIndex;

        // カメラ位置を瞬間移動（固定カメラでOK）
        if (cameraPositions != null && cameraPositions.Length > currentStage && cameraPositions[currentStage] != null)
        {
            mainCamera.transform.position = cameraPositions[currentStage].position;
            // カメラのZ位置は保つ（cameraPositions の Z を使う場合は注意）
        }

        // プレイヤー有効化して初期化
        if (players[currentStage] != null)
        {
            players[currentStage].SetActive(true);
            // PlayerController があればステージインデックスを設定
            PlayerController pc = players[currentStage].GetComponent<PlayerController>();
            if (pc != null)
            {
                pc.Initialize(this, currentStage);
                // プレイヤー位置は家の位置にセットしておく
                if (housePositions != null && housePositions.Length > currentStage && housePositions[currentStage] != null)
                {
                    players[currentStage].transform.position = housePositions[currentStage].position;
                    pc.SetRespawnToHouse(); // set default respawn
                }
            }
        }
    }

    /// <summary>
    /// プレイヤーがゴールに到達した（Item 持ち） -> 次のステージへ
    /// </summary>
    public void OnPlayerReachedGoal(int stageIndex)
    {
        if (stageIndex != currentStage)
        {
            Debug.LogWarning("OnPlayerReachedGoal: stage mismatch");
            return;
        }

        // 次ステージへ
        int next = currentStage + 1;
        if (next < players.Length)
        {
            StartStage(next);
        }
        else
        {
            // 全ステージクリア
            if (!string.IsNullOrEmpty(clearSceneName))
                SceneManager.LoadScene(clearSceneName);
        }
    }

    /// <summary>
    /// プレイヤーが死亡した。復活位置を返す（アイテム取得済みならアイテムの元位置、未取得なら家）
    /// 呼び出し元は PlayerController
    /// </summary>
    public Vector3 GetRespawnPositionForStage(int stageIndex)
    {
        // 優先順位: そのステージのアイテムがあり、かつアイテム取得済み -> アイテムの元位置
        if (items != null && stageIndex < items.Length && items[stageIndex] != null)
        {
            if (items[stageIndex].IsCollected)
            {
                return items[stageIndex].OriginalPosition;
            }
        }

        // それ以外は家（スタート）位置
        if (housePositions != null && stageIndex < housePositions.Length && housePositions[stageIndex] != null)
            return housePositions[stageIndex].position;

        // フォールバック: カメラ前に戻す
        if (cameraPositions != null && stageIndex < cameraPositions.Length && cameraPositions[stageIndex] != null)
            return cameraPositions[stageIndex].position;

        return Vector3.zero;
    }

    /// <summary>
    /// アイテムが取得されたら StageManager 側でフラグ更新（ItemController から呼ばれる）
    /// </summary>
    public void NotifyItemCollected(int stageIndex)
    {
        // items[stageIndex] の IsCollected は ItemController 側で true になっている想定
        // ここで追加の処理が必要なら実行（例：UI更新）
    }
}