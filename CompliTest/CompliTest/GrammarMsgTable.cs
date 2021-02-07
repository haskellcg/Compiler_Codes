using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompliTest
{
    /// <summary>
    /// 语法分析输出信息记录表
    /// </summary>
    class GrammarMsgTable
    {
        /// <summary>
        /// 存储表的实体
        /// </summary>
        private List<GrammarMsgTableItem> msgList;


        public GrammarMsgTable()
        {
            msgList = new List<GrammarMsgTableItem>();
        }

        /// <summary>
        /// 向表中插入一条
        /// </summary>
        /// <param name="item"></param>
        public void Add(GrammarMsgTableItem item)
        {
            msgList.Add(item);
        }

        /// <summary>
        /// 表中的数目
        /// </summary>
        public int Count()
        {
            return msgList.Count();
        }

        /// <summary>
        /// 清除表中的所有元素
        /// </summary>
        public void Clear()
        {
            msgList.Clear();
        }

        /// <summary>
        /// 打印表项，根据类型
        /// </summary>
        /// <param name="type"></param>
        public void PrintByType(ConstTable.GrammarMsgType type)
        {
            if (type == ConstTable.GrammarMsgType.error)
            {
                System.Console.WriteLine("************************************************");
                System.Console.WriteLine("输出的错误信息:");
                foreach (GrammarMsgTableItem item in msgList)
                {
                    if (item.MsgType == ConstTable.GrammarMsgType.error)
                        System.Console.WriteLine(item.MsgString);
                }
                System.Console.WriteLine("************************************************");
            }else if(type == ConstTable.GrammarMsgType.identified)
            {
                System.Console.WriteLine("************************************************");
                System.Console.WriteLine("已经识别出的信息:");
                foreach (GrammarMsgTableItem item in msgList)
                {
                    if (item.MsgType == ConstTable.GrammarMsgType.identified)
                        System.Console.WriteLine(item.MsgString);
                }
                System.Console.WriteLine("************************************************");

            }
        }

        /// <summary>
        /// 按顺序打印表项
        /// </summary>
        public void PrintAll()
        {
            System.Console.WriteLine("************************************************");
            System.Console.WriteLine("按顺序输出信息:");
            foreach (GrammarMsgTableItem item in msgList)
            {
                    System.Console.WriteLine(item.MsgString);
            }
            System.Console.WriteLine("************************************************");
        }
    }

    /// <summary>
    /// 语法分析输出信息记录表的表项
    /// </summary>
    class GrammarMsgTableItem
    {
        /// <summary>
        /// 语法分析输出的字符串信息
        /// </summary>
        private string msgString;
        public string MsgString
        {
            get { return msgString; }
            set { msgString = value; }
        }

        /// <summary>
        ///语法分析输出的类型
        /// </summary>
        private ConstTable.GrammarMsgType msgType;
        public ConstTable.GrammarMsgType MsgType
        {
            get { return msgType; }
            set { msgType = value; }
        }

        public GrammarMsgTableItem(string msgString,ConstTable.GrammarMsgType msgType)
        {
            this.msgString = msgString;
            this.msgType = msgType;
        }
    }
}
