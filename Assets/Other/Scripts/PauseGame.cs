using UnityEngine;

public class PauseGame : MonoBehaviour
{
    // 是否暂停
    public bool pause = false;

    private void Start()
    {
        // 加载该脚本时，将脚本增加进总暂停脚本
        GameObject.Find("Map").GetComponent<PauseGameAll>().Add(this);
    }

    public void OnPauseGame()
    {
        pause = true;
    }

    public void UnPauseGame()
    {
        pause = false;
    }
}
