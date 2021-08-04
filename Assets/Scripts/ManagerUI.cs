using UnityEngine;

public class ManagerUI : MonoBehaviour
{
    public static ManagerUI MUI;

    public GameObject buttonRestart;

    public void Awake()
    {
        MUI = this;
    }

    public void Endgame(bool _win)
    {
        buttonRestart.SetActive(true);
    }
}
