using UnityEngine;

public class BlockControl : MonoBehaviour
{
    public MapCreator map_creator = null; // MapCreatorを保管するための変数

    void Start()
    {
        // MapCreatorを取得して、メンバー変数map_creatorに保管
        map_creator = GameObject.Find("GameRoot").GetComponent<MapCreator>();
    }

    void Update()
    {
        if (this.map_creator.isDelete(this.gameObject)) // 見切れてるなら
        {
            GameObject.Destroy(this.gameObject); // 自分自身を削除
        }
    }
}
