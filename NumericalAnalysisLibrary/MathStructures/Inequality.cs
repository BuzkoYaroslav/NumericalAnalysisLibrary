using System;
using System.Collections.Generic;
using NumericalAnalysisLibrary.Functions;

namespace NumericalAnalysisLibrary.MathStructures
{
    public enum InequalityType { Greater, Less, Equal}; // it is respectively to the left part (i.e. greater mean left > right)

    public class Inequality
    {
        public MultiMathFunction Left { private set; get; }
        public MultiMathFunction Right { private set; get; }

        public Inequality(MultiMathFunction left, MultiMathFunction right)
        {
            Left = left;
            Right = right;
        }

        public InequalityType DetermineType(Dictionary<uint, double> values)
        {
            double calculatedLeft = Left.Calculate(values),
                   calculatedRight = Right.Calculate(values);

            if (Math.Abs(calculatedLeft - calculatedRight) <= Constants.epsilan)
                return InequalityType.Equal;
            else if (calculatedLeft > calculatedRight)
                return InequalityType.Greater;
            else
                return InequalityType.Less;
        }
    }
}
