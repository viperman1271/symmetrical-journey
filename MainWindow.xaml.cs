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
                        int bytesPerSample = reader.WaveFormat.BitsPerSample / 8;

                        WaveFormat newFormat = new WaveFormat(reader.WaveFormat.SampleRate, reader.WaveFormat.BitsPerSample, NUMBER_OF_CHANNELS);
                        using(WaveFileWriter writer = new WaveFileWriter(Path.ChangeExtension(File, ".modifie.wav"), newFormat))
                        {
                            int currentOffset = 0;
                            while(reader.Position < reader.Length)
                            {
                                byte[] data = new byte[bytesPerSample * reader.WaveFormat.Channels];
                                reader.Read(data, 0, data.Length);

                                currentOffset += data.Length;

                                ReadOnlySpan<byte> convertedValues = ConvertValues(data, bytesPerSample, reader.WaveFormat.Channels);
                                writer.Write(convertedValues);
                            }
                        }
                    }
                }
            });
        }

        private void CopyToChannel(byte[] dst, int dstChannel, byte[] src, int srcChannel, int bytesPerSample)
        {
            for(int i = 0; i < bytesPerSample; ++i)
            {
                dst[(dstChannel * bytesPerSample) + i] = src[(srcChannel * bytesPerSample) + i];
            }
        }

        private void ConvertValuesForSaison(byte[] srcValues, byte[] dstValues, int srcChannelCount, int[] dstChannelConfig, int bytesPerSample)
        {
            if (srcChannelCount == 2)
            {
                CopyToChannel(dstValues, 0, srcValues, 0, bytesPerSample);
                CopyToChannel(dstValues, 3, srcValues, 1, bytesPerSample);
                CopyToChannel(dstValues, 18, srcValues, 1, bytesPerSample);
                CopyToChannel(dstValues, 19, srcValues, 0, bytesPerSample);
            }
            else if (srcChannelCount == 3)
            {
                CopyToChannel(dstValues, 0, srcValues, 0, bytesPerSample);
                CopyToChannel(dstValues, 3, srcValues, 1, bytesPerSample);
                CopyToChannel(dstValues, 18, srcValues, 1, bytesPerSample);
                CopyToChannel(dstValues, 19, srcValues, 0, bytesPerSample);
            }
            else if (srcChannelCount == 4)
            {
                CopyToChannel(dstValues, 0, srcValues, 0, bytesPerSample);
                CopyToChannel(dstValues, 3, srcValues, 1, bytesPerSample);
                CopyToChannel(dstValues, 18, srcValues, 2, bytesPerSample);
                CopyToChannel(dstValues, 19, srcValues, 3, bytesPerSample);
            }
            else if (srcChannelCount == 5)
            {
                CopyToChannel(dstValues, 0, srcValues, 0, bytesPerSample);
                CopyToChannel(dstValues, 3, srcValues, 1, bytesPerSample);
                CopyToChannel(dstValues, 18, srcValues, 3, bytesPerSample);
                CopyToChannel(dstValues, 19, srcValues, 4, bytesPerSample);
            }
            else if (srcChannelCount == 6)
            {
                CopyToChannel(dstValues, 0, srcValues, 0, bytesPerSample);
                CopyToChannel(dstValues, 3, srcValues, 1, bytesPerSample);
                CopyToChannel(dstValues, 18, srcValues, 4, bytesPerSample);
                CopyToChannel(dstValues, 19, srcValues, 5, bytesPerSample);
            }
        }

        private byte[] ConvertValues(byte[] srcValues, int bytesPerSample, int srcChannelCount)
        {
            byte[] dstValues = new byte[bytesPerSample * NUMBER_OF_CHANNELS];
            switch (SelectTemplate)
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