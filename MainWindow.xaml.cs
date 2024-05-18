using NAudio.Wave;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace SymmetricalJourney
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        const int NUMBER_OF_CHANNELS = 26;
        ObservableCollection<string> _Collection;
        private string _InputFile;
        private string _DestinationFolder;
        private string _SelectedTemplate;

        public MainWindow()
        {
            _InputFile = string.Empty;
            _DestinationFolder = string.Empty;
            _SelectedTemplate = string.Empty;

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

        public string InputFile
        {
            get => _InputFile;
            set
            {
                _InputFile = value;
                OnPropertyChanged();
            }
        }

        public string DestinationFolder
        {
            get => _DestinationFolder;
            set
            {
                _DestinationFolder = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> Collection
        {
            get { return _Collection; }
        }

        public string SelectedTemplate
        {
            get => _SelectedTemplate;
            set
            {
                _SelectedTemplate = value;
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
                    InputFile = dialog.FileName;
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
                        DestinationFolder = dialog.SelectedPath;
                    }
                }
            });
        }

        public ICommand Execute
        {
            get => new DelegateCommand(delegate
            {
                if (!File.Exists(InputFile))
                {
                    MessageBox.Show($"Fichier {InputFile} n'existe pas", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else if (!Directory.Exists(DestinationFolder))
                {
                    MessageBox.Show($"Dossier {DestinationFolder} n'existe pas", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else if(string.IsNullOrEmpty(SelectedTemplate))
                {
                    MessageBox.Show("Saison pas selectionné", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    ConvertFile(InputFile, DestinationFolder, SelectedTemplate);
                }
            });
        }

        private static void ConvertFile(string inputFile, string destination, string template)
        {
            using (WaveFileReader reader = new WaveFileReader(inputFile))
            {
                int bytesPerSample = reader.WaveFormat.BitsPerSample / 8;

                WaveFormat newFormat = new WaveFormat(reader.WaveFormat.SampleRate, reader.WaveFormat.BitsPerSample, NUMBER_OF_CHANNELS);
                string path = Path.Combine(destination, Path.GetFileName(Path.ChangeExtension(inputFile, ".modifie.wav")));
                using (WaveFileWriter writer = new WaveFileWriter(path, newFormat))
                {
                    int currentOffset = 0;
                    while (reader.Position < reader.Length)
                    {
                        byte[] data = new byte[bytesPerSample * reader.WaveFormat.Channels];
                        reader.Read(data, 0, data.Length);

                        currentOffset += data.Length;

                        ReadOnlySpan<byte> convertedValues = ConvertValues(template, data, bytesPerSample, reader.WaveFormat.Channels);
                        writer.Write(convertedValues);
                    }
                }
            }
        }

        private static void CopyToChannel(byte[] dst, int dstChannel, byte[] src, int srcChannel, int bytesPerSample)
        {
            for(int i = 0; i < bytesPerSample; ++i)
            {
                dst[(dstChannel * bytesPerSample) + i] = src[(srcChannel * bytesPerSample) + i];
            }
        }

        private static void ConvertValuesForSaison(byte[] srcValues, byte[] dstValues, int srcChannelCount, int[] dstChannelConfig, int bytesPerSample)
        {
            if (srcChannelCount == 2)
            {
                CopyToChannel(dstValues, dstChannelConfig[0], srcValues, 0, bytesPerSample);
                CopyToChannel(dstValues, dstChannelConfig[1], srcValues, 1, bytesPerSample);
                CopyToChannel(dstValues, dstChannelConfig[2], srcValues, 1, bytesPerSample);
                CopyToChannel(dstValues, dstChannelConfig[3], srcValues, 0, bytesPerSample);
            }
            else if (srcChannelCount == 3)
            {
                CopyToChannel(dstValues, dstChannelConfig[0], srcValues, 0, bytesPerSample);
                CopyToChannel(dstValues, dstChannelConfig[1], srcValues, 1, bytesPerSample);
                CopyToChannel(dstValues, dstChannelConfig[2], srcValues, 1, bytesPerSample);
                CopyToChannel(dstValues, dstChannelConfig[3], srcValues, 0, bytesPerSample);
            }
            else if (srcChannelCount == 4)
            {
                CopyToChannel(dstValues, dstChannelConfig[0], srcValues, 0, bytesPerSample);
                CopyToChannel(dstValues, dstChannelConfig[1], srcValues, 1, bytesPerSample);
                CopyToChannel(dstValues, dstChannelConfig[2], srcValues, 2, bytesPerSample);
                CopyToChannel(dstValues, dstChannelConfig[3], srcValues, 3, bytesPerSample);
            }
            else if (srcChannelCount == 5)
            {
                CopyToChannel(dstValues, dstChannelConfig[0], srcValues, 0, bytesPerSample);
                CopyToChannel(dstValues, dstChannelConfig[1], srcValues, 1, bytesPerSample);
                CopyToChannel(dstValues, dstChannelConfig[2], srcValues, 3, bytesPerSample);
                CopyToChannel(dstValues, dstChannelConfig[3], srcValues, 4, bytesPerSample);
            }
            else if (srcChannelCount == 6)
            {
                CopyToChannel(dstValues, dstChannelConfig[0], srcValues, 0, bytesPerSample);
                CopyToChannel(dstValues, dstChannelConfig[1], srcValues, 1, bytesPerSample);
                CopyToChannel(dstValues, dstChannelConfig[2], srcValues, 4, bytesPerSample);
                CopyToChannel(dstValues, dstChannelConfig[3], srcValues, 5, bytesPerSample);
            }
        }

        private static byte[] ConvertValues(string template, byte[] srcValues, int bytesPerSample, int srcChannelCount)
        {
            byte[] dstValues = new byte[bytesPerSample * NUMBER_OF_CHANNELS];
            switch (template)
            {
                case "Été":
                    ConvertValuesForSaison(srcValues, dstValues, srcChannelCount, [0, 3, 18, 19], bytesPerSample);
                    break;

                case "Automne":
                    ConvertValuesForSaison(srcValues, dstValues, srcChannelCount, [4, 7, 20, 21], bytesPerSample);
                    break;

                case "Hiver":
                    ConvertValuesForSaison(srcValues, dstValues, srcChannelCount, [10, 13, 22, 23], bytesPerSample);
                    break;

                case "Printemps":
                    ConvertValuesForSaison(srcValues, dstValues, srcChannelCount, [14, 17, 24, 25], bytesPerSample);
                    break;
            }

            return dstValues;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            if(string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

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

        protected void OnPropertyChanged()
        {
            CanExecuteChanged?.Invoke(this, new EventArgs());
        }
    }
}