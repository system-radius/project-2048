using System.Collections.Generic;
using UnityEngine;

public class VersusSettingCompiler : ButtonTrigger
{
    [SerializeField]
    private List<LevelSelectionController> levelControllers = new();

    [SerializeField]
    private Configuration config;

    private void OnEnable()
    {
        OnButtonPress += CompileSettings;
    }

    private void OnDisable()
    {
        OnButtonPress -= CompileSettings;
    }

    public void CompileSettings()
    {
        config.playerLevels.Clear();
        foreach (var controller in levelControllers)
        {
            config.playerLevels.Add(controller.GetLevel());
        }
    }
}