using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using FilePath = System.IO.Path;

namespace WpfApp3_pd
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<List<ImageMark>> obj;
        List<ImageMark> img;
        List<ImageMark> tmp;
        bool switchLists = true;

        public MainWindow()
        {
            InitializeComponent();
            tmp = new List<ImageMark>();

            obj = new List<List<ImageMark>>();
            string[] allfolders = Directory.GetDirectories(Directory.GetCurrentDirectory() + "/Images");
            foreach (string item in allfolders)
            {
                Default.IsSelected = true;

                string t = item.Substring((Directory.GetCurrentDirectory() + "/Images").Length + 1);
                lbFolder.Items.Add(t);

                img = new List<ImageMark>();
                string path = (Directory.GetCurrentDirectory() + "\\Images\\" + t);
                int CountImages = Directory.GetFiles(path).Length;
                for (int i = 0; i < CountImages; i++)
                {
                    BitmapImage imageSource = new BitmapImage();
                    imageSource.BeginInit();
                    imageSource.UriSource = new Uri($"Images/{t}/{i}.jpg", UriKind.Relative);
                    imageSource.EndInit();
                    img.Add(new ImageMark($"Images/{t}/{i}.jpg"));
                }
                obj.Add(img);
            }
            lbFolder.SelectedIndex = 0;
            sldPicture.Minimum = 0;
            sldPicture.Maximum = obj[lbFolder.SelectedIndex].Count - 1;
            sldPicture.Value = 0;
            ChangePicture();
            LoadDataImageMark();


            if (obj[lbFolder.SelectedIndex][Convert.ToInt32(sldPicture.Value)].Mark != 0)
            {
                if (spRadioButtons.Children[obj[lbFolder.SelectedIndex][Convert.ToInt32(sldPicture.Value)].Mark - 1] is RadioButton r)
                {
                    r.IsChecked = true;
                }
            }
        }

        private void ChangePicture()
        {
            try
            {
                var directory = FilePath.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                if (switchLists)
                {
                    BitmapImage bitmap = new BitmapImage(new Uri(FilePath.Combine(directory, obj[lbFolder.SelectedIndex][Convert.ToInt32(sldPicture.Value)].Path)));
                    imgMain.Source = bitmap;
                    FileInfo file = new FileInfo(FilePath.Combine(directory, obj[lbFolder.SelectedIndex][Convert.ToInt32(sldPicture.Value)].Path));
                    sizeKB.Content = ((double)file.Length / 1024).ToString("#.##") + " Kb";
                    resolution.Content = (bitmap.Width + "x" + bitmap.Height);
                }
                else
                {
                    BitmapImage bitmap = new BitmapImage(new Uri(FilePath.Combine(directory, tmp[Convert.ToInt32(sldPicture.Value)].Path)));
                    imgMain.Source = bitmap;
                    FileInfo file = new FileInfo(FilePath.Combine(directory, tmp[Convert.ToInt32(sldPicture.Value)].Path));
                    sizeKB.Content = ((double)file.Length / 1024).ToString("#.##") + " Kb";
                    resolution.Content = (bitmap.Width + "x" + bitmap.Height);
                }

            }
            catch
            {
                imgMain.Source = null;
                sizeKB.Content = "---";
                resolution.Content = "---";
            }
        }

        private void sldPicture_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ChangePicture();
            try
            {
                for (int i = 0; i < 5; i++)
                {
                    if (spRadioButtons.Children[i] is RadioButton r)
                    {
                        if (r.IsChecked == true)
                        {
                            r.IsChecked = false;
                        }
                    }
                }
                if (switchLists)
                {
                    if (obj[lbFolder.SelectedIndex][Convert.ToInt32(sldPicture.Value)].Mark != 0)
                    {
                        if (spRadioButtons.Children[obj[lbFolder.SelectedIndex][Convert.ToInt32(sldPicture.Value)].Mark - 1] is RadioButton r)
                        {
                            r.IsChecked = true;
                        }
                    }
                }
                else
                {
                    if (tmp[Convert.ToInt32(sldPicture.Value)].Mark != 0)
                    {
                        if (spRadioButtons.Children[tmp[Convert.ToInt32(sldPicture.Value)].Mark - 1] is RadioButton r)
                        {
                            r.IsChecked = true;
                        }
                    }
                }
            }
            catch{}
        }

        private void Prev_Click(object sender, RoutedEventArgs e)
        {
            if (sldPicture.Value > 0)
            {
                sldPicture.Value -= 1;
            }
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            if (sldPicture.Value < obj[lbFolder.SelectedIndex].Count)
            {
                sldPicture.Value += 1;
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (switchLists)
            {
                spRadioButtons.IsEnabled = true;
                if (sender is RadioButton rbt)
                {
                    if (rbt.IsChecked.Value)
                    {
                        int a = 0;
                        if (int.TryParse(rbt.Content.ToString(), out a))
                        {
                            obj[lbFolder.SelectedIndex][Convert.ToInt32(sldPicture.Value)].Mark = a;
                        }
                    }
                }
                SaveDataImageMark();
            }
            else
            {
                spRadioButtons.IsEnabled = false;
            }
        }

        private void lbFolder_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                lbFolder.IsEnabled = true;
                sldPicture.Value = 0;

                sldPicture_ValueChanged(this, null);
                ChangePicture();

                if (switchLists)
                {
                    sldPicture.Maximum = obj[lbFolder.SelectedIndex].Count - 1;
                }
                else
                {
                    lbFolder.IsEnabled = false;
                    sldPicture.Maximum = tmp.Count - 1;
                }
            }
            catch
            {
                sldPicture.Maximum = 0;
                imgMain.Source = null;
                for (int i = 0; i < 5; i++)
                {
                    if (spRadioButtons.Children[i] is RadioButton r)
                    {
                        if (r.IsChecked == true)
                        {
                            r.IsChecked = false;
                        }
                    }
                }
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            tmp = new List<ImageMark>();
            if (sender is ComboBox comboBox)
            {
                ComboBoxItem selectedItem = (ComboBoxItem)comboBox.SelectedItem;
                int x = 0;
                if (int.TryParse(selectedItem.Content.ToString(), out x))
                {
                    RadioButton_Checked(this, null);
                    foreach (List<ImageMark> item in obj)
                    {
                        foreach (ImageMark jtem in item)
                        {
                            if (jtem.Mark == x)
                            {
                                tmp.Add(jtem);
                            }
                        }
                    }
                    if (tmp.Count != 0)
                    {
                        switchLists = false;
                        lbFolder_SelectionChanged(this, null);
                    }
                    else
                    {
                        imgMain.Source = null;
                        sizeKB.Content = "---";
                        resolution.Content = "---";
                        if (spRadioButtons.Children[x - 1] is RadioButton r)
                        {
                            r.Checked -= RadioButton_Checked;
                            r.IsChecked = true;
                            r.Checked += RadioButton_Checked;
                            spRadioButtons.IsEnabled = false;
                        }
                    }
                }
                else
                {
                    switchLists = true;
                    tmp = new List<ImageMark>();
                    lbFolder.IsEnabled = true;
                    lbFolder_SelectionChanged(this, null);
                }
            }
        }

        private void SaveDataImageMark()
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            try
            {
                using (Stream fStream = File.Create("Data.bin"))
                {
                    binaryFormatter.Serialize(fStream, obj);
                }
            }
            catch
            {
                MessageBox.Show("Data save error!");
            }
        }

        private void LoadDataImageMark()
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            try
            {
                using (Stream fStream = File.OpenRead("Data.bin"))
                {
                    List<List<ImageMark>> objtmp = new List<List<ImageMark>>();
                    objtmp = (List<List<ImageMark>>)binaryFormatter.Deserialize(fStream);

                    bool allisOK = true;
                    if (objtmp.Count == obj.Count)
                    {
                        for (int i = 0; i < objtmp.Count; i++)
                        {
                            if (objtmp[i].Count != obj[i].Count)
                            {
                                allisOK = false;
                                break;
                            }
                        }
                    }
                    else
                    {
                        allisOK = false;
                    }

                    if (!allisOK)
                    {
                        MessageBox.Show("Count of files was changed, data file does not loaded!");
                    }
                    else
                    {
                        obj = objtmp;
                    }
                }
            }
            catch{}
        }
    }
}
