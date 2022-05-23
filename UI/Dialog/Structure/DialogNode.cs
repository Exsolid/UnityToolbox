using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;

[System.Serializable, NodeMenuItem("Custom/DialogNode")]
public class DialogNode : BaseNode
{
	public string title;
	public string description;
	public string soundToPlay;

	public List<string> options;

	[Output(name = "Next Node")]
	public float next;

	public override string		name => "DialogNode";

	protected override void Process()
	{
	}
}
