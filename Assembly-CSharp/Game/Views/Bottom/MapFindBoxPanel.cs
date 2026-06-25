using System;
using System.Collections.Generic;
using FrameWork;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Map;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.Views.Bottom
{
	// Token: 0x02000C3D RID: 3133
	public class MapFindBoxPanel : MonoBehaviour, IAsyncMethodRequestHandler
	{
		// Token: 0x06009F1D RID: 40733 RVA: 0x004A5F0C File Offset: 0x004A410C
		private void Awake()
		{
			this._currentPresetIndex = 0;
			this.toggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnToggleChanged));
			GEvent.Add(UiEvents.MapBlockChange, new GEvent.Callback(this.OnMapBlockChange));
		}

		// Token: 0x06009F1E RID: 40734 RVA: 0x004A5F5A File Offset: 0x004A415A
		private void OnDestroy()
		{
			GEvent.Remove(UiEvents.MapBlockChange, new GEvent.Callback(this.OnMapBlockChange));
		}

		// Token: 0x06009F1F RID: 40735 RVA: 0x004A5F79 File Offset: 0x004A4179
		private void OnEnable()
		{
			this.OnFilterDataChanged(false);
		}

		// Token: 0x06009F20 RID: 40736 RVA: 0x004A5F84 File Offset: 0x004A4184
		private void OnMapBlockChange(ArgumentBox argBox)
		{
			argBox.Get("PresetIndex", out this._currentPresetIndex);
			argBox.Get<MapBlockFindData>("FilterData", out this._currentFilterData);
			argBox.Get("ToggleState", out this._currentToggleState);
			this.OnFilterDataChanged(false);
			this.toggle.SetIsOnWithoutNotify(this._currentToggleState);
		}

		// Token: 0x06009F21 RID: 40737 RVA: 0x004A5FE4 File Offset: 0x004A41E4
		private void OnFilterDataChanged(bool isAddToMark = false)
		{
			MapFindBoxPanel.<>c__DisplayClass9_0 CS$<>8__locals1 = new MapFindBoxPanel.<>c__DisplayClass9_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.isAddToMark = isAddToMark;
			bool flag = this._currentFilterData != null;
			if (flag)
			{
				CS$<>8__locals1.<OnFilterDataChanged>g__SetPreset|1(CS$<>8__locals1.isAddToMark);
			}
			else
			{
				MapDomainMethod.AsyncCall.GetMapBlockFindDataPreset(this, this._currentPresetIndex, delegate(int offset, RawDataPool pool)
				{
					bool flag2 = !CS$<>8__locals1.<>4__this;
					if (!flag2)
					{
						Serializer.Deserialize(pool, offset, ref CS$<>8__locals1.<>4__this._currentFilterData);
						if (CS$<>8__locals1.<>4__this._currentFilterData == null)
						{
							CS$<>8__locals1.<>4__this._currentFilterData = new MapBlockFindData();
						}
						base.<OnFilterDataChanged>g__SetPreset|1(CS$<>8__locals1.isAddToMark);
					}
				});
			}
		}

		// Token: 0x06009F22 RID: 40738 RVA: 0x004A6040 File Offset: 0x004A4240
		private void RefreshResLabel(List<Location> locations)
		{
			bool flag = locations != null && locations.Count > 0;
			if (flag)
			{
				this.resLabel.text = LanguageKey.LK_FindMapBlock_Res_Find_1.TrFormat(locations.Count);
			}
			else
			{
				this.resLabel.text = LanguageKey.LK_FindMapBlock_Res_Null_1.Tr();
			}
			LayoutRebuilder.ForceRebuildLayoutImmediate(this.resLabel.GetComponent<RectTransform>());
			LayoutRebuilder.ForceRebuildLayoutImmediate(base.GetComponent<RectTransform>());
		}

		// Token: 0x06009F23 RID: 40739 RVA: 0x004A60BC File Offset: 0x004A42BC
		private void OnToggleChanged(bool isOn)
		{
			bool flag = !isOn;
			if (flag)
			{
				SingletonObject.getInstance<WorldMapModel>().ClearAllTemporaryMarkList();
			}
			else
			{
				this.OnFilterDataChanged(true);
			}
		}

		// Token: 0x06009F24 RID: 40740 RVA: 0x004A60EB File Offset: 0x004A42EB
		public void RegisterAsyncMethodCall(int requestId)
		{
		}

		// Token: 0x06009F25 RID: 40741 RVA: 0x004A60EE File Offset: 0x004A42EE
		public void ClearAsyncMethodCalls()
		{
		}

		// Token: 0x06009F26 RID: 40742 RVA: 0x004A60F1 File Offset: 0x004A42F1
		public void ChangeMapFindToggleValue()
		{
			this.toggle.isOn = !this.toggle.isOn;
		}

		// Token: 0x04007B0F RID: 31503
		[SerializeField]
		private TextMeshProUGUI resLabel;

		// Token: 0x04007B10 RID: 31504
		[SerializeField]
		private CToggle toggle;

		// Token: 0x04007B11 RID: 31505
		private int _currentPresetIndex;

		// Token: 0x04007B12 RID: 31506
		private MapBlockFindData _currentFilterData;

		// Token: 0x04007B13 RID: 31507
		private bool _currentToggleState;
	}
}
