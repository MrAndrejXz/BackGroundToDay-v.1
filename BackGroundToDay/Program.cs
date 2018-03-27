using System;
using System.Net;
using System.IO;
using System.Runtime.InteropServices;
using System.Drawing;
using Microsoft.Win32;

namespace BackGroundToDay
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            /*
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
             */
            Func();
        }

        #region Функция Win32
        const int SPI_SETDESKWALLPAPER = 20;
        const int SPIF_UPDATEINIFILE = 0x01;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        static private void SetWallPaper(string path)
        {
            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, path, SPIF_UPDATEINIFILE);
        }
        #endregion

        /// <summary>
        /// Обрезать картинку
        /// </summary>
        /// <param name="source">Картинка</param>
        /// <returns>Обрезанная картинка</returns>
        static public Bitmap CropImage(Bitmap source)
        {
            Rectangle section = new Rectangle(new Point(0, 0), new Size(source.Width, source.Height - 30));
            Bitmap bmp = new Bitmap(section.Width, section.Height);
            Graphics g = Graphics.FromImage(bmp);
            g.DrawImage(source, 0, 0, section, GraphicsUnit.Pixel);
            return bmp;
        }

        /// <summary>
        /// Загружает картинку
        /// </summary>
        /// <returns>Загруженная картинка</returns>
        static Bitmap DownloadImage()
        {
            WebClient wc = new WebClient();
            MemoryStream ms;
            Bitmap image;
            // Пробуем загрузить картинку
            while (true)
            {
                try
                {
                    byte[] bytes = wc.DownloadData("https://yandex.ru/images/today");
                    ms = new MemoryStream(bytes);
                    image = new Bitmap(Image.FromStream(ms));
                    break;
                }
                catch
                {
                    System.Threading.Thread.Sleep(10000);
                }
            }
            return image;
        }

        /// <summary>
        /// Путь к папке для сохранения картинок
        /// </summary>
        static string path = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + "\\BackGroundToDay";
        
        /// <summary>
        /// Триггер удаления
        /// </summary>
        static bool delete = false;

        /// <summary>
        /// Создает папку, если такой нет
        /// </summary>
        static void CreateFolder()
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
        
        /// <summary>
        /// Удаляет все картинку, кроме текущей
        /// </summary>
        static void Delete()
        {
            if (delete)
            {
                // Удаление всех, кроме текущей
                var x = System.IO.Directory.GetFiles(path);
                foreach (var i in x)
                {
                    if (!i.Contains(DateTime.Now.ToShortDateString()))
                        System.IO.File.Delete(i);
                }
            }
        }

        static void Func()
        {
            // Загружаем картинку
            Bitmap source = DownloadImage();
            // Обрезаем картинку
            source = new Bitmap(CropImage(source), source.Width, source.Height);
            // Создаем папку для сохранения
            CreateFolder();
            // Сохраняем картинку
            source.Save(path + @"\WallPaper_" + DateTime.Now.ToShortDateString() + ".bmp", System.Drawing.Imaging.ImageFormat.Bmp);
            // Устанавливаем фон
            SetWallPaper(path + @"\WallPaper_" + DateTime.Now.ToShortDateString() + ".bmp");
            // Удаляем все, кроме текущего
            Delete();
        }
    }
}
