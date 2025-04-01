using System;

namespace SamuelIH.Nwn.Blueprints
{
    [Serializable]
    public class ItemBlueprint : Blueprint
    {
        public ItemBlueprint()
        {
            BlueprintType = "item";
        }
        
        [Inherited]
        public byte? ArmorValue { get; set; }
        
        [NonOptionalInherited]
        public uint? BaseItem { get; set; }

        [Inherited]
        public int? Charges { get; set; }
        
        [Inherited]
        public bool? Identified { get; set; }

        [Inherited]
        public string? LocalizedDescription { get; set; }

        [Inherited]
        public string? LocalizedName { get; set; }

        [Inherited]
        public byte[]? ModelParts { get; set; }

        [Inherited]
        public bool? Plot { get; set; }

        [Inherited]
        public Property[]? Properties { get; set; }
        
        [Inherited]
        public OverridableList<Property>? PropertiesList { get; set; }

        [Inherited]
        public string? Tag { get; set; }

        [Serializable]
        public class Property
        {
            public int CostTable { get; set; }
            public int CostTableValue { get; set; } = -1;
            public int? Param1Value { get; set; } = -1;
            public int PropertyName { get; set; }
            public int SubType { get; set; } = -1;
        }
        
        // Colors
        [Inherited]
        public byte? Cloth1Color { get; set; }
        [Inherited]
        public byte? Cloth2Color { get; set; }
        [Inherited]
        public byte? Leather1Color { get; set; }
        [Inherited]
        public byte? Leather2Color { get; set; }
        [Inherited]
        public byte? Metal1Color { get; set; }
        [Inherited]
        public byte? Metal2Color { get; set; }
        
        // armor parts
        [Inherited]
        public byte? ArmorPartBelt { get; set; }
        [Inherited]
        public byte? ArmorPartLBicep { get; set; }
        [Inherited]
        public byte? ArmorPartLForearm { get; set; }
        [Inherited]
        public byte? ArmorPartLFoot { get; set; }
        [Inherited]
        public byte? ArmorPartLHand { get; set; }
        [Inherited]
        public byte? ArmorPartLShin { get; set; }
        [Inherited]
        public byte? ArmorPartLShoulder { get; set; }
        [Inherited]
        public byte? ArmorPartLThigh { get; set; }
        [Inherited]
        public byte? ArmorPartNeck { get; set; }
        [Inherited]
        public byte? ArmorPartPelvis { get; set; }
        [Inherited]
        public byte? ArmorPartRBicep { get; set; }
        [Inherited]
        public byte? ArmorPartRForearm { get; set; }
        [Inherited]
        public byte? ArmorPartRFoot { get; set; }
        [Inherited]
        public byte? ArmorPartRHand { get; set; }
        [Inherited]
        public byte? ArmorPartRShin { get; set; }
        [Inherited]
        public byte? ArmorPartRShoulder { get; set; }
        [Inherited]
        public byte? ArmorPartRThigh { get; set; }
        [Inherited]
        public byte? ArmorPartRobe { get; set; }
        [Inherited]
        public byte? ArmorPartTorso { get; set; }
        
        [Inherited]
        public OverridableDictionary<int>? IntVars { get; set; }
        
        [Inherited]
        public OverridableDictionary<string>? StrVars { get; set; }
        
        [Inherited]
        public OverridableDictionary<int>? IntMetadata { get; set; }
        
        [Inherited]
        public OverridableDictionary<string>? StringMetadata { get; set; }
        
        [Inherited]
        public OverridableDictionary<bool>? BoolMetadata { get; set; }
        
        [Inherited]
        public OverridableDictionary<float>? FloatMetadata { get; set; }
        
        
        public ItemBlueprint Clone() => MemberwiseClone() as ItemBlueprint ?? throw new Exception("Failed to clone ItemBlueprint");
    }
}