using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardModel {

    HashSet<Vector2Int> collected;
    SortedList<int, Vector2Int> newGenerationsCandidats;

    readonly BlockType[,] board;
    BoardEventReceiver receiver;

    public BoardModel (int boardSize, BoardEventReceiver receiver) {
        board = new BlockType[boardSize, boardSize];
        this.receiver = receiver;
    }

    public void setCell (int x, int y, BlockType blockType) {
        board[x, y] = blockType;
    }

    public BlockType getCell (int x, int y) {
        return board[x, y];
    }

    public void collect (Vector2Int start, Vector2Int end) {
        collected = new HashSet<Vector2Int>();
        var startType = board[start.x, start.y];
        var endType = board[end.x, end.y];
        collect(startType, end, start - end);
        collect(endType, start, end - start);
        if (collected.Count > 0) {
            board[start.x, start.y] = endType;
            board[end.x, end.y] = startType;
            receiver.switchBlocks(start, end);
        } else {
            receiver.cancelSwitch(start, end);
        }
    }

    public void commitCollection () {
        newGenerationsCandidats = new SortedList<int, Vector2Int>();
        var pushDownColunms = new HashSet<int>();
        foreach (var position in collected) {
            board[position.x, position.y] = BlockType.none;
            pushDownColunms.Add(position.x);
        }
        pushDownAll(pushDownColunms);
        generateNew();
        collected = new HashSet<Vector2Int>();
    }

    void pushDownAll(HashSet<int> colunms) {
        foreach(var x in colunms) {
            for (var y = 0; y < board.GetLength(0); y += 1) {
                if(board[x, y] == BlockType.none) {
                    pushDown(new Vector2Int(x, y));
                }
            }
        }
    }

    void pushDown (Vector2Int position) {
        for (
            var newPosition = position + Vector2Int.up;
            inBound(newPosition);
            newPosition += Vector2Int.up
        ) {
            
            var candidat = board[newPosition.x, newPosition.y];
            if (candidat != BlockType.none) {
                board[position.x, position.y] = candidat;
                receiver.moveBlocks(newPosition, position);
                board[newPosition.x, newPosition.y] = BlockType.none;
                return;
            }
        }

        //keys must be unique and sorted in column, according to y
        var key = position.x * board.GetLength(0) + position.y;
        newGenerationsCandidats.Add(key, position);
    }

    void generateNew() {
        //offsets is <column number: how many new blocks in this colunm will be>
        var offsets = new Dictionary<int, int>();
        foreach(var data in newGenerationsCandidats) {
            var position = data.Value;
            if (offsets.ContainsKey(position.x)) {
                offsets[position.x] += 1;
            } else {
                offsets.Add(position.x, 1);
            }
            int offset = offsets[position.x];
            receiver.newGeneration(position, offset);
        }
    }

    void collect (BlockType blockType, Vector2Int position, Vector2Int skip) {
        Vector2Int leftBound, rightBound, upBound, downBound;
        leftBound = rightBound = upBound = downBound = position;
        if (skip != Vector2Int.left) {
            leftBound = calculateBound(blockType, position, Vector2Int.left);
        }
        if (skip != Vector2Int.right) {
            rightBound = calculateBound(blockType, position, Vector2Int.right);
        }
        if (skip != Vector2Int.up) {
            upBound = calculateBound(blockType, position, Vector2Int.up);
        }
        if (skip != Vector2Int.down) {
            downBound = calculateBound(blockType, position, Vector2Int.down);
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
        var startX = Mathf.Min(start.x, end.x);
        var endX = Mathf.Max(start.x, end.x);
        for (var x = startX; x <= endX; x++) {
            collected.Add(new Vector2Int(x, start.y));
        }
    }

    void collectVertical (Vector2Int start, Vector2Int end) {
        var startY = Mathf.Min(start.y, end.y);
        var endY = Mathf.Max(start.y, end.y);
        for (var y = startY; y <= endY; y++) {
            collected.Add(new Vector2Int(start.x, y));
        }
    }

    bool isSame (BlockType blockType, Vector2Int position) {
        if (!inBound(position)) {
            return false;
        }
        return board[position.x, position.y] == blockType;
    }

    bool inBound (Vector2 position) {
        if (position.x < 0) {
            return false;
        }
        if (position.x >= board.GetLength(0)) {
            return false;
        }
        if (position.y < 0) {
            return false;
        }
        if (position.y >= board.GetLength(1)) {
            return false;
        }
        return true;
    }
}

public interface BoardEventReceiver {
    void cancelSwitch (Vector2Int firsh, Vector2Int second);
    void switchBlocks (Vector2Int firsh, Vector2Int second);
    void moveBlocks (Vector2Int from, Vector2Int to);
    void newGeneration (Vector2Int position, int offset);
}