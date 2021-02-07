using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompliTest
{
    /// <summary>
    /// 布尔表达式的属性文法
    /// </summary>
    class Property_BoolE
    {
        /// <summary>
        /// 布尔表达式的“真”列表
        /// </summary>
        private List<int> trueList;
        public List<int> TrueList
        {
            get { return this.trueList; }
            set { this.trueList = value; }
        }

        /// <summary>
        /// 布尔表达式的“假”列表
        /// </summary>
        private List<int> falseList;
        public List<int> FalseList
        {
            get { return this.falseList; }
            set { this.falseList = value; }
        }


        public Property_BoolE()
        {
            this.trueList = new List<int>();
            this.falseList = new List<int>();
        }
    }

    /// <summary>
    /// M   的属性文法
    /// </summary>
    class Property_ME
    {
        /// <summary>
        /// 唯一属性quad
        /// </summary>
        private int quad;
        public int Quad
        {
            get { return this.quad; }
            set { this.quad = value; }
        }

        public Property_ME(int quad)
        {
            this.quad = quad;
        }

    }

    /// <summary>
    /// 算术表达式的属性文法
    /// </summary>
    class Property_CalcuE
    {
        /// <summary>
        /// 唯一属性  值
        /// </summary>
        private string varValue;
        public string VarValue
        {
            get { return this.varValue; }
            set { varValue = value; }
        }

        public Property_CalcuE(string varValue)
        {
            this.varValue = varValue;
        }
    }

    /// <summary>
    /// 语句的属性文法
    /// </summary>
    class Property_SE
	{
        /// <summary>
        /// 唯一属性    转移指令列表
        /// </summary>
        private List<int> nextList;
        public List<int> NextList
        {
            get { return this.nextList; }
            set { this.nextList = value; }
        }

        public Property_SE()
        {
            this.nextList = new List<int>();
        }
	}

    /// <summary>
    /// N-->e的属性文法
    /// </summary>
    class Property_NE
    {
        /// <summary>
        /// 唯一属性    转移指令列表
        /// </summary>
        private List<int> nextList;
        public List<int> NextList
        {
            get { return this.nextList; }
            set { this.nextList = value; }
        }

        public Property_NE()
        {
            this.nextList = new List<int>();
        }
    }
}
