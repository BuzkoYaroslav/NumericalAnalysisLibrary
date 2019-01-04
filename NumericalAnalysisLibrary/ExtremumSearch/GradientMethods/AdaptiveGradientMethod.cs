using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using NumericalAnalysisLibrary.Functions;
using NumericalAnalysisLibrary.MathStructures;

namespace NumericalAnalysisLibrary.ExtremumSearch.GradientMethods
{
    public class AdaptiveGradientMethod : NoConditionExtremumMethod
    {
        private double alpha;
        private double lambda;
        private int varsNumber;

        public AdaptiveGradientMethod(double alpha, double lambda, double epsilon, int varsNumber): base()
        {
            Epsilon = epsilon;
            Alpha = alpha;
            Lambda = lambda;
            VariablesCount = varsNumber;
        }

        public double Alpha
        {
            set { alpha = value; }
            get { return alpha; }
        }
        public double Lambda
        {
            set { lambda = value; }
            get { return lambda; }
        }
        public int VariablesCount
        {
            set { varsNumber = value; }
            get { return varsNumber; }
        }

        public override string ToString()
        {
            return string.Format("AdaptiveGradientMethod:\nAlpha = {0}\nLambda = {1}", 
                Alpha, Lambda);
        }

        protected override Vector[] FindExtremum(MultiMathFunction func, ExtremumType type, Vector startPoint)
        {
            int multiplier = type == ExtremumType.Maximum ? -1 : 1;
            var seq = new List<Vector>();

            Vector xCurrent = startPoint,
                xPrev;
            MultiMathFunction[] derivatives = func.DerivativeVector();

            seq.Add(xCurrent);
            do
            {
                xPrev = xCurrent;

                Vector dValue = derivatives.Select(der => { return der.Calculate(xPrev); }).ToArray();

                xCurrent = xPrev - multiplier 
                    * StepForIteration(xPrev, func, derivatives, type) 
                    * dValue;

                seq.Add(xCurrent);
            } while ((xCurrent - xPrev).EuclideanNorm > Epsilon);

            return seq.ToArray();
        }

        protected override Vector[] FindExtremumWithInfo(MultiMathFunction func, ExtremumType type, Vector startPoint, string debugInfoFile)
        {
            StreamWriter writer = new StreamWriter(debugInfoFile);
            int multiplier = type == ExtremumType.Maximum ? -1 : 1;
            var seq = new List<Vector>();

            writer.Write(GapForTable(func, type));

            Vector xCurrent = startPoint,
                xPrev;
            MultiMathFunction[] derivatives = func.DerivativeVector();

            seq.Add(xCurrent);
            int k = 0;
            do
            {
                xPrev = xCurrent;

                Vector dValue = derivatives.Select(der => { return der.Calculate(xPrev); }).ToArray();
                double alpha_k = StepForIteration(xPrev, func, derivatives, type);

                xCurrent = xPrev - multiplier
                    * alpha_k
                    * dValue;

                writer.Write(RowForIteration(++k, xPrev, func.Calculate(xPrev), -dValue, dValue.EuclideanNorm, alpha_k, xCurrent));
                seq.Add(xCurrent);
            } while ((xCurrent - xPrev).EuclideanNorm > Epsilon);

            writer.Close();

            return seq.ToArray();
        }

        private double StepForIteration(Vector x, MultiMathFunction func, MultiMathFunction[] derivative, ExtremumType type)
        {
            double alpha = Alpha;
            double multiplier = type == ExtremumType.Maximum ? -1 : 1;

            int i = 0;
            Vector dValue = ((double[])x).Select(_ => { return multiplier * derivative[i++].Calculate(x); }).ToArray();

            double fValue;

            do
            {
                var xNew = x - alpha * dValue;
                fValue = func.Calculate(xNew) - func.Calculate(x);

                if (multiplier * fValue <= -Epsilon * alpha * Math.Pow(dValue.EuclideanNorm, 2))
                    break;
                else
                    alpha *= Lambda;
            } while (true);

            return alpha;
        }
    }
}
