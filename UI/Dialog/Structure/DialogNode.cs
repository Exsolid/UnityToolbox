using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;

[System.Serializable, NodeMenuItem("Custom/DialogNode")]
public class DialogNode : BaseNode
{
	public string Title;
	public string Description;
	public string SoundToPlay;
	public string CompletionToSet;

	public List<string> Options;

	[Output(name = "Next Node")]
	public float next;

	public override string		name => "DialogNode";

	protected override void Process()
	{
	}
}
