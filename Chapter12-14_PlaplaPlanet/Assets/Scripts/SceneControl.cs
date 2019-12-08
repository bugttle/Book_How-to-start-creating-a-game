using UnityEngine;

public class SceneControl : MonoBehaviour
{
    private GameStatus game_status = null;
    private PlayerControl player_control = null;
    private float GAME_OVER_TIME = 60.0f; // 制限時間は60秒

    public enum STEP // ゲームステータス
    {
        NONE = -1, // ステータスなし
        PLAY = 0, // プレイ中
        CLEAR, // クリア状態
        GAMEOVER, // ゲームオーバー状態
        NUM, // ステータスが何種類あるかを示す（=3）
    }

    public STEP step = STEP.NONE; // 現在のステップ
    public STEP next_step = STEP.NONE; // 次のステップ
    public float step_timer = 0.0f; // タイマー
    private float clear_time = 0.0f; // クリア時間
    public GUIStyle guistyle; // フォントスタイル

    void Start()
    {
        this.game_status = this.gameObject.GetComponent<GameStatus>();
        this.player_control = GameObject.Find("Player").GetComponent<PlayerControl>();
        this.step = STEP.PLAY;
        this.next_step = STEP.PLAY;
        this.guistyle.fontSize = 64;
    }

    void Update()
    {
        this.step_timer += Time.deltaTime;
        if (this.next_step == STEP.NONE)
        {
            switch (this.step)
            {
                case STEP.PLAY:
                    if (this.game_status.isGameClear())
                    {
                        // クリア状態に移行
                        this.next_step = STEP.CLEAR;
                    }
                    if (this.game_status.isGameOver())
                    {
                        // ゲームオーバー状態に移行
                        this.next_step = STEP.GAMEOVER;
                    }
                    if (this.step_timer > GAME_OVER_TIME)
                    {
                        // 制限時間を超えていたらゲームオーバー
                        this.next_step = STEP.GAMEOVER;
                    }
                    break;

                // クリア時およびゲームオーバー時の処理
                case STEP.CLEAR:
                case STEP.GAMEOVER:
                    if (Input.GetMouseButtonDown(0))
                    {
                        // マウスボタンが押されたらGameSceneを再読み込み
                        Application.LoadLevel("GameScene");
                    }
                    break;
            }
        }

        while (this.next_step != STEP.NONE)
        {
            this.step = this.next_step;
            this.next_step = STEP.NONE;
            switch (this.step)
            {
                case STEP.CLEAR:
                    // PlayerControlを制御不可に
                    this.player_control.enabled = false;
                    //
                    this.clear_time = this.step_timer;
                    break;
                case STEP.GAMEOVER:
                    // PlayerControlを制御不可に
                    this.player_control.enabled = false;
                    break;
            }
            this.step_timer = 0.0f;
        }
    }

    void OnGUI()
    {
        float pos_x = Screen.width * 0.1f;
        float pos_y = Screen.height * 0.5f;
        switch (this.step)
        {
            case STEP.PLAY:
                GUI.color = Color.black;
                GUI.Label(new Rect(pos_x, pos_y, 200, 20), this.step_timer.ToString("0.00"), guistyle); // 経過時間を表示

                // 制限時間に達するまでの残り時間を表示
                float blast_time = GAME_OVER_TIME - this.step_timer;
                GUI.Label(new Rect(pos_x, pos_y + 64, 200, 20), blast_time.ToString("0.00"));
                break;
            case STEP.CLEAR:
                GUI.color = Color.black;
                // クリアメッセージとクリア時間を表示
                GUI.Label(new Rect(pos_x, pos_y, 200, 20), "脱出" + this.clear_time.ToString("0.00"), guistyle); // 経過時間を表示
                pos_y -= 32;
                int ct = (int)clear_time; // クリア時間（float）をintに変換
                if (ct > 50) // 50秒～制限時間内
                {
                    GUI.Label(new Rect(pos_y, pos_y, 200, 20), "ぎりぎり脱出！ 50秒以内を目指そう！");
                }
                else if (ct > 40) // 40秒～50秒
                {
                    GUI.Label(new Rect(pos_y, pos_y, 200, 20), "素敵！40秒以内を目指そう！");
                }
                else if (ct > 30) // 30秒～40秒
                {
                    GUI.Label(new Rect(pos_y, pos_y, 200, 20), "凄い！30秒以内を目指そう！");
                }
                else // 30秒以内！
                {
                    GUI.Label(new Rect(pos_y, pos_y, 200, 20), "早い！ぷらぷらマスター！");
                }
                break;
            case STEP.GAMEOVER:
                GUI.color = Color.black;
                // ゲームオーバーメッセージを表示
                GUI.Label(new Rect(pos_x, pos_y, 200, 20), "ゲームオーバー", guistyle); // 経過時間を表示
                break;
        }
    }
}
