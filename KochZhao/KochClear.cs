﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace INFINPIC
{
    public class KochClear
    {
        public FormClear form = null;

        public byte[] hideInf = null; //информация для скрытия
        public int P = 25; //коэффициент
        public int mKey = 0; //ключ
        public int sizeSegment = 2; //размер сегмента
        public String infilename = ""; //путь до
        public Bitmap image1 = null; //первая картинка

        public Point p1; //координаты
        public Point p2;

        public ImageFormat imFormat = null; //расширение картинки, png или bmp
        public String datafile = null;
        public int maxSize = 0; // максимальный размер встраиваемого сообщения
        public bool flag = false; //флаг

        public KochClear(FormClear form) //определение 1 формы
        {
            this.form = form;
        }

        public bool inlining() //встраивание информации
        {
            try
            {
                Bitmap workImage = new Bitmap(image1);// рабочее изображение, первое
                changeProgress(0); // изменение прогресса на 0
                blockPanel(true);
                sendMessToForm("Начало встраивания"); //отправление сообщения в richtextbox
                initPoint(); //инициализация координат
                String ex = infilename.Substring(infilename.LastIndexOf(".") + 1); //расширение контейнера(картинки)
                sendMessToForm("Расширение контейнера: " + ex);
                sendMessToForm("P = " + P);
                sendMessToForm("Размер блоков = " + sizeSegment);
                sendMessToForm("Точки встраивания: (" + p1.X + "; " + p1.Y + ")  (" + p2.X + "; " + p2.Y + ")");
                sendMessToForm("Ключ для извлечения: " + hideInf.Length);

                if (ex.Equals("bmp"))//если расширение bmp,то imformat = bmp
                {
                    imFormat = ImageFormat.Bmp;
                }
                if (ex.Equals("png"))
                {
                    imFormat = ImageFormat.Png;
                }

                if ((workImage.Width % sizeSegment) != 0 || (workImage.Height % sizeSegment) != 0)
                {
                    trim(ref workImage, sizeSegment);// подрезание изображения,чтобы размеры изображения были кратными размеру сегмента 
                }
                int sizeX = workImage.Width;//ширина изображения
                int sizeY = workImage.Height;// высота изображения
                Byte[,] R = new Byte[sizeX, sizeY]; //массив пикселей R
                Byte[,] G = new Byte[sizeX, sizeY]; //массив пикселей G
                Byte[,] B = new Byte[sizeX, sizeY]; //массив пикселей B
                for (int i = 0; i < sizeX; i++)
                {
                    for (int j = 0; j < sizeY; j++)
                    {
                        R[i, j] = workImage.GetPixel(i, j).R; //Запись в массивы пикселей
                        G[i, j] = workImage.GetPixel(i, j).G;
                        B[i, j] = workImage.GetPixel(i, j).B;
                    }
                }
                changeProgress(5);
                List<byte[,]> sepB = new List<byte[,]>();
                separation(B, ref sepB, sizeX, sizeY, sizeSegment);//разделение изображения на сегменты
                sendMessToForm("Вычисление ДКП");
                List<double[,]> DKP = new List<double[,]>();
                foreach (byte[,] b in sepB) //Вычисление ДКП
                {
                    DKP.Add(dkp(b));
                }
                changeProgress(15);
                sendMessToForm("Встраивание информации");
                inliningMess(hideInf, ref DKP, P, p1, p2);
                changeProgress(65);
                sendMessToForm("Вычисление ОДКП");
                List<double[,]> ODKP = new List<double[,]>();
                foreach (double[,] d in DKP)//обратное ДКП
                {
                    ODKP.Add(odkp(d));
                }
                changeProgress(75);
                Double[,] newB = new Double[sizeX, sizeY];
                integration(ODKP, ref newB, sizeX, sizeY, sizeSegment);
                newB = normaliz(newB);//нормализация
                changeProgress(85);
                for (int i = 0; i < sizeX; i++)
                {
                    for (int j = 0; j < sizeY; j++)
                    {
                        workImage.SetPixel(i, j, Color.FromArgb(R[i, j], G[i, j], (byte)Math.Round(newB[i, j]))); //второе изображение
                    }
                }
                changeProgress(95);
                sendMessToForm("Встраивание завершено!");
                sendPict(workImage);
                String name = infilename.Substring(infilename.LastIndexOf("\\") + 1);
                String path = Path.GetDirectoryName(infilename);
                File.WriteAllText(path + "\\Зашифрованная_картинка-" + name + ".key.txt", Convert.ToString(hideInf.Length), Encoding.Default);
                path += "\\Зашифрованная_картинка-" + name;
                workImage.Save(path, imFormat);
                changeProgress(100);
                sendMessToForm("Сохранено: " + path + "\n\n");
                return true;
            }
            catch (Exception e)
            {
                changeProgress(100);
                sendMessToForm("Ошибка встраивания: " + e.Message + "\n\n");
                return false;
            }
            finally
            {
                blockPanel(false);
            }
        }

        public bool extraction()
        {
            try
            {
                changeProgress(0);
                blockPanel(true);
                sendMessToForm("Начало извлечения");
                initPoint();
                sendMessToForm("P = " + P);
                sendMessToForm("размер блоков = " + sizeSegment);
                sendMessToForm("точки встраивания: (" + p1.X + "; " + p1.Y + ")  (" + p2.X + "; " + p2.Y + ")");
                sendMessToForm("Ключ: " + mKey);

                Bitmap image = (Bitmap)Image.FromFile(infilename);
                int sizeX = image.Width;
                int sizeY = image.Height;
                Byte[,] R = new Byte[sizeX, sizeY];
                Byte[,] G = new Byte[sizeX, sizeY];
                Byte[,] B = new Byte[sizeX, sizeY];
                for (int i = 0; i < sizeX; i++)
                {
                    for (int j = 0; j < sizeY; j++)
                    {
                        R[i, j] = image.GetPixel(i, j).R;
                        G[i, j] = image.GetPixel(i, j).G;
                        B[i, j] = image.GetPixel(i, j).B;
                    }
                }
                changeProgress(5);
                List<byte[,]> sepB = new List<byte[,]>();
                separation(B, ref sepB, sizeX, sizeY, sizeSegment);
                sendMessToForm("Вычисление ДКП");
                changeProgress(10);
                List<double[,]> DKP = new List<double[,]>();
                foreach (byte[,] b in sepB)
                {
                    DKP.Add(dkp(b));
                }
                changeProgress(20);
                sendMessToForm("Извлечение информации");
                int size = mKey;
                string stringBits = "";
                List<byte> message = new List<byte>();
                int key = mKey;
                List<int> allPos = new List<int>();
                for (int i = 0; i < DKP.Count; i++)
                {
                    allPos.Add(i);
                }
                changeProgress(30);
                double incr = 50 / size;
                double prgb = 30;
                while (size > 0)
                {
                    key = multicarry(key, allPos.Count);
                    int pos = allPos[key];
                    allPos.RemoveAt(key);
                    double AbsPoint1 = Math.Abs(DKP[pos][p1.X, p1.Y]);
                    double AbsPoint2 = Math.Abs(DKP[pos][p2.X, p2.Y]);
                    if (AbsPoint1 > AbsPoint2)
                    {
                        stringBits += "0";
                    }
                    if (AbsPoint1 < AbsPoint2)
                    {
                        stringBits += "1";
                    }
                    if (stringBits.Length == 8)
                    {
                        message.Add(Convert.ToByte(Convert.ToInt32(stringBits, 2)));
                        stringBits = "";
                        size--;
                        prgb += incr;
                        changeProgress(prgb);
                    }
                }
                changeProgress(80);
                image.Dispose();
                String[] ex = processing(message);
                changeProgress(90);
                List<byte> message2 = new List<byte>();
                for (int i = ex[0].Length + 1; i < message.Count - ex[0].Length - 1; i++)
                {
                    message2.Add(message[i]);
                }
                changeProgress(95);
                sendMessToForm("Извлечение завершено!");
                sendMessToForm("Извлечено " + message2.Count + " байт");
                String path = Path.GetDirectoryName(infilename);
                path += "\\mess." + ex[0];
                if (saveMessage(message2, path))
                {
                    sendMessToForm("Сохранено: " + path);
                }
                if (ex[0].CompareTo(ex[1]) != 0)
                {
                    List<byte> message3 = new List<byte>();
                    for (int i = ex[1].Length + 1; i < message.Count - ex[1].Length - 1; i++)
                    {
                        message3.Add(message[i]);
                    }
                    String path2 = Path.GetDirectoryName(infilename);
                    path2 += "\\mess2." + ex[1];
                    if (saveMessage(message3, path2))
                    {
                        sendMessToForm("Сохранено (попытка 2): " + path2);
                    }

                    List<byte> message4 = new List<byte>();
                    for (int i = 4 + 1; i < message.Count - 4; i++)
                    {
                        message4.Add(message[i]);
                    }
                    String path3 = Path.GetDirectoryName(infilename);
                    path3 += "\\mess3";
                    if (saveMessage(message4, path3))
                    {
                        sendMessToForm("Сохранено (попытка 3): " + path3);
                    }
                }
                sendMessToForm("\n\n");
                changeProgress(100);
                return true;
            }
            catch (Exception ex)
            {
                changeProgress(100);
                sendMessToForm("Не удалось извлечь сообщение!\n\n");
                return false;
            }
            finally
            {
                blockPanel(false);
            }
        }

        public bool saveMessage(List<byte> message, String path)
        {
            try
            {
                File.WriteAllBytes(path, message.ToArray());
                return true;
            }
            catch (Exception e)
            {
                sendMessToForm("Не удалось извлечь сообщение!\n\n");
                return false;
            }
        }

        public String[] processing(List<byte> message)
        {
            String[] ex = new String[2];
            String tmp = "";
            for (int i = 0; i < message.Count; i++)
            {
                int n = Convert.ToInt32(message[i]);
                if (n == 126)
                {
                    break;
                }
                tmp += Convert.ToChar(n);
            }
            String tmp2 = "";
            for (int i = message.Count - 1; i >= 0; i--)
            {
                int n = Convert.ToInt32(message[i]);
                if (n == 126)
                {
                    break;
                }
                tmp2 += Convert.ToChar(n);
            }
            String tmp3 = "";
            for (int i = tmp2.Length - 1; i >= 0; i--)
            {
                tmp3 += tmp2[i];
            }
            if (tmp.Length < 10)
            {
                ex[0] = tmp;
            }
            else
            {
                ex[0] = tmp.Substring(0, 9);
            }
            if (tmp3.Length < 10)
            {
                ex[1] = tmp3;
            }
            else
            {
                ex[1] = tmp3.Substring(0, 9);
            }
            return ex;
        }

        public void maxMessSize()
        {
            if (image1 != null && sizeSegment > 0)
            {
                int countB = (int)(image1.Height / sizeSegment) * (int)(image1.Width / sizeSegment);
                int max = countB / 8;
                maxSize = max;
            }
            else
            {
                maxSize = 0;
            }
        }

        private void trim(ref Bitmap image, int sizeSegment)
        {
            int x = image.Width % sizeSegment;
            int y = image.Height % sizeSegment;
            Size newSize = new Size(image.Width - x, image.Height - y);
            Bitmap b = new Bitmap(newSize.Width, newSize.Height);
            for (int i = 0; i < b.Width; i++)
            {
                for (int j = 0; j < b.Height; j++)
                {
                    b.SetPixel(i, j, image.GetPixel(i, j));
                }
            }
            image = b;
        }

        private void inliningMess(byte[] message, ref List<double[,]> DKP, int P, Point p1, Point p2)
        {
            int key = message.Length;
            List<int> allPos = new List<int>();
            for (int i = 0; i < DKP.Count; i++)
            {
                allPos.Add(i);
            }
            changeProgress(20);
            double incr = 45 / message.Length;
            double pgrb = 20;
            for (int i = 0; i < message.Length; i++)
            {
                Converter conv = new Converter(message[i]);
                for (int j = 0; j < 8; j++)
                {
                    key = multicarry(key, allPos.Count);
                    int pos = allPos[key];
                    allPos.RemoveAt(key);
                    double AbsPoint1 = Math.Abs(DKP[pos][p1.X, p1.Y]);
                    double AbsPoint2 = Math.Abs(DKP[pos][p2.X, p2.Y]);
                    int z1 = 1, z2 = 1;
                    if (DKP[pos][p1.X, p1.Y] < 0)
                    {
                        z1 = -1;
                    }
                    if (DKP[pos][p2.X, p2.Y] < 0)
                    {
                        z2 = -1;
                    }
                    if ((int)(conv.bits[j]) == 0)
                    {
                        if (AbsPoint1 - AbsPoint2 <= P)
                        {
                            AbsPoint1 = P + AbsPoint2 + 1;
                        }
                    }
                    if ((int)(conv.bits[j]) == 1)
                    {
                        if (AbsPoint1 - AbsPoint2 >= -P)
                        {
                            AbsPoint2 = P + AbsPoint1 + 1;
                        }
                    }
                    DKP[pos][p1.X, p1.Y] = z1 * AbsPoint1;
                    DKP[pos][p2.X, p2.Y] = z2 * AbsPoint2;
                }
                pgrb += incr;
                changeProgress(pgrb);
            }
        }

        private int multicarry(long x, int maxSize)
        {
            long a = 0xffffda61L;
            x = (a * (x & 65535)) + (x >> 16);
            x = Math.Abs((int)x);
            if (x >= maxSize)
            {
                x = x % maxSize;
            }
            return (int)x;
        }

        private void separation(byte[,] B, ref List<byte[,]> C, int sizeX, int sizeY, int sizeSegment)
        {
            int Nx = sizeX / sizeSegment;
            int Ny = sizeY / sizeSegment;
            for (int i = 0; i < Nx; i++)
            {
                int startX = i * sizeSegment;
                int endX = startX + sizeSegment - 1;
                for (int j = 0; j < Ny; j++)
                {
                    int startY = j * sizeSegment;
                    int endY = startY + sizeSegment - 1;
                    C.Add(submatrix(B, startX, endX, startY, endY));
                }
            }
        }

        private byte[,] submatrix(byte[,] B, int startX, int endX, int startY, int endY)
        {
            int Nx = endX - startX + 1;
            int Ny = endY - startY + 1;
            byte[,] res = new byte[Ny, Nx];
            for (int i = 0; i < Nx; i++)
            {
                for (int j = 0; j < Ny; j++)
                {
                    res[i, j] = B[i + startX, j + startY];
                }
            }
            return res;
        }

        private void insert(ref double[,] newB, double[,] tmp, int startX, int endX, int startY, int endY)
        {
            int Nx = endX - startX + 1;
            int Ny = endY - startY + 1;
            int u = 0;
            for (int i = startX; i < endX + 1; i++)
            {
                int v = 0;
                for (int j = startY; j < endY + 1; j++)
                {
                    newB[i, j] = tmp[u, v];
                    v++;
                }
                u++;
            }
        }

        private void integration(List<double[,]> ODKP, ref double[,] newB, int sizeX, int sizeY, int sizeSegment)
        {
            Double[][,] tmp = ODKP.ToArray();
            int Nx = sizeX / sizeSegment;
            int Ny = sizeY / sizeSegment;
            int k = 0;
            for (int i = 0; i < Nx; i++)
            {
                int startX = i * sizeSegment;
                int endX = startX + sizeSegment - 1;
                for (int j = 0; j < Ny; j++)
                {
                    int startY = j * sizeSegment;
                    int endY = startY + sizeSegment - 1;
                    if (k > tmp.GetLength(0))
                    {
                        throw new IndexOutOfRangeException();
                    }
                    insert(ref newB, tmp[k], startX, endX, startY, endY);
                    k++;
                }
            }
        }

        public void loadMessage(String filename)
        {
            FileStream fs = File.Open(filename, FileMode.Open, FileAccess.Read);
            List<byte> ls = new List<byte>();
            int b1 = 0;
            while (true)
            {
                b1 = fs.ReadByte();
                if (b1 == -1)
                {
                    break;
                }
                ls.Add((byte)b1);
            }
            String ex = filename.Substring(filename.LastIndexOf(".") + 1);
            String startStr = ex + "~";
            String endStr = "~" + ex;
            byte[] tmp = Encoding.GetEncoding(1251).GetBytes(startStr);
            List<byte> ls2 = new List<byte>();
            ls2.AddRange(tmp);
            ls2.AddRange(ls);
            tmp = Encoding.GetEncoding(1251).GetBytes(endStr);
            ls2.AddRange(tmp);
            hideInf = ls2.ToArray();
            fs.Close();
        }

        private void initPoint()
        {
            if (sizeSegment == 2)
            {
                p1 = new Point(1, 0);
                p2 = new Point(1, 1);
            }
            if (sizeSegment == 4)
            {
                p1 = new Point(3, 2);
                p2 = new Point(2, 3);
            }
            if (sizeSegment == 8)
            {
                p1 = new Point(6, 3);
                p2 = new Point(3, 6);
            }
        }

        private double[,] dkp(byte[,] C)
        {
            int n = C.GetLength(0);
            double[,] result = new double[n, n];
            double U, V, temp = 0;
            for (int v = 0; v < n; v++)
            {
                for (int u = 0; u < n; u++)
                {
                    if (v == 0) V = 1.0 / Math.Sqrt(2);
                    else V = 1;
                    if (u == 0) U = 1.0 / Math.Sqrt(2);
                    else U = 1;
                    temp = 0;
                    for (int i = 0; i < n; i++)
                    {
                        for (int j = 0; j < n; j++)
                        {
                            temp += C[i, j] * Math.Cos(Math.PI * v * (2 * i + 1) / (2 * n)) *
                                Math.Cos(Math.PI * u * (2 * j + 1) / (2 * n));
                        }
                    }
                    result[v, u] = U * V * temp / (Math.Sqrt(2 * n));
                }
            }
            return result;
        }

        private double[,] odkp(double[,] dkp)
        {
            int n = dkp.GetLength(0);
            double[,] result = new double[n, n];
            double U, V, temp = 0;
            for (int v = 0; v < n; v++)
            {
                for (int u = 0; u < n; u++)
                {
                    temp = 0;
                    for (int i = 0; i < n; i++)
                    {
                        for (int j = 0; j < n; j++)
                        {
                            if (i == 0) V = 1.0 / Math.Sqrt(2);
                            else V = 1;
                            if (j == 0) U = 1.0 / Math.Sqrt(2);
                            else U = 1;
                            temp += U * V * dkp[i, j] * Math.Cos(Math.PI * i * (2 * v + 1) / (2 * n)) *
                                Math.Cos(Math.PI * j * (2 * u + 1) / (2 * n));
                        }
                    }
                    result[v, u] = temp / (Math.Sqrt(2 * n));
                }
            }
            return result;
        }

        private double[,] normaliz(double[,] odkp)
        {
            double min = Double.MaxValue, max = Double.MinValue;
            for (int i = 0; i < odkp.GetLength(0); i++)
            {
                for (int j = 0; j < odkp.GetLength(1); j++)
                {
                    if (odkp[i, j] > max)
                        max = odkp[i, j];
                    if (odkp[i, j] < min)
                        min = odkp[i, j];
                }
            }
            double[,] result = new double[odkp.GetLength(0), odkp.GetLength(1)];
            for (int i = 0; i < odkp.GetLength(0); i++)
            {
                for (int j = 0; j < odkp.GetLength(1); j++)
                {
                    result[i, j] = 255 * (odkp[i, j] + Math.Abs(min)) / (max + Math.Abs(min));
                }
            }
            return result;
        }

        private void sendMessToForm(String mess)
        {
            if (form.InvokeRequired)
                form.Invoke((MethodInvoker)delegate { form.sendMess(mess); });
            else
                form.sendMess(mess);
        }

        private void sendPict(Bitmap image)
        {
            if (form.InvokeRequired)
                form.Invoke((MethodInvoker)delegate { form.pict2(image); });
            else
                form.pict2(image);
        }

        private void changeProgress(double val)
        {
            if (form.InvokeRequired)
                form.Invoke((MethodInvoker)delegate { form.progress(val); });
            else
                form.progress(val);
        }

        private void blockPanel(bool flag)
        {
            if (flag)
            {
                if (form.InvokeRequired)
                    form.Invoke((MethodInvoker)delegate { form.blockPanel(); });
                else
                    form.blockPanel();
            }
            if (!flag)
            {
                if (form.InvokeRequired)
                    form.Invoke((MethodInvoker)delegate { form.unblockPanel(); });
                else
                    form.unblockPanel();
            }
        }

    }
}
