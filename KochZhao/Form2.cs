using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace INFINPIC
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();

            ToolTip t = new ToolTip();
            t.SetToolTip(button1, "Кодирование информации \nв изображение");
            t.SetToolTip(button2, "Декодирование информации \nиз изображения");
            t.SetToolTip(button3, "Сравнение изображений \nпопиксельно");
            t.SetToolTip(button4, "Очистка изображения путем кодирования \nслучайной информации в изображение");

            t.SetToolTip(button10, "Удаление всех папок\nв каталоге с программой");

            string subPath1 = "Кодирование"; 
            bool exists1 = System.IO.Directory.Exists(subPath1);
            if (!exists1)
                System.IO.Directory.CreateDirectory(subPath1);

            string subPath2 = "Декодирование"; 
            bool exists2 = System.IO.Directory.Exists(subPath2);
            if (!exists2)
                System.IO.Directory.CreateDirectory(subPath2);

            string subPath3 = "Сравнение изображений"; 
            bool exists3 = System.IO.Directory.Exists(subPath3);
            if (!exists3)
                System.IO.Directory.CreateDirectory(subPath3);

            string subPath4 = "Очистка изображений"; 
            bool exists4 = System.IO.Directory.Exists(subPath4);
            if (!exists4)
                System.IO.Directory.CreateDirectory(subPath4);

            //label1.Focus();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form ifrm = new Form2();
            ifrm.Show(); // отображаем Form2
            this.Hide(); // скрываем Form1 (this - текущая форма)


        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            Form ifrm = new Form1();
            ifrm.Show(); // отображаем Form2
            this.Hide(); // скрываем Form1 (this - текущая форма)
        }

        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {

            Application.Exit();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form ifrm = new Form3();
            ifrm.Show(); // отображаем Form2
            this.Hide(); // скрываем Form1 (this - текущая форма)
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form ifrm = new Form4();
            ifrm.Show(); // отображаем Form4
            this.Hide(); // скрываем Form1 (this - текущая форма)
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Form ifrm = new FormClear_new();
            ifrm.Show(); // отображаем Form4
            this.Hide(); // скрываем Form1 (this - текущая форма)
        }

        private void button1_MouseEnter(object sender, EventArgs e)
        {
           // button1.BackgroundImage = Properties.Resources.image11;
        }

        private void button1_MouseLeave(object sender, EventArgs e)
        {
           // button1.BackgroundImage = Properties.Resources.image1;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Form ifrm = new Help(Properties.Resources.help2);
            ifrm.Show(); // отображаем Form4
           // this.Hide(); // скрываем Form1 (this - текущая форма)
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Form ifrm = new Help(Properties.Resources.help3);
            ifrm.Show(); 
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Form ifrm = new Help(Properties.Resources.help4);
            ifrm.Show(); 
        }

        private void button9_Click(object sender, EventArgs e)
        {
            Form ifrm = new Help(Properties.Resources.help5);
            ifrm.Show();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            //УДАЛЕНИЕ всех папок

            DialogResult dialogResult = MessageBox.Show("Вы точно уверены, что хотите удалить \nвсе папки, подпапки, и вложенные в них файлы?", "", MessageBoxButtons.YesNo);

            if (dialogResult == DialogResult.Yes)
            {
                string path = Application.StartupPath;
                DirectoryInfo myDir = new DirectoryInfo(path);
                Console.WriteLine(path);

                DeleteRecursiveFolder(path);
            }
            else if (dialogResult == DialogResult.No)
            {
                //do something else
                
            }
            

        }

        private void DeleteRecursiveFolder(string pFolderPath)
        {
            
            try
            {
                foreach (string Folder in Directory.GetDirectories(pFolderPath))
                {
                    DeleteRecursiveFolder(Folder);
                }

                foreach (string file in Directory.GetFiles(pFolderPath))
                {
                    Console.WriteLine(file);
                    if (file.ToString() != "INFINPIC.exe")
                    {
                        var pPath = Path.Combine(pFolderPath, file);
                        FileInfo fi = new FileInfo(pPath);
                        File.SetAttributes(pPath, FileAttributes.Normal);
                        File.Delete(file);
                    }
                }

                Directory.Delete(pFolderPath);
            }
            catch
            {
                Console.WriteLine("error");
            }




        }
    }
}
