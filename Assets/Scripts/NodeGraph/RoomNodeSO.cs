using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;

public class RoomNodeSO : ScriptableObject
{
    [HideInInspector] public string id;
    [HideInInspector] public List<string> parentRoomNodeIDList = new List<string>();
    [HideInInspector] public List<string> childRoomNodeIDList = new List<string>();
    [HideInInspector] public RoomNodeGraphSO roomNodeGraph;
    public RoomNodeTypeSO roomNodeType;
    [HideInInspector] public RoomNodeTypeListSO roomNodeTypeList;
    #region Editor code
#if UNITY_EDITOR
    [HideInInspector] public Rect rect;
    [HideInInspector] public bool isLeftClickDragging = false;
    [HideInInspector] public bool isSelected = false;
    public void Initialise(Rect rect, RoomNodeGraphSO roomNodeGraph, RoomNodeTypeSO roomNodeType)
    {
        this.rect = rect;
        this.id = Guid.NewGuid().ToString();
        this.name = "RoomNode";
        this.roomNodeGraph = roomNodeGraph;
        this.roomNodeType = roomNodeType;

        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
    }
    public void Draw(GUIStyle nodeStyle)
    {
        GUILayout.BeginArea(rect, nodeStyle);
        EditorGUI.BeginChangeCheck();
        //if the room node has a parent or is of type entrance then display a label else display a popup
        if(parentRoomNodeIDList.Count > 0 || roomNodeType.isEntrance)
        {
            //display a label that cannot be changed
            EditorGUILayout.LabelField(roomNodeType.roomNodeTypeName);
        }
        else
        {
            //display a popup using the RoomNodeType name values that can be selected from
            int selected = roomNodeTypeList.list.FindIndex(x => x == roomNodeType);
            int selection = EditorGUILayout.Popup("", selected, GetRoomNodeTypesToDisplay());
            roomNodeType = roomNodeTypeList.list[selection];
        }
        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(this);
        }
        GUILayout.EndArea();
    }
    public string[] GetRoomNodeTypesToDisplay()
    {
        string[] roomArray = new string[roomNodeTypeList.list.Count];
        for (int i = 0; i < roomNodeTypeList.list.Count; i++)
        {
            if (roomNodeTypeList.list[i].displayInNodeGraphEditor)
            {
                roomArray[i] = roomNodeTypeList.list[i].roomNodeTypeName;
            }
        }
        return roomArray;
    }
    public void ProcessEvents(Event currentEvent)
{
    switch (currentEvent.type)
    {
        // Process mouse down event
        case EventType.MouseDown:
            ProcessMouseDownEvent(currentEvent);
            break;

        // Process mouse drag event
        case EventType.MouseDrag:
            ProcessMouseDragEvent(currentEvent);
            break;

        // Process mouse up event
        case EventType.MouseUp:
            ProcessMouseUpEvent(currentEvent);
            break;
        
        // Default case
        default:
            break;
    }
}

    private void ProcessMouseDownEvent(Event currentEvent)
    {
        if (currentEvent.button == 0)
        {
            ProcessLeftClickDownEvent();
        }
        if (currentEvent.button == 1)
        {
            ProcessRightClickDownEvent(currentEvent);
        }
    }
    private void ProcessLeftClickDownEvent()
    {
        Selection.activeObject = this;
        if (isSelected == true)
        {
            isSelected = false;
        }
        else
        {
            isSelected = true;
        }
    }
    private void ProcessRightClickDownEvent(Event currentEvent)
    {
        roomNodeGraph.SetNodeToDrawConnectionFrom(this, currentEvent.mousePosition);
    }
    
    private void ProcessMouseUpEvent(Event currentEvent)
    {
        if (currentEvent.button == 0)
        {
            ProcessLeftClickUpEvent();
        }
    }
    private void ProcessLeftClickUpEvent()
    {
        if (isLeftClickDragging == true)
        {
            isLeftClickDragging = false;
        }
    }
    private void ProcessMouseDragEvent(Event currentEvent)
    {
        if (currentEvent.button == 0)
        {
            ProcessLeftClickDragEvent(currentEvent);
        }
    }
    private void ProcessLeftClickDragEvent(Event currentEvent)
    {
        isLeftClickDragging = true;
        DragNode(currentEvent.delta);
        GUI.changed = true;
    }
    private void DragNode(Vector2 delta)
    {
        rect.position += delta;
        EditorUtility.SetDirty(this);
    }

    public bool AddChildRoomNodeIDtToRoomNode(string childRoomNodeID)
    {
        //check if child node can be added validly to parent node
        if(IsChildRoomValid(childRoomNodeID))
        {
            childRoomNodeIDList.Add(childRoomNodeID);
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool IsChildRoomValid(string childRoomNodeID)
    {
        bool isConnectedBossNodeAlready = false;
        //check if there is already a boss node connected to this node
        foreach (RoomNodeSO roomNode in roomNodeGraph.roomNodeList)
        {
            if (roomNode.roomNodeType.isBossRoom && roomNode.parentRoomNodeIDList.Count > 0)
            {
                isConnectedBossNodeAlready = true;
            }
        }
        // if the child node has a type of boss room and there is already a boss room connected to this node then return false
        if (roomNodeGraph.GetRoomNode(childRoomNodeID).roomNodeType.isBossRoom && isConnectedBossNodeAlready)
        {
            return false;
        }
        // if the child node has a type of none then return false
        if (roomNodeGraph.GetRoomNode(childRoomNodeID).roomNodeType.isNone)
        {
            return false;
        }
        // if the child node has a child with this child id then return false
        if(childRoomNodeIDList.Contains(childRoomNodeID))
        {
            return false;
        }
        // if this room node ID and the child room node ID are the same then return false
        if(id == childRoomNodeID)
        {
            return false;
        }
        // if the child node has a parent with this room node id then return false
        if(parentRoomNodeIDList.Contains(childRoomNodeID))
        {
            return false;
        }
        // if the child node already has a parent then return false
        if(roomNodeGraph.GetRoomNode(childRoomNodeID).parentRoomNodeIDList.Count > 0)
        {
            return false;
        }
        //if child is a corridor and this room node is a corridor then return false
        if(roomNodeGraph.GetRoomNode(childRoomNodeID).roomNodeType.isCorridor && roomNodeType.isCorridor)
        {
            return false;
        }
        // if child is not a corridor and this node is not a corridor then return false
        if(!roomNodeGraph.GetRoomNode(childRoomNodeID).roomNodeType.isCorridor && !roomNodeType.isCorridor)
        {
            return false;
        }
        // if adding a corridor check that this node has less than the max number of corridors
        if(roomNodeGraph.GetRoomNode(childRoomNodeID).roomNodeType.isCorridor && childRoomNodeIDList.Count >= Settings.maxChildCorridors)
        {
            return false;
        }
        // if the child room is an entrance return false, the entrance must be the top level node
        if(roomNodeGraph.GetRoomNode(childRoomNodeID).roomNodeType.isEntrance)
        {
            return false;
        }
        // if adding a room to a corridor check that the corridor is not already connected to a room
        if(!roomNodeGraph.GetRoomNode(childRoomNodeID).roomNodeType.isCorridor && childRoomNodeIDList.Count > 0)
        {
            return false;
        }
        return true;

    }
    public bool AddParentRoomNodeIDtToRoomNode(string parentRoomNodeID)
    {
        parentRoomNodeIDList.Add(parentRoomNodeID);
        return true;
    }
#endif

    #endregion Editor code
}
