using UnityEngine;

public class GameRoot : MonoBehaviour
{
    public float step_timer = 0.0f; // 経過時間を保持
    private PlayerControl player = null;

    void Start()
    {
        this.player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
    }

    void Update()
    {
        this.step_timer += Time.deltaTime; // 刻々と経過時間を足していく

        if (this.player.isPlayEnd())
        {
            Application.LoadLevel("TitleScene");
        }
    }

    public float getPlayTime()
    {
        float time;
        time = this.step_timer;
        return (time); // 呼び出し元に、経過時間を教えてあげる
    }
}
