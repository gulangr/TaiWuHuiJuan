using System;
using Config;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using TMPro;
using UnityEngine;

namespace Game.Views.RecordSelect
{
	// Token: 0x020007BA RID: 1978
	public class RevertArchiveItem : Refers
	{
		// Token: 0x17000BCA RID: 3018
		// (get) Token: 0x06006084 RID: 24708 RVA: 0x002C3E12 File Offset: 0x002C2012
		public Game.Components.Avatar.Avatar Avatar
		{
			get
			{
				return this._avatar;
			}
		}

		// Token: 0x17000BCB RID: 3019
		// (get) Token: 0x06006085 RID: 24709 RVA: 0x002C3E1A File Offset: 0x002C201A
		public TextMeshProUGUI NameText
		{
			get
			{
				return this._name;
			}
		}

		// Token: 0x17000BCC RID: 3020
		// (get) Token: 0x06006086 RID: 24710 RVA: 0x002C3E22 File Offset: 0x002C2022
		public TextMeshProUGUI AgeSamsara
		{
			get
			{
				return this._ageSamsara;
			}
		}

		// Token: 0x17000BCD RID: 3021
		// (get) Token: 0x06006087 RID: 24711 RVA: 0x002C3E2A File Offset: 0x002C202A
		public TextMeshProUGUI SaveTime
		{
			get
			{
				return this._saveTime;
			}
		}

		// Token: 0x17000BCE RID: 3022
		// (get) Token: 0x06006088 RID: 24712 RVA: 0x002C3E32 File Offset: 0x002C2032
		public TextMeshProUGUI Location
		{
			get
			{
				return this._location;
			}
		}

		// Token: 0x17000BCF RID: 3023
		// (get) Token: 0x06006089 RID: 24713 RVA: 0x002C3E3A File Offset: 0x002C203A
		public CImage Hovor
		{
			get
			{
				return this._hovor;
			}
		}

		// Token: 0x17000BD0 RID: 3024
		// (get) Token: 0x0600608A RID: 24714 RVA: 0x002C3E42 File Offset: 0x002C2042
		public CToggle Toggle
		{
			get
			{
				return this._toggle;
			}
		}

		// Token: 0x0600608B RID: 24715 RVA: 0x002C3E4C File Offset: 0x002C204C
		public void SetText(string name, string ageSamsara, string saveTime, string location, string location2)
		{
			this._name.text = name;
			this._ageSamsara.text = ageSamsara;
			this._saveTime.text = saveTime;
			this._location.text = location + "-" + location2;
			this._locationIcon.SetSprite("ui9_icon_main_arrive_icon_0_" + ((int)(this.GetStateId(location) - 1)).ToString(), false, null);
		}

		// Token: 0x0600608C RID: 24716 RVA: 0x002C3EC8 File Offset: 0x002C20C8
		public void InitToggleListener(Action<bool> onValueChanged)
		{
			this._toggle.onValueChanged.RemoveAllListeners();
			this._toggle.onValueChanged.AddListener(delegate(bool isOn)
			{
				Action<bool> onValueChanged2 = onValueChanged;
				if (onValueChanged2 != null)
				{
					onValueChanged2(isOn);
				}
			});
		}

		// Token: 0x0600608D RID: 24717 RVA: 0x002C3F14 File Offset: 0x002C2114
		public void SetDoubleClickCallback(Action callback)
		{
			bool flag = this._pointClickBridge;
			if (flag)
			{
				this._pointClickBridge.OnDoubleClick = delegate()
				{
					Action callback2 = callback;
					if (callback2 != null)
					{
						callback2();
					}
				};
			}
		}

		// Token: 0x0600608E RID: 24718 RVA: 0x002C3F57 File Offset: 0x002C2157
		public void SetToggleOn(bool isOn)
		{
			this._toggle.isOn = isOn;
		}

		// Token: 0x0600608F RID: 24719 RVA: 0x002C3F68 File Offset: 0x002C2168
		private short GetStateId(string stateName)
		{
			short stateId = 0;
			MapState.Instance.Iterate(delegate(MapStateItem e)
			{
				bool flag = e.Name == stateName;
				bool result;
				if (flag)
				{
					stateId = (short)e.TemplateId;
					result = false;
				}
				else
				{
					result = true;
				}
				return result;
			});
			return stateId;
		}

		// Token: 0x040042E9 RID: 17129
		[SerializeField]
		private Game.Components.Avatar.Avatar _avatar;

		// Token: 0x040042EA RID: 17130
		[SerializeField]
		private TextMeshProUGUI _name;

		// Token: 0x040042EB RID: 17131
		[SerializeField]
		private TextMeshProUGUI _ageSamsara;

		// Token: 0x040042EC RID: 17132
		[SerializeField]
		private TextMeshProUGUI _saveTime;

		// Token: 0x040042ED RID: 17133
		[SerializeField]
		private TextMeshProUGUI _location;

		// Token: 0x040042EE RID: 17134
		[SerializeField]
		private CImage _locationIcon;

		// Token: 0x040042EF RID: 17135
		[SerializeField]
		private CImage _hovor;

		// Token: 0x040042F0 RID: 17136
		[SerializeField]
		private CToggle _toggle;

		// Token: 0x040042F1 RID: 17137
		[SerializeField]
		private PointClickBridge _pointClickBridge;
	}
}
