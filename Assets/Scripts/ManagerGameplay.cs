using UnityEngine;

public class ManagerGameplay : MonoBehaviour
{
    public Camera myCamera;

    public Transform parentCells;
    public Transform prefabCell;

    public Transform parentMines;
    public Transform parentFlags;
    public Transform parentKnown;
    public Sprite spriteMine;
    public Sprite spriteFlag;
    public Sprite spriteSafe;
    public Sprite[] spritesMinesAround;

    private Cell[,] board;

    private float timer;

    private bool isPlaying;

    private struct Cell
    {
        public bool mined;
        public bool flagged;
        public bool known;
    }

    private void Start()
    {
        DrawBoard();
        SpawnMines();
        isPlaying = true;
    }

    private void Update()
    {
        if (!isPlaying) return;

        timer += Time.deltaTime;
        ManagerUI.MUI.UpdateTimer(timer);

        if (Input.GetMouseButtonDown(0))
        {
            int cellX = Mathf.RoundToInt(myCamera.ScreenToWorldPoint(Input.mousePosition).x);
            int cellY = Mathf.RoundToInt(myCamera.ScreenToWorldPoint(Input.mousePosition).y);

            if (IsCellBoard(new Vector2Int(cellX, cellY)) && !board[cellX, cellY].flagged)
            {
                if (board[cellX, cellY].mined)
                {
                    SpawnIcon("Mine", spriteMine, new Vector2Int(cellX, cellY), parentMines);
                    ManagerUI.MUI.Endgame(false);
                    isPlaying = false;
                }
                else
                {
                    ShowCell(new Vector2Int(cellX, cellY));
                }
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            int cellX = Mathf.RoundToInt(myCamera.ScreenToWorldPoint(Input.mousePosition).x);
            int cellY = Mathf.RoundToInt(myCamera.ScreenToWorldPoint(Input.mousePosition).y);

            if (IsCellBoard(new Vector2Int(cellX, cellY)) && (!board[cellX, cellY].known))
            {
                if (board[cellX, cellY].flagged)
                {
                    for (int i = 0; i < parentFlags.childCount; i++)
                    {
                        if (parentFlags.GetChild(i).transform.position.x == cellX && parentFlags.GetChild(i).transform.position.y == cellY)
                        {
                            Destroy(parentFlags.GetChild(i).gameObject);
                            board[cellX, cellY].flagged = false;
                            break;
                        }
                    }
                }
                else
                {
                    SpawnIcon("Flag", spriteFlag, new Vector2Int(cellX, cellY), parentFlags);
                    board[cellX, cellY].flagged = true;
                    CheckWin();
                }
            }
        }
    }

    public void ButtonRestart()
    {
        for (int i = 0; i < parentMines.childCount; i++)
            Destroy(parentMines.GetChild(i).gameObject);

        for (int i = 0; i < parentFlags.childCount; i++)
            Destroy(parentFlags.GetChild(i).gameObject);

        for (int i = 0; i < parentKnown.childCount; i++)
            Destroy(parentKnown.GetChild(i).gameObject);

        SpawnMines();
        timer = 0;
        isPlaying = true;
    }

    private void DrawBoard()
    {
        myCamera.transform.position = new Vector3(Settings.S.boardSizeX / 2 - 0.5f, Settings.S.boardSizeY / 2 - 0.5f, -1);

        for (int x = 0; x < Settings.S.boardSizeX; x++)
            for (int y = 0; y < Settings.S.boardSizeY; y++)
                Instantiate(prefabCell, new Vector2(x, y), Quaternion.identity, parentCells);
    }

    private void SpawnMines()
    {
        board = new Cell[Settings.S.boardSizeX, Settings.S.boardSizeY];

        int rX = int.MinValue;
        int rY = int.MinValue;

        for (int i = 0; i < Settings.S.nMines; i++)
        {
            do
            {
                rX = Random.Range(0, Settings.S.boardSizeX);
                rY = Random.Range(0, Settings.S.boardSizeY);
            } while (board[rX, rY].mined);

            board[rX, rY].mined = true;
        }
    }

    private void ShowCell(Vector2Int _cell)
    {
        if (!IsCellBoard(_cell) || board[_cell.x, _cell.y].known) return;

        board[_cell.x, _cell.y].known = true;

        int nMinesAround = MinesAround(_cell);

        if (nMinesAround == 0)
        {
            SpawnIcon("Safe", spriteSafe, _cell, parentKnown);
            ShowCell(new Vector2Int(_cell.x - 1, _cell.y + 1));
            ShowCell(new Vector2Int(_cell.x, _cell.y + 1));
            ShowCell(new Vector2Int(_cell.x + 1, _cell.y + 1));
            ShowCell(new Vector2Int(_cell.x - 1, _cell.y));
            ShowCell(new Vector2Int(_cell.x + 1, _cell.y));
            ShowCell(new Vector2Int(_cell.x - 1, _cell.y - 1));
            ShowCell(new Vector2Int(_cell.x, _cell.y - 1));
            ShowCell(new Vector2Int(_cell.x + 1, _cell.y - 1));
        }
        else
        {
            SpawnIcon("Known", spritesMinesAround[nMinesAround - 1], _cell, parentKnown);
        }
    }

    private void SpawnIcon(string _name, Sprite _sprite, Vector2Int _pos, Transform _parent)
    {
        GameObject go = new GameObject(_name);
        go.transform.localScale = new Vector3(0.8f, 0.8f, 1);
        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = _sprite;
        go.transform.parent = _parent;
        go.transform.position = new Vector2(_pos.x, _pos.y);
    }

    private void CheckWin()
    {
        if (parentFlags.childCount != Settings.S.nMines) return;

        for (int i = 0; i < parentFlags.childCount; i++)
            if (!board[(int)parentFlags.GetChild(i).transform.position.x, (int)parentFlags.GetChild(i).transform.position.y].mined) return;

        ManagerUI.MUI.Endgame(true);
        isPlaying = false;
    }

    private bool IsCellBoard(Vector2Int _cell)
    {
        if (_cell.x < 0) return false;
        if (_cell.x >= Settings.S.boardSizeX) return false;
        if (_cell.y < 0) return false;
        if (_cell.y >= Settings.S.boardSizeY) return false;
        return true;
    }

    private int MinesAround(Vector2Int _cell)
    {
        int nMines = 0;

        Vector2Int currentCell = new Vector2Int(_cell.x - 1, _cell.y + 1);
        if (IsCellBoard(currentCell) && board[currentCell.x, currentCell.y].mined) nMines++;
        currentCell = new Vector2Int(_cell.x, _cell.y + 1);
        if (IsCellBoard(currentCell) && board[currentCell.x, currentCell.y].mined) nMines++;
        currentCell = new Vector2Int(_cell.x + 1, _cell.y + 1);
        if (IsCellBoard(currentCell) && board[currentCell.x, currentCell.y].mined) nMines++;
        currentCell = new Vector2Int(_cell.x - 1, _cell.y);
        if (IsCellBoard(currentCell) && board[currentCell.x, currentCell.y].mined) nMines++;
        currentCell = new Vector2Int(_cell.x + 1, _cell.y);
        if (IsCellBoard(currentCell) && board[currentCell.x, currentCell.y].mined) nMines++;
        currentCell = new Vector2Int(_cell.x - 1, _cell.y - 1);
        if (IsCellBoard(currentCell) && board[currentCell.x, currentCell.y].mined) nMines++;
        currentCell = new Vector2Int(_cell.x, _cell.y - 1);
        if (IsCellBoard(currentCell) && board[currentCell.x, currentCell.y].mined) nMines++;
        currentCell = new Vector2Int(_cell.x + 1, _cell.y - 1);
        if (IsCellBoard(currentCell) && board[currentCell.x, currentCell.y].mined) nMines++;

        return nMines;
    }
}
