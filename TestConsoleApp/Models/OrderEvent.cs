using System;

namespace TestConsoleApp.Models
{
    public enum OrderAction
    {
        Unknown = 0,    
        New = 1,
        Cancel = 2,
        Execute = 3
    }

    public class OrderEvent
    {
        public string OrderId { get; set; }
        public string TraderId { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Action { get; set; }
        public int Quantity { get; set; }
    }
}
