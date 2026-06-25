using System;
using System.Collections;
using System.Collections.Generic;
using Config;
using FrameWork;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using Game.Components.Common;
using GameData.Common;
using GameData.Domains.Global;
using GameData.GameDataBridge;
using GameData.Serializer;
using TMPro;
using UnityEngine;

namespace Game.Views.Achievement
{
	// Token: 0x02000C82 RID: 3202
	public class ViewAchievement : UIBase
	{
		// Token: 0x0600A377 RID: 41847 RVA: 0x004C7F6C File Offset: 0x004C616C
		public override void OnInit(ArgumentBox argsBox)
		{
			bool flag = argsBox == null || argsBox.Get("TemplateId", out this._preloadTemplateId);
			if (flag)
			{
				this._preloadTemplateId = -1;
			}
			this.NeedDataListenerId = true;
			UIElement element = this.Element;
			element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(delegate()
			{
				GlobalDomainMethod.Call.GetAchievementDisplayData(this.Element.GameDataListenerId);
			}));
		}

		// Token: 0x0600A378 RID: 41848 RVA: 0x004C7FCA File Offset: 0x004C61CA
		public override void InitMonitorFieldIds()
		{
			this.MonitorFields.Add(new UIBase.MonitorDataField(0, 9, ulong.MaxValue, null));
			this.MonitorFields.Add(new UIBase.MonitorDataField(1, 54, ulong.MaxValue, null));
		}

		// Token: 0x0600A379 RID: 41849 RVA: 0x004C7FFC File Offset: 0x004C61FC
		private void Awake()
		{
			this.toggleGroup.Init(new Action(this.OnToggleGroupChange));
			ToggleGroupHotkeyController.Set(this.Element, this.toggleGroup, 0, null);
			this.btnPrev.ClearAndAddListener(new Action(this.OnClickBtnPrev));
			this.btnNext.ClearAndAddListener(new Action(this.OnClickBtnNext));
			this.btnClose.ClearAndAddListener(new Action(this.QuickHide));
			this.scroll.InitPageCount();
			this.scroll.OnItemRender += this.OnRenderAchievement;
			foreach (AchievementInfoItem config in ((IEnumerable<AchievementInfoItem>)AchievementInfo.Instance))
			{
				List<int> totalCount = this._totalCount;
				int num = totalCount[0];
				totalCount[0] = num + 1;
				List<int> totalCount2 = this._totalCount;
				num = (int)config.Level;
				int num2 = totalCount2[num];
				totalCount2[num] = num2 + 1;
			}
		}

		// Token: 0x0600A37A RID: 41850 RVA: 0x004C8118 File Offset: 0x004C6318
		private void OnEnable()
		{
			this.toggleGroup.RefreshName();
		}

		// Token: 0x0600A37B RID: 41851 RVA: 0x004C8127 File Offset: 0x004C6327
		private void OnDisable()
		{
			GlobalDomainMethod.Call.SetLastTimeOpenAchievements();
		}

		// Token: 0x0600A37C RID: 41852 RVA: 0x004C8130 File Offset: 0x004C6330
		public override void OnNotifyGameData(List<NotificationWrapper> notifications)
		{
			bool statsChanged = false;
			bool achievementRefreshed = false;
			foreach (NotificationWrapper wrapper in notifications)
			{
				Notification notification = wrapper.Notification;
				byte type = notification.Type;
				byte b = type;
				if (b != 0)
				{
					if (b == 1)
					{
						if (notification.DomainId == 0 && notification.MethodId == 23)
						{
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._data);
							AchievementDisplayData data = this._data;
							if (data.Achievements == null)
							{
								data.Achievements = new Dictionary<short, long>();
							}
							this.Refresh();
							achievementRefreshed = true;
							this.Element.ShowAfterRefresh();
						}
					}
				}
				else
				{
					DataUid uid = notification.Uid;
					if (uid.DomainId != 0 || uid.DataId != 9)
					{
						uid = notification.Uid;
						if (uid.DomainId == 1 && uid.DataId == 54)
						{
							this._statProgress.ApplyLocalModification(wrapper.DataPool, notification.ValueOffset);
							statsChanged = true;
						}
					}
					else
					{
						this._statProgress.ApplyGlobalModification(wrapper.DataPool, notification.ValueOffset);
						statsChanged = true;
					}
				}
			}
			bool flag = this._data == null;
			if (!flag)
			{
				bool flag2 = statsChanged || achievementRefreshed;
				if (flag2)
				{
					this.scroll.ReRender();
				}
				bool flag3 = achievementRefreshed && !statsChanged;
				if (flag3)
				{
					this.scroll.StartCoroutine(this.ReRenderProgressNextFrame());
				}
			}
		}

		// Token: 0x0600A37D RID: 41853 RVA: 0x004C82E8 File Offset: 0x004C64E8
		private IEnumerator ReRenderProgressNextFrame()
		{
			yield return null;
			bool flag = this._data != null;
			if (flag)
			{
				this.scroll.ReRender();
			}
			yield break;
		}

		// Token: 0x0600A37E RID: 41854 RVA: 0x004C82F8 File Offset: 0x004C64F8
		private void Refresh()
		{
			int count = 10;
			for (int i = 0; i < count; i++)
			{
				this.toggleGroup.SetIsNew((EAchievementInfoType)i, false);
			}
			foreach (KeyValuePair<short, long> keyValuePair in this._data.Achievements)
			{
				short num;
				long num2;
				keyValuePair.Deconstruct(out num, out num2);
				short id = num;
				long timeStamp = num2;
				bool flag = this.IsNew(timeStamp);
				if (flag)
				{
					this.toggleGroup.SetIsNew(AchievementInfo.Instance[id].Type, true);
				}
			}
			bool flag2 = this.toggleGroup.GetActiveIndex() < 0;
			if (flag2)
			{
				this.toggleGroup.Set(0, false);
			}
			else
			{
				this.toggleGroup.Get(this.toggleGroup.GetActiveIndex()).SetIsOnWithoutNotify(true);
			}
			for (int j = 0; j < this._achievedCount.Count; j++)
			{
				this._achievedCount[j] = 0;
			}
			foreach (AchievementInfoItem config in ((IEnumerable<AchievementInfoItem>)AchievementInfo.Instance))
			{
				bool flag3 = this._data.Achievements.ContainsKey(config.TemplateId);
				if (flag3)
				{
					List<int> achievedCount = this._achievedCount;
					int num3 = achievedCount[0];
					achievedCount[0] = num3 + 1;
					List<int> achievedCount2 = this._achievedCount;
					num3 = (int)config.Level;
					int num4 = achievedCount2[num3];
					achievedCount2[num3] = num4 + 1;
				}
			}
			for (int k = 0; k < this._achievedCount.Count; k++)
			{
				string color = (this._achievedCount[k] == this._totalCount[k]) ? "brightblue" : "8f755f";
				bool flag4 = k == 0;
				if (flag4)
				{
					this.counts.GetChild(k).GetComponent<TextMeshProUGUI>().text = LanguageKey.LK_Achievement_Count_All.TrFormat(string.Format("{0}/{1}", this._achievedCount[k].ToString().SetColor(color), this._totalCount[k]));
				}
				else
				{
					this.counts.GetChild(k).GetComponent<TextMeshProUGUI>().text = LanguageKey.LK_Achievement_Count_Level.TrFormat(LocalStringManager.Get(string.Format("LK_Num_{0}", k)), string.Format("{0}/{1}", this._achievedCount[k].ToString().SetColor(color), this._totalCount[k]));
				}
			}
			bool flag5 = this._preloadTemplateId >= 0;
			if (flag5)
			{
				this.toggleGroup.SetWithoutNotify((int)AchievementInfo.Instance[this._preloadTemplateId].Type);
			}
			this.OnToggleGroupChange();
			this._preloadTemplateId = -1;
		}

		// Token: 0x0600A37F RID: 41855 RVA: 0x004C862C File Offset: 0x004C682C
		private void OnToggleGroupChange()
		{
			this._achievements.Clear();
			int index = 0;
			EAchievementInfoType curr = (EAchievementInfoType)this.toggleGroup.GetActiveIndex();
			this.toggleGroup.SetIsNew(curr, false);
			foreach (AchievementInfoItem config in ((IEnumerable<AchievementInfoItem>)AchievementInfo.Instance))
			{
				bool flag = config.Type == curr;
				if (flag)
				{
					this._achievements.Add(config.TemplateId);
				}
			}
			this._achievements.Sort(new Comparison<short>(this.CompareAchievement));
			for (int i = 0; i < this._achievements.Count; i++)
			{
				bool flag2 = this._achievements[i] == this._preloadTemplateId;
				if (flag2)
				{
					index = i;
				}
			}
			this.scroll.SetDataCount(this._achievements.Count);
			this.scroll.ScrollTo(index, 0.3f);
		}

		// Token: 0x0600A380 RID: 41856 RVA: 0x004C873C File Offset: 0x004C693C
		private void OnClickBtnPrev()
		{
			int curr = this.toggleGroup.GetActiveIndex();
			bool flag = curr == 0;
			if (!flag)
			{
				this.toggleGroup.Set(curr - 1, false);
			}
		}

		// Token: 0x0600A381 RID: 41857 RVA: 0x004C8770 File Offset: 0x004C6970
		private void OnClickBtnNext()
		{
			int curr = this.toggleGroup.GetActiveIndex();
			bool flag = curr == 9;
			if (!flag)
			{
				this.toggleGroup.Set(curr + 1, false);
			}
		}

		// Token: 0x0600A382 RID: 41858 RVA: 0x004C87A8 File Offset: 0x004C69A8
		private void OnRenderAchievement(int index, GameObject obj)
		{
			short id = this._achievements[index];
			long timeStamp = this._data.Achievements.GetValueOrDefault(id, -1L);
			int progressCurrent = 0;
			int progressTarget = 0;
			bool showProgress = this.TryGetProgress(id, out progressCurrent, out progressTarget);
			obj.GetComponent<AchievementLongItem>().Set(id, timeStamp, this.IsNew(timeStamp), progressCurrent, progressTarget, showProgress);
		}

		// Token: 0x0600A383 RID: 41859 RVA: 0x004C8804 File Offset: 0x004C6A04
		private bool TryGetProgress(short achievementId, out int current, out int target)
		{
			current = 0;
			target = 0;
			short statId;
			bool flag = !AchievementProgressResolver.TryResolve(achievementId, out statId, out target);
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = this._statProgress.TryGetValue(statId, out current);
				if (flag2)
				{
					current = Math.Clamp(current, 0, target);
				}
				result = true;
			}
			return result;
		}

		// Token: 0x0600A384 RID: 41860 RVA: 0x004C8850 File Offset: 0x004C6A50
		private int CompareAchievement(short a, short b)
		{
			long aTimeStamp;
			bool aAchieved = this._data.Achievements.TryGetValue(a, out aTimeStamp);
			long bTimeStamp;
			bool bAchieved = this._data.Achievements.TryGetValue(b, out bTimeStamp);
			bool flag = aAchieved && bAchieved;
			int result;
			if (flag)
			{
				result = -aTimeStamp.CompareTo(bTimeStamp);
			}
			else
			{
				bool flag2 = aAchieved;
				if (flag2)
				{
					result = -1;
				}
				else
				{
					bool flag3 = bAchieved;
					if (flag3)
					{
						result = 1;
					}
					else
					{
						result = a.CompareTo(b);
					}
				}
			}
			return result;
		}

		// Token: 0x0600A385 RID: 41861 RVA: 0x004C88C2 File Offset: 0x004C6AC2
		private bool IsNew(long timeStampAchievement)
		{
			return timeStampAchievement >= this._data.LastTimeOpen;
		}

		// Token: 0x04007F2F RID: 32559
		[SerializeField]
		private Transform counts;

		// Token: 0x04007F30 RID: 32560
		[SerializeField]
		private AchievementTypeToggleGroup toggleGroup;

		// Token: 0x04007F31 RID: 32561
		[SerializeField]
		private CButton btnPrev;

		// Token: 0x04007F32 RID: 32562
		[SerializeField]
		private CButton btnNext;

		// Token: 0x04007F33 RID: 32563
		[SerializeField]
		private CButton btnClose;

		// Token: 0x04007F34 RID: 32564
		[SerializeField]
		private InfinityScroll scroll;

		// Token: 0x04007F35 RID: 32565
		private const string Incomplete = "8f755f";

		// Token: 0x04007F36 RID: 32566
		private List<short> _achievements = new List<short>();

		// Token: 0x04007F37 RID: 32567
		private AchievementDisplayData _data;

		// Token: 0x04007F38 RID: 32568
		private List<int> _totalCount = new List<int>
		{
			0,
			0,
			0,
			0,
			0
		};

		// Token: 0x04007F39 RID: 32569
		private List<int> _achievedCount = new List<int>
		{
			0,
			0,
			0,
			0,
			0
		};

		// Token: 0x04007F3A RID: 32570
		private short _preloadTemplateId = -1;

		// Token: 0x04007F3B RID: 32571
		private readonly GameStatProgressReader _statProgress = new GameStatProgressReader();
	}
}
