using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController: MonoBehaviour {

    public GameObject board;

    const int boardSize = 10;
    const float cellWidth = 1f;

    void Start () {
        const float width = boardSize / 2;
        Camera.main.orthographicSize = Screen.height / (Screen.width / width);
        setUpBoard();
    }

    void setUpBoard () {
        const float cellOffset = cellWidth / 2;
        const float rowStartPosition = -5, columnStartPosition = -5;
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
            }
        }
        var boardY = SceneUtils.instance.minY + boardSize / 2;
        board.transform.position = new Vector3(0, boardY, 0);
    }
}
