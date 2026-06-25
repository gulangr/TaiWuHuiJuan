using System;
using System.Linq;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;

namespace Game.Views.Legacy.AdventureEditor
{
	// Token: 0x02000A00 RID: 2560
	public class ResourceEditor : MonoBehaviour
	{
		// Token: 0x06007DC9 RID: 32201 RVA: 0x003A67E8 File Offset: 0x003A49E8
		private void Awake()
		{
			this.removeButton.onClick.ResetListener(delegate()
			{
				this._remove(this.index);
				this._refresh();
			});
			this.valueInput.onEndEdit.ResetListener(delegate(string str)
			{
				int val;
				bool flag = !int.TryParse(str, out val);
				if (flag)
				{
					Debug.LogError("Invalid value: " + str);
				}
				this._setData(this.index, new ValueTuple<int, int>(this.type.value, val));
			});
			this.type.onValueChanged.ResetListener(delegate(int idx)
			{
				this._setData(this.index, new ValueTuple<int, int>(idx, this._getData(this.index).Item2));
			});
			this.type.ClearOptions();
			this.type.AddOptions(this.options.Select(new Func<string, string>(LocalStringManager.Get)).ToList<string>());
		}

		// Token: 0x06007DCA RID: 32202 RVA: 0x003A6884 File Offset: 0x003A4A84
		public void Set(int itemIndex, Func<int, ValueTuple<int, int>> getData, Action<int, ValueTuple<int, int>> setData, Action<int> remove, Action refresh)
		{
			this.index = itemIndex;
			this._getData = getData;
			this._setData = setData;
			this._remove = remove;
			this._refresh = refresh;
			ValueTuple<int, int> data = getData(this.index);
			this.type.SetValueWithoutNotify(data.Item1);
			this.valueInput.SetTextWithoutNotify(data.Item2.ToString());
		}

		// Token: 0x04005FD5 RID: 24533
		private Func<int, ValueTuple<int, int>> _getData;

		// Token: 0x04005FD6 RID: 24534
		private Action<int, ValueTuple<int, int>> _setData;

		// Token: 0x04005FD7 RID: 24535
		private Action<int> _remove;

		// Token: 0x04005FD8 RID: 24536
		private Action _refresh;

		// Token: 0x04005FD9 RID: 24537
		[SerializeField]
		private int index;

		// Token: 0x04005FDA RID: 24538
		[SerializeField]
		private CDropdown type;

		// Token: 0x04005FDB RID: 24539
		[SerializeField]
		private TMP_InputField valueInput;

		// Token: 0x04005FDC RID: 24540
		[SerializeField]
		private CButton removeButton;

		// Token: 0x04005FDD RID: 24541
		[SerializeField]
		private string[] options;
	}
}
