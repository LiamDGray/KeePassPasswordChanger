using System;
using CefBrowserControl;

namespace KeePassPasswordChanger.Templates
{
    public class Condition
    {
        //allowed are:
        /*
         ==
         !=
         Contains
        */
        public string UsedOperator = "==";

        public static string[] AllowedOperators = new[] {"==", "!=", "Contains"};

        public string FirstOperand = BaseObject.ConvertStringToPlaceholderString("successfull"), SecondOperand = true.ToString();

        public string UCID;

        public Condition()
        {
            UCID = HashingEx.Hashing.GetSha1Hash(DateTime.Now.ToString() + " " + KeePassPasswordChangerExt.GeneratePassword(10, true, true, true, true, true, true, false, false).ReadString());
        }

        public Condition(string usedOperator, string firstOperand, string secondOperand) : this()
        {
            UsedOperator = usedOperator;
            FirstOperand = firstOperand;
            SecondOperand = secondOperand;
            HashingEx.Hashing.GetSha1Hash(DateTime.Now.ToString() + " " + firstOperand + secondOperand + usedOperator + KeePassPasswordChangerExt.GeneratePassword(10, true, true, true, true, true, true, false, false).ReadString());
        }

        public bool Compare(string firstOperand, string secondOperand)
        {
            switch (UsedOperator)
            {
                case "==":
                    return firstOperand == secondOperand;
                case "!=":
                    return firstOperand != secondOperand;
                case "Contains":
                    return firstOperand.Contains(secondOperand);
                default:
                {
                    ExceptionHandling.Handling.GetException("Unexpected",
                        new Exception("Operator " + UsedOperator + " was not expected!"));
                    return false;
                }
            }
        }

        public string ConditionToString(string firstOperand, string secondOperand)
        {
            return FirstOperand + " " + UsedOperator + " " + SecondOperand + " = " + firstOperand + " " + UsedOperator +
                   " " + secondOperand + " = " + Compare(firstOperand, secondOperand).ToString();
        }
    }
}
