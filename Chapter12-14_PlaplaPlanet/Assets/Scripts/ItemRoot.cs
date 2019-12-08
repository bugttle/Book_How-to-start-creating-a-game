using UnityEngine;

public class Item
{
    public enum TYPE // アイテムの種類
    {
        NONE = -1, // なし
        IRON = 0, // 鉄鉱石
        APPLE, // リンゴ
        PLANT, // 植物
        NUM, // アイテムが何種類あるかを示す（=3）
    }
}

public class ItemRoot : MonoBehaviour
{
    // アイテムの種類を、Item.TYPE型で返すメソッド
    public Item.TYPE getItemType(GameObject item_go)
    {
        Item.TYPE type = Item.TYPE.NONE;
        if (item_go != null) // 引数で受け取ったGameObjectが空っぽでないなら
        {
            switch (item_go.tag) // タグで分岐
            {
                case "Iron": type = Item.TYPE.IRON; break;
                case "Apple": type = Item.TYPE.APPLE; break;
                case "Plant": type = Item.TYPE.PLANT; break;
            }
        }
        return (type);
    }
}
