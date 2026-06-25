using System;
using System.Linq;
using Config;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Character.Display;
using GameData.Domains.Map;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.Views.World
{
	// Token: 0x02000723 RID: 1827
	public class Area : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
	{
		// Token: 0x17000A82 RID: 2690
		// (get) Token: 0x06005748 RID: 22344 RVA: 0x00288961 File Offset: 0x00286B61
		public static string AreaStateItemKey
		{
			get
			{
				return "Game.Views.World.AreaMap.AreaStateItemKey";
			}
		}

		// Token: 0x17000A83 RID: 2691
		// (get) Token: 0x06005749 RID: 22345 RVA: 0x00288968 File Offset: 0x00286B68
		public TooltipInvoker Displayer
		{
			get
			{
				return this.displayer;
			}
		}

		// Token: 0x17000A84 RID: 2692
		// (get) Token: 0x0600574A RID: 22346 RVA: 0x00288970 File Offset: 0x00286B70
		public RectTransform EffectRect
		{
			get
			{
				return this.icon.rectTransform;
			}
		}

		// Token: 0x17000A85 RID: 2693
		// (get) Token: 0x0600574B RID: 22347 RVA: 0x0028897D File Offset: 0x00286B7D
		public RectTransform EffectRectBase
		{
			get
			{
				return base.transform as RectTransform;
			}
		}

		// Token: 0x0600574C RID: 22348 RVA: 0x0028898C File Offset: 0x00286B8C
		public void SetTwelveImmortals(NameAndAvatar data, PositionFollower[] followers)
		{
			bool flag = !data.IsValid;
			if (flag)
			{
				bool flag2 = this.mainStoryRedEffect;
				if (flag2)
				{
					this.mainStoryRedEffect.gameObject.SetActive(false);
				}
				this.icon.material = null;
			}
			else
			{
				bool flag3 = this.mainStoryRedEffect;
				if (flag3)
				{
					this.icon.material = this.outlineMaterial;
					this.mainStoryRedEffect.gameObject.SetActive(true);
				}
				short charTemplateId = data.Name.CharTemplateId;
				if (!true)
				{
				}
				PositionFollower positionFollower;
				switch (charTemplateId)
				{
				case 1075:
					positionFollower = followers[0];
					break;
				case 1076:
					positionFollower = followers[1];
					break;
				case 1077:
					positionFollower = followers[2];
					break;
				case 1078:
					positionFollower = followers[3];
					break;
				case 1079:
					positionFollower = followers[4];
					break;
				case 1080:
					positionFollower = followers[5];
					break;
				case 1081:
					positionFollower = followers[6];
					break;
				case 1082:
					positionFollower = followers[7];
					break;
				case 1083:
					positionFollower = followers[8];
					break;
				case 1084:
					positionFollower = followers[9];
					break;
				case 1085:
					positionFollower = followers[10];
					break;
				case 1086:
					positionFollower = followers[11];
					break;
				default:
					positionFollower = null;
					break;
				}
				if (!true)
				{
				}
				PositionFollower item = positionFollower;
				bool flag4 = item != null && item;
				if (flag4)
				{
					item.Target = this.EffectRect;
					item.gameObject.SetActive(true);
				}
			}
		}

		// Token: 0x0600574D RID: 22349 RVA: 0x00288AF4 File Offset: 0x00286CF4
		public void SetNameScale(float scale)
		{
			this.nameBase.transform.localScale = Vector3.one * scale;
		}

		// Token: 0x0600574E RID: 22350 RVA: 0x00288B14 File Offset: 0x00286D14
		private bool CreateAreaStateItem(int index, int value)
		{
			bool flag = value == 0 || string.IsNullOrWhiteSpace(MapLegend.Instance[index].Sprite) || (AreaStateItemController.Checker.HideAll && MapLegend.Instance[index].ShowInAreaMap) || !AreaStateItemController.Checker.Enabled.CheckIndex(index) || !AreaStateItemController.Checker.Enabled[index];
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				AreaStateItem state = PoolManager.GetObject<AreaStateItem>(Area.AreaStateItemKey);
				state.transform.SetParent(this.extraInfoContainer, false);
				AreaStateItem areaStateItem = state;
				AreaMap areaMap = this.map;
				areaStateItem.Set(index, value, ((areaMap != null) ? areaMap.areaStateItemSpriteSuffix : null) ?? "");
				result = true;
			}
			return result;
		}

		// Token: 0x0600574F RID: 22351 RVA: 0x00288BD0 File Offset: 0x00286DD0
		public void InsertAreaStateItem(int index, int value)
		{
			bool flag = this.CreateAreaStateItem(index, value);
			if (flag)
			{
				this.extraInfo.gameObject.SetActive(true);
			}
		}

		// Token: 0x06005750 RID: 22352 RVA: 0x00288BFC File Offset: 0x00286DFC
		public void RemoveAllAreaStateItem()
		{
			int idx = this.extraInfoContainer.childCount;
			while (idx-- > 0)
			{
				PoolManager.Destroy(Area.AreaStateItemKey, this.extraInfoContainer.GetChild(idx).gameObject);
			}
			this.extraInfoContainer.gameObject.SetActive(false);
		}

		// Token: 0x17000A86 RID: 2694
		// (get) Token: 0x06005751 RID: 22353 RVA: 0x00288C52 File Offset: 0x00286E52
		public MapAreaItem Config
		{
			get
			{
				return MapArea.Instance[this.templateId];
			}
		}

		// Token: 0x06005752 RID: 22354 RVA: 0x00288C64 File Offset: 0x00286E64
		private void Awake()
		{
			bool flag = this.selected;
			if (flag)
			{
				this.selected.gameObject.SetActive(this.toggle.isOn);
				this.toggle.onValueChanged.ResetListener(delegate(bool isOn)
				{
					this.selected.gameObject.SetActive(isOn || this._highlighted);
				});
			}
			bool flag2 = this.hover;
			if (flag2)
			{
				this.hover.gameObject.SetActive(false);
			}
		}

		// Token: 0x06005753 RID: 22355 RVA: 0x00288CE0 File Offset: 0x00286EE0
		public void SetHighlight(bool active)
		{
			this._highlighted = active;
			bool flag = this.selected;
			if (flag)
			{
				this.selected.gameObject.SetActive(active || this.toggle.isOn);
			}
		}

		// Token: 0x06005754 RID: 22356 RVA: 0x00288D26 File Offset: 0x00286F26
		public void RefreshHighlight()
		{
			this.selected.gameObject.SetActive(this._highlighted || this.toggle.isOn);
		}

		// Token: 0x06005755 RID: 22357 RVA: 0x00288D50 File Offset: 0x00286F50
		public void Set(AreaDisplayData displayData, bool isTravel)
		{
			this.AreaDisplayData = displayData;
			this.brokenLevel.gameObject.SetActive(displayData.IsBroken);
			CImage cimage = this.erosionEffect;
			if (cimage != null)
			{
				cimage.gameObject.SetActive(displayData.IsBroken);
			}
			bool isBroken = displayData.IsBroken;
			if (isBroken)
			{
				this.brokenLevel.text = displayData.BrokenLevel.ToString();
			}
			this.RemoveAllAreaStateItem();
			bool activeExtraInfo = false;
			for (int i = 0; i < displayData.States.Count; i++)
			{
				activeExtraInfo |= this.CreateAreaStateItem(i, displayData.States[i]);
			}
			this.RefreshStyleEffect(isTravel ? (!displayData.IsUnlocked) : (!this.toggle.interactable));
			Sprite[] baseSp = activeExtraInfo ? this.extraToggleSp : this.baseToggleSp;
			bool flag = baseSp != null && baseSp.Length == 3;
			if (flag)
			{
				SpriteState spstat = this.toggle.spriteState;
				spstat.highlightedSprite = baseSp[1];
				spstat.pressedSprite = baseSp[2];
				this.nameBase.sprite = (spstat.selectedSprite = (spstat.disabledSprite = baseSp[0]));
				this.toggle.spriteState = spstat;
				this.extraInfoContainer.gameObject.SetActive(activeExtraInfo);
			}
			else
			{
				this.extraInfo.gameObject.SetActive(activeExtraInfo);
				this.areaName.color = (displayData.IsUnlocked ? (displayData.IsBroken ? Colors.Instance["brightyellow"] : Colors.Instance["black"]) : Colors.Instance["grey"]);
			}
			int unlockState = displayData.IsUnlocked ? (displayData.IsBroken ? 2 : 0) : 1;
			sbyte areaType = this.Config.AreaType;
			if (!true)
			{
			}
			Sprite[] array;
			if (areaType != 0)
			{
				if (areaType != 1)
				{
					array = this.normalNameBases;
				}
				else
				{
					array = this.sectNameBases;
				}
			}
			else
			{
				array = this.cityNameBases;
			}
			if (!true)
			{
			}
			Sprite[] nameSp = array;
			bool flag2 = this.nameBase && nameSp.CheckIndex(unlockState);
			if (flag2)
			{
				this.nameBase.sprite = nameSp[unlockState];
			}
		}

		// Token: 0x06005756 RID: 22358 RVA: 0x00288FA0 File Offset: 0x002871A0
		public void SetStyle(bool disable, bool alsoSetInteractable = true)
		{
			this.RefreshStyleEffect(disable);
			if (alsoSetInteractable)
			{
				this.toggle.interactable = !disable;
			}
		}

		// Token: 0x06005757 RID: 22359 RVA: 0x00288FCC File Offset: 0x002871CC
		public void RefreshStyleEffect(bool isDisable)
		{
			bool flag = this.AreaDisplayData.IsBroken && !this.root._skipList.Contains(this.nameBase);
			if (flag)
			{
				this.root._skipList.Add(this.nameBase);
			}
			else
			{
				this.root._skipList.Remove(this.nameBase);
			}
			this.root.SetStyleEffect(isDisable, false);
		}

		// Token: 0x06005758 RID: 22360 RVA: 0x00289048 File Offset: 0x00287248
		public void Set(MapAreaItem config, bool isTaiwuVillage = false)
		{
			this.displayer.enabled = false;
			this.templateId = config.TemplateId;
			this.areaName.text = config.Name;
			bool flag = this.selected;
			if (flag)
			{
				this.selected.rectTransform.sizeDelta = this.selected.rectTransform.sizeDelta.SetX((config.AreaType == 0) ? this.citySelected : ((config.AreaType == 1 || isTaiwuVillage) ? this.sectSelected : this.normalSelected));
				this.selected.transform.localPosition = this.selected.transform.localPosition.SetY((config.AreaType == 0) ? this.citySelectY : this.normalSelectY);
			}
			bool flag2 = this.hover;
			if (flag2)
			{
				this.hover.rectTransform.sizeDelta = this.hover.rectTransform.sizeDelta.SetX((config.AreaType == 0) ? this.citySelected : ((config.AreaType == 1 || isTaiwuVillage) ? this.sectSelected : this.normalSelected));
				this.hover.transform.localPosition = this.hover.transform.localPosition.SetY((config.AreaType == 0) ? this.citySelectY : this.normalSelectY);
			}
			sbyte style = SharedMethods.GetAreaStyle(this.templateId);
			Sprite[] array = this.erosionEffectIcons;
			bool flag3 = array != null && array.CheckIndex((int)style);
			if (flag3)
			{
				this.erosionEffect.sprite = this.erosionEffectIcons[(int)style];
			}
			this.nameIcon.sprite = (isTaiwuVillage ? this.taiwuVillageNameIcon : ((config.AreaType != 1) ? this.normalNameIcons[(int)config.AreaType] : this.sectNameIcons[(int)(config.StateID - 1)]));
			bool flag4 = this.icon;
			if (flag4)
			{
				base.transform.localPosition = new Vector2(config.ImagePos[0], config.ImagePos[1]);
				this.nameBase.rectTransform.pivot = this.nameBase.rectTransform.pivot.SetY((config.AreaType == 0) ? this.cityPivot : this.normalPivot);
			}
			this.SetIcon(config, isTaiwuVillage);
		}

		// Token: 0x06005759 RID: 22361 RVA: 0x002892B8 File Offset: 0x002874B8
		private void SetIcon(MapAreaItem config, bool isTaiwuVillage = false)
		{
			string iconName = (isTaiwuVillage ? MapArea.DefValue.Born : config).BigMapIcon;
			bool flag = this.icon;
			if (flag)
			{
				this.icon.SetSprite(iconName, false, null);
			}
		}

		// Token: 0x0600575A RID: 22362 RVA: 0x002892F8 File Offset: 0x002874F8
		public void OnPointerEnter(PointerEventData eventData)
		{
			bool flag = this.hover && this.toggle.interactable;
			if (flag)
			{
				this.hover.gameObject.SetActive(true);
			}
			AreaMap areaMap = this.map;
			if (areaMap != null)
			{
				Action<short> onMouseEnterAreaTemplateId = areaMap.OnMouseEnterAreaTemplateId;
				if (onMouseEnterAreaTemplateId != null)
				{
					onMouseEnterAreaTemplateId(this.templateId);
				}
			}
		}

		// Token: 0x0600575B RID: 22363 RVA: 0x0028935C File Offset: 0x0028755C
		public void OnPointerExit(PointerEventData eventData)
		{
			bool flag = this.hover;
			if (flag)
			{
				this.hover.gameObject.SetActive(false);
			}
			AreaMap areaMap = this.map;
			if (areaMap != null)
			{
				Action<short> onMouseExitAreaTemplateId = areaMap.OnMouseExitAreaTemplateId;
				if (onMouseExitAreaTemplateId != null)
				{
					onMouseExitAreaTemplateId(this.templateId);
				}
			}
		}

		// Token: 0x0600575C RID: 22364 RVA: 0x002893B0 File Offset: 0x002875B0
		public void SetShowAreaState(bool show)
		{
			bool flag = !show;
			if (flag)
			{
				this.extraInfoContainer.gameObject.SetActive(false);
			}
			else
			{
				bool flag2 = this.extraInfoContainer.Cast<Transform>().Any((Transform item) => item.gameObject.activeSelf);
				if (flag2)
				{
					this.extraInfoContainer.gameObject.SetActive(true);
				}
				else
				{
					this.extraInfoContainer.gameObject.SetActive(false);
				}
			}
		}

		// Token: 0x04003BB8 RID: 15288
		[SerializeField]
		private short templateId;

		// Token: 0x04003BB9 RID: 15289
		[SerializeField]
		private RectTransform extraInfoContainer;

		// Token: 0x04003BBA RID: 15290
		[SerializeField]
		internal AreaMap map;

		// Token: 0x04003BBB RID: 15291
		[SerializeField]
		internal CToggle toggle;

		// Token: 0x04003BBC RID: 15292
		[SerializeField]
		internal CImage icon;

		// Token: 0x04003BBD RID: 15293
		[SerializeField]
		internal CImage selected;

		// Token: 0x04003BBE RID: 15294
		[SerializeField]
		internal CImage nameBase;

		// Token: 0x04003BBF RID: 15295
		[SerializeField]
		internal CImage nameIcon;

		// Token: 0x04003BC0 RID: 15296
		[SerializeField]
		internal CImage extraInfo;

		// Token: 0x04003BC1 RID: 15297
		[SerializeField]
		internal CImage erosionEffect;

		// Token: 0x04003BC2 RID: 15298
		[SerializeField]
		internal CImage hover;

		// Token: 0x04003BC3 RID: 15299
		[SerializeField]
		private TMP_Text areaName;

		// Token: 0x04003BC4 RID: 15300
		[SerializeField]
		private TMP_Text brokenLevel;

		// Token: 0x04003BC5 RID: 15301
		[SerializeField]
		private float citySelected = 420f;

		// Token: 0x04003BC6 RID: 15302
		[SerializeField]
		private float sectSelected = 270f;

		// Token: 0x04003BC7 RID: 15303
		[SerializeField]
		private float normalSelected = 250f;

		// Token: 0x04003BC8 RID: 15304
		[SerializeField]
		private float normalPivot = 0.5f;

		// Token: 0x04003BC9 RID: 15305
		[SerializeField]
		private float cityPivot = -0.5f;

		// Token: 0x04003BCA RID: 15306
		[SerializeField]
		private float normalSelectY = 0f;

		// Token: 0x04003BCB RID: 15307
		[SerializeField]
		private float citySelectY = 0f;

		// Token: 0x04003BCC RID: 15308
		[SerializeField]
		private Sprite taiwuVillageNameIcon;

		// Token: 0x04003BCD RID: 15309
		[SerializeField]
		private Sprite[] cityNameBases;

		// Token: 0x04003BCE RID: 15310
		[SerializeField]
		private Sprite[] sectNameBases;

		// Token: 0x04003BCF RID: 15311
		[SerializeField]
		private Sprite[] normalNameBases;

		// Token: 0x04003BD0 RID: 15312
		[SerializeField]
		private Sprite[] normalNameIcons;

		// Token: 0x04003BD1 RID: 15313
		[SerializeField]
		private Sprite[] sectNameIcons;

		// Token: 0x04003BD2 RID: 15314
		[SerializeField]
		private Sprite[] erosionEffectIcons;

		// Token: 0x04003BD3 RID: 15315
		[SerializeField]
		private DisableStyleRoot root;

		// Token: 0x04003BD4 RID: 15316
		private bool _highlighted;

		// Token: 0x04003BD5 RID: 15317
		[SerializeField]
		private TooltipInvoker displayer;

		// Token: 0x04003BD6 RID: 15318
		[SerializeField]
		private Sprite[] baseToggleSp;

		// Token: 0x04003BD7 RID: 15319
		[SerializeField]
		private Sprite[] extraToggleSp;

		// Token: 0x04003BD8 RID: 15320
		public AreaDisplayData AreaDisplayData;

		// Token: 0x04003BD9 RID: 15321
		[CanBeNull]
		[SerializeField]
		private CImage mainStoryRedEffect;

		// Token: 0x04003BDA RID: 15322
		[SerializeField]
		private UnityEngine.Material outlineMaterial;
	}
}
