using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
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
using System.Windows.Threading;

namespace HT
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private PFC laitteisto = new PFC();

        CancellationTokenSource cTokenSource = new CancellationTokenSource();

        private List<TextBox> textBoxes = new List<TextBox>();
        //private List<Tuple<int,int>> paramsLimits = new List<Tuple<int,int>>();
        private Dictionary<string, ProgressBar> progressbars = new Dictionary<string, ProgressBar>();

        /// <summary>
        /// Constructor for MainWindow
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            laitteisto.ProcessItemsChanged_PFC += ProcessItemsChanged;

            progressbars["LI100"] = LI100_prog;
            progressbars["LI200"] = LI200_prog;
            progressbars["LI400"] = LI400_prog;
            progressbars["PI300"] = PI300_prog;
            progressbars["TI300"] = TI300_prog;

            textBoxes.Add(Cooking_temp_value);
            textBoxes.Add(Cooking_press_value);
            textBoxes.Add(Cooking_time_value);
            textBoxes.Add(Impregnation_time_value);
        }
        /// <summary>
        /// Validation method, which sets textbox item to take
        /// numbers and decimal separator "." only
        /// </summary>
        /// <param name="source"> The object which sends the event </param>
        /// <param name="args"> The custom event arguments </param>
        private void NumberValidationTextBox(object source, TextCompositionEventArgs args)
        {
            var regex = new Regex(@"^[0-9]*(?:\.[0-9]*)?$");
            if (regex.IsMatch(args.Text) && !(args.Text == "." && ((TextBox)source).Text.Contains(args.Text)))
                args.Handled = false;

            else
                args.Handled = true;
        }
        /// <summary>
        /// Event handler to handle process item changed events
        /// </summary>
        /// <param name="source"> The object which sends the event </param>
        /// <param name="args"> The custom event arguments </param>
        private void ProcessItemsChanged(object source, ProcessItemsChangedEventArgs args)
        {
            Dispatcher.BeginInvoke((Action)(() => UpdateStatus(args.ProcessItemID, args.ProcessItemValue)));
        }
        /// <summary>
        /// Method updates the UI elements based on which process items value
        /// is changed
        /// </summary>
        /// <param name="id"> ID of the UI element </param>
        /// <param name="value"> New value for the UI element </param>
        private void UpdateStatus(string id, string value)
        {
            try
            {
                if (progressbars.ContainsKey(id))
                {
                    progressbars[id].Value = Math.Round(Double.Parse(value), 2 , MidpointRounding.ToEven);
                }
                else if (id == "connectionStatus")
                {
                    Connection_Status_Value.Text = value;
                }
            }
            catch (Exception e)
            {
                // ...
            }
        }
        /// <summary>
        /// Locks the elements which are used to change the process
        /// parameters values
        /// </summary>
        private void disableProcessParams()
        {
            this.Dispatcher.Invoke(() =>
            {
                SetValuesButton.IsEnabled = false;
                StartButton.IsEnabled = false;
                foreach (var textBox in textBoxes)
                {
                    textBox.IsReadOnly = true;
                }
            });

        }
        /// <summary>
        /// Unlocks the elements which are used to change the process
        /// parameters values
        /// </summary>
        private void enableProcessParams()
        {
            this.Dispatcher.Invoke(() =>
            {
                SetValuesButton.IsEnabled = true;
                StartButton.IsEnabled = true;
                foreach (var textBox in textBoxes)
                {
                    textBox.IsReadOnly = false;
                }
            });
        }
        /// <summary>
        /// Event handler for the connect button click
        /// </summary>
        /// <param name="source">  The object which send the event </param>
        /// <param name="args"> The event arguments </param>
        private async void ConnectButton_Click(object source, RoutedEventArgs args)
        {
            try
            {
                await Task.Run(() =>
                {
                    laitteisto.connect_PFC();
                });

            }
            catch(Exception exp){
                
            }

        }
        /// <summary>
        /// Event handler for the disconnect button click
        /// </summary>
        /// <param name="source">  The object which send the event </param>
        /// <param name="args"> The event arguments </param>
        private async void DisconnectButton_Click(object source, RoutedEventArgs args)
        {
            try
            {
                await Task.Run(() =>
                {
                    laitteisto.disconnect();
                });
            }
            catch(Exception exp) 
            {

            }
        }
        /// <summary>
        /// Event handler for the start button click
        /// </summary>
        /// <param name="source">  The object which send the event </param>
        /// <param name="args"> The event arguments </param>
        private async void StartButton_Click(object source, RoutedEventArgs args)
        {
            try
            {
                await Task.Run(async () =>
                {
                    cTokenSource.Dispose();
                    cTokenSource = new CancellationTokenSource();
                    disableProcessParams();
                    this.Dispatcher.Invoke(() =>
                    {
                        Process_Status_Value.Text = "Process in progress...";
                    });
                    await laitteisto.mppProcedureSequence(cTokenSource.Token);
                    this.Dispatcher.Invoke(() =>
                    {
                        Process_Status_Value.Text = "Process complited";
                    });
                    Thread.Sleep(2000);
                    this.Dispatcher.Invoke(() =>
                    {
                        Process_Status_Value.Text = "No process running";
                    });
                    enableProcessParams();
                });
            }
            catch (OperationCanceledException exp)
            {
                await Task.Run(() =>
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        Process_Status_Value.Text = "Process canceled";
                    });
                    laitteisto.stopMppProcedureSequence();
                    Thread.Sleep(2000);
                    this.Dispatcher.Invoke(() =>
                    {
                        Process_Status_Value.Text = "No process running";
                    });
                    enableProcessParams();
                });
            }

            catch(Exception exp) 
            {
                await Task.Run(() =>
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        Process_Status_Value.Text = "Cannot run process";
                    });
                    Thread.Sleep(2000);
                    this.Dispatcher.Invoke(() =>
                    {
                        Process_Status_Value.Text = "No process running";
                    });
                    enableProcessParams();
                });
            }
        }
        /// <summary>
        /// Event handler for the stop button click
        /// </summary>
        /// <param name="source">  The object which send the event </param>
        /// <param name="args"> The event arguments </param>
        private void StopButton_Click(object source, RoutedEventArgs args)
        {
            try
            {
                cTokenSource.Cancel();
            }
            catch (Exception exp) { } 
        }
        /// <summary>
        /// Event handler for the set value button click
        /// </summary>
        /// <param name="source">  The object which send the event </param>
        /// <param name="args"> The event arguments </param>
        private async void SetValuesButton_Click(object source, RoutedEventArgs args)
        {
            try
            {
                double cookingTemp = Double.Parse(textBoxes[0].Text, System.Globalization.CultureInfo.InvariantCulture);
                int cookingPress = Int32.Parse(textBoxes[1].Text);
                int cookingTime = Int32.Parse(textBoxes[2].Text);
                int impregnationTime = Int32.Parse(textBoxes[3].Text);

                await Task.Run(() =>
                {
                    laitteisto.setProcessParameters(cookingTemp, cookingPress,
                                                    cookingTime, impregnationTime);
                    this.Dispatcher.Invoke(() =>
                    {
                        Parameter_Status.Text = $"T:{cookingTemp} Pc:{cookingPress} Tc:{cookingTime} Ti:{impregnationTime}";
                    });
                    Thread.Sleep(2000);
                    this.Dispatcher.Invoke(() =>
                    {
                        Parameter_Status.Text = "";
                    });
                });
            }
            catch (Exception exp)
            {
                await Task.Run(() =>
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        Parameter_Status.Text = "Invalid parameters.";
                    });
                    Thread.Sleep(2000);
                    this.Dispatcher.Invoke(() =>
                    {
                        Parameter_Status.Text = "";
                    });
                });
            }
        }
        /// <summary>
        /// Main window close event handler
        /// </summary>
        /// <param name="source">  The object which send the event </param>
        /// <param name="args"> The event arguments </param>
        private void CloseCommandHandler(object source, ExecutedRoutedEventArgs args)
        {
            try
            {
                Task.Run(() => {
                    laitteisto.stopMppProcedureSequence();
                    laitteisto.disconnect();
                });
            }
            catch(Exception exp) { }
            finally { this.Close(); }
        }
    }
}
