using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController: MonoBehaviour {

    const float cellWidth = 1f;

    public GameObject board;
    public int boardSize = 10;

    BoardModel boardModel;
    Vector2Int? switchStartPosition;

    void Start () {
        setUpBoard();
    }

    void Update () {
        if (Input.GetMouseButtonDown(0) && switchStartPosition == null) {
            setUpCursor();
        } else if (Input.GetMouseButtonDown(0) && switchStartPosition != null) {
            handleSwitch();
        }
    }

    void setUpCursor () {
        switchStartPosition = cellPosition(Input.mousePosition);
        var cell = getCellFromPosition(switchStartPosition.Value);
        cell.setCursor();
    }

    void resetCursor () {
        var cell = getCellFromPosition(switchStartPosition.Value);
        cell.resetCursor();
        switchStartPosition = null;
	}

	void handleSwitch () {
        var switchEnd = cellPosition(Input.mousePosition);
        if (needToSwith(switchStartPosition.Value, switchEnd)) {
            switchBlocks(switchStartPosition.Value, switchEnd);
        }
        resetCursor();
    }

    bool needToSwith (Vector2Int start, Vector2Int end) {
		var distance = start - end;
        return Utils.equalFloat(distance.magnitude, 1);
    }

    void switchBlocks (Vector2Int start, Vector2Int end) {
        boardModel.collect(start, end);

        if (boardModel.collected.Count > 0) {
            var startCell = getCellFromPosition(start);
            var endCell = getCellFromPosition(end);
            var startBlock = startCell.transform.GetChild(0);
            var endBlock = endCell.transform.GetChild(0);
            startBlock.transform.parent = endCell.transform;
            endBlock.transform.parent = startCell.transform;
            startBlock.transform.position = endCell.transform.position;
            endBlock.transform.position = startCell.transform.position;
        }

        foreach(var position in boardModel.collected) {
            var cell = getCellFromPosition(position);
            cell.collect();
        }

        boardModel.collected = new HashSet<Vector2Int>();
    }

    Vector2Int cellPosition (Vector2 pixelPosition) {
        var cameraSize = 15;
        var blockSizeInPixel = Screen.height / cameraSize;
        var offsetX = Screen.width - blockSizeInPixel * boardSize;
        var offsetY = Screen.height - blockSizeInPixel * boardSize;
        var offset = new Vector2(offsetX, offsetY);
        var position = (pixelPosition - offset) / blockSizeInPixel;
        var x = (int)Mathf.Floor(position.x);
        var y = (int)Mathf.Floor(position.y);
        return new Vector2Int(x, y);
    }

    Cell getCellFromPosition (Vector2Int position) {
        var childIndex = position.y * boardSize + position.x;
        var cell = board.transform.GetChild(childIndex);
        return cell.GetComponent<Cell>();
    }

    bool inBound (Vector2 pixelPosition) {
        if (pixelPosition.x < 0) {
            return false;
        }
        if (pixelPosition.x > boardSize) {
            return false;
        }
        if (pixelPosition.y < 0) {
            return false;
        }
        if (pixelPosition.y > boardSize) {
            return false;
        }
        return true;
    }

    void setUpBoard () {
        boardModel = new BoardModel(boardSize);
        const float cellOffset = cellWidth / 2f;
        float rowStartPosition = -boardSize / 2f,
            columnStartPosition = -boardSize / 2f;
        var cellPrefab = Resources.Load<GameObject>("Prefabs/Cell");
        for (var column = 0; column < boardSize; column += 1) {
            for (var row = 0; row < boardSize; row += 1) {
                var x = rowStartPosition + row + cellOffset;
                var y = columnStartPosition + column + cellOffset;
                var position = new Vector3(x, y, 0);
                var instance = Instantiate(
                    cellPrefab,
                    position,
                    Quaternion.identity
                );
                instance.transform.parent = board.transform;
                BlockData blockData = BlockData.getRandom();
                instance.GetComponent<Cell>().setUp(blockData);
                boardModel.setCell(row, column, blockData.type);
            }
        }
        var boardY = SceneUtils.instance.maxY - boardSize / 2f;
        var boardX = SceneUtils.instance.maxX - boardSize / 2f;
        board.transform.position = new Vector3(boardX, boardY, 0);
    }

    public void toMenu () {
        SceneManager.LoadScene("MenuScene");
    }
}
