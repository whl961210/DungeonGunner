using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Callbacks;
using UnityEditor;

public class RoomNodeGraphEditor : EditorWindow

{
    private GUIStyle roomNodeStyle;
    private GUIStyle roomNodeSelectedStyle;
    private static RoomNodeGraphSO currentRoomNodeGraph;
    private RoomNodeSO currentRoomNode = null;
    private RoomNodeTypeListSO roomNodeTypeList;
    private const float nodeWidth = 160f;
    private const float nodeHeight = 75f;
    private const int nodePadding = 25;
    private const int nodeBorder = 12;
    private const float connectingLineWidth = 3f;
    private const float connectingLineArrowSize = 6f;
    [MenuItem("Room Node Graph Editor", menuItem = "Window/Dungeon Editor/Room Node Graph Editor")]
    private static void OpenWindow()
    {
        GetWindow<RoomNodeGraphEditor>("Room Node Graph Editor");
    }
    private void OnEnable()
    {
        //Subscribe to the inspector selection changed event
        Selection.selectionChanged += InspectorSelectionChanged;

        //define the style for the room node
        roomNodeStyle = new GUIStyle();
        roomNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
        roomNodeStyle.normal.textColor = Color.white;
        roomNodeStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
        roomNodeStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);
        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;

        //define the style for the selected room node
        roomNodeSelectedStyle = new GUIStyle();
        roomNodeSelectedStyle.normal.background = EditorGUIUtility.Load("node1 on") as Texture2D;
        roomNodeSelectedStyle.normal.textColor = Color.white;
        roomNodeSelectedStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
        roomNodeSelectedStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);

    }
    private void OnDisable()
    {
        //Unsubscribe from the inspector selection changed event
        Selection.selectionChanged -= InspectorSelectionChanged;
    }
    [OnOpenAsset(0)]
    public static bool OnDoubleClickAsset(int instanceID, int line)
    {
        RoomNodeGraphSO roomNodeGraph = EditorUtility.InstanceIDToObject(instanceID) as RoomNodeGraphSO;
        if (roomNodeGraph != null)
        {
            OpenWindow();
            currentRoomNodeGraph = roomNodeGraph;
            return true;
        }
        return false;
    }
    private void OnGUI()
    {
        if (currentRoomNodeGraph == null)
        {
            EditorGUILayout.LabelField("No Room Node Graph selected.");
        }
        else
        {
            DrawDraggedLine();
            ProcessEvents(Event.current);
            DrawRoomConnections();
            DrawRoomNodes();
        }
        if (GUI.changed) Repaint();
    }

    private void DrawDraggedLine()
    {
        if (currentRoomNodeGraph.linePosition != Vector2.zero)
        {
            Handles.DrawBezier(currentRoomNodeGraph.roomNodeToDrawFrom.rect.center, currentRoomNodeGraph.linePosition,
            currentRoomNodeGraph.roomNodeToDrawFrom.rect.center, currentRoomNodeGraph.linePosition, Color.white, null,
            connectingLineWidth);
        }
    }
    private void ProcessEvents(Event currentEvent)
    {
        if (currentRoomNode == null || currentRoomNode.isLeftClickDragging == false)
        {
            currentRoomNode = IsMouseOverRoomNode(currentEvent);
        }
        if (currentRoomNode == null || currentRoomNodeGraph.roomNodeToDrawFrom != null)
        {
            ProcessRoomNodeGraphEvents(currentEvent);
        }
        else
        {
            currentRoomNode.ProcessEvents(currentEvent);
        }
    }
    private RoomNodeSO IsMouseOverRoomNode(Event currentEvent)
    {
        for (int i = currentRoomNodeGraph.roomNodeList.Count - 1; i >= 0; i--)
        {
            if (currentRoomNodeGraph.roomNodeList[i].rect.Contains(currentEvent.mousePosition))
            {
                return currentRoomNodeGraph.roomNodeList[i];
            }
        }
        return null;
    }
    private void ProcessRoomNodeGraphEvents(Event currentEvent)
    {
        switch (currentEvent.type)
        {
            case EventType.MouseDown:
                ProcessMouseDownEvent(currentEvent);
                break;
            case EventType.MouseUp:
                ProcessMouseUpEvent(currentEvent);
                break;
            case EventType.MouseDrag:
                ProcessMouseDragEvent(currentEvent);
                break;
            default:
                break;
        }
    }
    private void ProcessRoomNodeEvents(Event currentEvent)
    {
        switch (currentEvent.type)
        {
            case EventType.MouseDown:
                ProcessMouseDownEvent(currentEvent);
                break;
            default:
                break;
        }
    }
    private void ProcessMouseDownEvent(Event currentEvent)
    {
        if (currentEvent.button == 1)
        {
            ShowContextMenu(currentEvent.mousePosition);
        }
        //process left click down event
        else if (currentEvent.button == 0)
        {
            ClearLineDrag();
            ClearAllSelectedRoomNodes();
        }
    }
    private void ProcessMouseUpEvent(Event currentEvent)
    {
        if (currentEvent.button == 1 && currentRoomNodeGraph.roomNodeToDrawFrom != null)
        {
            RoomNodeSO roomNode = IsMouseOverRoomNode(currentEvent);
            if (roomNode != null)
            {
                if (currentRoomNodeGraph.roomNodeToDrawFrom.AddChildRoomNodeIDtToRoomNode(roomNode.id))
                {
                    roomNode.AddParentRoomNodeIDtToRoomNode(currentRoomNodeGraph.roomNodeToDrawFrom.id);
                }
            }
            ClearLineDrag();
        }
    }
    private void ProcessMouseDragEvent(Event currentEvent)
    {
        if (currentEvent.button == 1)
        {
            ProcessRightClickDragEvent(currentEvent);
        }
    }
    private void ProcessRightClickDragEvent(Event currentEvent)
    {
        if (currentRoomNodeGraph.roomNodeToDrawFrom != null)
        {
            DragConnectingLine(currentEvent.delta);
            GUI.changed = true;
        }
    }
    public void DragConnectingLine(Vector2 delta)
    {
        currentRoomNodeGraph.linePosition += delta;
    }
    private void ShowContextMenu(Vector2 mousePosition)
    {
        GenericMenu contextMenu = new GenericMenu();
        contextMenu.AddItem(new GUIContent("Add Room Node"), false, CreateRoomNode, mousePosition);
        contextMenu.AddSeparator("");
        contextMenu.AddItem(new GUIContent("Select All Room Nodes"), false, SelectAllRoomNodes);
        contextMenu.AddSeparator("");
        contextMenu.AddItem(new GUIContent("Delete Selected Room Nodes Links"), false, DeleteSelectedRoomNodesLinks);
        contextMenu.AddItem(new GUIContent("Delete Selected Room Nodes"), false, DeleteSelectedRoomNodes);
        contextMenu.ShowAsContext();
    }
    private void CreateRoomNode(object mousePositionObject)
    {
        if(currentRoomNodeGraph.roomNodeList.Count == 0)
        {
            CreateRoomNode(new Vector2(200f, 200f), roomNodeTypeList.list.Find(x => x.isEntrance));
        }
        CreateRoomNode(mousePositionObject, roomNodeTypeList.list.Find(x => x.isNone));
    }
    private void DeleteSelectedRoomNodesLinks()
    {
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
           if(roomNode.isSelected && roomNode.childRoomNodeIDList.Count > 0)
            {
                for(int i = roomNode.childRoomNodeIDList.Count - 1; i >= 0; i--)
                {
                    RoomNodeSO childRoomNode = currentRoomNodeGraph.GetRoomNode(roomNode.childRoomNodeIDList[i]);
                    if (childRoomNode != null && childRoomNode.isSelected)
                    {
                        roomNode.RemoveChildRoomNodeIDtFromRoomNode(childRoomNode.id);
                        childRoomNode.RemoveParentRoomNodeIDtFromRoomNode(roomNode.id);
                    }
                }
            }
        }
        ClearAllSelectedRoomNodes();
    }
    private void DeleteSelectedRoomNodes()
    {
        Queue<RoomNodeSO> roomNodeDeletionQueue = new Queue<RoomNodeSO>();
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            if (roomNode.isSelected && !roomNode.roomNodeType.isEntrance)
            {
                roomNodeDeletionQueue.Enqueue(roomNode);
                foreach (string childRoomNodeID in roomNode.childRoomNodeIDList)
                {
                    RoomNodeSO childRoomNode = currentRoomNodeGraph.GetRoomNode(childRoomNodeID);
                    if (childRoomNode != null)
                    {
                        childRoomNode.RemoveParentRoomNodeIDtFromRoomNode(roomNode.id);
                    }
                }
                foreach (string parentRoomNodeID in roomNode.parentRoomNodeIDList)
                {
                    RoomNodeSO parentRoomNode = currentRoomNodeGraph.GetRoomNode(parentRoomNodeID);
                    if (parentRoomNode != null)
                    {
                        parentRoomNode.RemoveChildRoomNodeIDtFromRoomNode(roomNode.id);
                    }
                }
            }
        }
        //Delete the room nodes in the queue
        while (roomNodeDeletionQueue.Count > 0)
        {
            RoomNodeSO roomNodeToDelete = roomNodeDeletionQueue.Dequeue();
            currentRoomNodeGraph.roomNodeDictionary.Remove(roomNodeToDelete.id);
            currentRoomNodeGraph.roomNodeList.Remove(roomNodeToDelete);
            DestroyImmediate(roomNodeToDelete, true);
            AssetDatabase.SaveAssets();
        }
    }
    private void CreateRoomNode(object mousePositionObject, RoomNodeTypeSO roomNodeType)
    {
        Vector2 mousePosition = (Vector2)mousePositionObject;
        RoomNodeSO roomNode = ScriptableObject.CreateInstance<RoomNodeSO>();
        currentRoomNodeGraph.roomNodeList.Add(roomNode);
        roomNode.Initialise(new Rect(mousePosition, new Vector2(nodeWidth, nodeHeight)), currentRoomNodeGraph, roomNodeType);
        AssetDatabase.AddObjectToAsset(roomNode, currentRoomNodeGraph);
        AssetDatabase.SaveAssets();
        currentRoomNodeGraph.OnValidate();
    }
    private void ClearAllSelectedRoomNodes()
    {
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            if (roomNode.isSelected)
            {
                roomNode.isSelected = false;
                GUI.changed = true;
            }
        }
    }
    private void SelectAllRoomNodes()
    {
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            roomNode.isSelected = true;
        }
        GUI.changed = true;
    }
    private void DrawRoomNodes()
    {
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            if (roomNode.isSelected)
            {
                roomNode.Draw(roomNodeSelectedStyle);
            }
            else
            roomNode.Draw(roomNodeStyle);
        }
        GUI.changed = true;
    }
    private void ClearLineDrag()
    {
        currentRoomNodeGraph.roomNodeToDrawFrom = null;
        currentRoomNodeGraph.linePosition = Vector2.zero;
        GUI.changed = true;
    }
    private void DrawRoomConnections()
    {
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            if (roomNode.childRoomNodeIDList.Count > 0)
            {
                foreach (string childRoomNodeID in roomNode.childRoomNodeIDList)
                {
                    if (currentRoomNodeGraph.roomNodeDictionary.ContainsKey(childRoomNodeID))
                    {
                        DrawConnectionLine(roomNode, currentRoomNodeGraph.roomNodeDictionary[childRoomNodeID]);
                        GUI.changed = true;
                    }
                }
            }
        }
    }

    private void DrawConnectionLine(RoomNodeSO roomNodeA, RoomNodeSO roomNodeB)
    {
        Vector2 startPosition = roomNodeA.rect.center;
        Vector2 endPosition = roomNodeB.rect.center;
        Vector2 midPosition = (startPosition + endPosition) / 2f;
        Vector2 direction = endPosition - startPosition;
        Vector2 arrowTailPoint1 = midPosition - new Vector2(-direction.y, direction.x).normalized * connectingLineArrowSize;
        Vector2 arrowTailPoint2 = midPosition + new Vector2(-direction.y, direction.x).normalized * connectingLineArrowSize;
        Vector2 arrowHeadPoint = midPosition + direction.normalized * connectingLineArrowSize;
        Handles.DrawBezier(arrowHeadPoint, arrowTailPoint1, arrowHeadPoint, arrowTailPoint1, Color.white, null, connectingLineWidth);
        Handles.DrawBezier(arrowHeadPoint, arrowTailPoint2, arrowHeadPoint, arrowTailPoint2, Color.white, null, connectingLineWidth);
        Handles.DrawBezier(startPosition, endPosition, startPosition, endPosition, Color.white, null, connectingLineWidth);
               

        GUI.changed = true;
    }
    private void InspectorSelectionChanged()
    {
        RoomNodeGraphSO roomNodeGraph = Selection.activeObject as RoomNodeGraphSO;
        if (roomNodeGraph != null)
        {
            currentRoomNodeGraph = roomNodeGraph;
            GUI.changed = true;
        }
    }
}
