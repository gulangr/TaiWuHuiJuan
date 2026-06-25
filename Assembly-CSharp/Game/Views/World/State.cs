using System;
using System.Collections.Generic;
using Config;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Map;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.Views.World
{
	// Token: 0x0200072B RID: 1835
	public class State : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
	{
		// Token: 0x17000A93 RID: 2707
		// (get) Token: 0x060057BA RID: 22458 RVA: 0x0028BB37 File Offset: 0x00289D37
		public MapStateItem Config
		{
			get
			{
				return MapState.Instance[this.templateId];
			}
		}

		// Token: 0x17000A94 RID: 2708
		public Area this[int id]
		{
			get
			{
				return this.areas[id];
			}
		}

		// Token: 0x060057BC RID: 22460 RVA: 0x0028BB54 File Offset: 0x00289D54
		private void Awake()
		{
			bool flag = this.dropdown;
			if (flag)
			{
				this.dropdown.onValueChanged.AddListener(new UnityAction<bool>(this.areaHolder.gameObject.SetActive));
				this.areaHolder.gameObject.SetActive(this.dropdown.isOn);
			}
			else
			{
				this.ReadLocationFromConfig();
			}
			bool flag2 = this.stateName;
			if (flag2)
			{
				this.stateName.enabled = false;
			}
		}

		// Token: 0x060057BD RID: 22461 RVA: 0x0028BBDC File Offset: 0x00289DDC
		public void Init(bool canSelectBrokenArea, bool isTravel, AreaDisplayData[] data, WorldMapModel mapModel)
		{
			bool flag = this.dropdownLabel;
			if (flag)
			{
				this.dropdownLabel.text = this.Config.Name;
			}
			bool flag2 = this.stateName;
			if (flag2)
			{
				string currLang = SingletonObject.getInstance<GlobalSettings>().Language.ToLower();
				this.stateName.SetSprite(string.Format("ui9_text_largemap_region_name_{0}_{1}_0", currLang, (int)(this.templateId - 1)), false, null);
				this._stateNameHover = this.stateName.sprite;
				this.stateName.SetSprite(string.Format("ui9_text_largemap_region_name_{0}_{1}", currLang, (int)(this.templateId - 1)), false, null);
				this._stateNameNormal = this.stateName.sprite;
				this.stateName.enabled = true;
				this.stateName.SetNativeSize();
			}
			foreach (Area area in this.areas)
			{
				area.map = this.parent;
				Selectable toggle = area.toggle;
				bool interactable;
				if (!canSelectBrokenArea && !isTravel)
				{
					short areaIdByAreaTemplateId = mapModel.GetAreaIdByAreaTemplateId(area.Config.TemplateId);
					interactable = (areaIdByAreaTemplateId >= 0 && areaIdByAreaTemplateId < 45);
				}
				else
				{
					interactable = true;
				}
				toggle.interactable = interactable;
				AreaDisplayData displayData = data[(int)mapModel.GetAreaIdByAreaTemplateId(area.Config.TemplateId)];
				area.Set(displayData, isTravel);
				area.Set(area.Config, mapModel.GetTaiwuVillageAreaId() == mapModel.GetAreaIdByAreaTemplateId(area.Config.TemplateId));
				AreaMap areaMap = this.parent;
				if (areaMap != null)
				{
					Action<Area, AreaDisplayData> postRender = areaMap.PostRender;
					if (postRender != null)
					{
						postRender(area, displayData);
					}
				}
			}
		}

		// Token: 0x060057BE RID: 22462 RVA: 0x0028BD9C File Offset: 0x00289F9C
		public void Sort()
		{
			Array.Sort<Area>(this.areas, 2, this.areas.Length - 2, default(State.Comparer));
			short num = this.areas[0].Config.TemplateId;
			bool flag = num >= 1 && num <= 15;
			if (flag)
			{
				Area[] array = this.areas;
				Area[] array2 = this.areas;
				Area area2 = this.areas[1];
				Area area3 = this.areas[0];
				array[0] = area2;
				array2[1] = area3;
			}
			foreach (Area area in this.areas)
			{
				area.transform.SetAsLastSibling();
			}
		}

		// Token: 0x060057BF RID: 22463 RVA: 0x0028BE50 File Offset: 0x0028A050
		public void ShowIcon(Func<short, bool> showIcon)
		{
			foreach (Area area in this.areas)
			{
				bool show = showIcon(area.Config.TemplateId);
				(area.icon ? area.icon.gameObject : area.gameObject).SetActive(show);
			}
		}

		// Token: 0x060057C0 RID: 22464 RVA: 0x0028BEB4 File Offset: 0x0028A0B4
		public void ShowAreaState(bool show)
		{
			foreach (Area area in this.areas)
			{
				area.SetShowAreaState(show);
			}
		}

		// Token: 0x060057C1 RID: 22465 RVA: 0x0028BEE4 File Offset: 0x0028A0E4
		public bool RefreshTaiwuLocation(int currentArea, PositionFollower taiwuLocation)
		{
			bool flag = !this.parent;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				foreach (Area area in this.areas)
				{
					bool flag2 = (int)area.Config.TemplateId == currentArea;
					if (flag2)
					{
						bool flag3 = taiwuLocation;
						if (flag3)
						{
							taiwuLocation.gameObject.SetActive(true);
							taiwuLocation.Target = area.transform;
						}
						this.parent.LookAt(area.transform.localPosition + base.transform.localPosition, 0f, -1f, default(Vector2));
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x060057C2 RID: 22466 RVA: 0x0028BFB4 File Offset: 0x0028A1B4
		public void OnPointerEnter(PointerEventData _)
		{
			StateMask stateMask = this.stateMask;
			if (stateMask != null)
			{
				stateMask.OnPointerEnter(true);
			}
		}

		// Token: 0x060057C3 RID: 22467 RVA: 0x0028BFCA File Offset: 0x0028A1CA
		public void OnPointerExit(PointerEventData _)
		{
			StateMask stateMask = this.stateMask;
			if (stateMask != null)
			{
				stateMask.OnPointerExit(true);
			}
		}

		// Token: 0x060057C4 RID: 22468 RVA: 0x0028BFE0 File Offset: 0x0028A1E0
		public void OnMaskHover(bool isStart)
		{
			bool flag = this.stateName;
			if (flag)
			{
				this.stateName.sprite = (isStart ? this._stateNameHover : this._stateNameNormal);
			}
		}

		// Token: 0x060057C5 RID: 22469 RVA: 0x0028C01C File Offset: 0x0028A21C
		public void OnDragBegin()
		{
			bool flag = this.stateMask;
			if (flag)
			{
				this.stateMask.Drag = true;
			}
		}

		// Token: 0x060057C6 RID: 22470 RVA: 0x0028C048 File Offset: 0x0028A248
		public void OnDragEnd()
		{
			bool flag = this.stateMask;
			if (flag)
			{
				this.stateMask.Drag = false;
			}
		}

		// Token: 0x060057C7 RID: 22471 RVA: 0x0028C074 File Offset: 0x0028A274
		private void ReadLocationFromConfig()
		{
			bool flag = !this.stateMask;
			if (!flag)
			{
				this.stateMask.ReadLocationFromConfig(this.Config);
				this.self.localPosition = this.stateMask.transform.localPosition;
			}
		}

		// Token: 0x04003C2E RID: 15406
		[SerializeField]
		private RectTransform self;

		// Token: 0x04003C2F RID: 15407
		[SerializeField]
		private RectTransform areaHolder;

		// Token: 0x04003C30 RID: 15408
		[SerializeField]
		private CRawImage hoverDelim;

		// Token: 0x04003C31 RID: 15409
		[SerializeField]
		internal AreaMap parent;

		// Token: 0x04003C32 RID: 15410
		[SerializeField]
		internal Area[] areas;

		// Token: 0x04003C33 RID: 15411
		[SerializeField]
		internal sbyte templateId;

		// Token: 0x04003C34 RID: 15412
		[CanBeNull]
		[SerializeField]
		public StateMask stateMask;

		// Token: 0x04003C35 RID: 15413
		[CanBeNull]
		[SerializeField]
		internal CToggle dropdown;

		// Token: 0x04003C36 RID: 15414
		[CanBeNull]
		[SerializeField]
		private TMP_Text dropdownLabel;

		// Token: 0x04003C37 RID: 15415
		[CanBeNull]
		[SerializeField]
		private CImage stateName;

		// Token: 0x04003C38 RID: 15416
		private Sprite _stateNameNormal;

		// Token: 0x04003C39 RID: 15417
		private Sprite _stateNameHover;

		// Token: 0x02001BDD RID: 7133
		private struct Comparer : IComparer<Area>
		{
			// Token: 0x0600E570 RID: 58736 RVA: 0x005ED968 File Offset: 0x005EBB68
			public int Compare(Area x, Area y)
			{
				short? num = (x != null) ? new short?(x.Config.TemplateId) : null;
				int? num2 = (num != null) ? new int?((int)num.GetValueOrDefault()) : null;
				num = ((y != null) ? new short?(y.Config.TemplateId) : null);
				int? num3 = (num != null) ? new int?((int)num.GetValueOrDefault()) : null;
				bool flag = num2.GetValueOrDefault() == num3.GetValueOrDefault() & num2 != null == (num3 != null);
				int result;
				if (flag)
				{
					result = 0;
				}
				else
				{
					bool flag2 = !x;
					if (flag2)
					{
						result = 1;
					}
					else
					{
						bool flag3 = !y;
						if (flag3)
						{
							result = -1;
						}
						else
						{
							bool flag4 = x.AreaDisplayData.BrokenLevel <= 0;
							if (flag4)
							{
								result = -1;
							}
							else
							{
								bool flag5 = y.AreaDisplayData.BrokenLevel <= 0;
								if (flag5)
								{
									result = 1;
								}
								else
								{
									bool flag6 = x.AreaDisplayData.IsUnlocked != y.AreaDisplayData.IsUnlocked;
									if (flag6)
									{
										result = (x.AreaDisplayData.IsUnlocked ? -1 : 1);
									}
									else
									{
										result = x.Config.TemplateId.CompareTo(y.Config.TemplateId);
									}
								}
							}
						}
					}
				}
				return result;
			}
		}
	}
}
