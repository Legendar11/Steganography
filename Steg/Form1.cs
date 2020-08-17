using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Steg
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            CryptSteganography cs = new CryptSteganography();

            if (comboBox1.SelectedIndex == 0)
                pictureBox2.Image = cs.EncodeSteg(new Bitmap(pictureBox1.Image), new MemoryStream(Encoding.ASCII.GetBytes(richTextBox1.Text)), Convert.ToInt32(comboBox2.Text));

            if (comboBox1.SelectedIndex == 1)
                pictureBox2.Image = cs.EncodeStegM(new Bitmap(pictureBox1.Image), new MemoryStream(Encoding.ASCII.GetBytes(richTextBox1.Text)), Convert.ToInt32(comboBox2.Text));

            button4.Visible = true;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            CryptSteganography cs = new CryptSteganography();
            richTextBox1.Text = cs.DecodeSteg(new Bitmap(pictureBox1.Image));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog opf = new OpenFileDialog();
            opf.InitialDirectory = System.Environment.CurrentDirectory;
            if (opf.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Load(opf.FileName);
                button1.Visible = false;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                pictureBox2.Image.Save(sfd.FileName + ".bmp");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = pictureBox2.Image = null;
            button1.Visible = true;
            button4.Visible = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
        }
    }
}
