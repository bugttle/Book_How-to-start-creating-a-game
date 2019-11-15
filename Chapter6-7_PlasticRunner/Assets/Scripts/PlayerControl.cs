using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public static float ACCELERATION = 10.0f;           // 加速度
    public static float SPEED_MIN = 4.0f;               // 速度の最小値
    public static float SPEED_MAX = 8.0f;               // 速度の最大値
    public static float JUMP_HEIGHT_MAX = 3.0f;         // ジャンプの高さ
    public static float JUMP_KEY_RELEASE_REDUCE = 0.5f; // ジャンプからの減速値
    public static float NARAKU_HEIGHT = -5.0f;

    public enum STEP // Playerの各種状態を表すデータ型
    {
        NONE = -1, // 状態情報なし
        RUN = 0,   // 走る
        JUMP,      // ジャンプ
        MISS,      // ミス
        NUM,       // 状態が何種類あるかを示す (=3)

    }

    public STEP step = STEP.NONE;      // Playerの現在の状態
    public STEP next_step = STEP.NONE; // Playerの次の状態

    public float step_timer = 0.0f;       // 経過時間
    private bool is_landed = false;       // 着地しているかどうか
    private bool is_colllided = false;    // 何かとぶつかっているかどうか
    private bool is_key_released = false; // ボタンが離されているかどうか
    public float current_speed = 0.0f; // 現在のスピード
    public LevelControl level_control = null; // LevelControlを保持
    private float click_timer = -1.0f; // ボタンが押されてからの時間
    private float CLICK_GRACE_TIME = 0.5f; // 「ジャンプしたい意志」を受け付ける時間

    void Start()
    {
        this.next_step = STEP.RUN;
    }

    void Update()
    {
        Rigidbody rigidbody = this.transform.GetComponent<Rigidbody>(); // 新しい Unity のバージョンでは、 this.rigidbody は存在しない
        Vector3 velocity = rigidbody.velocity; // 速度を設定
        this.current_speed = this.level_control.getPlayerSpeed();
        this.check_landed(); // 着地状態かどうかをチェック

        switch (this.step)
        {
            case STEP.RUN:
            case STEP.JUMP:
                // 現在の位置がしきい値よりも下ならば
                if (this.transform.position.y < NARAKU_HEIGHT)
                {
                    this.next_step = STEP.MISS; // 「ミス」状態にする
                }
                break;
        }

        this.step_timer += Time.deltaTime; // 経過時間を進める

        if (Input.GetMouseButtonDown(0)) // ボタンが押されたら
        {
            this.click_timer = 0.0f; // タイマーをリセット
        }
        else
        {
            if (this.click_timer >= 0.0f) // そうでなければ
            {
                this.click_timer += Time.deltaTime; // 経過時間を加算
            }
        }

        // 「次の状態」が決まっていなければ、状態の変化を調べる
        if (this.next_step == STEP.NONE)
        {
            switch (this.step) // Playerの現在の状態で分岐
            {
                case STEP.RUN: // 走行中の場合
                    // click_timerが0以上、CLICK_GRACE_TIME以下ならば
                    if (0.0f <= this.click_timer && this.click_timer <= CLICK_GRACE_TIME)
                    {
                        if (this.is_landed) // 着地しているならば
                        {
                            this.click_timer = -1.0f; // 「ボタンが押されてない」ことを表す -1.0fに
                            this.next_step = STEP.JUMP; // ジャンプ状態に

                        }
                    }
                    if (!this.is_landed)
                    {
                        // 走行中で、着地していない場合、何もしない
                    }
                    else
                    {
                        if (Input.GetMouseButtonDown(0))
                        {
                            // 走行中で、着地していて、左ボタンが押されていたら
                            // 次の状態をジャンプに変更
                            this.next_step = STEP.JUMP;
                        }
                    }
                    break;
                case STEP.JUMP: // ジャンプ中の場合
                    if (this.is_landed)
                    {
                        // ジャンプ中で、着地していたら、次の状態を走行中に変更
                        this.next_step = STEP.RUN;
                    }
                    break;
            }
        }

        // 「次の状態」が「状態情報なし」以外の間
        while (this.next_step != STEP.NONE)
        {
            this.step = this.next_step; // 「現在の状態」を「次の状態」に更新
            this.next_step = STEP.NONE; // 「次の状態」を「状態なし」に変更
            switch (this.step) // 更新された「現在の状態」が
            {
                case STEP.JUMP: // 「ジャンプ」の場合
                    // ジャンプの高さからジャンプの初速を計算（オマジナイ）
                    velocity.y = Mathf.Sqrt(2.0f * 9.8f * PlayerControl.JUMP_HEIGHT_MAX);
                    // 「ボタンが離されたフラグ」をクリアする
                    this.is_key_released = false;
                    break;
            }
            this.step_timer = 0.0f; // 状態が変化したので、経過時間をゼロにリセット
        }

        switch (this.step)
        {
            case STEP.RUN: // 走行中の場合
                // 速度を上げる
                velocity.x += PlayerControl.ACCELERATION * Time.deltaTime;

                // 計算で求めたスピードが、設定すべきスピードを超えていたら
                if (Mathf.Abs(velocity.x) > this.current_speed)
                {
                    // 超えないように調整する
                    velocity.x *= this.current_speed / Mathf.Abs(velocity.x);
                }

                break;
            case STEP.JUMP: // ジャンプ中の場合
                do
                {
                    // 「ボタンが離された瞬間」じゃなかったら
                    if (!Input.GetMouseButtonUp(0))
                    {
                        break; // 何もせずにループを抜ける
                    }
                    // 減速済みなら（2回以上減速しないように）
                    if (this.is_key_released)
                    {
                        break; // 何もせずにループを抜ける
                    }
                    // 上下方向の速度が0以下なら（下降中なら）
                    if (velocity.y <= 0.0f)
                    {
                        break; // 何もせずにループを抜ける
                    }
                    // ボタンが離されていて、上昇中なら、減速開始
                    // ジャンプの上昇はここでおしまい
                    velocity.y *= JUMP_KEY_RELEASE_REDUCE;

                    this.is_key_released = true;
                } while (false);
                break;
            case STEP.MISS:
                // 加速値（ACCELERATION）を引き算して、Playerの速度を遅くしていく
                velocity.x -= PlayerControl.ACCELERATION * Time.deltaTime;
                if (velocity.x < 0.0f)  // Playerの速度が負の数なら
                {
                    velocity.x = 0.0f; // ゼロにする
                }
                break;
        }
        // Rigidbodyの速度を、上記で求めた速度で更新
        // （この行は、状態にかかわらず毎回実行される）
        rigidbody.velocity = velocity;
    }

    private void check_landed()
    {
        this.is_landed = false; // とりあえずfalseにしておく

        do
        {
            Vector3 s = this.transform.position; // Playerの現在の位置
            Vector3 e = s + Vector3.down * 1.0f; // sから下に1.0fに移動した位置

            RaycastHit hit;
            if (!Physics.Linecast(s, e, out hit)) // sからeの間に何もない場合
            {
                break; // 何もせずdow～hileループを抜ける（脱出口へ）
            }

            // sからeの間に何かがあった場合、以下の処理が行われる
            if (this.step == STEP.JUMP) // 現在、ジャンプ状態ならば
            {
                // 経過時間が3.0f未満ならば
                if (this.step_timer < Time.deltaTime * 3.0f)
                {
                    break; // 何もせずdow～hileループを抜ける（脱出口へ）
                }
            }
            // sからeの間に何かがあり、JUMP直後でない場合のみ、以下が実行される
            this.is_landed = true;
        } while (false);
        // ループの脱出口
    }
}
