using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Data;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        DataSet ds = new DataSet();
        DataTable? A, B, result;
        public Form1()
        {
            InitializeComponent();
        }

        private string openpath()
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "請選擇資料夾";
            string filepath = "";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                filepath = dialog.SelectedPath;
                return filepath;
            }
            return "error";
        }

        // A目的地
        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = openpath();
            //取得路徑內的所有檔案
            A = new DataTable();
            A.Columns.Add("filename");
            string Apath, Afiles;
            Apath = textBox1.Text;
            foreach (string file in Directory.GetFiles(Apath))
            {
                Afiles = Path.GetFileName(file);
                A.Rows.Add(Afiles);
            }
            ds.Tables.Add(A);
        }

        // B目的地
        private void button2_Click(object sender, EventArgs e)
        {
            textBox2.Text = openpath();
            //取得路徑內的所有檔案
            B = new DataTable();
            B.Columns.Add("filename");
            string Bpath, Bfiles;
            Bpath = textBox2.Text;
            foreach (string file in Directory.GetFiles(Bpath))
            {
                Bfiles = Path.GetFileName(file);
                B.Rows.Add(Bfiles);
            }
            ds.Tables.Add(B);
        }

        // DataTable資料互相比對
        private void button3_Click(object sender, EventArgs e)
        {
            DataTable Adt = ds.Tables[0];
            DataTable Bdt = ds.Tables[1];
            var list = Adt.AsEnumerable().Except(Bdt.AsEnumerable(), DataRowComparer.Default);
            result = new DataTable();
            if (list.Count() > 0)
            {
                result.Columns.Add("filename");
                foreach (DataRow row in list)
                {
                    result.Rows.Add(row.ItemArray);
                }
            }
            dataGridView1.DataSource = result;
        }

        //複製A到B
        private void button4_Click(object sender, EventArgs e)
        {
            string pathA = textBox1.Text;
            string pathB = textBox2.Text + "\\";
            result.Columns.Add("處理結果");
            for (int i = 0; i < result.Rows.Count; i++)
            {
                string target = pathA + "\\" + result.Rows[i][0].ToString();
                string newfile = pathB + "\\" + result.Rows[i][0].ToString();
                FileInfo fileInfo = new FileInfo(target);
                dataGridView1.Refresh();
                if (fileInfo.Exists)
                {
                    fileInfo.CopyTo(newfile);
                    result.Rows[i][1] = "success";
                }
                else
                {
                    result.Rows[i][1] = "error";
                }
                dataGridView1.DataSource = result;
            }
        }

        //結果另存
        private void button5_Click(object sender, EventArgs e)
        {
            //建立一個新的excel檔案
            IWorkbook wb = new XSSFWorkbook();
            ISheet sh = wb.CreateSheet();

            //設定儲存格內容
            IRow row;
            ICell cell;
            int resultrows = result.Rows.Count,
                i;
            string info;

            //excel抬頭
            row = sh.CreateRow(0);
            cell = row.CreateCell(0);
            cell.SetCellValue("filename");
            cell = row.CreateCell(1);
            cell.SetCellValue("處理結果");

            //result結果填入excel
            for (i = 0; i < resultrows; i++)
            {
                row = sh.CreateRow(i + 1);
                cell = row.CreateCell(0);
                info = result.Rows[i][0].ToString();
                cell.SetCellValue(info);

                cell = row.CreateCell(1);
                info = result.Rows[i][1].ToString();
                cell.SetCellValue(info);
            }

            //開stream寫入資料
            FileStream fileStream;
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.CreatePrompt = true;
            saveFileDialog.Filter = "xlsx files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string path = saveFileDialog.FileName;
                fileStream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite);
                try
                {
                    wb.Write(fileStream, true);
                    MessageBox.Show("已成功寫入，可以關閉視窗了");

                }
                catch
                {
                    MessageBox.Show("寫入資料有誤，請再試一次");
                }
                fileStream.Close();
            }
        }
    }
}