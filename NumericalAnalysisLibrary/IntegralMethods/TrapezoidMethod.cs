using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumericalAnalysisLibrary
{
    public class TrapezoidMethod : IntegralMethod
    {
        public double Solve(MathFunction func, double a, double b, int n)
        {
            if (a == b) return 0;
            if (a > b) { double tmp = a; a = b; b = tmp; }

            double result = 0;
            double difference = (b - a) / n;

            for (int i = 1; i < n; i++)
                result += func.Calculate(a + difference * i);
            result += (func.Calculate(a) + func.Calculate(b)) / 2;
            result *= difference;

            return result;
        }
    }
}
