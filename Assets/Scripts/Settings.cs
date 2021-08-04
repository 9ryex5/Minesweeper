using UnityEngine;

public class Settings : MonoBehaviour
{
    public static Settings S;

    public int boardSizeX, boardSizeY;
    public int nMines;

    private void Awake()
    {
        S = this;
    }
}
