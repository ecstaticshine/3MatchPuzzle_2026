using System.Collections.Generic;
using UnityEngine;

public class BoardView : MonoBehaviour
{
    public GameObject tilePrefab;
    public GameObject fruitPrefab;
    public Sprite[] fruitSprites; // Inspector에서 6종류 할당
    public float cellSize = 1f;

    private GameObject[] tilePool = new GameObject[81];
    private GameObject[] fruitPool = new GameObject[81];

    private void Awake()
    {
        InitPool();
    }

    public void Render(BoardModel boardModel)
    {
        for (int r = 0; r < BoardConstants.ROWS; r++)
        {
            for (int c = 0; c < BoardConstants.COLS; c++)
            {
                Cell cell = boardModel.GetCell(r, c);
                int index = r * BoardConstants.COLS + c;
                Vector3 tilePos = new Vector3(c * cellSize, -r * cellSize, 0);
                Vector3 fruitPos = new Vector3(c * cellSize, -r * cellSize, -1);

                if (cell.isActive)
                {
                    // 타일 GameObject 생성
                    tilePool[index].SetActive(true);
                    tilePool[index].transform.position = tilePos;
                    fruitPool[index].SetActive(true);
                    fruitPool[index].transform.position = fruitPos;
                    fruitPool[index].GetComponent<SpriteRenderer>().sprite = fruitSprites[(int)cell.type - 1];
                    // PuzzleType에 따라 과일 Sprite 설정
                }
            }
        }
    }
    // 초기화
    public void InitPool()
    {
        for (int i = 0; i < 81; i++)
        {
            tilePool[i] = Instantiate(tilePrefab);
            tilePool[i].SetActive(false);

            fruitPool[i] = Instantiate(fruitPrefab);
            fruitPool[i].SetActive(false);
        }
    }

    // 바꾼 부분만 업데이트
    public void UpdateCell(int r, int c, PuzzleType type)
    {
        int index = r * BoardConstants.COLS + c;
        fruitPool[index].GetComponent<SpriteRenderer>().sprite = fruitSprites[(int)type - 1];
        fruitPool[index].SetActive(type != PuzzleType.None);
    }

    public void UpdateCol(int col, BoardModel boardModel)
    {
        for(int r = 0; r < BoardConstants.ROWS; r++)
        {
            int index = r * BoardConstants.COLS + col;
            Cell cell = boardModel.GetCell(r, col);

            if (cell.isActive && cell.type != PuzzleType.None)
            {
                fruitPool[index].SetActive(true);
                fruitPool[index].GetComponent<SpriteRenderer>().sprite = fruitSprites[(int)cell.type - 1];
            }
            else
            {
                fruitPool[index].SetActive(false);
            }
        }
    }
}
