using System;
using System.Collections.Generic;
using Tuni.MppOpcUaClientLib;
using UaLib = Tuni.MppOpcUaClientLib;


namespace HT
{
    /// <summary>
    /// The batch process controller class
    /// </summary>
    public class BatchProcessController : IDisposable
    {
        public event EventHandler<ProcessItemsChangedEventArgs> ProcessItemsChanged_BPC;

        private UaLib.MppClient m_mppClient = null;
        private Dictionary<string, bool> bool_system_values = new Dictionary<string, bool>();
        private Dictionary<string, int> int_system_values = new Dictionary<string, int>();
        private Dictionary<string, double> double_system_values = new Dictionary<string, double>();
        
        /// <summary>
        /// The constructer for batch process controller class
        /// </summary>
        public BatchProcessController() { }
        /// <summary>
        /// Connects to the mppClient, adds process items to subscription
        /// and sets default values for process items
        /// </summary>
        /// <param name="url"> The url which we want to connect to </param>
        public void Connect(string url)
        {
            try
            {
                var ctorParams = new UaLib.ConnectionParamsHolder(url);
                m_mppClient = new UaLib.MppClient(ctorParams);

                m_mppClient.ProcessItemsChanged += m_mppClient_ProcessItemsChanged;
                m_mppClient.ConnectionStatus += m_mppClient_ConnectionStatusChanged;

                m_mppClient.Init();

                // Säiliöiden lämpötilat
                m_mppClient.AddToSubscription("TI300");
                m_mppClient.AddToSubscription("TI100");

                double_system_values["TI300"] = 0.0;
                double_system_values["TI100"] = 0.0;

                // Pumpujen tehot
                m_mppClient.AddToSubscription("P100");
                m_mppClient.AddToSubscription("P200");

                int_system_values["P100"] = 0;
                int_system_values["P200"] = 0;

                // Pumppujen esivalinta
                m_mppClient.AddToSubscription("P100_P200_PRESET");

                bool_system_values["P100_P200_PRESET"] = false;

                // Säiliöiden pinnankorkeus
                m_mppClient.AddToSubscription("LI100");
                m_mppClient.AddToSubscription("LI200");
                m_mppClient.AddToSubscription("LI400");

                int_system_values["LI200"] = 0;
                int_system_values["LI100"] = 0;
                int_system_values["LI400"] = 0;

                // Säätöventtiilien tila
                m_mppClient.AddToSubscription("V102");
                m_mppClient.AddToSubscription("V104");

                int_system_values["V102"] = 0;
                int_system_values["V104"] = 0;

                // on/off venttiilien tila
                m_mppClient.AddToSubscription("V103");
                m_mppClient.AddToSubscription("V201");
                m_mppClient.AddToSubscription("V204");

                bool_system_values["V103"] = false;
                bool_system_values["V201"] = false;
                bool_system_values["V204"] = false;

                m_mppClient.AddToSubscription("V301");
                m_mppClient.AddToSubscription("V302");
                m_mppClient.AddToSubscription("V303");
                m_mppClient.AddToSubscription("V304");

                bool_system_values["V301"] = false;
                bool_system_values["V302"] = false;
                bool_system_values["V303"] = false;
                bool_system_values["V304"] = false;

                m_mppClient.AddToSubscription("V401");
                m_mppClient.AddToSubscription("V404");
                bool_system_values["V401"] = false;
                bool_system_values["V404"] = false;

                // Säiliönpaine
                m_mppClient.AddToSubscription("PI300");
                int_system_values["PI300"] = 0;

                // Rajakytkimet
                m_mppClient.AddToSubscription("LA+100");
                m_mppClient.AddToSubscription("LS-200");
                m_mppClient.AddToSubscription("LS+300"); 
                m_mppClient.AddToSubscription("LS-300");

                bool_system_values["LA+100"] = true;
                bool_system_values["LS-200"] = false;
                bool_system_values["LS+300"] = false;
                bool_system_values["LS-300"] = false;

                // Lämmitin
                m_mppClient.AddToSubscription("E100");
                bool_system_values["E100"] = false;
            }
            catch (Exception e)
            {
                Dispose();
                throw;
            }
        }
        /// <summary>
        /// Method to control process items which are on or off
        /// </summary>
        /// <param name="id"> ID of the system item </param>
        /// <param name="value"> Control value for the system item</param>
        public void on_off(string id, bool value)
        {
            try
            {
                m_mppClient.SetOnOffItem(id, value);
            }
            catch (Exception e)
            {
                throw;
            }
        }
        /// <summary>
        /// Method to control pump items
        /// </summary>
        /// <param name="id"> ID of the pump </param>
        /// <param name="value"> Control value of the pump </param>
        public void pumpControl(string id, int value)
        {
            try
            {
                m_mppClient.SetPumpControl(id, value);
            }
            catch (Exception e)
            {
                throw;
            }
        }
        /// <summary>
        /// Method to control regulation valve items
        /// </summary>
        /// <param name="id"> ID of the regulation valve </param>
        /// <param name="value"> Control value of the regulation valve </param>
        public void regulationValveControl(string id, int value)
        {
            try
            {
                m_mppClient.SetValveOpening(id, value);
            }
            catch (Exception e)
            {
                throw;
            }
         }
         /// <summary>
         /// Method to get current int value for the wanted system value
         /// </summary>
         /// <param name="id"> ID of the wanted system item </param>
         /// <returns> If exists, returns the value of the system item </returns>
         public int getIntSystemVar(string id){
            try
            {
                return int_system_values[id];
            }
            catch (Exception)
            {
                throw;
            }
            //if (int_system_values.ContainsKey(id))
            //{
            //    return int_system_values[id];
            //}
            //return 0;
        }
        /// <summary>
        /// Method to get current double value for the wanted system value
        /// </summary>
        /// <param name="id"> ID of the wanted system item </param>
        /// <returns> If exists, returns the value of the system item </returns>
        public double getDoubleSystemVar(string id){
            //if (double_system_values.ContainsKey(id))
            //{
            //    return double_system_values[id];
            //}
            //return 0.0;
            try
            {
                return double_system_values[id];
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Method to get current bool value for the wanted system value
        /// </summary>
        /// <param name="id"> ID of the wanted system item </param>
        /// <returns> If exists, returns the value of the system item </returns>
        public bool getBoolSystemVar(string id){
            try
            {
                return bool_system_values[id];
            }
            catch (Exception)
            {
                throw;
            }
            //if (bool_system_values.ContainsKey(id))
            //{
            //    return bool_system_values[id];
            //}
            //return false;
        }
        /// <summary>
        /// Event handler for the mppClient connection status event
        /// </summary>
        /// <param name="source"> The object which sends the event </param>
        /// <param name="args"> The custom event arguments </param>
        private void m_mppClient_ConnectionStatusChanged(object source,
            UaLib.ConnectionStatusEventArgs args)
        {
            try
            {
                string status = args.StatusInfo.SimplifiedStatus.ToString();
                Console.WriteLine("Testi: " + args.StatusInfo.FullStatusString);
                ProcessItemsChanged_BPC(this, new ProcessItemsChangedEventArgs("connectionStatus", status));
            }
            catch (Exception e)
            {

            }
        }
        /// <summary>
        /// Event handler for the mppClient process items changed event
        /// </summary>
        /// <param name="source"> The object which sends the event </param>
        /// <param name="args"> The custom event arguments </param>
        private void m_mppClient_ProcessItemsChanged(object source,
            UaLib.ProcessItemChangedEventArgs args)
        {
            try
            {
                foreach (var key in args.ChangedItems.Keys)
                {

                    if (int_system_values.ContainsKey(key))
                    {
                        var valueObject =
                             (UaLib.MppValueInt)args.ChangedItems[key];
                        var actualValue = valueObject.Value;
                        int_system_values[key] = actualValue;
                        ProcessItemsChanged_BPC(this, new ProcessItemsChangedEventArgs(key,actualValue.ToString()));
                    }
                    else if (double_system_values.ContainsKey(key))
                    {
                        var valueObject =
                             (UaLib.MppValueDouble)args.ChangedItems[key];
                        var actualValue = valueObject.Value;
                        double_system_values[key] = actualValue;
                        ProcessItemsChanged_BPC(this, new ProcessItemsChangedEventArgs(key, actualValue.ToString()));
                    }
                    else if (bool_system_values.ContainsKey(key))
                    {
                        var valueObject =
                             (UaLib.MppValueBool)args.ChangedItems[key];
                        var actualValue = valueObject.Value;
                        bool_system_values[key] = actualValue;
                        ProcessItemsChanged_BPC(this, new ProcessItemsChangedEventArgs(key, actualValue.ToString()));
                    }
                }
            }
            catch (Exception e)
            {

            }
        }
        /// <summary>
        /// Method to dispose the current mppClient
        /// </summary>
        public void Dispose()
        {
            try
            {
                if (m_mppClient != null)
                {
                    m_mppClient.Dispose();
                    m_mppClient = null;
                }
            }
            catch (Exception e) 
            {

            }
        }

    }
}
