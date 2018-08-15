using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour {

    public BlockData data {
        set {
            GetComponent<SpriteRenderer>().color = value.color;
        }
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

public class BlockData {

    static public BlockData getRandom () {
        
        var blockTypesCount = System.Enum.GetValues(typeof(BlockType)).Length;
        var blockTypesIndex = Random.Range(0, blockTypesCount);
        switch ((BlockType)blockTypesIndex) {
            case BlockType.red: return red;
            case BlockType.green: return green;
            case BlockType.blue: return blue;
            default: throw new KeyNotFoundException();
        }
    }

    public enum BlockType { red, green, blue };

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
    BlockType type;

    BlockData (BlockType type, Color color) {
        this.type = type;
        this.color = color;
    }
}