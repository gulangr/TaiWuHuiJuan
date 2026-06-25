using System;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using GameData.Domains.Character.Display;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.SettlementInformation
{
	// Token: 0x0200078C RID: 1932
	public class SettlementChar : MonoBehaviour
	{
		// Token: 0x17000B54 RID: 2900
		// (get) Token: 0x06005DBF RID: 23999 RVA: 0x002B1E43 File Offset: 0x002B0043
		public int CharacterId
		{
			get
			{
				return this._characterDisplayData.CharacterId;
			}
		}

		// Token: 0x17000B55 RID: 2901
		// (get) Token: 0x06005DC0 RID: 24000 RVA: 0x002B1E50 File Offset: 0x002B0050
		public string CharName
		{
			get
			{
				return this.unknown ? "" : this.NameCache;
			}
		}

		// Token: 0x06005DC1 RID: 24001 RVA: 0x002B1E67 File Offset: 0x002B0067
		private void Awake()
		{
			this.tip.Type = TipType.Character;
			this.button.onClick.ResetListener(delegate()
			{
				CharacterDisplayData characterDisplayData = this._characterDisplayData;
				bool flag = characterDisplayData == null || characterDisplayData.CharacterId == -1;
				if (!flag)
				{
					UIElement.CharacterMenu.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("CharacterId", this._characterDisplayData.CharacterId).Set("PreviousView", 7).Set("CanOperate", false));
					UIManager.Instance.ShowUI(UIElement.CharacterMenu, true);
				}
			});
		}

		// Token: 0x17000B56 RID: 2902
		// (get) Token: 0x06005DC2 RID: 24002 RVA: 0x002B1E94 File Offset: 0x002B0094
		// (set) Token: 0x06005DC3 RID: 24003 RVA: 0x002B1E9C File Offset: 0x002B009C
		public bool Interactable
		{
			get
			{
				return this.interactable;
			}
			set
			{
				this.interactable = value;
				this.Refresh();
			}
		}

		// Token: 0x17000B57 RID: 2903
		// (get) Token: 0x06005DC4 RID: 24004 RVA: 0x002B1EAD File Offset: 0x002B00AD
		// (set) Token: 0x06005DC5 RID: 24005 RVA: 0x002B1EB5 File Offset: 0x002B00B5
		public bool Unknown
		{
			get
			{
				return this.unknown;
			}
			set
			{
				this.unknown = value;
				this.Interactable = this.interactable;
			}
		}

		// Token: 0x06005DC6 RID: 24006 RVA: 0x002B1ECC File Offset: 0x002B00CC
		public void Refresh()
		{
			Selectable selectable = this.button;
			Behaviour behaviour = this.tip;
			bool enabled;
			if (!this.unknown)
			{
				CharacterDisplayData characterDisplayData = this._characterDisplayData;
				enabled = (characterDisplayData != null && characterDisplayData.CharacterId != -1);
			}
			else
			{
				enabled = false;
			}
			selectable.interactable = ((behaviour.enabled = enabled) && this.interactable);
			CharacterDisplayData character;
			bool flag;
			if (!this.unknown)
			{
				character = this._characterDisplayData;
				flag = (character == null || character.CharacterId == -1);
			}
			else
			{
				flag = true;
			}
			bool flag2 = flag;
			if (flag2)
			{
				this.avatar.Refresh(this.unknownSp, new Vector2?(Vector2.zero));
				this.avatar.transform.localScale = Vector3.one;
			}
			else
			{
				this.avatar.Refresh(character, true);
				this.avatar.transform.localScale = Vector3.one * 0.75f;
			}
			this.charName.text = (this.unknown ? LanguageKey.LK_Character_Name_NotMetYet.Tr() : this.NameCache);
		}

		// Token: 0x06005DC7 RID: 24007 RVA: 0x002B1FDC File Offset: 0x002B01DC
		public void Set(CharacterDisplayData characterDisplayData)
		{
			this.interactable = true;
			this._characterDisplayData = characterDisplayData;
			this.influence.text = LanguageKey.LK_InfluencePowerInfo.TrFormat(characterDisplayData.InfluencePower);
			this.NameCache = NameCenter.GetMonasticTitleOrDisplayName(characterDisplayData, characterDisplayData.CharacterId == ViewSettlementInformation.TaiwuCharIdCache);
			TooltipInvoker tooltipInvoker = this.tip;
			ArgumentBox argumentBox;
			if ((argumentBox = tooltipInvoker.RuntimeParam) == null)
			{
				argumentBox = (tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>());
			}
			argumentBox.Set("charId", characterDisplayData.CharacterId).Set("locationShow", true);
			this.Unknown = (characterDisplayData.FavorabilityToTaiwu == short.MinValue && this.CharacterId != ViewSettlementInformation.TaiwuCharIdCache);
		}

		// Token: 0x0400408B RID: 16523
		[SerializeField]
		private CButton button;

		// Token: 0x0400408C RID: 16524
		[SerializeField]
		private Game.Components.Avatar.Avatar avatar;

		// Token: 0x0400408D RID: 16525
		[SerializeField]
		private TMP_Text charName;

		// Token: 0x0400408E RID: 16526
		[SerializeField]
		private TMP_Text influence;

		// Token: 0x0400408F RID: 16527
		[SerializeField]
		private Sprite unknownSp;

		// Token: 0x04004090 RID: 16528
		[SerializeField]
		private TooltipInvoker tip;

		// Token: 0x04004091 RID: 16529
		[SerializeField]
		private bool unknown = true;

		// Token: 0x04004092 RID: 16530
		[SerializeField]
		private bool interactable = true;

		// Token: 0x04004093 RID: 16531
		[NonSerialized]
		public int SettlementId = -1;

		// Token: 0x04004094 RID: 16532
		[NonSerialized]
		public string NameCache = string.Empty;

		// Token: 0x04004095 RID: 16533
		private CharacterDisplayData _characterDisplayData;
	}
}
