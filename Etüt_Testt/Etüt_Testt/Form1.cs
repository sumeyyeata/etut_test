using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;

namespace Etüt_Testt
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        SqlConnection baglanti = new SqlConnection(@"Data Source=DESKTOP-5FU8Q6M;Initial Catalog=TestEtutt;Integrated Security=True");

        void derslistesi()
        {
            SqlDataAdapter da = new SqlDataAdapter("Select * From tbldersler", baglanti);
            DataTable dt = new DataTable();
            da.Fill(dt);
            CmbDers.ValueMember = "DERSID";
            CmbDers.DisplayMember = "DERSAD";
            CmbDers.DataSource = dt;

        }

        //Etüt Listesi

        void etutlistesi()
        {
            SqlDataAdapter da3 = new SqlDataAdapter("execute etut", baglanti);
            DataTable dt3 = new DataTable();
            da3.Fill(dt3);
            dataGridView1.DataSource = dt3;

            //duruma göre datagrid satır renklendirme 
            for(int i=0; i< dt3.Rows.Count; i++)
            {
                bool durum = Convert.ToBoolean(dt3.Rows[i]["DURUM"]);
                if (durum == true)
                {
                    dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.Red;
                }
                else
                {
                    dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.Yellow;
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            derslistesi();
            etutlistesi();
            dersListesi2();
        }

        private void CmbDers_SelectedIndexChanged(object sender, EventArgs e)
        {
            SqlDataAdapter da2 = new SqlDataAdapter("Select *from tblogretmen where bransıd=" + CmbDers.SelectedValue, baglanti);
            DataTable dt2 = new DataTable();
            da2.Fill(dt2);
            CmbOgretmen.ValueMember = "OGRTID";
            CmbOgretmen.DisplayMember = "AD";
            CmbOgretmen.DataSource = dt2;

        }

        private void BtnEtutOlustur_Click(object sender, EventArgs e)
        {
            baglanti.Open();
            SqlCommand komut = new SqlCommand("insert into tbletut (dersıd,ogretmenıd,tarıh,saat) values (@p1,@p2,@p3,@p4)", baglanti);
            komut.Parameters.AddWithValue("@p1", CmbDers.SelectedValue);
            komut.Parameters.AddWithValue("@p2", CmbOgretmen.SelectedValue);
            komut.Parameters.AddWithValue("@p3", MskTarih.Text);
            komut.Parameters.AddWithValue("@p4", MskSaat.Text);
            komut.ExecuteNonQuery();

            baglanti.Close();
            MessageBox.Show("Etüt Oluşturuldu", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int secilen = dataGridView1.SelectedCells[0].RowIndex;

            TxtEtutId.Text = dataGridView1.Rows[secilen].Cells[0].Value.ToString();

        }

        private void BtnEtutVer_Click(object sender, EventArgs e)
        {
            baglanti.Open();
            SqlCommand komut = new SqlCommand("Update tbletut set ogrencııd=@p1, durum=@p2 where ıd=@p3", baglanti);
            komut.Parameters.AddWithValue("@p1", TxtOgrenciId.Text);
            komut.Parameters.AddWithValue("@p2", "True");
            komut.Parameters.AddWithValue("@p3", TxtEtutId.Text);
            komut.ExecuteNonQuery();
            baglanti.Close();
            MessageBox.Show("Etüt Öğrenciye Verildi", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Warning); 
        }

        private void BtnFotoğraf_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            pictureBox1.ImageLocation = openFileDialog1.FileName;

        }

        private void BtnOgrenciEkle_Click(object sender, EventArgs e)
        {
            baglanti.Open();
            SqlCommand komut = new SqlCommand("insert into tblogrencı (ad,soyad,fotograf,sınıf,telefon,maıl) values (@p1,@p2,@p3,@p4,@p5,@p6)", baglanti);
            komut.Parameters.AddWithValue("@p1", TxtAd.Text);
            komut.Parameters.AddWithValue("@p2", TxtSoyad.Text);
            komut.Parameters.AddWithValue("@p3", pictureBox1.ImageLocation);
            komut.Parameters.AddWithValue("@p4", TxtSınıf.Text);
            komut.Parameters.AddWithValue("@p5", MskTelefon.Text);
            komut.Parameters.AddWithValue("@p6", TxtMail.Text);
            komut.ExecuteNonQuery();
            baglanti.Close();
            MessageBox.Show("Öğrenci Sisteme Kaydedildi", "Bilgi", MessageBoxButtons.OK,MessageBoxIcon.Information);


        }
        bool durum;

        void dersListesi2()
        {
            //öğretmen ekle kısmındaki dersler için

            SqlDataAdapter da5 = new SqlDataAdapter("Select * From TBLDERSLER", baglanti);
            DataTable dt5 = new DataTable();
            da5.Fill(dt5);
            CmbDersAdı.ValueMember = "DERSID";
            CmbDersAdı.DisplayMember = "DERSAD";
            CmbDersAdı.DataSource = dt5;
        }
        void MukerrerKontrol()
        {
            baglanti.Open();
            SqlCommand komut = new SqlCommand("Select * from TBLDERSLER where DERSAD=@p1", baglanti);
            komut.Parameters.AddWithValue("@p1", TxtDers.Text);
            SqlDataReader dr = komut.ExecuteReader();
            if (dr.Read())
            {
                durum = false; //bu kayıt zaten var
            }
            else
            {
                durum = true; //kayıt yok, eklenebilir
            }

            baglanti.Close();
        }

       
        private void BtnDersEkle_Click(object sender, EventArgs e)
        {
            MukerrerKontrol();
            if (durum == true)
            {
                baglanti.Open();
                SqlCommand komut = new SqlCommand("Insert into TBLDERSLER (DERSAD) values (@p1)", baglanti);
                komut.Parameters.AddWithValue("@p1", TxtDers.Text);
                komut.ExecuteNonQuery();
                baglanti.Close();
                MessageBox.Show("Ders sisteme eklendi");
                

            }

            else
            {
                MessageBox.Show(TxtDers.Text + "isimli ders sistemde zaten var"); 
            }
        }
        bool ogretmenkontrol;
        void kontrolOgretmen()
        {
            baglanti.Open();
            SqlCommand cmd3 = new SqlCommand("select * FROM TBLOGRETMEN WHERE AD=@o1 AND SOYAD=@o2", baglanti);
            cmd3.Parameters.AddWithValue("@o1", TxtOgretmenAd.Text);
            cmd3.Parameters.AddWithValue("@o2", TxtOgretmenSoyad.Text);
            SqlDataReader dr = cmd3.ExecuteReader();
            if (dr.Read())
            {
                ogretmenkontrol = false;
            }
            else
            {
                ogretmenkontrol = true;
            }
            baglanti.Close();
        }
        private void BtnOgretmenKaydet_Click(object sender, EventArgs e) 
        { 
            kontrolOgretmen();
            if (ogretmenkontrol == true)
            {
                baglanti.Open();
                SqlCommand ogrtekle = new SqlCommand("INSERT INTO TBLOGRETMEN (AD,SOYAD,BRANSID) VALUES (@a1,@a2,@a3)", baglanti);
                ogrtekle.Parameters.AddWithValue("@a1", TxtOgretmenAd.Text);
                ogrtekle.Parameters.AddWithValue("@a2", TxtOgretmenSoyad.Text);
                ogrtekle.Parameters.AddWithValue("@a3", CmbDersAdı.SelectedValue);
                ogrtekle.ExecuteNonQuery();
                baglanti.Close();
                MessageBox.Show("Öğretmen Kaydedildi");
            }
            else
            {
                MessageBox.Show("Bu Öğretmen Sistemde Mevcut.Lütfen Bilgileri Kontrol Ediniz", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void CmbDersAdı_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
