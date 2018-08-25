using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace INFINPIC
{
    public partial class Form4 : Form
    {
        Bitmap image1 = null;
        Bitmap image2 = null;
        Bitmap image3 = null;
        public Form4 form = null;

        public ImageFormat imFormat1 = null;
        public ImageFormat imFormat2 = null;

        public string filename1=null;
        public string filename2 = null;
        public Form4()
        {
            InitializeComponent();
            image1 = new Bitmap(Properties.Resources.image);
            pictureBox1.Image = resizeImage(image1, this.panel1.Size);
            pictureBox1.Invalidate();
            image2 = new Bitmap(Properties.Resources.image);
            pictureBox2.Image = resizeImage(image2, this.panel2.Size);
            pictureBox2.Invalidate();

            //pictureBox3.Invalidate();
            //pictureBox3.Image = resizeImage(image3, this.panel3.Size);
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
            pictureBox3.Image = null;

            OpenFileDialog load = new OpenFileDialog();
            load.Multiselect = false;
            load.Filter = "Image Files(*.bmp;*.png)|*.bmp;*.png|All files (*.*)|*.*";
            if (load.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    String filename = load.FileName;
                    filename1 = filename;
                    String ex = filename.Substring(filename.LastIndexOf(".") + 1);
                    if (String.Compare(ex, "bmp") != 0 && String.Compare(ex, "png") != 0 && String.Compare(ex, "Bmp") != 0 && String.Compare(ex, "Png") != 0)
                    {
                        throw new Exception("Неподдерживемый формат");
                    }
                    image1 = new Bitmap(load.FileName);
                    pictureBox1.Image = image1;//resizeImage(image1, this.panel1.Size)

                    if (ex.Equals("bmp"))
                    {
                        imFormat1 = ImageFormat.Bmp;
                    }
                    if (ex.Equals("png"))
                    {
                        imFormat1 = ImageFormat.Png;
                    }
                    if (ex.Equals("Bmp"))
                    {
                        imFormat1 = ImageFormat.Bmp;
                    }
                    if (ex.Equals("Png"))
                    {
                        imFormat1 = ImageFormat.Png;
                    }
                    /*int imageX = pictureBox1.Image.Width;
                    int imageY = pictureBox1.Image.Height;
                    int panelX = panel1.Width;
                    int panelY = panel1.Height;
                    int dX = (panelX - imageX) / 2;
                    int dY = (panelY - imageY) / 2;
                    pictureBox1.Left = dX;
                    pictureBox1.Top = dY;
                    pictureBox1.Size = new Size(imageX, imageY);*/

                    pictureBox1.Invalidate();
                    Stream inputStream = File.OpenRead(filename);
                    long size = inputStream.Length;
                    inputStream.Close();

                    
                    
                    //label1.Text = "Изображение №1: " + filename;
                    update();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка открытия файла\n" + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    form.Dispose();
                }
                /*finally
                {
                    pictureBox2.Image = null;
                    pictureBox2.Size = panel2.Size;
                    pictureBox2.Top = 0;
                    pictureBox2.Left = 0;
                }*/
            }
        }

        private void pictureBox2_Click_1(object sender, EventArgs e)
        {
            pictureBox3.Image = null;
           
            OpenFileDialog load = new OpenFileDialog();
            load.Multiselect = false;
            load.Filter = "Image Files(*.bmp;*.png)|*.bmp;*.png|All files (*.*)|*.*";
            if (load.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    String filename = load.FileName;
                    filename2 = filename;
                    String ex = filename.Substring(filename.LastIndexOf(".") + 1);
                    if (String.Compare(ex, "bmp") != 0 && String.Compare(ex, "png") != 0 && String.Compare(ex, "Bmp") != 0 && String.Compare(ex, "Png") != 0)
                    {
                        throw new Exception("Неподдерживемый формат");
                    }

                    if (ex.Equals("bmp"))
                    {
                        imFormat2 = ImageFormat.Bmp;
                    }
                    if (ex.Equals("png"))
                    {
                        imFormat2 = ImageFormat.Png;
                    }
                    if (ex.Equals("Bmp"))
                    {
                        imFormat2 = ImageFormat.Bmp;
                    }
                    if (ex.Equals("Png"))
                    {
                        imFormat2 = ImageFormat.Png;
                    }

                    image2 = new Bitmap(load.FileName);
                    pictureBox2.Image = image2;//resizeImage(image2, this.panel2.Size)
                   /* int imageX = pictureBox2.Image.Width;
                    int imageY = pictureBox2.Image.Height;
                    int panelX = panel2.Width;
                    int panelY = panel2.Height;
                    int dX = (panelX - imageX) / 2;
                    int dY = (panelY - imageY) / 2;
                    pictureBox2.Left = dX;
                    pictureBox2.Top = dY;
                    pictureBox2.Size = new Size(imageX, imageY);*/
                    pictureBox2.Invalidate();
                    Stream inputStream = File.OpenRead(filename);
                    long size = inputStream.Length;
                    inputStream.Close();


                   // label2.Text = "Изображение №2: " + filename;
                    update();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка открытия файла\n" + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                /*finally
                {
                    pictureBox2.Image = null;
                    pictureBox2.Size = panel2.Size;
                    pictureBox2.Top = 0;
                    pictureBox2.Left = 0;
                }*/
            }
        }

        private void update()
        {
            if (progressBar1.Value > 99)
            {
                progressBar1.Value = 0;
            }

            if (image1!=null && image2!=null)
            {
                butCompare.Enabled = true;
            }  
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Invoke(new Action(() =>
            {
                this.ControlBox = false;
                this.butCompare.Enabled = false;
                this.progressBar1.Value = 0;
                this.richTextBox1.Clear();

                //это окошко обратно в главный поток
                //код написанный тут выполнится в главном потоке не вызывая ошибок
            }));
            try
            {
                int width = image1.Width;
                int height = image1.Height;
                sendMess("Сравнение изображений началось...");
                sendMess("Результат процесса сравнения изображений будет показан на изображении №3.");
                Color black = Color.FromName("Black");
                Color white = Color.FromName("White");

                if (image1.Width == image2.Width &&
                    image1.Height == image2.Height)
                {
                    sendMess("Ширина у изображений равная.");

                    sendMess("Высота у изображений равная.");

                    this.Invoke(new Action(() =>
                    {
                        pictureBox3.Invalidate();
                        pictureBox3.Image = null;
                        //это окошко обратно в главный поток
                        //код написанный тут выполнится в главном потоке не вызывая ошибок
                    }));


                    Bitmap bitmap = new Bitmap(pictureBox1.Image.Width, pictureBox1.Image.Height);

                    int temp = (image1.Height * image1.Width) / 10;
                    // int percent = 0;


                    for (int i = 0; i < image1.Height; i++)
                    {
                        for (int j = 0; j < image1.Width; j++)
                        {

                            if ((i * j) >= temp)
                            {

                                this.Invoke(new Action(() =>
                                {
                                    progressBar1.PerformStep();
                                    //sendMess("Процесс сравнения: "+percent + "%.");
                                }));
                                temp += temp;
                                //percent += 25;

                            }

                            this.Invoke(new Action(() =>
                            {
                                if (image1.GetPixel(j, i) == image2.GetPixel(j, i))
                                {



                                    bitmap.SetPixel(j, i, Color.Black);

                                    //это окошко обратно в главный поток
                                    //код написанный тут выполнится в главном потоке не вызывая ошибок

                                }
                                else
                                {
                                    bitmap.SetPixel(j, i, Color.White);
                                }
                            }));
                        }

                    }

                    string time = DateTime.Now.ToString("dd.MM.yyyy");//yyyy.MM.dd_HH-mm-ss
                    string time1 = DateTime.Now.ToString("HH.mm.ss");


                    String path = @"Сравнение изображений\" + time + "_" + time1;
                    bool exists1 = System.IO.Directory.Exists(path);
                    if (!exists1)
                        System.IO.Directory.CreateDirectory(path);
                    string path1 = path + "\\Изображение1_." + System.IO.Path.GetFileName(filename1);
                    image1.Save(path1, imFormat1);
                    string path2 = path + "\\Изображение2_." + System.IO.Path.GetFileName(filename2);
                    image2.Save(path2, imFormat2);
                    //File.Copy(infilename, path, true);


                    this.Invoke(new Action(() =>
                    {
                        pictureBox3.Image = bitmap;
                        pictureBox3.Refresh();
                        //это окошко обратно в главный поток
                        //код написанный тут выполнится в главном потоке не вызывая ошибок
                    }));
                    string path3 = path + "\\Изображение3_." + System.IO.Path.GetFileName(filename2);
                    bitmap.Save(path3, ImageFormat.Png);

                }
                else
                {
                    sendMess("Ширина и(или) высота изображений не равны.");
                }
                this.Invoke(new Action(() =>
                {
                    this.ControlBox = true;
                    this.butCompare.Enabled = true;
                    //это окошко обратно в главный поток
                    //код написанный тут выполнится в главном потоке не вызывая ошибок
                }));

            }
            catch (Exception ex)
            {

                MessageBox.Show("Ошибка\n" + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

      

        public void sendMess(String mess)
        {
            this.Invoke(new Action(() =>
            {
                this.richTextBox1.Text += mess + "\n";
                this.richTextBox1.SelectionStart = richTextBox1.Text.Length;
                this.richTextBox1.ScrollToCaret();
                //это окошко обратно в главный поток
                //код написанный тут выполнится в главном потоке не вызывая ошибок
            }));

            
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

       

        /*private void button2_Click_1(object sender, EventArgs e)
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
            koch = new Koch4(this);
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
            this.butInlining.Enabled = false;
            this.richTextBox1.Text = "";
            this.label1.Text = "После встраивания:";
            this.label2.Text = "Исходное изображение: ";
            this.progressBar1.Value = 0;
        }*/





        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {

           
            Form ifrm = new Form2();
            ifrm.Show(); // отображаем Form2
            this.Dispose(); // скрываем INFINPIC_Form11 (this - текущая форма)
        }

        
    }
}
