using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Room_", menuName = "ScriptableObjects/Dungeon/Room")]
public class RoomTemplateSO : ScriptableObject
{
    [HideInInspector] public string guid;
    #region Header
    [Space(10)]
    [Header("ROOM PREFAB")]
    #endregion Header ROOM PREFAB
    #region Tooltip
    [Tooltip("The gameobject prefab for this room (it will contain all the tilemaps for the room and environment)")]
    #endregion Tooltip
    public GameObject prefab;
    [HideInInspector] public GameObject previousPrefab;//used to regenerate the guid
    #region Header ROOM CONFIGURATION
    [Space(10)]
    [Header("ROOM CONFIGURATION")]
    #endregion Header ROOM CONFIGURATION
    #region Tooltip
    [Tooltip("The room node type SO. The room node types correspond to the room nodes used in the room node graph. The exceptions being with corridors. In the room node graph, there is just one corridor type. For the room templates, there are 2 corridor types (one for CorridorNS and CorridorEW).")]
    #endregion Tooltip
    public RoomNodeTypeSO roomNodeType;
    #region Tooltip
    [Tooltip("If you imagine a rectangle around the tilemap that just completely encloses the tilemap, the room lower bounds represent the bottom left corner of that rectangle. This should be determined from the tilemap for the room (using the coordinate brush pointer to get the tilemap grid position for that bottom left corner).")]
    #endregion Tooltip
    public Vector2Int lowerBounds;
    #region Tooltip
    [Tooltip("If you imagine a rectangle around the tilemap that just completely encloses the tilemap, the room upper bounds represent the top right corner of that rectangle. This should be determined from the tilemap for the room (using the coordinate brush pointer to get the tilemap grid position for that top right corner).")]
    #endregion Tooltip
    public Vector2Int upperBounds;
    #region Tooltip
    [Tooltip("There should be a maximum of four doorways for a room - one for each compass direction. These should have a consistent 3 tile opening size, with the middle tile position being the doorway position. The doorway position should be determined from the tilemap for the room (using the coordinate brush pointer to get the tilemap grid position for the doorway).")]
    #endregion Tooltip
    [SerializeField] public List<Doorway> doorwayList;
    #region Tooltip
    [Tooltip("Each possible spawn position (used for enemies and chests) for the room in tilemap coordinates should be added to this array")]
    #endregion Tooltip
    public Vector2Int[] spawnPositionArray;
    public list<Doorway> GetDoorwayList()
    {
        return doorwayList;
    }
    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        //set unique GUID if empty or the prefab has changed
        if (guid == "" || previousPrefab != prefab)
        {
            guid = GUID.Generate().ToString();
            previousPrefab = prefab;
            EditorUtility.SetDirty(this);
        }
        HelperUtilities.ValidateCheckEnumerableValues(this,nameof(doorwayList),doorwayList);
        HelperUtilities.ValidateCheckEnumerableValues(this,nameof(spawnPositionArray),spawnPositionArray);
    }
#endif
    #endregion Validation
}
