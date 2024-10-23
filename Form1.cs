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
            dialog.Description = "�п�ܸ�Ƨ�";
            string filepath = "";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                filepath = dialog.SelectedPath;
                return filepath;
            }
            return "error";
        }

        // A�ت��a
        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = openpath();
            //���o���|�����Ҧ��ɮ�
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

        // B�ت��a
        private void button2_Click(object sender, EventArgs e)
        {
            textBox2.Text = openpath();
            //���o���|�����Ҧ��ɮ�
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

        // DataTable��Ƥ��ۤ��
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

        //�ƻsA��B
        private void button4_Click(object sender, EventArgs e)
        {
            string pathA = textBox1.Text;
            string pathB = textBox2.Text + "\\";
            result.Columns.Add("�B�z���G");
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

        //���G�t�s
        private void button5_Click(object sender, EventArgs e)
        {
            //�إߤ@�ӷs��excel�ɮ�
            IWorkbook wb = new XSSFWorkbook();
            ISheet sh = wb.CreateSheet();

            //�]�w�x�s�椺�e
            IRow row;
            ICell cell;
            int resultrows = result.Rows.Count,
                i;
            string info;

            //excel���Y
            row = sh.CreateRow(0);
            cell = row.CreateCell(0);
            cell.SetCellValue("filename");
            cell = row.CreateCell(1);
            cell.SetCellValue("�B�z���G");

            //result���G��Jexcel
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

            //�}stream�g�J���
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
                    MessageBox.Show("�w���\�g�J�A�i�H���������F");

                }
                catch
                {
                    MessageBox.Show("�g�J��Ʀ��~�A�ЦA�դ@��");
                }
                fileStream.Close();
            }
        }
    }
}