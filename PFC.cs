using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HT
{
    /// <summary>
    /// The PFC a.k.a  Procedural Function Chart class
    /// </summary>
    public class PFC
    {
        public event EventHandler<ProcessItemsChangedEventArgs> ProcessItemsChanged_PFC;
        public BatchProcessController laitteet = new BatchProcessController();

        private PI_Controller piController = new PI_Controller(100, 0, 100);
        private Stopwatch s = new Stopwatch();

        public int cookingTime;
        public int impregnationTime;
        public int cookingPress;
        public double cookingTemp;
        public bool connected;
        /// <summary>
        /// The constructer of the PFC class
        /// </summary>
        public PFC()
        {
            laitteet.ProcessItemsChanged_BPC += ProcessItemsChanged;
        }
        /// <summary>
        /// The handler of the process item changed event
        /// </summary>
        /// <param name="source"> The object which sends the event </param>
        /// <param name="args"> The custom event arguments </param>
        private void ProcessItemsChanged(object source, ProcessItemsChangedEventArgs args)
        {
            if (args.ProcessItemID == "connectionStatus")
            {
                if (args.ProcessItemValue == "Connected")
                {
                    connected = true;
                }
                else
                {
                    connected = false;
                }
            }
            ProcessItemsChanged_PFC(this, args);
        }
        /// <summary>
        /// The method for setting new system item values
        /// </summary>
        /// <param name="cookingTemp"> The wanted cooking temperature </param>
        /// <param name="cookingPress"> The wanted cooking pressure </param>
        /// <param name="cookingTime"> The wanted cooking time </param>
        /// <param name="impregnationTime"> The wanted impregnation time </param>
        public void setProcessParameters(double cookingTemp, int cookingPress, int cookingTime, int impregnationTime)
        {
            if (cookingTemp <= 0.0 || cookingPress <= 0 || cookingTime <= 0 || impregnationTime <= 0)
            {
                throw new ArgumentException("Parameters cannot be zero or less.");
            }
            else if (cookingTemp > 25.0 || cookingPress > 250 || cookingTemp < 23.0 || cookingPress < 100)
            {
                throw new ArgumentOutOfRangeException("Cooking pressure or/and temperature out of range.");
            }
            else
            {
                this.cookingTemp = cookingTemp;
                this.cookingPress = cookingPress;
                this.cookingTime = cookingTime;
                this.impregnationTime = impregnationTime;
            }
        }
        /// <summary>
        /// The method to connect mppClient through batch process controller class
        /// </summary>
        public void connect_PFC()
        {
            try
            {
                laitteet.Connect("opc.tcp://127.0.0.1:8087");
            }
            catch(Exception e)
            {
                throw;
            }
        }
        /// <summary>
        /// The method to disconnect mppClient through batch process controller class
        /// </summary>
        public void disconnect()
        {
            try
            {
                laitteet.Dispose();
            }
            catch (Exception e)
            {
                throw;
            }
            
        }
        /// <summary>
        ///  Timer
        /// </summary>
        /// <param name="millisecs"></param>
        private void timer(int millisecs)
        {
            System.Threading.Thread.Sleep(millisecs);
        }
        /// <summary>
        /// The method equal of the EM1_OP1 Procedural Function Chart
        /// </summary>
        private void EM1_OP1()
        {
            laitteet.regulationValveControl("V102", 100);
            laitteet.on_off("V304", true);
            laitteet.on_off("E100", true);
            if (laitteet.getIntSystemVar("LI100") >= 100)
            {
                laitteet.pumpControl("P100", 100);         
            }
        }
        /// <summary>
        /// The method equal of the EM1_OP2 Procedural Function Chart
        /// </summary>
        private void EM1_OP2()
        {
            laitteet.regulationValveControl("V102", 100);
            laitteet.on_off("V304", true);
            if (laitteet.getIntSystemVar("LI100") >= 100)
            {
                laitteet.pumpControl("P100", 100);
            }
        }
        /// <summary>
        /// The method equal of the EM1_OP3 Procedural Function Chart
        /// </summary>
        private void EM1_OP3()
        {
            laitteet.regulationValveControl("V102", 0);
            laitteet.on_off("V304", false);
            laitteet.pumpControl("P100", 0);
            laitteet.on_off("E100", false);
        }
        /// <summary>
        /// The method equal of the EM1_OP4 Procedural Function Chart
        /// </summary>
        private void EM1_OP4()
        {
            laitteet.regulationValveControl("V102", 0);
            laitteet.on_off("V304", false);
            laitteet.pumpControl("P100", 0);
        }
        /// <summary>
        /// The method equal of the EM2_OP1 Procedural Function Chart
        /// </summary>
        private void EM2_OP1()
        {
            laitteet.on_off("V201", true);
        }
        /// <summary>
        /// The method equal of the EM2_OP2 Procedural Function Chart
        /// </summary>
        private void EM2_OP2()
        {
            laitteet.on_off("V201", false);
        }
        /// <summary>
        /// The method equal of the EM3_OP1 Procedural Function Chart
        /// </summary>
        private void EM3_OP1()
        {
            laitteet.regulationValveControl("V104", 0);
            laitteet.on_off("V204", false);
            laitteet.on_off("V401", false);
        }
        /// <summary>
        /// The method equal of the EM3_OP2 Procedural Function Chart
        /// </summary>
        private void EM3_OP2()
        {
            laitteet.on_off("V204", true);
            laitteet.on_off("V301", true);
        }
        /// <summary>
        /// The method equal of the EM3_OP3 Procedural Function Chart
        /// </summary>
        private void EM3_OP3()
        {
            laitteet.on_off("V301", true);
            laitteet.on_off("V401", true);
        }
        /// <summary>
        /// The method equal of the EM3_OP4 Procedural Function Chart
        /// </summary>
        private void EM3_OP4()
        {
            laitteet.regulationValveControl("V104", 100);
            laitteet.on_off("V301", true);
        }
        /// <summary>
        /// The method equal of the EM3_OP5 Procedural Function Chart
        /// </summary>
        private void EM3_OP5()
        {
            laitteet.on_off("V204", true);
            laitteet.on_off("V302", true);
        }
        /// <summary>
        /// The method equal of the EM3_OP6 Procedural Function Chart
        /// </summary>
        private void EM3_OP6()
        {
            laitteet.regulationValveControl("V104", 0);
            laitteet.on_off("V204", false);
            laitteet.on_off("V301", false);
            laitteet.on_off("V401", false);
        }
        /// <summary>
        /// The method equal of the EM3_OP7 Procedural Function Chart
        /// </summary>
        private void EM3_OP7()
        {
            laitteet.on_off("V302", false);
            laitteet.on_off("V204", false);
        }
        /// <summary>
        /// The method equal of the EM3_OP8 Procedural Function Chart
        /// </summary>
        private void EM3_OP8()
        {
            laitteet.on_off("V204", true);
            timer(1000);
            laitteet.on_off("V204", false);
        }
        /// <summary>
        /// The method equal of the EM4_OP1 Procedural Function Chart
        /// </summary>
        private void EM4_OP1()
        {
            laitteet.on_off("V404", true);
        }
        /// <summary>
        /// The method equal of the EM4_OP2 Procedural Function Chart
        /// </summary>
        private void EM4_OP2()
        {
            laitteet.on_off("V404", false);
        }
        /// <summary>
        /// The method equal of the EM5_OP1 Procedural Function Chart
        /// </summary>
        private void EM5_OP1()
        {
            laitteet.on_off("V303", true);
            laitteet.pumpControl("P200", 100);
        }
        /// <summary>
        /// The method equal of the EM5_OP2 Procedural Function Chart
        /// </summary>
        private void EM5_OP2()
        {
            laitteet.on_off("V103", true);
            laitteet.on_off("V303", true);
            laitteet.pumpControl("P200", 100);
        }
        /// <summary>
        /// The method equal of the EM5_OP3 Procedural Function Chart
        /// </summary>
        private void EM5_OP3()
        {
            laitteet.on_off("V303", false);
            laitteet.pumpControl("P200", 0);
        }
        /// <summary>
        /// The method equal of the EM5_OP4 Procedural Function Chart
        /// </summary>
        private void EM5_OP4()
        {
            laitteet.on_off("V103", false);
            laitteet.on_off("V303", false);
            laitteet.pumpControl("P200", 0);
        }
        /// <summary>
        /// The method equal of the U1_OP1 Procedural Function Chart
        /// </summary>
        private void U1_OP1(int cookingPress)
        {
            piController.goal = cookingPress;
            //piController.controllerMinLimit = 0;
            //piController.controllerMaxLimit = 100;
            //Starting pressure reculation

            var control = piController.getControlChange(laitteet.getIntSystemVar("PI300"));
            laitteet.regulationValveControl("V104", control);
            Console.WriteLine(control);
        }
        /// <summary>
        /// The method equal of the U1_OP2 Procedural Function Chart
        /// </summary>
        private void U1_OP2(double T)
        {
            //Starting temperature reculation
            if (laitteet.getDoubleSystemVar("TI300") < T)
            {
                laitteet.on_off("E100", true);
            }
            else if (laitteet.getDoubleSystemVar("TI300") >= T)
            {
                laitteet.on_off("E100", false);
            }
        }
        /// <summary>
        /// The method equal of the U1_OP3 Procedural Function Chart
        /// </summary>
        private void U1_OP3()
        {
            //Stopping pressure reculation
            laitteet.regulationValveControl("V104", 0);
        }
        /// <summary>
        /// The method equal of the U1_OP4 Procedural Function Chart
        /// </summary>
        private void U1_OP4()
        {
            //Stopping heath reculation
            laitteet.on_off("E100", false);
        }
        /// <summary>
        /// The method equal of the impregnation Procedural Function Chart
        /// </summary>
        /// <param name="cToken"> Cancellation token </param>
        /// <returns> Returns the impregnation task </returns>
        private async Task impregnationSequence(CancellationToken cToken)
        {
            await Task.Run(() =>
            {
                while (!cToken.IsCancellationRequested && connected)
                {
                    Console.WriteLine("Starting sequence...");
                    laitteet.on_off("P100_P200_PRESET", true);
                    EM2_OP1();
                    EM5_OP1();
                    EM3_OP2();
                    Console.WriteLine("Opening valves and started the pump...");
                    while (!laitteet.getBoolSystemVar("LS+300") && connected)
                    {
                        if (cToken.IsCancellationRequested)
                        {
                            cToken.ThrowIfCancellationRequested();
                        }
                        Console.WriteLine("Filling up the tank...");
                        timer(1000);
                    }
                    if (cToken.IsCancellationRequested)
                    {
                        cToken.ThrowIfCancellationRequested();
                    }
                    Console.WriteLine("Tanks full!");
                    Console.WriteLine("Closing outlets...");
                    EM3_OP1();
                    Console.WriteLine("Pressurizing......");
                    timer(impregnationTime * 1000);
                    Console.WriteLine("Pressurized!");
                    Console.WriteLine("Closing valves and shutting down the pump...");
                    EM2_OP2();
                    EM5_OP3();
                    EM3_OP6();
                    EM3_OP8();
                    Console.WriteLine("The impregnation sequence is completed!");
                    laitteet.on_off("P100_P200_PRESET", false);
                    break;
                }
            }, cToken);
        }
        /// <summary>
        /// The method equal of the black liquor fill Procedural Function Chart
        /// </summary>
        /// <param name="cToken"> Cancellation token </param>
        /// <returns> Returns the black liquor fill task </returns>
        private async Task blackLiquorFillSequence(CancellationToken cToken)
        {
            await Task.Run(() =>
            {
                while (!cToken.IsCancellationRequested && connected)
                {
                    Console.WriteLine("Starting black liquor fill sequence...");
                    laitteet.on_off("P100_P200_PRESET", true);
                    EM3_OP2();
                    EM5_OP1();
                    EM4_OP1();
                    Console.WriteLine("Opening valves and started the pump...");
                    while (laitteet.getIntSystemVar("LI400") >= 35 && connected)
                    {
                        if (cToken.IsCancellationRequested)
                        {
                            cToken.ThrowIfCancellationRequested();
                        }
                        Console.WriteLine("Displacing liquor...");
                        Console.WriteLine(laitteet.getIntSystemVar("LI400"));
                        timer(1000);
                    }
                    if (cToken.IsCancellationRequested)
                    {
                        cToken.ThrowIfCancellationRequested();
                    }
                    Console.WriteLine("Enough liquor displaced!");
                    Console.WriteLine("Closing valves and shutting down the pump...");
                    EM3_OP6();
                    EM5_OP3();
                    EM4_OP2();
                    Console.WriteLine("The black liquor fill sequence is completed!");
                    laitteet.on_off("P100_P200_PRESET", false);
                    break;
                }
            }, cToken);
        }
        /// <summary>
        /// The method equal of the white liquor fill Procedural Function Chart
        /// </summary>
        /// <param name="cToken"> Cancellation token </param>
        /// <returns> Returns the white liquor fill task </returns>
        private async Task whiteLiquorFillSequence(CancellationToken cToken)
        {
            await Task.Run(() =>
            {
                while (!cToken.IsCancellationRequested && connected)
                {
                    Console.WriteLine("Starting white liquor fill sequence...");
                    laitteet.on_off("P100_P200_PRESET", true);
                    EM3_OP3();
                    EM1_OP2();
                    Console.WriteLine("Opening valves and started the pump...");
                    while (laitteet.getIntSystemVar("LI400") <= 80 && connected)
                    {
                        if (cToken.IsCancellationRequested)
                        {
                            cToken.ThrowIfCancellationRequested();
                        }
                        Console.WriteLine("Displacing liquor...");
                        timer(1000);
                    }
                    if (cToken.IsCancellationRequested)
                    {
                        cToken.ThrowIfCancellationRequested();
                    }
                    Console.WriteLine("Enough liquor displaced!");
                    Console.WriteLine("Closing valves and shutting down the pump...");
                    EM3_OP6();
                    EM1_OP4();
                    Console.WriteLine("The white liquor fill sequence is completed!");
                    laitteet.on_off("P100_P200_PRESET", false);
                    break;
                }
            }, cToken);
        }
        /// <summary>
        /// The method equal of the cooking Procedural Function Chart
        /// </summary>
        /// <param name="cToken"> Cancellation token </param>
        /// <returns> Returns the cooking task </returns>
        private async Task cookingSequence(CancellationToken cToken)
        {
            await Task.Run(() =>
            {
                while (!cToken.IsCancellationRequested)
                {
                    s.Reset();
                    Console.WriteLine("Starting cooking sequence...");
                    laitteet.on_off("P100_P200_PRESET", true);
                    EM3_OP4();
                    EM1_OP1();
                    Console.WriteLine("Opening valves and started the pump...");
                    while (laitteet.getDoubleSystemVar("TI300") <= cookingTemp && connected)
                    {
                        if (cToken.IsCancellationRequested)
                        {
                            cToken.ThrowIfCancellationRequested();
                        }
                        Console.WriteLine("Heating liquor...");
                        timer(1000);
                    }
                    if (cToken.IsCancellationRequested)
                    {
                        cToken.ThrowIfCancellationRequested();
                    }
                    EM3_OP1();
                    EM1_OP2();
                    s.Start();
                    while (s.Elapsed < TimeSpan.FromSeconds(cookingTime) && connected)
                    {
                        if (cToken.IsCancellationRequested)
                        {
                            cToken.ThrowIfCancellationRequested();
                        }
                        U1_OP1(cookingPress);
                        U1_OP2(cookingTemp);
                    }
                    s.Stop();
                    if (cToken.IsCancellationRequested)
                    {
                        cToken.ThrowIfCancellationRequested();
                    }
                    U1_OP3();
                    U1_OP4();

                    EM3_OP6();
                    EM1_OP4();
                    EM3_OP8();
                    Console.WriteLine("The cooking sequence is completed!");
                    laitteet.on_off("P100_P200_PRESET", false);
                    break;
                }

            }, cToken);
        }
        /// <summary>
        /// The method equal of the discharge Procedural Function Chart
        /// </summary>
        /// <param name="cToken"> Cancellation token </param>
        /// <returns> Returns the discharge task </returns>
        private async Task dischargeSequence(CancellationToken cToken)
        {
            await Task.Run(() =>
            {
                while (!cToken.IsCancellationRequested && connected)
                {
                    Console.WriteLine("Starting discharge sequence...");
                    laitteet.on_off("P100_P200_PRESET", true);
                    EM5_OP2();
                    EM3_OP5();
                    Console.WriteLine("Opening valves and started the pump...");
                    while (laitteet.getBoolSystemVar("LS-300") && connected)
                    {
                        if (cToken.IsCancellationRequested)
                        {
                            cToken.ThrowIfCancellationRequested();
                        }
                        Console.WriteLine("Discharging liquor...");
                        timer(1000);
                    }
                    if (cToken.IsCancellationRequested)
                    {
                        cToken.ThrowIfCancellationRequested();
                    }
                    Console.WriteLine("Enough liquor discharged!");
                    Console.WriteLine("Closing valves and shutting down the pump...");
                    EM5_OP4();
                    EM3_OP7();
                    Console.WriteLine("The discharge sequence is completed!");
                    laitteet.on_off("P100_P200_PRESET", false);
                    break;
                }
            }, cToken);
        }
        /// <summary>
        /// The method which sets all the system items control values to default
        /// </summary>
        public void stopMppProcedureSequence()
        {
            laitteet.on_off("P100_P200_PRESET", true);
            laitteet.regulationValveControl("V102", 0);
            laitteet.regulationValveControl("V104", 0);

            laitteet.pumpControl("P200", 0);
            laitteet.pumpControl("P100", 0);

            laitteet.on_off("E100", false);
            laitteet.on_off("V103", false);
            laitteet.on_off("V201", false);
            laitteet.on_off("V204", false);
            laitteet.on_off("V301", false);
            laitteet.on_off("V302", false);
            laitteet.on_off("V303", false);
            laitteet.on_off("V304", false);
            laitteet.on_off("V401", false);
            laitteet.on_off("V404", false);
        }
        /// <summary>
        /// The method equal of the mpp procedure Procedural Function Chart
        /// </summary>
        /// <param name="cToken"> Cancellation token </param>
        /// <returns> Returns the mpp procedure task </returns>
        public async Task mppProcedureSequence(CancellationToken cToken)
        {
            try
            {
                await impregnationSequence(cToken);
                await blackLiquorFillSequence(cToken);
                await whiteLiquorFillSequence(cToken);
                await cookingSequence(cToken);
                await dischargeSequence(cToken);
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}