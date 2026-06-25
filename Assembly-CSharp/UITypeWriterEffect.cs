using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Token: 0x020000B1 RID: 177
[RequireComponent(typeof(TextMeshProUGUI))]
public class UITypeWriterEffect : MonoBehaviour
{
	// Token: 0x06000611 RID: 1553 RVA: 0x00028B78 File Offset: 0x00026D78
	private void Awake()
	{
		this.TextObject = base.GetComponent<TextMeshProUGUI>();
		this._outputString = this.TextObject.text;
		this._richtext = this.TextObject.richText;
		this.TextObject.text = "";
		this._offset = -1;
		this._delay = 1f / (float)this.CharsPerSec;
		this._defaultDelay = new WaitForSeconds(this._delay);
		this._nlDelay = new WaitForSeconds(this._delay + this.NewLineDelay);
		this._pDelay = new WaitForSeconds(this._delay + this.PeriodDelay);
		this._tagString = "";
	}

	// Token: 0x06000612 RID: 1554 RVA: 0x00028C2C File Offset: 0x00026E2C
	private void Start()
	{
		bool active = this.Active;
		if (active)
		{
			bool richtext = this._richtext;
			if (richtext)
			{
				base.StartCoroutine("OnTypeWritingRichText");
			}
			else
			{
				base.StartCoroutine("OnTypeWriting");
			}
		}
	}

	// Token: 0x06000613 RID: 1555 RVA: 0x00028C6C File Offset: 0x00026E6C
	public void StartTypeWriter()
	{
		this.Active = true;
		bool richtext = this._richtext;
		if (richtext)
		{
			base.StartCoroutine("OnTypeWritingRichText");
		}
		else
		{
			base.StartCoroutine("OnTypeWriting");
		}
	}

	// Token: 0x06000614 RID: 1556 RVA: 0x00028CA8 File Offset: 0x00026EA8
	public void StartTypeWriter(string str)
	{
		bool flag = str.IsNullOrEmpty();
		if (!flag)
		{
			this.ResetTypeWriter();
			this._outputString = str;
			this.Active = true;
			bool richtext = this._richtext;
			if (richtext)
			{
				base.StartCoroutine("OnTypeWritingRichText");
			}
			else
			{
				base.StartCoroutine("OnTypeWriting");
			}
		}
	}

	// Token: 0x06000615 RID: 1557 RVA: 0x00028D00 File Offset: 0x00026F00
	public void SetCharsPerSec(int cps)
	{
		this.CharsPerSec = cps;
		this._delay = 1f / (float)this.CharsPerSec;
		this._defaultDelay = new WaitForSeconds(this._delay);
		this._nlDelay = new WaitForSeconds(this._delay + this.NewLineDelay);
		this._pDelay = new WaitForSeconds(this._delay + this.PeriodDelay);
	}

	// Token: 0x06000616 RID: 1558 RVA: 0x00028D6C File Offset: 0x00026F6C
	public void SetSpecialDelay(params float[] args)
	{
		bool flag = args == null || args.Length == 0;
		if (!flag)
		{
			bool flag2 = args.Length != 0;
			if (flag2)
			{
				this.NewLineDelay = args[0];
				this._nlDelay = new WaitForSeconds(this._delay + this.NewLineDelay);
			}
			bool flag3 = args.Length > 1;
			if (flag3)
			{
				this.PeriodDelay = args[1];
				this._pDelay = new WaitForSeconds(this._delay + this.PeriodDelay);
			}
		}
	}

	// Token: 0x06000617 RID: 1559 RVA: 0x00028DE4 File Offset: 0x00026FE4
	public void ResetTypeWriter()
	{
		base.StopAllCoroutines();
		this.Active = false;
		this._offset = -1;
		this.TextObject.text = "";
		this._tagString = "";
	}

	// Token: 0x06000618 RID: 1560 RVA: 0x00028E18 File Offset: 0x00027018
	public void StopTypeWriter()
	{
		this.onFinish();
		this.TextObject.text = this._outputString;
	}

	// Token: 0x06000619 RID: 1561 RVA: 0x00028E34 File Offset: 0x00027034
	private IEnumerator OnTypeWriting()
	{
		while (this.Active)
		{
			this._offset++;
			this.TextObject.text = this._outputString.Substring(0, this._offset + 1);
			bool flag = this._offset >= this._outputString.Length - 1;
			if (flag)
			{
				this.onFinish();
				break;
			}
			char c = this._outputString[this._offset];
			bool flag2 = c == '.' || c == '。' || c == '！' || c == '？';
			if (flag2)
			{
				yield return this._pDelay;
			}
			else
			{
				bool flag3 = c == '\n';
				if (flag3)
				{
					yield return this._nlDelay;
				}
				else
				{
					yield return this._defaultDelay;
				}
			}
		}
		yield break;
	}

	// Token: 0x0600061A RID: 1562 RVA: 0x00028E43 File Offset: 0x00027043
	private IEnumerator OnTypeWritingRichText()
	{
		while (this.Active)
		{
			bool flag = this._offset >= this._outputString.Length - 1;
			if (flag)
			{
				this.onFinish();
				break;
			}
			bool flag2;
			do
			{
				string outputString = this._outputString;
				int num = this._offset + 1;
				this._offset = num;
				if (outputString[num] != '<')
				{
					break;
				}
				this.ProcessTag();
				flag2 = (this._offset >= this._outputString.Length - 1);
			}
			while (!flag2);
			IL_E7:
			this.TextObject.text = this._outputString.Substring(0, this._offset + 1) + this._tagString;
			char c = this._outputString[this._offset];
			bool flag3 = c == '.' || c == '。' || c == '！' || c == '？';
			if (flag3)
			{
				yield return this._pDelay;
			}
			else
			{
				bool flag4 = c == '\n';
				if (flag4)
				{
					yield return this._nlDelay;
				}
				else
				{
					yield return this._defaultDelay;
				}
			}
			continue;
			goto IL_E7;
		}
		yield break;
	}

	// Token: 0x0600061B RID: 1563 RVA: 0x00028E54 File Offset: 0x00027054
	private void ProcessTag()
	{
		bool flag = this._outputString[this._offset] != '<';
		if (!flag)
		{
			int prevOffset = this._offset;
			bool flag2;
			do
			{
				string outputString = this._outputString;
				int num = this._offset + 1;
				this._offset = num;
				if (outputString[num] == '>')
				{
					goto Block_3;
				}
				flag2 = (this._offset >= this._outputString.Length - 1);
			}
			while (!flag2);
			this._offset = prevOffset;
			return;
			Block_3:
			string currTag = this._outputString.Substring(prevOffset, this._offset - prevOffset + 1);
			bool flag3 = this.tagDic.ContainsKey(currTag);
			if (flag3)
			{
				this._tagString = this._tagString.Insert(0, this.tagDic[currTag]);
			}
			else
			{
				bool flag4 = this.tagDic.ContainsValue(currTag);
				if (flag4)
				{
					this._tagString = this._tagString.Remove(0, currTag.Length);
				}
				else
				{
					this._offset = prevOffset;
				}
			}
		}
	}

	// Token: 0x0600061C RID: 1564 RVA: 0x00028F65 File Offset: 0x00027165
	private void onFinish()
	{
		this.Active = false;
	}

	// Token: 0x040004F6 RID: 1270
	public TextMeshProUGUI TextObject;

	// Token: 0x040004F7 RID: 1271
	public int CharsPerSec = 10;

	// Token: 0x040004F8 RID: 1272
	public float NewLineDelay = 0f;

	// Token: 0x040004F9 RID: 1273
	public float PeriodDelay = 0f;

	// Token: 0x040004FA RID: 1274
	public bool Active = false;

	// Token: 0x040004FB RID: 1275
	private string _outputString;

	// Token: 0x040004FC RID: 1276
	private int _offset;

	// Token: 0x040004FD RID: 1277
	private float _delay;

	// Token: 0x040004FE RID: 1278
	private bool _richtext;

	// Token: 0x040004FF RID: 1279
	private static UITypeWriterEffect.StartsWithComparer _startsWithComparer = new UITypeWriterEffect.StartsWithComparer();

	// Token: 0x04000500 RID: 1280
	private WaitForSeconds _defaultDelay;

	// Token: 0x04000501 RID: 1281
	private WaitForSeconds _nlDelay;

	// Token: 0x04000502 RID: 1282
	private WaitForSeconds _pDelay;

	// Token: 0x04000503 RID: 1283
	private string _tagString;

	// Token: 0x04000504 RID: 1284
	private Dictionary<string, string> tagDic = new Dictionary<string, string>(UITypeWriterEffect._startsWithComparer)
	{
		{
			"<size=",
			"</size>"
		},
		{
			"<color=",
			"</color>"
		},
		{
			"<b>",
			"</b>"
		},
		{
			"<i>",
			"</i>"
		}
	};

	// Token: 0x02001112 RID: 4370
	public class StartsWithComparer : IEqualityComparer<string>
	{
		// Token: 0x0600C14D RID: 49485 RVA: 0x0056E4EC File Offset: 0x0056C6EC
		public bool Equals(string x, string y)
		{
			return x.StartsWith(y, StringComparison.InvariantCultureIgnoreCase) || y.StartsWith(x, StringComparison.InvariantCultureIgnoreCase);
		}

		// Token: 0x0600C14E RID: 49486 RVA: 0x0056E514 File Offset: 0x0056C714
		public int GetHashCode(string obj)
		{
			return base.GetHashCode();
		}
	}
}
