using System;
using FrameWork;
using TMPro;
using UnityEngine;

namespace Game.Views.MouseTips
{
	// Token: 0x02000876 RID: 2166
	public class MouseTipRecordIncompatible : MouseTipBase
	{
		// Token: 0x0600684F RID: 26703 RVA: 0x002FB1E9 File Offset: 0x002F93E9
		protected override void Init(ArgumentBox argsBox)
		{
			this.Refresh(argsBox);
		}

		// Token: 0x06006850 RID: 26704 RVA: 0x002FB1F4 File Offset: 0x002F93F4
		public override void Refresh(ArgumentBox argsBox)
		{
			this.ReadArgs(argsBox);
			this.dataArea.SetActive(this._showDataArea);
			this.versionArea.SetActive(this._showVersionArea);
			bool showVersionArea = this._showVersionArea;
			if (showVersionArea)
			{
				this.recordVersion.text = this._recordVersion;
				this.gameVersion.text = this._gameVersion;
			}
		}

		// Token: 0x06006851 RID: 26705 RVA: 0x002FB260 File Offset: 0x002F9460
		private void ReadArgs(ArgumentBox argsBox)
		{
			argsBox.Get("ShowDataArea", out this._showDataArea);
			argsBox.Get("ShowVersionArea", out this._showVersionArea);
			bool showVersionArea = this._showVersionArea;
			if (showVersionArea)
			{
				argsBox.Get("RecordVersion", out this._recordVersion);
				argsBox.Get("GameVersion", out this._gameVersion);
			}
		}

		// Token: 0x040049F4 RID: 18932
		private bool _showDataArea;

		// Token: 0x040049F5 RID: 18933
		private bool _showVersionArea;

		// Token: 0x040049F6 RID: 18934
		private string _recordVersion;

		// Token: 0x040049F7 RID: 18935
		private string _gameVersion;

		// Token: 0x040049F8 RID: 18936
		[SerializeField]
		private GameObject dataArea;

		// Token: 0x040049F9 RID: 18937
		[SerializeField]
		private GameObject versionArea;

		// Token: 0x040049FA RID: 18938
		[SerializeField]
		private TextMeshProUGUI recordVersion;

		// Token: 0x040049FB RID: 18939
		[SerializeField]
		private TextMeshProUGUI gameVersion;
	}
}
