using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumericalAnalysisLibrary
{
    internal static class Constants
    {
        internal const double epsilan = 0.0001;

        internal static class CollocationMethodConstants
        {
            internal const int collocationN = 10;
        }
        internal static class GaussIntegralMethodConstants
        {
            internal const int powerForCoefCalculation = 10;
            internal const int powerForIntegralCalculation = 50;
        }
        internal static class AdamsMethodConstants
        {
            internal const int n = 0;
            internal const int maxRightBound = 100;
            internal const double funcConditionStep = 0.0001;
        }
        internal static class RungeKuttaMethodConstants
        {
            internal const double startStep = 0.1;
            internal const int methodPower = 3;
        }
        internal static class MatrixConstants
        {
            internal const string matrixWithoutDeterminantString = "Matrix does not have determinant!";
            internal const string incorrectMatrixString = "Matrix has incorrect lengths!";
            internal const string matriciesCannotBeMultipliedString = "Matricies cannot be multiplied!";
            internal const string matriciesCannotBeAddedString = "Maticies cannot be added!";
            internal const string matriciesCannotBeSubstrainedString = "Maticies cannot be substrained!";
        }
        internal static class SLAEConstants
        {
            internal const string infinitCountOfSolutionsString = "Infinit number system's solutions";
            internal const string noSolutionsString = "No system's solutions";
            internal const string incorrectMatrixAndVectorSizesString = "Incorrect matrix and vector sizes!";
        }
        internal static class VectorConstants
        {
            internal const string incorrectVectorString = "Matrix has incorrect lengths!";
            internal const string vectorAndMatrixCannotBeMultipliedString = "Matricies cannot be multiplied!";
            internal const string vectorsCannotBeAddedString = "Maticies cannot be added!";
            internal const string vectorsCannotBeSubstrainedString = "Maticies cannot be substrained!";
        }
        internal static class MethodConstants
        {
            internal const string methodIsNotApplyableString = "Method is not applyable!";
            internal const string incorrectMatrixString = "Matrix has incorrect lengths!";
            internal const string matricesCannotBeMultipliedString = "Matricies cannot be multiplied!";
            internal const string iterationOverflowString = "Iteration overflow! (Iteration count = {0})";

            internal const double epsilanIter = 0.001;
            internal const int maxIterationCount = 10000000;
        }

        internal static class MathFunctionConstants
        {
            internal const string derivativeDoesNotExistExceptionMessage = "Derivative does not exist!";
        }
    }
}
