using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CreateAssetMenu(fileName = "Room_", menuName = "Scriptable Objects/Dungeon/Room")]
public class RoomTemplateSO : ScriptableObject
{
    [HideInInspector] public string guid;

    #region Header room prefab

    [Space(10)]
    [Header("ROOM PREFAB")]

    #endregion Header ROOM PREFAB
    
    #region Tooltip

    [Tooltip(
        "The gameobject prefab for the room (this will contain all the tilemaps for the room and environment game objects)")]

    #endregion Tooltip
    
    public GameObject prefab;

    [HideInInspector] public GameObject previousPrefab; //this is used to regenarete the guid if the so is copied and the prefab is changed

    #region Header ROOM CONFIGURATION

    [Space(10)]
    [Header("ROOM CONFIGURATION")]

    #endregion Header ROOM CONFIGURATION

    #region Tooltip

    [Tooltip(
        "The room node type SO. the room node types correspond to the room nodes used in the room node graph. the expectations" +
        "being with corridors. in the room node graph there is just one corridor type 'corridor'. for the room templates there are 2" +
        "corridor node types - corridorns and corridorew")]

    #endregion Tooltip

    public RoomNodeTypeSO roomNodeType;

    #region Tooltip

    [Tooltip(
        "If u imagine a rectangle around the room tilemap that just completely encloses it, the room lower bounds represents the bottom left corner of that rectangle." +
        "This should be determined from the tilemap for the room (using the coordinate brush poiner to get the tilemap grid position for that bottom left corner) (note : this is the" +
        "local tilemap position and not world position)")]

    #endregion Tooltip

    public Vector2Int lowerBounds;

    #region Tooltip

    [Tooltip(
        "If you imagine a rectangle around the room tilemap that just completely encloses it, the room upper bounds represents the top right corner" +
        "of that rectangle. this should be determined from the tilemap for the room (using the coordinate brush pointer to get the tilemap grid position for" +
        "that top right corner. Note: this is the local tilemap position and not the world position")]

    #endregion Tooltip

    public Vector2Int upperBounds;

    #region Tooltip

    [Tooltip(
        "there should be a maximum of four doorways for a room- one for each compass direction. these should have a consistent 3 tile opening size, with the middle tile" +
        "position being the doorway coordinate 'position'")]

    #endregion Tooltip

    [SerializeField]
    public List<Doorway> doorwayList;


    #region Tooltip

    [Tooltip(
        "Each possible spawn position (used for enemies and chests) for the room in tilemap coordinates should be added to this array")]

    #endregion Tooltip

    public Vector2Int[] spawnPositionArray;


    public List<Doorway> GetDoorwayList()
    {
        return doorwayList;
    }

    #region Validation
#if UNITY_EDITOR

    //validate SO fields
    private void Onvalidate()
    {
        //set unique guid if empty or the prefab changes
        if (guid == "" || previousPrefab !=prefab)
        {
            guid = GUID.Generate().ToString();
            previousPrefab = prefab;
            EditorUtility.SetDirty(this);
        }
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(doorwayList), doorwayList);
        //check spawn positions populated
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(spawnPositionArray), spawnPositionArray);
    }
    
#endif
    #endregion Validation

}

