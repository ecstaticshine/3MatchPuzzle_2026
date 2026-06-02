using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    BoardModel boardModel = new BoardModel();
    [SerializeField] BoardView boardView;

    private Vector2Int? firstSelected = null;
    private Vector2Int? secondSelected = null;
    float cellSize;


    private void Start()
    {
        StartCoroutine(initBoard());
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            int c = Mathf.RoundToInt(worldPos.x / cellSize);
            int r = Mathf.RoundToInt(-worldPos.y / cellSize);

            if (firstSelected == null)
            {
                firstSelected = new Vector2Int(c,r);
            }
            else
            {
                secondSelected = new Vector2Int(c, r);

                int dr = Mathf.Abs(secondSelected.Value.x - firstSelected.Value.x);
                int dc = Mathf.Abs(secondSelected.Value.y - firstSelected.Value.y);

                if (dr + dc == 1)
                {
                    boardModel.Swap(firstSelected.Value, secondSelected.Value);
                    HashSet<Vector2Int> matched = boardModel.FindMatches();
                    if (matched.Count == 0)
                    {
                        boardModel.Swap(firstSelected.Value, secondSelected.Value); // 되돌리기
                    }
                    else
                    {
                        StartCoroutine(ProcessBoard(matched));
                    }
                    firstSelected = null;
                    secondSelected = null;
                }
                else
                {
                    // 인접하지 않으면 첫 번째 선택 초기화
                    firstSelected = null;
                }
            }


            // 범위 체크
            // firstSelected가 null이면 첫 번째 선택
            // 아니면 두 번째 선택 → 스왑
        }
    }

    IEnumerator initBoard()
    {
        yield return new WaitUntil(()=> DataManager.Instance.stageDataList != null);
        int stage = DataManager.Instance.selectedStage;
        StageData stageData = DataManager.Instance.stageDataList[stage - 1];
        boardModel.Initialize(stageData);
        boardView.Render(boardModel);
        cellSize = boardView.cellSize;
    }

    public IEnumerator ProcessBoard(HashSet<Vector2Int> matched)
    {

        while (matched.Count > 0)
        {
            HashSet<int> affectedCols = new HashSet<int>();
            foreach (var pos in matched)
            {
                affectedCols.Add(pos.y);
            }

            // View 갱신
            boardModel.RemoveMatches(matched);
            foreach (int col in affectedCols)
            {
                boardView.UpdateCol(col, boardModel);
            }
            yield return new WaitForSeconds(0.3f);
            // View 갱신
            boardModel.Fall();
            foreach (int col in affectedCols)
            {
                boardView.UpdateCol(col, boardModel);
            }
            yield return new WaitForSeconds(0.3f);
            // View 갱신
            boardModel.Refill();
            foreach (int col in affectedCols)
            {
                boardView.UpdateCol(col, boardModel);
            }
            yield return new WaitForSeconds(0.3f);
            matched = boardModel.FindMatches();
        }
    }
}
