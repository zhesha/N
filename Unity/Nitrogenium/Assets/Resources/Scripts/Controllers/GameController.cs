using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController: MonoBehaviour, BoardEventReceiver {

    const string fileWithLevel = "Level.json";
    const float cameraSize = 15;
    const float cellWidth = 1f;

    public GameObject board;
    public Text scoreText;

    BoardModel boardModel;
    Vector2Int? switchStartPosition;
    Vector2Int? switchEndPosition;
    float controllFreezeOnSwitch;
    int boardSize;

    public void toMenu () {
        SceneManager.LoadScene("MenuScene");
    }

    #region BoardEventReceiver

    public void resetBlock (Vector2Int position, BlockType type, Vector2Int offset) {
        var cell = getCellFromPosition(position);
        var data = BlockData.getFromType(type);
        cell.setUpBlock(data, offset);
    }

    public int moveLenght {
        set {
            controllFreezeOnSwitch = value;
        }
    }

    #endregion

    void Start () {
        setUpBoard();
    }

    void Update () {
        if (controllFreezeOnSwitch > 0) {
            controllFreezeOnSwitch -= Time.deltaTime;
            if (controllFreezeOnSwitch <= 0) {
                finishMove();
            }
            return;
        }

        if (Input.GetMouseButtonDown(0) && switchStartPosition == null) {
            setUpCursor();
        } else if (Input.GetMouseButtonDown(0) && switchStartPosition != null) {
            handleSwitch();
        }
    }

    void setUpCursor () {
        switchStartPosition = cellPosition(Input.mousePosition);
        var cell = getCellFromPosition(switchStartPosition.Value);
        cell.cursorSeted = true;
    }

    void resetCursor () {
        var cell = getCellFromPosition(switchStartPosition.Value);
        cell.cursorSeted = false;
	}

	void handleSwitch () {
        if (switchEndPosition == null) {
            switchEndPosition = cellPosition(Input.mousePosition);
        }
        if (canSwith(switchStartPosition.Value, switchEndPosition.Value)) {
            boardModel.collect(switchStartPosition.Value, switchEndPosition.Value);
            resetCursor();
        }

    }

    bool canSwith (Vector2Int start, Vector2Int end) {
		var distance = start - end;
        return Utils.equalFloat(distance.magnitude, 1);
    }

    Vector2Int cellPosition (Vector2 pixelPosition) {
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

    void finishMove () {
        var commited = boardModel.commitCollection();
        if (commited < 0 && switchStartPosition != null && switchEndPosition != null) {
            handleSwitch();
        } else {
            updateScore(commited);
        }
        switchStartPosition = null;
        switchEndPosition = null;
    }

    void updateScore (int count) {
        var oldScore = System.Int32.Parse(scoreText.text);
        scoreText.text = (oldScore + count).ToString();
    }

    void setUpBoard () {
        var path = Path.Combine(Application.streamingAssetsPath, fileWithLevel);
        if (File.Exists(path)) {
            var data = File.ReadAllText(path);
            LevelData levelData = JsonUtility.FromJson<LevelData>(data);
            setUpBoard(levelData);
        } else {
            Debug.LogError("Level not loaded");
        }
    }

    void setUpBoard (LevelData levelData) {
        boardSize = levelData.boardSize;
        boardModel = new BoardModel(boardSize, this);
        const float cellOffset = cellWidth / 2f;
        float rowStartPosition = -boardSize / 2f,
            columnStartPosition = -boardSize / 2f;
        var cellPrefab = Resources.Load<GameObject>("Prefabs/Cell");
        for (var row = 0; row < boardSize; row += 1) {
            for (var column = 0; column < boardSize; column += 1) {
                var x = columnStartPosition + column + cellOffset;
                var y = rowStartPosition + row + cellOffset;
                var position = new Vector3(x, y, 0);
                var instance = Instantiate(
                    cellPrefab,
                    position,
                    Quaternion.identity
                );
                instance.transform.parent = board.transform;
                var blockData = BlockData.getFromType((BlockType)levelData.cell(column, row));
                instance.GetComponent<Cell>().setUp(blockData);
                boardModel.setCell(column, row, blockData.type);
            }
        }
        var boardY = SceneUtils.instance.maxY - boardSize / 2f;
        var boardX = SceneUtils.instance.maxX - boardSize / 2f;
        board.transform.position = new Vector3(boardX, boardY, 0);
    }
}