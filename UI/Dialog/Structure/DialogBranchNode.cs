using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;

[System.Serializable, NodeMenuItem("Custom/DialogBranchNode")]
public class DialogBranchNode : DialogNode
{
	[Input(name = "Previos Node")]
	public float Previous;

	public override string Name => "DialogBranchNode";

	protected override void Process()
	{
	}
}
