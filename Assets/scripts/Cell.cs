using UnityEngine;
using System.Collections;

public class Cell : MonoBehaviour, IExplodable
{
	private Chip _chip = null;
	private CellBlocker _blocker = null;
	private Callback _explodeCallback = null;

    /** Координата x клетки.*/
    public int x;
    /** Координата y клетки.*/
    public int y;

	// =======
	public void initialize(CellBlocker blocker, Chip chip)
	{
		_blocker = blocker;
		_chip    = chip;
	}

	public bool canLeave()
	{
		return _blocker.canLeave();
	}
	
	public bool canEnter()
	{
		return _blocker.canEnter();
	}
	
	public bool canPass()
    {
		return _blocker.canPass();
    }

	public bool explode(Callback callback)
	{
		_explodeCallback = callback;

		Callback tmpCallback = _onExplodeComplete;

		// Защищена ли фишка
		bool chipProtected   = false;

		// 
		bool blockerExploded = false;

        if (_blocker.explode(tmpCallback)) {
			blockerExploded = true;

			if (_blocker.isProtecting()) {
				chipProtected = true;
			}

			if (_blocker.hasNext()) {
				BlockerType nextBlockerType = _blocker.getNext();
				
				Destroy(_blocker.gameObject);
				
				try {
					_blocker = BlockFactory.createNew(nextBlockerType, gameObject);
                } catch (System.Exception e) {
                    Debug.LogError(e.Message);
				}
			}
		}

		if (!chipProtected) {
			if (_chip == null || !_chip.explode(tmpCallback)) {
				return blockerExploded;
			}

			Destroy(_chip.gameObject);

			_chip = null;
        }

		return true;
	}

    private void _onExplodeComplete()
	{
		if (_explodeCallback != null) {
			_explodeCallback();
		}
	}

	public Chip getChip ()
	{
		return _chip;
	}
	

}
