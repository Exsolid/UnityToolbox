using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.IO;
using System.Linq;
using UnityToolbox.General.Preferences;

namespace UnityToolbox.UI.Dialog.Editor
{
    /// <summary>
    /// The dialog graph used within a window.
    /// </summary>
    public class DialogGraphView : GraphView
    {
        private Vector2 _mousePos;

        private HashSet<int> _ids;
        public HashSet<int> IDS
        {
            get { return _ids.ToHashSet(); }
        }

        public DialogGraphView()
        {
            AddGrid();
            AddStyle();
            AddManipulators();
            _ids = new HashSet<int>();
            this.graphViewChanged += OnGraphChange;
        }

        private DialogNode CreateNode(Vector2 position)
        {
            do
            {
            } while (_ids.Add(UnityEngine.Random.Range(0, Int32.MaxValue)));

            DialogNode dialogBaseNode = new DialogNode(position, _ids.ElementAt(_ids.Count - 1));
            dialogBaseNode.Draw();

            return dialogBaseNode;
        }

        /// <summary>
        /// Adds a new <see cref="DialogNode"/> to the graph and draws it.
        /// </summary>
        /// <param name="node"></param>
        public void AddNode(DialogNode node)
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
                e => e.menu.AppendAction("Add Node", action => AddElement(CreateNode((action.eventInfo.mousePosition - new Vector2(viewTransform.position.x, viewTransform.position.y)) / scale))));
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
                if (startPort == port)
                {
                    return;
                }

                if (startPort.node == port.node)
                {
                    return;
                }

                if (startPort.direction == port.direction)
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
            if (paths.Length > 0)
            {
                return paths[0].Replace(Application.dataPath, "Assets/");
            }

            return "";
        }

        private GraphViewChange OnGraphChange(GraphViewChange change)
        {
            if (change.elementsToRemove != null)
            {
                foreach (GraphElement e in change.elementsToRemove)
                {
                    if (e.GetType() == typeof(Edge))
                    {
                        Edge edge = (Edge)e;
                        ((DialogNode)edge.output.node).UpdateValues();

                        int id = ((DialogNode)edge.output.node).OutputIDs.IndexOf(((DialogNode)edge.input.node).ID);
                        foreach (Edge otherEdges in edge.output.connections)
                        {
                            ((DialogNode)otherEdges.input.node).UpdateHigherInputConnectionLabel(id);
                        }
                    }
                }
            }

            return change;
        }
    }

}