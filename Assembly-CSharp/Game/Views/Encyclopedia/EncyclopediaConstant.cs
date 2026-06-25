using System;
using System.IO;
using UnityEngine;

namespace Game.Views.Encyclopedia
{
	// Token: 0x02000A4F RID: 2639
	public static class EncyclopediaConstant
	{
		// Token: 0x0600826A RID: 33386 RVA: 0x003CCA40 File Offset: 0x003CAC40
		public static string GetEncyclopediaDataFolderPath()
		{
			return Path.Combine(Application.dataPath, "Dlc/" + EncyclopediaConstant.EncyclopediaName);
		}

		// Token: 0x0600826B RID: 33387 RVA: 0x003CCA6C File Offset: 0x003CAC6C
		public static string GetEncyclopediaDataPath()
		{
			string path = EncyclopediaConstant.GetEncyclopediaDataFolderPath();
			return Path.Combine(path, "EncyclopediaData.txt");
		}

		// Token: 0x040063B2 RID: 25522
		public static string EncyclopediaName = "Encyclopedia";

		// Token: 0x040063B3 RID: 25523
		public static string TextName = "Text";

		// Token: 0x040063B4 RID: 25524
		public static string IconName = "Icon";

		// Token: 0x040063B5 RID: 25525
		public static string SubTitleName = "SubTitle";

		// Token: 0x040063B6 RID: 25526
		public static string SubTitleIdName = "SubTitleId";

		// Token: 0x040063B7 RID: 25527
		public static string TitleName = "Title";

		// Token: 0x040063B8 RID: 25528
		public static string ImageName = "Image";

		// Token: 0x040063B9 RID: 25529
		public static string LinkId = "LinkId";

		// Token: 0x040063BA RID: 25530
		public static string LinkIdName = "LinkIdName";

		// Token: 0x040063BB RID: 25531
		public static string LinkTypeName = "LinkType";

		// Token: 0x040063BC RID: 25532
		public static string TableName = "Table";

		// Token: 0x040063BD RID: 25533
		public static string TableCellsName = "TableCells";

		// Token: 0x040063BE RID: 25534
		public static string TableRowName = "TableRow";

		// Token: 0x040063BF RID: 25535
		public static string TableColName = "TableCol";
	}
}
