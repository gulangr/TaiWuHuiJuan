using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork.UISystem.UIElements;
using Game.Views.Legacy.AdventureEditor.Migrate;
using GameData.Adventure;
using GameData.Adventure.Editor;
using GameData.Utilities;
using Property;
using TMPro;
using UnityEngine;

namespace SubEditor.AdventureCommonRefersListEditor
{
	// Token: 0x0200069D RID: 1693
	public class AdventureCharacterDataEditor : AdventureCommonRefersListEditor<AdventureCharacterData>
	{
		// Token: 0x06004F61 RID: 20321 RVA: 0x00252221 File Offset: 0x00250421
		internal static AdventureCharacterData CreateEmptyCharacterData()
		{
			return new AdventureCharacterData
			{
				Type = EAdventureCharacterType.Necessary,
				FilterRuleTemplateId = -1,
				SearchRangeType = 0
			};
		}

		// Token: 0x06004F62 RID: 20322 RVA: 0x00252240 File Offset: 0x00250440
		protected override void OnEnable()
		{
			this.Creator = ((IList<AdventureCharacterData> _) => AdventureCharacterDataEditor.CreateEmptyCharacterData());
			this.RefreshAction = new Action<IList<AdventureCharacterData>, MonoBehaviour, int, Action>(this.DefaultRefresh);
			base.OnEnable();
			this.Start();
		}

		// Token: 0x06004F63 RID: 20323 RVA: 0x00252294 File Offset: 0x00250494
		private void SetOverviewBlocksCharacters()
		{
			this.OverviewBlocks = (from x in AdventureEditorKit.BlackBoard.Editing.Blocks
			select x.Index).ToList<AdventureBlockIndex>();
			this.PrepareRefresh();
			List<AdventureCharacterData> data = this._overviewBlocksCache.Keys.ToList<AdventureCharacterData>();
			for (int i = data.Count - 1; i >= 0; i--)
			{
				AdventureCharacterData based = data[i];
				List<AdventureBlockIndex> blockList = this._overviewBlocksCache[based];
				for (int j = 1; j < blockList.Count; j++)
				{
					data.Insert(i, based);
				}
			}
			base.SetEditableList(data);
		}

		// Token: 0x06004F64 RID: 20324 RVA: 0x0025235C File Offset: 0x0025055C
		private void PrepareRefresh()
		{
			this._overviewBlocksCache.Clear();
			bool flag = this.OverviewBlocks != null;
			if (flag)
			{
				AdventureEditorKit.UpdateElementCache();
				foreach (AdventureBlockSnapshot block in from x in AdventureEditorKit.BlackBoard.Editing.Blocks
				where this.OverviewBlocks.Contains(x.Index)
				select x)
				{
					foreach (AdventureElementSnapshot elementData in block.ElementCoreIds.Select(new Func<int, AdventureElementSnapshot>(AdventureEditorKit.GetElementFromCache)))
					{
						AdventureCharacterData characterData = elementData.CharacterData;
						bool flag2 = characterData == null;
						if (!flag2)
						{
							List<AdventureBlockIndex> blockList = this._overviewBlocksCache.GetOrNew(characterData);
							blockList.Add(block.Index);
						}
					}
				}
			}
			bool flag3 = this.EditCharacterIdSetter != null && this.EditCharacterIdGetter != null;
			if (flag3)
			{
				bool flag4 = this.dropdownDataTargetMode != null;
				if (flag4)
				{
					SingletonObject.getInstance<YieldHelper>().DelayFrameDo(2U, delegate
					{
						this.dropdownDataTargetMode.SetupEditor("LK_AdventureEditor_Character_DataTargetMode_{0}", delegate(AdventureCharacterDataEditor.DataTargetMode mode)
						{
							switch (mode)
							{
							case AdventureCharacterDataEditor.DataTargetMode.None:
							{
								bool flag7 = this.OriginalEditingOwner != null;
								if (flag7)
								{
									this.OriginalEditingOwner.CharacterData = null;
									this.OriginalEditingOwner.CharacterKey = null;
								}
								this.EditCharacterIdSetter(-1);
								Action<short> editCharacterIdUpdate = this._editCharacterIdUpdate;
								if (editCharacterIdUpdate != null)
								{
									editCharacterIdUpdate(-1);
								}
								break;
							}
							case AdventureCharacterDataEditor.DataTargetMode.TemplateIdOnly:
							{
								bool flag8 = this.OriginalEditingOwner != null;
								if (flag8)
								{
									this.OriginalEditingOwner.CharacterData = null;
									this.OriginalEditingOwner.CharacterKey = null;
								}
								CDropdown cdropdown = this.dropdownCharacterTemplateId;
								if (cdropdown != null)
								{
									CDropdown.DropdownEvent onValueChanged = cdropdown.onValueChanged;
									if (onValueChanged != null)
									{
										onValueChanged.Invoke(this.dropdownCharacterTemplateId.value);
									}
								}
								break;
							}
							case AdventureCharacterDataEditor.DataTargetMode.CharacterDataOnly:
							{
								bool flag9 = this.OriginalEditingOwner != null;
								if (flag9)
								{
									this.OriginalEditingOwner.CharacterData = this.List[0];
									this.OriginalEditingOwner.CharacterKey = null;
								}
								this.EditCharacterIdSetter(-1);
								Action<short> editCharacterIdUpdate2 = this._editCharacterIdUpdate;
								if (editCharacterIdUpdate2 != null)
								{
									editCharacterIdUpdate2(-1);
								}
								break;
							}
							case AdventureCharacterDataEditor.DataTargetMode.ReservedCharacter:
							{
								bool flag10 = this.OriginalEditingOwner != null;
								if (flag10)
								{
									this.OriginalEditingOwner.CharacterData = null;
									AdventureElementSnapshot originalEditingOwner = this.OriginalEditingOwner;
									TMP_InputField tmp_InputField = this.characterKeyInput;
									originalEditingOwner.CharacterKey = ((tmp_InputField != null) ? tmp_InputField.text : null);
								}
								this.EditCharacterIdSetter(-1);
								Action<short> editCharacterIdUpdate3 = this._editCharacterIdUpdate;
								if (editCharacterIdUpdate3 != null)
								{
									editCharacterIdUpdate3(-1);
								}
								break;
							}
							default:
								throw new ArgumentOutOfRangeException("mode", mode, null);
							}
						}, delegate(AdventureCharacterDataEditor.DataTargetMode mode)
						{
							if (!true)
							{
							}
							bool result;
							switch (mode)
							{
							case AdventureCharacterDataEditor.DataTargetMode.None:
							{
								bool flag7;
								if (this.EditCharacterIdGetter() < 0)
								{
									AdventureElementSnapshot originalEditingOwner = this.OriginalEditingOwner;
									if (originalEditingOwner != null && originalEditingOwner.CharacterData == null)
									{
										flag7 = string.IsNullOrEmpty(this.OriginalEditingOwner.CharacterKey);
										goto IL_53;
									}
								}
								flag7 = false;
								IL_53:
								result = flag7;
								break;
							}
							case AdventureCharacterDataEditor.DataTargetMode.TemplateIdOnly:
								result = (this.EditCharacterIdGetter() >= 0);
								break;
							case AdventureCharacterDataEditor.DataTargetMode.CharacterDataOnly:
							{
								AdventureElementSnapshot originalEditingOwner = this.OriginalEditingOwner;
								result = (originalEditingOwner != null && originalEditingOwner.CharacterData != null);
								break;
							}
							case AdventureCharacterDataEditor.DataTargetMode.ReservedCharacter:
							{
								AdventureElementSnapshot originalEditingOwner = this.OriginalEditingOwner;
								result = (originalEditingOwner != null && originalEditingOwner.CharacterKey != null && !string.IsNullOrEmpty(this.OriginalEditingOwner.CharacterKey));
								break;
							}
							default:
								result = false;
								break;
							}
							if (!true)
							{
							}
							return result;
						}, true);
					});
				}
				bool flag5 = this.dropdownCharacterTemplateId != null;
				if (flag5)
				{
					SingletonObject.getInstance<YieldHelper>().DelayFrameDo(1U, delegate
					{
						this.dropdownCharacterTemplateId.SetupEditor(Character.Instance.RefNameMap.Where(delegate(KeyValuePair<string, int> pair)
						{
							bool flag7 = pair.Value < 0;
							bool result;
							if (flag7)
							{
								result = true;
							}
							else
							{
								CharacterItem template = Character.Instance.GetItem((short)pair.Value);
								bool flag8;
								if (template != null)
								{
									byte creatingType = template.CreatingType;
									if (creatingType - 2 <= 1)
									{
										flag8 = true;
										goto IL_40;
									}
								}
								flag8 = false;
								IL_40:
								result = flag8;
							}
							return result;
						}), (KeyValuePair<string, int> pair) => pair.Key, delegate(KeyValuePair<string, int> pair)
						{
							this.EditCharacterIdSetter((short)pair.Value);
							Action<short> editCharacterIdUpdate = this._editCharacterIdUpdate;
							if (editCharacterIdUpdate != null)
							{
								editCharacterIdUpdate((short)pair.Value);
							}
						}, (KeyValuePair<string, int> pair) => pair.Value == (int)this.EditCharacterIdGetter(), true);
					});
				}
				bool flag6 = this.characterKeyInput != null;
				if (flag6)
				{
					SingletonObject.getInstance<YieldHelper>().DelayFrameDo(1U, delegate
					{
						this.characterKeyInput.characterValidation = TMP_InputField.CharacterValidation.CustomValidator;
						this.characterKeyInput.onValidateInput = ((string text, int charIndex, char addedChar) => (char.IsLetterOrDigit(addedChar) || addedChar == '_') ? addedChar : '\0');
						this.characterKeyInput.onValueChanged.RemoveAllListeners();
						this.characterKeyInput.onValueChanged.AddListener(delegate(string v)
						{
							bool flag8 = this.OriginalEditingOwner != null;
							if (flag8)
							{
								this.OriginalEditingOwner.CharacterKey = v;
							}
						});
						bool flag7 = this.OriginalEditingOwner != null;
						if (flag7)
						{
							this.characterKeyInput.text = (this.OriginalEditingOwner.CharacterKey ?? string.Empty);
						}
					});
				}
			}
			this._editCharacterIdUpdate = null;
		}

		// Token: 0x06004F65 RID: 20325 RVA: 0x00252510 File Offset: 0x00250710
		protected override void Refresh(IList<AdventureCharacterData> list)
		{
			bool flag = this.overviewMode;
			if (flag)
			{
				this.SetOverviewBlocksCharacters();
				base.Refresh(this.List);
			}
			else
			{
				this.PrepareRefresh();
				base.Refresh(list);
			}
		}

		// Token: 0x06004F66 RID: 20326 RVA: 0x00252554 File Offset: 0x00250754
		private void DefaultRefresh(IList<AdventureCharacterData> subList, MonoBehaviour subUnit, int subIndex, Action subOnUpdate)
		{
			AdventureCharacterData data = subList[subIndex];
			AdventureCharacterDataEditorTemplate template = subUnit.GetComponent<AdventureCharacterDataEditorTemplate>();
			base.FixColumns(subUnit.GetComponent<RectTransform>());
			TextMeshProUGUI title = template.nameText;
			CDropdown dropDownType = template.type;
			CDropdown dropDownFilterRule = template.filterRule;
			CDropdown dropDownSearchRange = template.searchRange;
			dropDownType.SetupEditor(Enum.GetValues(typeof(EAdventureCharacterType)).Cast<EAdventureCharacterType>(), (EAdventureCharacterType type) => LocalStringManager.Get("LK_AdventureEditor_AdventureCharacterType_" + type.ToString()), delegate(EAdventureCharacterType type)
			{
				data.Type = type;
			}, (EAdventureCharacterType type) => type == data.Type, true);
			dropDownFilterRule.SetupEditor(from pair in CharacterFilterRules.Instance.RefNameMap
			orderby pair.Value
			select pair, (KeyValuePair<string, int> pair) => pair.Key, delegate(KeyValuePair<string, int> pair)
			{
				data.FilterRuleTemplateId = (int)((short)pair.Value);
			}, (KeyValuePair<string, int> pair) => pair.Value == data.FilterRuleTemplateId, true);
			dropDownSearchRange.SetupEditor(new sbyte[]
			{
				0,
				1,
				2
			}, (sbyte r) => LocalStringManager.Get(string.Format("LK_AdventureEditor_CharacterSearchRange_{0}", r)), delegate(sbyte r)
			{
				data.SearchRangeType = (int)r;
			}, (sbyte r) => (int)r == data.SearchRangeType, true);
			this._editCharacterIdUpdate = delegate(short _)
			{
				bool flag2 = this.dropdownCharacterTemplateId && this.dropdownDataTargetMode;
				if (flag2)
				{
					this.dropdownCharacterTemplateId.gameObject.SetActive(this.dropdownDataTargetMode.value == 1);
				}
				bool flag3 = this.characterKeyInput && this.dropdownDataTargetMode;
				if (flag3)
				{
					this.characterKeyInput.gameObject.SetActive(this.dropdownDataTargetMode.value == 3);
				}
				bool flag4 = this.OriginalEditingOwner == null;
				if (!flag4)
				{
					bool hasData = this.OriginalEditingOwner.CharacterData != null;
					subUnit.gameObject.SetActive(hasData);
					this.columnsHeader.gameObject.SetActive(hasData);
				}
			};
			List<AdventureBlockIndex> blockList;
			bool flag = this._overviewBlocksCache.TryGetValue(data, out blockList) && blockList.Count > 0;
			bool readOnly;
			if (flag)
			{
				title.text = blockList[0].ToString();
				blockList.RemoveAt(0);
				readOnly = true;
			}
			else
			{
				title.text = string.Empty;
				readOnly = false;
			}
			dropDownType.interactable = (dropDownFilterRule.interactable = (dropDownSearchRange.interactable = !readOnly));
		}

		// Token: 0x04003699 RID: 13977
		[SerializeField]
		private CDropdown dropdownDataTargetMode;

		// Token: 0x0400369A RID: 13978
		[SerializeField]
		private CDropdown dropdownCharacterTemplateId;

		// Token: 0x0400369B RID: 13979
		[SerializeField]
		private TMP_InputField characterKeyInput;

		// Token: 0x0400369C RID: 13980
		[SerializeField]
		private bool overviewMode;

		// Token: 0x0400369D RID: 13981
		[NonSerialized]
		public Action<short> EditCharacterIdSetter;

		// Token: 0x0400369E RID: 13982
		[NonSerialized]
		public Func<short> EditCharacterIdGetter;

		// Token: 0x0400369F RID: 13983
		public IReadOnlyList<AdventureBlockIndex> OverviewBlocks;

		// Token: 0x040036A0 RID: 13984
		private readonly Dictionary<AdventureCharacterData, List<AdventureBlockIndex>> _overviewBlocksCache = new Dictionary<AdventureCharacterData, List<AdventureBlockIndex>>();

		// Token: 0x040036A1 RID: 13985
		private Action<short> _editCharacterIdUpdate;

		// Token: 0x040036A2 RID: 13986
		internal AdventureElementSnapshot OriginalEditingOwner;

		// Token: 0x02001AE3 RID: 6883
		private enum DataTargetMode
		{
			// Token: 0x0400B748 RID: 46920
			None,
			// Token: 0x0400B749 RID: 46921
			TemplateIdOnly,
			// Token: 0x0400B74A RID: 46922
			CharacterDataOnly,
			// Token: 0x0400B74B RID: 46923
			ReservedCharacter
		}
	}
}
