using System;
using Config;
using UnityEngine;

// Token: 0x020001CF RID: 463
[ExecuteInEditMode]
public class QiContainer : MonoBehaviour
{
	// Token: 0x06001CC8 RID: 7368 RVA: 0x000C9AC0 File Offset: 0x000C7CC0
	public void Awake()
	{
		bool flag = this.Self.childCount < 6;
		if (flag)
		{
			QiNumber[] newItems = new QiNumber[6];
			for (int i = 0; i < this.Children.Length; i++)
			{
				newItems[i] = this.Children[i];
				newItems[i].gameObject.SetActive(true);
			}
			for (int j = this.Children.Length; j < newItems.Length; j++)
			{
				newItems[j] = Object.Instantiate<QiNumber>(newItems[0], this.Self);
			}
			this.Children = newItems;
		}
		else
		{
			for (int k = 6; k < this.Children.Length; k++)
			{
				this.Children[k].gameObject.SetActive(false);
			}
		}
		this.Children[0].Init(QiDisorderEffect.DefValue.Cutoff, QiNumber.EQiNumberState.End);
		for (int l = 1; l < 4; l++)
		{
			this.Children[l].Init(QiDisorderEffect.Instance[5 - l], QiNumber.EQiNumberState.Value);
		}
		this.Children[5].Init(QiDisorderEffect.Instance[0], QiNumber.EQiNumberState.Start);
	}

	// Token: 0x06001CC9 RID: 7369 RVA: 0x000C9BEC File Offset: 0x000C7DEC
	public void OnValueChanged(int value)
	{
		Array.ForEach<QiNumber>(this.Children, delegate(QiNumber x)
		{
			x.ApplyValue(value);
		});
	}

	// Token: 0x0400165C RID: 5724
	public RectTransform Self;

	// Token: 0x0400165D RID: 5725
	public QiNumber[] Children;
}
