using NumericalAnalysisLibrary.Functions;

namespace NumericalAnalysisLibrary.IntegralApproximation
{
    public class SimpsonMethod : IntegralMethod
    {
        public double Solve(MathFunction func, double a, double b, int n)
        {
            if (a == b) return 0;
            if (a > b) { double tmp = a; a = b; b = tmp; }

            double result = 0;
            double difference = (b - a) / (2 * n);

            for (int k = 1; k < n; k++)
            {
                result += 4 * func.Calculate(a + difference * (2 * k - 1));
                result += 2 * func.Calculate(a + difference * 2 * k);
            }
            result += 4 * func.Calculate(b - difference) + func.Calculate(a) + func.Calculate(b);
            result *= difference / 3;

            return result;
        }
    }
}
