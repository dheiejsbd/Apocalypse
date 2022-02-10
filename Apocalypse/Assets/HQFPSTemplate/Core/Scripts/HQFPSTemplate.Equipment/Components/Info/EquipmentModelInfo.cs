using System;
using UnityEngine;

namespace HQFPSTemplate.Equipment
{
	[Serializable]
	public struct EquipmentSkin
	{
		public string Name;

		public Mesh SharedMesh;
		public Material[] SharedMaterials;
	}

	[CreateAssetMenu(menuName = "HQ FPS Template/Equipment Component/Model", fileName = "Equipment Model Info")]
	public class EquipmentModelInfo : ScriptableObject
	{
		public bool HasSkins => Skins != null && Skins.Count > 0;

		public string FovProperty = "_FOV";

		[Range(10, 120)]
		public float TargetFOV = 45f;

		[Space]

		[DatabaseProperty]
		public string SkinIDProperty = "Skin ID";

		[Reorderable]
		public FPItemSkinsList Skins = new FPItemSkinsList();
	}
}
