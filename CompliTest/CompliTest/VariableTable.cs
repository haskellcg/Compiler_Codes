using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompliTest
{
    /// <summary>
    /// 变量的记录表
    /// </summary>
    public class VariableTable
    {
        /// <summary>
        /// 存储表的实体
        /// </summary>
        private List<VariableItem> table;

        public VariableTable()
        {
            table = new List<VariableItem>();
        }

        /// <summary>
        /// 向表中插入一条
        /// </summary>
        /// <param name="item"></param>
        public void Add(VariableItem item)
        {
            table.Add(item);
        }

        /// <summary>
        /// 向表中插入多条记录
        /// </summary>
        /// <param name="list"></param>
        public void AddRange(List<VariableItem> list)
        {
            table.AddRange(list);
        }

        /// <summary>
        /// 根据索引获得表项
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public VariableItem GetItem(int index)
        {
            return table[index];
        }

        /// <summary>
        /// 表中的数目
        /// </summary>
        public int Count
        {
            get { return table.Count; }
        }

        /// <summary>
        /// 清除表中的所有元素
        /// </summary>
        public void Clear()
        {
            table.Clear();
        }

        /// <summary>
        /// 打印表项，根据类型
        /// </summary>
        /// <param name="type"></param>
        public void PrintByType(ConstTable.VariableType type)
        {
            string typeName = "";
            switch (type)
            {
                case ConstTable.VariableType.Identity:
                    typeName = "Identity";
                    break;
                case ConstTable.VariableType.Number:
                    typeName = "Number";
                    break;
                case ConstTable.VariableType.Operation:
                    typeName = "Operation";
                    break;
                case ConstTable.VariableType.Reverse:
                    typeName = "Reverse";
                    break;
            }
            System.Console.WriteLine("************************************************");
            foreach (VariableItem item in table)
            {
                if (item.varType == type)
                {
                    System.Console.WriteLine("(\t" + item.varValue + "\t\t," + typeName + "\t\t," + item.lineNumber + ")");
                }
            }
            System.Console.WriteLine("************************************************");
        }

        /// <summary>
        /// 按顺序打印表项
        /// </summary>
        public void PrintAll()
        {
            System.Console.WriteLine("************************************************");
            foreach (VariableItem item in table)
            {
                string typeName = "";
                switch (item.varType)
                {
                    case ConstTable.VariableType.Identity:
                        typeName = "Identity";
                        break;
                    case ConstTable.VariableType.Number:
                        typeName = "Number";
                        break;
                    case ConstTable.VariableType.Operation:
                        typeName = "Operation";
                        break;
                    case ConstTable.VariableType.Reverse:
                        typeName = "Reverse";
                        break;
                }


                System.Console.WriteLine("(\t" + item.varValue + "\t\t," + typeName + "\t\t," + item.lineNumber + ")");

            }
            System.Console.WriteLine("************************************************");
        }

        /// <summary>
        /// 根据在表中的位置确定在行中的位置
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public int GetLineIndex(int index)
        {
            if(index >= table.Count)
                return -1;
            int count = 1;
            for (int i = 0; i < index; i++)
                if (table[i].lineNumber == table[index].lineNumber)
                    count++;

            return count;
        }
    }

    /// <summary>
    /// 表的单个表项
    /// </summary>
    public class VariableItem
    {
        /// <summary>
        /// 变量的值
        /// </summary>
        private string _varValue;
        public string varValue
        {
            get { return _varValue; }
            set { _varValue = value; }
        }

        /// <summary>
        /// 变量的类型
        /// </summary>
        private ConstTable.VariableType _varType;
        public ConstTable.VariableType varType
        {
            get { return _varType; }
            set { _varType = value; }
        }

        /// <summary>
        /// 行号，便于报错
        /// </summary>
        private int _lineNumber;
        public int lineNumber
        {
            get { return this._lineNumber;}
            set { this._lineNumber = value; }
        }

        public VariableItem(ConstTable.VariableType _varType,string _varValue,int _lineNumber)
        {
            this._varValue = _varValue;
            this._varType = _varType;
            this._lineNumber = _lineNumber;
        }

        
    }
}
