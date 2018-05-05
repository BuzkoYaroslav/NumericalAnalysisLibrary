using NumericalAnalysisLibrary.Functions;

namespace NumericalAnalysisLibrary.IntegralApproximation
{
     public interface IntegralMethod
    {
        double Solve(MathFunction func, double a, double b, int n);
    }
}
