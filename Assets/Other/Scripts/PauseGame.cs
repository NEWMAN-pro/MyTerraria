using UnityEngine;

public class PauseGame : MonoBehaviour
{
    // �Ƿ���ͣ
    public bool pause = false;

    private void Start()
    {
        // ���ظýű�ʱ�����ű����ӽ�����ͣ�ű�
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
