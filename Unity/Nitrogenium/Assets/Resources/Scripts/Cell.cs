using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour {

	// Use this for initialization
	void Start () {
        var blockPrefab = Resources.Load<GameObject>("Prefabs/Block");
        var blockInstance = Instantiate(blockPrefab, transform.position, Quaternion.identity);
        blockInstance.transform.parent = transform;
        blockInstance.GetComponent<Block>().data = BlockData.getRandom();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	
}