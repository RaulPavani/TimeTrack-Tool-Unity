using UnityEngine;
using UnityEditor;
using System;
using System.IO;

#if UNITY_EDITOR

public class TimeTrackTool : EditorWindow {

    static TimeTrackData timeTrackData = new TimeTrackData();

    public static DateTime m_CreatedTime;
    public static TimeSpan m_TotalTime;


    [MenuItem("Tools/Time Track")]
    public static void ShowWindow() {
        var window = GetWindow(typeof(TimeTrackTool));
        window.Show();
    }

    private void OnGUI() {
        GUILayout.Label("Time Track Infos", EditorStyles.boldLabel);

        TimeSpan displayTime = m_TotalTime + TimeSpan.FromSeconds(EditorApplication.timeSinceStartup);

        ReadOnlyTextField("Project Created:", m_CreatedTime.ToString("MM/dd/yyyy - HH:mm"));
        ReadOnlyTextField("Time worked: ", displayTime.Days + " Days, " + displayTime.Hours + " Hours, " + displayTime.Minutes + " Minutes");
    }

    void ReadOnlyTextField(string label, string text) {
        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.LabelField(label, GUILayout.Width(EditorGUIUtility.labelWidth));
            EditorGUILayout.SelectableLabel(text, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
        }
        EditorGUILayout.EndHorizontal();
    }

    public static void SaveData() {
        m_TotalTime += TimeSpan.FromSeconds(EditorApplication.timeSinceStartup);

        timeTrackData.totalHoursWorked = m_TotalTime.ToString();

        File.WriteAllText(Application.persistentDataPath + "/timetrackdata.json", JsonUtility.ToJson(timeTrackData));
    }

    public static void LoadData() {

        if (File.Exists(Application.persistentDataPath + "/timetrackdata.json")) {
            string saveJSON = File.ReadAllText(Application.persistentDataPath + "/timetrackdata.json");

            timeTrackData = JsonUtility.FromJson<TimeTrackData>(saveJSON);

            m_CreatedTime = DateTime.Parse(timeTrackData.createdTime);
            m_TotalTime = TimeSpan.Parse(timeTrackData.totalHoursWorked);

        } else {
            m_CreatedTime = DateTime.Now;
            m_TotalTime = DateTime.Now - m_CreatedTime;

            timeTrackData.createdTime = m_CreatedTime.ToString();
            timeTrackData.totalHoursWorked = m_TotalTime.ToString();

            File.WriteAllText(Application.persistentDataPath + "/timetrackdata.json", JsonUtility.ToJson(timeTrackData));
        }
    }
}

[InitializeOnLoad]
public class Startup {
    static Startup() {
        TimeTrackTool.LoadData();
    }
}

[InitializeOnLoad]
public class EditorQuit {
    static void Quit() {
        TimeTrackTool.SaveData();
    }

    static EditorQuit() {
        EditorApplication.quitting += Quit;
    }
}

public class TimeTrackData {
    public string createdTime;
    public string totalHoursWorked;
}

#endif

