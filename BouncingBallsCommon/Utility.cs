using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BouncingBallsCommon
{
    public static class Utility
    {
        private static readonly Random mRandom = new Random(DateTime.Now.Millisecond);

        public static int GetRandomInt(int aMinValue, int aMaxValue)
        {
            return mRandom.Next(aMinValue, aMaxValue);
        }
    }
}