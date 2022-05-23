using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;

[System.Serializable, NodeMenuItem("Custom/DialogRootNode")]
public class DialogRootNode : DialogNode
{
	public string referenceID;

	private float selectedOption = -1;
	public float SelectedOption { get { return selectedOption; } set { selectedOption = value; } }

	public override string		name => "DialogRootNode";

	protected override void Process()
	{
	}
}
