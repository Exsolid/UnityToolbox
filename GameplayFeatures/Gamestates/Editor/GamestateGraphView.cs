using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.IO;
using System.Linq;

/// <summary>
/// The gamestate graph used within a window.
/// </summary>
public class GamestateGraphView : GraphView
{
    private HashSet<int> _ids;
    public HashSet<int> IDS
    {
        get { return _ids.ToHashSet(); }
    }

    public GamestateGraphView()
    {
        AddGrid();
        AddStyle();
        AddManipulators();
        _ids = new HashSet<int>();
    }

    private GamestateNode CreateNode(Vector2 position)
    {
        do
        {
        } while (_ids.Add(UnityEngine.Random.Range(0, Int32.MaxValue)));

        GamestateNode gamestateBaseNode = new GamestateNode(position, _ids.ElementAt(_ids.Count - 1));
        gamestateBaseNode.Draw();

        return gamestateBaseNode;
    }

    /// <summary>
    /// Adds a new <see cref="GamestateNode"/> to the graph and draws it.
    /// </summary>
    /// <param name="node"></param>
    public void AddNode(GamestateNode node)
    {
        _ids.Add(node.ID);
        AddElement(node);
        node.Draw();
    }

    private void AddGrid()
    {
        GridBackground background = new GridBackground();

        background.StretchToParentSize();
        Insert(0, background);
    }

    private void AddStyle()
    {
        StyleSheet style = AssetDatabase.LoadAssetAtPath<StyleSheet>(GetPathOfStyle());
        styleSheets.Add(style);
    }

    private void AddManipulators()
    {
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(CreateNodeManipulator());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
    }

    private IManipulator CreateNodeManipulator()
    {
        ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
            e => e.menu.AppendAction("Add Node", action => AddElement(CreateNode(action.eventInfo.localMousePosition)))
            );
        return contextualMenuManipulator;
    }

    private Group CreateGroup(string title, Vector2 localMousePosition)
    {
        Group group = new Group()
        {
            title = title,
        };
        group.SetPosition(new Rect(localMousePosition, Vector2.zero));
        return group;
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        List<Port> compPorts = new List<Port>();

        ports.ForEach(port =>
        {
            if(startPort == port)
            {
                return;
            }

            if(startPort.node == port.node)
            {
                return;
            }

            if(startPort.direction == port.direction)
            {
                return;
            }

            compPorts.Add(port);
        });

        return compPorts;
    }

    private string GetPathOfStyle()
    {
        string[] paths = Directory.GetFiles(Application.dataPath + ProjectPrefs.GetString(ProjectPrefKeys.UNITYTOOLBOXPATH), "DialogGraphViewStyle.uss", SearchOption.AllDirectories);
        if(paths.Length > 0)
        {
            return paths[0].Replace(Application.dataPath, "Assets/");
        }

        return "";
    }
}
