using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using HQFPSTemplate.Items;
using HQFPSTemplate.Equipment;

namespace HQFPSTemplate.UserInterface
{
	public class UI_WeaponInfo : UserInterfaceBehaviour
	{
		#region Internal
		[Serializable]
		public struct FireModeDisplayer
		{
			[BHeader("GENERAL", true)]

			public Image FireModeImage;

			[DatabaseProperty]
			public string FireModeProperty;

			[Space]

			[BHeader("Fire Mode Sprites...", order = 100)]

			public Sprite SafetyModeSprite;
			public Sprite SemiAutoModeSprite;
			public Sprite FullAutoModeSprite;
			public Sprite BurstModeSprite;
		}

		[Serializable]
		public class AmmoAmountDisplayer
		{
			[BHeader("General", true)]

			public Text StorageTxt;

			[Range(1, 100)]
			public int MaxMagSize = 30;

			[Range(1, 100)]
			[Tooltip("At what percent the ammo in the magazine is considered low (e.g. reload message will appear)")]
			public float LowAmmoPercent = 30;

			[DatabaseProperty]
			public string AmmoTypeProperty = "Ammo Type";

			[Space]

			public GridLayoutGroup BulletsLayoutGroup;
			public Image BulletTemplateImg;

			[Space]

			public Color NormalBulletColor = Color.white;
			public Color LowAmmoBulletColor = Color.red;
			public Color BulletConsumedColor = Color.black;

			[Space]

			[Group]
			public BulletDisplayer[] BulletDisplayers;

			[BHeader("Reload Message...", order = 100)]

			public Image ReloadMessage = null;

			// Not visible in the inspector
			public readonly List<Image> BulletImages = new List<Image>(35);

			// -- Internal -- 
			[Serializable]
			public struct BulletDisplayer
			{
				[DatabaseItem]
				public string BulletItem;

				public Vector2 BulletSpriteSize;

				public Vector2 LayoutGroupSpacing;

				public int XOffset;

				public float BulletLineWidth;
			}
		}
		#endregion

		#region Anim Hashing
		//Hashed animator strings (Improves performance)
		private readonly int animHash_FireModeChanged = Animator.StringToHash("Fire Mode Changed");
		private readonly int animHash_ammoConsumed = Animator.StringToHash("Ammo Consumed");
		private readonly int animHash_Show = Animator.StringToHash("Show");
		#endregion

		[SerializeField]
		private Animator m_Animator = null;

		[SerializeField]
		private Image m_WeaponIconImg = null;

		[Space]

		[SerializeField]
		[Group]
		private AmmoAmountDisplayer m_AmmoDisplayer = null;

		[SerializeField]
		[Group]
		private FireModeDisplayer m_FireModeDisplayer = new FireModeDisplayer();

		private bool m_IsNewWeapon;

		private ProjectileWeapon m_Weapon;
		private RectTransform m_BulletsLayoutGroupRct;


		public override void OnPostAttachment()
		{
			Player.ActiveEquipmentItem.AddChangeListener(OnChangeItem);
			Player.ChangeUseMode.AddListener(UpdateFireModeUI);

			//Spawn the bullet images
			if (m_AmmoDisplayer.BulletTemplateImg != null)
			{
				for (int i = 0; i < m_AmmoDisplayer.MaxMagSize; i++)
					m_AmmoDisplayer.BulletImages.Add(Instantiate(m_AmmoDisplayer.BulletTemplateImg,
						m_AmmoDisplayer.BulletTemplateImg.rectTransform.position, //Position
						m_AmmoDisplayer.BulletTemplateImg.rectTransform.rotation, //Rotation
						m_AmmoDisplayer.BulletsLayoutGroup.transform));     //Parent

				m_AmmoDisplayer.BulletTemplateImg.enabled = false;

				m_BulletsLayoutGroupRct = m_AmmoDisplayer.BulletsLayoutGroup.GetComponent<RectTransform>();
			}
		}

		private void OnChangeItem(EquipmentItem eItem)
		{
			m_Weapon = Player.ActiveEquipmentItem.Get() as ProjectileWeapon;
			var prevWeapon = Player.ActiveEquipmentItem.GetPreviousValue() as ProjectileWeapon;

			if (prevWeapon != null)
				prevWeapon.CurrentAmmoInfo.RemoveChangeListener(UpdateAmmoAmountUI);

			m_IsNewWeapon = true;

			if (m_Weapon == null || !m_Weapon.AmmoEnabled)
				m_Animator.SetBool(animHash_Show, false);
			else
			{
				m_Animator.SetBool(animHash_Show, true);

				m_WeaponIconImg.sprite = m_Weapon.EHandler.Item.Info.Icon;

				m_Weapon.CurrentAmmoInfo.AddChangeListener(UpdateAmmoAmountUI);
				UpdateAmmoAmountUI(m_Weapon.CurrentAmmoInfo.Get());

				UpdateAmmoTypeUI();
				UpdateFireModeUI();

				m_IsNewWeapon = false;
			}
		}

		private void UpdateAmmoTypeUI() 
		{
			string ammoTypeName = ItemDatabase.GetItemById(m_Weapon.EHandler.Item.GetProperty(m_AmmoDisplayer.AmmoTypeProperty).ItemId).Name;

			//Change the bullet images
			foreach (var bulDisplayer in m_AmmoDisplayer.BulletDisplayers)
			{
				if ( bulDisplayer.BulletItem == ammoTypeName)
				{
					//Disable the previous bullet images
					for (int j = 0; j < m_AmmoDisplayer.BulletImages.Count; j++)
						m_AmmoDisplayer.BulletImages[j].gameObject.SetActive(false);

					//Change the bullet images if the new weapon uses a different type of ammo
					for (int j = 0; j < m_Weapon.MagazineSize; j++)
					{
						if (ItemDatabase.TryGetItemByName(bulDisplayer.BulletItem, out ItemInfo itmData))
						{
							if (itmData.Icon == m_AmmoDisplayer.BulletImages[j])
								break;

							m_AmmoDisplayer.BulletImages[j].sprite = itmData.Icon;

							m_AmmoDisplayer.BulletImages[j].gameObject.SetActive(true);
							m_AmmoDisplayer.BulletImages[j].transform.localScale = bulDisplayer.BulletSpriteSize;
						}
					}

					m_BulletsLayoutGroupRct.sizeDelta = new Vector2(bulDisplayer.BulletLineWidth, m_BulletsLayoutGroupRct.sizeDelta.y);
					m_AmmoDisplayer.BulletsLayoutGroup.spacing = bulDisplayer.LayoutGroupSpacing;
					m_AmmoDisplayer.BulletsLayoutGroup.padding.right = bulDisplayer.XOffset;

					break;
				}
			}
		}

		private void UpdateAmmoAmountUI(ProjectileWeapon.AmmoInfo ammoInfo)
		{
			if (m_Weapon == null)
				return;

			int newCountInMagazine = ammoInfo.CurrentInMagazine;
			int lastCountInMagazine = m_Weapon.CurrentAmmoInfo.GetPreviousValue().CurrentInMagazine;
			int magSize = m_Weapon.MagazineSize;

			//If the player used the Weapon, consume the bullet
			if (!m_IsNewWeapon)
			{
				if (m_Animator != null && lastCountInMagazine > newCountInMagazine)
				{
					m_Animator.SetTrigger(animHash_ammoConsumed);
					m_AmmoDisplayer.BulletImages[magSize - ammoInfo.CurrentInMagazine - 1].color = m_AmmoDisplayer.BulletConsumedColor;
				}
				//If the player is reloading add the bullets back up
				else if (lastCountInMagazine < newCountInMagazine)
				{
					for (int i = magSize - ammoInfo.CurrentInMagazine; i < m_AmmoDisplayer.BulletImages.Count; i++)
						m_AmmoDisplayer.BulletImages[i].color = m_AmmoDisplayer.NormalBulletColor;
				}
			}
			//Activate as many bullets as the current weapon has in the magazine
			for (int i = 0; i < magSize; i++)
				m_AmmoDisplayer.BulletImages[i].color = m_AmmoDisplayer.BulletConsumedColor;

			for (int i = magSize - newCountInMagazine; i < magSize; i++)
				m_AmmoDisplayer.BulletImages[i].color = m_AmmoDisplayer.NormalBulletColor;

			//Change the bullets color to the "low bullet color".
			if (newCountInMagazine <= magSize * (m_AmmoDisplayer.LowAmmoPercent / 100))
			{
				for (int i = 0; i < m_AmmoDisplayer.BulletImages.Count; i++)
				{
					if (m_AmmoDisplayer.BulletImages[i].color == m_AmmoDisplayer.NormalBulletColor)
						m_AmmoDisplayer.BulletImages[i].color = m_AmmoDisplayer.LowAmmoBulletColor;
				}
			}
			//Change the bullets color back to normal bullet color.
			else
			{
				for (int i = 0; i < m_AmmoDisplayer.BulletImages.Count; i++)
				{
					if(m_AmmoDisplayer.BulletImages[i].color == m_AmmoDisplayer.LowAmmoBulletColor)
						m_AmmoDisplayer.BulletImages[i].color = m_AmmoDisplayer.NormalBulletColor;
				}
			}

			//Update the storage Text
			m_AmmoDisplayer.StorageTxt.text = m_Weapon.CurrentAmmoInfo.Get().CurrentInStorage.ToString();

			UpdateReloadMessage(newCountInMagazine);
		}

		private void UpdateReloadMessage(int newCountInMagazine)
		{
			//Activate Reload Message.
			if (newCountInMagazine <= m_Weapon.MagazineSize * (m_AmmoDisplayer.LowAmmoPercent / 100))
				m_AmmoDisplayer.ReloadMessage.gameObject.SetActive(true);
			else
				m_AmmoDisplayer.ReloadMessage.gameObject.SetActive(false);
		}

		private void UpdateFireModeUI() 
		{
			//If the new weapon doesn't have a "Fire Mode" property disable the fire mode image
			if (m_Weapon != null)
			{
				if (m_IsNewWeapon && !m_Weapon.EHandler.Item.HasProperty(m_FireModeDisplayer.FireModeProperty))
					m_FireModeDisplayer.FireModeImage.color = Color.clear;
				else
				{
					m_FireModeDisplayer.FireModeImage.color = Color.white;

					//Get fire mode
					m_Animator.SetTrigger(animHash_FireModeChanged);

					int fireMode = m_Weapon.EHandler.Item.GetProperty(m_FireModeDisplayer.FireModeProperty).Integer;

					//Get the fire mode property
					if (fireMode == (int)ProjectileWeaponInfo.FireMode.Burst)
						m_FireModeDisplayer.FireModeImage.sprite = m_FireModeDisplayer.BurstModeSprite;
					else if (fireMode == (int)ProjectileWeaponInfo.FireMode.FullAuto)
						m_FireModeDisplayer.FireModeImage.sprite = m_FireModeDisplayer.FullAutoModeSprite;
					else if (fireMode == (int)ProjectileWeaponInfo.FireMode.SemiAuto)
						m_FireModeDisplayer.FireModeImage.sprite = m_FireModeDisplayer.SemiAutoModeSprite;
					else if (fireMode == (int)ProjectileWeaponInfo.FireMode.Safety)
						m_FireModeDisplayer.FireModeImage.sprite = m_FireModeDisplayer.SafetyModeSprite;
				}
			}
		}
    }
}
