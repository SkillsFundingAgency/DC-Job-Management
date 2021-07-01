using System.Collections.Generic;
using ESFA.DC.Jobs.Model.Enums;

namespace ESFA.DC.Jobs.Model
{
    public class JobTypeConstants
    {
        public static List<int> FailedStates = new List<int>
        {
            (int)JobStatusType.Failed,
            (int)JobStatusType.FailedRetry
        };

        public static List<int> ReadyStates = new List<int>
        {
            (int)JobStatusType.Ready,
            (int)JobStatusType.MovedForProcessing
        };

        public static int Processing = (int)JobStatusType.Processing;

        public static int Completed = (int)JobStatusType.Completed;
    }
}