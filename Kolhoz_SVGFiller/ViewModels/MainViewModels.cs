using Microsoft.Win32;
using Svg;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace Kolhoz_SVGFiller.ViewModels
{
    public class MainViewModels
    {
        private string source = "";
        private void InitializeSource()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                source = openFileDialog.FileName;
                //'(<text.*<\/text>)'
                var sas = Regex.Matches(File.ReadAllText(source), @"(<text.*<\/text>)");
                
            }
                
        }

        public MainViewModels()
        {
            InitializeSource();
        }

        public object Image 
        { 
            get 
            {
                if (string.IsNullOrWhiteSpace(source))
                {
                    InitializeSource();
                }
                using (MemoryStream memory = new MemoryStream())
                {
                    var svgDoc = SvgDocument.Open(source);
                    Bitmap bmp = svgDoc.Draw();

                    bmp.Save(memory, ImageFormat.Png);
                    memory.Position = 0;
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = memory;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();
                    return bitmapImage;
                }
            }
        }
    }
}
