using System;

// Token: 0x020003BC RID: 956
public class AreaBroken : Refers
{
	// Token: 0x06003A0E RID: 14862 RVA: 0x001D92B0 File Offset: 0x001D74B0
	public void SetBroken(bool isBroken, int brokenLevel)
	{
		base.gameObject.SetActive(isBroken && brokenLevel > 0);
		bool activeSelf = base.gameObject.activeSelf;
		if (activeSelf)
		{
			for (int i = 1; i < base.transform.childCount; i++)
			{
				bool inLevel = brokenLevel > i;
				base.transform.GetChild(i).GetComponent<CImage>().SetSprite(inLevel ? "largemap_part_1_qingshi_1" : "largemap_part_1_qingshi_2", false, null);
			}
		}
	}

	// Token: 0x040029DD RID: 10717
	private const string InLevelSprite = "largemap_part_1_qingshi_1";

	// Token: 0x040029DE RID: 10718
	private const string OutLevelSprite = "largemap_part_1_qingshi_2";
}
