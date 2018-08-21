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

    BlockData blockData {
        set {
            _block.GetComponent<Block>().data = value;
        }
    }

    Vector3 blockPosition {
        set {
            _block.transform.position = value;
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

    public void setUpBlock (BlockData data, Vector2Int? offset = null) {
        Vector3 position;
        if (offset.HasValue) {
            position = new Vector3(
                transform.position.x + offset.Value.x,
                transform.position.y + offset.Value.y,
                0
            );
        } else {
            position = transform.position;
        }

        blockData = data;
        blockPosition = position;
    }

    public void fakeSwitchAnimation (Cell destination) {
        _block.GetComponent<Block>().destination = destination.transform.position;
    }
}