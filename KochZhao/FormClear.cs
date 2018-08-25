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
    public partial class FormClear : Form
    {
        Bitmap image1 = null;
        Bitmap image2 = null;
        KochClear koch;

        public FormClear()
        {
            InitializeComponent();
            image1 = new Bitmap("Res//image.png");
            pictureBox1.Image = resizeImage(image1, this.pictureBox1.Size);
            pictureBox1.Invalidate();
            koch = new KochClear(this);
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
            
            try
            {
                String filename = "/BIN.txt";
                /*string text = "1010hsadh110111adhg10001101hdshs00";
                using (StreamWriter sw = new StreamWriter(filename, false, System.Text.Encoding.Default))
                {
                    sw.WriteLine(text);
                }*/

                koch.datafile = filename;
                FileStream fs = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.None);
                fs.Close();
                koch.loadMessage(filename);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Размер сообщения: файл не найден");
                label8.Text = "Размер сообщения: файл не найден";
                koch.datafile = null;
                koch.hideInf = null;
                //butInlining.Enabled = false;
            }
            update();


            OpenFileDialog load = new OpenFileDialog();
            load.Multiselect = false;
            load.Filter = "Image Files(*.bmp;*.png)|*.bmp;*.png|All files (*.*)|*.*";
            if (load.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    String filename = load.FileName;
                    String ex = filename.Substring(filename.LastIndexOf(".") + 1);
                    if (String.Compare(ex, "bmp") != 0 && String.Compare(ex, "png") != 0)
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

                    label7.Text = "Размер контейнера: " + image1.Width + "x" + image1.Height + " (" + size + " байт)";
                    koch.infilename = filename;
                    koch.image1 = new Bitmap(image1);
                    label2.Text = "Исходное изображение: " + filename;
                    update();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка открытия файла\n" + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    pictureBox2.Image = null;
                    pictureBox2.Size = panel2.Size;
                    pictureBox2.Top = 0;
                    pictureBox2.Left = 0;
                }
            }
        }

        private void update()
        {
            if (progressBar1.Value > 99)
            {
                progressBar1.Value = 0;
            }
            if (koch.sizeSegment != 0 && koch.image1 != null)
            {
                int count = (int)(koch.image1.Height / koch.sizeSegment) * (int)(koch.image1.Width / koch.sizeSegment);
                label9.Text = "Количество блоков: " + count;
            }
            else
            {
                label9.Text = "Количество блоков: ";
            }
            if (koch.hideInf != null)
            {
                label8.Text = "Размер сообщения: " + koch.hideInf.Length + " байт";
            }
            else
            {
                label8.Text = "Размер сообщения: ";
            }
            maxDataSize();
            if (koch.sizeSegment != 0 && koch.image1 != null)
            {
                label10.Text = "Макс. объем встраиваемой инф.: " + koch.maxSize + " байт";
            }
            else
            {
                label10.Text = "Макс. объем встраиваемой инф.: ";
            }
            /*if (koch.sizeSegment != 0 && koch.image1 != null && koch.flag)
            {
                butInlining.Enabled = true;
            }
            else
            {
                butInlining.Enabled = false;
            }*/
            getKey();
            if (koch.sizeSegment != 0 && koch.image1 != null && koch.mKey != 0)
            {
                butExtr.Enabled = true;
            }
            else
            {
                butExtr.Enabled = false;
            }
            trackBar1.Value = koch.P;

            if (koch.hideInf == null)
            {
                textBox1.Text = "";
            }
            if (koch.image1 == null)
            {
                label7.Text = "Размер контейнера: ";
                label2.Text = "Исходное изображение: ";
            }
            if (image2 == null)
            {
                label1.Text = "После встраивания: ";
            }
        }

        private void getKey()
        {
            if (this.textBox2.Text.Length != 0)
            {
                koch.mKey = Convert.ToInt32(textBox2.Text);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            /*OpenFileDialog load = new OpenFileDialog();
            load.Multiselect = false;
            DialogResult result = load.ShowDialog();
            String filename = load.FileName;
            load.Dispose();
            if (result == DialogResult.OK)
            {
                try
                {
                    textBox1.Text = filename;
                    koch.datafile = filename;
                    koch.loadMessage(filename);
                    update();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка открытия файла\n" + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }*/
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //textBox2.Text = Convert.ToString(koch.hideInf.Length);
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

        public void pict2(Bitmap image)
        {
            this.image2 = image;
            this.pictureBox2.Size = this.panel2.Size;
            pictureBox2.Image = resizeImage(image2, this.pictureBox2.Size);
            int imageX = pictureBox2.Image.Width;
            int imageY = pictureBox2.Image.Height;
            int panelX = panel2.Width;
            int panelY = panel2.Height;
            int dX = (panelX - imageX) / 2;
            int dY = (panelY - imageY) / 2;
            pictureBox2.Left = dX;
            pictureBox2.Top = dY;
            pictureBox2.Size = new Size(imageX, imageY);
            pictureBox2.Invalidate();
        }

        private void button2_Click(object sender, EventArgs e)
        {
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

        public void progress(double val)
        {
            if (val > 100)
            {
                progressBar1.Value = 100;
            }
            else
            {
                progressBar1.Value = (int)val;
            }
        }

        public void maxDataSize()
        {
            koch.maxMessSize();
            if (koch.hideInf == null)
            {
                koch.flag = false;
                return;
            }
            if (koch.maxSize > 0 && koch.hideInf.Length > 0 && koch.hideInf.Length <= koch.maxSize)
            {
                koch.flag = true;
            }
            else koch.flag = false;
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            if (image1 != null)
            {
                image1.Dispose();
                image1 = null;
            }
            if (image2 != null)
            {
                image2.Dispose();
                image2 = null;
            }

            if (koch.image1 != null)
            {
                koch.image1.Dispose();
            }
            koch = new KochClear(this);
            pictureBox2.Image = null;
            pictureBox2.Size = panel2.Size;
            pictureBox2.Top = 0;
            pictureBox2.Left = 0;
            pictureBox1.Size = panel1.Size;
            pictureBox1.Top = 0;
            pictureBox1.Left = 0;
            image1 = new Bitmap("Res//image.png");
            pictureBox1.Image = resizeImage(image1, this.pictureBox1.Size);
            pictureBox1.Invalidate();

            this.textBox2.Text = "";
            this.label10.Text = "Макс. объем встраиваемой инф.:";
            this.label9.Text = "Количество блоков: ";
            this.label8.Text = "Размер сообщения: ";
            this.label7.Text = "Размер контейнера: ";
            this.textBox1.Text = "";
            this.label5.Text = "Размер блоков";
            this.label4.Text = "25";
            this.trackBar1.Value = 25;
            this.butExtr.Enabled = false;
            this.butInlining.Enabled = true;
            this.richTextBox1.Text = "";
            this.label1.Text = "После встраивания:";
            this.label2.Text = "Исходное изображение: ";
            this.progressBar1.Value = 0;
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
            update();
            textBox2.Select(textBox2.Text.Length, textBox2.Text.Length);
        }


        private void TrackBar1_ValueChanged(object sender, System.EventArgs e)
        {
            try
            {
                this.label4.Text = this.trackBar1.Value.ToString();
                koch.P = this.trackBar1.Value;
            }
            catch (Exception ex)
            {
                koch.P = 25;
                this.label4.Text = "25";
                this.trackBar1.Value = 25;
            }
        }

        private void textBox1_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                String filename = textBox1.Text;
                koch.datafile = filename;
                FileStream fs = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.None);
                fs.Close();
                koch.loadMessage(filename);
            }
            catch (Exception ex)
            {
                label8.Text = "Размер сообщения: файл не найден";
                koch.datafile = null;
                koch.hideInf = null;
                butInlining.Enabled = false;
            }
            update();
        }

        private void ComboBox1_SelectedIndexChanged(object sender, System.EventArgs e)
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

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form ifrm = new Form2();
            ifrm.Show(); // отображаем Form2
            this.Dispose(); // скрываем INFINPIC_Form11 (this - текущая форма)
        }
    }
}
