using System;

namespace HQFPSTemplate.Items
{
    [Flags]
    public enum ItemContainerFlags
    {
        Storage = 1,
        Holster = 2,
        External = 4,
        Everything = ~0
    }

    public enum ItemPropertyType
    {
        Boolean,
        Integer,
        Float,
        ItemId
    }
}