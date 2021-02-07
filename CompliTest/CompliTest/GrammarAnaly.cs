using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompliTest
{
    /// <summary>
    /// 语法分析，采用LL1文法，按照递归向下分析程序分析
    /// </summary>
    class GrammarAnaly
    {
        #region 成员变量以及构造函数
		 /// <summary>
        /// 词法分析后生成的单词表
        /// </summary>
        private VariableTable variableTB;

        /// <summary>
        /// 目前匹配的位置，初始位置0
        /// </summary>
        private int matchIndex;
        public int MatchIndex
        {
            get { return this.matchIndex; }
        }

        /// <summary>
        /// 记录输出信息
        /// </summary>
        private GrammarMsgTable msgTB;
        public GrammarMsgTable MsgTB
        {
            get { return msgTB; }
        }

        /// <summary>
        /// 开始的匹配位置为0
        /// </summary>
        /// <param name="variableTB"></param>
        public GrammarAnaly(VariableTable variableTB)
        {
            this.variableTB = variableTB;
            this.matchIndex = 0;
            this.msgTB = new GrammarMsgTable();
        }
	    #endregion
        
        #region 函数(Function)文法的递归分析函数
        /// <summary>
        /// Function
        ///     错误代码：
        ///         0       无错误
        ///         -1      未匹配函数名
        ///         -2      未匹配函数名后的封号
        ///         -6      未匹配关键字begin
        ///         -21     未匹配关键字end
        ///         -22     未匹配关键字.
        /// </summary>
        /// <returns>
        /// </returns>
        public int FunctionGram()
        {
            string startWord = this.variableTB.GetItem(this.matchIndex).varValue;
            int startLine = this.variableTB.GetItem(this.matchIndex).lineNumber;

            if (this.variableTB.GetItem(this.matchIndex).varValue.Equals("program"))
                this.matchIndex++;
            else
            {
                msgTB.Add(new GrammarMsgTableItem("\t报错(第" + this.variableTB.GetItem(this.matchIndex).lineNumber + "行\"" + this.variableTB.GetItem(this.matchIndex).varValue + "\"单词附近)：缺乏函数声明关键字program", ConstTable.GrammarMsgType.error));
                return 0;
            }

            if (this.variableTB.GetItem(this.matchIndex).varType == ConstTable.VariableType.Identity)
                this.matchIndex++;
            else
            {
                msgTB.Add(new GrammarMsgTableItem("\t报错(第" + this.variableTB.GetItem(this.matchIndex).lineNumber + "行\"" + this.variableTB.GetItem(this.matchIndex).varValue + "\"单词附近)：函数缺乏函数名", ConstTable.GrammarMsgType.error));
                return -1;
            }

            if (this.variableTB.GetItem(this.matchIndex).varValue.Equals(";"))
                this.matchIndex++;
            else
            {
                msgTB.Add(new GrammarMsgTableItem("\t报错(第" + this.variableTB.GetItem(this.matchIndex).lineNumber + "行\"" + this.variableTB.GetItem(this.matchIndex).varValue + "\"单词附近)：函数名后缺乏封号", ConstTable.GrammarMsgType.error));
                return -2;
            }

            int CliamResult = ClaimGram();
            if (CliamResult != 0)
            {
                msgTB.Add(new GrammarMsgTableItem("\t报错(第" + this.variableTB.GetItem(this.matchIndex).lineNumber + "行\"" + this.variableTB.GetItem(this.matchIndex).varValue + "\"单词附近)：声明语句有误", ConstTable.GrammarMsgType.error));
                return CliamResult;
            }

            if (this.variableTB.GetItem(this.matchIndex).varValue.Equals("begin"))
                this.matchIndex++;
            else
            {
                msgTB.Add(new GrammarMsgTableItem("\t报错(第" + this.variableTB.GetItem(this.matchIndex).lineNumber + "行\"" + this.variableTB.GetItem(this.matchIndex).varValue + "\"单词附近)：函数缺乏关键字begin", ConstTable.GrammarMsgType.error));
                return -6;
            }

            int ExpressionResult = ExpressionGram();
            if (ExpressionResult != 0)
            {
                msgTB.Add(new GrammarMsgTableItem("\t报错(第" + this.variableTB.GetItem(this.matchIndex).lineNumber + "行\"" + this.variableTB.GetItem(this.matchIndex).varValue + "\"单词附近)：函数主体语句有误", ConstTable.GrammarMsgType.error));
                return ExpressionResult;
            }


            if (this.variableTB.GetItem(this.matchIndex).varValue.Equals("end"))
                this.matchIndex++;
            else
            {
                msgTB.Add(new GrammarMsgTableItem("\t报错(第" + this.variableTB.GetItem(this.matchIndex).lineNumber + "行\"" + this.variableTB.GetItem(this.matchIndex).varValue + "\"单词附近)：函数语句缺乏关键字end", ConstTable.GrammarMsgType.error));
                return -21;
            }

            if (this.variableTB.GetItem(this.matchIndex).varValue.Equals("."))
                this.matchIndex++;
            else
            {
                msgTB.Add(new GrammarMsgTableItem("\t报错(第" + this.variableTB.GetItem(this.matchIndex).lineNumber + "行\"" + this.variableTB.GetItem(this.matchIndex).varValue + "\"单词附近)：函数语句缺乏结尾的点号", ConstTable.GrammarMsgType.error));
                return -22;
            }

            string endWord = this.variableTB.GetItem(this.matchIndex - 1).varValue;
            int endLine = this.variableTB.GetItem(this.matchIndex - 1).lineNumber;

            
             msgTB.Add(new GrammarMsgTableItem("第" + startLine + "行单词" + startWord + "到第" + endLine + "行单词" + endWord + "之间识别为：函数", ConstTable.GrammarMsgType.identified));

            return 0;
        }
        #endregion

        #region 声明(Claim)文法的递归分析函数
        /// <summary>
        /// Cliam
        /// </summary>
        /// <returns>
        ///     返回代码：
        ///         0   无错误
        ///         -3  未匹配关键字int
        ///         -5  未匹配封号
        ///         
        /// </returns>
        public int ClaimGram()
        {
            string startWord = this.variableTB.GetItem(this.matchIndex).varValue;
            int startLine = this.variableTB.GetItem(this.matchIndex).lineNumber;

            if (this.variableTB.GetItem(this.matchIndex).varValue.Equals("var"))
                this.matchIndex++;
            else
                return 0;

            if (this.variableTB.GetItem(this.matchIndex).varValue.Equals("int"))
                this.matchIndex++;
            else
            {
                msgTB.Add(new GrammarMsgTableItem("\t报错(第" + this.variableTB.GetItem(this.matchIndex).lineNumber + "行\"" + this.variableTB.GetItem(this.matchIndex).varValue + "\"单词附近)：声明语句缺乏类型的定义", ConstTable.GrammarMsgType.error));
                return -3;
            }

            int IdentityGroupResult = IdentityGroupGram();
            if (IdentityGroupResult != 0)
            {
                msgTB.Add(new GrammarMsgTableItem("\t报错(第" + this.variableTB.GetItem(this.matchIndex).lineNumber + "行\"" + this.variableTB.GetItem(this.matchIndex).varValue + "\"单词附近)：声明语句缺乏变量", ConstTable.GrammarMsgType.error));
                return IdentityGroupResult;
            }


            if (this.variableTB.GetItem(this.matchIndex).varValue.Equals(";"))
                this.matchIndex++;
            else
            {
                msgTB.Add(new GrammarMsgTableItem("\t报错(第" + this.variableTB.GetItem(this.matchIndex).lineNumber + "行\"" + this.variableTB.GetItem(this.matchIndex).varValue + "\"单词附近)：声明语句缺乏结尾的封号", ConstTable.GrammarMsgType.error));
                return -5;
            }

            string endWord = this.variableTB.GetItem(this.matchIndex - 1).varValue;
            int endLine = this.variableTB.GetItem(this.matchIndex - 1).lineNumber;

            msgTB.Add(new GrammarMsgTableItem("第" + startLine + "行单词" + startWord + "到第" + endLine + "行单词" + endWord + "之间识别为：变量声明语句",ConstTable.GrammarMsgType.identified));

            return 0;
        }
        
        /// <summary>
        /// IdentityGroup
        ///     错误代码：
        ///         0   无错误
        ///         -4  未匹配标识符
        /// </summary>
        /// <returns>
        /// </returns>
        public int IdentityGroupGram()
        {
            if (this.variableTB.GetItem(this.matchIndex).varType==ConstTable.VariableType.Identity)
                this.matchIndex++;
            else
                return -4;

            int IdentityGroup_Result = IdentityGroup_Gram();
            if (IdentityGroup_Result != 0)
                return IdentityGroup_Result;


            return 0;
        }

        /// <summary>
        /// IdentityGroup_
        ///     错误代码：
        ///         0   无错误
        /// </summary>
        /// <returns>
        /// </returns>
        public int IdentityGroup_Gram()
        {
            if (this.variableTB.GetItem(this.matchIndex).varValue.Equals(","))
                this.matchIndex++;
            else
                return 0;

            int IdentityGroupResult = IdentityGroupGram();
            if (IdentityGroupResult != 0)
                return IdentityGroupResult;

            return 0;
        }
        #endregion

        #region 表达式(Expression)文法的递归分析程序
        /// <summary>
        /// Expression
        ///     错误代码：
        ///         0   无错误
        /// </summary>
        /// <returns>
        /// </returns>
        public int ExpressionGram()
        {

            return SentenceGram();
        }
        #endregion

        #region 算术表达式(CalcuExp)文法的递归分析程序
        /// <summary>
        /// CalcuExp
        ///     错误代码:
        ///         0   无错误
        ///         -7  未匹配算术表达式的括号
        /// </summary>
        /// <returns>
        /// </returns>
        public int CalcuExpGram()
        {
            if (this.variableTB.GetItem(this.matchIndex).varValue.Equals("("))
            {
                this.matchIndex++;
                int CalcuExpResult = CalcuExpGram();
                if (CalcuExpResult != 0)
                    return CalcuExpResult;

                if (this.variableTB.GetItem(this.matchIndex).varValue.Equals(")"))
                    this.matchIndex++;
                else
                {
                    msgTB.Add(new GrammarMsgTableItem("\t报错(第" + this.variableTB.GetItem(this.matchIndex).lineNumber + "行\"" + this.variableTB.GetItem(this.matchIndex).varValue + "\"单词附近)：算术表达式括号不匹配", ConstTable.GrammarMsgType.error));
                    return -7;
                }

                int Calculator_Result = Calculator_Gram();
                if (Calculator_Result != 0)
                    return Calculator_Result;

                return 0;
            }
            else
            {
                return CalculatorGram();

            }
            
        }

        /// <summary>
        /// Calculator
        ///     错误代码：
        ///         0   无错误
        /// </summary>
        /// <returns>
        /// </returns>
        public int CalculatorGram()
        {
            int EntityResult = EntityGram();
            if (EntityResult != 0)
            {
                msgTB.Add(new GrammarMsgTableItem("\t报错(第" + this.variableTB.GetItem(this.matchIndex).lineNumber + "行\"" + this.variableTB.GetItem(this.matchIndex).varValue + "\"单词附近)：算术表达式缺乏运算的实体", ConstTable.GrammarMsgType.error));
                return EntityResult;
            }

            int Calculator_Result = Calculator_Gram();
            if (Calculator_Result != 0)
                return Calculator_Result;

            return 0;
        }

        /// <summary>
        /// Calculator_
        ///     错误代码：
        ///         0   无错误
        /// </summary>
        /// <returns>
        /// </returns>
        public int Calculator_Gram()
        {
            int CalOptResult = CalOptGram();
            if(CalOptResult!=0)
                return 0;

            int CalcuExpResult = CalcuExpGram();
            if (CalcuExpResult != 0)
                return CalcuExpResult;

            int Calculator_Result=Calculator_Gram();
            if(Calculator_Result!=0)
                return Calculator_Result;

            return 0;
        }

        /// <summary>
        /// CalOpt
        ///     错误代码：
        ///         0   无错误
        ///         -9  未匹配算术运算符
        /// </summary>
        /// <returns>
        /// </returns>
        public int CalOptGram()
        {
            if (this.variableTB.GetItem(this.matchIndex).varValue.Equals("+"))
                this.matchIndex++;
            else if (this.variableTB.GetItem(this.matchIndex).varValue.Equals("*"))
                this.matchIndex++;
            else
                return -9;

            return 0;
        }

        /// <summary>
        /// Entity
        ///     错误代码：
        ///         0   无错误
        ///         -8  未匹配标识符或数字
        /// </summary>
        /// <returns>
        /// </returns>
        public int EntityGram()
        {
            if (this.variableTB.GetItem(this.matchIndex).varType == ConstTable.VariableType.Identity)
                this.matchIndex++;
            else if (this.variableTB.GetItem(this.matchIndex).varType == ConstTable.VariableType.Number)
                this.matchIndex++;
            else
                return -8;
                

            return 0;
        }
        #endregion

        #region 布尔表达式(BoolExp)文法的递归分析程序
        /// <summary>
        /// BoolExp
        ///     错误代码：
        ///         0   无错误
        ///         -10 未匹配布尔表达式括号
        /// </summary>
        /// <returns>
        /// </returns>
        public int BoolExpGram()
        {
            if (this.variableTB.GetItem(this.matchIndex).varValue.Equals("("))
            {
                this.matchIndex++;

                int BoolExpResult = BoolExpGram();
                if (BoolExpResult != 0)
                    return BoolExpResult;

                if (this.variableTB.GetItem(this.matchIndex).varValue.Equals(")"))
                    this.matchIndex++;
                else
                {
                    msgTB.Add(new GrammarMsgTableItem("\t报错(第" + this.variableTB.GetItem(this.matchIndex).lineNumber + "行\"" + this.variableTB.GetItem(this.matchIndex).varValue + "\"单词附近)：布尔表达式括号不匹配", ConstTable.GrammarMsgType.error));
                    return -10;
                }

                int BoolExp_result = BoolExp_Gram();
                if (BoolExp_result != 0)
                    return BoolExp_result;

                return 0;
            }
            else if (this.variableTB.GetItem(this.matchIndex).varValue.Equals("not"))
            {
                this.matchIndex++;

                int BoolExpResult = BoolExpGram();
                if (BoolExpResult != 0)
                    return BoolExpResult;

                return 0;
            }
            else
            {
                int CompareEResult = CompareEGram();
                if (CompareEResult != 0)
                    return CompareEResult;

                int BoolExp_Result = BoolExp_Gram();
                if (BoolExp_Result != 0)
                    return BoolExp_Result;

                return 0;
            }

            
        }

        /// <summary>
        /// BoolExp_
        ///     错误代码：
        ///         0   无错误
        ///         
        /// </summary>
        /// <returns>
        /// </returns>
        public int BoolExp_Gram()
        {
            int LogicOptResult = LogicOptGram();
            if (LogicOptResult != 0)
                return 0;

            int BoolExpResult = BoolExpGram();
            if (BoolExpResult != 0)
                return BoolExpResult;

            int BoolExp_Result = BoolExp_Gram();
            if (BoolExp_Result != 0)
                return BoolExp_Result;

            return 0;
        }

        /// <summary>
        /// LogicOpt
        ///     错误代码：
        ///         0   无错误
        ///         -11 未匹配逻辑运算符
        /// </summary>
        /// <returns>
        /// </returns>
        public int LogicOptGram()
        {
            if (this.variableTB.GetItem(this.matchIndex).varValue.Equals("and"))
                this.matchIndex++;
            else if (this.variableTB.GetItem(this.matchIndex).varValue.Equals("or"))
                this.matchIndex++;
            else
                return -11;
            return 0;
        }

        /// <summary>
        /// CompareE
        ///     错误代码：
        ///         0   无错误
        /// </summary>
        /// <returns></returns>
        public int CompareEGram()
        {
            int EntityResult = EntityGram();
            if (EntityResult != 0)
            {
                msgTB.Add(new GrammarMsgTableItem("\t报错(第" + this.variableTB.GetItem(this.matchIndex).lineNumber + "行\"" + this.variableTB.GetItem(this.matchIndex).varValue+ "\"单词附近)：比较表达式缺乏比较的实体", ConstTable.GrammarMsgType.error));
                return EntityResult;
            }

            int BoolOptResult = BoolOptGram();
            if (BoolOptResult != 0)
            {
                msgTB.Add(new GrammarMsgTableItem("\t报错(第" + this.variableTB.GetItem(this.matchIndex).lineNumber + "行\"" + this.variableTB.GetItem(this.matchIndex).varValue + "\"单词附近)：比较表达式缺乏比较运算符", ConstTable.GrammarMsgType.error));
                return BoolOptResult;
            }

            int EntityResult2 = EntityGram();
            if (EntityResult2 != 0)
            {
                msgTB.Add(new GrammarMsgTableItem("\t报错(第" + this.variableTB.GetItem(this.matchIndex).lineNumber + "行\"" + this.variableTB.GetItem(this.matchIndex).varValue + "\"单词附近)：比较表达式缺乏被比较的实体", ConstTable.GrammarMsgType.error));
                return EntityResult2;
            }

            return 0;
        }

        /// <summary>
        /// BoolOpt
        ///     错误代码：
        ///         0   无错误
        ///         -12 未匹配布尔运算符
        /// </summary>
        /// <returns></returns>
        public int BoolOptGram()
        {
            if (this.variableTB.GetItem(this.matchIndex).varValue.Equals("=") ||
               this.variableTB.GetItem(this.matchIndex).varValue.Equals(">") ||
               this.variableTB.GetItem(this.matchIndex).varValue.Equals(">=") ||
               this.variableTB.GetItem(this.matchIndex).varValue.Equals("<") ||
               this.variableTB.GetItem(this.matchIndex).varValue.Equals("<=") ||
               this.variableTB.GetItem(this.matchIndex).varValue.Equals("<>"))
                this.matchIndex++;
            else
                return -12;

            return 0;
        }
        #endregion

        #region 语句(Sentence)文法的递归分析程序
        /// <summary>
        /// Sentence
        ///     错误代码：
        ///         0   无错误
        /// </summary>
        /// <returns></returns>
        public int SentenceGram()
        {
             if (this.variableTB.GetItem(this.matchIndex).varType==ConstTable.VariableType.Identity)
            {
                int FillSentenceResult = FillSentenceGram();
                if (FillSentenceResult != 0)
                {
                    msgTB.Add(new GrammarMsgTableItem("\t报错(第" + this.variableTB.GetItem(this.matchIndex).lineNumber + "行\"" + this.variableTB.GetItem(this.matchIndex).varValue + "\"单词附近)：赋值语句有误", ConstTable.GrammarMsgType.error));
                    return FillSentenceResult;
                }

                return SentenceGram();
            }
            else if (this.variableTB.GetItem(this.matchIndex).varValue.Equals("if"))
            {
                int FZSentenceResult = FZSentenceGram();
                if (FZSentenceResult != 0)
                {
                    msgTB.Add(new GrammarMsgTableItem("\t报错(第" + this.variableTB.GetItem(this.matchIndex).lineNumber + "行\"" + this.variableTB.GetItem(this.matchIndex).varValue + "\"单词附近)：if分支语句有误", ConstTable.GrammarMsgType.error));
                    return FZSentenceResult;
                }

                return SentenceGram();
            }
            else if (this.variableTB.GetItem(this.matchIndex).varValue.Equals("while"))
            {
                int XHSentenceResult = XHSentenceGram();
                if (XHSentenceResult != 0)
                {
                    msgTB.Add(new GrammarMsgTableItem("\t报错(第" + this.variableTB.GetItem(this.matchIndex).lineNumber + "行\"" + this.variableTB.GetItem(this.matchIndex).varValue + "\"单词附近)：while循环有误", ConstTable.GrammarMsgType.error));
                    return XHSentenceResult;
                }

                return SentenceGram();
            }
            else if (this.variableTB.GetItem(this.matchIndex).varValue.Equals("begin"))
            {
                int ComplexSentenceResult = ComplexSentenceGram();
                if (ComplexSentenceResult != 0)
                {
                    msgTB.Add(new GrammarMsgTableItem("\t报错(第" + this.variableTB.GetItem(this.matchIndex).lineNumber + "行\"" + this.variableTB.GetItem(this.matchIndex).varValue + "\"单词附近)：复合语句语句有错误", ConstTable.GrammarMsgType.error));
                    return ComplexSentenceResult;
                }

                return SentenceGram();
            }
            return 0;
        }
        #endregion

        #region 赋值语句(FillSentence)文法的递归分析程序
        /// <summary>
        /// FillSentence
        ///     错误代码：
        ///         0   无错误
        ///         -13 未匹配赋值语句变量
        ///         -14 未匹配赋值号
        ///         -23 未匹配赋值表达式封号
        /// </summary>
        /// <returns></returns>
        public int FillSentenceGram()
        {
            string startWord = this.variableTB.GetItem(this.matchIndex).varValue;
            int startLine = this.variableTB.GetItem(this.matchIndex).lineNumber;

            if (this.variableTB.GetItem(this.matchIndex).varType == ConstTable.VariableType.Identity)
                this.matchIndex++;
            else
            {
                msgTB.Add(new GrammarMsgTableItem("\t报错(第" + this.variableTB.GetItem(this.matchIndex).lineNumber + "行\"" + this.variableTB.GetItem(this.matchIndex).varValue + "\"单词附近)：赋值语句未指定被赋值的变量", ConstTable.GrammarMsgType.error));
                return -13;
            }


            if (this.variableTB.GetItem(this.matchIndex).varValue.Equals(":="))
                this.matchIndex++;
            else
            {
                msgTB.Add(new GrammarMsgTableItem("\t报错(第" + this.variableTB.GetItem(this.matchIndex).lineNumber + "行\"" + this.variableTB.GetItem(this.matchIndex).varValue + "\"单词附近)：赋值语句缺乏赋值符号", ConstTable.GrammarMsgType.error));
                return -14;
            }

            int FillSentence_Result = FillSentence_Gram();
            if (FillSentence_Result != 0)
            {
                msgTB.Add(new GrammarMsgTableItem("\t报错(第" + this.variableTB.GetItem(this.matchIndex).lineNumber + "行\"" + this.variableTB.GetItem(this.matchIndex).varValue + "\"单词附近)：赋值语句赋值号右部的表达式有误", ConstTable.GrammarMsgType.error));
                return FillSentence_Result;
            }

            if (this.variableTB.GetItem(this.matchIndex).varValue.Equals(";"))
                this.matchIndex++;
            else
            {
                msgTB.Add(new GrammarMsgTableItem("\t报错(第" + this.variableTB.GetItem(this.matchIndex).lineNumber + "行\"" + this.variableTB.GetItem(this.matchIndex).varValue + "\"单词附近)：赋值语句缺乏结尾的封号", ConstTable.GrammarMsgType.error));
                return -23;
            }

            string endWord = this.variableTB.GetItem(this.matchIndex - 1).varValue;
            int endLine = this.variableTB.GetItem(this.matchIndex - 1).lineNumber;


            msgTB.Add(new GrammarMsgTableItem("第" + startLine + "行单词" + startWord + "到第" + endLine + "行单词" + endWord + "之间识别为：赋值语句",ConstTable.GrammarMsgType.identified));

            return 0;
        }

        /// <summary>
        /// FillSentence_
        ///     错误代码：
        ///         0   无错误
        /// </summary>
        /// <returns></returns>
        public int FillSentence_Gram()
        {
            return CalcuExpGram();
        }
        #endregion

        #region 分支语句(FZSentence)文法的递归分析程序
        /// <summary>
        /// FZSentence
        ///     错误代码：
        ///         0   无错误
        ///         -15 未匹配If
        ///         -16 未匹配Then
        /// </summary>
        /// <returns></returns>
        public int FZSentenceGram()
        {
            string startWord = this.variableTB.GetItem(this.matchIndex).varValue;
            int startLine = this.variableTB.GetItem(this.matchIndex).lineNumber;

            bool elseFlag = false;

            if (this.variableTB.GetItem(this.matchIndex).varValue.Equals("if"))
                this.matchIndex++;
            else
            {
                msgTB.Add(new GrammarMsgTableItem("\t报错(第" + this.variableTB.GetItem(this.matchIndex).lineNumber + "行\"" + this.variableTB.GetItem(this.matchIndex).varValue + "\"单词附近)：if分支语句缺乏关键字if", ConstTable.GrammarMsgType.error));
                return -15;
            }

            int BoolExpResult = BoolExpGram();
            if (BoolExpResult != 0)
            {
                msgTB.Add(new GrammarMsgTableItem("\t报错(第" + this.variableTB.GetItem(this.matchIndex).lineNumber + "行\"" + this.variableTB.GetItem(this.matchIndex).varValue + "\"单词附近)：if分支语句判断部分出错", ConstTable.GrammarMsgType.error));
                return BoolExpResult;
            }

            if (this.variableTB.GetItem(this.matchIndex).varValue.Equals("then"))
                this.matchIndex++;
            else
            {
                msgTB.Add(new GrammarMsgTableItem("\t报错(第" + this.variableTB.GetItem(this.matchIndex).lineNumber + "行\"" + this.variableTB.GetItem(this.matchIndex).varValue + "\"单词附近)：if分支语句缺乏关键字then", ConstTable.GrammarMsgType.error));
                return -16;
            }

            if (this.variableTB.GetItem(this.matchIndex).varType == ConstTable.VariableType.Identity)
            {
                int FillSentenceResult = FillSentenceGram();
                if (FillSentenceResult != 0)
                {
                    msgTB.Add(new GrammarMsgTableItem("\t报错(第" + this.variableTB.GetItem(this.matchIndex).lineNumber + "行\"" + this.variableTB.GetItem(this.matchIndex).varValue + "\"单词附近)：if分支语句then后的语句有错误", ConstTable.GrammarMsgType.error));
                    return FillSentenceResult;
                }
            }
            else if (this.variableTB.GetItem(this.matchIndex).varValue.Equals("if"))
            {
                int FZSentenceResult = FZSentenceGram();
                if (FZSentenceResult != 0)
                {
                    msgTB.Add(new GrammarMsgTableItem("\t报错(第" + this.variableTB.GetItem(this.matchIndex).lineNumber + "行\"" + this.variableTB.GetItem(this.matchIndex).varValue + "\"单词附近)：if分支语句then后的语句有错误", ConstTable.GrammarMsgType.error));
                    return FZSentenceResult;
                }
            }
            else if (this.variableTB.GetItem(this.matchIndex).varValue.Equals("while"))
            {
                int XHSentenceResult = XHSentenceGram();
                if (XHSentenceResult != 0)
                {
                    msgTB.Add(new GrammarMsgTableItem("\t报错(第" + this.variableTB.GetItem(this.matchIndex).lineNumber + "行\"" + this.variableTB.GetItem(this.matchIndex).varValue + "\"单词附近)：if分支语句then后的语句有错误", ConstTable.GrammarMsgType.error));
                    return XHSentenceResult;
                }
            }
            else if (this.variableTB.GetItem(this.matchIndex).varValue.Equals("begin"))
            {
                int ComplexSentenceResult = ComplexSentenceGram();
                if (ComplexSentenceResult != 0)
                {
                    msgTB.Add(new GrammarMsgTableItem("\t报错(第" + this.variableTB.GetItem(this.matchIndex).lineNumber + "行\"" + this.variableTB.GetItem(this.matchIndex).varValue + "\"单词附近)：if分支语句then后的语句有错误", ConstTable.GrammarMsgType.error));
                    return ComplexSentenceResult;
                }
            }

            if (this.variableTB.GetItem(this.matchIndex).varValue.Equals("else"))
                elseFlag = true;

            int FZSentence_Result = FZSentence_Gram();
            if (FZSentence_Result != 0)
            {
                msgTB.Add(new GrammarMsgTableItem("\t报错(第" + this.variableTB.GetItem(this.matchIndex).lineNumber + "行\"" + this.variableTB.GetItem(this.matchIndex).varValue + "\"单词附近)：if分支语句else后的语句有错误", ConstTable.GrammarMsgType.error));
                return FZSentence_Result;
            }

            string endWord = this.variableTB.GetItem(this.matchIndex - 1).varValue;
            int endLine = this.variableTB.GetItem(this.matchIndex - 1).lineNumber;

            if(!elseFlag)
                msgTB.Add(new GrammarMsgTableItem("第"+startLine+"行单词"+startWord+"到第"+endLine+"行单词"+endWord+"之间识别为：if-then分支语句",ConstTable.GrammarMsgType.identified));
            else
                msgTB.Add(new GrammarMsgTableItem("第" + startLine + "行单词" + startWord + "到第" + endLine + "行单词" + endWord + "之间识别为：if-then-else分支语句",ConstTable.GrammarMsgType.identified));
            return 0;
        }

        /// <summary>
        /// FZSentence_
        ///     错误代码：
        ///         0   无错误
        /// </summary>
        /// <returns></returns>
        public int FZSentence_Gram()
        {
            if (this.variableTB.GetItem(this.matchIndex).varValue.Equals("else"))
                this.matchIndex++;
            else
                return 0;

            if (this.variableTB.GetItem(this.matchIndex).varType == ConstTable.VariableType.Identity)
            {
                int FillSentenceResult = FillSentenceGram();
                if (FillSentenceResult != 0)
                    return FillSentenceResult;
            }
            else if (this.variableTB.GetItem(this.matchIndex).varValue.Equals("if"))
            {
                int FZSentenceResult = FZSentenceGram();
                if (FZSentenceResult != 0)
                    return FZSentenceResult;
            }
            else if (this.variableTB.GetItem(this.matchIndex).varValue.Equals("while"))
            {
                int XHSentenceResult = XHSentenceGram();
                if (XHSentenceResult != 0)
                    return XHSentenceResult;
            }
            else if (this.variableTB.GetItem(this.matchIndex).varValue.Equals("begin"))
            {
                int ComplexSentenceResult = ComplexSentenceGram();
                if (ComplexSentenceResult != 0)
                    return ComplexSentenceResult;
            }

            return 0;
        }
        #endregion

        #region 循环语句(XHSentence)文法的递归分析程序
        /// <summary>
        /// XHSentence
        ///     错误代码：
        ///         0   无错误
        ///         -17 未匹配while
        ///         -18 未匹配do
        /// </summary>
        /// <returns></returns>
        public int XHSentenceGram()
        {
            string startWord = this.variableTB.GetItem(this.matchIndex).varValue;
            int startLine = this.variableTB.GetItem(this.matchIndex).lineNumber;

            if (this.variableTB.GetItem(this.matchIndex).varValue.Equals("while"))
                this.matchIndex++;
            else
            {
                msgTB.Add(new GrammarMsgTableItem("\t报错(第" + this.variableTB.GetItem(this.matchIndex).lineNumber + "行\"" + this.variableTB.GetItem(this.matchIndex).varValue + "\"单词附近)：while循环语句缺乏关键字while", ConstTable.GrammarMsgType.error));
                return -17;
            }

            int BoolExpResult = BoolExpGram();
            if (BoolExpResult != 0)
            {
                msgTB.Add(new GrammarMsgTableItem("\t报错(第" + this.variableTB.GetItem(this.matchIndex).lineNumber + "行\"" + this.variableTB.GetItem(this.matchIndex).varValue + "\"单词附近)：while循环语句中判断部分出错", ConstTable.GrammarMsgType.error));
                return BoolExpResult;
            }

            if (this.variableTB.GetItem(this.matchIndex).varValue.Equals("do"))
                this.matchIndex++;
            else
            {
                msgTB.Add(new GrammarMsgTableItem("\t报错(第" + this.variableTB.GetItem(this.matchIndex).lineNumber + "行\"" + this.variableTB.GetItem(this.matchIndex).varValue + "\"单词附近)：while循环语句中缺乏关键字do", ConstTable.GrammarMsgType.error));
                return -18;
            }

            if (this.variableTB.GetItem(this.matchIndex).varType == ConstTable.VariableType.Identity)
            {
                int FillSentenceResult = FillSentenceGram();
                if (FillSentenceResult != 0)
                {
                    msgTB.Add(new GrammarMsgTableItem("\t报错(第" + this.variableTB.GetItem(this.matchIndex).lineNumber + "行\"" + this.variableTB.GetItem(this.matchIndex).varValue + "\"单词附近)：while循环中do后的语句有错误", ConstTable.GrammarMsgType.error));
                    return FillSentenceResult;
                }
            }
            else if (this.variableTB.GetItem(this.matchIndex).varValue.Equals("if"))
            {
                int FZSentenceResult = FZSentenceGram();
                if (FZSentenceResult != 0)
                {
                    msgTB.Add(new GrammarMsgTableItem("\t报错(第" + this.variableTB.GetItem(this.matchIndex).lineNumber + "行\"" + this.variableTB.GetItem(this.matchIndex).varValue + "\"单词附近)：while循环中do后的语句有错误", ConstTable.GrammarMsgType.error));
                    return FZSentenceResult;
                }
            }
            else if (this.variableTB.GetItem(this.matchIndex).varValue.Equals("while"))
            {
                int XHSentenceResult = XHSentenceGram();
                if (XHSentenceResult != 0)
                {
                    msgTB.Add(new GrammarMsgTableItem("\t报错(第" + this.variableTB.GetItem(this.matchIndex).lineNumber + "行\"" + this.variableTB.GetItem(this.matchIndex).varValue + "\"单词附近)：while循环中do后的语句有错误", ConstTable.GrammarMsgType.error));
                    return XHSentenceResult;
                }
            }
            else if (this.variableTB.GetItem(this.matchIndex).varValue.Equals("begin"))
            {
                int ComplexSentenceResult = ComplexSentenceGram();
                if (ComplexSentenceResult != 0)
                {
                    msgTB.Add(new GrammarMsgTableItem("\t报错(第" + this.variableTB.GetItem(this.matchIndex).lineNumber + "行\"" + this.variableTB.GetItem(this.matchIndex).varValue + "\"单词附近)：while循环中do后的语句有错误", ConstTable.GrammarMsgType.error));
                    return ComplexSentenceResult;
                }
            }

            string endWord = this.variableTB.GetItem(this.matchIndex - 1).varValue;
            int endLine = this.variableTB.GetItem(this.matchIndex - 1).lineNumber;

            msgTB.Add(new GrammarMsgTableItem("第" + startLine + "行单词" + startWord + "到第" + endLine + "行单词" + endWord + "之间识别为：while循环语句",ConstTable.GrammarMsgType.identified));

            return 0;
        }
        #endregion

        #region 复杂语句(ComplexSentence)文法的递归分析程序
        /// <summary>
        /// ComplexSentence
        ///     错误代码：
        ///         0   无错误
        ///         -19 未匹配复杂语句begin
        ///         -20 未匹配复杂语句end
        /// </summary>
        /// <returns></returns>
        public int ComplexSentenceGram()
        {
            string startWord = this.variableTB.GetItem(this.matchIndex).varValue;
            int startLine = this.variableTB.GetItem(this.matchIndex).lineNumber;

            if (this.variableTB.GetItem(this.matchIndex).varValue.Equals("begin"))
                this.matchIndex++;
            else
            {
                msgTB.Add(new GrammarMsgTableItem("\t报错(第" + this.variableTB.GetItem(this.matchIndex).lineNumber + "行\"" + this.variableTB.GetItem(this.matchIndex).varValue + "\"单词附近)：复合语句中缺乏begin关键字", ConstTable.GrammarMsgType.error));
                return -19;
            }

            int SentenceResult = SentenceGram();
            if (SentenceResult != 0)
            {
                msgTB.Add(new GrammarMsgTableItem("\t报错(第" + this.variableTB.GetItem(this.matchIndex).lineNumber + "行\"" + this.variableTB.GetItem(this.matchIndex).varValue + "\"单词附近)：复合语句中语句出错", ConstTable.GrammarMsgType.error));
                return SentenceResult;
            }

            if (this.variableTB.GetItem(this.matchIndex).varValue.Equals("end"))
                this.matchIndex++;
            else
            {
                msgTB.Add(new GrammarMsgTableItem("\t报错(第" + this.variableTB.GetItem(this.matchIndex).lineNumber + "行\"" + this.variableTB.GetItem(this.matchIndex).varValue + "\"单词附近)：复合语句中缺乏end关键字", ConstTable.GrammarMsgType.error));
                return -20;
            }

            string endWord = this.variableTB.GetItem(this.matchIndex-1).varValue;
            int endLine = this.variableTB.GetItem(this.matchIndex-1).lineNumber;

            msgTB.Add(new GrammarMsgTableItem("第" + startLine + "行单词" + startWord + "到第" + endLine + "行单词" + endWord + "之间识别为：复合语句",ConstTable.GrammarMsgType.identified));

            return 0;
        }
        #endregion
    }
}
