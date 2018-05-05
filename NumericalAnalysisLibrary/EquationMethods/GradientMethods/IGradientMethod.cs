using NumericalAnalysisLibrary.Functions;
using NumericalAnalysisLibrary.MathStructures;

namespace NumericalAnalysisLibrary.ExtremumSearch.GradientMethods
{
    public interface IGradientMethod
    {
        Vector[] FindExtremum(MultiMathFunction func, ExtremumType type, string debugInfoFile = null);
    }
}
