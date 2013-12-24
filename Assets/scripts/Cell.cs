using UnityEngine;
using System.Collections;

public class Cell : MonoBehaviour, IExplodable
{
	public Chip chip;
	public CellBlocker blocker;
	private Callback _explodeCallback = null;

	// =======
	public void initialize(CellBlocker blocker, Chip chip)
	{
		this.blocker = blocker;
		this.chip    = chip;
	}

	public bool canLeave()
	{
		return blocker.canLeave();
	}
	
	public bool canEnter()
	{
		return blocker.canEnter();
	}
	
	public bool canPass()
    {
		return blocker.canPass();
    }

	public bool explode(Callback callback)
	{
		_explodeCallback = callback;

		Callback tmpCallback = _onExplodeComplete;

		// Защищена ли фишка
		bool chipProtected   = false;

		// 
		bool blockerExploded = false;

        if (blocker.explode(tmpCallback)) {
			blockerExploded = true;

			if (blocker.isProtecting()) {
				chipProtected = true;
			}

			if (blocker.hasNext()) {
				BlockerType nextBlockerType = blocker.getNext();
				
				Destroy(blocker.gameObject);
				
				try {
					blocker = BlockFactory.createNew(nextBlockerType, gameObject);
                } catch (System.Exception e) {
                    Debug.LogError(e.Message);
				}
			}
		}

		if (!chipProtected) {
			if (chip == null || !chip.explode(tmpCallback)) {
				return blockerExploded;
			}

			Destroy(chip.gameObject);

			chip = null;
        }

		return true;
	}

    private void _onExplodeComplete()
	{
		if (_explodeCallback != null) {
			_explodeCallback();
		}
	}
}
