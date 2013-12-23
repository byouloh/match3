using UnityEngine;
using System.Collections;

public class BlockerWrap2 : CellBlocker
{
	public override bool canLeave()
	{
		return false;
	}
	
	public override bool canEnter()
	{
		return false;
	}
	
	public override bool canPass()
	{
		return false;
	}

	public override bool hasNext()
	{
		return true;
	}

	public override bool isProtecting()
	{
		return false;
	}

	public override BlockerType getNext()
	{
		return BlockerType.WRAP;
	}
}
