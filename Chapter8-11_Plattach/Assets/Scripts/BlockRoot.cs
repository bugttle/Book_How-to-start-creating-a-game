using UnityEngine;

public class BlockRoot : MonoBehaviour
{
    public GameObject BlockPrefab = null; // 作り出すべきブロックのPrefab
    public BlockControl[,] blocks; // マス目（グリッド）
    private GameObject main_camera = null; // メインカメラ
    private BlockControl grabbed_block = null; // つかんだブロック

    void Start()
    {
        this.main_camera = GameObject.FindGameObjectWithTag("MainCamera");
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mouse_position; // マウスの位置
        this.unprojectMousePosition(out mouse_position, Input.mousePosition); // マウスの位置を取得

        // 取得したマウスの位置をXとYだけにする
        Vector2 mouse_position_xy = new Vector2(mouse_position.x, mouse_position.y);

        if (this.grabbed_block == null) // ブロックをつかんでいないとき
        {
            //if (!this.is_has_falling_block())
            //{
            if (Input.GetMouseButtonDown(0)) // マウスボタンが押されたら
            {
                // blocks配列のすべての要素を順に処理する
                foreach (BlockControl block in this.blocks)
                {
                    if (!block.isGrabbable()) // ブロックがつかめないなら
                    {
                        continue; // 次のブロックへ
                    }
                    // マウス位置がブロックの領域内にないなら
                    if (!block.isContainedPosition(mouse_position_xy))
                    {
                        continue; // 次のブロックへ
                    }

                    // 処理中のブロックをgrabbled_blockに登録
                    this.grabbed_block = block;
                    // つかんだときの処理を実行
                    this.grabbed_block.beginGrab();
                    break;
                }
            }
            //}
        }
        else // ブロックをつかんでいるとき
        {
            if (!Input.GetMouseButton(0)) // マウスボタンが押されていないなら
            {
                this.grabbed_block.endGrab(); // ブロックを話したときの処理を実行
                this.grabbed_block = null; // grabbed_blockを空っぽに設定
            }
        }
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

    public bool unprojectMousePosition(out Vector3 world_position, Vector3 mouse_position)
    {
        bool ret;

        // 板を作成。この板はカメラから見える面が表で
        // ブロックの半分のサイズ分、手前に置かれる
        Plane plane = new Plane(Vector3.back, new Vector3(0.0f, 0.0f, -Block.COLLISION_SIZE / 2.0f));

        // カメラとマウスを通る光線を作成
        Ray ray = this.main_camera.GetComponent<Camera>().ScreenPointToRay(mouse_position);

        float depth;

        // 光線(ray)が板(plane)に当たっているなら
        if (plane.Raycast(ray, out depth))
        {
            // 引数world_positionを、マウスの位置で上書き
            world_position = ray.origin + ray.direction * depth;
            ret = true;
        }
        else // 当たっていないなら
        {
            // 引数world_positionをゼロのベクターで上書き
            world_position = Vector3.zero;
            ret = false;
        }
        return (ret);
    }
}
