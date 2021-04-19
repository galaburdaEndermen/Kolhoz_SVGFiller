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
            openFileDialog.Filter = "Svg files(*.svg)| *.svg";
            if (openFileDialog.ShowDialog() == true)
            {
                source = openFileDialog.FileName;
                var matches = Regex.Matches(File.ReadAllText(source), @"(<text.*<\/text>)");
                foreach (var match in matches)
                {
                    var sas = Regex.Matches(match.ToString(), @"(?<=>)([\w\s]+)(?=<\/)");
                    foreach (var item in sas)
                    {
                        var field = new Field() { Name = item.ToString(), Value = item.ToString() };
                        field.PropertyChanged += Field_PropertyChanged;
                        Variables.Add(field);
                    }
                   
                }
                RaisePropertyChanged(nameof(Variables));
                
            }
            else
            {
                Application.Current.Shutdown();
            }
                
        }

        private void Field_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {

            generated = File.ReadAllText(source);
            foreach (var variable in Variables)
            {
                generated = generated.Replace(variable.Name, variable.Value);
            }
            RaisePropertyChanged(nameof(Image));
        }

        public ObservableCollection<Field> Variables
        {
            get;
            set;
        } = new ObservableCollection<Field>();

        public MainViewModels()
        {
            InitializeSource();
            render = new Command(renderPNG);
        }

        private Command render;
        public Command Render { get { return render; } }
        void renderPNG(object o)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Png files(*.png)| *.png";
            saveFileDialog.DefaultExt = "png";
            saveFileDialog.FileName = Path.GetFileNameWithoutExtension(source) + ".png";
            if (saveFileDialog.ShowDialog() == true)
            {
                var fileName = saveFileDialog.FileName;


                using (FileStream file = new FileStream(path: fileName, mode: FileMode.OpenOrCreate))
                {
                    var svgDoc = SvgDocument.FromSvg<SvgDocument>(generated);
                    Bitmap bmp = svgDoc.Draw();

                    bmp.Save(file, ImageFormat.Png);
                    file.Position = 0;
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = file;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();

                }
            }
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
                        var svgDoc = SvgDocument.FromSvg<SvgDocument>(generated);
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
