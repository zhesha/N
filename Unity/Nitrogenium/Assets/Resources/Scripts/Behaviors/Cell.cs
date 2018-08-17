using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell: MonoBehaviour {

    static GameObject _prefab;
    GameObject prefab {
        get {
            if (_prefab == null) {
                _prefab = Resources.Load<GameObject>("Prefabs/Block");
            }
            return _prefab;
        }
    }

    public bool cursorSeted {
        set {
            GetComponent<SpriteRenderer>().enabled = value;
        }
    }

    GameObject _block;
    GameObject block {
        get {
            return _block;
        }
    }

    BlockData blockData {
        get {
            return block.GetComponent<Block>().data;
        }
        set {
            block.GetComponent<Block>().data = value;
        }
    }

    Vector3 blockPosition {
        get {
            return block.transform.position;
        }
        set {
            block.transform.position = value;
        }
    }

    public void setUp (BlockData blockData) {
        _block = Instantiate(
            prefab,
            transform.position,
            Quaternion.identity
        );
        _block.transform.parent = transform;
        setUpBlock(blockData);
    }

    public void setUpBlock (BlockData data, float? y = null) {
        Vector3 position;
        if (y.HasValue) {
            position = new Vector3(transform.position.x, y.Value, 0);
        } else {
            position = transform.position;
        }

        blockData = data;
        blockPosition = position;
    }

    public void fakeSwitchAnimation (Cell destination) {
        _block.GetComponent<Block>().destination = destination.transform.position;
    }

    public void swap (Cell other) {
        var thisBlock = blockData;
        var otherBlock = other.blockData;
        blockData = otherBlock;
        other.blockData = thisBlock;
        blockPosition = other.transform.position;
        other.blockPosition = transform.position;
    }

    public void move (Cell from) {
        blockData = from.blockData;
        blockPosition = from.transform.position;
	}
}