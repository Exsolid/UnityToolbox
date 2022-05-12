using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPreferencesManager : Module
{
    [SerializeField] private string keyword = "";
    public string Keyword { get { return keyword + "_"; } }
}
