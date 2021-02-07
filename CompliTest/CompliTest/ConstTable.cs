using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompliTest
{
    /// <summary>
    /// 常量表
    /// </summary>
    public class ConstTable
    {
        /// <summary>
        /// 保留字表
        /// </summary>
        public static readonly string[] ReserverStrings = { 
                                                           "program",
                                                           "begin",
                                                           "end",
                                                           "var",
                                                           "int",
                                                           "and",
                                                           "or",
                                                           "not",
                                                           "if",
                                                           "then",
                                                           "else",
                                                           "while",
                                                           "do"
                                                   };

        public static readonly char[] SingleOperator = {
                                                           '+',
                                                           '*',
                                                           '(',
                                                           ')',
                                                           ',',
                                                           '.',
                                                           '=',
                                                           ';'
                                                   }; 

        /// <summary>
        /// 定义标识符的长度限制
        /// </summary>
        public const int IdentifyMaxLength = 10;

        /// <summary>
        /// 定义数字的长度限制
        /// </summary>
        public const int NumberMaxLength = 8;

        /// <summary>
        /// 定义单词符号的类型
        /// </summary>
        public enum VariableType
        {
            Reverse,
            Identity,
            Number,
            Operation
        };

        /// <summary>
        /// 语法分析输出信息的类型
        /// </summary>
        public enum GrammarMsgType
        {
            identified,
            error
        };

        public enum QuaternionType
        {
            BOOLEXP,
            CALCUEXP,
            FILLEXP,
            NEXP,
            WHILEEXP
        };
    }

}
