﻿namespace Assignment2.Models.ViewModels
{
    public class BrokerageViewModel
    {
        public IEnumerable<Brokerage> Brokerages { get; set; }
        public IEnumerable<Client> Clients { get; set; }
        public IEnumerable<Subscription> Subscriptions { get; set; }
    }
}
