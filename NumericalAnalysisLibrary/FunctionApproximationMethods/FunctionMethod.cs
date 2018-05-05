using NumericalAnalysisLibrary.MathStructures;

namespace NumericalAnalysisLibrary.FunctionApproximation
{
    public interface FunctionMethod
    {
        Polynomial Solve(Vector x, Vector f);
       
    }
}
