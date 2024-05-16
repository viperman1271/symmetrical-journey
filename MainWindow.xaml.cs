using NAudio.Wave;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace MuseeAudio
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        ObservableCollection<string> _Collection;
        private string file;
        private string destination;
        private string selectTemplate;

        public MainWindow()
        {
            _Collection = new ObservableCollection<string>
            {
                "Printemps",
                "Été",
                "Automne",
                "Hiver"
            };

            InitializeComponent();

            DataContext = this;
        }

        public string File
        {
            get => file;
            set
            {
                file = value;
                OnPropertyChanged();
            }
        }

        public string Destination
        {
            get => destination;
            set
            {
                destination = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> Collection
        {
            get { return _Collection; }
        }

        public string SelectTemplate
        {
            get => selectTemplate;
            set
            {
                selectTemplate = value;
                OnPropertyChanged();
            }
        }

        public ICommand ChooseFile
        {
            get => new DelegateCommand(delegate
            {
                var dialog = new Microsoft.Win32.OpenFileDialog();
                dialog.DefaultExt = ".wav"; // Default file extension
                dialog.Filter = "Fichier wav (.wav)|*.wav"; // Filter files by extension

                // Show open file dialog box
                bool? result = dialog.ShowDialog();

                // Process open file dialog box results
                if (result == true)
                {
                    // Open document
                    File = dialog.FileName;
                }
            });
        }

        public ICommand ChooseDestination
        {
            get => new DelegateCommand(delegate
            {
                using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
                {
                    dialog.Description = "Selectionner le dossier destinaire";

                    System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                    if (result == System.Windows.Forms.DialogResult.OK)
                    {
                        Destination = dialog.SelectedPath;
                    }
                }
            });
        }

        public ICommand Execute
        {
            get => new DelegateCommand(delegate
            {
                if (!System.IO.File.Exists(File))
                {
                    MessageBox.Show($"Fichier {File} n'existe pas", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else if (!Directory.Exists(Destination))
                {
                    MessageBox.Show($"Dossier {Destination} n'existe pas", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else if(string.IsNullOrEmpty(SelectTemplate))
                {
                    MessageBox.Show("Saison pas selectionné", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    using (WaveFileReader reader = new WaveFileReader(File))
                    {
                        if (reader.WaveFormat.Channels != 4)
                        {
                            MessageBox.Show($"Mauvais nombre de cannaux detecté [{reader.WaveFormat.Channels}].", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        else
                        {
                            int bytesPerSample = reader.WaveFormat.BitsPerSample / 8;

                            WaveFormat newFormat = new WaveFormat(reader.WaveFormat.SampleRate, reader.WaveFormat.BitsPerSample, 28);
                            using(WaveFileWriter writer = new WaveFileWriter(Path.ChangeExtension(File, ".modifie.wav"), newFormat))
                            {
                                int currentOffset = 0;
                                while(reader.Position < reader.Length)
                                {
                                    byte[] data = new byte[bytesPerSample * reader.WaveFormat.Channels];
                                    reader.Read(data, 0, data.Length);

                                    currentOffset += data.Length;

                                    ReadOnlySpan<byte> convertedValues = ConvertValues(data, bytesPerSample);
                                    writer.Write(convertedValues);
                                }
                            }
                        }
                    }
                }
            });
        }

        private float[] ConvertValues(float[] values)
        {
            switch(SelectTemplate)
            {
                case "Printemps":
                    return new float[]
                    {
                        values[0],  //1
                        0,          //2
                        0,          //3
                        values[1],  //4
                        0,          //5
                        0,          //6
                        0,          //7
                        0,          //8
                        0,          //9
                        0,          //10
                        0,          //11
                        0,          //12
                        0,          //13
                        0,          //14
                        0,          //15
                        0,          //16
                        0,          //17
                        0,          //18
                        values[2],  //19
                        values[3],  //20
                        0,          //21
                        0,          //22
                        0,          //23
                        0,          //24
                        0,          //25
                        0,          //26
                        0,          //28
                        0,          //29
                    };

                case "Été":
                    return new float[]
                    {
                        values[0],  //1
                        0,          //2
                        0,          //3
                        values[1],  //4
                        0,          //5
                        0,          //6
                        0,          //7
                        0,          //8
                        0,          //9
                        0,          //10
                        0,          //11
                        0,          //12
                        0,          //13
                        0,          //14
                        0,          //15
                        0,          //16
                        0,          //17
                        0,          //18
                        values[2],  //19
                        values[3],  //20
                        0,          //21
                        0,          //22
                        0,          //23
                        0,          //24
                        0,          //25
                        0,          //26
                        0,          //28
                        0,          //29
                    };

                case "Automne":
                    return new float[]
                    {
                        values[0],  //1
                        0,          //2
                        0,          //3
                        values[1],  //4
                        0,          //5
                        0,          //6
                        0,          //7
                        0,          //8
                        0,          //9
                        0,          //10
                        0,          //11
                        0,          //12
                        0,          //13
                        0,          //14
                        0,          //15
                        0,          //16
                        0,          //17
                        0,          //18
                        values[2],  //19
                        values[3],  //20
                        0,          //21
                        0,          //22
                        0,          //23
                        0,          //24
                        0,          //25
                        0,          //26
                        0,          //28
                        0,          //29
                    };

                case "Hiver":
                    return new float[]
                    {
                        values[0],  //1
                        0,          //2
                        0,          //3
                        values[1],  //4
                        0,          //5
                        0,          //6
                        0,          //7
                        0,          //8
                        0,          //9
                        0,          //10
                        0,          //11
                        0,          //12
                        0,          //13
                        0,          //14
                        0,          //15
                        0,          //16
                        0,          //17
                        0,          //18
                        values[2],  //19
                        values[3],  //20
                        0,          //21
                        0,          //22
                        0,          //23
                        0,          //24
                        0,          //25
                        0,          //26
                        0,          //28
                        0,          //29
                    };
            }

            return new float[]
            {
                0,          //1
                0,          //2
                0,          //3
                0,          //4
                0,          //5
                0,          //6
                0,          //7
                0,          //8
                0,          //9
                0,          //10
                0,          //11
                0,          //12
                0,          //13
                0,          //14
                0,          //15
                0,          //16
                0,          //17
                0,          //18
                0,          //19
                0,          //20
                0,          //21
                0,          //22
                0,          //23
                0,          //24
                0,          //25
                0,          //26
                0,          //28
                0,          //29
            };
        }

        private void CopyToChannel(byte[] dst, int dstChannel, byte[] src, int srcChannel, int bytesPerSample)
        {
            for(int i = 0; i < bytesPerSample; ++i)
            {
                dst[(dstChannel * bytesPerSample) + i] = src[(srcChannel * bytesPerSample) + i];
            }
        }

        private byte[] ConvertValues(byte[] values, int bytesPerSample)
        {
            byte[] returnVal = new byte[bytesPerSample * 28];
            switch (SelectTemplate)
            {
                case "Printemps":
                    CopyToChannel(returnVal, 0, values, 0, bytesPerSample);
                    CopyToChannel(returnVal, 3, values, 1, bytesPerSample);
                    CopyToChannel(returnVal, 18, values, 2, bytesPerSample);
                    CopyToChannel(returnVal, 19, values, 3, bytesPerSample);
                    break;

                case "Été":
                    CopyToChannel(returnVal, 4, values, 0, bytesPerSample);
                    CopyToChannel(returnVal, 7, values, 1, bytesPerSample);
                    CopyToChannel(returnVal, 21, values, 2, bytesPerSample);
                    CopyToChannel(returnVal, 22, values, 3, bytesPerSample);
                    break;

                case "Automne":
                    CopyToChannel(returnVal, 8, values, 0, bytesPerSample);
                    CopyToChannel(returnVal, 11, values, 1, bytesPerSample);
                    CopyToChannel(returnVal, 24, values, 2, bytesPerSample);
                    CopyToChannel(returnVal, 25, values, 3, bytesPerSample);
                    break;

                case "Hiver":
                    CopyToChannel(returnVal, 12, values, 0, bytesPerSample);
                    CopyToChannel(returnVal, 16, values, 1, bytesPerSample);
                    CopyToChannel(returnVal, 26, values, 2, bytesPerSample);
                    CopyToChannel(returnVal, 27, values, 3, bytesPerSample);
                    break;
            }

            return returnVal;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class DelegateCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        private readonly Action _Action;

        public DelegateCommand(Action a)
        {
            _Action = a;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            _Action();
        }
    }
}