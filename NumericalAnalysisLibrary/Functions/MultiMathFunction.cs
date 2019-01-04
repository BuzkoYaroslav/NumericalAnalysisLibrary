using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace NumericalAnalysisLibrary.Functions
{
    public enum MultiMathFunctionType {

        [Description("+")]
        Addition = 0,
        [Description("*")]
        Multiplication,
        [Description("/")]
        Division,
        [Description("^")]
        Special

    };

    public static class MultiMathFunctionTypeExtension
    {
        public static string ToDescriptionString(this MultiMathFunctionType en)
        {
            DescriptionAttribute[] attributes = (DescriptionAttribute[])en.GetType().GetField(en.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }
    }
    public static class MultiMathFunctionTypeFactory
    {
        public static MultiMathFunctionType Populate(string desc)
        {
            if (desc == MultiMathFunctionType.Addition.ToDescriptionString())
                return MultiMathFunctionType.Addition;
            else if (desc == MultiMathFunctionType.Multiplication.ToDescriptionString())
                return MultiMathFunctionType.Multiplication;
            else if (desc == MultiMathFunctionType.Division.ToDescriptionString())
                return MultiMathFunctionType.Division;
            else
                return MultiMathFunctionType.Special;
        }
    }

    public class MultiMathFunction: ICloneable
    {
        private delegate void CalculatingVoid(ref double result, double newValue);
        private delegate void TransformingVoid(ref MathFunction result, MathFunction newFunc);

        protected double coef;
        protected List<MultiMathFunction> functions;
        protected MultiMathFunctionType type;     

        public int NumberOfVariables
        {
            get
            {
                var set = new HashSet<int>();

                GetAllVariables(ref set);

                return set.Count;
            }
        }

        #region Initialization

        public MultiMathFunction(double coef, MultiMathFunctionType type, params MultiMathFunction[] functions)
        {
            Initialize(coef, type, functions);
        }
        public MultiMathFunction()
        {
            coef = 1.0;
            type = MultiMathFunctionType.Addition;
        }
        private void Initialize(double coef, MultiMathFunctionType type, IEnumerable<MultiMathFunction> functions)
        {
            this.coef = coef;
            this.type = type;

            switch (type)
            {
                case MultiMathFunctionType.Addition:
                    InitializeAddition(functions);
                    break;
                case MultiMathFunctionType.Multiplication:
                    InitializeMultiplication(functions);
                    break;
                case MultiMathFunctionType.Division:
                    InitializeDivision(functions);
                    break;
                default:
                    break;
            }
        }

        private void InitializeAddition(IEnumerable<MultiMathFunction> functions)
        {
            this.functions = new List<MultiMathFunction>();

            foreach (var func in functions)
                if (!func.IsZero())
                    this.functions.Add(func.Clone() as MultiMathFunction);
        }
        private void InitializeMultiplication(IEnumerable<MultiMathFunction> functions)
        {
            this.functions = new List<MultiMathFunction>();

            foreach (var func in functions)
            {
                if (func.IsZero())
                {
                    this.functions.Clear();
                    coef = 0;
                    break;
                }
                this.functions.Add(func.Clone() as MultiMathFunction);
            }
        }
        private void InitializeDivision(IEnumerable<MultiMathFunction> functions)
        {
            this.functions = new List<MultiMathFunction>();

            MultiMathFunction up = 1,
                low = 1;

            bool first = true;

            foreach (var func in functions)
            {
                if (func.IsZero() && first)
                {
                    this.functions.Clear();
                    coef = 0;
                    break;
                } else if (func.IsZero())
                {
                    throw new DivideByZeroException(Constants.MultiMathFunctionsConstants.denominatorIsZeroException);
                }

                if (first)
                    up *= func;
                else
                    low *= func;

                first = false;
            }

            this.functions.Add(up);
            this.functions.Add(low);
        }

        #endregion

        #region ICloneable
        public virtual object Clone()
        {
            return new MultiMathFunction(coef, type, functions.ToArray());
        }
        #endregion

        #region MultiMathFunction sequence representation

        private delegate MultiMathFunction BinaryOperator(MultiMathFunction f, MultiMathFunction g);
        private static Tuple<char, BinaryOperator>[] binaryOperators = new Tuple<char, BinaryOperator>[]
        {
            new Tuple<char, BinaryOperator>('*', (x, y) => x * y),
            new Tuple<char, BinaryOperator>('/', (x, y) => x / y),
            new Tuple<char, BinaryOperator>('^', (x, y) => x ^ y),
            new Tuple<char, BinaryOperator>('+', (x, y) => x + y)
        };

        private class MultiMathFunctionSequence
        {
            private SequenceNode first;
            private SequenceNode last;

            public MultiMathFunctionSequence(MultiMathFunction func)
            {
                first = new SequenceNode();
                first.value = func;

                last = first;
            } 

            public void AddFunction(MultiMathFunction newF, MultiMathFunctionType opType)
            {
                var newSN = new SequenceNode();

                newSN.value = newF;
                newSN.prev = last;
                newSN.prevOperation = opType;
                last.next = newSN;
                last.nextOperation = opType;

                last = newSN;
            }

            public MultiMathFunction EvaluateSequence()
            {
                foreach (var op in binaryOperators)
                {
                    SequenceNode node;
                    var operation = MultiMathFunctionTypeFactory.Populate(op.Item1.ToString());

                    while ((node = FindNode(operation)) != null)
                    {
                        var newNode = new SequenceNode();

                        newNode.value = op.Item2(node.value, node.next.value);
                        newNode.prev = node.prev;
                        newNode.prevOperation = node.prevOperation;

                        if (node.prev != null)
                            node.prev.next = newNode;
                        else
                            first = newNode;

                        newNode.next = node.next.next;
                        newNode.nextOperation = node.next.nextOperation;

                        if (node.next.next != null)
                            node.next.next.prev = newNode;
                        else
                            last = newNode;
                    }
                }

                return first.value;
            }

            // seaching function that is connected with the next by the opType operation
            private SequenceNode FindNode(MultiMathFunctionType opType)
            {
                for (var current = first; current.next != null; current = current.next)
                {
                    if (current.nextOperation == opType)
                        return current;
                }

                return null;
            }
        }
        private class SequenceNode
        {
            public MultiMathFunction value;
            public SequenceNode next;
            public SequenceNode prev;

            public MultiMathFunctionType? nextOperation;
            public MultiMathFunctionType? prevOperation;
        }

        #endregion

        #region Parsing from string
        private static Tuple<char, char>[] braces = new Tuple<char, char>[]
            {
                new Tuple<char, char>('(', ')')
            };

        private delegate MultiMathFunction CreateSpecialFunction(params object[] args);
        private static Dictionary<string, CreateSpecialFunction> specialFunctions = new Dictionary<string, CreateSpecialFunction>()
        {
            {"sin", (args) => {
                return new SinMultiFunction(args[0] as double? ?? 1.0, args[1] as MultiMathFunction);
                } },
            {"cos", (args) => {
                return new CosMultiFunction(args[0] as double? ?? 1.0, args[1] as MultiMathFunction);
                } },
            {"ln", (args) => {
                return new LnMultiFunction(args[0] as double? ?? 1.0, args[1] as MultiMathFunction);
                }},
            {"asin",(args) => {
                return new ASinMultiFunction(args[0] as double? ?? 1.0, args[1] as MultiMathFunction);
                }},
            {"acos",(args) => {
                return new ACosMultiFunction(args[0] as double? ?? 1.0, args[1] as MultiMathFunction);
                }},
            {"x", (args) => {
                return new ArgumentFunction(args[0] as double? ?? 1.0, args[1] as uint? ?? 0);
                }}
        };

        // example - "0.5 * cos[x] + sin[x] * cos[x] ^ (2 * 3)"
        public static MultiMathFunction Parse(string function)
        {
            // checking function string format
            if (!HasRightFormat(function))
                throw new FormatException("String has incorrect format! Check braces!");

            PerformPartization(ref function);

            return UnsafeParse(function);
        }

        private static MultiMathFunction UnsafeParse(string function)
        {
            // checking if passed string is just number
            double num;
            if (double.TryParse(function, out num))
                return num;

            var keys = specialFunctions.Keys;
            var indexes = specialFunctions.Keys.Select(x => new Tuple<string, int>(x, function.IndexOf(x))).ToList();
            var start = indexes.Find(new Predicate<Tuple<string, int>>(x => x.Item2 == 0));

            if (start != null && start.Item1 == "x")
            {
                return specialFunctions[start.Item1](1.0, uint.Parse(function.Substring(start.Item1.Length)));
            } else if (start != null) {
                return specialFunctions[start.Item1](1.0, UnsafeParse(function.Substring(start.Item1.Length + 1, function.Length - start.Item1.Length - 2)));
            }

            MultiMathFunctionSequence seq = null;
            Tuple<int, int> bracesIndexes = new Tuple<int, int>(-1, -1);
            while ((bracesIndexes = BracesPositions(function, braces.First(), bracesIndexes.Item2 + 1)) != null) {
                if (seq == null)
                {
                    seq = new MultiMathFunctionSequence(UnsafeParse(function.Substring(
                        bracesIndexes.Item1 + 1,
                        bracesIndexes.Item2 - bracesIndexes.Item1 - 1)));
                    continue;
                }

                seq.AddFunction(
                    UnsafeParse(function.Substring(
                        bracesIndexes.Item1 + 1,
                        bracesIndexes.Item2 - bracesIndexes.Item1 - 1)), 
                    MultiMathFunctionTypeFactory.Populate(function[bracesIndexes.Item1 - 1].ToString()));
            }

            return seq.EvaluateSequence();
        }
        
        private static bool HasRightFormat(string function)
        {
            // if string is emtpy - format is incorrect
            if (function == null)
                return false; 

            // extracting closing and opening characters
            var closing = braces.Select(x => x.Item2).ToArray();
            var opening = braces.Select(x => x.Item1).ToArray();

            int openIndex = function.IndexOfAny(opening);

            // if there is no opening brace and there is closing - format is incorrect
            // otherwise format is correct
            if (openIndex == -1)
                return function.IndexOfAny(closing) == -1;

            char close = braces.First(x => function[openIndex] == x.Item1).Item2;

            int closeIndex = function.IndexOf(close);

            // if closing brace stays in the string earlier than opening
            // or there is no closing brace
            if (closeIndex < openIndex)
                return false;
            
            // checking format in the braces
            bool innerFormat = HasRightFormat(function.Substring(openIndex + 1, closeIndex - openIndex - 1));
            // check format of function after closing brace
            bool trailFormat = closeIndex == function.Length - 1 ? true :
                HasRightFormat(function.Substring(closeIndex + 1));

            return innerFormat && trailFormat;
        }
        private static void PerformPartization(ref string function)
        {
            var defaultOpenBrace = braces.First().Item1;
            var defaultCloseBrace = braces.First().Item2;

            function = function.Replace("-", string.Format("+{0}-1{1}*", defaultOpenBrace, defaultCloseBrace));
            function.TrimStart('+');

            foreach (var special in specialFunctions.Keys)
            {
                if (special == "x")
                    continue;

                int pos = 0;

                while ((pos = function.IndexOf(special, pos)) != -1)
                {
                    var br = BracesPositions(function, braces.First(), pos);

                    function = function.Insert(br.Item1, defaultOpenBrace.ToString());
                    function = function.Insert(br.Item2 + 1, defaultCloseBrace.ToString());

                    pos = br.Item2 + 2;
                }
            }

            function = function.Insert(0, defaultOpenBrace.ToString());
            function = function.Insert(function.Length, defaultCloseBrace.ToString());

            foreach (var op in binaryOperators)
                function = function.Replace(op.Item1.ToString(), string.Format("{2}{0}{1}", op.Item1, defaultOpenBrace, defaultCloseBrace));


            function = function.Replace(" ", "");
        }
        private static Tuple<int, int> BracesPositions(string function, Tuple<char, char> braces, int startIndex)
        {
            int opening = function.IndexOf(braces.Item1, startIndex);

            if (opening == -1)
                return null;

            int opCount = 0,
                clCount = 0;

            int closing = function.Substring(opening).ToList().FindIndex(
                new Predicate<char>(x =>
                {
                    if (x == braces.Item1)
                        opCount++;
                    else if (x == braces.Item2)
                        clCount++;

                    return opCount == clCount;
                })
            ) + opening;

            return new Tuple<int, int>(opening, closing);
        }

        #endregion


        public override string ToString()
        {
            if (coef == 0 || functions.Count == 0)
                return "0";

            string result = coef != 1 ? Math.Round(coef, 2) + " * " : "" + "(";
            string splitter = type == MultiMathFunctionType.Addition ? " + " : type == MultiMathFunctionType.Multiplication ? " * " : " : ";

            for (int i = 0; i < functions.Count; i++)
                result += functions[i].ToString() + (i != (functions.Count - 1) ? splitter : "");

            result += ")";

            return result;
        }

        public virtual double Calculate(Dictionary<uint, double> x)
        {
            if (coef == 0)
                return 0;

            if (functions.Count == 0)
                return coef;

            if (functions.Count == 1)
                return coef * functions[0].Calculate(x);

            switch (type)
            {
                case MultiMathFunctionType.Addition:
                    return Calculate(x, (ref double result, double newValue) => { result += newValue; });
                case MultiMathFunctionType.Multiplication:
                    return Calculate(x, (ref double result, double newValue) => { result *= newValue; });
                case MultiMathFunctionType.Division:
                    return Calculate(x, (ref double result, double newValue) => { result /= newValue; });
                default:
                    return coef;
            }
        }
        private double Calculate(Dictionary<uint, double> x, CalculatingVoid cVoid)
        {
            if (functions == null || functions.Count == 0)
                return coef;

            double result = functions[0].Calculate(x);

            for (int i = 1; i < functions.Count; i++)
                cVoid(ref result, functions[i].Calculate(x));

            result *= coef;
            return result;
        }

        public virtual bool IsZero()
        {
            if (coef == 0)
                return true;

            if (functions.Count == 0)
                return coef == 0;

            switch (type)
            {
                case MultiMathFunctionType.Addition:
                    foreach (var func in functions)
                        if (!func.IsZero())
                            return false;

                    return true;
                case MultiMathFunctionType.Multiplication:
                    foreach (var func in functions)
                        if (func.IsZero())
                            return true;

                    return false;
                case MultiMathFunctionType.Division:
                    return functions[0].IsZero();
                default:
                    return true;
            }
        }

        public virtual MultiMathFunction Derivative(int order, int index)
        {
            if (order == 0)
                return this;

            if (coef == 0 || functions.Count == 0)
                return 0;

            MultiMathFunction derivative = 0;

            switch (type)
            {
                case MultiMathFunctionType.Addition:
                    for (int i = 0; i < functions.Count; i++)
                    {
                        MultiMathFunction newDerivative = functions[i].Derivative(order, index);

                        if (!newDerivative.IsZero())
                            derivative += newDerivative;
                    }

                    return derivative;
                case MultiMathFunctionType.Multiplication:
                    for (int i = 0; i < functions.Count; i++)
                    {
                        MultiMathFunction der = functions[i].Derivative(1, index);

                        if (der.IsZero())
                            continue;

                        for (int j = 0; j < functions.Count; j++)
                        {
                            if (i != j)
                            {
                                der *= functions[j];
                            }
                        }
                        
                        derivative += der;
                    }

                    return derivative.Derivative(order - 1, index);
                case MultiMathFunctionType.Division:
                    var up = functions[0];
                    var low = functions[1];

                    derivative = (up.Derivative(1, index) * low - low.Derivative(1, index) * up) / (low ^ 2);

                    return derivative.Derivative(order - 1, index);
                default:
                    return 0;
            }
        }

        public virtual void GetAllVariables(ref HashSet<int> vars)
        {
            if (functions == null)
                return;

            foreach (var f in functions)
                f.GetAllVariables(ref vars);
        }

        public MultiMathFunction[] DerivativeVector()
        {
            HashSet<int> vars = new HashSet<int>();

            GetAllVariables(ref vars);

            SortedList<int, MultiMathFunction> list = new SortedList<int, MultiMathFunction>();

            foreach (var index in vars)
                list.Add(index, Derivative(1, index));

            return list.Select(x => { return x.Value; }).ToArray();
        }
        public MultiMathFunction[,] GesseMatrix()
        {
            HashSet<int> vars = new HashSet<int>();
            GetAllVariables(ref vars);

            SortedList<Tuple<int, int>, MultiMathFunction> list = new SortedList<Tuple<int, int>, MultiMathFunction>();

            int[] indexes = vars.ToArray();

            for (int i = 0; i < indexes.Length; i++)
                for (int j = 0; j < indexes.Length; j++)
                    list.Add(new Tuple<int, int>(i, j), Derivative(1, i).Derivative(1, j));

            MultiMathFunction[,] matrix = new MultiMathFunction[indexes.Length, indexes.Length];
            foreach (var item in list)
                matrix[item.Key.Item1, item.Key.Item2] = item.Value;

            return matrix;
        }

        #region Transform to MathFunction type

        public virtual MathFunction TransformToSimpleFunction(Dictionary<uint, MathFunction> transform)
        {
            List<MathFunction> simpleFunctions = new List<MathFunction>();

            foreach (var func in functions)
                simpleFunctions.Add(func.TransformToSimpleFunction(transform));

            MathFunctionType sType;
            switch (type)
            {
                case MultiMathFunctionType.Addition:
                    sType = MathFunctionType.Sum;
                    break;
                case MultiMathFunctionType.Division:
                    sType = MathFunctionType.Division;
                    break;
                case MultiMathFunctionType.Multiplication:
                    sType = MathFunctionType.Multiplication;
                    break;
                default:
                    sType = MathFunctionType.Sum;
                    break;
            }

            return new MathFunction(coef, sType, simpleFunctions.ToArray());
        }
        public virtual MathFunction TransformToSimpleFunction(Dictionary<uint, double> variables)
        {
            if (functions == null || functions.Count == 0)
                return new ConstFunction(coef);

            if (coef == 0)
                return new ConstFunction(0);

            switch(type)
            {
                case MultiMathFunctionType.Addition:
                    return TransformToSimpleFunction(variables, (ref MathFunction result, MathFunction newFunc) =>
                    {
                        result += newFunc;
                    });
                case MultiMathFunctionType.Multiplication:
                    return TransformToSimpleFunction(variables, (ref MathFunction result, MathFunction newFunc) =>
                    {
                        result *= newFunc;
                    });
                case MultiMathFunctionType.Division:
                    return TransformToSimpleFunction(variables, (ref MathFunction result, MathFunction newFunc) =>
                    {
                        result /= newFunc;
                    });
                default:
                    return coef;
            }
        }
        private MathFunction TransformToSimpleFunction(Dictionary<uint, double> variables, TransformingVoid tVoid)
        {
            if (functions == null || functions.Count == 0)
                return new ConstFunction(coef);

            MathFunction result = functions[0].TransformToSimpleFunction(variables);

            for (int i = 1; i < functions.Count; i++)
                tVoid(ref result, functions[i].TransformToSimpleFunction(variables));

            result *= coef;

            return result;
        }

        #endregion

        #region Operators implementation

        public static MultiMathFunction operator+(MultiMathFunction f1, MultiMathFunction f2)
        {
            return AddFunctions(f1, f2);
        }
        public static MultiMathFunction operator-(MultiMathFunction f1, MultiMathFunction f2)
        {
            return AddFunctions(f1, -f2);
        }
        private static MultiMathFunction AddFunctions(MultiMathFunction f1, MultiMathFunction f2)
        {
            if (f1 is ConstMultiFunction &&
                f2 is ConstMultiFunction)
                return f1.coef + f2.coef;

            if (f1.type == MultiMathFunctionType.Addition)
            {
                MultiMathFunction f1Copy = f1.Clone() as MultiMathFunction;
                f1Copy.functions.Add(f2.Clone() as MultiMathFunction);
                return f1Copy;
            }

            return new MultiMathFunction(1.0, MultiMathFunctionType.Addition, f1, f2);
        }
        public static MultiMathFunction operator-(MultiMathFunction f)
        {
            MultiMathFunction fCopy = f.Clone() as MultiMathFunction;
            fCopy.coef = -fCopy.coef;

            return fCopy;
        }

        public static MultiMathFunction operator*(MultiMathFunction f1, MultiMathFunction f2)
        {
            if (f1 is ConstMultiFunction &&
                f2 is ConstMultiFunction)
                return f1.coef * f2.coef;
            else if (f1 is ConstMultiFunction)
                return f1.coef * f2;
            else if (f2 is ConstMultiFunction)
                return f2.coef * f1;

            if (f1.type == MultiMathFunctionType.Multiplication)
            {
                MultiMathFunction f1Copy = f1.Clone() as MultiMathFunction;
                f1Copy.functions.Add(f2.Clone() as MultiMathFunction);
                return f1Copy;
            }

            return new MultiMathFunction(1.0, MultiMathFunctionType.Multiplication, f1, f2);
        }
        public static MultiMathFunction operator*(double coef, MultiMathFunction f)
        {
            MultiMathFunction newF = f.Clone() as MultiMathFunction;
            newF.coef *= coef;

            return newF;
        }
        public static MultiMathFunction operator*(MultiMathFunction f, double coef)
        {
            return coef * f;
        }

        public static MultiMathFunction operator/(MultiMathFunction f1, MultiMathFunction f2)
        {
            if (f1 is ConstMultiFunction && f2 is ConstMultiFunction)
                return f1.coef / f2.coef;

            if (f1.type == MultiMathFunctionType.Division)
            {
                if (f1.IsZero())
                    return 0;

                MultiMathFunction f1Copy = f1.Clone() as MultiMathFunction;
                f1Copy.functions[1].functions.Add(f2.Clone() as MultiMathFunction);
                return f1Copy;
            }

            return new MultiMathFunction(1.0, MultiMathFunctionType.Division, f1, f2);
        }
        public static MultiMathFunction operator/(MultiMathFunction f, ConstMultiFunction k)
        {
            return f / k.coef;
        }
        public static MultiMathFunction operator/(MultiMathFunction f, double coef)
        {
            MultiMathFunction fCopy = f.Clone() as MultiMathFunction;
            fCopy.coef /= coef;

            return fCopy;
        }

        public static implicit operator MultiMathFunction(double coef)
        {
            return new ConstMultiFunction(coef);
        }
        public static MultiMathFunction operator^(MultiMathFunction f1, MultiMathFunction f2)
        {
            return new PowerMultiFunction(1.0, f1, f2);
        }

        #endregion

        #region Express Variable

        public virtual MultiMathFunction[] GetFunctionForVariable(List<MultiMathFunction> right, int index)
        {
            NormalizeRighPart(ref right);

            HashSet<int> vars = new HashSet<int>();

            GetAllVariables(ref vars);

            if (!vars.Contains(index))
                throw new Exception(); // Raises when variable index is not a part of the function

            MultiMathFunction left = null;

            switch (type)
            {
                case MultiMathFunctionType.Addition:
                    left = UpdateRightPart(ref right, index, 
                        (ref List<MultiMathFunction> r, MultiMathFunction f) => 
                    {
                        for (int i = 0; i < r.Count; i++)
                            r[i] -= f;
                    });

                    break;
                case MultiMathFunctionType.Multiplication:
                    left = UpdateRightPart(ref right, index,
                        (ref List<MultiMathFunction> r, MultiMathFunction f) =>
                        {
                            for (int i = 0; i < r.Count; i++)
                                r[i] /= f;
                        });

                    break;
                case MultiMathFunctionType.Division:
                    MultiMathFunction up = functions[0],
                        low = functions[1];

                    vars.Clear();
                    up.GetAllVariables(ref vars);

                    if (vars.Contains(index))
                    {
                        for (int i = 0; i < right.Count; i++)
                            right[i] *= low;
                        left = up;
                    }

                    vars.Clear();
                    low.GetAllVariables(ref vars);

                    if (vars.Contains(index))
                    {
                        if (left != null)
                            throw new Exception(); // Unable to get right part for variable index

                        left = low;
                        for (int i = 0; i < right.Count; i++)
                            right[i] = up / right[i];
                    }

                    break;
                default:
                    break;
            }

            return left.GetFunctionForVariable(right, index);
        }

        private delegate void Operation(ref List<MultiMathFunction> right, MultiMathFunction func);
        private MultiMathFunction UpdateRightPart(ref List<MultiMathFunction> right, int index, Operation action)
        {
            MultiMathFunction left = null;
            HashSet<int> vars = new HashSet<int>();

            foreach (var func in functions)
            {
                vars.Clear();
                func.GetAllVariables(ref vars);

                if (vars.Contains(index))
                {
                    if (left != null)
                        throw new Exception(); // Unable to get right part for variable index

                    left = func;
                }
                else
                {
                    action(ref right, func);
                }
            }

            return left;
        }

        protected delegate MultiMathFunction[] UpdateFunc(MultiMathFunction func);
        protected void UpdateRightPart(ref List<MultiMathFunction> right, UpdateFunc func)
        {
            List<MultiMathFunction> newRight = new List<MultiMathFunction>();

            foreach (var f in right)
            {
                var arr = func(f);

                newRight.AddRange(arr);
            }

            right = newRight;
        }

        protected void NormalizeRighPart(ref List<MultiMathFunction> right)
        {
            for (int i = 0; i < right.Count; i++)
                right[i] /= coef;
        }

        #endregion
    }
    public class FoundationFunction : MultiMathFunction
    {
        protected MultiMathFunction foundation;

        public FoundationFunction() : base()
        {
            this.type = MultiMathFunctionType.Special;
        }
        public FoundationFunction(double coef, MultiMathFunction foundation) : this()
        {
            this.coef = coef;
            this.foundation = foundation.Clone() as MultiMathFunction;
        }
        public override void GetAllVariables(ref HashSet<int> vars)
        {
            foundation.GetAllVariables(ref vars);
        }

        public void SetFoundation(MultiMathFunction f)
        {
            foundation = f.Clone() as MultiMathFunction;
        }
        public void SetCoef(double coef)
        {
            this.coef = coef;
        }
    }

    public class ArgumentFunction : MultiMathFunction
    {
        private uint index;

        // Constructors
        public ArgumentFunction(double coef, uint index)
        {
            this.coef = coef;
            this.index = index;
            type = MultiMathFunctionType.Special;
        }

        // IClonable implementing
        public override object Clone()
        {
            return new ArgumentFunction(coef, index);
        }
 
        public override string ToString()
        {
            return string.Format("{0}x{1}", coef != 1 ? Math.Round(coef, 2).ToString() + " * " : "", index);
        }

        public override bool IsZero()
        {
            return coef == 0;
        }

        public override MultiMathFunction Derivative(int order, int index)
        {
            if (order == 0)
                return this;

            if (order > 1)
                return 0;

            if (this.index == (uint)index)
                return coef;

            return 0;
        }

        public override void GetAllVariables(ref HashSet<int> vars)
        {
            vars.Add((int)index);
        }

        public override double Calculate(Dictionary<uint, double> x)
        {
            return coef * x[index];
        }

        #region Transform to simple function

        public override MathFunction TransformToSimpleFunction(Dictionary<uint, double> variables)
        {
            if (variables.ContainsKey(index))
                return new ConstFunction(coef * variables[index]);
            else
                return new XFunction(coef);

        }
        public override MathFunction TransformToSimpleFunction(Dictionary<uint, MathFunction> transform)
        {
            return coef * transform[index];
        }

        #endregion

        public override MultiMathFunction[] GetFunctionForVariable(List<MultiMathFunction> right, int index)
        {
            if ((int)this.index != index)
                throw new Exception(); // left does not contain variable index

            NormalizeRighPart(ref right);

            return right.ToArray();
        }
    } 
    public class CosMultiFunction : FoundationFunction
    {
        public CosMultiFunction() : base() { }

        public CosMultiFunction(double coef, MultiMathFunction foundation) : base(coef, foundation)
        {
        }

        public override object Clone()
        {
            return new CosMultiFunction(coef, foundation);
        }

        public override string ToString()
        {
            return string.Format("{0}cos[{1}]", coef != 1 ? Math.Round(coef, 2).ToString() + " * " : "", foundation);
        }

        public override bool IsZero()
        {
            return (foundation - Math.PI / 2).IsZero();
        }

        public override MultiMathFunction Derivative(int order, int index)
        {
            if (order == 0)
                return this;

            var fDer = foundation.Derivative(1, index);

            if (fDer.IsZero())
                return 0;

            return (new SinMultiFunction(-coef, foundation) * fDer).Derivative(order - 1, index);
        }

        public override double Calculate(Dictionary<uint, double> x)
        {
            return coef * Math.Cos(foundation.Calculate(x));
        }

        #region Transform to simple function
        public override MathFunction TransformToSimpleFunction(Dictionary<uint, double> variables)
        {
            return new CosFunction(coef, foundation.TransformToSimpleFunction(variables));
        }
        public override MathFunction TransformToSimpleFunction(Dictionary<uint, MathFunction> transform)
        {
            return new CosFunction(coef, foundation.TransformToSimpleFunction(transform));
        }
        #endregion

        public override MultiMathFunction[] GetFunctionForVariable(List<MultiMathFunction> right, int index)
        {
            NormalizeRighPart(ref right);

            UpdateRightPart(ref right, x =>
            {
                int countOfSolutions = Constants.MultiMathFunctionsConstants.countOfSolutions;

                MultiMathFunction[] arr = new MultiMathFunction[countOfSolutions * 2];

                for (int i = 0; i < countOfSolutions; i++)
                {
                    arr[i] = new ACosMultiFunction(1.0, x) + 2 * Math.PI * i;
                    arr[i + 1] = new ACosMultiFunction(-1.0, x) + 2 * Math.PI * i;
                }

                return arr;
            });

            return foundation.GetFunctionForVariable(right, index);
        }
    }
    public class SinMultiFunction : FoundationFunction
    {
        public SinMultiFunction() : base() { }

        public SinMultiFunction(double coef, MultiMathFunction foundation) : base(coef, foundation)
        {
        }

        public override object Clone()
        {
            return new SinMultiFunction(coef, foundation);
        }

        public override string ToString()
        {
            return string.Format("{0}sin[{1}]", coef != 1 ? Math.Round(coef, 2).ToString() + " * " : "", foundation);
        }

        public override bool IsZero()
        {
            return foundation.IsZero();
        }

        public override MultiMathFunction Derivative(int order, int index)
        {
            if (order == 0)
                return this;

            var fDer = foundation.Derivative(1, index);

            if (fDer.IsZero())
                return 0;

            return (new CosMultiFunction(coef, foundation) * fDer).Derivative(order - 1, index);
        }

        public override double Calculate(Dictionary<uint, double> x)
        {
            return coef * Math.Sin(foundation.Calculate(x));
        }

        public override MathFunction TransformToSimpleFunction(Dictionary<uint, double> variables)
        {
            return new SinFunction(coef, foundation.TransformToSimpleFunction(variables));
        }
        public override MathFunction TransformToSimpleFunction(Dictionary<uint, MathFunction> transform)
        {
            return new SinFunction(coef, foundation.TransformToSimpleFunction(transform));
        }

        public override MultiMathFunction[] GetFunctionForVariable(List<MultiMathFunction> right, int index)
        {
            NormalizeRighPart(ref right);

            UpdateRightPart(ref right, x =>
            {
                int countOfSolutions = Constants.MultiMathFunctionsConstants.countOfSolutions;

                MultiMathFunction[] arr = new MultiMathFunction[countOfSolutions * 2];

                for (int i = 0; i < 2 * countOfSolutions; i++)
                {
                    arr[i] = new ASinMultiFunction(Math.Pow(-1, i), x) + Math.PI * i;
                }

                return arr;
            });

            return foundation.GetFunctionForVariable(right, index);
        }
    }
    public class LnMultiFunction : FoundationFunction
    {
        public LnMultiFunction() : base() { }

        public LnMultiFunction(double coef, MultiMathFunction foundation) : base(coef, foundation)
        {
        }

        public override object Clone()
        {
            return new LnMultiFunction(coef, foundation);
        }

        public override string ToString()
        {
            return string.Format("{0}ln[{1}]", coef != 1 ? Math.Round(coef, 2).ToString() + " * " : "", foundation.ToString());
        }

        public override bool IsZero()
        {
            return (foundation - 1).IsZero();
        }

        public override MultiMathFunction Derivative(int order, int index)
        {
            if (order == 0)
                return this;

            var der = foundation.Derivative(1, index);

            if (der.IsZero())
                return 0;

            return (coef / foundation * der).Derivative(order - 1, index);
        }

        public override double Calculate(Dictionary<uint, double> x)
        {
            return coef * Math.Log(foundation.Calculate(x));
        }

        public override MathFunction TransformToSimpleFunction(Dictionary<uint, double> variables)
        {
            return new LnFunction(coef, foundation.TransformToSimpleFunction(variables));
        }
        public override MathFunction TransformToSimpleFunction(Dictionary<uint, MathFunction> transform)
        {
            return new LnFunction(coef, foundation.TransformToSimpleFunction(transform));
        }

        public override MultiMathFunction[] GetFunctionForVariable(List<MultiMathFunction> right, int index)
        {
            NormalizeRighPart(ref right);

            for (int i = 0; i < right.Count; i++)
                right[i] = Math.E ^ right[i];

            return foundation.GetFunctionForVariable(right, index);
        }
    }
    public class PowerMultiFunction : MultiMathFunction
    {
        MultiMathFunction foundation;
        MultiMathFunction power;

        public PowerMultiFunction(double coef, MultiMathFunction foundation, MultiMathFunction power)
        {
            this.coef = coef;
            this.foundation = foundation.Clone() as MultiMathFunction;
            this.power = power.Clone() as MultiMathFunction;
            this.type = MultiMathFunctionType.Special;
        }

        public override object Clone()
        {
            return new PowerMultiFunction(coef, foundation, power);
        }
        
        public override string ToString()
        {
            return string.Format("{0}({1}) ^ [{2}]", coef != 1 ? Math.Round(coef, 2).ToString() + " * " : "", foundation, power);
        }

        public override bool IsZero()
        {
            return coef == 0;
        }

        public override MultiMathFunction Derivative(int order, int index)
        {
            if (order == 0)
                return this;

            if (foundation is ConstMultiFunction)
            {
                var pDer = power.Derivative(1, index);

                if (pDer.IsZero())
                    return 0;

                return ((Clone() as PowerMultiFunction) * Math.Log((double)(foundation as ConstMultiFunction))).Derivative(order - 1, index);
            } else
            {
                var fDer = foundation.Derivative(1, index);

                if (fDer.IsZero())
                    return 0;

                return (new PowerMultiFunction(coef, foundation, power + -1) * fDer * power).Derivative(order - 1, index);
            }
        }

        public override void GetAllVariables(ref HashSet<int> vars)
        {
            foundation.GetAllVariables(ref vars);
            power.GetAllVariables(ref vars);
        }

        public override double Calculate(Dictionary<uint, double> x)
        {
            return coef * Math.Pow(foundation.Calculate(x), power.Calculate(x));
        }

        public override MathFunction TransformToSimpleFunction(Dictionary<uint, double> variables)
        {
            if (foundation is ConstMultiFunction)
                return new StepFunction(coef, foundation.TransformToSimpleFunction(variables), power.TransformToSimpleFunction(variables));
            else 
                return new PowerFunction(coef, foundation.TransformToSimpleFunction(variables), power.TransformToSimpleFunction(variables));
        }
        public override MathFunction TransformToSimpleFunction(Dictionary<uint, MathFunction> transform)
        {
            if (foundation is ConstMultiFunction)
                return new StepFunction(coef, foundation.TransformToSimpleFunction(transform), power.TransformToSimpleFunction(transform));
            else
                return new PowerFunction(coef, foundation.TransformToSimpleFunction(transform), power.TransformToSimpleFunction(transform));
        }

        public override MultiMathFunction[] GetFunctionForVariable(List<MultiMathFunction> right, int index)
        {
            NormalizeRighPart(ref right);

            HashSet<int> vars = new HashSet<int>();
            UpdateFunc func = null;
            MultiMathFunction left = null;

            foundation.GetAllVariables(ref vars);
            if (vars.Contains(index) && (power is ConstMultiFunction && (power as ConstMultiFunction).IsEven()))
            {
                left = foundation;

                func = x => {
                    return new MultiMathFunction[] {
                    new PowerMultiFunction(1.0, x, 1 / power),
                    new PowerMultiFunction(-1.0, x, 1 / power) };
                };
            }
            else if (vars.Contains(index))
            {
                left = foundation;

                func = x => {
                    return new MultiMathFunction[] {
                    new PowerMultiFunction(1.0, x, 1 / power) };
                };
            }

            vars.Clear();
            power.GetAllVariables(ref vars);
            if (vars.Contains(index))
            {
                if (func != null)
                    throw new Exception(); // Unable to solve for variable index

                var number = (double)(foundation as ConstMultiFunction);

                func = x =>
                {
                    return new MultiMathFunction[]
                    {
                        new LnMultiFunction(1.0 / Math.Log(number), x)
                    };
                };

                left = power;
            }
        

            UpdateRightPart(ref right, func);

            return left.GetFunctionForVariable(right, index);
        }
    }
    public class ConstMultiFunction : MultiMathFunction
    {
        public ConstMultiFunction(double coef)
        {
            this.coef = coef;
            this.type = MultiMathFunctionType.Special;
        }

        public override object Clone()
        {
            return new ConstMultiFunction(coef);
        }

        public override string ToString()
        {
            return Math.Round(coef, 2).ToString();
        }

        public override MultiMathFunction Derivative(int order, int index)
        {
            if (order == 0)
                return this;

            return 0;
        }

        public override bool IsZero()
        {
            return coef == 0;
        }

        public override double Calculate(Dictionary<uint, double> x)
        {
            return coef;
        }

        public override MathFunction TransformToSimpleFunction(Dictionary<uint, double> variables)
        {
            return new ConstFunction(coef);
        }
        public override MathFunction TransformToSimpleFunction(Dictionary<uint, MathFunction> transform)
        {
            return coef;
        }

        public static explicit operator double(ConstMultiFunction f)
        {
            return f.coef;
        }

        public bool IsEven()
        {
            return coef % 2 == 0;
        }

        public override MultiMathFunction[] GetFunctionForVariable(List<MultiMathFunction> right, int index)
        {
            throw new Exception(); // Left part does not contain variable index
        }
    }
    public class ACosMultiFunction : FoundationFunction
    {
        public ACosMultiFunction() : base() { }

        public ACosMultiFunction(double coef, MultiMathFunction foundation) : base(coef, foundation)
        {
        }

        public override object Clone()
        {
            return new ACosMultiFunction(coef, foundation);
        }

        public override MathFunction TransformToSimpleFunction(Dictionary<uint, double> variables)
        {
            return new ACosFunction(coef, foundation.TransformToSimpleFunction(variables));
        }
        public override MathFunction TransformToSimpleFunction(Dictionary<uint, MathFunction> transform)
        {
            return new ACosFunction(coef, foundation.TransformToSimpleFunction(transform));
        }

        public override bool IsZero()
        {
            return (foundation - Math.PI / 2).IsZero();
        }

        public override string ToString()
        {
            return string.Format("{1}ACos[{0}]", foundation,
                coef == 1 ? "" : Math.Round(coef, 2) + " * ");
        }

        public override double Calculate(Dictionary<uint, double> x)
        {
            return coef * Math.Acos(foundation.Calculate(x));
        }

        public override MultiMathFunction Derivative(int order, int index)
        {
            if (order == 0)
                return this;

            return (-coef / (1 + (foundation ^ 2)) * foundation.Derivative(1, index)).Derivative(order - 1, index);
        }

        public override MultiMathFunction[] GetFunctionForVariable(List<MultiMathFunction> right, int index)
        {
            throw new NotImplementedException();
        }
    }
    public class ASinMultiFunction : FoundationFunction
    {
        public ASinMultiFunction() : base() { }

        public ASinMultiFunction(double coef, MultiMathFunction foundation) : base(coef, foundation)
        {
        }

        public override object Clone()
        {
            return new ASinMultiFunction(coef, foundation);
        }

        public override bool IsZero()
        {
            return foundation.IsZero();
        }

        public override double Calculate(Dictionary<uint, double> x)
        {
            return coef * Math.Asin(foundation.Calculate(x));
        }

        public override MathFunction TransformToSimpleFunction(Dictionary<uint, double> variables)
        {
            return new ASinFunction(coef, foundation.TransformToSimpleFunction(variables));
        }
        public override MathFunction TransformToSimpleFunction(Dictionary<uint, MathFunction> transform)
        {
            return new ASinFunction(coef, foundation.TransformToSimpleFunction(transform));
        }

        public override string ToString()
        {
            return string.Format("{1}ASin[{0}]", foundation,
                coef == 1 ? "" : Math.Round(coef, 2) + " * ");
        }

        public override MultiMathFunction Derivative(int order, int index)
        {
            if (order == 0)
                return this;

            return (coef / (1 + (foundation ^ 2)) * foundation.Derivative(1, index)).Derivative(order - 1, index);
        }

        public override MultiMathFunction[] GetFunctionForVariable(List<MultiMathFunction> right, int index)
        {
            throw new NotImplementedException();
        }
    }
}
