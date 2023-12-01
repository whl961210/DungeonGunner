using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomNodeGraph_", menuName = "ScriptableObjects/Dungeon/RoomNodeGraph")]
public class RoomNodeGraphSO : ScriptableObject
{
    [HideInInspector] public RoomNodeTypeListSO roomNodeTypeList;
    [HideInInspector] public List<RoomNodeSO> roomNodeList = new List<RoomNodeSO>();
    [HideInInspector] public Dictionary<string, RoomNodeSO> roomNodeDictionary = new Dictionary<string, RoomNodeSO>();
    private void Awake()
    {
        LoadRoomNodeDictionary();
    }
    private void LoadRoomNodeDictionary()
    {
        roomNodeDictionary.Clear();
        foreach (RoomNodeSO roomNode in roomNodeList)
        {
            roomNodeDictionary[roomNode.id] = roomNode;
        }
    }
    #region Editor code
#if UNITY_EDITOR
    [HideInInspector] public RoomNodeSO roomNodeToDrawFrom = null;
    [HideInInspector] public Vector2 linePosition;

    public void OnValidate()
    {
        LoadRoomNodeDictionary();
    }

    public void SetNodeToDrawConnectionFrom(RoomNodeSO node, Vector2 position)
    {
        roomNodeToDrawFrom = node;
        linePosition = position;
    }
#endif

    #endregion Editor code

}
