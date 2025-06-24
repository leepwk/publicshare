using System.Collections.Generic;
using TestConsoleApp.Models;

namespace TestConsoleApp
{
    public class Orders
    {
        private const double CancelTheshold = 80;
        private const int MinOrders = 5;
        private const int TimeLimit = 60;

        const string StatusNew = "NEW";
        const string StatusCancel = "CANCEL";
        const string StatusExecute = "EXECUTE";

        public List<string> FindTraders(List<OrderEvent> events)
        {
            List<string> results = new List<string>();

            var traderStates = new Dictionary<string, OrderState>();
            var orderCreationMap = new Dictionary<string, OrderEvent>();

            foreach (var orderEvent in events)
            {
                // handle NEW
                if (orderEvent.Action == StatusNew)
                {
                    orderCreationMap[orderEvent.OrderId] = orderEvent;

                    if (!traderStates.ContainsKey(orderEvent.TraderId))
                    {
                        traderStates[orderEvent.TraderId] = new OrderState { TraderId = orderEvent.TraderId };
                    }

                    traderStates[orderEvent.TraderId].NumOrders++;
                }
                // handle CANCEL
                else if (orderEvent.Action == StatusCancel)
                {
                    if (orderCreationMap.TryGetValue(orderEvent.OrderId, out var original) &&
                        original.TraderId == orderEvent.TraderId)
                    {
                        var timeDiff = (orderEvent.TimeStamp - original.TimeStamp).TotalSeconds;
                        if (timeDiff <= TimeLimit)
                        {
                            if (!traderStates.ContainsKey(orderEvent.TraderId))
                                traderStates[orderEvent.TraderId] = new OrderState { TraderId = orderEvent.TraderId };

                            traderStates[orderEvent.TraderId].NumCancelled++;
                        }
                    }
                }
            }

            foreach (var state in traderStates.Values)
            {
                if (state.NumOrders >= MinOrders && ((double)state.NumCancelled / state.NumOrders * 100) >= CancelTheshold)
                {
                    results.Add(state.TraderId);
                }
            }

            return results;
        }
    }
}
