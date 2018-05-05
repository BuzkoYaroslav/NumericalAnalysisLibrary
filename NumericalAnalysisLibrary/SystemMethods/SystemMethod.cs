using NumericalAnalysisLibrary.MathStructures;

namespace NumericalAnalysisLibrary.SLAESolution
{
    public interface SystemMethod
    {
        Vector Solve(SLAE system);
    }
}
