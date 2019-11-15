using System.Collections.Generic;
using UnityEngine;

public class LevelData
{
    public struct Range // 範囲を表す構造体
    {
        public int min; // 範囲の最小値
        public int max; // 範囲の最大値
    }

    public float end_time; // 終了時間
    public float player_speed; // プレイヤーの速度

    public Range floor_count; // 足場ブロック数の範囲
    public Range hole_count; // 穴の個数の範囲
    public Range height_diff; // 足場の高さの範囲

    public LevelData()
    {
        this.end_time = 15.0f; // 終了時間を初期化
        this.player_speed = 6.0f; // プレイヤーの速度を初期化
        this.floor_count.max = 10; // 足場ブロック数の最小値を初期化
        this.floor_count.max = 10; // 足場ブロック数の最大値を初期化
        this.hole_count.min = 2; // 穴の個数の最小値を初期化
        this.hole_count.max = 6; // 穴の個数の最大値を初期化
        this.height_diff.min = 0; // 足場の高さ変化の最小値を初期化
        this.height_diff.max = 0; // 足場の高さ変化の最大値を初期化
    }
}

public class LevelControl
{
    private List<LevelData> level_datas = new List<LevelData>();

    public int HEIGHT_MAX = 20;
    public int HEIGHT_MIN = -4;

    public void loadLevelData(TextAsset level_data_text)
    {
        // テキストデータを、文字列として取り込む
        string level_texts = level_data_text.text;

        // 改行コード'\n'ごとに分割し、文字列の配列に入れる
        string[] lines = level_texts.Split('\n');

        // lines内の各行に対して、順番に処理していくループ
        foreach (var line in lines)
        {
            if (line == "") // 行が空っぽなら
            {
                continue; // 以下の処理はせずにループの先頭にジャンプ
            }
            Debug.Log(line); // 行の内容をデバッグ出力
            string[] words = line.Split(); // 行内のワードを配列に格納
            int n = 0;

            // LevelData型の変数を作成
            // ここに、現在処理している行のデータを入れていく
            LevelData level_data = new LevelData();

            // words内の各ワードに対して、順番に処理していくループ
            foreach (var word in words)
            {
                if (word.StartsWith("#"))  // ワードの先頭文字が#なら
                {
                    break; //ループを脱出
                }
                if (word == "") // ワードが空っぽなら
                {
                    continue; // ループの先頭にジャンプ
                }

                // 「n」の値を0, 1, 2, ...7と変化させていくことで、8項目を処理
                // 各ワードをfloat値に変化し、level_dataに格納する
                switch (n)
                {
                    case 0:
                        level_data.end_time = float.Parse(word);
                        break;
                    case 1:
                        level_data.player_speed = float.Parse(word);
                        break;
                    case 2:
                        level_data.floor_count.min = int.Parse(word);
                        break;
                    case 3:
                        level_data.floor_count.max = int.Parse(word);
                        break;
                    case 4:
                        level_data.hole_count.min = int.Parse(word);
                        break;
                    case 5:
                        level_data.hole_count.max = int.Parse(word);
                        break;
                    case 6:
                        level_data.height_diff.min = int.Parse(word);
                        break;
                    case 7:
                        level_data.height_diff.max = int.Parse(word);
                        break;
                }
                n++;
            }

            if (n >= 8) // 8項目（以上）がきちんと処理されたなら
            {
                // List構造のlevel_datasにlevel_dataを追加
                this.level_datas.Add(level_data);
            }
            else // そうでないなら（エラーの可能性あり）
            {
                if (n == 0) // 1ワードも処理していない場合はコメントなので、
                {
                    // 問題なし。何もしない
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
            // level_datasに、デフォルトのLevelDataを1つ追加しておく
            this.level_datas.Add(new LevelData());
        }
    }

    public struct CreationInfo
    {
        public Block.TYPE block_type; // ブロックの種類
        public int max_count;         //ブロックの最大個数
        public int height;            //ブロックを配置する高さ
        public int current_count;     // 作成したブロックの個数
    }

    public CreationInfo previous_block; // 前回、どういうブロックを作ったか
    public CreationInfo current_block;  // 今回、どういうブロックを作るべきか
    public CreationInfo next_block;     // 次回、どういうブロックを作るべきか

    public int block_count = 0; // 作成したブロックの総数
    public int level = 0;       // 難易度

    private void clear_next_block(ref CreationInfo block)
    {
        // 受け取ったブロック(block)の中身を初期化
        block.block_type = Block.TYPE.FLOOR;
        block.max_count = 15;
        block.height = 0;
        block.current_count = 0;
    }

    public void initialize()
    {
        this.block_count = 0; // ブロックの総数をゼロに

        // 前回、今回、次回のブロックのそれぞれを
        // clear_next_block()に渡して初期化してもらう
        this.clear_next_block(ref this.previous_block);
        this.clear_next_block(ref this.current_block);
        this.clear_next_block(ref this.next_block);
    }

    private void update_level(ref CreationInfo current, CreationInfo previous, float passage_time) // 新設の引数passage_timeで、プレイの経過時間を受け取る
    {
        // 「レベル1～レベル5」の繰り返しに
        float local_time = Mathf.Repeat(passage_time, this.level_datas[this.level_datas.Count - 1].end_time);

        // 現在のレベルを求める
        int i;
        for (i = 0; i < this.level_datas.Count - 1; i++)
        {
            if (local_time <= this.level_datas[i].end_time)
            {
                break;
            }
        }
        this.level = i;

        current.block_type = Block.TYPE.FLOOR;
        current.max_count = 1;

        if (this.block_count >= 10)
        {
            // 現在のレベル用のレベルデータを取得
            LevelData level_data;
            level_data = this.level_datas[this.level];

            switch (previous.block_type)
            {
                case Block.TYPE.FLOOR:                    // 前回のブロックが床の場合
                    current.block_type = Block.TYPE.HOLE; // 今回は穴を作る

                    // 穴の長さの最小値～最大値の間の、ランダムな値
                    current.max_count = Random.Range(level_data.hole_count.min, level_data.hole_count.max);

                    current.height = previous.height; // 高さを前回と同じにする
                    break;

                case Block.TYPE.HOLE:                      // 前回のブロックが穴の場合
                    current.block_type = Block.TYPE.FLOOR; // 今回は床を作る

                    // 床の長さの最小値～最大値の間の、ランダムな値
                    current.max_count = Random.Range(level_data.floor_count.min, level_data.floor_count.max);

                    // 床の高さの最小値と最大値を求める
                    int height_min = previous.height + level_data.height_diff.min;
                    int height_max = previous.height + level_data.height_diff.max;
                    height_min = Mathf.Clamp(height_min, HEIGHT_MIN, HEIGHT_MAX);
                    height_max = Mathf.Clamp(height_max, HEIGHT_MIN, HEIGHT_MAX);

                    // 床の高さの最小値～最大値の間の、ランダムな値
                    current.height = Random.Range(height_min, height_max);
                    break;
            }
        }
    }

    //     public void update()
    public void update(float passage_time)
    {
        // 「今回作ったブロックの個数」をインクリメント
        this.current_block.current_count++;

        // 「今回作ったブロックの個数」が予定数(max_count)以上なら
        if (this.current_block.current_count >= this.current_block.max_count)
        {
            this.previous_block = this.current_block;
            this.current_block = this.next_block;

            // 次に作るべきブロックの内容を初期化
            this.clear_next_block(ref this.next_block);
            // 次に作るべきブロックを設定
            // this.update_level(ref this.next_block, this.current_block);
            this.update_level(ref this.next_block, this.current_block, passage_time);
        }
        this.block_count++; // 「ブロックの総数」をインクリメント
    }

    public float getPlayerSpeed()
    {
        return (this.level_datas[this.level].player_speed);
    }
}
