using UnityEngine;

public class BlockRoot : MonoBehaviour
{
    public GameObject BlockPrefab = null; // 作り出すべきブロックのPrefab
    public BlockControl[,] blocks; // マス目（グリッド）

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    // ブロックを作り出して、横9ます、縦9マスに配置
    public void initialSetUp()
    {
        // マス目のサイズを9×9にする
        this.blocks = new BlockControl[Block.BLOCK_NUM_X, Block.BLOCK_NUM_Y];
        // ブロックの色番号
        int color_index = 0;

        for (int y = 0; y < Block.BLOCK_NUM_Y; y++) // 先頭行から最終行まで
        {
            for (int x = 0; x < Block.BLOCK_NUM_X; x++) // 左端から右端まで
            {
                // BlockPrefabのインスタンスをシーン上に作る
                GameObject game_object = Instantiate(this.BlockPrefab) as GameObject;
                // 上で作ったブロックのBlockControlクラスを取得
                BlockControl block = game_object.GetComponent<BlockControl>();
                // ブロックをマス目に格納
                this.blocks[x, y] = block;

                // ブロックの位置情報（グリッド座標）を設定
                block.i_pos.x = x;
                block.i_pos.y = y;
                // 各BlockControlが連携するGameRootは自分だと設定
                block.block_root = this;

                // グリッド座標を実際の位置（シーン上の座標）に変換
                Vector3 position = BlockRoot.calcBlockPosition(block.i_pos);
                // シーン上のブロックの位置を移動
                block.transform.position = position;
                // ブロックの色を変更
                block.setColor((Block.COLOR)color_index);
                // ブロックの名前を設定（後述）
                block.name = "block(" + block.i_pos.x.ToString() + "," + block.i_pos.y.ToString() + ")";

                // 全種類の色の中から、ランダムに1色を選択
                color_index = Random.Range(0, (int)Block.COLOR.NORMAL_COLOR_NUM);
            }
        }
    }

    // 指定されたグリッド座標から、シーン上の座標を求める
    public static Vector3 calcBlockPosition(Block.iPosition i_pos)
    {
        // 配置する左上隅の位置を初期値として設定
        Vector3 position = new Vector3(-(Block.BLOCK_NUM_X / 2.0f - 0.5f), -(Block.BLOCK_NUM_Y / 2.0f - 0.5f), 0.0f);

        // 初期値＋グリッド座標×ブロックサイズ
        position.x += (float)i_pos.x * Block.COLLISION_SIZE;
        position.y += (float)i_pos.y * Block.COLLISION_SIZE;

        return (position); // シーン上の座標を返す
    }
}
