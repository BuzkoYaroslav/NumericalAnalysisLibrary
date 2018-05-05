using NumericalAnalysisLibrary.Functions;
using NumericalAnalysisLibrary.MathStructures;

namespace NumericalAnalysisLibrary.ExtremumSearch.GradientMethods
{
    public abstract class GradientMethod : IGradientMethod
    {
        private string tableFormat = "{0,5}{1,20}{2,10:F4}{3,20}{4,10:F4}{5,10:F4}{6,20}\n";
        private double epsilon;

        public GradientMethod()
        {
            epsilon = 0.00001;
        }

        public double Epsilon
        {
            get { return epsilon; }
            set { epsilon = value; }
        }

        public Vector[] FindExtremum(MultiMathFunction func, ExtremumType type, string debugInfoFile = null)
        {
            if (debugInfoFile != null)
                return FindExtremumWithInfo(func, type, debugInfoFile);
            else
                return FindExtremum(func, type);
        }

        protected abstract Vector[] FindExtremum(MultiMathFunction func, ExtremumType type);
        protected abstract Vector[] FindExtremumWithInfo(MultiMathFunction func, ExtremumType type, string debugInfoFile);

        protected string GapForTable(MultiMathFunction func, ExtremumType type)
        {
            string result = string.Format("Function info:\nF(X) = {0}\nMethod Info:\n{1}\nTask: F(X) -> {2}\n", 
                func, this, type == ExtremumType.Minimum ? "min" : "max");

            result += string.Format(tableFormat, 
                "k", "x_k", "f(x_k)", "-f'(x_k)", "||f'(x_k)||", "alpha_k", "x_k+1");

            return result;
        }
        protected string RowForIteration(params object[] args)
        {
            return string.Format(tableFormat,
                (int)args[0],
                args[1] as Vector,
                (double)args[2],
                -(args[3] as Vector),
                (double)args[4],
                (double)args[5],
                args[6] as Vector);
        }
    }
}
