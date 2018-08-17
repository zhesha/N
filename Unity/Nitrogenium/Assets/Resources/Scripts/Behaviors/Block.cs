using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block: MonoBehaviour {

    const float movementEpsilon = 0.1f;

    public Vector3? destination;

    BlockData _data;
    public BlockData data {
        set {
            _data = value;
            GetComponent<SpriteRenderer>().color = value.color;
        }
        get {
            return _data;
        }
    }

    public Vector3 cellPosition {
        get {
            return transform.parent.position;
        }
    }

    void Update () {
        if (destination.HasValue) {
            moveToDestination();
        } else {
            moveToCell();
        }
    }

    void moveToDestination () {
        var distance = (transform.position - destination.Value).magnitude;
        if (distance > movementEpsilon) {
            transform.position = Vector3.MoveTowards(
                transform.position,
                destination.Value,
                Time.deltaTime * GameSetup.animationSpeed
            );
        } else {
            destination = null;
        }
    }

    void moveToCell () {
        var distance = (transform.position - cellPosition).magnitude;
        if (distance > movementEpsilon) {
            transform.position = Vector3.MoveTowards(
                transform.position,
                cellPosition,
                Time.deltaTime * GameSetup.animationSpeed
            );
        } else {
            transform.position = cellPosition;
        }
    }
}