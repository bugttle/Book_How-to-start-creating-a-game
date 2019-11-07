using UnityEngine;

public class CameraControl : MonoBehaviour
{
    private GameObject player = null;
    private Vector3 position_offset = Vector3.zero;

    void Start()
    {
        // メンバー変数playerに、Playerオブジェクトを取得
        this.player = GameObject.FindGameObjectWithTag("Player");
        // カメラの位置(this.transform.position)と
        // プレイヤープレイヤーの位置(this.player.transform.position)との差分を保管
        this.position_offset = this.transform.position - this.player.transform.position;
    }

    void LateUpdate()
    {
        // カメラの現在位置をnew_positionに取得
        Vector3 new_position = this.transform.position;
        // プレイヤーのX座標に差分を足して、変数new_positionのXに代入する
        new_position.x = this.player.transform.position.x + this.position_offset.x;
        // 毛mらの位置を、新しい位置(new_position)に更新
        this.transform.position = new_position;
    }
}
