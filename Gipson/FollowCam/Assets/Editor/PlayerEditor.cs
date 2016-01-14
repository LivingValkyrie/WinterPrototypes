using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(Player))]
public class MyTypeEditor : Editor {

    Player m_Instance;
    PropertyField[] m_fields;

    public void OnEnable() {
        m_Instance = target as Player;
        m_fields = ExposeProperties.GetProperties( m_Instance );
    }

    public override void OnInspectorGUI() {

        if(m_Instance == null) {
            return;
        }

        DrawDefaultInspector();
        ExposeProperties.Expose( m_fields );

    }

}