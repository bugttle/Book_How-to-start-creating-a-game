using UnityEngine;

public class Block
{
    public static float COLLISION_SIZE = 1.0f; // ブロックのアタリのサイズ
    public static float VANISH_TIME = 3.0f; // 着火して消えるまでの時間

    public struct iPosition // グリッドでの座標を表す構造体
    {
        public int x; // X座標
        public int y; // Y座標
    }

    public enum COLOR // ブロックのカラー
    {
        NONE = -1, // 色指定なし
        PINK = 0, // 桃色
        BLUE, // 青
        YELLOW, // 黄
        GREEN, // 緑
        MAGENTA, // マゼンタ
        ORANGE, // オレンジ
        GRAY, // グレー
        NUM, // カラーが何種類あるかを示す
        FIRST = PINK, // 初期カラー（桃色）
        LAST = ORANGE, // 最終カラー（オレンジ）
        NORMAL_COLOR_NUM = GRAY, // 通常カラー（グレー以外の色）の数
    }

    public enum DIR4 // 上下左右の４方向
    {
        NONE = -1, // 方向指定なし
        RIGHT, // 右
        LEFT, // 左
        UP, // 上
        DOWN, // 下
        NUM, // 方向が何種類あるかを示す (=4)
    }

    public enum STEP // ブロックの状態を表す
    {
        NONE = -1, // 状態情報なし
        IDLE = 0, // 待機中
        GRABBED, // つかまれている
        RELEASED, // 離された瞬間
        SLIDE, // スライドしている
        VACANT, // 消滅中
        RESPAWN, // 再生成中
        FALL, // 落下中
        LONG_SLIDE, // 大きくスライドしている
        NUM, // 状態が何種類あるかを示す（=8）
    }

    public static int BLOCK_NUM_X = 9; // ブロックを配置できるX方向の最大数
    public static int BLOCK_NUM_Y = 9; // ブロックを配置できるY方向の最大数
}

public class BlockControl : MonoBehaviour
{
    public Block.COLOR color = (Block.COLOR)0; // ブロックの色
    public BlockRoot block_root = null; // ブロックの神様
    public Block.iPosition i_pos; // ブロックの座標
    public Block.STEP step = Block.STEP.NONE; // 今の状態
    public Block.STEP next_step = Block.STEP.NONE; // 次の状態
    private Vector3 position_offset_initial = Vector3.zero; // 入れ替え前の位置
    private Vector3 position_offset = Vector3.zero; // 入れ替え後の位置　
    public float vanish_timer = -1.0f; // ブロックが消えるまでの時間
    public Block.DIR4 slide_dir = Block.DIR4.NONE; // スライドされた方向
    public float step_timer = 0.0f; // ブロックが入れ替わったときの移動時間など

    void Start()
    {
        this.setColor(this.color); // 色塗りを行う
        this.next_step = Block.STEP.IDLE; // 次のブロックを待機中に
    }

    void Update()
    {
        Vector3 mouse_position; // マウスの位置
        this.block_root.unprojectMousePosition(out mouse_position, Input.mousePosition); // マウスの位置を取得

        // 取得したマウス位置をXとYだけにする
        Vector2 mouse_position_xy = new Vector2(mouse_position.x, mouse_position.y);

        this.step_timer += Time.deltaTime;
        float slide_time = 0.2f;

        if (this.next_step == Block.STEP.NONE) // 「状態情報なし」の場合
        {
            switch (this.step)
            {
                case Block.STEP.SLIDE:
                    if (this.step_timer >= slide_time)
                    {
                        // スライド中にブロックが消滅したら、VACANT(消える)状態に移行
                        if (this.vanish_timer == 0.0f)
                        {
                            this.next_step = Block.STEP.VACANT;
                        }
                        else // vanish_timerが0でないなら、IDLE(待機)状態に移行
                        {
                            this.next_step = Block.STEP.IDLE;
                        }
                    }
                    break;
            }
        }

        // 「次のブロック」の状態が「情報なし」以外である間
        // ＝「次のブロック」の状態が変更されていた場合
        while (this.next_step != Block.STEP.NONE)
        {
            this.step = this.next_step;
            this.next_step = Block.STEP.NONE;

            switch (this.step)
            {
                case Block.STEP.IDLE: // 「待機」状態
                    this.position_offset = Vector3.zero;
                    // ブロックの表示サイズを通常サイズにする
                    this.transform.localScale = Vector3.one * 1.0f;
                    break;
                case Block.STEP.GRABBED: // 「つかまれている」状態
                    // ブロックの表示サイズを大きくする
                    this.transform.localScale = Vector3.one * 1.2f;
                    break;
                case Block.STEP.RELEASED: // 「離されている」状態
                    this.position_offset = Vector3.zero;
                    // ブロックの表示サイズを通常サイズにする
                    this.transform.localScale = Vector3.one * 1.0f;
                    break;
                case Block.STEP.VACANT:
                    this.position_offset = Vector3.zero;
                    break;
            }
            this.step_timer = 0.0f;
        }

        switch (this.step)
        {
            case Block.STEP.GRABBED: // 「つかまれた」状態
                // 「つかまれた」状態のときは、常にスライド方向をチェック
                this.slide_dir = this.calcSlideDir(mouse_position_xy);
                break;
            case Block.STEP.SLIDE: // スライド（入れ替え）中
                // ブロックを徐々に移動する処理（難しいので今は分からなくても大丈夫です）
                float rate = this.step_timer / slide_time;
                rate = Mathf.Min(rate, 1.0f);
                rate = Mathf.Sin(rate * Mathf.PI / 2.0f);
                this.position_offset = Vector3.Lerp(this.position_offset_initial, Vector3.zero, rate);
                break;
        }

        // グリッド座標を実座標（シーン上の座標）に変換し、position_offsetを加える
        Vector3 position = BlockRoot.calcBlockPosition(this.i_pos) + this.position_offset;

        // 実際の位置を、新しい位置に変更
        this.transform.position = position;
    }

    // 引数colorの色で、ブロックを塗る
    public void setColor(Block.COLOR color)
    {
        this.color = color; // 今回指定された色をメンバー変数に保管
        Color color_value; // Colorクラスは色を表す

        switch (this.color) // 塗るべき色によって分岐
        {
            default:
            case Block.COLOR.PINK:
                color_value = new Color(1.0f, 0.5f, 0.5f);
                break;
            case Block.COLOR.BLUE:
                color_value = Color.blue;
                break;
            case Block.COLOR.YELLOW:
                color_value = Color.yellow;
                break;
            case Block.COLOR.GREEN:
                color_value = Color.green;
                break;
            case Block.COLOR.MAGENTA:
                color_value = Color.magenta;
                break;
            case Block.COLOR.ORANGE:
                color_value = new Color(1.0f, 0.46f, 0.0f);
                break;
        }
        // このGameObjectのマテリアルカラーを変更
        gameObject.GetComponent<Renderer>().material.color = color_value;
    }

    public void beginGrab()
    {
        this.next_step = Block.STEP.GRABBED;
    }

    public void endGrab()
    {
        this.next_step = Block.STEP.IDLE;
    }

    public bool isGrabbable()
    {
        bool is_grabbable = false;
        switch (this.step)
        {
            case Block.STEP.IDLE: // 「待機」状態のときのみ
                is_grabbable = true;
                break;
        }

        return (is_grabbable);
    }

    public bool isContainedPosition(Vector2 position)
    {
        bool ret = false;
        Vector3 center = this.transform.position;
        float h = Block.COLLISION_SIZE / 2.0f;

        do
        {
            // X座標が自分に重なっていないなら、breakでループを抜ける
            if (position.x < center.x - h || center.x + h < position.x)
            {
                break;
            }
            // Y座標が自分に重なっていないなら、breakでループを抜ける
            if (position.y < center.y - h || center.y + h < position.y)
            {
                break;
            }
            // X座標、Y座標の両方が重なっていたら、true（重なっている）を返す
            ret = true;
        } while (false);

        return (ret);
    }

    public Block.DIR4 calcSlideDir(Vector2 mouse_position)
    {
        Block.DIR4 dir = Block.DIR4.NONE;
        // 指定されたmouse_positionと現在位置との差を示すベクトル
        Vector2 v = mouse_position - new Vector2(this.transform.position.x, this.transform.position.y);

        // ベクトルの大きさが0.1より大きいなら
        // （それより小さい場合は、スライドしていないと見なす）
        if (v.magnitude > 0.1f)
        {
            if (v.y > v.x)
            {
                if (v.y > -v.x)
                {
                    dir = Block.DIR4.UP;
                }
                else
                {
                    dir = Block.DIR4.LEFT;
                }
            }
            else
            {
                if (v.y > -v.x)
                {
                    dir = Block.DIR4.RIGHT;
                }
                else
                {
                    dir = Block.DIR4.DOWN;
                }
            }
        }
        return (dir);
    }

    public float calcDirOffset(Vector2 position, Block.DIR4 dir)
    {
        float offset = 0.0f;
        // 指定された位置と、ブロックの現在位置との差を表すベクトル　
        Vector2 v = position - new Vector2(this.transform.position.x, this.transform.position.y);
        switch (dir) // 指定された方向によって分岐
        {
            case Block.DIR4.RIGHT:
                offset = v.x;
                break;
            case Block.DIR4.LEFT:
                offset = -v.x;
                break;
            case Block.DIR4.UP:
                offset = v.y;
                break;
            case Block.DIR4.DOWN:
                offset = -v.y;
                break;
        }
        return (offset);
    }

    public void beginSlide(Vector3 offset)
    {
        this.position_offset_initial = offset;
        this.position_offset = this.position_offset_initial;
        // 状態をSLIDEに変更
        this.next_step = Block.STEP.SLIDE;
    }
}
