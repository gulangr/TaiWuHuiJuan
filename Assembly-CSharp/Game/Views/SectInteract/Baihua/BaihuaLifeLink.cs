using System;
using System.Text;
using Config;
using DisplayConfig;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using Game.Views.CharacterMenu;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.SectInteract.Baihua
{
	// Token: 0x020009F2 RID: 2546
	public class BaihuaLifeLink : MonoBehaviour
	{
		// Token: 0x06007D4D RID: 32077 RVA: 0x003A33F4 File Offset: 0x003A15F4
		public void Init(int index, bool isLifeGate, Action<bool> onClickBtnSelect, Action<int, bool> onClickBtnCancel)
		{
			this._index = index;
			this._isLifeGate = isLifeGate;
			this._onClickBtnSelect = onClickBtnSelect;
			this._onClickBtnCancel = onClickBtnCancel;
			this.btnCancel.ClearAndAddListener(new Action(this.OnClickBtnCancel));
			this.btnSelect.ClearAndAddListener(new Action(this.OnClickBtnSelect));
			this.btnDetail.ClearAndAddListener(new Action(this.OpenSelectedCharacterDetail));
			this.pointerTrigger.EnterEvent.RemoveAllListeners();
			this.pointerTrigger.EnterEvent.AddListener(delegate()
			{
				this.hover.SetActive(true);
			});
			this.pointerTrigger.ExitEvent.RemoveAllListeners();
			this.pointerTrigger.ExitEvent.AddListener(delegate()
			{
				this.hover.SetActive(false);
			});
		}

		// Token: 0x06007D4E RID: 32078 RVA: 0x003A34C4 File Offset: 0x003A16C4
		public void OpenSelectedCharacterDetail()
		{
			bool flag = this._characterId <= 0;
			if (!flag)
			{
				UIElement.CharacterMenu.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("CharacterId", this._characterId).Set("PreviousView", 9).SetObject("ViewCharacterMenuTaretPage", new SubPageIndex(ECharacterSubToggleBase.CharacterBase, ECharacterSubPage.None)));
				UIManager.Instance.ShowUI(UIElement.CharacterMenu, true);
			}
		}

		// Token: 0x06007D4F RID: 32079 RVA: 0x003A3538 File Offset: 0x003A1738
		public void Set(CharacterDisplayDataForLifeLink data, int total)
		{
			sbyte actualNeiliType = data.NeiliPercent.GetNeiliType(SingletonObject.getInstance<TimeManager>().GetMonthInCurrYear());
			NeiliTypeItem neiliTypeCfg = NeiliType.Instance[actualNeiliType];
			this._characterId = data.ListData.CharacterId;
			this.avatar.Refresh(data.ListData.AvatarRelatedData, data.ListData.CharacterTemplateId);
			this.neiliType.SetSprite(string.Format("{0}{1}", "ui9_icon_new_game_five_elements_", neiliTypeCfg.FiveElements), false, null);
			this.nameText.SetText(NameCenter.GetMonasticTitleOrDisplayName(ref data.ListData.NameData, data.ListData.CharacterId == SingletonObject.getInstance<BasicGameData>().TaiwuCharId, false), true);
			this.healthBarProgress.fillAmount = (float)data.ListData.Health / (float)Math.Max(1, data.ListData.MaxLeftHealth);
			this.avatar.gameObject.SetActive(true);
			this.label.SetActive(true);
			this.healthBar.SetActive(true);
			this.pointerTrigger.enabled = true;
			this.btnSelect.interactable = true;
			this.avatarTips.enabled = true;
			this.avatarTips.RuntimeParam = new ArgumentBox().Set("charId", data.ListData.CharacterId).Set("locationShow", true).Set("isDreamBack", false);
			this.neiliTips.PresetParam = new string[]
			{
				LanguageKey.LK_NeiliProportionOfFiveElements.Tr(),
				BaihuaLifeLink.GetFiveElementsTips(data.NeiliPercent, total)
			};
			this.healthTips.PresetParam = new string[]
			{
				LanguageKey.LK_Health.Tr(),
				this.GetHealthTips(data.HealthType)
			};
			SpriteState newSpriteState = new SpriteState
			{
				highlightedSprite = this.imageHoverBack,
				selectedSprite = this.imagePressBack,
				pressedSprite = this.imagePressBack,
				disabledSprite = this.btnSelect.spriteState.disabledSprite
			};
			this.btnSelect.image.sprite = this.imageNormalBack;
			this.btnSelect.spriteState = newSpriteState;
		}

		// Token: 0x06007D50 RID: 32080 RVA: 0x003A3780 File Offset: 0x003A1980
		public void SetEmpty()
		{
			this.avatar.gameObject.SetActive(false);
			this.label.SetActive(false);
			this.healthBar.SetActive(false);
			this.pointerTrigger.enabled = false;
			this.avatarTips.enabled = false;
			this.btnSelect.interactable = true;
			SpriteState newSpriteState = new SpriteState
			{
				highlightedSprite = this.imageHover,
				selectedSprite = this.imagePress,
				pressedSprite = this.imagePress,
				disabledSprite = this.btnSelect.spriteState.disabledSprite
			};
			this.btnSelect.image.sprite = this.imageNormal;
			this.btnSelect.spriteState = newSpriteState;
		}

		// Token: 0x06007D51 RID: 32081 RVA: 0x003A3854 File Offset: 0x003A1A54
		public void SetDisable()
		{
			this.avatar.gameObject.SetActive(false);
			this.label.SetActive(false);
			this.healthBar.SetActive(false);
			this.pointerTrigger.enabled = false;
			this.avatarTips.enabled = false;
			this.btnSelect.interactable = false;
		}

		// Token: 0x06007D52 RID: 32082 RVA: 0x003A38B5 File Offset: 0x003A1AB5
		private void OnClickBtnCancel()
		{
			this._onClickBtnCancel(this._index, this._isLifeGate);
		}

		// Token: 0x06007D53 RID: 32083 RVA: 0x003A38D0 File Offset: 0x003A1AD0
		private void OnClickBtnSelect()
		{
			this._onClickBtnSelect(this._isLifeGate);
		}

		// Token: 0x06007D54 RID: 32084 RVA: 0x003A38E8 File Offset: 0x003A1AE8
		public unsafe static string GetFiveElementsTips(NeiliProportionOfFiveElements fiveElements, int sum)
		{
			StringBuilder stringBuilder = EasyPool.Get<StringBuilder>();
			for (sbyte fiveElementType = 0; fiveElementType < 5; fiveElementType += 1)
			{
				FiveElementItem config = FiveElement.Instance[(int)fiveElementType];
				sbyte value = *fiveElements[(int)fiveElementType];
				stringBuilder.Append(LocalStringManager.Get(LanguageKey.LK_Dot_Symbol));
				stringBuilder.AppendFormat("<SpName={0}>", string.Format("{0}{1}", "ui9_icon_mousetip_elements_", fiveElementType));
				stringBuilder.Append(config.Name);
				stringBuilder.Append(LocalStringManager.Get(LanguageKey.LK_Colon_Symbol));
				stringBuilder.Append(value);
				stringBuilder.Append('%');
				bool flag = value > 0 && sum > 0;
				if (flag)
				{
					int rate = (int)(value * 100) / sum;
					bool flag2 = rate > 0;
					if (flag2)
					{
						string rateStr = LocalStringManager.GetFormat(LanguageKey.LK_Brackets_Fix, string.Format("{0}%", rate)).SetColor("brightblue");
						stringBuilder.Append(rateStr);
					}
				}
				stringBuilder.AppendLine();
			}
			string result = stringBuilder.ToString();
			EasyPool.Free<StringBuilder>(stringBuilder);
			return result;
		}

		// Token: 0x06007D55 RID: 32085 RVA: 0x003A3A08 File Offset: 0x003A1C08
		private string GetHealthTips(sbyte healthType)
		{
			StringBuilder stringBuilder = EasyPool.Get<StringBuilder>();
			if (stringBuilder == null)
			{
				stringBuilder = new StringBuilder();
			}
			stringBuilder.Clear();
			stringBuilder.Append(LocalStringManager.Get(LanguageKey.LK_Dot_Symbol));
			stringBuilder.Append(string.Format("<SpName={0}{1}>", "ui9_icon_health_big_", healthType));
			stringBuilder.Append(LocalStringManager.Get(LanguageKey.LK_CharacterHealth));
			stringBuilder.Append(LocalStringManager.Get(LanguageKey.LK_Colon_Symbol));
			stringBuilder.Append(CommonUtils.GetHealthString(healthType));
			string result = stringBuilder.ToString();
			EasyPool.Free<StringBuilder>(stringBuilder);
			return result;
		}

		// Token: 0x04005F49 RID: 24393
		[SerializeField]
		private Game.Components.Avatar.Avatar avatar;

		// Token: 0x04005F4A RID: 24394
		[SerializeField]
		private GameObject hover;

		// Token: 0x04005F4B RID: 24395
		[SerializeField]
		private GameObject label;

		// Token: 0x04005F4C RID: 24396
		[SerializeField]
		private TextMeshProUGUI nameText;

		// Token: 0x04005F4D RID: 24397
		[SerializeField]
		private CImage neiliType;

		// Token: 0x04005F4E RID: 24398
		[SerializeField]
		private CButton btnCancel;

		// Token: 0x04005F4F RID: 24399
		[SerializeField]
		private CButton btnSelect;

		// Token: 0x04005F50 RID: 24400
		[SerializeField]
		private CButton btnDetail;

		// Token: 0x04005F51 RID: 24401
		[SerializeField]
		private GameObject healthBar;

		// Token: 0x04005F52 RID: 24402
		[SerializeField]
		private CImage healthBarProgress;

		// Token: 0x04005F53 RID: 24403
		[SerializeField]
		private PointerTrigger pointerTrigger;

		// Token: 0x04005F54 RID: 24404
		[SerializeField]
		private TooltipInvoker avatarTips;

		// Token: 0x04005F55 RID: 24405
		[SerializeField]
		private TooltipInvoker neiliTips;

		// Token: 0x04005F56 RID: 24406
		[SerializeField]
		private TooltipInvoker healthTips;

		// Token: 0x04005F57 RID: 24407
		[SerializeField]
		private Sprite imageNormal;

		// Token: 0x04005F58 RID: 24408
		[SerializeField]
		private Sprite imageHover;

		// Token: 0x04005F59 RID: 24409
		[SerializeField]
		private Sprite imagePress;

		// Token: 0x04005F5A RID: 24410
		[SerializeField]
		private Sprite imageNormalBack;

		// Token: 0x04005F5B RID: 24411
		[SerializeField]
		private Sprite imageHoverBack;

		// Token: 0x04005F5C RID: 24412
		[SerializeField]
		private Sprite imagePressBack;

		// Token: 0x04005F5D RID: 24413
		private int _index;

		// Token: 0x04005F5E RID: 24414
		private bool _isLifeGate;

		// Token: 0x04005F5F RID: 24415
		private Action<bool> _onClickBtnSelect;

		// Token: 0x04005F60 RID: 24416
		private Action<int, bool> _onClickBtnCancel;

		// Token: 0x04005F61 RID: 24417
		private int _characterId;
	}
}
