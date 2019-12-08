using UnityEngine;

public class GameStatus : MonoBehaviour
{
    // 鉄鉱石、植物のそれぞれを使ったときの修理具合
    public static float GAIN_REPAIRMENT_IRON = 0.30f;
    public static float GAIN_REPAIRMENT_PLANT = 0.10f;

    // 鉄鉱石、リンゴ、植物のそれぞれを選んだときに減るお腹具合
    public static float CONSUME_SATIETY_IRON = 0.15f;
    public static float CONSUME_SATIETY_APPLE = 0.1f;
    public static float CONSUME_SATIETY_PLANT = 0.1f;

    // リンゴ、植物のそれぞれを食べたときに回復するお腹具合
    public static float REGAIN_SATIETY_APPLE = 0.7f;
    public static float REGAIN_SATIETY_PLANT = 0.2f;

    public float repairment = 0.0f; // 宇宙船の修理具合（0.0f～1.0f）
    public float satiety = 1.0f; // 満腹度（0.0f～1.0f）

    public GUIStyle guistyle; // フォントスタイル

    void Start()
    {
        this.guistyle.fontSize = 24; // フォントサイズを24に設定
    }

    void Update()
    {

    }

    void OnGUI()
    {
        float x = Screen.width * 0.2f;
        float y = 20.0f;
        // 満腹度を表示
        GUI.Label(new Rect(x, y, 200.0f, 20.0f), "おなか:" + (this.satiety * 100.0f).ToString("000"), guistyle);
        x += 200;
        // 修理具合を表示
        GUI.Label(new Rect(x, y, 200.0f, 20.0f), "ロケット:" + (this.repairment * 100.0f).ToString("000"), guistyle);
    }

    public void addRepairment(float add)
    {
        this.repairment = Mathf.Clamp01(this.repairment + add);
    }

    public void addSatiety(float add)
    {
        this.satiety = Mathf.Clamp01(this.satiety + add);
    }

    public bool isGameClear()
    {
        bool is_clear = false;
        if (this.repairment >= 1.0f) // 修理具合が100%以上なら
        {
            is_clear = true; // クリアしている
        }
        return (is_clear);
    }
    public bool isGameOver()
    {
        bool is_over = false;
        if (this.satiety <= 0.0f) // 満腹度が0以下なら
        {
            is_over = true; // ゲームオーバー
        }
        return (is_over);
    }
}
