using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stellaris_Lobby_Manager
{
    public class SettingItem
    {
        public object? Value { get; set; }
        [DefaultValue(false)]
        public bool IsLocked { get; set; } = false;
        [DefaultValue(false)]
        public bool IsSkipped { get; set; } = false;

        [JsonIgnore]
        public string? Name { get; set; }
        [JsonIgnore]
        public object? ValueGame { get; set; }
        [JsonIgnore]
        public object? ValueSet { get; set; }
        [JsonIgnore]
        public Type? ValueType { get; set; }
        [JsonIgnore]
        public int Offset { get; set; } = 0;
    }

    public class GalaxySettingItem : SettingItem
    {
        [JsonIgnore]
        public bool IsGalaxySize { get; set; } = true;
        [JsonIgnore]
        public int StrOffset { get; set; } = 0;
        [JsonIgnore]
        public Dictionary<string, (int, IntPtr)> Options = new Dictionary<string, (int, IntPtr)>();
        public GalaxySettingItem() { ValueType = typeof(IntPtr); }
        public string GetOptionStrByInt(int value) => Options.FirstOrDefault(x => x.Value.Item1 == value).Key;
        public IntPtr GetOptionPtrByInt(int value) => Options.FirstOrDefault(x => x.Value.Item1 == value).Value.Item2;
    }

    public class YearSettingItem : SettingItem
    {
        [JsonIgnore]
        public int DefaultValue = 2200;
        public YearSettingItem() { ValueType = typeof(int); }
    }

    public class PreciseNumericSettingItem : SettingItem
    {
        [JsonIgnore]
        public int FullPrecision { get; set; } = 0;
        [JsonIgnore]
        public int DefaultPrecision { get; set; } = 0;
        public PreciseNumericSettingItem() { ValueType = typeof(ulong); }
    }

    public class NumericSettingItem : SettingItem
    {
        public NumericSettingItem() { ValueType = typeof(uint); }
    }

    public class NumericRandomSettingItem : NumericSettingItem
    {
    }

    public class SelectSettingItem : SettingItem
    {
        public SelectSettingItem() { ValueType = typeof(byte); }
    }

    public class CheckSettingItem : SettingItem
    {
        public CheckSettingItem() { ValueType = typeof(bool); }
    }

    public class LobbySettings
    {
        public static readonly List<string> GalaxySizeSrc = new() { "tiny", "small", "medium", "large", "huge" };
        public static readonly List<string> GalaxyShapeSrc = new() { "elliptical", "spiral_2", "spiral_3", "spiral_4", "spiral_6", "ring", "bar", "starburst", "cartwheel", "spoked" };

        public static Dictionary<string, SettingItem> defaultSettings () => new Dictionary<string, SettingItem> {
             {"galaxySize", new GalaxySettingItem
                 {
                    Name = "galaxySize",
                    Offset = 0x8,
                    StrOffset = Properties.Settings.Default.sizeStringOffset,
                 }
             },
             {"galaxyShape", new GalaxySettingItem
                 {
                    Name = "galaxyShape",
                    Offset = 0x48,
                    StrOffset = Properties.Settings.Default.shapeStringOffset,
                    IsGalaxySize = false,
                 }
             },
            {"habitableWorlds", new PreciseNumericSettingItem
                {
                    Name = "habitableWorlds",
                    FullPrecision = 5,
                    DefaultPrecision = 2,
                    Offset = 0x98,
                }
            },
            {"guaranteedHabitable", new NumericSettingItem
                {
                    Name = "guaranteedHabitable",
                    Offset = 0xE0,
                }
            },
            {"empirePlacementSP", new SelectSettingItem
                {
                    Name = "empirePlacementSP",
                    Offset = 0xC8,
                }
            },
            {"empirePlacementMP", new SelectSettingItem
                {
                    Name = "empirePlacementMP",
                    Offset = 0xE5,
                }
            },
            {"preFTL", new PreciseNumericSettingItem
                {
                    Name = "preFTL",
                    FullPrecision = 5,
                    DefaultPrecision = 2,
                    Offset = 0xA0,
                }
            },
            {"hyperlaneDensity", new PreciseNumericSettingItem
                {
                    Name = "hyperlaneDensity",
                    FullPrecision = 5,
                    DefaultPrecision = 2,
                    Offset = 0x78,
                }
            },
            {"abandonedGateways", new PreciseNumericSettingItem
                {
                    Name = "abandonedGateways",
                    FullPrecision = 5,
                    DefaultPrecision = 2,
                    Offset = 0x70,
                }
            },
            {"wormholePairs", new PreciseNumericSettingItem
                {
                    Name = "wormholePairs",
                    FullPrecision = 5,
                    DefaultPrecision = 2,
                    Offset = 0x68,
                }
            },
            {"techCost", new PreciseNumericSettingItem
                {
                    Name = "techCost",
                    FullPrecision = 5,
                    DefaultPrecision = 2,
                    Offset = 0xB0,
                }
            },
            {"logisticGrowthCeiling", new PreciseNumericSettingItem
                {
                    Name = "logisticGrowthCeiling",
                    FullPrecision = 5,
                    DefaultPrecision = 2,
                    Offset = 0xB8,
                }
            },
            {"growthRequiredScaling", new PreciseNumericSettingItem
                {
                    Name = "growthRequiredScaling",
                    FullPrecision = 5,
                    DefaultPrecision = 2,
                    Offset = 0xC0,
                }
            },
            {"caravaneers", new CheckSettingItem
                {
                    Name = "caravaneers",
                    Offset = 0xD0,
                }
            },
            {"lGates", new CheckSettingItem
                {
                    Name = "lGates",
                    Offset = 0xD2,
                }
            },
            {"xenoCompatibility", new CheckSettingItem
                {
                    Name = "xenoCompatibility",
                    Offset = 0xD1,
                }
            },
            {"AIEmpires", new NumericRandomSettingItem
                {
                    Name = "AIEmpires",
                    Offset = 0x50,
                }
            },
            {"AIEmpiresRandom", new CheckSettingItem
                {
                    Name = "AIEmpiresRandom",
                    Offset = 0x60,
                }
            },
            {"advancedAIStarts", new NumericRandomSettingItem
                {
                    Name = "advancedAIStarts",
                    Offset = 0x54,
                }
            },
            {"advancedAIStartsRandom", new CheckSettingItem
                {
                    Name = "advancedAIStartsRandom",
                    Offset = 0x61,
                }
            },
            {"advancedNeighbors", new CheckSettingItem
                {
                    Name = "advancedNeighbors",
                    Offset = 0xC9,
                }
            },
            {"fallenEmpires", new NumericRandomSettingItem
                {
                    Name = "fallenEmpires",
                    Offset = 0x58,
                }
            },
            {"fallenEmpiresRandom", new CheckSettingItem
                {
                    Name = "fallenEmpiresRandom",
                    Offset = 0x63,
                }
            },
            {"marauderEmpires", new NumericRandomSettingItem
                {
                    Name = "marauderEmpires",
                    Offset = 0x5C,
                }
            },
            {"marauderEmpiresRandom", new CheckSettingItem
                {
                    Name = "marauderEmpiresRandom",
                    Offset = 0x62,
                }
            },
            {"crisisStrength", new PreciseNumericSettingItem
                {
                    Name = "crisisStrength",
                    FullPrecision = 5,
                    DefaultPrecision = 2,
                    Offset = 0xA8,
                }
            },
            {"crisisType", new SelectSettingItem
                {
                    Name = "crisisType",
                    Offset = 0x90,
                }
            },
            {"difficulty", new SelectSettingItem
                {
                    Name = "difficulty",
                    Offset = 0x88,
                }
            },
            {"scalingDifficulty", new SelectSettingItem
                {
                    Name = "scalingDifficulty",
                    Offset = 0xCC,
                }
            },
            {"difficultyAdjustedAI", new CheckSettingItem
                {
                    Name = "difficultyAdjustedAI",
                    Offset = 0xE4,
                }
            },
            {"AIAggressiveness", new SelectSettingItem
                {
                    Name = "AIAggressiveness",
                    Offset = 0x8C,
                }
            },
            {"midGameStartYear", new YearSettingItem
                {
                    Name = "midGameStartYear",
                    DefaultValue = 2300,
                    Offset = 0xD4,
                }
            },
            {"endGameStartYear", new YearSettingItem
                {
                    Name = "endGameStartYear",
                    DefaultValue = 2400,
                    Offset = 0xD8,
                }
            },
            {"victoryYear", new YearSettingItem
                {
                    Name = "victoryYear",
                    DefaultValue = 2500,
                    Offset = 0xDC,
                }
            },
            {"ironmanMode", new CheckSettingItem
                {
                    Name = "ironmanMode",
                    Offset = 0xD3,
                }
            },
        };
    }
}
