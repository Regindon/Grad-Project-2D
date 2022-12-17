using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using UnityEditor;
using UnityEditor.MPE;
using UnityEditor.U2D.Animation;
using UnityEngine;

public class RoomNodeGraphEditor : EditorWindow
{
    //a
    private GUIStyle roomNodeStyle;
    private static RoomNodeGraphSO currentRoomNodeGraph;
    private RoomNodeTypeListSO roomNodeTypeList;

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
        
        //load room node type
        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;

    }

    [OnOpenAsset(0)] //this is mandatory to create for onasset methods the zero is order to execute
    public static bool OnDoubleClickAsset(int instanceID, int line)
    {
        RoomNodeGraphSO roomNodeGraph = EditorUtility.InstanceIDToObject(instanceID) as RoomNodeGraphSO;

        if (roomNodeGraph !=null)
        {
            OpenWindow();
            currentRoomNodeGraph = roomNodeGraph;
            return true;
        }

        return false;
    }

    
    //drawing editor ui
    private void OnGUI()
    {
        if (currentRoomNodeGraph != null)
        {
            //process events, mouse clicks pressing anything in editor
            ProcessEvent(Event.current);
            
            //draw room nodes
            DrawRoomNodes();
        }

        if (GUI.changed)
        {
            Repaint();
        }
    }

    private void ProcessEvent(Event currentEvent)
    {
        ProcessRoomNodeGraphEvents(currentEvent);
    }

    private void ProcessRoomNodeGraphEvents(Event currentEvent)
    {
        switch (currentEvent.type)
        {
            //mouse down events
            case EventType.MouseDown:
                ProcessMouseDownEvent(currentEvent);
                break;
            default:
                break;
        }
    }

    private void ProcessMouseDownEvent(Event currentEvent)
    {
        //checking if right click
        if (currentEvent.button==1)
        {
            ShowContextMenu(currentEvent.mousePosition);
        }
    }

    private void ShowContextMenu(Vector2 mousePosition)
    {
        GenericMenu menu = new GenericMenu();
        
        menu.AddItem(new GUIContent("Create Room Node"), false, CreateRoomNode, mousePosition);
        
        menu.ShowAsContext();
    }

    //creating overloaded event
    private void CreateRoomNode(object mousePositionObject)
    {
        //finding none list objects
        CreateRoomNode(mousePositionObject, roomNodeTypeList.list.Find(x=> x.isNone));
    }

    private void CreateRoomNode(object mousePositionObject, RoomNodeTypeSO roomNodeType)
    {
        //converting to vector2 just in case
        Vector2 mousePosition = (Vector2)mousePositionObject;
        
        //create room node scriptable object asset
        RoomNodeSO roomNode = ScriptableObject.CreateInstance<RoomNodeSO>();
        
        //add room node to current room node graph room node list
        currentRoomNodeGraph.roomNodeList.Add(roomNode);

        roomNode.Initialise(new Rect(mousePosition, new Vector2(nodeWidth, nodeHeight)), currentRoomNodeGraph,
            roomNodeType);
        AssetDatabase.AddObjectToAsset(roomNode,currentRoomNodeGraph);
        AssetDatabase.SaveAssets();
    }

    private void DrawRoomNodes()
    {
        //loop through all room noedes and draw them
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            roomNode.Draw(roomNodeStyle);
        }

        GUI.changed = true;
    }
}
