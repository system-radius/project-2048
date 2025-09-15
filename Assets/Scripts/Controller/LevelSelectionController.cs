using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Dropdown))]
public class LevelSelectionController : MonoBehaviour
{
    private TMP_Dropdown dropdown;

    public int selectedIndex = 0;

    private List<Level> levels = new();

    private void Awake()
    {
        dropdown = GetComponent<TMP_Dropdown>();
        dropdown.options = CreateOptions();

        //dropdown.onValueChanged.AddListener(OnDropdownChange);

        selectedIndex = PlayerPrefs.GetInt(gameObject.name, 0);
        //dropdown.onValueChanged.RemoveListener(OnDropdownChange);
        dropdown.value = selectedIndex;
        dropdown.RefreshShownValue();
        dropdown.onValueChanged.AddListener(OnDropdownChange);
    }

    private List<TMP_Dropdown.OptionData> CreateOptions()
    {
        List<TMP_Dropdown.OptionData> options = new();

        foreach (var level in Enum.GetValues(typeof(Level)))
        {
            options.Add(new TMP_Dropdown.OptionData(level.ToString()));
            levels.Add((Level) level);
        }

        return options;
    }

    private void OnDropdownChange(int index)
    {
        selectedIndex = index;
        PlayerPrefs.SetInt(gameObject.name, selectedIndex);
        PlayerPrefs.Save();
    }

    public Level GetLevel()
    {
        return levels[selectedIndex];
    }
}