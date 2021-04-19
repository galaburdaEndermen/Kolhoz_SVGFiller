using Kolhoz_SVGFiller.Models;
using Microsoft.Win32;
using Svg;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    public class MainViewModels : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private string source = "";
        private string generated = "";
        private void InitializeSource()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                source = openFileDialog.FileName;
                var matches = Regex.Matches(File.ReadAllText(source), @"(<text.*<\/text>)");
                foreach (var match in matches)
                {
                    var sas = Regex.Match(match.ToString(), @"(?<=>)([\w\s]+)(?=<\/)");
                    variables.Add(new Field() { Name = sas.ToString(), Value = "" });
                }
                RaisePropertyChanged(nameof(Variables));
                
            }
                
        }

        private List<Field> variables = new List<Field>();
        public ObservableCollection<Field> Variables 
        {
            get
            {
                return new ObservableCollection<Field>(variables);
            }
            set
            {
                variables = new List<Field>(value);
                RaisePropertyChanged(nameof(Variables));

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
                if (string.IsNullOrWhiteSpace(generated))
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
                else
                {
                    using (MemoryStream memory = new MemoryStream())
                    {
                        var svgDoc = SvgDocument.Open(generated);
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
}
