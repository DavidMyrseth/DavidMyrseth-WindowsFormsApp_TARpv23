using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static WindowsFormsApp_TARpv23.AndmebassWSDataSet;

namespace Andmebass_TARpv23
{
    public partial class Form1 : Form
    {
        SqlConnection conn = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=AndmebassWQW;Integrated Security=True");
        SqlCommand cmd;
        SqlDataAdapter adapter;
        OpenFileDialog open;
        SaveFileDialog save;
        string extension;

        //Create database AndmebassWS
        //USE AndmebassWS
        //CREATE TABLE Toode(
        //ID INT PRIMARY KEY IDENTITY(1,1),
        //Nimetus varchar(50),
        //Kogus int,
        //Hind decimal (3))

        //Create database AndmebassWQW
        //USE AndmebassWQW
        //CREATE TABLE Products(
        //ProductID INT PRIMARY KEY IDENTITY(1,1),
        //ProductName varchar(50),
        //ProductAmount int,
        //ProductPrice decimal (3),
        //ProductPicture VARBINARY(MAX));

        public Form1()
        {
            InitializeComponent();
            NaitaAndmed();
        }
        public void NaitaAndmed()
        {
            conn.Open();
            DataTable dt = new DataTable();
            cmd = new SqlCommand("SELECT * FROM Toode", conn);
            adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dt);
            dataGridView1.DataSource = dt;
            conn.Close();
        }

        private void Lisa_btn_Click(object sender, EventArgs e)
        {
            conn.Open();
            cmd = new SqlCommand("Insert into Toode(Nimetus, Kogus, Hind, Pilt) Values (@toode,@kogus,@hind,@pilt)", conn);
            cmd.Parameters.AddWithValue("@toode", Nimetus_txt.Text);
            cmd.Parameters.AddWithValue("@kogus", Kogus_txt.Text);
            cmd.Parameters.AddWithValue("@hind", Hind_txt.Text);
            cmd.Parameters.AddWithValue("@pilt", Nimetus_txt.Text + extension);
            cmd.ExecuteNonQuery();

            OpenFileDialog.Filter = "Bitmap Image (*.bmp)|*.bmp|Metafile Image (*.mwf)|*.mwf| Icon(*.ico)|*.ico| JPEG Image(*.jpg)|*.jpeg|(*.jpg)|*.jpeg|"

            if (OpenFileDialog.ShowDialog() == DialogResult.OK)
                byte[] imageBytes = File.ReadAllBytes(OpenFileDialog.FileName);
                cmd.Parameters.AddWithValue("@Picture", imageBytes);
                cmd.ExecuteNonQuery();

            Connection.Close();
            productsTableAdapter.Fill(productsDataSet.Products)


        private void Kustuta_btn_Click(object sender, EventArgs e)
        {
            try
            {
                ID = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["Id"].Value);
                if (ID != 0)
                {
                    conn.Open();
                    cmd = new SqlCommand("DELETE FROM Toode WHERE Id=@id", conn);
                    cmd.Parameters.AddWithValue("@id", ID);
                    cmd.ExecuteNonQuery();

                    conn.Close();
                    Emaldamine();
                    NaitaAndmed();

                    MessageBox.Show("Kirje kustutatud");
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Viga kustutamisel");
            }
        }

        private void Uuenda_btn_Click(object sender, EventArgs e)
        {
            if (Nimetus_txt.Text.Trim() != string.Empty && Kogus_txt.Text.Trim() != string.Empty && Hind_txt.Text.Trim() != string.Empty)
            {
                try
                {
                    conn.Open();
                    cmd = new SqlCommand("Update Toode SET Nimetus=@toode, Kogus=@kogus, Hind=@hind WHERE Id=@id", conn);
                    cmd.Parameters.AddWithValue("@id", ID);
                    cmd.Parameters.AddWithValue("@toode", Nimetus_txt.Text);
                    cmd.Parameters.AddWithValue("@kogus", Kogus_txt.Text);
                    cmd.Parameters.AddWithValue("@hind", Hind_txt.Text);
                    cmd.Parameters.AddWithValue("@pilt", Nimetus_txt.Text + extension);
                    cmd.ExecuteNonQuery();

                    conn.Close();
                    NaitaAndmed();
                    Emaldamine();
                }
                catch (Exception)
                {
                    MessageBox.Show("Andmebaasiga viga");
                }
            }
            else
            {
                MessageBox.Show("Sisesta andmeid");
            }
        }

        private void Emaldamine()
        {
            MessageBox.Show("Andmed elukalt uuendatud", "Uuendamine");
            Nimetus_txt.Text = "";
            Kogus_txt.Text = "";
            Hind_txt.Text = "";
            pictureBox1.Image = Image.FromFile(Path.Combine(Path.GetFullPath(@"..\..\Pildid"), "pilt.jpg"));
        }

        int ID = 0;
        private void dataGridView1_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            ID = (int)dataGridView1.Rows[e.RowIndex].Cells["Id"].Value;
            Nimetus_txt.Text = dataGridView1.Rows[e.RowIndex].Cells["Nimetus"].Value.ToString();
            Kogus_txt.Text = dataGridView1.Rows[e.RowIndex].Cells["Kogus"].Value.ToString();
            Hind_txt.Text = dataGridView1.Rows[e.RowIndex].Cells["Hind"].Value.ToString();
            try
            {
                pictureBox1.Image = Image.FromFile(Path.Combine(Path.GetFullPath(@"..\..\Pildid"),
                    dataGridView1.Rows[e.RowIndex].Cells["Pilt"].Value.ToString()));
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            }
            catch (Exception)
            {
                pictureBox1.Image = Image.FromFile(Path.Combine(Path.GetFullPath(@"..\..\Pildid"), "pilt.jpg"));
            }
        }

        private void Pildi_otsing_btn_Click(object sender, EventArgs e)
        {
            open = new OpenFileDialog();
            open.InitialDirectory = @"C:\Users\opilane\Pictures\";
            open.Multiselect = false;
            open.Filter = "Images Files(*.jpeg;*.png;*.bmp;*.jpg)|*.jpeg;*.png;*.bmp;*.jpg";
            FileInfo openfile = new FileInfo(@"C:\Users\opilane\Pictures\" + open.FileName);
            if (open.ShowDialog() == DialogResult.OK && Nimetus_txt.Text != null)
            {
                save = new SaveFileDialog();
                save.InitialDirectory = Path.GetFullPath(@"..\..\Pildid");
                extension = Path.GetExtension(open.FileName);
                save.FileName = Nimetus_txt.Text + extension;
                save.Filter = "Images" + Path.GetExtension(open.FileName) + "|" + Path.GetExtension(open.FileName);
                if (save.ShowDialog() == DialogResult.OK && Nimetus_txt != null)
                {
                    File.Copy(open.FileName, save.FileName);
                    pictureBox1.Image = Image.FromFile(save.FileName);
                }
            }
            else
            {
                MessageBox.Show("Puudub toode nimetus või ole Cancel vajutatud");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'andmebassWSDataSet.AndmebassW' table. You can move, or remove it, as needed.
            this.andmebassWTableAdapter.Fill(this.andmebassWSDataSet.AndmebassW);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string filePath = Path.Combine(Path.GetFullPath(@"..\..\Pildid")), file+extension);
                MessageBox.Show($"Püüan kustutada faili: {filePath}");
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    MessageBox.Show("Fail on kustutatud");
                }
                else
                {
                    MessageBox.Show("Fail ei leitud");
                }
                catch (Exception) 
            {
                MessageBox.Show("Failiga probleemid");
            }
        }
    }
}
