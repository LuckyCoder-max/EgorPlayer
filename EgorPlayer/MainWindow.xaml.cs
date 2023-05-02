using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace EgorPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            SoundPower.Value = _player.Volume;
        }

        private List<string> _paths = new();
        private MediaPlayer _player = new();

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void AddBtn_OnClick(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            ofd.Filter = "mp3 files(*.mp3)|*.mp3|wav files(*.wav)|*.wav|aiff files(*.aiff)|*.aiff|ape files(*.ape)|*.ape|flac files(*.flac)|*.flac|ogg files(*.ogg)|*.ogg";
            if (ofd.ShowDialog() == false) return;
            foreach (var path in ofd.FileNames)
            {
                _paths.Add(path);
            }

            foreach (var name in ofd.SafeFileNames)
            {
                Playlist.Items.Add(name);
            }
        }

        private void Playlist_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(Playlist.SelectedIndex == -1) return;
            _player.Open(new Uri(_paths[Playlist.SelectedIndex], UriKind.RelativeOrAbsolute));
            _player.Play();
        }


        private void PlayBtn_OnClick(object sender, RoutedEventArgs e) => _player.Play();
        private void PauseBtn_OnClick(object sender, RoutedEventArgs e) => _player.Pause();
        private void StopBtn_OnClick(object sender, RoutedEventArgs e) => _player.Stop();
        private void Hide_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;
        private void Close_Click(object sender, RoutedEventArgs e) => Close();
        private void trackProgress_MouseDown(object sender, MouseButtonEventArgs e) => _player.Position = TimeSpan.FromSeconds((e.GetPosition(trackProgress).X * trackProgress.Maximum) / trackProgress.ActualWidth);


        private void Playlist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Playlist.SelectedIndex == -1) _player.Stop();
            else
            {
                _player.Open(new Uri(_paths[Playlist.SelectedIndex], UriKind.RelativeOrAbsolute));
                _player.Play();
            }
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            if (Playlist.SelectedIndex < Playlist.Items.Count - 1) Playlist.SelectedIndex++;
            else
            {
                Playlist.SelectedIndex = 0;
            }
        }

        private void Previous_Click(object sender, RoutedEventArgs e)
        {
            if (Playlist.SelectedIndex > 0) Playlist.SelectedIndex--;
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            var sfd = new SaveFileDialog();
            sfd.Filter = "Playlist files(*.pls)|*.pls";
            if (sfd.ShowDialog() == false) return;
            var writer = new StreamWriter(sfd.FileName);
            foreach (var item in _paths)
            {
                writer.WriteLine(item);
            }
            writer.Close();
            MessageBox.Show("Playlist saved");
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Playlist.SelectedItem != null)
            {
                _paths.RemoveAt(Playlist.SelectedIndex);
                Playlist.Items.RemoveAt(Playlist.SelectedIndex);
            }
        }

        private void LoadBtn_Click(object sender, RoutedEventArgs e)
        {
            var opd = new OpenFileDialog();
            opd.Filter = "Playlist files(*.pls)|*.pls";
            if (opd.ShowDialog() == false) return;
            ClearBtn_Click(sender, e);
            using (StreamReader sr = new StreamReader(opd.FileName))
            {
                string? line;
                while ((line = sr.ReadLine()) != null)
                {
                    _paths.Add(line);
                    Playlist.Items.Add(System.IO.Path.GetFileName(line));
                }
            }
        }

        private void ClearBtn_Click(object sender, RoutedEventArgs e)
        {
            Playlist.Items.Clear();
            _paths.Clear();
        }

        private void Grid_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            _player.Volume += e.Delta > 0 ? 0.01 : -0.01;
            VolumeTB.Text = (Math.Round(_player.Volume * 100)) + "%";
            SoundPower.Value = _player.Volume;
        }

        private void RandomBtn_Click(object sender, RoutedEventArgs e)
        {
            Random random = new Random();
            Playlist.SelectedIndex = random.Next(0, Playlist.Items.Count);
            _player.Play();
        }

        private void SoundPower_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _player.Volume = SoundPower.Value;
            VolumeTB.Text = (Math.Round(_player.Volume * 100)) + "%";
        }
    }
}
