using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "RoomNodeType_", menuName = "ScriptableObjects/Dungeon/RoomNodeType")]
public class RoomNodeTypeSO : ScriptableObject
{
    public string roomNodeTypeName;
    #region Header
    [Header("Only flag the RoomNodeTypes that you want to display in the NodeGraphEditor")]
    #endregion
    public bool displayInNodeGraphEditor = true;
    #region Header
    [Header("One type Shuold be A Corridor")]
    #endregion
    public bool isCorridor;
    #region Header
    [Header("One type Shuold be A CorridorNS")]
    #endregion
    public bool isCorridorNS;
    #region Header
    [Header("One type Shuold be A CorridorEW")]
    #endregion
    public bool isCorridorEW;
    #region Header
    [Header("One type Shuold be A Entrance")]
    #endregion
    public bool isEntrance;
    #region Header
    [Header("One type Shuold be A Basic Room")]
    #endregion
    public bool isBossRoom;
    #region Header
    [Header("One type Shuold be None (Unassigned)")]
    #endregion
    public bool isNone;

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(roomNodeTypeName), roomNodeTypeName);
    }
#endif
    #endregion
}
