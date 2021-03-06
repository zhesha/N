﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BlockType { none, red, green, blue, cyan, magenta, yellow };

public class BlockData {

    static public BlockData getFromType (BlockType type) {
        Color color;
        switch (type) {
            case BlockType.red:
                color = new Color(255, 0, 0);
                break;
            case BlockType.green: 
                color = new Color(0, 255, 0);
                break;
            case BlockType.blue: 
                color = new Color(0, 0, 255);
                break;
            case BlockType.cyan: 
                color = new Color(0, 255, 255);
                break;
            case BlockType.magenta: 
                color = new Color(255, 0, 255);
                break;
            case BlockType.yellow: 
                color = new Color(255, 255, 0);
                break;
            default: throw new KeyNotFoundException();
        }

        return new BlockData(type, color);
    }

    readonly public Color color;
    readonly public BlockType type;

    BlockData (BlockType type, Color color) {
        this.type = type;
        this.color = color;
    }
}
