using System;
using System.Collections.Generic;
using FrameWork;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Character.AvatarSystem;
using GameData.Domains.Global;
using GameData.Domains.Global.Inscription;
using GameData.GameDataBridge;
using TMPro;
using UnityEngine;

namespace Game.Views.NewGame
{
	// Token: 0x020007FB RID: 2043
	public class NewGameSubPageAvatarPresetPage : NewGameSubPageAvatarPageBase
	{
		// Token: 0x060063B7 RID: 25527 RVA: 0x002DB4FA File Offset: 0x002D96FA
		public override void Init(IAvatarSubPageParent avatarPage)
		{
			base.Init(avatarPage);
			GEvent.Add(EEvents.InscriptionChange, new GEvent.Callback(this.OnInscriptionChange));
		}

		// Token: 0x060063B8 RID: 25528 RVA: 0x002DB51E File Offset: 0x002D971E
		public override void UpdateUI()
		{
			this.RefreshAllItems();
		}

		// Token: 0x060063B9 RID: 25529 RVA: 0x002DB528 File Offset: 0x002D9728
		public void SetPresetType(bool isCustom)
		{
			bool flag = this._isCustomPreset != isCustom;
			if (flag)
			{
				this._selectedPresetIndex = -1;
				this._selectedInscriptionKey = InscribedCharacterKey.Invalid;
			}
			this._isCustomPreset = isCustom;
			this.RefreshAllItems();
		}

		// Token: 0x060063BA RID: 25530 RVA: 0x002DB568 File Offset: 0x002D9768
		public void RefreshAllItems()
		{
			this.LoadData();
			this.RefreshUI();
		}

		// Token: 0x060063BB RID: 25531 RVA: 0x002DB57C File Offset: 0x002D977C
		private void LoadData()
		{
			this._presets.Clear();
			bool isCustomPreset = this._isCustomPreset;
			if (isCustomPreset)
			{
				this._presets = NewGameSubPageAvatarPresetHelper.LoadCustomPresets();
			}
			else
			{
				this._presets = NewGameSubPageAvatarPresetHelper.LoadBuiltinPresets();
			}
			this._inscribedChars.Clear();
			bool isCustomPreset2 = this._isCustomPreset;
			if (isCustomPreset2)
			{
				Dictionary<InscribedCharacterKey, InscribedCharacter> inscribedChars = GlobalOperations.InscribedCharacters;
				bool flag = inscribedChars != null;
				if (flag)
				{
					foreach (KeyValuePair<InscribedCharacterKey, InscribedCharacter> kv in inscribedChars)
					{
						this._inscribedChars.Add(kv);
					}
				}
			}
		}

		// Token: 0x060063BC RID: 25532 RVA: 0x002DB634 File Offset: 0x002D9834
		private void RefreshUI()
		{
			bool isCustomPreset = this._isCustomPreset;
			if (isCustomPreset)
			{
				this.inscriptionGroup.SetActive(this._inscribedChars.Count > 0);
				this.presetGroup.SetActive(true);
				this.RefreshInscriptionGroup();
				this.RefreshPresetGroup();
			}
			else
			{
				this.inscriptionGroup.SetActive(false);
				this.presetGroup.SetActive(true);
				this.RefreshPresetGroup();
			}
		}

		// Token: 0x060063BD RID: 25533 RVA: 0x002DB6A8 File Offset: 0x002D98A8
		private void RefreshInscriptionGroup()
		{
			bool flag = this.inscriptionTitleText != null;
			if (flag)
			{
				this.inscriptionTitleText.text = LanguageKey.LK_NewGame_Avatar_SubPage_Inscription.Tr();
			}
			CommonUtils.PrepareEnoughChildren(this.inscriptionGrid, this.inscriptionItemTemplate, this._inscribedChars.Count, null);
			for (int i = 0; i < this._inscribedChars.Count; i++)
			{
				Transform child = this.inscriptionGrid.GetChild(i);
				InscriptionPresetItem item = child.GetComponent<InscriptionPresetItem>();
				bool flag2 = item == null;
				if (!flag2)
				{
					item.Init(this);
					KeyValuePair<InscribedCharacterKey, InscribedCharacter> kv = this._inscribedChars[i];
					item.Refresh(kv.Key, kv.Value, kv.Key.Equals(this._selectedInscriptionKey));
				}
			}
		}

		// Token: 0x060063BE RID: 25534 RVA: 0x002DB790 File Offset: 0x002D9990
		public void OnSelectInscribedCharacter(InscribedCharacterKey key, InscribedCharacter character)
		{
			InscribedCharacterKey previousKey = this._selectedInscriptionKey;
			this._selectedPresetIndex = -1;
			this._selectedInscriptionKey = key;
			bool flag = !previousKey.Equals(this._selectedInscriptionKey);
			if (flag)
			{
				this.RefreshInscriptionGroup();
				this.RefreshPresetGroup();
			}
			bool flag2 = this.AvatarPage != null && !this.AvatarPage.IsAvatarDirty();
			if (flag2)
			{
				this.AvatarPage.ApplyInscribedCharacter(key, character);
			}
			else
			{
				DialogCmd cmd = new DialogCmd
				{
					Title = LanguageKey.LK_NewGame_Avatar_Use_Preset_Dialog_Title.Tr(),
					Content = LanguageKey.LK_NewGame_Avatar_Use_Preset_Dialog_Content.Tr(),
					Type = 0,
					Yes = delegate()
					{
						IAvatarSubPageParent avatarPage = this.AvatarPage;
						if (avatarPage != null)
						{
							avatarPage.ApplyInscribedCharacter(key, character);
						}
					},
					No = delegate()
					{
						this._selectedInscriptionKey = previousKey;
						this.RefreshInscriptionGroup();
					}
				};
				UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd));
				UIManager.Instance.MaskUI(UIElement.Dialog);
			}
		}

		// Token: 0x060063BF RID: 25535 RVA: 0x002DB8B4 File Offset: 0x002D9AB4
		public void OnDeselectInscribedCharacter(InscribedCharacterKey key)
		{
			bool flag = key.Equals(this._selectedInscriptionKey);
			if (flag)
			{
				this._selectedInscriptionKey = InscribedCharacterKey.Invalid;
				this.RefreshInscriptionGroup();
			}
		}

		// Token: 0x060063C0 RID: 25536 RVA: 0x002DB8E8 File Offset: 0x002D9AE8
		public void OnDeleteInscribedCharacter(InscribedCharacterKey key)
		{
			GlobalDomainMethod.Call.RemoveInscribedCharacter(key);
			bool flag = key.Equals(this._selectedInscriptionKey);
			if (flag)
			{
				this._selectedInscriptionKey = InscribedCharacterKey.Invalid;
			}
			this.LoadData();
			this.RefreshUI();
		}

		// Token: 0x060063C1 RID: 25537 RVA: 0x002DB92C File Offset: 0x002D9B2C
		private void OnInscriptionChange(ArgumentBox _ = null)
		{
			bool flag = !this._isCustomPreset;
			if (!flag)
			{
				this.LoadData();
				this.RefreshUI();
			}
		}

		// Token: 0x060063C2 RID: 25538 RVA: 0x002DB958 File Offset: 0x002D9B58
		private void RefreshPresetGroup()
		{
			bool flag = this.presetTitleText != null;
			if (flag)
			{
				bool isCustomPreset = this._isCustomPreset;
				if (isCustomPreset)
				{
					this.presetTitleText.text = LanguageKey.LK_NewGame_Avatar_SubPage_CustomPreset.TrFormat(this._presets.Count, 21);
				}
				else
				{
					this.presetTitleText.text = LanguageKey.LK_NewGame_Avatar_SubPage_FixedPreset.Tr();
				}
			}
			int displayCount = this._presets.Count;
			bool flag2 = this._isCustomPreset && this._presets.Count < 21;
			if (flag2)
			{
				displayCount++;
			}
			CommonUtils.PrepareEnoughChildren(this.presetGrid, this.presetItemTemplate, displayCount, null);
			for (int i = 0; i < displayCount; i++)
			{
				Transform child = this.presetGrid.GetChild(i);
				AvatarPresetItem item = child.GetComponent<AvatarPresetItem>();
				bool flag3 = item == null;
				if (!flag3)
				{
					item.Init(this);
					bool isEmptySlot = this._isCustomPreset && i == this._presets.Count && this._presets.Count < 21;
					bool flag4 = isEmptySlot;
					if (flag4)
					{
						item.RefreshEmpty(i);
					}
					else
					{
						item.Refresh(i, this._presets[i], this._isCustomPreset, i == this._selectedPresetIndex);
					}
				}
			}
		}

		// Token: 0x060063C3 RID: 25539 RVA: 0x002DBAD0 File Offset: 0x002D9CD0
		public void OnSelectPreset(int index)
		{
			bool flag = index < 0 || index >= this._presets.Count;
			if (!flag)
			{
				int previousIndex = this._selectedPresetIndex;
				this._selectedInscriptionKey = InscribedCharacterKey.Invalid;
				this._selectedPresetIndex = index;
				bool flag2 = previousIndex != this._selectedPresetIndex;
				if (flag2)
				{
					this.RefreshInscriptionGroup();
					this.RefreshPresetGroup();
				}
				AvatarPreset preset = this._presets[index];
				bool flag3 = this.AvatarPage != null && !this.AvatarPage.IsAvatarDirty();
				if (flag3)
				{
					this.AvatarPage.ApplyPreset(preset);
				}
				else
				{
					DialogCmd cmd = new DialogCmd
					{
						Title = LanguageKey.LK_NewGame_Avatar_Use_Preset_Dialog_Title.Tr(),
						Content = LanguageKey.LK_NewGame_Avatar_Use_Preset_Dialog_Content.Tr(),
						Type = 0,
						Yes = delegate()
						{
							IAvatarSubPageParent avatarPage = this.AvatarPage;
							if (avatarPage != null)
							{
								avatarPage.ApplyPreset(preset);
							}
						},
						No = delegate()
						{
							this._selectedPresetIndex = previousIndex;
							this.RefreshPresetGroup();
						}
					};
					UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd));
					UIManager.Instance.MaskUI(UIElement.Dialog);
				}
			}
		}

		// Token: 0x060063C4 RID: 25540 RVA: 0x002DBC14 File Offset: 0x002D9E14
		public void OnDeselectPreset(int index)
		{
			bool flag = index == this._selectedPresetIndex;
			if (flag)
			{
				this._selectedPresetIndex = -1;
				this.RefreshPresetGroup();
			}
		}

		// Token: 0x060063C5 RID: 25541 RVA: 0x002DBC40 File Offset: 0x002D9E40
		public void OnDeletePreset(int index)
		{
			bool flag = index < 0 || index >= this._presets.Count;
			if (!flag)
			{
				this._presets.RemoveAt(index);
				NewGameSubPageAvatarPresetHelper.SaveCustomPresets(this._presets);
				this.RefreshUI();
				IAvatarSubPageParent avatarPage = this.AvatarPage;
				if (avatarPage != null)
				{
					avatarPage.TrySavePendingPreset();
				}
			}
		}

		// Token: 0x060063C6 RID: 25542 RVA: 0x002DBCA0 File Offset: 0x002D9EA0
		public void ScrollToLast()
		{
			bool flag = this.scrollRect == null || this.presetGrid == null;
			if (!flag)
			{
				bool flag2 = this._presets.Count == 0;
				if (!flag2)
				{
					RectTransform lastItem = this.presetGrid.GetChild(this._presets.Count - 1) as RectTransform;
					bool flag3 = lastItem != null;
					if (flag3)
					{
						this.scrollRect.ScrollTo(lastItem, 0.3f);
					}
				}
			}
		}

		// Token: 0x060063C7 RID: 25543 RVA: 0x002DBD24 File Offset: 0x002D9F24
		public void OnRenamePreset(int index, string newName)
		{
			bool flag = index < 0 || index >= this._presets.Count;
			if (!flag)
			{
				AvatarPreset preset = this._presets[index];
				preset.Name = newName;
				this._presets[index] = preset;
				NewGameSubPageAvatarPresetHelper.SaveCustomPresets(this._presets);
				this.RefreshUI();
			}
		}

		// Token: 0x060063C8 RID: 25544 RVA: 0x002DBD88 File Offset: 0x002D9F88
		public void OnAddPreset()
		{
			bool flag = this._presets.Count >= 21;
			if (!flag)
			{
				IAvatarSubPageParent avatarPage = this.AvatarPage;
				AvatarData currentAvatarData = (avatarPage != null) ? avatarPage.GetCurrentAvatarData() : null;
				bool flag2 = currentAvatarData == null;
				if (!flag2)
				{
					AvatarData newData = new AvatarData(currentAvatarData);
					newData.ClothDisplayId = currentAvatarData.ClothDisplayId;
					AvatarPreset avatarPreset = default(AvatarPreset);
					avatarPreset.Name = LanguageKey.LK_NewGame_Avatar_Default_Preset_Avatar_Name.TrFormat(this._presets.Count + 1);
					avatarPreset.Data = newData;
					IAvatarSubPageParent avatarPage2 = this.AvatarPage;
					avatarPreset.IsTransGender = (avatarPage2 != null && avatarPage2.GetIsTransGender());
					IAvatarSubPageParent avatarPage3 = this.AvatarPage;
					avatarPreset.Gender = ((avatarPage3 != null) ? avatarPage3.GetGender() : 1);
					AvatarPreset newPreset = avatarPreset;
					this._presets.Add(newPreset);
					NewGameSubPageAvatarPresetHelper.SaveCustomPresets(this._presets);
					this.RefreshUI();
				}
			}
		}

		// Token: 0x060063C9 RID: 25545 RVA: 0x002DBE70 File Offset: 0x002DA070
		public void ClearSelection()
		{
			bool changed = false;
			bool flag = this._selectedPresetIndex != -1;
			if (flag)
			{
				this._selectedPresetIndex = -1;
				changed = true;
			}
			bool flag2 = !this._selectedInscriptionKey.Equals(InscribedCharacterKey.Invalid);
			if (flag2)
			{
				this._selectedInscriptionKey = InscribedCharacterKey.Invalid;
				changed = true;
			}
			bool flag3 = changed;
			if (flag3)
			{
				this.RefreshInscriptionGroup();
				this.RefreshPresetGroup();
			}
		}

		// Token: 0x060063CA RID: 25546 RVA: 0x002DBED6 File Offset: 0x002DA0D6
		private void OnDestroy()
		{
			GEvent.Remove(EEvents.InscriptionChange, new GEvent.Callback(this.OnInscriptionChange));
		}

		// Token: 0x04004592 RID: 17810
		[Header("铭刻人物组")]
		[SerializeField]
		private GameObject inscriptionGroup;

		// Token: 0x04004593 RID: 17811
		[SerializeField]
		private TextMeshProUGUI inscriptionTitleText;

		// Token: 0x04004594 RID: 17812
		[SerializeField]
		private Transform inscriptionGrid;

		// Token: 0x04004595 RID: 17813
		[SerializeField]
		private GameObject inscriptionItemTemplate;

		// Token: 0x04004596 RID: 17814
		[Header("预设组")]
		[SerializeField]
		private GameObject presetGroup;

		// Token: 0x04004597 RID: 17815
		[SerializeField]
		private TextMeshProUGUI presetTitleText;

		// Token: 0x04004598 RID: 17816
		[SerializeField]
		private Transform presetGrid;

		// Token: 0x04004599 RID: 17817
		[SerializeField]
		private GameObject presetItemTemplate;

		// Token: 0x0400459A RID: 17818
		[Header("滚动")]
		[SerializeField]
		private CScrollRect scrollRect;

		// Token: 0x0400459B RID: 17819
		private List<AvatarPreset> _presets = new List<AvatarPreset>();

		// Token: 0x0400459C RID: 17820
		private List<KeyValuePair<InscribedCharacterKey, InscribedCharacter>> _inscribedChars = new List<KeyValuePair<InscribedCharacterKey, InscribedCharacter>>();

		// Token: 0x0400459D RID: 17821
		private bool _isCustomPreset;

		// Token: 0x0400459E RID: 17822
		private int _selectedPresetIndex = -1;

		// Token: 0x0400459F RID: 17823
		private InscribedCharacterKey _selectedInscriptionKey = InscribedCharacterKey.Invalid;
	}
}
