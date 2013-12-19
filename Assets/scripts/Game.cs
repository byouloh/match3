using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Game : MonoBehaviour
{
	public float CELL_WIDTH = 1;
	public float CELL_HEIGHT = 1;

	public GameObject cellPrefab;
	public GameObject cellsRoot;
	public List<Cell> cells;

	void Start()
	{
		cells = new List<Cell>();

		int i;
		for (i=0; i < 5; i++) {
			GameObject cell = (GameObject)Instantiate(cellPrefab);
			cell.transform.parent = cellsRoot.transform;
			cell.transform.position = new Vector3(CELL_WIDTH * i, 0, 0);

			Cell c = cell.GetComponent<Cell>();

			CellBlocker blocker = BlockFactory.createNew(BlockerType.CHAIN, c.gameObject);
			Chip chip = ChipFactory.createNew(ChipType.BLUE, BonusType.SAME_TYPE, c.gameObject);
			c.initialize(blocker, chip);

			cells.Add(c);
		}
	}
	
	void Update()
	{
		if (Input.GetMouseButtonDown(0) && cells.Count > 0) {
			if (!cells[0].explode(null)) {
				cells.RemoveAt(0);
			}
		}
	}

	void OnGUI()
	{
		/*if (GUI.Button(new Rect(5,5, 100, 20), "create")) {
			Vector3 pos = new Vector3(Random.Range(-5, 5), Random.Range(-5, 5), 0);
			GameObject explosion = (GameObject) Instantiate(Resources.Load<GameObject>("prefabs/explosionAnim"), pos, Quaternion.identity);

			//explosion.GetComponent<Animator>().Play("explosionAnim");
			//explosion.GetComponent<Animator>()
		}*/
	}
}
