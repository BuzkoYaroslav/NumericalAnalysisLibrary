using NumericalAnalysisLibrary.Functions;

namespace NumericalAnalysisLibrary.ExtremumSearch
{
    public interface IExtremumSolver
    {
        double? FindExtremum(MathFunction func, double a, double b, ExtremumType type, string fileName = null);
    }
}
