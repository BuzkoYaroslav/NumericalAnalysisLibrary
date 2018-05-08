using NumericalAnalysisLibrary.Functions;
using NumericalAnalysisLibrary.MathStructures;

namespace NumericalAnalysisLibrary.ExtremumSearch
{
    public interface INoConditionExtremumMethod
    {
        Vector[] FindExtremum(MultiMathFunction func, ExtremumType type, Vector startPoint, string debugInfoFile = null);
    }
}
