using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardModel {

    public HashSet<Vector2Int> collected = new HashSet<Vector2Int>();

    BlockType[,] board;

    public BoardModel (int boardSize) {
        board = new BlockType[boardSize, boardSize];
    }

    public void setCell (int x, int y, BlockType blockType) {
        board[x, y] = blockType;
    }

    public BlockType getCell (int x, int y) {
        return board[x, y];
    }

    public void collect (Vector2Int start, Vector2Int end) {
        var startType = board[start.x, start.y];
        var endType = board[end.x, end.y];
        collect(startType, end, start - end);
        collect(endType, start, end - start);
        if (collected.Count > 0) {
            board[start.x, start.y] = endType;
            board[end.x, end.y] = startType;
        }
        cleanUpCollected();
    }

    void collect (BlockType blockType, Vector2Int position, Vector2Int skip) {
        Vector2Int leftBound, rightBound, upBound, downBound;
        if (skip != Vector2Int.left) {
            leftBound = calculateBound(blockType, position, Vector2Int.left);
        } else {
            leftBound = position;
        }
        if (skip != Vector2Int.right) {
            rightBound = calculateBound(blockType, position, Vector2Int.right);
        } else {
            rightBound = position;
        }
        if (skip != Vector2Int.up) {
            upBound = calculateBound(blockType, position, Vector2Int.up);
        } else {
            upBound = position;
        }
        if (skip != Vector2Int.down) {
            downBound = calculateBound(blockType, position, Vector2Int.down);
        } else {
            downBound = position;
        }

        if ((leftBound - rightBound).magnitude > 1) {
            collectHorizontal(leftBound, rightBound);
        }
        if ((upBound - downBound).magnitude > 1) {
            collectVertical(upBound, downBound);
        }
    }

    Vector2Int calculateBound (
        BlockType blockType,
        Vector2Int position,
        Vector2Int direction
    ) {
        
        position += direction;
        while (isSame(blockType, position)) {
            position += direction;
        }
        return position - direction;
    }

    void collectHorizontal (Vector2Int start, Vector2Int end) {
        int startX, endX;
        if (start.x - end.x > 0) {
            startX = end.x;
            endX = start.x;
        } else {
            startX = start.x;
            endX = end.x;
        }
        for (var x = startX; x <= endX; x++) {
            collected.Add(new Vector2Int(x, start.y));
        }
    }

    void collectVertical (Vector2Int start, Vector2Int end) {
        int startY, endY;
        if (start.y - end.y > 0) {
            startY = end.y;
            endY = start.y;
        } else {
            startY = start.y;
            endY = end.y;
        }
        for (var y = startY; y <= endY; y++) {
            collected.Add(new Vector2Int(start.x, y));
        }
    }

    void cleanUpCollected () {
        foreach (var position in collected) {
            board[position.x, position.y] = BlockType.none;
        }
    }

    bool isSame (BlockType blockType, Vector2Int position) {
        if (!inBound(position)) {
            return false;
        }
        return board[position.x, position.y] == blockType;
    }

    bool inBound (Vector2 position) {
        var boardSize = board.GetLength(0) - 1;
        if (position.x < 0) {
            return false;
        }
        if (position.x > boardSize) {
            return false;
        }
        if (position.y < 0) {
            return false;
        }
        if (position.y > boardSize) {
            return false;
        }
        return true;
    }
}
