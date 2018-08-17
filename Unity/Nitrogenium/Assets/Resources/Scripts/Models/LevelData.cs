using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
struct LevelData {
    public int boardSize;
    public int[] board;

    public int cell (int row, int column) {
        return board[column * boardSize + row];
    }
}
