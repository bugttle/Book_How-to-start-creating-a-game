using System.Collections.Generic;
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
    public GameObject ironPrefab = null; // Prefab「Iron」の保持
    public GameObject plantPrefab = null; // Prefab「Plant」の保持
    public GameObject applePrefab = null; // Prefab「Apple」の保持
    protected List<Vector3> respawn_points; // 出現ポイントのList

    public float step_timer = 0.0f;
    public static float RESPAWN_TIME_APPPLE = 20.0f; // リンゴの出現時間定数
    public static float RESPAWN_TIME_IRON = 12.0f; // 鉄鉱石の出現時間定数
    public static float RESPAWN_TIME_PLANT = 6.0f; // 植物の出現時間定数
    private float respawn_timer_apple = 0.0f; // リンゴの出現時間
    private float respawn_timer_iron = 0.0f; // 鉄鉱石の出現時間
    private float respawn_timer_plant = 0.0f; // 植物の出現時間

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

    public void respawnIron()
    {
        // 鉄鉱石プレハブをインスタンス化
        GameObject go = GameObject.Instantiate(this.ironPrefab) as GameObject;
        // 鉄鉱石の出現ポイントを取得
        Vector3 pos = GameObject.Find("IronRespawn").transform.position;
        // 出現位置を調整
        pos.y = 1.0f;
        pos.x += Random.Range(-1.0f, 1.0f);
        pos.z += Random.Range(-1.0f, 1.0f);
        // 鉄鉱石の位置を移動　
        go.transform.position = pos;
    }

    public void respawnApple()
    {
        // リンゴプレハブをインスタンス化
        GameObject go = GameObject.Instantiate(this.applePrefab) as GameObject;

        // リンゴの出現ポイントを取得
        Vector3 pos = GameObject.Find("AppleRespawn").transform.position;
        // 出現位置を調整
        pos.y = 1.0f;
        pos.x += Random.Range(-1.0f, 1.0f);
        pos.z += Random.Range(-1.0f, 1.0f);
        // リンゴの位置を移動　
        go.transform.position = pos;
    }

    public void respawnPlant()
    {
        if (this.respawn_points.Count > 0) // Listが空っぽでないなら
        {
            // 植物プレハブをインスタンス化
            GameObject go = GameObject.Instantiate(this.plantPrefab) as GameObject;
            // 植物の出現ポイントをランダムに取得
            int n = Random.Range(0, this.respawn_points.Count);
            Vector3 pos = this.respawn_points[n];
            // 出現位置を調整
            pos.y = 1.0f;
            pos.x += Random.Range(-1.0f, 1.0f);
            pos.z += Random.Range(-1.0f, 1.0f);
            // 植物の位置を移動
            go.transform.position = pos;
        }
    }

    void Start()
    {
        // メモリ領域を確保
        this.respawn_points = new List<Vector3>();
        // "PlantRespawn"タグの付いた全オブジェクトを配列に格納
        GameObject[] respawns = GameObject.FindGameObjectsWithTag("PlantRespawn");

        // 配列respawns内の個々のGameObjectを順に処理する
        foreach (GameObject go in respawns)
        {
            // レンダラーを取得
            MeshRenderer renderer = go.GetComponentInChildren<MeshRenderer>();
            if (renderer != null) // レンダラーが存在するなら
            {
                renderer.enabled = false; // そのレンダラーを不可視に
            }
            // 出現ポイントListに位置情報を追加
            this.respawn_points.Add(go.transform.position);
        }
        // リンゴの出現ポイントを取得し、レンダラーを不可視に
        GameObject applerespawn = GameObject.Find("AppleRespawn");
        applerespawn.GetComponent<MeshRenderer>().enabled = false;
        // 鉄鉱石の出現ポイントを取得し、レンダラーを不可視に
        GameObject ironrespawn = GameObject.Find("IronRespawn");
        ironrespawn.GetComponent<MeshRenderer>().enabled = false;

        this.respawnIron(); // 鉄鉱石を1つ作成
        this.respawnPlant(); // 植物を3つ作成
        this.respawnPlant();
        this.respawnPlant();
    }

    void Update()
    {
        respawn_timer_apple += Time.deltaTime;
        respawn_timer_iron += Time.deltaTime;
        respawn_timer_plant += Time.deltaTime;

        if (respawn_timer_apple > RESPAWN_TIME_APPPLE)
        {
            respawn_timer_apple = 0.0f;
            this.respawnApple(); // リンゴを出現させる
        }
        if (respawn_timer_iron > RESPAWN_TIME_IRON)
        {
            respawn_timer_iron = 0.0f;
            this.respawnIron(); // 鉄鉱石を出現させる
        }
        if (respawn_timer_plant > RESPAWN_TIME_APPPLE)
        {
            respawn_timer_plant = 0.0f;
            this.respawnPlant(); // 植物を出現させる
        }
    }

    public float getGainRepairment(GameObject item_go)
    {
        float gain = 0.0f;
        if (item_go == null)
        {
            gain = 0.0f;
        }
        else
        {
            Item.TYPE type = this.getItemType(item_go);
            switch (type) // 持っているアイテムの種類で分岐
            {
                case Item.TYPE.IRON:
                    gain = GameStatus.GAIN_REPAIRMENT_IRON; break;
                case Item.TYPE.PLANT:
                    gain = GameStatus.GAIN_REPAIRMENT_PLANT; break;
            }
        }
        return (gain);
    }

    public float getConsumeSatiety(GameObject item_go)
    {
        float consume = 0.0f;
        if (item_go == null)
        {
            consume = 0.0f;
        }
        else
        {
            Item.TYPE type = this.getItemType(item_go);
            switch (type) // 持っているアイテムの種類で分岐
            {
                case Item.TYPE.IRON:
                    consume = GameStatus.CONSUME_SATIETY_IRON; break;
                case Item.TYPE.APPLE:
                    consume = GameStatus.CONSUME_SATIETY_APPLE; break;
                case Item.TYPE.PLANT:
                    consume = GameStatus.CONSUME_SATIETY_PLANT; break;
            }
        }
        return (consume);
    }

    public float getRegainSatiety(GameObject item_go)
    {
        float regain = 0.0f;
        if (item_go == null)
        {
            regain = 0.0f;
        }
        else
        {
            Item.TYPE type = this.getItemType(item_go);
            switch (type) // 持っているアイテムの種類で分岐
            {
                case Item.TYPE.APPLE:
                    regain = GameStatus.REGAIN_SATIETY_APPLE; break;
                case Item.TYPE.PLANT:
                    regain = GameStatus.REGAIN_SATIETY_PLANT; break;
            }
        }
        return (regain);
    }
}
