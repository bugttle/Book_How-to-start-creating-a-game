using UnityEngine;

public class SceneControl : MonoBehaviour
{
    private BlockRoot block_root = null;
    private ScoreCounter score_counter = null;

    public enum STEP
    {
        NONE = -1, // 状態情報なし
        PLAY = 0, // プレイ中
        CLEAR, // クリア
        NUM, // 状態が何種類あるかを示す（=2）
    }

    public STEP step = STEP.NONE; // 現在の状態
    public STEP next_step = STEP.NONE; // 次の状態
    public float step_timer = 0.0f; // 経過時間
    private float clear_time = 0.0f; // クリア時間
    public GUIStyle guistyle; // フォントスタイル

    void Start()
    {
        // BlockRootスクリプトを取得
        this.block_root = this.gameObject.GetComponent<BlockRoot>();
        this.block_root.create();
        // BlockRootスクリプトのinitialSetUp()を呼び出す
        this.block_root.initialSetUp();

        // ScoreCounterを取得
        this.score_counter = this.gameObject.GetComponent<ScoreCounter>();
        this.next_step = STEP.PLAY; // 次の状態を「プレイ中」に
        this.guistyle.fontSize = 24; // フォントのサイズを24に
    }

    void Update()
    {
        this.step_timer += Time.deltaTime;
        switch (this.step)
        {
            case STEP.CLEAR:
                if (Input.GetMouseButtonDown(0))
                {
                    Application.LoadLevel("TitleScene");
                }
                break;
        }

        // 状態変化待ち-----
        if (this.next_step == STEP.NONE)
        {
            switch (this.step)
            {
                case STEP.PLAY:
                    // クリア条件を満たしていたら
                    if (this.score_counter.isGameClear())
                    {
                        this.next_step = STEP.CLEAR; // クリア状態に移行
                    }
                    break;
            }
        }
        // 状態が変化したら------
        while (this.next_step != STEP.NONE)
        {
            this.step = this.next_step;
            this.next_step = STEP.NONE;
            switch (this.step)
            {
                case STEP.CLEAR:
                    // block_rootを停止
                    this.block_root.enabled = false;
                    // 経過時間をクリア時間として設定
                    this.clear_time = this.step_timer;
                    break;
            }
            this.step_timer = 0.0f;
        }
    }

    void OnGUI()
    {
        switch (this.step)
        {
            case STEP.PLAY:
                GUI.color = Color.black;
                // 経過時間を表示
                GUI.Label(new Rect(40.0f, 10.0f, 200.0f, 20.0f), "時間" + Mathf.CeilToInt(this.step_timer).ToString() + "秒", guistyle);
                GUI.color = Color.white;
                break;

            case STEP.CLEAR:
                GUI.color = Color.black;
                // 「☆クリアー！☆」という文字列を表示
                GUI.Label(new Rect(Screen.width / 2.0f - 80.0f, 20.0f, 200.0f, 20.0f), "☆クリアー！☆", guistyle);
                // クリア時間を表示
                GUI.Label(new Rect(Screen.width / 2.0f - 80.0f, 40.0f, 200.0f, 20.0f), "クリア時間" + Mathf.CeilToInt(this.clear_time).ToString() + "秒", guistyle);
                GUI.color = Color.white;
                break;
        }
    }
}
