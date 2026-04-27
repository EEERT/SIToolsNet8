using ClosedXML.Excel;
using System.Data;

namespace SITools.BLL.Services
{
    /// <summary>
    /// Excel 导入/导出服务。
    /// 使用第三方库 ClosedXML 操作 Excel 文件（.xlsx/.xls/.xlsm 格式）。
    /// ClosedXML 是一个开源的 .NET Excel 操作库，无需安装 Microsoft Office 即可读写 Excel 文件。
    ///
    /// 主要功能：
    ///   1. ImportFromExcel：从 Excel 文件读取数据，返回 DataTable（数据表格对象）
    ///   2. ExportToExcel：将 DataTable 中的数据写入 Excel 文件并保存
    /// </summary>
    public class ExcelService
    {
        /// <summary>
        /// 从 Excel 文件导入数据到 DataTable。
        ///
        /// 工作流程：
        ///   1. 打开 Excel 工作簿（Workbook）
        ///   2. 取第一个工作表（Worksheet）
        ///   3. 读取第一行作为列标题（DataTable 的列名）
        ///   4. 读取后续每一行作为数据行，填入 DataTable
        ///   5. 返回包含所有数据的 DataTable
        ///
        /// DataTable 是 .NET 中表示二维表格数据的标准类，类似于内存中的数据库表。
        /// </summary>
        /// <param name="filePath">Excel 文件的完整路径</param>
        /// <returns>包含 Excel 数据的 DataTable（第一行为列名，其余为数据）</returns>
        public DataTable ImportFromExcel(string filePath)
        {
            var dt = new DataTable();  // 创建空的 DataTable

            // using 语句确保工作簿对象在使用完毕后自动释放文件资源（关闭文件）
            using var workbook = new XLWorkbook(filePath);
            var worksheet = workbook.Worksheets.First();  // 取第一个工作表
            bool firstRow = true;  // 标记是否在处理第一行（标题行）

            // 遍历工作表中所有非空行
            foreach (var row in worksheet.RowsUsed())
            {
                if (firstRow)
                {
                    // 第一行：将每个单元格的内容作为 DataTable 的列名
                    foreach (var cell in row.CellsUsed())
                        dt.Columns.Add(cell.GetString());
                    firstRow = false;
                    continue;  // 跳过第一行，进入下一行处理
                }

                // 后续行：创建新数据行，按列顺序填入单元格内容
                var dataRow = dt.NewRow();
                int colIndex = 0;
                // row.Cells(1, dt.Columns.Count) 取第1列到最后一列的所有单元格（含空格）
                foreach (var cell in row.Cells(1, dt.Columns.Count))
                {
                    dataRow[colIndex++] = cell.GetString();
                }
                dt.Rows.Add(dataRow);
            }

            return dt;
        }

        /// <summary>
        /// 将 DataTable 的数据导出为 Excel 文件（.xlsx 格式）。
        ///
        /// 工作流程：
        ///   1. 创建新的 Excel 工作簿，添加一个名为 sheetName 的工作表
        ///   2. 写入标题行（列名加粗、灰色背景，用于区分标题与数据）
        ///   3. 逐行写入数据（从第2行开始）
        ///   4. 自动调整所有列宽为内容最适宽度
        ///   5. 保存文件到指定路径
        /// </summary>
        /// <param name="data">要导出的数据（DataTable）</param>
        /// <param name="filePath">保存的 Excel 文件完整路径（含文件名）</param>
        /// <param name="sheetName">工作表名称，默认为 "Sheet1"</param>
        public void ExportToExcel(DataTable data, string filePath, string sheetName = "Sheet1")
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add(sheetName);

            // 第一行：写入列标题（Bold加粗 + 灰色背景，视觉上与数据区分）
            for (int col = 0; col < data.Columns.Count; col++)
            {
                var cell = worksheet.Cell(1, col + 1);   // Excel 行列索引从1开始
                cell.Value = data.Columns[col].Caption;   // 列名（Caption与ColumnName相同）
                cell.Style.Font.Bold = true;               // 标题加粗
                cell.Style.Fill.BackgroundColor = XLColor.LightGray;  // 标题灰色背景
            }

            // 从第2行开始写入数据行
            for (int row = 0; row < data.Rows.Count; row++)
            {
                for (int col = 0; col < data.Columns.Count; col++)
                {
                    var val = data.Rows[row][col];
                    // DBNull.Value 是数据库空值，需要转换为空字符串（Excel不支持DBNull）
                    worksheet.Cell(row + 2, col + 1).Value = val == DBNull.Value ? "" : val.ToString();
                }
            }

            // 自动调整所有列的宽度，使内容完整显示
            worksheet.Columns().AdjustToContents();

            // 保存工作簿到指定文件路径
            workbook.SaveAs(filePath);
        }
    }
}
