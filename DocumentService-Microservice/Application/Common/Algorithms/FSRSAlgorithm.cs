using System;
using System.Transactions;
using Domain.Entities;
using Domain.Extensions;

namespace Application.Common.Algorithms
{
    public static class FSRSAlgorithm
    {
        private static readonly double[] DefaultParameters = {
            0.40255, 1.18385, 3.173, 15.69105, 7.1949, 0.5345, 1.4604, 0.0046,
            1.54575, 0.1192, 1.01925, 1.9395, 0.11, 0.29605, 2.2698, 0.2315,
            2.9898, 0.51655, 0.6621
        };

        private const double DEFAULT_REQUEST_RETENTION = 0.9;

        public enum State
        {
            New,
            Learning,
            Review,
            Relearning
        }

        public enum Rating
        {
            Again = 1,
            Hard = 2,
            Good = 3,
            Easy = 4
        }

     }
} 