using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PT_lab5
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private int highestPercentageReached = 0;
        public MainWindow()
        {
            InitializeComponent();


        }

        private int[] ParseNBInput()
        {
            int[] output = new int[2];
            if (NValueTextBox.Text != "" && KValueTextBox.Text != "")
            {
                try
                {
                    output[0] = int.Parse(NValueTextBox.Text);
                    output[1] = int.Parse(KValueTextBox.Text);
                }catch (FormatException e){
                    output[0] = -1;
                    output[1] = -1;
                }
            }
            return output;
        }

        private void TasksButton_Click(object sender, RoutedEventArgs e)
        {
            int[] data = ParseNBInput();

            int N = data[0];
            int K = data[1];

            if (N == -1 || K == -1)
            {
                TasksOutTextBox.Text = "Incorrect input data";
            }

            else if (N >= K)
            {

                BinomialCoef binomialCoeff = new BinomialCoef(N, K);
                int result = binomialCoeff.CalculateByTask();

                TasksOutTextBox.Text = result.ToString();

            }
            else
            {
                TasksOutTextBox.Text = "N < K";
            }
            
        }

        private void DelegatesButton_Click(object sender, RoutedEventArgs e)
        {
            int[] data = ParseNBInput();

            int N = data[0];
            int K = data[1];

            if (N == -1 || K == -1)
            {
                DelegatestTextBox.Text = "Incorrect input data";
            }

            else if (N >= K)
            {
                BinomialCoef binomialCoeff = new BinomialCoef(N, K);
                int result = binomialCoeff.CalculateByDelegate();

                DelegatestTextBox.Text = result.ToString();
            }
            else
            {
                DelegatestTextBox.Text = "N < K";
            }

        }

        private async void AsyncAwaitButton_Click(object sender, RoutedEventArgs e)
        {
            int[] data = ParseNBInput();

            int N = data[0];
            int K = data[1];

            if (N == -1 || K == -1)
            {
                AsyncAwaitTextBox.Text = "Incorrect input data";
            }

            else if (N >= K)
            {
                BinomialCoef binomialCoeff = new BinomialCoef(N, K);
                int result = await binomialCoeff.CalculateAsync();

                AsyncAwaitTextBox.Text = result.ToString();
            } 
            else
            {
                AsyncAwaitTextBox.Text = "N < K";
            }
        }

        private void FibGetButton_Click(object sender, RoutedEventArgs e)
        {
            if (iFibTextBox.Text != "")
            {
                try
                {
                    int i = int.Parse(iFibTextBox.Text);
                    
                    // if i > 91 - overflow
                    if (i >= 0 && i <= 91)
                    {
                        BackgroundWorker fibBW = new BackgroundWorker();
                        fibBW.DoWork += fibWorkerDoWork;
                        fibBW.ProgressChanged += ((object senderprim, ProgressChangedEventArgs args) => {
                            FibProgressBar.Value = args.ProgressPercentage;
                        });
                        fibBW.RunWorkerCompleted += ((object senderprim, RunWorkerCompletedEventArgs args) => {
                            iFibOutTextBox.Text = args.Result.ToString();
                        });

                        fibBW.WorkerReportsProgress = true;
                        highestPercentageReached = 0;
                        fibBW.RunWorkerAsync(i);
                    }
                    else {
                        iFibOutTextBox.Text = "0 <= i <= 91";
                    }
                }
                catch (FormatException)
                {
                    iFibOutTextBox.Text = "Incorrect input data";
                }

            }
            
        }

        private void fibWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            e.Result = ComputeFibonacci((int)e.Argument, worker, e);
        }


        long ComputeFibonacci(int n, BackgroundWorker worker, DoWorkEventArgs e)
        {
            long result = 0;

            if (worker.CancellationPending)
            {
                e.Cancel = true;
            }
            else
            {

                if (n == 0)
                {
                    return 0;
                }

                if (n == 1)
                {
                    return 1;
                }

                long num1 = 0;
                long num2 = 1;
                result = 0;

                for (int i=2; i <= n; i++)
                {
 
                    result = num1 + num2;
                    num1 = num2;
                    num2 = result;

                    // Progress report
                    int percentComplete = (int)((float)i/(float)n * 100);
                    if (percentComplete > highestPercentageReached)
                    {
                        highestPercentageReached = percentComplete;
                        worker.ReportProgress(percentComplete);
                    }

                    Thread.Sleep(20);

                }

            }

            return result;
        }

        private void CompressButton_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new FolderBrowserDialog() { Description = "Select directory to compress" };
            var dialogResult = dlg.ShowDialog();
            if (dialogResult == System.Windows.Forms.DialogResult.OK)
            {
                var path = dlg.SelectedPath;
                var dir = new DirectoryInfo(path);

                Parallel.ForEach<FileInfo>(dir.GetFiles(), d =>
                {
                    var fullPath = d.FullName;
                    var fullPathCompressed = fullPath + ".gz";

                    Compression.Compress(fullPath, fullPathCompressed);

                });

                showMessageBox("Compression completed");

            }
        }

        private void DecompressButton_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new FolderBrowserDialog() { Description = "Select directory to decompress" };
            var dialogResult = dlg.ShowDialog();
            if (dialogResult == System.Windows.Forms.DialogResult.OK)
            {
                var path = dlg.SelectedPath;
                var dir = new DirectoryInfo(path);

                Parallel.ForEach<FileInfo>(dir.GetFiles(), d =>
                {
                    if (d.Extension == ".gz")
                    {
                        var fullPathCompressed = d.FullName;
                        var fullPath = System.IO.Path.ChangeExtension(fullPathCompressed, null);

                        Compression.Decompress(fullPathCompressed, fullPath);
                    }
                });

                showMessageBox("Deompression completed");

            }
        }


        private void DNSResolveButton_Click(object sender, RoutedEventArgs e)
        {
            string[] hostNames = { "www.microsoft.com", "www.apple.com",
                                    "www.google.com", "www.ibm.com", "cisco.netacad.net",
                                    "www.oracle.com", "www.nokia.com", "www.hp.com", "www.dell.com",
                                    "www.samsung.com", "www.toshiba.com", "www.siemens.com",
                                    "www.amazon.com", "www.sony.com", "www.canon.com", "www.alcatel-lucent.com", "www.acer.com", "www.motorola.com" 
            };


            var ipAddresses = from elem in hostNames.AsParallel()
                              select 
                              new {
                                  host = elem,
                                  ip = Dns.GetHostAddresses(elem).Last().ToString()
                              };

            foreach (var data in ipAddresses)
            {
                DNSOutTextBox.Text += data.host + " => " + data.ip + "\n";
            }

        }



        private static void showMessageBox(string text)
        {
            string message = text;
            string caption = "Notification";
            MessageBoxButtons buttons = MessageBoxButtons.OK;
            System.Windows.MessageBox.Show(message, caption, (MessageBoxButton)buttons);
        }


    }
}
