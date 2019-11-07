using UnityEngine;

public class BlockCreator : MonoBehaviour
{
    public GameObject[] blockPrefabs; // ブロックを格納する配列
    private int block_count = 0;      // 作成したブロックの個数

    void Start()
    {
    }

    void Update()
    {
    }

    public void createBlock(Vector3 block_position)
    {
        // 作成すべきブロックの種類(白か赤)を求める
        int next_block_type = this.block_count % this.blockPrefabs.Length;

        // ブロックを生成し、goに保管
        GameObject go = GameObject.Instantiate(this.blockPrefabs[next_block_type]) as GameObject;

        go.transform.position = block_position; // ブロックの位置を移動
        this.block_count++; // ブロックの個数をインクリメント
    }
}
