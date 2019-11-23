using UnityEngine;

public class SceneControl : MonoBehaviour
{
    private BlockRoot block_root = null;

    void Start()
    {
        // BlockRootスクリプトを取得
        this.block_root = this.gameObject.GetComponent<BlockRoot>();
        // BlockRootスクリプトのinitialSetUp()を呼び出す
        this.block_root.initialSetUp();
    }

    void Update()
    {
    }
}
