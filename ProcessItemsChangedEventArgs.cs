using System;

namespace HT
{
    /// <summary>
    /// Custom event arguments
    /// </summary>
    public class ProcessItemsChangedEventArgs : EventArgs
    {
        public string ProcessItemID { get; set; }
        public string ProcessItemValue { get; set; }

        /// <summary>
        /// Constructor for the custom event arguments
        /// </summary>
        /// <param name="processItemID"> The ID of the changed process item </param>
        /// <param name="processItemValue"> The changed value of the process item </param>
        public ProcessItemsChangedEventArgs(string processItemID, string processItemValue)
        {
            this.ProcessItemID = processItemID;
            this.ProcessItemValue = processItemValue;
        }
    }
}
