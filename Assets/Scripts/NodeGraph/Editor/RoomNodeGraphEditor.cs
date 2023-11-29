using UnityEngine;
using UnittEditor.Callbacks;
using UnityEditor;

public class RoomNodeGraphEditor : EditorWindow

{
    private GUIStyle roomNodeStyle;
    private static RoomNodeGrapgSO currentRoomNodeGraph;
    private RoomNodeTypesListSO roomNodeTypesList;
    private const float nodeWidth = 160f;
    private const float nodeHeight = 75f;
    private const int nodePadding = 25;
    private const int nodeBorder = 12;
    [MenuItem("Room Node Graph Editor", menuItem = "Window/Dungeon Editor/Room Node Graph Editor")]
    private static void OpenWindow()
    {
        GetWindow<RoomNodeGraphEditor>("Room Node Graph Editor");
    }
    private void OnEnable()
    {
        roomNodeStyle = new GUIStyle();
        roomNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
        roomNodeStyle.normal.textColor = Color.white;
        roomNodeStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
        roomNodeStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);  
        roomNodeTypesList = GameResources.Instance.roomNodeTypeList;
    }
    [OnOpenAsset(0)]
    public static bool OnDoubleClickAsset(int instanceID, int line)
    {
        RoomNodeGrapgSo roomNodeGraph = EditorUtility.InstanceIDToObject(instanceID) as RoomNodeGrapgSo;
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
            ProcessNodeEvents(Event.current);
            DrawRoomNodes();
        }
        if (GUI.changed) Repaint();
    }
    private void ProcessEvents(Event currentEvent)
    {
        ProcessRoomNodeEvents(currentEvent);
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
        if(currentEvent.button == 1)
        {
            ShowContextMenu(currentEvent.mousePosition);
        }
    }
    private void ShowContextMenu(Vector2 mousePosition)
    {
        GenericMenu contextMenu = new GenericMenu();
        contextMenu.AddItem(new GUIContent("Add Room Node"), false, CreateRoomNode, mousePosition);
        contextMenu.ShowAsContext();
    }
    private void CreateRoomNode(object mousePositionObject)
    {
       CreateRoomNode(mousePositionObject, roomNodeTypesList.list.Find(x => x.isNone));
    }
    private void CreateRoomNode(object mousePositionObject, RoomNodeTypeSO roomNodeType)
    {
        Vector2 mousePosition = (Vector2)mousePositionObject;
        RoomNodeSO roomNode = ScriptableObject.CreateInstance<RoomNodeSO>();
        currentRoomNodeGraph.roomNodeList.Add(roomNode);
        roomNode.Initialise(new Rect(moustPosition, new Vector2(nodeWidth, nodeHeight)), currentRoomNodeGraph,roomNodeType);
        AssetDatabase.AddObjectToAsset(roomNode, currentRoomNodeGraph);
        AssetDatabase.SaveAssets();
    }
}
