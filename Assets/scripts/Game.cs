using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Game. Главный класс игры в нем создается клетки.
/// </summary>
public class Game : MonoBehaviour
{
	public float CELL_WIDTH = 0.5f;
	public float CELL_HEIGHT = 0.5f;

	public GameObject cellPrefab;
	public GameObject cellsRoot;
	public List<List <Cell>> grid;

	void Start()
	{
		CELL_WIDTH = 0.5f;
		CELL_HEIGHT = 0.5f;
		grid = new List<List <Cell>>();

		int i, j;

		for (i=0; i < 5; i++) {
			List <Cell> row = new List<Cell>();
			for (j=0; j<5; j++) {
				GameObject cell = (GameObject)Instantiate(cellPrefab);
				cell.transform.parent = cellsRoot.transform;
				cell.transform.position = new Vector3(CELL_WIDTH * i - 1.4f, CELL_WIDTH * j - 1.4f, 0);
				
				Cell c = cell.GetComponent<Cell>();
				
				CellBlocker blocker = BlockFactory.createNew(BlockerType.CHAIN, c.gameObject);
				Chip chip = ChipFactory.createNew(ChipType.BLUE, BonusType.SAME_TYPE, c.gameObject);
				c.initialize(blocker, chip);
				
				row.Add(c);
			}
			grid.Add(row);


		}
	}
	
	void Update()
	{
		//if (Input.GetMouseButtonDown(0) && 25 > 0) {
		//	if (!grid[0][0].explode(null)) {
		//		grid[][].RemoveAt(0);
		//	}
		//}
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
