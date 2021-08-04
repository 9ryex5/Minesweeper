using UnityEngine;
using UnityEngine.UI;

public class ManagerUI : MonoBehaviour
{
    public static ManagerUI MUI;

    public Text textTimer;
    public GameObject buttonRestart;

    public void Awake()
    {
        MUI = this;
    }

    public void UpdateTimer(float _timer)
    {
        textTimer.text = Mathf.FloorToInt(_timer / 60).ToString("D2") + ":" + Mathf.FloorToInt(_timer % 60).ToString("D2");
    }

    public void Endgame(bool _win)
    {
        buttonRestart.SetActive(true);
    }
}
