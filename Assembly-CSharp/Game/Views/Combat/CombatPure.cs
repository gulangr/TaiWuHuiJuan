using System;
using System.Collections.Generic;
using FrameWork;
using UnityEngine;

namespace Game.Views.Combat
{
	// Token: 0x02000AF9 RID: 2809
	public class CombatPure : MonoBehaviour, ICombatComponent
	{
		// Token: 0x06008A4F RID: 35407 RVA: 0x00400CD8 File Offset: 0x003FEED8
		private void Awake()
		{
			this._needHideObjectOriginalPos.Clear();
			foreach (RectTransform target in this.needHideObject)
			{
				this._needHideObjectOriginalPos.TryAdd(target.name, target.localPosition);
			}
			foreach (RectTransform target2 in this.pureUI)
			{
				this._needHideObjectOriginalPos.TryAdd(target2.name, target2.localPosition);
			}
		}

		// Token: 0x06008A50 RID: 35408 RVA: 0x00400D62 File Offset: 0x003FEF62
		public void Setup()
		{
			GEvent.Add(UiEvents.CombatPureMode, new GEvent.Callback(this.CombatPureMode));
		}

		// Token: 0x06008A51 RID: 35409 RVA: 0x00400D81 File Offset: 0x003FEF81
		public void Close()
		{
			GEvent.Remove(UiEvents.CombatPureMode, new GEvent.Callback(this.CombatPureMode));
		}

		// Token: 0x06008A52 RID: 35410 RVA: 0x00400DA0 File Offset: 0x003FEFA0
		private void CombatPureMode(ArgumentBox argBox)
		{
			this.OpenPure(CombatUtils.CombatPureOpen);
		}

		// Token: 0x06008A53 RID: 35411 RVA: 0x00400DB0 File Offset: 0x003FEFB0
		public void OpenPure(bool openPure)
		{
			foreach (RectTransform target in this.needHideObject)
			{
				target.localPosition = (openPure ? this._hideLocation : this._needHideObjectOriginalPos[target.name]);
			}
			foreach (RectTransform target2 in this.pureUI)
			{
				target2.localPosition = ((!openPure) ? this._hideLocation : this._needHideObjectOriginalPos[target2.name]);
			}
		}

		// Token: 0x04006A14 RID: 27156
		public RectTransform[] needHideObject;

		// Token: 0x04006A15 RID: 27157
		public RectTransform[] pureUI;

		// Token: 0x04006A16 RID: 27158
		private Dictionary<string, Vector3> _needHideObjectOriginalPos = new Dictionary<string, Vector3>();

		// Token: 0x04006A17 RID: 27159
		private readonly Vector3 _hideLocation = new Vector3(0f, 10000f, 0f);
	}
}
