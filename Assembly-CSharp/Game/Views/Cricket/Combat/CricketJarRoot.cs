using System;
using System.Collections.Generic;
using FrameWork;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Taiwu;
using GameData.Utilities;
using UnityEngine;

namespace Game.Views.Cricket.Combat
{
	// Token: 0x02000ADD RID: 2781
	public class CricketJarRoot : MonoBehaviour, ICricketCombatComponent
	{
		// Token: 0x17000F1A RID: 3866
		// (get) Token: 0x060088D6 RID: 35030 RVA: 0x003F6075 File Offset: 0x003F4275
		public bool Ally
		{
			get
			{
				return this.ally;
			}
		}

		// Token: 0x17000F1B RID: 3867
		// (get) Token: 0x060088D7 RID: 35031 RVA: 0x003F607D File Offset: 0x003F427D
		// (set) Token: 0x060088D8 RID: 35032 RVA: 0x003F6085 File Offset: 0x003F4285
		public ICricketCombatHandler Handler { get; set; }

		// Token: 0x060088D9 RID: 35033 RVA: 0x003F6090 File Offset: 0x003F4290
		public void OnEvent(ECricketCombatGlobalEventType type, ArgumentBox argBox)
		{
			bool flag = type == ECricketCombatGlobalEventType.Initialize;
			if (flag)
			{
				this.Initialize();
			}
			bool flag2 = type == ECricketCombatGlobalEventType.CricketDataReady || (type == ECricketCombatGlobalEventType.SelfCricketChanged && this.ally);
			if (flag2)
			{
				this.UpdateCrickets();
			}
			bool flag3 = type == ECricketCombatGlobalEventType.CombatStatusChanged;
			if (flag3)
			{
				this.OnStatusChanged();
			}
			bool flag4 = type == ECricketCombatGlobalEventType.AllowSelectCricketChanged && this.ally;
			if (flag4)
			{
				this.UpdateAllowSelectCricket();
			}
			bool flag5 = type == ECricketCombatGlobalEventType.MatchPrepare;
			if (flag5)
			{
				this.PutJarToBoard();
			}
		}

		// Token: 0x060088DA RID: 35034 RVA: 0x003F6104 File Offset: 0x003F4304
		private void OnEnable()
		{
			GEvent.Add(EEvents.OnConfirmQuitGameState, new GEvent.Callback(this.OnConfirmQuitGameState));
		}

		// Token: 0x060088DB RID: 35035 RVA: 0x003F6120 File Offset: 0x003F4320
		private void OnDisable()
		{
			GEvent.Remove(EEvents.OnConfirmQuitGameState, new GEvent.Callback(this.OnConfirmQuitGameState));
		}

		// Token: 0x060088DC RID: 35036 RVA: 0x003F613C File Offset: 0x003F433C
		private void Initialize()
		{
			IReadOnlyList<CricketJar> cricketJars = this._cricketJars;
			bool flag = cricketJars == null || cricketJars.Count <= 0;
			if (flag)
			{
				this._cricketJars = base.GetComponentsInChildren<CricketJar>(true);
			}
			this.SetupDragItems();
			for (int i = 0; i < this._cricketJars.Count; i++)
			{
				this._cricketJars[i].Clear(i);
				this._cricketJars[i].InitData(this, i);
			}
			bool flag2 = this.ally;
			if (flag2)
			{
				this.UpdateAllowSelectCricket();
			}
		}

		// Token: 0x060088DD RID: 35037 RVA: 0x003F61D4 File Offset: 0x003F43D4
		private void SetupDragItems()
		{
			bool flag = !this.ally;
			if (!flag)
			{
				bool flag2 = this._dragItems.Count == this._cricketJars.Count;
				if (flag2)
				{
					for (int i = 0; i < this._dragItems.Count; i++)
					{
						this._dragItems[i].Initialize(this, i);
					}
				}
				else
				{
					this._dragItems.Clear();
					for (int j = 0; j < this._cricketJars.Count; j++)
					{
						CricketJarLongPressDragItem dragItem = this._cricketJars[j].gameObject.GetOrAddComponent<CricketJarLongPressDragItem>();
						dragItem.Initialize(this, j);
						this._dragItems.Add(dragItem);
					}
				}
			}
		}

		// Token: 0x060088DE RID: 35038 RVA: 0x003F62A4 File Offset: 0x003F44A4
		private void UpdateCrickets()
		{
			IReadOnlyList<ItemDisplayData> readOnlyList;
			if (!this.ally)
			{
				readOnlyList = CricketCombatKit.Board.EnemyCrickets;
			}
			else
			{
				IReadOnlyList<ItemDisplayData> selfCrickets = CricketCombatKit.Board.SelfCrickets;
				readOnlyList = selfCrickets;
			}
			IReadOnlyList<ItemDisplayData> crickets = readOnlyList;
			for (int i = 0; i < this._cricketJars.Count; i++)
			{
				CricketJar jar = this._cricketJars[i];
				ItemDisplayData cricketData = crickets.GetOrDefault(i);
				bool flag = cricketData == null;
				if (flag)
				{
					jar.Clear(i);
				}
				else
				{
					jar.Set(cricketData, i);
					jar.SetVisible(this.ShouldShowCricket(i));
					jar.Cricket.SetSoundData(false, false, this.ally);
				}
			}
		}

		// Token: 0x060088DF RID: 35039 RVA: 0x003F634C File Offset: 0x003F454C
		private bool ShouldShowCricket(int cricketIndex)
		{
			bool flag = this.ally;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				bool flag2 = CricketCombatKit.Board.Status > ECricketCombatStatus.Preparing;
				if (flag2)
				{
					result = true;
				}
				else
				{
					bool isEnabled = CricketFairCombatHelper.IsEnabled;
					result = (!isEnabled && CricketCombatKit.Board.IsShowCricket(cricketIndex));
				}
			}
			return result;
		}

		// Token: 0x060088E0 RID: 35040 RVA: 0x003F639C File Offset: 0x003F459C
		private void OnStatusChanged()
		{
			this.UpdateCricketSingVolume();
			ECricketCombatStatus nowStatus = CricketCombatKit.Board.Status;
			bool flag = nowStatus != ECricketCombatStatus.Combating || this.ally;
			if (flag)
			{
				this.SetCancelButtonActive(false);
			}
			else
			{
				foreach (CricketJar jar in this._cricketJars)
				{
					jar.SetVisible(true);
				}
			}
		}

		// Token: 0x060088E1 RID: 35041 RVA: 0x003F641C File Offset: 0x003F461C
		private void UpdateAllowSelectCricket()
		{
			for (int i = 0; i < this._cricketJars.Count; i++)
			{
				this.RefreshJarInteractableState(i);
			}
		}

		// Token: 0x060088E2 RID: 35042 RVA: 0x003F644C File Offset: 0x003F464C
		private void PutJarToBoard()
		{
			CricketJar jar = this._cricketJars[CricketCombatKit.Board.CurrentMatch];
			jar.ShowName();
			bool flag = this.ally;
			if (flag)
			{
				CricketCombatKit.Board.SelfCricketJar = jar;
			}
			else
			{
				CricketCombatKit.Board.EnemyCricketJar = jar;
			}
		}

		// Token: 0x060088E3 RID: 35043 RVA: 0x003F649C File Offset: 0x003F469C
		private void UpdateCricketSingVolume()
		{
			bool combating = CricketCombatKit.Board.Status == ECricketCombatStatus.Combating;
			for (int i = 0; i < this._cricketJars.Count; i++)
			{
				CricketJar jar = this._cricketJars[i];
				bool inCombat = combating && i == CricketCombatKit.Board.CurrentMatch;
				jar.Cricket.SetSoundData(combating, inCombat, this.ally);
			}
		}

		// Token: 0x060088E4 RID: 35044 RVA: 0x003F650C File Offset: 0x003F470C
		private void OnConfirmQuitGameState(ArgumentBox argBox)
		{
			bool show;
			argBox.Get("ShowState", out show);
			foreach (CricketJar jar in this._cricketJars)
			{
				bool flag = show;
				if (flag)
				{
					jar.Cricket.PauseSing();
				}
				else
				{
					jar.Cricket.ResumeSing();
				}
			}
		}

		// Token: 0x060088E5 RID: 35045 RVA: 0x003F6584 File Offset: 0x003F4784
		internal bool CanStartJarDrag(int jarIndex)
		{
			bool flag = !this.ally;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = CricketCombatKit.Board.Status != ECricketCombatStatus.Preparing || !CricketCombatKit.Board.AllowSelectCricket;
				result = (!flag2 && CricketCombatKit.Board.SelfCrickets.GetOrDefault(jarIndex) != null);
			}
			return result;
		}

		// Token: 0x060088E6 RID: 35046 RVA: 0x003F65E0 File Offset: 0x003F47E0
		internal int GetDropJarIndex(Vector2 screenPosition, Camera eventCamera)
		{
			bool flag = !this.ally;
			int result;
			if (flag)
			{
				result = -1;
			}
			else
			{
				for (int i = 0; i < this._cricketJars.Count; i++)
				{
					RectTransform jarRect = (RectTransform)this._cricketJars[i].transform;
					bool flag2 = RectTransformUtility.RectangleContainsScreenPoint(jarRect, screenPosition, eventCamera);
					if (flag2)
					{
						return i;
					}
				}
				result = -1;
			}
			return result;
		}

		// Token: 0x060088E7 RID: 35047 RVA: 0x003F664C File Offset: 0x003F484C
		internal void HandleJarDrop(int fromJarIndex, int toJarIndex)
		{
			bool flag = !this.ally || toJarIndex < 0;
			if (!flag)
			{
				bool flag2 = !this.Handler.CanReorderSelfCricket(fromJarIndex, toJarIndex);
				if (!flag2)
				{
					this.Handler.ReorderSelfCricket(fromJarIndex, toJarIndex);
				}
			}
		}

		// Token: 0x060088E8 RID: 35048 RVA: 0x003F6693 File Offset: 0x003F4893
		internal void RefreshJarInteractableState(int jarIndex)
		{
			this._cricketJars[jarIndex].EnableSelect(CricketCombatKit.Board.AllowSelectCricket);
		}

		// Token: 0x060088E9 RID: 35049 RVA: 0x003F66B2 File Offset: 0x003F48B2
		public void SetSelfCrickets(int cricketIndex)
		{
			TaiwuDomainMethod.Call.SetCricketPlan(CricketCombatKit.Board.CurrentCricketPlanIndex, ItemKey.Invalid, cricketIndex);
			CricketCombatKit.Board.SelfCrickets.SetOrAdd(cricketIndex, null, null);
			this.Handler.OnEvent(ECricketCombatGlobalEventType.RequestData, null);
		}

		// Token: 0x060088EA RID: 35050 RVA: 0x003F66EC File Offset: 0x003F48EC
		public void SetCancelButtonActive(bool active)
		{
			foreach (CricketJar jar in this._cricketJars)
			{
				jar.SetCancelButtonActive(false);
			}
		}

		// Token: 0x040068D0 RID: 26832
		[SerializeField]
		private bool ally;

		// Token: 0x040068D1 RID: 26833
		private IReadOnlyList<CricketJar> _cricketJars;

		// Token: 0x040068D2 RID: 26834
		private readonly List<CricketJarLongPressDragItem> _dragItems = new List<CricketJarLongPressDragItem>();
	}
}
