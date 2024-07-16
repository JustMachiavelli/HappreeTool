using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace HappreeTool.Documents
{
    public static class ExcelUtils
    {
        /// <summary>
        /// 读取xlsx中的所有行数据，每行作为一个列表，组成一个大列表
        /// </summary>
        /// <param name="xlsxPath">xlsx路径</param>
        /// <param name="sheet">sheet名称</param>
        /// <returns>包含所有行数据的列表</returns>
        public static List<List<string?>> ReadAllRows(string xlsxPath, string sheet)
        {
            List<List<string?>> rows = new List<List<string?>>();

            using (FileStream fileStream = new FileStream(xlsxPath, FileMode.Open, FileAccess.Read))
            using (IWorkbook workbook = new XSSFWorkbook(fileStream))
            {
                ISheet worksheet = workbook.GetSheet(sheet);
                if (worksheet != null)
                {
                    for (int row = 0; row <= worksheet.LastRowNum; row++)
                    {
                        IRow currentRow = worksheet.GetRow(row);
                        if (currentRow == null) continue;
                        List<string?> rowData = new List<string?>();
                        for (int col = 0; col < currentRow.LastCellNum; col++)
                        {
                            ICell cell = currentRow.GetCell(col);
                            if (cell == null || string.IsNullOrWhiteSpace(cell.ToString()))
                            {
                                rowData.Add(null);
                            }else
                            {
                                rowData.Add(cell.ToString());
                            }
                        }
                        rows.Add(rowData);
                    }
                }
            }
            return rows;
        }

        /// <summary>
        /// 读取xlsx中的两列值组成字典
        /// </summary>
        /// <remarks>将前一列作key，后一列作value</remarks>
        /// <param name="xlsxPath">xlsx路径</param>
        /// <param name="sheet">sheet名称</param>
        /// <param name="keyCol">作为key的列序号</param>
        /// <param name="valueCol">作为value的列序号</param>
        /// <returns>字典</returns>
        public static Dictionary<string, string> ReadTwoColsAsDict(string xlsxPath, string sheet, int keyCol, int valueCol)
        {
            Dictionary<string, string> dict = new();

            using (FileStream fileStream = new FileStream(xlsxPath, FileMode.Open, FileAccess.Read))
            using (IWorkbook workbook = new XSSFWorkbook(fileStream))
            {
                ISheet worksheet = workbook.GetSheet(sheet);
                for (int row = 0; row <= worksheet.LastRowNum; row++)
                {
                    IRow currentRow = worksheet.GetRow(row);
                    if (currentRow == null) continue;
                    string? key = currentRow.GetCell(keyCol)?.ToString();
                    if (!string.IsNullOrWhiteSpace(key) && currentRow.GetCell(valueCol)?.ToString() is { } value)
                    {
                        dict[key] = value;
                    }
                }
            }
            return dict;
        }

        /// <summary>
        /// 读取xlsx中的一列值去重组成list
        /// </summary>
        /// <param name="xlsxPath">xlsx路径</param>
        /// <param name="sheet">sheet名称</param>
        /// <param name="col">目标列的序号</param>
        /// <returns>列表</returns>
        public static IEnumerable<string> ReadOneColAsList(string xlsxPath, string sheet, int col)
        {
            List<string> list = new();
            using (FileStream fileStream = new(xlsxPath, FileMode.Open, FileAccess.Read))
            using (IWorkbook workbook = new XSSFWorkbook(fileStream))
            {
                ISheet worksheet = workbook.GetSheet(sheet);

                for (int row = 0; row <= worksheet.LastRowNum; row++)
                {
                    IRow currentRow = worksheet.GetRow(row);
                    if (currentRow == null) continue;
                    string? cell = currentRow.GetCell(col)?.ToString();
                    if (!string.IsNullOrWhiteSpace(cell))
                    {
                        list.Add(cell);
                    }
                }
            }
            return list.Distinct();
        }

    }
}
