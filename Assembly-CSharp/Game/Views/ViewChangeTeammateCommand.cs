using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using Game.Components.Character;
using Game.Components.Common;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Extra;
using GameData.Serializer;
using GameData.Utilities;
using UnityEngine;

namespace Game.Views
{
	// Token: 0x020006F5 RID: 1781
	public class ViewChangeTeammateCommand : UIBase
	{
		// Token: 0x17000A62 RID: 2658
		// (get) Token: 0x0600547A RID: 21626 RVA: 0x00272808 File Offset: 0x00270A08
		private List<sbyte> AvailableCommands
		{
			get
			{
				return this._availableCommandsDict[this.commandType.GetActiveIndex()];
			}
		}

		// Token: 0x0600547B RID: 21627 RVA: 0x00272820 File Offset: 0x00270A20
		public override void OnInit(ArgumentBox argsBox)
		{
			argsBox.Get("CharacterId", out this._characterId);
			UIElement element = this.Element;
			element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(this.RequestData));
		}

		// Token: 0x0600547C RID: 21628 RVA: 0x0027285C File Offset: 0x00270A5C
		private void Awake()
		{
			this.equipped.Init(-1);
			for (int i = 0; i < this.equipped.transform.childCount; i++)
			{
				int index = i;
				this.equipped.transform.GetChild(index).GetChild(1).GetComponent<CButton>().ClearAndAddListener(delegate
				{
					this.OnCancelBtnClick(index);
				});
			}
			this.equipped.OnActiveIndexChange += delegate(int _, int _)
			{
				this.RefreshAvailableCommands();
			};
			this.commandType.Init(-1);
			ToggleGroupHotkeyController.Set(this.Element, this.commandType, 0, null);
			this.commandType.OnActiveIndexChange += this.OnCommandTypeChange;
			this._availableCommandsDict.Clear();
			for (int j = 0; j < this.commandType.Count(); j++)
			{
				this._availableCommandsDict.Add(j, new List<sbyte>());
			}
			this.scroll.InitPageCount();
			this.scroll.OnItemRender += this.OnRenderItem;
		}

		// Token: 0x0600547D RID: 21629 RVA: 0x00272990 File Offset: 0x00270B90
		protected override void OnClick(Transform btn)
		{
			bool flag = btn.name == "ButtonCloseView";
			if (flag)
			{
				this.QuickHide();
			}
		}

		// Token: 0x0600547E RID: 21630 RVA: 0x002729BC File Offset: 0x00270BBC
		public override void QuickHide()
		{
			bool needSave = this._needSave;
			if (needSave)
			{
				ExtraDomainMethod.Call.SetCharTeammateCommandsManual(this.Element.GameDataListenerId, this._characterId, this._equippedCommands);
				this._needSave = false;
				this.QuickHide();
				GEvent.OnEvent(UiEvents.TeammateCommandChanged, EasyPool.Get<ArgumentBox>().Set("CharacterId", this._characterId));
			}
			else
			{
				base.QuickHide();
			}
		}

		// Token: 0x0600547F RID: 21631 RVA: 0x00272A2E File Offset: 0x00270C2E
		private void RequestData()
		{
			this._reqCharacterList.Clear();
			this._reqCharacterList.Add(this._characterId);
			CharacterDomainMethod.AsyncCall.GetGroupCharDisplayDataList(this, this._reqCharacterList, delegate(int offset, RawDataPool pool)
			{
				this._groupCharDisplayDataList.Clear();
				Serializer.Deserialize(pool, offset, ref this._groupCharDisplayDataList);
				this.UpdateCommands();
				this.UpdateMedals();
				this.Element.ShowAfterRefresh();
			});
		}

		// Token: 0x06005480 RID: 21632 RVA: 0x00272A68 File Offset: 0x00270C68
		private void UpdateMedals()
		{
			this.medals.Set(this._groupCharDisplayDataList[0].AttackMedal, this._groupCharDisplayDataList[0].DefenceMedal, this._groupCharDisplayDataList[0].WisdomMedal);
			this.RefreshMedals();
		}

		// Token: 0x06005481 RID: 21633 RVA: 0x00272ABC File Offset: 0x00270CBC
		private void UpdateCommands()
		{
			this._templateId = this._groupCharDisplayDataList[0].CharacterTemplateId;
			this._equippedCommands.Clear();
			this._availableCommandsInConfig.Clear();
			List<sbyte> items = this._groupCharDisplayDataList[0].Command.Items;
			bool flag = items != null;
			if (flag)
			{
				this._equippedCommands.AddRange(items);
			}
			foreach (TeammateCommandItem config in ((IEnumerable<TeammateCommandItem>)Config.TeammateCommand.Instance))
			{
				bool flag2 = config.Type == ETeammateCommandType.Normal && config.MedalType != -1;
				if (flag2)
				{
					this._availableCommandsInConfig.Add(config.TemplateId);
				}
				else
				{
					List<short> compatibleCharacters = config.CompatibleCharacters;
					bool flag3 = compatibleCharacters != null && compatibleCharacters.Count > 0 && config.CompatibleCharacters.Contains(this._templateId);
					if (flag3)
					{
						this._availableCommandsInConfig.Add(config.TemplateId);
					}
				}
			}
			while (this._equippedCommands.Count < 3)
			{
				this._equippedCommands.Add(-1);
			}
			this.CalculateOwnedCommandMedals();
			this.GenerateAvailableCommands();
			this.RefreshEquippedCommands();
			this.RefreshAvailableCommands();
		}

		// Token: 0x06005482 RID: 21634 RVA: 0x00272C18 File Offset: 0x00270E18
		private void RefreshEquippedCommands()
		{
			for (int i = 0; i < this.equipped.transform.childCount; i++)
			{
				Transform item = this.equipped.transform.GetChild(i);
				Transform toggle = item.GetChild(0);
				Transform button = item.GetChild(1);
				bool flag = this._equippedCommands[i] >= 0;
				if (flag)
				{
					sbyte type = Config.TeammateCommand.Instance[this._equippedCommands[i]].MedalType;
					bool canUse = Math.Abs(this.GetHasMedalCount((int)type)) - this._equippedCommandMedalDict[(int)type] >= 0;
					toggle.GetComponent<Game.Components.Character.TeammateCommandLongItem>().Set((short)this._equippedCommands[i], canUse);
					button.GetComponent<CButton>().interactable = true;
				}
				else
				{
					toggle.GetComponent<Game.Components.Character.TeammateCommandLongItem>().Set((short)this._equippedCommands[i], false);
					button.GetComponent<CButton>().interactable = false;
				}
			}
		}

		// Token: 0x06005483 RID: 21635 RVA: 0x00272D1C File Offset: 0x00270F1C
		private void RefreshMedals()
		{
			int attackTotal = Math.Abs(this._groupCharDisplayDataList[0].AttackMedal);
			int attackCurr = attackTotal - Math.Abs(this._equippedCommandMedalDict[0]);
			int defenceTotal = Math.Abs(this._groupCharDisplayDataList[0].DefenceMedal);
			int defenceCurr = defenceTotal - Math.Abs(this._equippedCommandMedalDict[1]);
			int wisdomTotal = Math.Abs(this._groupCharDisplayDataList[0].WisdomMedal);
			int wisdomCurr = wisdomTotal - Math.Abs(this._equippedCommandMedalDict[2]);
			this.medals.SetValue(attackCurr, attackTotal, defenceCurr, defenceTotal, wisdomCurr, wisdomTotal);
		}

		// Token: 0x06005484 RID: 21636 RVA: 0x00272DC2 File Offset: 0x00270FC2
		private void RefreshAvailableCommands()
		{
			this.scroll.SetDataCount(this.AvailableCommands.Count);
		}

		// Token: 0x06005485 RID: 21637 RVA: 0x00272DDC File Offset: 0x00270FDC
		private void OnCancelBtnClick(int index)
		{
			this.equipped.Set(index, false);
			this.OnCurrCommandChange(-1);
		}

		// Token: 0x06005486 RID: 21638 RVA: 0x00272DF5 File Offset: 0x00270FF5
		private void OnCommandTypeChange(int togNew, int _)
		{
			this.RefreshAvailableCommands();
			this.scroll.ScrollTo(0, 0.3f);
		}

		// Token: 0x06005487 RID: 21639 RVA: 0x00272E14 File Offset: 0x00271014
		private void OnRenderItem(int index, GameObject obj)
		{
			CButton btn = obj.GetComponent<CButton>();
			Game.Components.Character.TeammateCommandLongItem item = obj.GetComponent<Game.Components.Character.TeammateCommandLongItem>();
			TeammateCommandItem config = Config.TeammateCommand.Instance[this.AvailableCommands[index]];
			bool available = this.IsCommandAvailable(this.AvailableCommands[index]);
			btn.ClearAndAddListener(delegate
			{
				this.OnAvailableCommandClick(index);
			});
			btn.interactable = available;
			item.Set((short)this.AvailableCommands[index], available);
			item.SetIsDisableByType(available ? -1 : config.MedalType);
		}

		// Token: 0x06005488 RID: 21640 RVA: 0x00272EC3 File Offset: 0x002710C3
		private void OnAvailableCommandClick(int index)
		{
			this.OnCurrCommandChange(this.AvailableCommands[index]);
		}

		// Token: 0x06005489 RID: 21641 RVA: 0x00272EDC File Offset: 0x002710DC
		private void OnCurrCommandChange(sbyte templateId)
		{
			bool flag = templateId >= 0 && !this.IsCommandAvailable(templateId);
			if (!flag)
			{
				this._equippedCommands[this.equipped.GetActiveIndex()] = templateId;
				this._needSave = true;
				this.CalculateOwnedCommandMedals();
				this.RefreshEquippedCommands();
				this.GenerateAvailableCommands();
				this.RefreshAvailableCommands();
				this.UpdateMedals();
			}
		}

		// Token: 0x0600548A RID: 21642 RVA: 0x00272F43 File Offset: 0x00271143
		private void CalculateOwnedCommandMedals()
		{
			CommonUtils.CalculateOwnedCommandMedals(this._equippedCommands, this._equippedCommandMedalDict);
		}

		// Token: 0x0600548B RID: 21643 RVA: 0x00272F58 File Offset: 0x00271158
		private void GenerateAvailableCommands()
		{
			List<sbyte> availableCommands = EasyPool.Get<List<sbyte>>();
			availableCommands.Clear();
			CommonUtils.MergeTeammateCommandList(this._availableCommandsInConfig, this._groupCharDisplayDataList[0].AdvancedCommand.Items, availableCommands);
			for (int i = 0; i < this.commandType.Count(); i++)
			{
				this._availableCommandsDict[i].Clear();
			}
			foreach (sbyte id in availableCommands)
			{
				bool flag = this._equippedCommands.Contains(id);
				if (!flag)
				{
					TeammateCommandItem config = Config.TeammateCommand.Instance[id];
					this._availableCommandsDict[(int)config.MedalType].Add(config.TemplateId);
				}
			}
			foreach (KeyValuePair<int, List<sbyte>> kv in this._availableCommandsDict)
			{
				kv.Value.Sort(new Comparison<sbyte>(this.CompareAvailableCommands));
			}
		}

		// Token: 0x0600548C RID: 21644 RVA: 0x0027309C File Offset: 0x0027129C
		private int CompareAvailableCommands(sbyte x, sbyte y)
		{
			bool xAvailable = this.IsCommandAvailable(x);
			bool yAvailable = this.IsCommandAvailable(y);
			bool flag = xAvailable && !yAvailable;
			int result;
			if (flag)
			{
				result = -1;
			}
			else
			{
				bool flag2 = !xAvailable && yAvailable;
				if (flag2)
				{
					result = 1;
				}
				else
				{
					TeammateCommandItem xConfig = Config.TeammateCommand.Instance[x];
					TeammateCommandItem yConfig = Config.TeammateCommand.Instance[y];
					bool flag3 = xConfig.Type != yConfig.Type;
					if (flag3)
					{
						result = ((xConfig.Type == ETeammateCommandType.Advance) ? -1 : 1);
					}
					else
					{
						result = x.CompareTo(y);
					}
				}
			}
			return result;
		}

		// Token: 0x0600548D RID: 21645 RVA: 0x00273130 File Offset: 0x00271330
		private bool IsCommandAvailable(sbyte commandId)
		{
			bool flag = this._equippedCommands.Contains(commandId);
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				TeammateCommandItem config = Config.TeammateCommand.Instance[commandId];
				sbyte medalType = config.MedalType;
				int needMedalCount = (int)config.MedalCount + this._equippedCommandMedalDict[(int)medalType];
				int selectedIndex = this.equipped.GetActiveIndex();
				bool flag2 = selectedIndex >= 0 && selectedIndex < this._equippedCommands.Count;
				if (flag2)
				{
					sbyte slotCommandId = this._equippedCommands[selectedIndex];
					bool flag3 = slotCommandId >= 0;
					if (flag3)
					{
						TeammateCommandItem slotConfig = Config.TeammateCommand.Instance[slotCommandId];
						bool flag4 = slotConfig.MedalType == medalType;
						if (flag4)
						{
							needMedalCount -= (int)slotConfig.MedalCount;
						}
					}
				}
				int hasMedalCount = Math.Abs(this.GetHasMedalCount((int)medalType));
				result = (hasMedalCount >= needMedalCount);
			}
			return result;
		}

		// Token: 0x0600548E RID: 21646 RVA: 0x0027320C File Offset: 0x0027140C
		private int GetHasMedalCount(int medalType)
		{
			if (!true)
			{
			}
			int result;
			switch (medalType)
			{
			case 0:
				result = this._groupCharDisplayDataList[0].AttackMedal;
				break;
			case 1:
				result = this._groupCharDisplayDataList[0].DefenceMedal;
				break;
			case 2:
				result = this._groupCharDisplayDataList[0].WisdomMedal;
				break;
			default:
				result = 0;
				break;
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x04003935 RID: 14645
		[SerializeField]
		private MedalSummary medals;

		// Token: 0x04003936 RID: 14646
		[SerializeField]
		private CToggleGroup equipped;

		// Token: 0x04003937 RID: 14647
		[SerializeField]
		private CToggleGroup commandType;

		// Token: 0x04003938 RID: 14648
		[SerializeField]
		private InfinityScroll scroll;

		// Token: 0x04003939 RID: 14649
		private const int OwnedCommandLength = 3;

		// Token: 0x0400393A RID: 14650
		private int _characterId;

		// Token: 0x0400393B RID: 14651
		private short _templateId;

		// Token: 0x0400393C RID: 14652
		private readonly List<int> _reqCharacterList = new List<int>();

		// Token: 0x0400393D RID: 14653
		private List<GroupCharDisplayData> _groupCharDisplayDataList = new List<GroupCharDisplayData>();

		// Token: 0x0400393E RID: 14654
		private readonly List<sbyte> _equippedCommands = new List<sbyte>();

		// Token: 0x0400393F RID: 14655
		private readonly Dictionary<int, List<sbyte>> _availableCommandsDict = new Dictionary<int, List<sbyte>>();

		// Token: 0x04003940 RID: 14656
		private List<sbyte> _availableCommandsInConfig = new List<sbyte>();

		// Token: 0x04003941 RID: 14657
		private Dictionary<int, int> _equippedCommandMedalDict = new Dictionary<int, int>();

		// Token: 0x04003942 RID: 14658
		private bool _needSave;
	}
}
