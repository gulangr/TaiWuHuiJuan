using System;
using System.Collections.Generic;
using FrameWork;
using FrameWork.UISystem.UIElements;
using GameData.Domains.CombatSkill;
using GameData.Domains.Extra;
using GameData.Utilities;
using UnityEngine;

namespace Game.Views.Combat
{
	// Token: 0x02000AF3 RID: 2803
	public class CombatSkillSortWidget : MonoBehaviour
	{
		// Token: 0x17000F31 RID: 3889
		// (get) Token: 0x060089B2 RID: 35250 RVA: 0x003FC20B File Offset: 0x003FA40B
		public bool IsOpened
		{
			get
			{
				return this.maskRoot.gameObject.activeSelf;
			}
		}

		// Token: 0x17000F32 RID: 3890
		// (get) Token: 0x060089B3 RID: 35251 RVA: 0x003FC21D File Offset: 0x003FA41D
		private CombatModel Model
		{
			get
			{
				return SingletonObject.getInstance<CombatModel>();
			}
		}

		// Token: 0x060089B4 RID: 35252 RVA: 0x003FC224 File Offset: 0x003FA424
		public void Init()
		{
			if (this._skillItemPool == null)
			{
				this._skillItemPool = new PoolItem("CombatSkillSortWidget_SkillItem", this.skillItemPrefab.gameObject);
			}
			if (this._emptyGridPool == null)
			{
				this._emptyGridPool = new PoolItem("CombatSkillSortWidget_EmptyGrid", this.emptyGridPrefab);
			}
			this.closeButton.ClearAndAddListener(new Action(this.Close));
			this.Model.OnGetProactiveSkillList += this.OnGetProactiveSkillList;
			this.maskRoot.gameObject.SetActive(false);
		}

		// Token: 0x060089B5 RID: 35253 RVA: 0x003FC2B5 File Offset: 0x003FA4B5
		private void OnDestroy()
		{
			this.Model.OnGetProactiveSkillList -= this.OnGetProactiveSkillList;
		}

		// Token: 0x060089B6 RID: 35254 RVA: 0x003FC2D0 File Offset: 0x003FA4D0
		public void Open()
		{
			bool isOpened = this.IsOpened;
			if (!isOpened)
			{
				this._selectedIndex = -1;
				this.RefreshFromModel();
				this.RefreshView();
				this.maskRoot.gameObject.SetActive(true);
				UIManager.Instance.MaskComponent(this.maskRoot);
				UIManager.Instance.SetEscHandler(new Action(this.Close));
				Action onOpen = this.OnOpen;
				if (onOpen != null)
				{
					onOpen();
				}
			}
		}

		// Token: 0x060089B7 RID: 35255 RVA: 0x003FC34C File Offset: 0x003FA54C
		public void Close()
		{
			bool flag = !this.IsOpened;
			if (!flag)
			{
				this.maskRoot.gameObject.SetActive(false);
				UIManager.Instance.UnMaskComponent(this.maskRoot);
				bool flag2 = UIManager.Instance.CheckEscHandler(new Action(this.Close));
				if (flag2)
				{
					UIManager.Instance.SetEscHandler(null);
				}
				this._selectedIndex = -1;
				Action onClose = this.OnClose;
				if (onClose != null)
				{
					onClose();
				}
			}
		}

		// Token: 0x060089B8 RID: 35256 RVA: 0x003FC3CC File Offset: 0x003FA5CC
		public void ForceClose()
		{
			UIManager.Instance.UnMaskComponent(this.maskRoot);
			bool isOpened = this.IsOpened;
			if (isOpened)
			{
				this.Close();
			}
		}

		// Token: 0x060089B9 RID: 35257 RVA: 0x003FC3FC File Offset: 0x003FA5FC
		private void OnGetProactiveSkillList(int charId)
		{
			bool flag = !this.IsOpened || charId != this.Model.SelfCharId;
			if (!flag)
			{
				this.RefreshFromModel();
				this.RefreshView();
			}
		}

		// Token: 0x060089BA RID: 35258 RVA: 0x003FC43C File Offset: 0x003FA63C
		private void OnSlotClicked(int index)
		{
			bool flag = this._selectedIndex < 0;
			if (flag)
			{
				this._selectedIndex = index;
				this.RefreshView();
			}
			else
			{
				bool flag2 = this._selectedIndex == index;
				if (flag2)
				{
					this._selectedIndex = -1;
					this.RefreshView();
				}
				else
				{
					List<short> combatSkillOrderPlan = this._combatSkillOrderPlan;
					int selectedIndex = this._selectedIndex;
					List<short> combatSkillOrderPlan2 = this._combatSkillOrderPlan;
					short value = this._combatSkillOrderPlan[index];
					short value2 = this._combatSkillOrderPlan[this._selectedIndex];
					combatSkillOrderPlan[selectedIndex] = value;
					combatSkillOrderPlan2[index] = value2;
					this._selectedIndex = -1;
					ExtraDomainMethod.Call.SetCombatSkillOrderPlan(new ShortList
					{
						Items = this._combatSkillOrderPlan
					});
					this.RefreshView();
				}
			}
		}

		// Token: 0x060089BB RID: 35259 RVA: 0x003FC508 File Offset: 0x003FA708
		private void RefreshFromModel()
		{
			this._combatSkillOrderPlan.Clear();
			List<short> orderedSkills;
			bool flag = !this.Model.OrderedProactiveSkillList.TryGetValue(this.Model.SelfCharId, out orderedSkills);
			if (flag)
			{
				orderedSkills = null;
			}
			for (int i = 0; i < 54; i++)
			{
				short skillId = (orderedSkills != null && i < orderedSkills.Count) ? orderedSkills[i] : -1;
				this._combatSkillOrderPlan.Add(skillId);
			}
		}

		// Token: 0x060089BC RID: 35260 RVA: 0x003FC584 File Offset: 0x003FA784
		private void RefreshView()
		{
			Dictionary<short, CombatSkillDisplayData> skillDataDict = EasyPool.Get<Dictionary<short, CombatSkillDisplayData>>();
			skillDataDict.Clear();
			IReadOnlyList<CombatSkillDisplayData> skillDataList;
			bool flag = this.Model.ProactiveSkillData.TryGetValue(this.Model.SelfCharId, out skillDataList);
			if (flag)
			{
				foreach (CombatSkillDisplayData skillData in skillDataList)
				{
					skillDataDict[skillData.TemplateId] = skillData;
				}
			}
			this.ClearSkillPanel(this.sortSkillRootMain);
			this.ClearSkillPanel(this.sortSkillRootExtra);
			this.CreateSkillSlots(0, 18, this.sortSkillRootMain, skillDataDict);
			this.CreateSkillSlots(18, 54, this.sortSkillRootExtra, skillDataDict);
			EasyPool.Free<Dictionary<short, CombatSkillDisplayData>>(skillDataDict);
		}

		// Token: 0x060089BD RID: 35261 RVA: 0x003FC650 File Offset: 0x003FA850
		private void ClearSkillPanel(Transform root)
		{
			for (int i = root.childCount - 1; i >= 0; i--)
			{
				Transform child = root.GetChild(i);
				CombatProactiveSkillView combatProactiveSkillView;
				bool hasSkillView = child.TryGetComponent<CombatProactiveSkillView>(out combatProactiveSkillView);
				bool flag = hasSkillView;
				if (flag)
				{
					this._skillItemPool.DestroyObject(child.gameObject);
				}
				else
				{
					this._emptyGridPool.DestroyObject(child.gameObject);
				}
			}
		}

		// Token: 0x060089BE RID: 35262 RVA: 0x003FC6BC File Offset: 0x003FA8BC
		private void CreateSkillSlots(int startIndex, int endIndex, Transform parent, Dictionary<short, CombatSkillDisplayData> skillDataDict)
		{
			for (int i = startIndex; i < endIndex; i++)
			{
				CombatSkillDisplayData skillData;
				bool flag = i < this._combatSkillOrderPlan.Count && this._combatSkillOrderPlan[i] >= 0 && skillDataDict.TryGetValue(this._combatSkillOrderPlan[i], out skillData);
				if (flag)
				{
					this.CreateSkillItem(i, parent, skillData);
				}
				else
				{
					this.CreateEmptySlot(i, parent);
				}
			}
		}

		// Token: 0x060089BF RID: 35263 RVA: 0x003FC730 File Offset: 0x003FA930
		private void CreateSkillItem(int index, Transform parent, CombatSkillDisplayData skillData)
		{
			GameObject obj = this._skillItemPool.GetObject();
			obj.transform.SetParent(parent);
			obj.transform.SetAsLastSibling();
			obj.transform.localScale = Vector3.one;
			Transform innerObj = obj.transform.GetChild(0);
			CombatProactiveSkillView skillView;
			bool flag = !innerObj.TryGetComponent<CombatProactiveSkillView>(out skillView);
			if (!flag)
			{
				int capturedIndex = index;
				skillView.SetData(skillData, delegate
				{
					this.OnSlotClicked(capturedIndex);
				}, null, delegate
				{
					this.OnSkillHover(skillView, capturedIndex, true);
				}, delegate
				{
					this.OnSkillHover(skillView, capturedIndex, false);
				});
				skillView.SetInteractable(true);
				bool flag2 = this._selectedIndex == capturedIndex;
				if (flag2)
				{
					skillView.OnMouseEnter();
				}
				else
				{
					skillView.OnMouseExit();
				}
			}
		}

		// Token: 0x060089C0 RID: 35264 RVA: 0x003FC818 File Offset: 0x003FAA18
		private void OnSkillHover(CombatProactiveSkillView skillView, int index, bool enter)
		{
			if (enter)
			{
				skillView.OnMouseEnter();
			}
			else
			{
				bool flag = this._selectedIndex == index;
				if (flag)
				{
					skillView.OnMouseEnter();
				}
				else
				{
					skillView.OnMouseExit();
				}
			}
		}

		// Token: 0x060089C1 RID: 35265 RVA: 0x003FC854 File Offset: 0x003FAA54
		private void CreateEmptySlot(int index, Transform parent)
		{
			GameObject obj = this._emptyGridPool.GetObject();
			obj.transform.SetParent(parent);
			obj.transform.SetAsLastSibling();
			obj.transform.localScale = Vector3.one;
			int capturedIndex = index;
			obj.GetComponent<CButton>().ClearAndAddListener(delegate
			{
				this.OnSlotClicked(capturedIndex);
			});
			PointerTrigger pointerTrigger;
			bool flag = obj.TryGetComponent<PointerTrigger>(out pointerTrigger);
			if (flag)
			{
				pointerTrigger.EnterEvent.RemoveAllListeners();
				pointerTrigger.EnterEvent.AddListener(delegate()
				{
					CombatSkillSortWidget.SetEmptySlotHover(obj, true);
				});
				pointerTrigger.ExitEvent.RemoveAllListeners();
				pointerTrigger.ExitEvent.AddListener(delegate()
				{
					CombatSkillSortWidget.SetEmptySlotHover(obj, this._selectedIndex == capturedIndex);
				});
			}
			CombatSkillSortWidget.SetEmptySlotHover(obj, this._selectedIndex == capturedIndex);
		}

		// Token: 0x060089C2 RID: 35266 RVA: 0x003FC952 File Offset: 0x003FAB52
		private static void SetEmptySlotHover(GameObject obj, bool active)
		{
			obj.transform.Find("Hover").gameObject.SetActive(active);
		}

		// Token: 0x0400699C RID: 27036
		private const int MainCombatSkillCount = 18;

		// Token: 0x0400699D RID: 27037
		private const int MaxCombatSkillCount = 54;

		// Token: 0x0400699E RID: 27038
		[SerializeField]
		private RectTransform maskRoot;

		// Token: 0x0400699F RID: 27039
		[SerializeField]
		private CButton closeButton;

		// Token: 0x040069A0 RID: 27040
		[SerializeField]
		private RectTransform sortSkillRootMain;

		// Token: 0x040069A1 RID: 27041
		[SerializeField]
		private RectTransform sortSkillRootExtra;

		// Token: 0x040069A2 RID: 27042
		[SerializeField]
		private GameObject skillItemPrefab;

		// Token: 0x040069A3 RID: 27043
		[SerializeField]
		private GameObject emptyGridPrefab;

		// Token: 0x040069A4 RID: 27044
		public Action OnOpen;

		// Token: 0x040069A5 RID: 27045
		public Action OnClose;

		// Token: 0x040069A6 RID: 27046
		private PoolItem _skillItemPool;

		// Token: 0x040069A7 RID: 27047
		private PoolItem _emptyGridPool;

		// Token: 0x040069A8 RID: 27048
		private readonly List<short> _combatSkillOrderPlan = new List<short>(54);

		// Token: 0x040069A9 RID: 27049
		private int _selectedIndex = -1;
	}
}
