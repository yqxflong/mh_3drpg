///////////////////////////////////////////////////////////////////////
//
//  DebuggerUtils.cs
//
//  Copyright (c) 2006-2013 KABAM, INC. All rights reserved.
//  This material contains the confidential and proprietary
//  information of Kabam and may not be copied in whole
//  or in part without the express written permission of Kabam.
//  This copyright notice does not imply publication.
//
///////////////////////////////////////////////////////////////////////

#if DEBUG
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Globalization;

public class DebuggerUtils : MonoBehaviour
{
    public static string _defaultSettingsPath = Application.persistentDataPath + "/defaultDebugSettings.txt";
    private static Dictionary<string, object> _settings = null;

    public static void SaveVariable(string name, object variable)
    {
        if (variable is int)
        {
            PlayerPrefs.SetInt(name, (int)variable);
        }
        else if (variable is bool)
        {
            PlayerPrefs.SetInt(name, (bool)variable ? 1 : 0);
        }
        else if (variable is float)
        {
            PlayerPrefs.SetFloat(name, (float)variable);
        }
        else if (variable is string)
        {
            PlayerPrefs.SetString(name, (string)variable);
        }
    }

    public static void LoadVariable(string name, ref bool variable)
    {
        if (!PlayerPrefs.HasKey(name))
        {
            return;
        }
        variable = PlayerPrefs.GetInt(name) == 1 ? true : false;
    }

    public static void LoadVariable(string name, ref int variable)
    {
        if (!PlayerPrefs.HasKey(name))
        {
            return;
        }
        variable = PlayerPrefs.GetInt(name);
    }

    public static void LoadVariable(string name, ref float variable)
    {
        if (!PlayerPrefs.HasKey(name))
        {
            return;
        }
        variable = PlayerPrefs.GetFloat(name);
    }

    public static void LoadVariable(string name, ref string variable)
    {
        if (!PlayerPrefs.HasKey(name))
        {
            return;
        }
        variable = PlayerPrefs.GetString(name);
    }

    public static object DisplayVariable(MemberInfo param, ref object value)
    {
        System.Type type = param is FieldInfo ? ((FieldInfo)param).FieldType : ((PropertyInfo)param).PropertyType;

        if (type == typeof(int))
        {
            string newVar = GUILayout.TextField(value.ToString());
            return int.Parse(newVar);
        }
        else if (type == typeof(bool))
        {
            return GUILayout.Toggle((bool)value, param.Name);
        }
        else if (type == typeof(float))
        {
            string newVar = GUILayout.TextField(value.ToString());
            return float.Parse(newVar, NumberStyles.Any, CultureInfo.InvariantCulture);
        }
        else if (type == typeof(string))
        {
            return GUILayout.TextField((string)value);
        }
        else
        {
            return null;
        }
    }

    public static void LoadSettings(IDebuggable system, DebugSystemComponent.SystemInfo info, string prefix)
    {
        LoadVariable(prefix + "_isEnabled", ref info.isDebuggingEnabled);
        LoadVariable(prefix + "_showLog", ref info.showLog);

        foreach (FieldInfo field in system.GetType().GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
        {
            if (CanDisplayField(field) && field.GetCustomAttributes(typeof(DontRememberFieldValue), false).Length == 0)
            {
				if (field.FieldType == typeof(bool))
				{
					bool value = (bool)field.GetValue(system);
					DebuggerUtils.LoadVariable(prefix + "_" + field.Name, ref value);
					field.SetValue(system, value);
				}
				else if (field.FieldType == typeof(int))
				{
					int value = (int)field.GetValue(system);
					DebuggerUtils.LoadVariable(prefix + "_" + field.Name, ref value);
					field.SetValue(system, value);
				}
				else if (field.FieldType == typeof(float))
				{
					float value = (float)field.GetValue(system);
					DebuggerUtils.LoadVariable(prefix + "_" + field.Name, ref value);
					field.SetValue(system, value);
				}
				else if (field.FieldType == typeof(string))
				{
					string value = (string)field.GetValue(system);
					DebuggerUtils.LoadVariable(prefix + "_" + field.Name, ref value);
					field.SetValue(system, value);
				}
            }
        }

        if (system is IDebuggableEx)
        {
            ((IDebuggableEx)system).OnPreviousValuesLoaded();
        }
    }

    public static bool CanDisplayField(FieldInfo field)
    {
		System.Type type = field.FieldType;
        return 
			(
				type == typeof(bool) || 
				(
					(
						type == typeof(float) ||
						type == typeof(int) ||
						type == typeof(string)
					) &&
					field.GetCustomAttributes(typeof(ShowFieldInDebuggerAttribute), false).Length > 0
				)
			) &&
			(field.IsPublic || (!field.IsPublic && field.GetCustomAttributes(typeof(ShowFieldInDebuggerAttribute), false).Length > 0));
    }

    public static void DeletePrefs(IDebuggable system, DebugSystemComponent.SystemInfo info, string prefix)
    {
        PlayerPrefs.DeleteKey(prefix + "isEnabled");
        PlayerPrefs.DeleteKey(prefix + "showLog");
        foreach (FieldInfo field in system.GetType().GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
        {
            if (CanDisplayField(field))
            {
                PlayerPrefs.DeleteKey(prefix + field.Name);
            }
        }
    }

    // Have the variable names readable (same format as Unity)
    public static string FormatFieldName(string name)
    {
        System.Text.StringBuilder builder = new System.Text.StringBuilder();

        name = name.Replace("_", "");

        int cursor = 0;
        int wordStart = 0;
        while (cursor < name.Length)
        {
            bool isNumber = char.IsDigit(name[wordStart]);
            bool isLetter = char.IsLetter(name[wordStart]);

            if (!isLetter && !isNumber)
            {
                cursor++;
                continue;
            }

            while (cursor < name.Length && ((isNumber && char.IsDigit(name[cursor])) || (isLetter && char.IsLower(name[cursor]))))
            {
                cursor++;
            }
            string word = name.Substring(wordStart, cursor - wordStart);
            builder.Append(word + (cursor == name.Length ? "" : " "));
            wordStart = cursor;

            // keep the capital letter
            cursor++;
        }

        if (char.IsLetter(builder[0]))
        {
            builder[0] = char.ToUpper(builder[0]);
        }

        return builder.ToString();
    }

    public static void LoadDefaultSettings(IDebuggable system, string prefix)
    {
        if (_settings == null)
        {
            LoadSettingsFromFile();
        }

        foreach (FieldInfo field in system.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
        {
            string key = prefix + "_" + field.Name;
            if (CanDisplayField(field) && _settings.ContainsKey(key))
            {
                field.SetValue(system, _settings[key]);
            }
        }

    }

    public static void SaveDefaultSettings(IDebuggable system, string prefix)
    {
        if (_settings == null)
        {
            LoadSettingsFromFile();
        }

        foreach (FieldInfo field in system.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
        {
            if (CanDisplayField(field))
            {
                _settings[prefix + "_" + field.Name] = field.GetValue(system);
            }
        }

        SaveSettingsToFile();
    }

    private static void LoadSettingsFromFile()
    {
        _settings = new Dictionary<string, object>();

        if (!System.IO.File.Exists(_defaultSettingsPath))
        {
            return;
        }

        string text = System.IO.File.ReadAllText(_defaultSettingsPath);
        foreach (string line in text.Split('\n'))
        {
            if (line.Contains("="))
            {
				string fieldName = line.Split('=')[0];
				object value = null;

				if (fieldName.Contains(":"))
				{
					string fieldType = fieldName.Split(':')[1];
					fieldName = fieldName.Split(':')[0];
					value = line.Split('=')[1];

					if (fieldType == typeof(bool).Name)
					{
						value = (value.ToString() == "1" ? true : false);
					}
					else if (fieldType == typeof(int).Name)
					{
						value = int.Parse(value.ToString());
					}
					else if (fieldType == typeof(float).Name)
					{
						value = float.Parse(value.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture);
					}
				}
				else
				{
					value = (line.Split('=')[1] == "1" ? true : false);
				}

				_settings.Add(fieldName, value);
            }
        }
    }

    private static void SaveSettingsToFile()
    {
        System.Text.StringBuilder builder = new System.Text.StringBuilder();

        foreach (KeyValuePair<string, object> pair in _settings)
        {
            builder.Append(pair.Key).Append(pair.Value.GetType().Name).Append("=");

			if (pair.Value is bool)
			{
				builder.Append((bool)pair.Value ? "1" : "0");
			}
			else if (pair.Value is int)
			{
				builder.Append((int)pair.Value);
			}
			else if (pair.Value is float)
			{
				builder.Append((float)pair.Value);
			}
			else
			{
				builder.Append((string)pair.Value);
			}

			builder.Append("\n");
        }

        if (!System.IO.File.Exists(_defaultSettingsPath))
        {
            System.IO.File.CreateText(_defaultSettingsPath);
        }

        System.IO.StreamWriter writer = new System.IO.StreamWriter(System.IO.File.OpenWrite(_defaultSettingsPath));
        writer.Write(builder.ToString());
        writer.Close();
    }
}
#endif