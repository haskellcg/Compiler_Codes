using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompliTest
{
    /// <summary>
    /// 产生四元式，并且进行一定的分析,记录
    ///     其中包括运行时表的记录
    /// </summary>
    class QuaternionGenerator
    {
        #region 四元式产生类的基本信息
        /// <summary>
        /// 全局的Quad
        /// </summary>
        private int globalQuad;

        /// <summary>
        /// 目前的匹配位置
        /// </summary>
        private int matchIndex;

        /// <summary>
        /// 记录临时变量的索引
        /// </summary>
        private int tempVarIndex;

        /// <summary>
        /// 当四元式中的元素为空时，显示的字符串
        /// </summary>
        private const string NullQuaternionElement = "-";  

        /// <summary>
        /// 存储词法分析器分析出来的单词表
        /// </summary>
        private VariableTable varTable;

        /// <summary>
        /// 用于记录产生出来的四元式的表
        /// </summary>
        private QuaternionTable QuaternionTb;
        public QuaternionTable QuaternionTB
        {
            get { return this.QuaternionTb; }
        }

        /// <summary>
        /// 运行时表
        /// </summary>
        private RuntimeVarTable RuntimeVarTb;
        public RuntimeVarTable RuntimeVarTB
        {
            get { return RuntimeVarTb; }
        }

        public QuaternionGenerator(VariableTable varTable)
        {
            this.varTable = varTable;
            this.globalQuad = 100;
            this.matchIndex = 0;
            this.tempVarIndex = 0;
            this.QuaternionTb = new QuaternionTable();
            this.RuntimeVarTb = new RuntimeVarTable();
        }
        #endregion

        #region 辅助函数
        /// <summary>
        /// 根据四元式表中的quad，修改其Nextquad
        /// </summary>
        /// <param name="quad"></param>
        /// <param name="nextquad"></param>
        /// <returns></returns>
        private bool AlterNextquad(int quad, int nextquad)
        {
            for (int i = 0; i < this.QuaternionTb.Count; i++)
            {
                if (this.QuaternionTb.GetItem(i).SelfQuad == quad)
                {
                    this.QuaternionTb.GetItem(i).NextQuad = nextquad;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 回填函数
        /// </summary>
        /// <param name="list"></param>
        /// <param name="quad"></param>
        private void BackPatch(List<int> list, int nextquad)
        {
            foreach (int quad in list)
            {
                AlterNextquad(quad, nextquad);
            }
        }

        /// <summary>
        /// 连接两个整型列表
        /// </summary>
        /// <returns></returns>
        private List<int> Merge(List<int> list1, List<int> list2)
        {
            List<int> list = new List<int>();
            list.AddRange(list2);
            list.AddRange(list1);

            return list;
        }

        /// <summary>
        /// 判断给定的字符是否为字母
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        private bool isLetter(char ch)
        {
            return ((ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z'));
        }

        /// <summary>
        /// 判断给出的单词名字是否为变量
        /// </summary>
        /// <param name="varName"></param>
        /// <returns></returns>
        private bool isIdentity(string varName)
        {
            char[] charArray = varName.ToCharArray();
            if (isLetter(charArray[0]))
                return true;
            else
                return false;
        }
        #endregion
        
        #region 布尔表达式
        /// <summary>
        /// BoolExp
        /// </summary>
        /// <returns></returns>
        private Property_BoolE BoolExpGram()
        {
            if (this.varTable.GetItem(this.matchIndex).varValue.Equals("("))
            {
                this.matchIndex++;

                Property_BoolE BoolE = BoolExpGram();

                if (this.varTable.GetItem(this.matchIndex).varValue.Equals(")"))
                    this.matchIndex++;

                return BoolExp_Gram(BoolE);
            }
            else if (this.varTable.GetItem(this.matchIndex).varValue.Equals("not"))
            {
                this.matchIndex++;

                Property_BoolE BoolE1 = BoolExpGram();

                Property_BoolE BoolE = new Property_BoolE();
                BoolE.TrueList = BoolE1.FalseList;
                BoolE.FalseList = BoolE1.TrueList;

                return BoolE;
            }
            else
            {
                Property_BoolE BoolE = CompareEGram();
                return BoolExp_Gram(BoolE);
            }


        }

        /// <summary>
        /// BoolExp_
        /// </summary>
        /// <param name="firstBoolE"></param>
        /// <returns></returns>
        private Property_BoolE BoolExp_Gram(Property_BoolE firstBoolE)
        {
            Property_BoolE BoolE = new Property_BoolE();

            bool LogicOptResult = LogicOptGram();
            if (!LogicOptResult)
                return firstBoolE;
            string opString = this.varTable.GetItem(this.matchIndex - 1).varValue;

            Property_ME ME = new Property_ME(this.globalQuad);

            Property_BoolE secondBoolE = BoolExpGram();

            switch (opString)
            {
                case "and":
                    BackPatch(firstBoolE.TrueList, ME.Quad);
                    BoolE.TrueList = secondBoolE.TrueList;
                    BoolE.FalseList = Merge(firstBoolE.FalseList, secondBoolE.FalseList);
                    break;
                case "or":
                    BackPatch(firstBoolE.FalseList, ME.Quad);
                    BoolE.TrueList = Merge(firstBoolE.TrueList, secondBoolE.TrueList);
                    BoolE.FalseList = secondBoolE.FalseList;
                    break;
            }

            return BoolExp_Gram(BoolE);

        }

        /// <summary>
        /// 分析是否为布尔运算符
        /// </summary>
        /// <returns></returns>
        private bool LogicOptGram()
        {
            if (this.varTable.GetItem(this.matchIndex).varValue.Equals("and"))
                this.matchIndex++;
            else if (this.varTable.GetItem(this.matchIndex).varValue.Equals("or"))
                this.matchIndex++;
            else
                return false;
            return true;
        }

        /// <summary>
        /// 如果分析出比较语句，产生四元式，并且写出属性文法
        /// </summary>
        /// <returns></returns>
        private Property_BoolE CompareEGram()
        {
            bool EntityResult = EntityGram();
            string firstEntity = this.varTable.GetItem(this.matchIndex - 1).varValue;

            bool BoolOptResult = BoolOptGram();
            string opString = this.varTable.GetItem(this.matchIndex - 1).varValue;

            bool EntityResult2 = EntityGram();
            string secondEntity = this.varTable.GetItem(this.matchIndex - 1).varValue;


            Property_BoolE BoolE = new Property_BoolE();

            if (EntityResult && BoolOptResult && EntityResult2)
            {
                QuaternionItem item1 = new QuaternionItem(this.globalQuad, opString, firstEntity, secondEntity, 0, ConstTable.QuaternionType.BOOLEXP);
                this.globalQuad++;
                QuaternionItem item2 = new QuaternionItem(this.globalQuad, NullQuaternionElement, NullQuaternionElement, NullQuaternionElement, 0, ConstTable.QuaternionType.BOOLEXP);
                this.QuaternionTb.Add(item1);
                this.QuaternionTb.Add(item2);

                BoolE = new Property_BoolE();
                List<int> trueList = new List<int>();
                trueList.Add(this.globalQuad - 1);
                BoolE.TrueList = trueList;
                List<int> falseList = new List<int>();
                falseList.Add(this.globalQuad);
                BoolE.FalseList = falseList;

                this.globalQuad++;
            }


            return BoolE;
        }

        /// <summary>
        /// 分析是否为比较运算符
        /// </summary>
        /// <returns></returns>
        private bool BoolOptGram()
        {
            if (this.varTable.GetItem(this.matchIndex).varValue.Equals("=") ||
               this.varTable.GetItem(this.matchIndex).varValue.Equals(">") ||
               this.varTable.GetItem(this.matchIndex).varValue.Equals(">=") ||
               this.varTable.GetItem(this.matchIndex).varValue.Equals("<") ||
               this.varTable.GetItem(this.matchIndex).varValue.Equals("<=") ||
               this.varTable.GetItem(this.matchIndex).varValue.Equals("<>"))
                this.matchIndex++;
            else
                return false;

            return true;
        }

        /// <summary>
        /// 分析是否为Entity
        /// </summary>
        /// <returns></returns>
        private bool EntityGram()
        {
            if (this.varTable.GetItem(this.matchIndex).varType == ConstTable.VariableType.Identity)
                this.matchIndex++;
            else if (this.varTable.GetItem(this.matchIndex).varType == ConstTable.VariableType.Number)
                this.matchIndex++;
            else
                return false;


            return true;
        }
        #endregion

        #region 算术表达式
        /// <summary>
        /// Calcu
        /// </summary>
        /// <returns></returns>
        private Property_CalcuE CalcuExpGram()
        {
            if (this.varTable.GetItem(this.matchIndex).varValue.Equals("("))
            {
                this.matchIndex++;

                Property_CalcuE CalcuE = CalcuExpGram();
                
                if (this.varTable.GetItem(this.matchIndex).varValue.Equals(")"))
                    this.matchIndex++;

                if (this.varTable.GetItem(this.matchIndex).varValue.Equals("+"))
                    return Calculator_Gram(CalcuE);
                else
                    return Factor_Gram(CalcuE);
            }
            else
            {
                return CalculatorGram();

            }

        }

        /// <summary>
        /// Calculator
        /// </summary>
        /// <returns></returns>
        private Property_CalcuE CalculatorGram()
        {  
            Property_CalcuE CalcuE = FactorGram();
            return Calculator_Gram(CalcuE);
        }

       /// <summary>
       /// Calculator_
       /// </summary>
       /// <param name="firstCalcuE"></param>
       /// <returns></returns>
        private Property_CalcuE Calculator_Gram(Property_CalcuE firstCalcuE)
        {


            if (this.varTable.GetItem(this.matchIndex).varValue.Equals("+"))
                this.matchIndex++;
            else
                return firstCalcuE;

            string opString = this.varTable.GetItem(this.matchIndex - 1).varValue;

            Property_CalcuE secondCalcuE = CalcuExpGram();

            Property_CalcuE CalcuE = new Property_CalcuE("@T" + this.tempVarIndex++);

            QuaternionItem item1 = new QuaternionItem(this.globalQuad,opString,firstCalcuE.VarValue,secondCalcuE.VarValue,CalcuE.VarValue,ConstTable.QuaternionType.CALCUEXP);
            this.QuaternionTb.Add(item1);
            this.globalQuad++;


            return Calculator_Gram(CalcuE);
        }

        private Property_CalcuE FactorGram()
        {
            bool EntityResult = EntityGram();
            string IDString = this.varTable.GetItem(this.matchIndex - 1).varValue;


            Property_CalcuE CalcuE = new Property_CalcuE(IDString);
            return Factor_Gram(CalcuE);
        }

        private Property_CalcuE Factor_Gram(Property_CalcuE firstCalcuE)
        {
            if (this.varTable.GetItem(this.matchIndex).varValue.Equals("*"))
                this.matchIndex++;
            else
                return firstCalcuE;

            string opString = this.varTable.GetItem(this.matchIndex - 1).varValue;

            Property_CalcuE secondCalcuE;
            if(this.varTable.GetItem(this.matchIndex).varValue.Equals("("))
            {
                this.matchIndex++;
                secondCalcuE = CalcuExpGram();
                this.matchIndex++;
            }
            else
                secondCalcuE = FactorGram();

            Property_CalcuE CalcuE = new Property_CalcuE("@T" + this.tempVarIndex++);

            QuaternionItem item1 = new QuaternionItem(this.globalQuad, opString, firstCalcuE.VarValue, secondCalcuE.VarValue, CalcuE.VarValue, ConstTable.QuaternionType.CALCUEXP);
            this.QuaternionTb.Add(item1);
            this.globalQuad++;


            return Factor_Gram(CalcuE);
        }
        #endregion

        #region 赋值语句

        private Property_SE FillSentenceGram()
        {
            if (this.varTable.GetItem(this.matchIndex).varType == ConstTable.VariableType.Identity)
                this.matchIndex++;
            string firstEntity = this.varTable.GetItem(this.matchIndex-1).varValue;


            if (this.varTable.GetItem(this.matchIndex).varValue.Equals(":="))
                this.matchIndex++;
            string opString = this.varTable.GetItem(this.matchIndex - 1).varValue;

            Property_CalcuE CalcuE = CalcuExpGram();

            if (this.varTable.GetItem(this.matchIndex).varValue.Equals(";"))
                this.matchIndex++;

            Property_SE SE = new Property_SE();

            QuaternionItem item1 = new QuaternionItem(this.globalQuad,opString,CalcuE.VarValue,NullQuaternionElement,firstEntity,ConstTable.QuaternionType.FILLEXP);
            this.QuaternionTb.Add(item1);
            this.globalQuad++;
           
            return SE;
        }
        #endregion

        #region 分支语句
        
        /// <summary>
        /// FZSentence
        /// </summary>
        /// <returns></returns>
        private Property_SE FZSentenceGram()
        {
            if (this.varTable.GetItem(this.matchIndex).varValue.Equals("if"))
                this.matchIndex++;

            Property_BoolE BoolE = BoolExpGram();

            if (this.varTable.GetItem(this.matchIndex).varValue.Equals("then"))
                this.matchIndex++;

            Property_ME ME1 = new Property_ME(this.globalQuad);


            Property_SE SE1 = new Property_SE();
            if (this.varTable.GetItem(this.matchIndex).varType == ConstTable.VariableType.Identity)
            {
                SE1 = FillSentenceGram();
            }
            else if (this.varTable.GetItem(this.matchIndex).varValue.Equals("if"))
            {
                SE1 = FZSentenceGram();
            }
            else if (this.varTable.GetItem(this.matchIndex).varValue.Equals("while"))
            {
                SE1 = XHSentenceGram();
            }
            else if (this.varTable.GetItem(this.matchIndex).varValue.Equals("begin"))
            {
                SE1 = ComplexSentenceGram();
            }

            Property_SE SE = new Property_SE();

            if (this.varTable.GetItem(this.matchIndex).varValue.Equals("else"))
            {
                Property_NE NE = new Property_NE();
                List<int> nextList = new List<int>();
                nextList.Add(this.globalQuad);
                NE.NextList = nextList;

                QuaternionItem item1 = new QuaternionItem(this.globalQuad, NullQuaternionElement, NullQuaternionElement, NullQuaternionElement, 0, ConstTable.QuaternionType.NEXP);
                this.QuaternionTb.Add(item1);
                this.globalQuad++;

                if (this.varTable.GetItem(this.matchIndex).varValue.Equals("else"))
                    this.matchIndex++;

                Property_ME ME2 = new Property_ME(this.globalQuad);

                Property_SE SE2=new Property_SE();
                if (this.varTable.GetItem(this.matchIndex).varType == ConstTable.VariableType.Identity)
                {
                     SE2= FillSentenceGram();
                }
                else if (this.varTable.GetItem(this.matchIndex).varValue.Equals("if"))
                {
                    SE2 = FZSentenceGram();
                }
                else if (this.varTable.GetItem(this.matchIndex).varValue.Equals("while"))
                {
                    SE2 = XHSentenceGram();
                }
                else if (this.varTable.GetItem(this.matchIndex).varValue.Equals("begin"))
                {
                    SE2 = ComplexSentenceGram();
                }
  
                BackPatch(BoolE.TrueList,ME1.Quad);
                BackPatch(BoolE.FalseList,ME2.Quad);
                SE.NextList = Merge(Merge(SE1.NextList, NE.NextList), SE2.NextList);
            }
            else
            {
                BackPatch(BoolE.TrueList,ME1.Quad);
                SE.NextList = Merge(BoolE.FalseList,SE1.NextList);
            }

            return SE;
        }
        #endregion

        #region 循环语句
        /// <summary>
        /// XHSentence
        /// </summary>
        /// <returns></returns>
        private Property_SE XHSentenceGram()
        {
            if (this.varTable.GetItem(this.matchIndex).varValue.Equals("while"))
                this.matchIndex++;

            Property_ME ME1 = new Property_ME(this.globalQuad);

            Property_BoolE BoolE = BoolExpGram();
            
            if (this.varTable.GetItem(this.matchIndex).varValue.Equals("do"))
                this.matchIndex++;

            Property_ME ME2 = new Property_ME(this.globalQuad);


            Property_SE SE1=new Property_SE();
            if (this.varTable.GetItem(this.matchIndex).varType == ConstTable.VariableType.Identity)
            {
                SE1 = FillSentenceGram();
            }
            else if (this.varTable.GetItem(this.matchIndex).varValue.Equals("if"))
            {
                SE1 = FZSentenceGram();
            }
            else if (this.varTable.GetItem(this.matchIndex).varValue.Equals("while"))
            {
                SE1 = XHSentenceGram();
            }
            else if (this.varTable.GetItem(this.matchIndex).varValue.Equals("begin"))
            {
                SE1 = ComplexSentenceGram();
            }

            Property_SE SE = new Property_SE();
            BackPatch(SE1.NextList,ME1.Quad);
            BackPatch(BoolE.TrueList, ME2.Quad);
            SE.NextList = BoolE.FalseList;

            QuaternionItem item1 = new QuaternionItem(this.globalQuad,NullQuaternionElement,NullQuaternionElement,NullQuaternionElement,ME1.Quad,ConstTable.QuaternionType.WHILEEXP);
            this.QuaternionTb.Add(item1);
            this.globalQuad++;

            return SE;
        }
        #endregion

        #region 复杂语句
        private Property_SE ComplexSentenceGram()
        {
            if (this.varTable.GetItem(this.matchIndex).varValue.Equals("begin"))
                this.matchIndex++;


            Property_SE SE = SentenceGram(new Property_SE());

            if (this.varTable.GetItem(this.matchIndex).varValue.Equals("end"))
                this.matchIndex++;

            return SE;
        }
        #endregion

        #region 语句
       /// <summary>
        /// Sentence
       /// </summary>
       /// <returns></returns>
        private Property_SE SentenceGram(Property_SE firstSE)
        {
            Property_SE SE = new Property_SE();

            Property_SE secondSE = new Property_SE();
            if (this.varTable.GetItem(this.matchIndex).varType == ConstTable.VariableType.Identity)
            {
                Property_ME ME = new Property_ME(this.globalQuad);
                BackPatch(firstSE.NextList, ME.Quad);
                secondSE = FillSentenceGram();
                SE.NextList = secondSE.NextList;
                return SentenceGram(SE);
            }
            else if (this.varTable.GetItem(this.matchIndex).varValue.Equals("if"))
            {
                Property_ME ME = new Property_ME(this.globalQuad);
                BackPatch(firstSE.NextList, ME.Quad);
                secondSE = FZSentenceGram();
                SE.NextList = secondSE.NextList;
                return SentenceGram(SE);
            }
            else if (this.varTable.GetItem(this.matchIndex).varValue.Equals("while"))
            {
                Property_ME ME = new Property_ME(this.globalQuad);
                BackPatch(firstSE.NextList, ME.Quad);
                secondSE = XHSentenceGram();
                SE.NextList = secondSE.NextList;
                return SentenceGram(SE);
            }
            else if (this.varTable.GetItem(this.matchIndex).varValue.Equals("begin"))
            {
                Property_ME ME = new Property_ME(this.globalQuad);
                BackPatch(firstSE.NextList, ME.Quad);
                secondSE = ComplexSentenceGram();
                SE.NextList = secondSE.NextList;
                return SentenceGram(SE);
            }
            return firstSE;
        }
        #endregion

        #region 表达式
       /// <summary>
        /// Expression
       /// </summary>
       /// <returns></returns>
        private Property_SE ExpressionGram()
        {
            return SentenceGram(new Property_SE());
        }
        #endregion

        #region 声明
       /// <summary>
       /// Claim
       /// </summary>
        private void ClaimGram()
        {
            if (this.varTable.GetItem(this.matchIndex).varValue.Equals("var"))
                this.matchIndex++;
            else
                return;

            if (this.varTable.GetItem(this.matchIndex).varValue.Equals("int"))
                this.matchIndex++;

             IdentityGroupGram();

            if (this.varTable.GetItem(this.matchIndex).varValue.Equals(";"))
                this.matchIndex++;
        }

        /// <summary>
        /// IdentityGroup
        /// </summary>
        private void IdentityGroupGram()
        {
            if (this.varTable.GetItem(this.matchIndex).varType == ConstTable.VariableType.Identity)
                this.matchIndex++;
            RuntimeItem item1 = new RuntimeItem(this.varTable.GetItem(this.matchIndex-1).varValue,0,false);
            this.RuntimeVarTb.Add(item1);
            IdentityGroup_Gram();
        }

        /// <summary>
        /// IdentityGroup_
        /// </summary>
        private void IdentityGroup_Gram()
        {
            if (this.varTable.GetItem(this.matchIndex).varValue.Equals(","))
                this.matchIndex++;
            else
                return;
            IdentityGroupGram();
        }
        #endregion

        #region 函数

        /// <summary>
        /// Function
        /// </summary>
        public void FunctionGram()
        {

            if (this.varTable.GetItem(this.matchIndex).varValue.Equals("program"))
                this.matchIndex++;
            else
                return;

            if (this.varTable.GetItem(this.matchIndex).varType == ConstTable.VariableType.Identity)
                this.matchIndex++;

            if (this.varTable.GetItem(this.matchIndex).varValue.Equals(";"))
                this.matchIndex++;

            ClaimGram();
       
            if (this.varTable.GetItem(this.matchIndex).varValue.Equals("begin"))
                this.matchIndex++;

            ExpressionGram();

            if (this.varTable.GetItem(this.matchIndex).varValue.Equals("end"))
                this.matchIndex++;       

            if (this.varTable.GetItem(this.matchIndex).varValue.Equals("."))
                this.matchIndex++;
        }
        #endregion

        #region 声明变量检测
        /// <summary>
        /// 根据分析出的运行时表以及四元式，检测变量的使用错误
        /// </summary>
        public void CheckVar()
        {
            for (int i = 0; i < this.QuaternionTb.Count;i++)
            {
                QuaternionItem item = QuaternionTb.GetItem(i);
                string firstEntity = item.FirstEnity;
                if (isIdentity(firstEntity))
                {
                    if (!RuntimeVarTb.isExists(firstEntity))
                        System.Console.WriteLine("使用未声明的变量<{0}>", new object[] { firstEntity });
                }
                string secondEntity = item.SecondEntity;
                if (isIdentity(secondEntity))
                {
                    if (!RuntimeVarTb.isExists(secondEntity))
                        System.Console.WriteLine("使用未声明的变量<{0}>" , new object[] { secondEntity });
                }
                if (item.OpString.Equals(":="))
                {
                    string resultString = item.NextQuad.ToString();
                    if (!RuntimeVarTb.isExists(resultString))
                        System.Console.WriteLine("使用未声明的变量<{0}>" , new object[] { secondEntity });
                }
            }
        }
        #endregion
    }
}
