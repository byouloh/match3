using UnityEngine;
using System.Collections;

public class LevelNumberText : MonoBehaviour {
	public int levelNumber;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		GetComponent<TextMesh>().text = "Level: " + levelNumber;
	}
}
