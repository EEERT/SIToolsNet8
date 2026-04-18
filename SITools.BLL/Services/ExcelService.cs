using ClosedXML.Excel;
using System.Data;

namespace SITools.BLL.Services
{
    /// <summary>
    /// Excel 导入/导出服务
    /// </summary>
    public class ExcelService
    {
        /// <summary>
        /// 从 Excel 文件导入数据到 DataTable（跳过第一行标题）
        /// </summary>
        public DataTable ImportFromExcel(string filePath)
        {
            var dt = new DataTable();
            using var workbook = new XLWorkbook(filePath);
            var worksheet = workbook.Worksheets.First();
            bool firstRow = true;

            foreach (var row in worksheet.RowsUsed())
            {
                if (firstRow)
                {
                    // 读取列名
                    foreach (var cell in row.CellsUsed())
                        dt.Columns.Add(cell.GetString());
                    firstRow = false;
                    continue;
                }

                var dataRow = dt.NewRow();
                int colIndex = 0;
                foreach (var cell in row.Cells(1, dt.Columns.Count))
                {
                    dataRow[colIndex++] = cell.GetString();
                }
                dt.Rows.Add(dataRow);
            }

            return dt;
        }

        /// <summary>
        /// 将 DataGridView 数据导出为 Excel 文件
        /// </summary>
        public void ExportToExcel(DataTable data, string filePath, string sheetName = "Sheet1")
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add(sheetName);

            // 写入列标题
            for (int col = 0; col < data.Columns.Count; col++)
            {
                var cell = worksheet.Cell(1, col + 1);
                cell.Value = data.Columns[col].Caption;
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.LightGray;
            }

            // 写入数据行
            for (int row = 0; row < data.Rows.Count; row++)
            {
                for (int col = 0; col < data.Columns.Count; col++)
                {
                    var val = data.Rows[row][col];
                    worksheet.Cell(row + 2, col + 1).Value = val == DBNull.Value ? "" : val.ToString();
                }
            }

            worksheet.Columns().AdjustToContents();
            workbook.SaveAs(filePath);
        }
    }
}
