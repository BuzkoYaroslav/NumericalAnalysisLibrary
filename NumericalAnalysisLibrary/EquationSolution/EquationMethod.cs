using NumericalAnalysisLibrary.MathStructures;

namespace NumericalAnalysisLibrary.EquationSolution
{
    public interface EquationMethod
    {
        double Solve(Equation eq, double a, double b);
    }
}
