using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Music_App
{
    public partial class MainWindow : Window
    {
        MediaPlayer mediaPlayer = new MediaPlayer();
        string filename;
        private bool isSliderDragging = false;

        private ColorTheme currentTheme = ColorTheme.Yellow;
        enum ColorTheme
        {
            Red,
            Green,
            Blue,
            Yellow,
        }

        DispatcherTimer timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(100),
        };

        public MainWindow()
        {
            InitializeComponent();
            string ImgPath = $"{Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.ToString()}\\Images\\notev2.png";
            Musicimg.Source = new BitmapImage(new Uri(ImgPath));
            timer.Tick += Timer_Tick;
        }

        private void Card_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void BT_Click_Close(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void BT_Click_Open(object sender, RoutedEventArgs e)
        {
            string path = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName + "\\Music";

            OpenFileDialog fileDialog = new OpenFileDialog
            {
                InitialDirectory = path,
                Multiselect = false,
                DefaultExt = ".mp3",
            };

            bool? dialogOk = fileDialog.ShowDialog();
            if (dialogOk == true)
            {
                filename = fileDialog.FileName;
                string filestr = fileDialog.SafeFileName;
                lblSongname.Text = filestr.Substring(0, filestr.Length - 4); 
                mediaPlayer.Open(new Uri(filename));

                mediaPlayer.MediaOpened += MediaPlayer_MediaOpened;
                mediaPlayer.MediaEnded += MediaPlayer_MediaEnded;
            }
        }

        private void MediaPlayer_MediaOpened(object sender, EventArgs e)
        {
            if (mediaPlayer.NaturalDuration.HasTimeSpan)
            {
                TimeSpan duration = mediaPlayer.NaturalDuration.TimeSpan;
                TimerSlider.Maximum = duration.TotalSeconds;
                lblMusiclength.Text = duration.ToString(@"m\:ss");
                mediaPlayer.Position = TimeSpan.FromSeconds(0);
                mediaPlayer.Play();
                timer.Start();
            }
        }

        private void MediaPlayer_MediaEnded(object sender, EventArgs e)
        {
            mediaPlayer.Stop();
            TimerSlider.Value = 0;
        }

        private void BT_Click_Replay(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Position = TimeSpan.FromSeconds(0);
            mediaPlayer.Play();
            timer.Start();
        }

        private void BT_Click_Pause(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Pause();
            timer.Stop();
        }

        private void BT_Click_Resume(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Play();
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (mediaPlayer.NaturalDuration.HasTimeSpan && !isSliderDragging)
            {
                TimeSpan currentSpan = mediaPlayer.Position;
                TimerSlider.Value = currentSpan.TotalSeconds;
                lblCurrenttime.Text = currentSpan.ToString(@"m\:ss");
            }
        }
        
        private void TimerSlider_DragStarted(object sender, DragStartedEventArgs e)
        {
            isSliderDragging = true;
        }

        private void TimerSlider_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            var seconds = TimerSlider.Value;
            TimeSpan time = TimeSpan.FromSeconds(seconds);
            mediaPlayer.Position = time;
            lblCurrenttime.Text = time.ToString(@"m\:ss");
            isSliderDragging = false;
        }

        private void BT_Click_ChangeTheme(object sender, RoutedEventArgs e)
        {
            currentTheme = ThemeChange();
            ApplyTheme(currentTheme);
        }

        private ColorTheme ThemeChange()
        {
            ColorTheme nextColor;
            switch (currentTheme)
            {
                case ColorTheme.Red:
                    nextColor = ColorTheme.Green;
                    break;
                case ColorTheme.Green:
                    nextColor = ColorTheme.Blue;
                    break;
                case ColorTheme.Blue:
                    nextColor = ColorTheme.Yellow;
                    break;
                case ColorTheme.Yellow:
                    nextColor = ColorTheme.Red;
                    break;
                default:
                    nextColor = ColorTheme.Red;
                    break;
            }
            return nextColor;
        }

        private void ApplyTheme(ColorTheme themeToDisplay)
        {
            Style buttonStyle, sliderStyle;
            string buttonStyleStr, sliderStyleStr;
            buttonStyleStr = "CActionButtons" + themeToDisplay.ToString();
            buttonStyle = (Style)FindResource(buttonStyleStr);

            sliderStyleStr = "CSliders" + themeToDisplay.ToString();
            sliderStyle = (Style)FindResource(sliderStyleStr);

            foreach (Button button in FindVisualChildren<Button>(this))
            {
                button.Style = buttonStyle;
            }
            TimerSlider.Style = sliderStyle;
        }

        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject parent) where T : DependencyObject
        {
            if (parent != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T nestedChild in FindVisualChildren<T>(child))
                    {
                        yield return nestedChild;
                    }
                }
            }
        }


    }
}
