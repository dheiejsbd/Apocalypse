using UnityEngine;
using System;

namespace HQFPSTemplate.UserInterface
{
	/// <summary>
	/// Represents an asset that stores data for a particular crosshair
	/// </summary>
	[CreateAssetMenu(menuName = "HQ FPS Template/User Interface/Crosshair")]
	public class UI_CrosshairInfo : ScriptableObject
    {
		#region Internal
		[Serializable]
		public struct CrosshairGfxSettings
		{
			public Color NormalColor,
						 OnEntityColor,
						 UnusableColor;

			[Space]

			public bool ShowWhenAiming;
			public float PivotRotation;

			[Group]
			public SpriteSetup LeftSprite,
							   RightSprite,
							   CenterSprite,
							   TopSprite,
							   BottomSprite;
		}

		[Serializable]
		public struct SpriteSetup
		{
			public Sprite Sprite;
			public Vector2 Size;
		}

		[Serializable]
		public struct CrosshairScaleSettings
		{
			[Group]
			public UI_Spring.Data ScaleSpringData;

			[Group]
			public UI_SpringForce ItemUseScaleForce;

			[Range(0f, 5f)]
			public float ScaleMultiplier;

			[Range(0f, 5f)]
			public float IdleScale;

			[Range(0f, 5f)]
			public float CrouchScale,
						 ProneScale,
						 WalkScale,
						 RunScale,
						 AirborneMultiplier,
						 AimScaleMultiplier;
		}
		#endregion

		[Group]
		public CrosshairGfxSettings GraphicsInfo = new CrosshairGfxSettings();

		[Group]
		public CrosshairScaleSettings ScaleInfo = new CrosshairScaleSettings();
	}
}
