using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace INFINPIC
{
    public partial class Form3 : Form
    {
        Bitmap image1 = null;
       
        Koch3 koch;

        public Form3()
        {
            InitializeComponent();
            image1 = new Bitmap(Properties.Resources.image);
            pictureBox1.Image = resizeImage(image1, this.pictureBox1.Size);
            pictureBox1.Invalidate();
            koch = new Koch3(this);
        }

        private static Image resizeImage(Image imgToResize, Size size)
        {
            int sourceWidth = imgToResize.Width;
            int sourceHeight = imgToResize.Height;
            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;
            nPercentW = ((float)size.Width / (float)sourceWidth);
            nPercentH = ((float)size.Height / (float)sourceHeight);
            if (nPercentH < nPercentW)
                nPercent = nPercentH;
            else
                nPercent = nPercentW;
            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);
            Bitmap b = new Bitmap(destWidth, destHeight);
            Graphics g = Graphics.FromImage((Image)b);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
            g.Dispose();
            return (Image)b;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            
            OpenFileDialog load = new OpenFileDialog();
            load.Multiselect = false;
            load.Filter = "Image Files(*.bmp;*.png)|*.bmp;*.png|All files (*.*)|*.*";
            if (load.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    String filename = load.FileName;
                    String ex = filename.Substring(filename.LastIndexOf(".") + 1);
                    if (String.Compare(ex, "bmp") != 0 && String.Compare(ex, "png") != 0 && String.Compare(ex, "jpg") != 0
                        && String.Compare(ex, "Bmp") != 0 && String.Compare(ex, "Png") != 0) 
                    {
                        throw new Exception("Неподдерживемый формат");
                    }
                    image1 = new Bitmap(load.FileName);
                    pictureBox1.Image = resizeImage(image1, this.pictureBox1.Size);
                    int imageX = pictureBox1.Image.Width;
                    int imageY = pictureBox1.Image.Height;
                    int panelX = panel1.Width;
                    int panelY = panel1.Height;
                    int dX = (panelX - imageX) / 2;
                    int dY = (panelY - imageY) / 2;
                    pictureBox1.Left = dX;
                    pictureBox1.Top = dY;
                    pictureBox1.Size = new Size(imageX, imageY);
                    pictureBox1.Invalidate();
                    Stream inputStream = File.OpenRead(filename);
                    long size = inputStream.Length;
                    inputStream.Close();

                 
                    koch.infilename = filename;
                    koch.image1 = new Bitmap(image1);
                    //label2.Text = "Исходное изображение: " + filename;
                    update();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка открытия файла\n" + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    
                }
            }
        }

       
        private void getKey()
        {
            if (this.textBox2.Text.Length != 0)
            {
                koch.mKey = Convert.ToInt32(textBox2.Text);
            }
        }

       

        private void button1_Click(object sender, EventArgs e)
        {
            textBox2.Text = Convert.ToString(koch.hideInf.Length);
            Thread InstanceCaller = new Thread(new ThreadStart(DoWork1));
            InstanceCaller.Start();
        }

        private void DoWork1()
        {
            koch.inlining();
        }

        

        public void sendMess(String mess)
        {
            this.richTextBox1.Text += mess + "\n";
            this.richTextBox1.SelectionStart = richTextBox1.Text.Length;
            this.richTextBox1.ScrollToCaret();
        }
        
        private void button2_Click(object sender, EventArgs e)
        {
            String[] ss = new String[0]; // Пустой массив
            this.richTextBox1.Lines = ss;
            Thread InstanceCaller = new Thread(new ThreadStart(DoWork2));
            InstanceCaller.Start();
        }

        private void DoWork2()
        {
            koch.extraction();
        }

        public void blockPanel()
        {
            this.panel3.Enabled = false;
        }

        public void unblockPanel()
        {
            this.panel3.Enabled = true;
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar >= 47 && e.KeyChar <= 58)
            {
                textBox2.Text += e.KeyChar;
                e.Handled = true;
            }
            else if (e.KeyChar == 8 && textBox2.Text.Length > 0)
            {
                textBox2.Text = textBox2.Text.Substring(0, textBox2.Text.Length - 1);
                if (textBox2.Text.Length > 0)
                {
                    int m = Convert.ToInt32(textBox2.Text);
                    koch.mKey = m;
                }
                else
                {
                    koch.mKey = 0;
                }
                e.Handled = true;
            }
            else
            {
                e.Handled = true;
            }
         
            textBox2.Select(textBox2.Text.Length, textBox2.Text.Length);
            update();
        }



        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form ifrm = new Form2();
            ifrm.Show(); // отображаем Form2
            this.Dispose(); // скрываем Form3 (this - текущая форма)
        }

        private void update()
        {
           
            
            getKey();
            if (koch.sizeSegment != 0 && koch.image1 != null && koch.mKey != 0)
            {
                butExtr.Enabled = true;
            }
            else
            {
                butExtr.Enabled = false;
            }

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            string selectedEmployee = (string)comboBox.SelectedItem;
            if (selectedEmployee.Equals("2x2"))
            {
                koch.sizeSegment = 2;
            }
            if (selectedEmployee.Equals("4x4"))
            {
                koch.sizeSegment = 4;
            }
            if (selectedEmployee.Equals("8x8"))
            {
                koch.sizeSegment = 8;
            }
            update();
        }
    }
}
