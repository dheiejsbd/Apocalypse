using System;
using UnityEngine;
using HQFPSTemplate.Items;
using HQFPSTemplate.Equipment;

namespace HQFPSTemplate
{
    [Serializable]
    public class AudioClipList : ReorderableArray<AudioClip> { }

    [Serializable]
    public class ItemPropertyDefinitionList : ReorderableArray<ItemPropertyDefinition> { }

    [Serializable]
    public class ItemPropertyInfoList : ReorderableArray<ItemPropertyInfo> { }

    [Serializable]
    public class ItemGeneratorList : ReorderableArray<ItemGenerator> { }

    [Serializable]
    public class ContainerGeneratorList : ReorderableArray<ContainerGenerator> { }

    [Serializable]
    public class FPArmsInfoList : ReorderableArray<FPArmsInfo> { }

    [Serializable]
    public class FPItemSkinsList : ReorderableArray<EquipmentSkin> { }

    [Serializable]
    public class EquipmentHandlersList : ReorderableArray<EquipmentHandler> { }
}