using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000DD RID: 221
public class Colors : MonoBehaviour
{
	// Token: 0x170000BF RID: 191
	// (get) Token: 0x060007DB RID: 2011 RVA: 0x00036C4F File Offset: 0x00034E4F
	// (set) Token: 0x060007DC RID: 2012 RVA: 0x00036C56 File Offset: 0x00034E56
	public static Colors Instance { get; private set; }

	// Token: 0x060007DD RID: 2013 RVA: 0x00036C5E File Offset: 0x00034E5E
	private void Awake()
	{
		Colors.Instance = this;
		this.InitPresetColors();
	}

	// Token: 0x170000C0 RID: 192
	public Color this[string colorName]
	{
		get
		{
			Color color;
			return this._presetColorsDic.TryGetValue(colorName, out color) ? color : Color.white;
		}
	}

	// Token: 0x060007DF RID: 2015 RVA: 0x00036C98 File Offset: 0x00034E98
	private void InitPresetColors()
	{
		this._presetColorsDic = new Dictionary<string, Color>();
		for (int i = 0; i < this.PresetColorNames.Count; i++)
		{
			this._presetColorsDic.Add(this.PresetColorNames[i], (i < this.PresetColors.Count) ? this.PresetColors[i] : Color.white);
		}
	}

	// Token: 0x040008C3 RID: 2243
	public Color[] GradeColors;

	// Token: 0x040008C4 RID: 2244
	public Color[] AttractionTypeColors;

	// Token: 0x040008C5 RID: 2245
	public Color[] HappinessTypeColors;

	// Token: 0x040008C6 RID: 2246
	public Color[] FameTypeColors;

	// Token: 0x040008C7 RID: 2247
	public Color[] FavorabilityTypeColors;

	// Token: 0x040008C8 RID: 2248
	public Color[] BehaviorTypeColors;

	// Token: 0x040008C9 RID: 2249
	public Color[] PersonalityTypeColors;

	// Token: 0x040008CA RID: 2250
	public Color[] PosionColors;

	// Token: 0x040008CB RID: 2251
	public Color[] FiveElementsColors;

	// Token: 0x040008CC RID: 2252
	public Color[] AlertnessColors;

	// Token: 0x040008CD RID: 2253
	public List<string> PresetColorNames;

	// Token: 0x040008CE RID: 2254
	public List<Color> PresetColors;

	// Token: 0x040008CF RID: 2255
	private Dictionary<string, Color> _presetColorsDic;
}
