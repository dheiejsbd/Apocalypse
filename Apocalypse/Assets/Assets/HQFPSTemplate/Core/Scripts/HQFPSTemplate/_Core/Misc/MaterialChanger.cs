using System.Collections.Generic;
using UnityEngine;

namespace HQFPSTemplate
{
	public class MaterialChanger : MonoBehaviour
	{
		#region Internal
		protected struct RendererSetup
		{
			public MeshRenderer Renderer;
			public int Id;
		}

		protected struct MaterialSetup
		{
			public Material[] DefaultMaterials;
			public Material[] MaterialsWithEffects;
		}
		#endregion

		[SerializeField]
		private bool m_EnableOutline;

		[SerializeField]
		private bool m_EnableHighlight;

		[Space]

		[SerializeField]
		private string m_OutlineWidthProperty = "_ASEOutlineWidth";

		[SerializeField]
		[Range(-1f, 1f)]
		private float m_OutlineWidth = 0.0065f;

		[SerializeField]
		private string m_HighlightStrengthProperty = "_LineTransparency";

		private static Dictionary<int, MaterialSetup> m_Materials = new Dictionary<int, MaterialSetup>();
		private RendererSetup[] m_Renderers;


		public void SetDefaultMaterial()
		{
			SetMaterials(false);
		}

		public void SetMaterialWithEffects()
		{
			SetMaterials(true);
		}

		private void Awake()
		{
			SetupMaterials();
			SetMaterials(false);
		}

		private void SetMaterials(bool withEffects)
		{
			if (m_Renderers == null)
				return;

			foreach (var renderSetup in m_Renderers)
			{
				MaterialSetup materialSetup;

				if (m_Materials.TryGetValue(renderSetup.Id, out materialSetup))
					renderSetup.Renderer.materials = withEffects ? materialSetup.MaterialsWithEffects : materialSetup.DefaultMaterials;
			}
		}

		private void SetupMaterials()
		{
			MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>(true);

			if (renderers.Length == 0)
				return;

			m_Renderers = new RendererSetup[renderers.Length];

			int rendererIndex = 0;

			foreach (MeshRenderer renderer in renderers)
			{
				int rendererId = CalculateRendererId(renderer);

				if (!m_Materials.ContainsKey(rendererId))
				{
					Material[] defaultMaterials = new Material[renderer.sharedMaterials.Length];
					Material[] materialsWithEffects = new Material[renderer.sharedMaterials.Length];

					int matIndex = 0;

					foreach (Material sharedMat in renderer.sharedMaterials)
					{
						defaultMaterials[matIndex] = sharedMat;

						if (m_EnableOutline || m_EnableHighlight)
						{
							Material materialWithEffects = new Material(sharedMat);
							materialWithEffects.name = materialWithEffects.name + "_WithEffects";

							materialWithEffects.SetFloat(m_OutlineWidthProperty, m_EnableOutline ? m_OutlineWidth : 0f);
							materialWithEffects.SetFloat(m_HighlightStrengthProperty, m_EnableHighlight ? 1f : 0f);

							materialsWithEffects[matIndex] = materialWithEffects;
						}
						else
							materialsWithEffects[matIndex] = sharedMat;

						matIndex++;
					}

					m_Materials.Add(rendererId, new MaterialSetup() { DefaultMaterials = defaultMaterials, MaterialsWithEffects = materialsWithEffects });
				}

				m_Renderers[rendererIndex] = new RendererSetup() { Renderer = renderer, Id = rendererId };
				rendererIndex++;
			}
		}

		private int CalculateRendererId(Renderer renderer)
		{
			int id = 0;

			foreach (var material in renderer.sharedMaterials)
			{
				if (material == null)
				{
					//Debug.LogErrorFormat();
					continue;
				}

				id += material.GetHashCode() / 2;
			}

			return id;
		}
	}
}