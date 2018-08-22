using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardModel {

    HashSet<Vector2Int> collected = new HashSet<Vector2Int>();
    SortedList<int, Vector2Int> newGenerationsCandidats = new SortedList<int, Vector2Int>();

    readonly BlockType[,] board;
    BoardEventReceiver receiver;

    public BoardModel (int boardSize, BoardEventReceiver receiver) {
        board = new BlockType[boardSize, boardSize];
        this.receiver = receiver;
    }

    public void setCell (int x, int y, BlockType blockType) {
        board[x, y] = blockType;
    }

    public void setCellWithCollecting (int x, int y, BlockType blockType) {
        setCell(x, y, blockType);
        collect(blockType, new Vector2Int(x, y), Vector2Int.zero);
    }

    public BlockType getCell (int x, int y) {
        return board[x, y];
    }

    void resetBlock(Vector2Int position, BlockType blockType, Vector2Int offset) {
        setCellWithCollecting(position.x, position.y, blockType);
        receiver.resetBlock(position, blockType, offset);
    }

    public void collect (Vector2Int start, Vector2Int end) {
        collected.Clear();
        var startType = getCell(start.x, start.y);
        var endType = getCell(end.x, end.y);
        //As call resetBlock two time in a row,
        //need to set cell to prevent cheking wrong type in second call
        setCell(start.x, start.y, endType);
        setCell(end.x, end.y, startType);
        resetBlock(start, endType, end - start);
        resetBlock(end, startType, start - end);
        receiver.moveLenght = 1;
    }

    public int commitCollection () {
        int result = collected.Count;
        newGenerationsCandidats.Clear();
        var pushDownColunms = new HashSet<int>();
        foreach (var position in collected) {
            board[position.x, position.y] = BlockType.none;
            pushDownColunms.Add(position.x);
        }
        collected.Clear();
        pushDownAll(pushDownColunms);
        generateNew();
        return result;
    }

    void pushDownAll(HashSet<int> colunms) {
        var candidates = new List<PushDownData>();
        foreach(var x in colunms) {
            for (var y = 0; y < board.GetLength(0); y += 1) {
                if(getCell(x, y) == BlockType.none) {
                    var position = new Vector2Int(x, y);
                    var candidat = pushDown(position);
                    if (candidat.HasValue) {
                        candidates.Add(candidat.Value);
                    }
                }
            }
        }

        foreach (var candidat in candidates) {
            resetBlock(candidat.position, candidat.blockType, candidat.offset);
        }
    }

    PushDownData? pushDown (Vector2Int position) {
        
        for (
            var newPosition = position + Vector2Int.up;
            inBound(newPosition);
            newPosition += Vector2Int.up
        ) {
            
            var candidat = getCell(newPosition.x, newPosition.y);
            if (candidat != BlockType.none) {
                var result = new PushDownData();
                result.blockType = board[newPosition.x, newPosition.y];
                result.offset = newPosition - position;
                result.position = position;
                setCell(newPosition.x, newPosition.y, BlockType.none);
                return result;
            }
        }

        //keys must be unique and sorted in column, according to y
        var key = position.x * board.GetLength(0) + position.y;
        newGenerationsCandidats.Add(key, position);
        return null;
    }

    void generateNew() {
        //offsets is <column number: how many new blocks in this colunm will be>
        var offsets = new Dictionary<int, int>();
        var maxMoveLenght = 0;
        foreach(var data in newGenerationsCandidats) {
            var position = data.Value;
            if (offsets.ContainsKey(position.x)) {
                offsets[position.x] += 1;
            } else {
                offsets.Add(position.x, 0);
            }
            int offset = offsets[position.x] + board.GetLength(1) - position.y;
            if (offset > maxMoveLenght) {
                maxMoveLenght = offset;
            }

            var type = randomType();
            resetBlock(position, type, new Vector2Int(0, offset));
        }

        receiver.moveLenght = maxMoveLenght;
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
        return getCell(position.x, position.y) == blockType;
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

    BlockType randomType () {
        var blockTypesCount = System.Enum.GetValues(typeof(BlockType)).Length;
        //first element is skipped because it is "none"
        var blockTypesIndex = Random.Range(1, blockTypesCount);
        return (BlockType)blockTypesIndex;
    }
}

struct PushDownData {
    public Vector2Int position;
    public BlockType blockType;
    public Vector2Int offset;
}

public interface BoardEventReceiver {
    void resetBlock (Vector2Int position, BlockType type, Vector2Int offset);
    int moveLenght { set; }
}