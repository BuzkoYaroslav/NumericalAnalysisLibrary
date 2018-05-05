using System.Collections.Generic;
using NumericalAnalysisLibrary.Functions;

namespace NumericalAnalysisLibrary.KoshiTask
{
    public class KoshiTask
    {
        public MultiMathFunction Derivative { get; private set; }
        public KeyValuePair<double, double> StartCondition { get; private set; }
        public KeyValuePair<double, double> Range { get; private set; }

        public KoshiTask(MultiMathFunction func, KeyValuePair<double, double> condition, KeyValuePair<double, double> range)
        {
            Derivative = func.Clone() as MultiMathFunction;
            StartCondition = new KeyValuePair<double, double>(condition.Key, condition.Value);
            Range = new KeyValuePair<double, double>(range.Key, range.Value);
        }

        public override string ToString()
        {
            string result = "*** KOSHI TASK ***\n";

            result += "Differential equation: x1'(x0) = " + Derivative.ToString() + "\n";
            result += "Start conditions: y(" + StartCondition.Key + ") = " + StartCondition.Value + "\n";
            result += "Range: [" + Range.Key + " ; " + Range.Value + "]\n";

            return result;
        }
    }
}
