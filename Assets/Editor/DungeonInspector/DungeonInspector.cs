using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using static DungeonGenerator;

[CustomEditor(typeof(DungeonGenerator))]
public class DungeonInspector : Editor
{
    public VisualTreeAsset m_InspectorXML;
    //public override VisualElement CreateInspectorGUI()
    //{
    //    var generator = this.target as DungeonGenerator;
    //    VisualElement inspector = new VisualElement();

    //    m_InspectorXML.CloneTree(inspector);

    //    VisualElement rankEditor = new VisualElement();
    //    rankEditor.style.backgroundColor = Color.black;

    //    Button addRankButton = new Button();
    //    addRankButton.text = "Add New Rank";
    //    addRankButton.RegisterCallback<ClickEvent>((evnt) =>
    //    {
    //        generator.setRoom(new DungeonRoom(), generator.ranks.Count);
    //        rankEditor.MarkDirtyRepaint();
    //    });
    //    rankEditor.Add(addRankButton);

    //    var rankNumber = 0;
    //    generator.ranks.ForEach((rank) =>
    //    {
    //        rankEditor.Add(new Label("Rank " + rankNumber));
    //        Button addRoomButton = new Button();
    //        addRoomButton.text = "Add new Room";
    //        addRoomButton.RegisterCallback<ClickEvent>(evnt =>
    //        {
    //            generator.setRoom(new DungeonRoom(), rankNumber);
    //            rankEditor.MarkDirtyRepaint();
    //        });
    //        rankEditor.Add(addRoomButton);
    //        rank.rooms.ForEach(room =>
    //        {
    //            Vector3Field loc = new Vector3Field();
    //            loc.value = room.location;
    //            loc.RegisterValueChangedCallback(evnt =>
    //            {
    //                room.location = evnt.newValue;
    //            });
    //            ObjectField prefab = new ObjectField();
    //            prefab.objectType = typeof(GameObject);
    //            prefab.value = room.prefab;
    //            prefab.RegisterValueChangedCallback(evnt =>
    //            {
    //                room.prefab = evnt.newValue.GameObject();
    //            });
    //            rankEditor.Add(loc);
    //            rankEditor.Add(prefab);
    //        });
    //        rankNumber++;
    //    });

    //    inspector.Add(rankEditor);

    //    return inspector;
    //}
}
