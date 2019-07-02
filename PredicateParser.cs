using System;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace RegexPredicateParser
{
    public class PredicateParser<T>
        where T: class
    {
        static string regexParanthesisString = @"(?<paranthesis>\w*\W*=\W*[\w\d])";
        static string regexExpressionString =
            @"(?<leftexp>[\s\w]+)(?<operator>(={2}|<=|>=|<|>){1})\s*(?<rightexp>[\'\w\d]+)";
        static string regexOperatorString = @"(?<operator>([\=\s\w]+((\|{2})|(\&{2}))?))";

        Regex parantethisRegex = new Regex(regexParanthesisString);
        Regex expressionRegex = new Regex(regexExpressionString);
        public Expression Parse(string query)
        {
            var match = expressionRegex.Match(query);

            if (!match.Success)
                throw new Exception($"Failed to parse the query {query}");

            var leftExp = match.Groups["leftexp"].Value;
            var rightExp = match.Groups["rightexp"].Value;
            var operatorStr = match.Groups["operator"].Value;

            if (string.IsNullOrWhiteSpace(leftExp))
            {
                throw new Exception(
                   $"Left expression can not be null or empty in expression '{query}'.");
            }
            
            if (string.IsNullOrWhiteSpace(rightExp))
            {
                throw new Exception(
                   $"Right expression can not be null or empty in expression '{query}'.");
            }

            leftExp = leftExp.Trim();
            rightExp = rightExp.Trim();

            var pe = Expression.Parameter(typeof(T),"t");

            Expression right = Expression.Constant(rightExp);
            Expression left = Expression.Property(pe, typeof(T).GetProperty(leftExp));

            return ApplyOperator(operatorStr, left, right);
        }

        public Expression ApplyOperator(
            string operatorStr,
            Expression left,
            Expression right)
        {
            if (string.IsNullOrWhiteSpace(operatorStr))
                throw new Exception("Operator can not be empty or null.");

            operatorStr = operatorStr.Trim();

            if (operatorStr == "==")
            {
                return Expression.Equal(left, right);
            }
            else if (operatorStr == "<=")
            {
                throw new NotImplementedException();
            }
            else if (operatorStr == ">=")
            {
                throw new NotImplementedException();
            }
            else if (operatorStr == ">")
            {
                throw new NotImplementedException();
            }
            else if (operatorStr == "<")
            {
                throw new NotImplementedException();
            }
            else
            {
                throw new Exception($"Invalid operator {operatorStr}.");
            }
        }

        public bool HasParanthesis(string query)
        {
            return parantethisRegex.IsMatch(query);
        }

    }
}
