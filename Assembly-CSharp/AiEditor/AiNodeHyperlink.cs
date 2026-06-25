using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AiEditor
{
	// Token: 0x0200067C RID: 1660
	[RequireComponent(typeof(TMP_Text))]
	public class AiNodeHyperlink : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
	{
		// Token: 0x17000990 RID: 2448
		// (get) Token: 0x06004E6E RID: 20078 RVA: 0x0024D8CF File Offset: 0x0024BACF
		private TMP_Text Text
		{
			get
			{
				return base.GetComponent<TMP_Text>();
			}
		}

		// Token: 0x17000991 RID: 2449
		// (get) Token: 0x06004E6F RID: 20079 RVA: 0x0024D8D7 File Offset: 0x0024BAD7
		// (set) Token: 0x06004E70 RID: 20080 RVA: 0x0024D8DF File Offset: 0x0024BADF
		public int TemplateId { get; private set; }

		// Token: 0x06004E71 RID: 20081 RVA: 0x0024D8E8 File Offset: 0x0024BAE8
		public void Bind(IAiNodeHyperlinkHandler handler)
		{
			this._handler = handler;
		}

		// Token: 0x06004E72 RID: 20082 RVA: 0x0024D8F4 File Offset: 0x0024BAF4
		public void SetCondition(int templateId)
		{
			this._setParams.Clear();
			AiConditionItem config = AiCondition.Instance[templateId];
			this.TemplateId = templateId;
			this._context = new ValueTuple<string, IReadOnlyList<int>, IReadOnlyList<int>>(config.Desc, config.ParamStrings, config.ParamInts);
			this.UpdateDesc();
		}

		// Token: 0x06004E73 RID: 20083 RVA: 0x0024D94C File Offset: 0x0024BB4C
		public void SetAction(int templateId)
		{
			this._setParams.Clear();
			AiActionItem config = AiAction.Instance[templateId];
			this.TemplateId = templateId;
			this._context = new ValueTuple<string, IReadOnlyList<int>, IReadOnlyList<int>>(config.Desc, config.ParamStrings, config.ParamInts);
			this.UpdateDesc();
		}

		// Token: 0x06004E74 RID: 20084 RVA: 0x0024D9A3 File Offset: 0x0024BBA3
		public void UpdateDesc()
		{
			this.Text.text = this._context.Parse(this._setParams);
		}

		// Token: 0x06004E75 RID: 20085 RVA: 0x0024D9C4 File Offset: 0x0024BBC4
		public void OnPointerClick(PointerEventData eventData)
		{
			int index = TMP_TextUtilities.FindIntersectingLink(this.Text, eventData.position, UIManager.Instance.UiCamera);
			bool flag = index < 0;
			if (!flag)
			{
				TMP_LinkInfo link = this.Text.textInfo.linkInfo[index];
				string[] linkArray = link.GetLinkID().Split('_', StringSplitOptions.None);
				int paramIndex2 = int.Parse(linkArray[0]);
				int num = int.Parse(linkArray[1]);
				int paramIndex = paramIndex2;
				int paramTemplateId = num;
				bool flag2 = AiParam.Instance[paramTemplateId] == null;
				if (flag2)
				{
					Debug.LogWarning(string.Format("Cannot edit ai param {0}, root is {1}", paramTemplateId, this._context.Desc));
				}
				else
				{
					ArgumentBox args = EasyPool.Get<ArgumentBox>();
					args.Set("TemplateId", paramTemplateId);
					args.SetObject("SelectCallback", new Action<string>(delegate(string str)
					{
						this._handler.SetParam(delegate
						{
							this._setParams[paramIndex] = str;
						});
						this.UpdateDesc();
					}));
					UIElement.AiParamInputField.SetOnInitArgs(args);
					UIManager.Instance.ShowUI(UIElement.AiParamInputField, true);
				}
			}
		}

		// Token: 0x06004E76 RID: 20086 RVA: 0x0024DADC File Offset: 0x0024BCDC
		public List<string> Save()
		{
			List<string> param = new List<string>(this._context.Count);
			for (int i = 0; i < this._context.Count; i++)
			{
				string set;
				param.Add(this._setParams.TryGetValue(i, out set) ? set : string.Empty);
			}
			return param;
		}

		// Token: 0x06004E77 RID: 20087 RVA: 0x0024DB3C File Offset: 0x0024BD3C
		public void Load(List<string> data)
		{
			this._setParams.Clear();
			for (int i = 0; i < data.Count; i++)
			{
				bool flag = AiParam.Instance[this._context.Get(i)].IsValid(data[i]);
				if (flag)
				{
					this._setParams[i] = data[i];
				}
			}
			this.UpdateDesc();
		}

		// Token: 0x0400363D RID: 13885
		private AiNodeHyperlink.ParseContext _context;

		// Token: 0x0400363E RID: 13886
		private readonly Dictionary<int, string> _setParams = new Dictionary<int, string>();

		// Token: 0x0400363F RID: 13887
		private IAiNodeHyperlinkHandler _handler;

		// Token: 0x02001AA9 RID: 6825
		public struct ParseContext
		{
			// Token: 0x170017B9 RID: 6073
			// (get) Token: 0x0600DEDD RID: 57053 RVA: 0x005D6683 File Offset: 0x005D4883
			public int Count
			{
				get
				{
					IReadOnlyList<int> paramStrings = this.ParamStrings;
					int num = (paramStrings != null) ? paramStrings.Count : 0;
					IReadOnlyList<int> paramInts = this.ParamInts;
					return num + ((paramInts != null) ? paramInts.Count : 0);
				}
			}

			// Token: 0x0600DEDE RID: 57054 RVA: 0x005D66AC File Offset: 0x005D48AC
			public int Get(int index)
			{
				bool flag = index < 0;
				if (flag)
				{
					throw new IndexOutOfRangeException(string.Format("Cannot use negative index {0}", index));
				}
				bool flag2 = index >= this.Count;
				if (flag2)
				{
					throw new IndexOutOfRangeException(string.Format("Index out of count {0} {1}", index, this.Count));
				}
				bool flag3 = this.ParamStrings != null && index < this.ParamStrings.Count;
				int result;
				if (flag3)
				{
					result = this.ParamStrings[index];
				}
				else
				{
					IReadOnlyList<int> paramInts = this.ParamInts;
					IReadOnlyList<int> paramStrings = this.ParamStrings;
					result = paramInts[index - ((paramStrings != null) ? paramStrings.Count : 0)];
				}
				return result;
			}

			// Token: 0x0600DEDF RID: 57055 RVA: 0x005D6758 File Offset: 0x005D4958
			public string Parse(Dictionary<int, string> setParams)
			{
				string result = this.Desc;
				for (int i = 0; i < this.Count; i++)
				{
					AiParamItem config = AiParam.Instance[this.Get(i)];
					string paramPos = string.Format("{{{0}}}", i);
					string str;
					string link = string.Format("<link=\"{0}_{1}\">{2}</link>", i, this.Get(i), setParams.TryGetValue(i, out str) ? config.ParseAliases(str, false).SetColor(Color.green) : "click".SetColor(Color.red));
					result = result.Replace(paramPos, link);
				}
				return result;
			}

			// Token: 0x0600DEE0 RID: 57056 RVA: 0x005D6810 File Offset: 0x005D4A10
			public static implicit operator AiNodeHyperlink.ParseContext(ValueTuple<string, IReadOnlyList<int>, IReadOnlyList<int>> tuple)
			{
				return new AiNodeHyperlink.ParseContext
				{
					Desc = tuple.Item1,
					ParamStrings = tuple.Item2,
					ParamInts = tuple.Item3
				};
			}

			// Token: 0x0400B6C1 RID: 46785
			public string Desc;

			// Token: 0x0400B6C2 RID: 46786
			public IReadOnlyList<int> ParamStrings;

			// Token: 0x0400B6C3 RID: 46787
			public IReadOnlyList<int> ParamInts;
		}
	}
}
