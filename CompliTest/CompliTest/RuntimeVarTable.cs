using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompliTest
{
    /// <summary>
    /// 运行时表，监视并记录变量运行时的状态
    /// </summary>
    class RuntimeVarTable
    {
        /// <summary>
        /// 使用字典存储表的实际内容
        /// </summary>
        private Dictionary<string, RuntimeItem> dic;

        public RuntimeVarTable()
        {
            dic = new Dictionary<string, RuntimeItem>();
        }

        /// <summary>
        ///向字典中加入一个变量的状态
        /// </summary>
        /// <param name="item"></param>
        public void Add(RuntimeItem item)
        {
            if (!isExists(item.VarName))
            {
                dic.Add(item.VarName,item);
            }
        }

        /// <summary>
        /// 查询某一个关键字是否存在
        /// </summary>
        /// <param name="varName"></param>
        /// <returns></returns>
        public bool isExists(string varName)
        {
            return dic.ContainsKey(varName);
        }

        /// <summary>
        /// 根据关键字查询项目
        /// </summary>
        /// <param name="varName"></param>
        /// <returns></returns>
        public RuntimeItem GetItem(string varName)
        {
            return dic[varName];
        }

        /// <summary>
        /// 获取字典的内容长度
        /// </summary>
        /// <returns></returns>
        public int Count()
        {
            return dic.Count;
        }

        /// <summary>
        /// 打印所有的字典内容
        /// </summary>
        public void PrintAll()
        {
            System.Console.WriteLine("☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆");
            System.Console.WriteLine("☆☆☆☆☆☆☆☆☆☆☆☆☆     运行时表        ☆☆☆☆☆☆☆☆☆☆☆☆☆☆");
            foreach (RuntimeItem item in dic.Values)
            {
                System.Console.WriteLine("[ "+item.VarName+" , "+item.VarValue+" , "+item.IsDefined+" ]");
            }
            System.Console.WriteLine("☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆");
        }

    }

    /// <summary>
    /// 运行时表项
    /// </summary>
    class RuntimeItem
    {
        /// <summary>
        /// 变量的名称
        /// </summary>
        private string varName;
        public string VarName
        {
            get { return this.varName; }
            set { this.varName = value; }
        }


        /// <summary>
        /// 变量的当前值
        /// </summary>
        private int varValue;
        public int VarValue
        {
            get { return this.varValue; }
            set { this.varValue = value; }
        }

        /// <summary>
        /// 是否被初始化
        /// </summary>
        private bool isDefined;
        public bool IsDefined
        {
            get { return this.isDefined; }
            set { this.isDefined = value; }
        }


        public RuntimeItem(string varName,int varValue,bool isDefined)
        {
            this.varName = varName;
            this.varValue = varValue;
            this.isDefined = isDefined;
        }

    }
}
