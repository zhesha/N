using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell: MonoBehaviour {

    public void setCursor () {
        GetComponent<SpriteRenderer>().enabled = true;
    }

    public void resetCursor () {
        GetComponent<SpriteRenderer>().enabled = false;
    }

    public void collect () {
        if (transform.childCount > 0) {
            Destroy(transform.GetChild(0).gameObject);
        }
    }

    public void setUp (BlockData blockData) {
        var blockPrefab = Resources.Load<GameObject>("Prefabs/Block");
        var blockInstance = Instantiate(
            blockPrefab,
            transform.position,
            Quaternion.identity
        );
        blockInstance.transform.parent = transform;
        blockInstance.GetComponent<Block>().data = blockData;

    }
}