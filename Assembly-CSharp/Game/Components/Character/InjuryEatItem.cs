using System;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Character;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using Spine.Unity;
using UnityEngine;

namespace Game.Components.Character
{
	// Token: 0x02000F2F RID: 3887
	public class InjuryEatItem : MonoBehaviour
	{
		// Token: 0x17001439 RID: 5177
		// (get) Token: 0x0600B2DE RID: 45790 RVA: 0x00516E9F File Offset: 0x0051509F
		public GameObject Highlight
		{
			get
			{
				return this.highlight;
			}
		}

		// Token: 0x0600B2DF RID: 45791 RVA: 0x00516EA8 File Offset: 0x005150A8
		private void Awake()
		{
			bool flag = this.button != null;
			if (flag)
			{
				this.button.ClearAndAddListener(new Action(this.OnClickEatArea));
			}
		}

		// Token: 0x0600B2E0 RID: 45792 RVA: 0x00516EE0 File Offset: 0x005150E0
		public void Set(int charId, bool isUnlocked, ItemDisplayData itemData, short duration, bool showTipsByDefault = true)
		{
			bool hasItem = itemData != null && itemData.Key.HasTemplate;
			this.mask.SetActive(hasItem);
			bool interactable = isUnlocked && !hasItem;
			string iconName = string.Empty;
			bool flag = hasItem;
			if (flag)
			{
				bool isWug = EatingItems.IsWug(itemData.Key);
				this.wugSkeleton.gameObject.SetActive(isWug);
				bool flag2 = !isWug;
				if (flag2)
				{
					iconName = ItemTemplateHelper.GetIcon(itemData.Key.ItemType, itemData.Key.TemplateId);
				}
				else
				{
					MedicineItem wugConfig = Medicine.Instance[itemData.Key.TemplateId];
					string wugName = InjuryEatItem.WugSkeletonNames[(int)wugConfig.WugGrowthType];
					SkeletonDataAsset dataAsset = this.wugSkeletonDataAssets[(int)wugConfig.WugGrowthType];
					CommonUtils.SetSkeletonDataAsset(this.wugSkeleton, dataAsset, "default", "animation", true);
					string slotOrAttachmentName = InjuryEatItem.WugSkeletonSlotOrAttachmentNames[(int)wugConfig.WugGrowthType];
					string slotName = string.Format("images/{0}", slotOrAttachmentName);
					string attachmentName = string.Format("images/{0}_{1}", slotOrAttachmentName, (int)(wugConfig.WugType + 1));
					this.wugSkeleton.Skeleton.FindSlot(slotName).Attachment = this.wugSkeleton.Skeleton.GetAttachment(slotName, attachmentName);
				}
				this.mouseTip.encyclopediaConfigTypeId = -1;
				bool flag3 = EatingItems.IsWug(itemData.Key) && !EatingItems.IsWugKing(itemData.Key);
				if (flag3)
				{
					this.mouseTip.Type = TipType.EatingWug;
					TooltipInvoker tooltipInvoker = this.mouseTip;
					if (tooltipInvoker.RuntimeParam == null)
					{
						tooltipInvoker.RuntimeParam = new ArgumentBox();
					}
					this.mouseTip.RuntimeParam.Set("TemplateId", itemData.Key.TemplateId);
					this.mouseTip.RuntimeParam.Set("EatingTime", duration);
					this.mouseTip.RuntimeParam.Set("CharId", charId);
				}
				else
				{
					this.mouseTip.Type = TooltipManager.ItemTypeToTipType[itemData.Key.ItemType];
					TooltipInvoker tooltipInvoker = this.mouseTip;
					if (tooltipInvoker.RuntimeParam == null)
					{
						tooltipInvoker.RuntimeParam = new ArgumentBox();
					}
					this.mouseTip.RuntimeParam.SetObject("ItemData", itemData);
					this.mouseTip.RuntimeParam.Set("EatingTime", duration);
					this.mouseTip.RuntimeParam.Set("CharId", charId);
					this.mouseTip.RuntimeParam.Set("AddProfessionBonus", charId == SingletonObject.getInstance<BasicGameData>().TaiwuCharId);
				}
				this.mouseTip.enabled = true;
			}
			else if (showTipsByDefault)
			{
				string tipsContent = LocalStringManager.Get(isUnlocked ? LanguageKey.LK_CharacterMenu_Injury_EatingNull : LanguageKey.LK_CharacterMenu_Injury_EatingLimit);
				this.mouseTip.Type = TipType.Simple;
				this.mouseTip.RuntimeParam = null;
				this.mouseTip.PresetParam = new string[]
				{
					LanguageKey.UI_MonthNotify_Jotting_Eating_Title.Tr(),
					tipsContent
				};
				this.mouseTip.encyclopediaConfigTypeId = 95;
				this.mouseTip.enabled = true;
			}
			else
			{
				this.mouseTip.enabled = false;
			}
			this.icon.SetSprite(iconName, false, null);
			this.normal.SetActive(interactable);
			this.locked.SetActive(!interactable);
			this.highlight.SetActive(false);
		}

		// Token: 0x0600B2E1 RID: 45793 RVA: 0x00517265 File Offset: 0x00515465
		public void SetAreaInteract(Action onClickEatArea)
		{
			this._onClickEatArea = onClickEatArea;
		}

		// Token: 0x0600B2E2 RID: 45794 RVA: 0x0051726F File Offset: 0x0051546F
		private void OnClickEatArea()
		{
			Action onClickEatArea = this._onClickEatArea;
			if (onClickEatArea != null)
			{
				onClickEatArea();
			}
		}

		// Token: 0x04008AC0 RID: 35520
		[SerializeField]
		private CImage icon;

		// Token: 0x04008AC1 RID: 35521
		[SerializeField]
		private SkeletonGraphic wugSkeleton;

		// Token: 0x04008AC2 RID: 35522
		[SerializeField]
		private SkeletonDataAsset[] wugSkeletonDataAssets;

		// Token: 0x04008AC3 RID: 35523
		[SerializeField]
		private GameObject highlight;

		// Token: 0x04008AC4 RID: 35524
		[SerializeField]
		private GameObject mask;

		// Token: 0x04008AC5 RID: 35525
		[SerializeField]
		private CButton button;

		// Token: 0x04008AC6 RID: 35526
		[SerializeField]
		private TooltipInvoker mouseTip;

		// Token: 0x04008AC7 RID: 35527
		[SerializeField]
		private GameObject normal;

		// Token: 0x04008AC8 RID: 35528
		[SerializeField]
		private GameObject locked;

		// Token: 0x04008AC9 RID: 35529
		public static readonly string[] WugSkeletonNames = new string[]
		{
			"Wug1",
			"Wug2",
			"Wug1",
			"Wug2",
			"WugGrown",
			"WugKing"
		};

		// Token: 0x04008ACA RID: 35530
		public static readonly string[] WugSkeletonSlotOrAttachmentNames = new string[]
		{
			"tiny",
			"mid",
			"tiny",
			"mid",
			"big",
			"king"
		};

		// Token: 0x04008ACB RID: 35531
		public const string WugSkeletonSlotTemplate = "images/{0}";

		// Token: 0x04008ACC RID: 35532
		public const string WugSkeletonAttachmentTemplate = "images/{0}_{1}";

		// Token: 0x04008ACD RID: 35533
		private Action _onClickEatArea;
	}
}
