﻿using System.Collections.Generic;
using System.Linq;
using StaticData;
using StaticData.Windows;
using UI.Services.Windows;
using UnityEngine;

namespace Infrastructure.Services.StaticData
{
    public class StaticDataService : IStaticDataService
    {
        private const string StaticDataMonstersPath = "StaticData/Monsters";
        private const string StaticDataLevelsPath = "StaticData/Levels";
        private const string StaticDataWindowsPath = "StaticData/UI/WindowStaticData";


        private Dictionary<MonsterTypeId,MonsterStaticData> _monsters;
        private Dictionary<string,LevelStaticData> _levels;
        private Dictionary<WindowId,WindowConfig> _windowConfigs;


        public void Load()
        {
            _monsters = Resources
                .LoadAll<MonsterStaticData>(StaticDataMonstersPath)
                .ToDictionary(x => x.MonsterTypeId, x => x);
            
            _levels = Resources
                .LoadAll<LevelStaticData>(StaticDataLevelsPath)
                .ToDictionary(x => x.LevelKey, x => x);
            
            _windowConfigs = Resources
                .Load<WindowStaticData>(StaticDataWindowsPath).Configs
                .ToDictionary(x => x.WindowId, x => x);
        }

        public MonsterStaticData ForMonster(MonsterTypeId typeId)
        {
            return _monsters.TryGetValue(typeId, out MonsterStaticData staticData) ? staticData : null;
        }

        public LevelStaticData ForLevel(string sceneKey)
        {
            return _levels.TryGetValue(sceneKey, out LevelStaticData staticData) ? staticData : null;
        }

        public WindowConfig ForWindow(WindowId windowId)
        {
            return _windowConfigs.TryGetValue(windowId, out WindowConfig windowConfig) ? windowConfig : null;
        }
    }
}