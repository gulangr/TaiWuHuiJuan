using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Game.Views.Alertness
{
	// Token: 0x02000C5E RID: 3166
	public class AlertnessRecordGroup : MonoBehaviour
	{
		// Token: 0x0600A165 RID: 41317 RVA: 0x004B6BCC File Offset: 0x004B4DCC
		public void SetData(string title, List<string> contentList, string prefabKey)
		{
			this.textTitle.text = title;
			this._prefabKey = prefabKey;
			foreach (string content in contentList)
			{
				GameObject item = PoolManager.GetObject(prefabKey);
				item.transform.SetParent(this.layout);
				item.transform.localScale = Vector3.one;
				TextMeshProUGUI text = item.GetComponentInChildren<TextMeshProUGUI>();
				text.text = content;
				text.GetComponent<TMPTextSpriteHelper>().Parse();
				this._itemList.Add(item.gameObject);
			}
			base.gameObject.SetActive(contentList.Count > 0);
		}

		// Token: 0x0600A166 RID: 41318 RVA: 0x004B6C98 File Offset: 0x004B4E98
		private void OnDisable()
		{
			for (int i = 0; i < this._itemList.Count; i++)
			{
				GameObject obj = this._itemList[i];
				obj.transform.SetParent(null);
				PoolManager.RemoveObject(this._prefabKey, obj);
			}
			this._itemList.Clear();
		}

		// Token: 0x04007D25 RID: 32037
		[SerializeField]
		private TextMeshProUGUI textTitle;

		// Token: 0x04007D26 RID: 32038
		[SerializeField]
		private Transform layout;

		// Token: 0x04007D27 RID: 32039
		private string _prefabKey;

		// Token: 0x04007D28 RID: 32040
		private readonly List<GameObject> _itemList = new List<GameObject>();
	}
}
