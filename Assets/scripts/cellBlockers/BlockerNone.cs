using UnityEngine;
using System.Collections;

public class BlockerNone : CellBlocker
{
	public override bool canLeave()
	{
		return true;
	}

	public override bool canEnter()
	{
		return true;
	}

	public override bool canPass()
	{
		return true;
	}

	public override bool isProtecting()
	{
		return false;
	}

}
