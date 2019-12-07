using System.Collections.Generic;
using UnityEngine;

public class LevelData
{
    public float[] probability; // ブロックの出現頻度を収める配列
    public float heat_time; // 燃焼時間

    public LevelData() // コンストラクタ
    {
        // ブロックの種類数と同じサイズでデータ領域を確保
        this.probability = new float[(int)Block.COLOR.NORMAL_COLOR_NUM];

        // 全種類の出現確率を、とりあえず均等にしておく
        for (int i = 0; i < (int)Block.COLOR.NORMAL_COLOR_NUM; i++)
        {
            this.probability[i] = 1.0f / (float)Block.COLOR.NORMAL_COLOR_NUM;
        }
    }

    // 全種類の出現確率をリセットするメソッド
    public void clear()
    {
        for (int i = 0; i < this.probability.Length; i++)
        {
            this.probability[i] = 0.0f;
        }
    }

    // 全種類の出現確率を100%(=1.0)にするメソッド
    public void normalize()
    {
        float sum = 0.0f;

        // 出現確率の「仮の合計数」を計算する
        for (int i = 0; i < this.probability.Length; i++)
        {
            sum += this.probability[i];
        }

        for (int i = 0; i < this.probability.Length; i++)
        {
            // それぞれの出現確率を「仮の合計値」で割ると
            // 合計値が100%(=1.0)ぴったりに
            this.probability[i] /= sum;

            // もしその値が無限大なら
            if (float.IsInfinity(this.probability[i]))
            {
                this.clear(); // 全ての確率を0にリセットして
                this.probability[0] = 1.0f; // 最初の要素のみ1.0にしておく
                break; // そして、ループを抜ける
            }
        }
    }
}

public class LevelControl
{
    private List<LevelData> level_datas = null; // 各レベルのレベルデータ
    private int select_level = 0; // 選択されたレベル

    public void initialize()
    {
        // Listを初期化
        this.level_datas = new List<LevelData>();
    }

    public void loadLevelData(TextAsset level_data_text)
    {
        // テキストデータを、文字列として取り込む
        string level_texts = level_data_text.text;

        // 改行コード'\'ごとに分割し、文字列の配列に入れる
        string[] lines = level_texts.Split('\n');

        // lines内の各行に対して、順番に処理していくループ
        foreach (var line in lines)
        {
            if (line == "") // 行が空っぽなら
            {
                continue; // 以下の処理はせずにループの先頭にジャンプ
            }
            string[] words = line.Split(); // 行内のワードを配列に格納
            int n = 0;

            // LevelData型の変数を作成
            // ここに、現在処理している行のデータを入れていく
            LevelData level_data = new LevelData();

            // words内の各ワードに対して、順番に処理していくループ
            foreach (var word in words)
            {
                if (word.StartsWith("#")) // ワードの先頭文字が#なら
                {
                    break; // ループを脱出
                }
                if (word == "") //ワードが空っぽなら
                {
                    continue; // ループの先頭にジャンプ
                }
                //
                //
                switch (n)
                {
                    case 0:
                        level_data.probability[(int)Block.COLOR.PINK] = float.Parse(word);
                        break;
                    case 1:
                        level_data.probability[(int)Block.COLOR.BLUE] = float.Parse(word);
                        break;
                    case 2:
                        level_data.probability[(int)Block.COLOR.GREEN] = float.Parse(word);
                        break;
                    case 3:
                        level_data.probability[(int)Block.COLOR.ORANGE] = float.Parse(word);
                        break;
                    case 4:
                        level_data.probability[(int)Block.COLOR.YELLOW] = float.Parse(word);
                        break;
                    case 5:
                        level_data.probability[(int)Block.COLOR.MAGENTA] = float.Parse(word);
                        break;
                    case 6:
                        level_data.heat_time = float.Parse(word);
                        break;
                }
                n++;
            }

            if (n >= 7) // 8項目（以上）がきちんと処理されたなら
            {
                // 出現確率の合計がきちんと100%になるようにしてから
                level_data.normalize();
                // List構造のlevel_datasにlevel_dataを追加
                this.level_datas.Add(level_data);
            }
            else // そうでないなら（エラーの可能性あり）
            {
                if (n == 0) // 1ワードも処理していない場合はコメントなので
                {
                    // 問題なし
                }
                else // それ以外ならエラー
                {
                    // データの個数が合っていないことを示すエラーメッセージを表示
                    Debug.LogError("[LevelData] Out of parameter.\n");
                }
            }
        }
        // level_datasにデータがひとつもないならば
        if (this.level_datas.Count == 0)
        {
            // エラーメッセージを表示
            Debug.LogError("[LevelData] Has no data.\n");
            // level_datasにLeelDataを1つ追加しておく
            this.level_datas.Add(new LevelData());
        }
    }

    public void selectLevel()
    {
        // 0～パターンの間の値をランダムに選択
        this.select_level = Random.Range(0, this.level_datas.Count);
        Debug.Log("select level = " + this.select_level.ToString());
    }

    public LevelData getCurrentLevelData()
    {
        // 選択されているパターンのレベルデータを返す
        return (this.level_datas[this.select_level]);
    }

    public float getVanishTime()
    {
        // 選択されているパターンの燃焼時間を返す
        return (this.level_datas[this.select_level].heat_time);
    }
}
