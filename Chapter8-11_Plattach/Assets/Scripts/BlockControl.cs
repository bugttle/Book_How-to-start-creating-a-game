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

    public static int BLOCK_NUM_X = 9; // ブロックを配置できるX方向の最大数
    public static int BLOCK_NUM_Y = 9; // ブロックを配置できるY方向の最大数
}

public class BlockControl : MonoBehaviour
{
    public Block.COLOR color = (Block.COLOR)0; // ブロックの色
    public BlockRoot block_root = null; // ブロックの神様
    public Block.iPosition i_pos; // ブロックの座標

    void Start()
    {
        this.setColor(this.color); // 色塗りを行う
    }

    void Update()
    {
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
}
