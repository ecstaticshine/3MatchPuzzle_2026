using System;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{
    public bool isActive;      // 이 셀이 보드에 존재하는지
    public PuzzleType type;    // 어떤 타일인지
}

public static class BoardConstants
{
    public const int ROWS = 9;
    public const int COLS = 9;
}


public class BoardModel
{
    private Cell[,] grid = new Cell[BoardConstants.ROWS, BoardConstants.COLS];

    public Cell GetCell(int r, int c)
    {
        return grid[r, c];
    }

    public void Initialize(StageData data)
    {
        for (int r = 0; r < BoardConstants.ROWS; r++)
        {
            for (int c = 0; c < BoardConstants.COLS; c++)
            {
                grid[r, c] = new Cell();
                grid[r, c].isActive = data.activeCells[r * BoardConstants.COLS + c] == 1;
                grid[r, c].type = grid[r, c].isActive ? GetRandomType(r, c) : PuzzleType.None;
            }
        }
    }

    PuzzleType GetRandomType(int r, int c)
    {
        List<PuzzleType> candidates = new List<PuzzleType>
        {
            PuzzleType.Cherry,
            PuzzleType.Orange,
            PuzzleType.Banana,
            PuzzleType.BlueBerry,
            PuzzleType.Melon,
            PuzzleType.Grape,
        };

        if (r >= 2)
        {
            if (grid[r - 2, c].type.Equals(grid[r - 1, c].type))
            {
                candidates.Remove(grid[r - 1, c].type);
            }
        }

        if (c >= 2)
        {
            if (grid[r, c - 2].type.Equals(grid[r, c - 1].type))
            {
                candidates.Remove(grid[r, c - 1].type);
            }
        }

        return candidates[UnityEngine.Random.Range(0, candidates.Count)];
    }

    public void Swap(Vector2Int a, Vector2Int b)
    {
        PuzzleType temp = grid[a.x, a.y].type;
        grid[a.x, a.y].type = grid[b.x, b.y].type;
        grid[b.x, b.y].type = temp;
    }

    public HashSet<Vector2Int> FindMatches()
    {
        HashSet<Vector2Int> matched = new HashSet<Vector2Int>();

        // 가로
        for (int r = 0; r < BoardConstants.ROWS; r++)
        {
            int count = 1;
            for (int c = 1; c < BoardConstants.COLS; c++)
            {
                if (grid[r, c].type != PuzzleType.None &&
                    grid[r, c - 1].type.Equals(grid[r, c].type))
                {
                    count++;

                }
                else
                {
                    if (count >= 3)
                    {
                        for (int i = c - count; i < c; i++)
                        {
                            matched.Add(new Vector2Int(r, i));
                        }
                    }
                    count = 1;

                }
            }

            if (count >= 3)
            {
                for (int i = BoardConstants.COLS - count; i < BoardConstants.COLS; i++)
                    matched.Add(new Vector2Int(r, i));
            }
        }
        //세로
        for (int c = 0; c < BoardConstants.COLS; c++)
        {
            int count = 1;
            for (int r = 1; r < BoardConstants.ROWS; r++)
            {
                if (grid[r, c].type != PuzzleType.None &&
                    grid[r - 1, c].type.Equals(grid[r, c].type))
                {
                    count++;

                }
                else
                {
                    if (count >= 3)
                    {
                        for (int i = r - count; i < r; i++)
                        {
                            matched.Add(new Vector2Int(i, c));
                        }
                    }
                    count = 1;

                }
            }
            if (count >= 3)
            {
                for (int i = BoardConstants.ROWS - count; i < BoardConstants.ROWS; i++)
                    matched.Add(new Vector2Int(i, c));
            }
        }

        return matched;
    }

    // 지우기
    public void RemoveMatches(HashSet<Vector2Int> matched)
    {
        foreach (Vector2Int pos in matched)
        {
            int r = pos.x;
            int c = pos.y;
            grid[r, c].type = PuzzleType.None;
        }
    }

    // 낙하
    public void Fall()
    {
        for (int c = 0; c < BoardConstants.COLS; c++)
        {
            int emptyRow = BoardConstants.ROWS - 1; // 빈칸 포인터

            for (int r = BoardConstants.ROWS - 1; r >= 0; r--)
            {
                if (grid[r, c].type != PuzzleType.None)
                {
                    grid[emptyRow, c].type = grid[r, c].type;
                    if (emptyRow != r)
                        grid[r, c].type = PuzzleType.None;
                    emptyRow--;
                }
            }

        }
    }

    public void Refill()
    {
        var values = Enum.GetValues(typeof(PuzzleType));

        for (int r = 0; r < BoardConstants.ROWS; r++)
        {
            for (int c = 0; c < BoardConstants.COLS; c++)
            {
                if (grid[r, c].isActive && grid[r, c].type == PuzzleType.None)
                {
                    grid[r, c].type = (PuzzleType)values.GetValue(UnityEngine.Random.Range(1, values.Length));
                }
            }
        }
    }

}
