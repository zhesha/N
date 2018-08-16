using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block: MonoBehaviour {

    public BlockData data {
        set {
            GetComponent<SpriteRenderer>().color = value.color;
        }
    }
}