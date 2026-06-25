using System;
using System.Collections.Generic;
using System.Linq;
using FrameWork.UISystem.Components;
using Game.Components.Avatar;
using Game.Views.CharacterMenu;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020001D6 RID: 470
public class GenealogyCharacterPanel : UIBehaviour
{
	// Token: 0x17000317 RID: 791
	// (get) Token: 0x06001EA0 RID: 7840 RVA: 0x000DD0DF File Offset: 0x000DB2DF
	public Game.Components.Avatar.Avatar Avatar
	{
		get
		{
			return this.avatar;
		}
	}

	// Token: 0x17000318 RID: 792
	// (set) Token: 0x06001EA1 RID: 7841 RVA: 0x000DD0E7 File Offset: 0x000DB2E7
	public string Name
	{
		set
		{
			this.nameFrame.SetName(value);
		}
	}

	// Token: 0x17000319 RID: 793
	// (get) Token: 0x06001EA2 RID: 7842 RVA: 0x000DD0F6 File Offset: 0x000DB2F6
	public string TestName
	{
		get
		{
			return this.nameFrame.NameLabel.text;
		}
	}

	// Token: 0x06001EA3 RID: 7843 RVA: 0x000DD108 File Offset: 0x000DB308
	protected override void Awake()
	{
		this.SetBirthAndDeathDate(-1, -1);
		this.SetRelation(GenealogyCharacterPanel.RelationType.Unknown, ushort.MaxValue);
		base.Awake();
	}

	// Token: 0x06001EA4 RID: 7844 RVA: 0x000DD128 File Offset: 0x000DB328
	public void OnPointerEnter()
	{
		this._onFocused.Invoke(0);
	}

	// Token: 0x06001EA5 RID: 7845 RVA: 0x000DD138 File Offset: 0x000DB338
	public void OnPointerExit()
	{
		this._onFocusRestore.Invoke(0);
	}

	// Token: 0x06001EA6 RID: 7846 RVA: 0x000DD148 File Offset: 0x000DB348
	internal void SetBirthAndDeathDate(int birth, int death)
	{
		TimeManager tm = SingletonObject.getInstance<TimeManager>();
		string textBirth = (birth < 0) ? LanguageKey.LK_Genealogy_BirthAndDeath_LessThanZero.TrFormat(-tm.GetYearByDate(birth)) : tm.GetYearByDate(birth).ToString();
		this.valueLabel.text = ((death < 0) ? LanguageKey.LK_Genealogy_BirthAndDeath_A.TrFormat(textBirth) : LanguageKey.LK_Genealogy_BirthAndDeath_B.TrFormat(textBirth, tm.GetYearByDate(death)));
	}

	// Token: 0x06001EA7 RID: 7847 RVA: 0x000DD1BD File Offset: 0x000DB3BD
	internal void SetBirthAndDeathDate(string text)
	{
		this.valueLabel.text = text;
	}

	// Token: 0x06001EA8 RID: 7848 RVA: 0x000DD1D0 File Offset: 0x000DB3D0
	internal void SetRelation(GenealogyCharacterPanel.RelationType type, ushort relation)
	{
		TextMeshProUGUI textMeshProUGUI = this.titleLabel;
		if (!true)
		{
		}
		string text;
		switch (type)
		{
		case GenealogyCharacterPanel.RelationType.Blood:
			text = LanguageKey.LK_RelationShipSpec_Blood.Tr();
			break;
		case GenealogyCharacterPanel.RelationType.Step:
			text = LanguageKey.LK_RelationShipSpec_Step.Tr();
			break;
		case GenealogyCharacterPanel.RelationType.Adoptive:
			text = LanguageKey.LK_RelationShipSpec_Adoptive.Tr();
			break;
		case GenealogyCharacterPanel.RelationType.Spouse:
			text = LanguageKey.LK_RelationShipSpec_Spouse.Tr();
			break;
		default:
			text = string.Empty;
			break;
		}
		if (!true)
		{
		}
		textMeshProUGUI.text = text;
		CImage cimage = this.backGround;
		if (!true)
		{
		}
		Sprite sprite;
		if (relation <= 32)
		{
			if (relation <= 4)
			{
				if (relation - 1 <= 1)
				{
					goto IL_F1;
				}
				if (relation != 4)
				{
					goto IL_FC;
				}
			}
			else
			{
				if (relation == 8 || relation == 16)
				{
					goto IL_F1;
				}
				if (relation != 32)
				{
					goto IL_FC;
				}
			}
		}
		else if (relation <= 128)
		{
			if (relation != 64 && relation != 128)
			{
				goto IL_FC;
			}
			goto IL_F1;
		}
		else if (relation != 256 && relation != 512)
		{
			if (relation != 1024)
			{
				goto IL_FC;
			}
			sprite = this.spriteBacks[2];
			goto IL_107;
		}
		sprite = this.spriteBacks[3];
		goto IL_107;
		IL_F1:
		sprite = this.spriteBacks[1];
		goto IL_107;
		IL_FC:
		sprite = this.spriteBacks[4];
		IL_107:
		if (!true)
		{
		}
		cimage.sprite = sprite;
		CImage cimage2 = this.titleIcon;
		if (!true)
		{
		}
		switch (type)
		{
		case GenealogyCharacterPanel.RelationType.Blood:
			sprite = this.spriteIcons[2];
			break;
		case GenealogyCharacterPanel.RelationType.Step:
			sprite = this.spriteIcons[0];
			break;
		case GenealogyCharacterPanel.RelationType.Adoptive:
			sprite = this.spriteIcons[1];
			break;
		case GenealogyCharacterPanel.RelationType.Spouse:
			sprite = this.spriteIcons[3];
			break;
		default:
			sprite = null;
			break;
		}
		if (!true)
		{
		}
		cimage2.sprite = sprite;
		this.titleIcon.enabled = (this.titleIcon.sprite != null);
		this.hsvStyleRoot.SetDefault();
	}

	// Token: 0x06001EA9 RID: 7849 RVA: 0x000DD37C File Offset: 0x000DB57C
	internal void SetRelationLinking(IEnumerable<GenealogyCharacterPanel.RelationLinking> links, RectTransform linkBaseRoot, TemplatedContainerAssembly templatedContainerAssembly, int mainCharId, ViewCharacterMenuGenealogy.EGeneration curGeneration, GenealogyCharacterPanel nodeCompToMain)
	{
		RectTransform rectSelf = base.GetComponent<RectTransform>();
		GenealogyCharacterPanel.RelationLinking[] data = links.ToArray<GenealogyCharacterPanel.RelationLinking>();
		List<Vector2> temp = new List<Vector2>();
		this._onFocused.RemoveAllListeners();
		this._onFocusRestore.RemoveAllListeners();
		this._onFocused.AddListener(delegate(int depth)
		{
			GenealogyCharacterPanel nodeCompToMain2 = nodeCompToMain;
			if (nodeCompToMain2 != null)
			{
				UnityEvent<int> onFocused = nodeCompToMain2._onFocused;
				if (onFocused != null)
				{
					onFocused.Invoke(++depth);
				}
			}
		});
		this._onFocusRestore.AddListener(delegate(int depth)
		{
			bool flag = depth >= 5;
			if (flag)
			{
				Debug.LogError("Genealogy _onFocused depth error");
			}
			else
			{
				GenealogyCharacterPanel nodeCompToMain2 = nodeCompToMain;
				if (nodeCompToMain2 != null)
				{
					UnityEvent<int> onFocusRestore = nodeCompToMain2._onFocusRestore;
					if (onFocusRestore != null)
					{
						onFocusRestore.Invoke(++depth);
					}
				}
			}
		});
		templatedContainerAssembly.transform.position = base.transform.position;
		templatedContainerAssembly.Rebuild(data.Length * 2, delegate(Refers unit, int idx)
		{
			bool isShadow = idx < data.Length;
			GenealogyCharacterPanel.RelationLinking link = data[isShadow ? idx : (idx - data.Length)];
			Matrix4x4 lineFromWorldMatrix = unit.RectTransform.worldToLocalMatrix;
			Line2DGenerator lineWork = unit.GetComponent<Line2DGenerator>();
			RectTransform rectTarget = link.Target.GetComponent<RectTransform>();
			Vector3 pointStart = rectSelf.position;
			bool reverse = false;
			bool flag = rectTarget.position.y > rectSelf.position.y;
			Vector3 pointEnd;
			if (flag)
			{
				pointEnd = rectTarget.localToWorldMatrix.MultiplyPoint(new Vector3(0f, rectTarget.rect.yMin));
				reverse = true;
			}
			else
			{
				pointEnd = rectTarget.localToWorldMatrix.MultiplyPoint(new Vector3(0f, rectTarget.rect.yMax));
			}
			temp.Clear();
			temp.Add(pointStart);
			bool flag2 = !Mathf.Approximately(pointStart.x, pointEnd.x);
			if (flag2)
			{
				bool aboveLine = link.AboveLine;
				float jointY;
				if (aboveLine)
				{
					RectTransform baseRect = reverse ? rectTarget : rectSelf;
					jointY = baseRect.localToWorldMatrix.MultiplyPoint(new Vector3(0f, baseRect.rect.yMax + 64f + 13.5f)).y;
				}
				else
				{
					RectTransform baseRect2 = reverse ? rectTarget : rectSelf;
					jointY = baseRect2.localToWorldMatrix.MultiplyPoint(new Vector3(0f, baseRect2.rect.yMin - 64f)).y;
				}
				temp.Add(new Vector2(pointStart.x, jointY));
				temp.Add(new Vector2(pointEnd.x, jointY));
			}
			temp.Add(pointEnd);
			bool flag3 = reverse;
			if (flag3)
			{
				temp.Reverse();
			}
			lineWork.Vertices = temp.Select(delegate(Vector2 global)
			{
				Vector3 local = lineFromWorldMatrix.MultiplyPoint(global);
				return new Vector2(local.x, local.y);
			}).ToArray<Vector2>();
			Color colorDefault = "484848".HexStringToColor();
			RawImage lineGraphic = lineWork.GetComponent<RawImage>();
			GenealogyCharacterPanel.RelationType type = link.Type;
			if (!true)
			{
			}
			Color color2;
			switch (type)
			{
			case GenealogyCharacterPanel.RelationType.Blood:
				color2 = "9d4140".HexStringToColor();
				break;
			case GenealogyCharacterPanel.RelationType.Step:
				color2 = "7ba2bb".HexStringToColor();
				break;
			case GenealogyCharacterPanel.RelationType.Adoptive:
				color2 = "8b8049".HexStringToColor();
				break;
			case GenealogyCharacterPanel.RelationType.Spouse:
				color2 = "ae6138".HexStringToColor();
				break;
			default:
				color2 = Color.white;
				break;
			}
			if (!true)
			{
			}
			Color color = color2;
			bool dashed = link.BrokenOff;
			bool flag4 = isShadow;
			if (flag4)
			{
				lineWork.Dashed.Enabled = false;
				lineWork.RoundedCorners.Enabled = false;
				lineWork.Colored = new Line2DGenerator.ColoredStyle
				{
					StartColor = colorDefault,
					EndColor = colorDefault
				};
				lineGraphic.texture = this.spriteLineGeneral;
				bool flag5 = linkBaseRoot;
				if (flag5)
				{
					GameObject lineClone = Object.Instantiate<GameObject>(lineWork.gameObject, linkBaseRoot);
					RectTransform lineCloneRect = lineClone.GetComponent<RectTransform>();
					lineCloneRect.localScale = Vector3.one;
					lineCloneRect.position = lineWork.GetComponent<RectTransform>().position;
					lineCloneRect.GetComponent<CRawImage>().enabled = true;
					lineWork.gameObject.SetActive(false);
					lineClone.SetActive(true);
				}
			}
			else
			{
				lineWork.Dashed.Enabled = dashed;
				lineWork.RoundedCorners.Enabled = dashed;
				lineWork.Colored = new Line2DGenerator.ColoredStyle
				{
					StartColor = color,
					EndColor = color
				};
				lineGraphic.texture = this.spriteLineGeneral;
				Debug.Log(string.Format("test this:{0} idx:{1} linkTarget:{2}", this.TestName, idx, link.Target.TestName));
				bool flag6 = link.Target.NodeData.NodeToMain == this.NodeData;
				if (flag6)
				{
					link.Target._onFocused.AddListener(delegate(int depth)
					{
						lineGraphic.enabled = true;
					});
					link.Target._onFocusRestore.AddListener(delegate(int depth)
					{
						lineGraphic.enabled = false;
					});
				}
				bool flag7 = this.NodeData.NodeToMain == link.Target.NodeData;
				if (flag7)
				{
					this._onFocused.AddListener(delegate(int depth)
					{
						lineGraphic.enabled = true;
					});
					this._onFocusRestore.AddListener(delegate(int depth)
					{
						lineGraphic.enabled = false;
					});
				}
			}
			bool flag8 = !isShadow;
			if (flag8)
			{
				lineGraphic.enabled = false;
			}
			lineWork.transform.localPosition = lineWork.transform.localPosition.SetZ(0f);
			lineWork.Width = (float)lineGraphic.texture.height;
			lineGraphic.SetAllDirty();
		});
		this._onFocusRestore.Invoke(0);
	}

	// Token: 0x06001EAA RID: 7850 RVA: 0x000DD458 File Offset: 0x000DB658
	private void TryAddSubFocus(GenealogyCharacterPanel genealogyCharacterPanel)
	{
		throw new NotImplementedException();
	}

	// Token: 0x04001736 RID: 5942
	[SerializeField]
	private Sprite[] spriteIcons;

	// Token: 0x04001737 RID: 5943
	[SerializeField]
	private Sprite[] spriteBacks;

	// Token: 0x04001738 RID: 5944
	[SerializeField]
	private Texture spriteLineGeneral;

	// Token: 0x04001739 RID: 5945
	[SerializeField]
	private TextMeshProUGUI titleLabel;

	// Token: 0x0400173A RID: 5946
	[SerializeField]
	private TextMeshProUGUI valueLabel;

	// Token: 0x0400173B RID: 5947
	[SerializeField]
	private Game.Components.Avatar.Avatar avatar;

	// Token: 0x0400173C RID: 5948
	[SerializeField]
	private CImage titleIcon;

	// Token: 0x0400173D RID: 5949
	[SerializeField]
	private CImage backGround;

	// Token: 0x0400173E RID: 5950
	[SerializeField]
	private CommonCharacterNameFrame nameFrame;

	// Token: 0x0400173F RID: 5951
	[SerializeField]
	private HSVStyleRoot hsvStyleRoot;

	// Token: 0x04001740 RID: 5952
	private readonly UnityEvent<int> _onFocused = new UnityEvent<int>();

	// Token: 0x04001741 RID: 5953
	private readonly UnityEvent<int> _onFocusRestore = new UnityEvent<int>();

	// Token: 0x04001742 RID: 5954
	public int CharacterId;

	// Token: 0x04001743 RID: 5955
	public GenealogyMaker.GenealogyNode NodeData;

	// Token: 0x02001445 RID: 5189
	internal enum RelationType
	{
		// Token: 0x0400A057 RID: 41047
		Unknown,
		// Token: 0x0400A058 RID: 41048
		Blood,
		// Token: 0x0400A059 RID: 41049
		Step,
		// Token: 0x0400A05A RID: 41050
		Adoptive,
		// Token: 0x0400A05B RID: 41051
		Spouse
	}

	// Token: 0x02001446 RID: 5190
	internal readonly struct RelationLinking
	{
		// Token: 0x0600CB53 RID: 52051 RVA: 0x00593820 File Offset: 0x00591A20
		internal RelationLinking(GenealogyCharacterPanel.RelationType type, GenealogyCharacterPanel target, bool brokenOff, bool aboveLine)
		{
			this.Type = type;
			this.Target = target;
			this.BrokenOff = brokenOff;
			this.AboveLine = aboveLine;
		}

		// Token: 0x0400A05C RID: 41052
		public readonly GenealogyCharacterPanel.RelationType Type;

		// Token: 0x0400A05D RID: 41053
		public readonly GenealogyCharacterPanel Target;

		// Token: 0x0400A05E RID: 41054
		public readonly bool BrokenOff;

		// Token: 0x0400A05F RID: 41055
		public readonly bool AboveLine;
	}
}
