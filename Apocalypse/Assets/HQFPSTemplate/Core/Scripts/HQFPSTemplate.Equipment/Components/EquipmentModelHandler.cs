using HQFPSTemplate.Items;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace HQFPSTemplate.Equipment
{
    [Serializable]
	public class EquipmentModelHandler
	{
		public float TargetFOV => m_FPModelSettings.TargetFOV;
		private bool HasSkins => m_FPModelSettings && m_EquipmentModel && m_FPModelSettings.HasSkins;

        [SerializeField]
		private SkinnedMeshRenderer m_EquipmentModel;

		[SerializeField]
		private EquipmentModelInfo m_FPModelSettings;

		private ItemProperty m_AttachedSkinProperty;
		private int m_CurrentSkinIndex = 0;


		public void UpdateSkinIDProperty(Item item)
		{
			if (HasSkins)
			{
				if (item.TryGetProperty(m_FPModelSettings.SkinIDProperty, out ItemProperty skinProperty))
				{
					if (m_AttachedSkinProperty != skinProperty)
						m_AttachedSkinProperty = skinProperty;

					m_CurrentSkinIndex = Mathf.Clamp(m_AttachedSkinProperty.Integer, 0, m_FPModelSettings.Skins.Count - 1);

					UpdateItemRenderer(m_FPModelSettings.Skins[m_CurrentSkinIndex]);
				}
			}
		}

		/// <summary>
		/// Use the predefined FOV
		/// </summary>
		public void UpdateMaterialsFov() 
		{
			if (m_FPModelSettings != null && m_EquipmentModel != null)
				UpdateMaterialsFOV(m_FPModelSettings.TargetFOV);
		}

		/// <summary>
		/// Apply a custom FOV
		/// </summary>
		public void UpdateMaterialsFOV(float fov) 
		{
			if (m_FPModelSettings != null && m_EquipmentModel != null)
			{
				if (m_FPModelSettings.HasSkins)
				{
					var allEquipmentMaterials = new List<Material>();

					// Get all of the distinct materials used on all of the skins
					foreach (var skin in m_FPModelSettings.Skins)
					{
						foreach (var material in skin.SharedMaterials)
						{
							if (!allEquipmentMaterials.Contains(material))
								allEquipmentMaterials.Add(material);
						}
					}

					foreach (var material in allEquipmentMaterials)
						material.SetFloat(m_FPModelSettings.FovProperty, fov);
				}
				else
				{
                    foreach (var material in m_EquipmentModel.sharedMaterials)
						material.SetFloat(m_FPModelSettings.FovProperty, fov);
				}
			}
		}

		public float GetMaterialFOV() 
		{
			if (m_FPModelSettings != null && m_EquipmentModel != null)
				return m_EquipmentModel.sharedMaterial.GetFloat(m_FPModelSettings.FovProperty);
			else
				return 0f;
		}

		/// <summary>
		/// Update the 'Current Skin' to the next one in the list
		/// </summary>
		public void UpdateSkin()
		{
			if (HasSkins)
			{
				var skin = m_FPModelSettings.Skins.ToArray().Select(ref m_CurrentSkinIndex, ItemSelection.Method.Sequence);

				if (m_AttachedSkinProperty != null)
					m_AttachedSkinProperty.Integer = m_FPModelSettings.Skins.IndexOf(skin);

				UpdateItemRenderer(skin);
			}
		}

		private void UpdateItemRenderer(EquipmentSkin skin)
		{
			m_EquipmentModel.sharedMesh = skin.SharedMesh;
			m_EquipmentModel.sharedMaterials = skin.SharedMaterials;
		}
	}
}
