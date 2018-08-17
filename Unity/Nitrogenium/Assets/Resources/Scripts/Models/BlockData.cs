using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BlockType { none, red, green, blue };

public class BlockData {

    static public BlockData getRandom () {

        var blockTypesCount = System.Enum.GetValues(typeof(BlockType)).Length;
        var blockTypesIndex = Random.Range(1, blockTypesCount);
        switch ((BlockType)blockTypesIndex) {
            case BlockType.red: return red;
            case BlockType.green: return green;
            case BlockType.blue: return blue;
            default: throw new KeyNotFoundException();
        }
    }

    static public BlockData red = new BlockData(
        BlockType.red,
        new Color(255, 0, 0)
    );
    static public BlockData green = new BlockData(
        BlockType.green,
        new Color(0, 255, 0)
    );
    static public BlockData blue = new BlockData(
        BlockType.blue,
        new Color(0, 0, 255)
    );

    readonly public Color color;
    readonly public BlockType type;

    BlockData (BlockType type, Color color) {
        this.type = type;
        this.color = color;
    }
}
