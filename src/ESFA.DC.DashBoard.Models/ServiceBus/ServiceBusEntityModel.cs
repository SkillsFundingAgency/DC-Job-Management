namespace ESFA.DC.DashBoard.Models.ServiceBus
{
    public sealed class ServiceBusEntityModel
    {
        public string Name { get; set; }

        public long MessageCount { get; set; }

        public long DeadLetterMessageCount { get; set; }

        public long TransferCount { get; set; }
    }
}