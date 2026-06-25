using System;
using System.Linq;
using System.Runtime.CompilerServices;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using GameData.Domains.Character.Display;
using GameData.Domains.Combat;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.LifeSkillCombat
{
	// Token: 0x02000986 RID: 2438
	public class LifeSkillCombatBeginTeammateItem : MonoBehaviour
	{
		// Token: 0x17000D35 RID: 3381
		// (get) Token: 0x0600751E RID: 29982 RVA: 0x003690C4 File Offset: 0x003672C4
		private ViewLifeSkillCombatBegin Parent
		{
			get
			{
				return UIElement.LifeSkillCombatBegin.UiBaseAs<ViewLifeSkillCombatBegin>();
			}
		}

		// Token: 0x0600751F RID: 29983 RVA: 0x003690D0 File Offset: 0x003672D0
		private void Awake()
		{
			this.pointerTrigger.EnterEvent.ResetListener(delegate()
			{
				this.hover.gameObject.SetActive(this.charBtn.interactable);
			});
			this.pointerTrigger.ExitEvent.ResetListener(delegate()
			{
				this.hover.gameObject.SetActive(false);
			});
			this.charBtn.ClearAndAddListener(new Action(this.<Awake>g__SelectTeammate|15_2));
			this.changeBtn.ClearAndAddListener(new Action(this.<Awake>g__SelectTeammate|15_2));
		}

		// Token: 0x06007520 RID: 29984 RVA: 0x00369149 File Offset: 0x00367349
		private void OnEnable()
		{
			this.avatar.gameObject.SetActive(this._isAvatarInit);
		}

		// Token: 0x06007521 RID: 29985 RVA: 0x00369164 File Offset: 0x00367364
		public void Set(bool isTaiwu, int index, CharacterDisplayData characterDisplayData)
		{
			this._isTaiwu = isTaiwu;
			this._index = index;
			this._characterDisplayData = characterDisplayData;
			this.levelObj.SetActive(this._characterDisplayData != null);
			this.charHolder.gameObject.SetActive(this._characterDisplayData != null);
			this.changeBtn.GetComponent<CImage>().enabled = (this._isTaiwu && this.Parent.AvailableSpectatorCount > 0);
			this.charBtn.interactable = this._isTaiwu;
			TooltipInvoker tip = base.GetComponent<TooltipInvoker>();
			TooltipInvoker tooltipInvoker = tip;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>();
			}
			bool flag = this._characterDisplayData == null;
			if (flag)
			{
				tip.Type = TipType.SingleDesc;
				tip.RuntimeParam.Set("arg0", LanguageKey.LK_LifeSkillCombat_Audience_Tips.Tr());
			}
			else
			{
				this.avatar.Refresh(this._characterDisplayData, true);
				this._isAvatarInit = true;
				this.avatar.gameObject.SetActive(this._isAvatarInit);
				this.charName.text = NameCenter.GetMonasticTitleOrDisplayName(this._characterDisplayData, false);
				tip.Type = TipType.LifeSkillCombatAudience;
				tip.RuntimeParam.SetObject("CharData", this._characterDisplayData);
				this.SetSpeakingTagAndDefeatMarks();
			}
		}

		// Token: 0x06007522 RID: 29986 RVA: 0x003692B8 File Offset: 0x003674B8
		private void SetSpeakingTagAndDefeatMarks()
		{
			bool flag = this._characterDisplayData.CreatingType == 2;
			if (flag)
			{
				this.<SetSpeakingTagAndDefeatMarks>g__SetLevelText|18_1(0);
			}
			else
			{
				CombatDomainMethod.AsyncCall.GetDefeatMarksCountOutOfCombat(null, this._characterDisplayData.CharacterId, delegate(int offset, RawDataPool dataPool)
				{
					DefeatMarksCountOutOfCombatData markGroupData = new DefeatMarksCountOutOfCombatData();
					Serializer.Deserialize(dataPool, offset, ref markGroupData);
					bool flag2 = markGroupData != null;
					if (flag2)
					{
						int totalCount = markGroupData.DefeatMarksDict.Values.Sum();
						this.<SetSpeakingTagAndDefeatMarks>g__SetLevelText|18_1(totalCount);
					}
				});
			}
		}

		// Token: 0x06007526 RID: 29990 RVA: 0x0036933D File Offset: 0x0036753D
		[CompilerGenerated]
		private void <Awake>g__SelectTeammate|15_2()
		{
			this.Parent.SelectTeammate(this._index);
		}

		// Token: 0x06007528 RID: 29992 RVA: 0x00369398 File Offset: 0x00367598
		[CompilerGenerated]
		private void <SetSpeakingTagAndDefeatMarks>g__SetLevelText|18_1(int totalCount)
		{
			this.levelText.text = Mathf.Min(totalCount, 99).ToString();
		}

		// Token: 0x040057DC RID: 22492
		[SerializeField]
		private Game.Components.Avatar.Avatar avatar;

		// Token: 0x040057DD RID: 22493
		[SerializeField]
		private TextMeshProUGUI charName;

		// Token: 0x040057DE RID: 22494
		[SerializeField]
		private CButton charBtn;

		// Token: 0x040057DF RID: 22495
		[SerializeField]
		private CButton changeBtn;

		// Token: 0x040057E0 RID: 22496
		[SerializeField]
		private PointerTrigger pointerTrigger;

		// Token: 0x040057E1 RID: 22497
		[SerializeField]
		private CImage hover;

		// Token: 0x040057E2 RID: 22498
		[SerializeField]
		private RectTransform charHolder;

		// Token: 0x040057E3 RID: 22499
		[SerializeField]
		private TextMeshProUGUI levelText;

		// Token: 0x040057E4 RID: 22500
		[SerializeField]
		private GameObject levelObj;

		// Token: 0x040057E5 RID: 22501
		private bool _isTaiwu;

		// Token: 0x040057E6 RID: 22502
		private int _index;

		// Token: 0x040057E7 RID: 22503
		private CharacterDisplayData _characterDisplayData;

		// Token: 0x040057E8 RID: 22504
		private bool _isAvatarInit;
	}
}
