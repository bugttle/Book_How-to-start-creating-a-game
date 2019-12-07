using UnityEngine;

public class ScoreCounter : MonoBehaviour
{
    public struct Count // スコア管理用の構造体
    {
        public int ignite; // 着火数
        public int score; // スコア
        public int total_score; // 合計スコア
    }

    public Count last; // 最後（今回）のスコア
    public Count best; // ベストスコア

    public static int QUOTA_SCORE = 1000; // クリアに必要なスコア
    public GUIStyle guistyle; // フォントスタイル

    void Start()
    {
        this.last.ignite = 0;
        this.last.score = 0;
        this.last.total_score = 0;
        this.guistyle.fontSize = 16;
    }

    void OnGUI()
    {
        int x = 20;
        int y = 50;
        GUI.color = Color.black;
        this.print_value(x + 20, y, "着火ウント", this.last.ignite);
        y += 30;
        this.print_value(x + 20, y, "加算スコア", this.last.score);
        y += 30;
        this.print_value(x + 20, y, "合計スコア", this.last.total_score);
        y += 30;
    }

    public void print_value(int x, int y, string label, int value)
    {
        // labelを表示
        GUI.Label(new Rect(x, y, 100, 20), label, guistyle);
        y += 15;
        // 次の行にvalueを表示
        GUI.Label(new Rect(x + 20, y, 100, 20), value.ToString(), guistyle);
        y += 15;
    }

    public void addIgniteCount(int count)
    {
        this.last.ignite += count; // 着火数にcountを加算
        this.update_score(); // スコアを計算
    }

    public void clearIgniteCount()
    {
        this.last.ignite = 0; // 着火回数をリセット
    }

    public void update_score()
    {
        this.last.score = this.last.ignite * 10; // スコアを更新
    }

    public void updateToScore()
    {
        this.last.total_score += this.last.score; // 合計スコアを更新
    }

    public bool isGameClear()
    {
        bool is_clear = false;
        // 現在の合計スコアがクリア基準よりも大きいなら
        if (this.last.total_score > QUOTA_SCORE)
        {
            is_clear = true;
        }
        return (is_clear);
    }
}
