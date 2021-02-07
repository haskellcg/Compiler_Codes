using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompliTest
{
    /// <summary>
    /// 四元式表
    /// </summary>
    class QuaternionTable
    {
         /// <summary>
        /// 存储表的实体
        /// </summary>
        private List<QuaternionItem> table;

        public QuaternionTable()
        {
            table = new List<QuaternionItem>();
        }

        /// <summary>
        /// 向表中插入一条
        /// </summary>
        /// <param name="item"></param>
        public void Add(QuaternionItem item)
        {
            table.Add(item);
        }

        /// <summary>
        /// 向表中插入多条记录
        /// </summary>
        /// <param name="list"></param>
        public void AddRange(List<QuaternionItem> list)
        {
            table.AddRange(list);
        }

        /// <summary>
        /// 根据索引获得表项
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public QuaternionItem GetItem(int index)
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

        public void PrintAll()
        {
            System.Console.WriteLine("----------------------------------------------------------------------");
            System.Console.WriteLine("{0,-3}   {1,-3} , {2,-15} , {3,-15} , {4,-15} ", new object[]{
                                                                                        "序号",
                                                                                        "运算符",
                                                                                        "运算数①",
                                                                                        "运算数②",
                                                                                        "结果"
                                                                                            });
            foreach (QuaternionItem item in table)
            {
                System.Console.WriteLine("{0,-3}  ( {1,-3} , {2,-15} , {3,-15} , {4,-15} )",new object[]{
                                                                                        item.SelfQuad,
                                                                                        item.OpString,
                                                                                        item.FirstEnity,
                                                                                        item.SecondEntity,
                                                                                        item.NextQuad
                                                                                            });
            }
            System.Console.WriteLine("----------------------------------------------------------------------");
        }
        
    }


    /// <summary>
    /// 四元式
    /// </summary>
    class QuaternionItem
    {
        /// <summary>
        /// 序列号
        /// </summary>
        private int selfQuad;
        public int SelfQuad
        {
            get{return this.selfQuad;}
            set{this.selfQuad=value;}
        }

        /// <summary>
        /// 操作符
        /// </summary>
        private string opString;
        public string OpString
        {
            get { return this.opString; }
            set { this.opString = value; }
        }

        /// <summary>
        /// 第一个操作数
        /// </summary>
        private string firstEntity;
        public string FirstEnity
        {
            get { return this.firstEntity; }
            set { this.firstEntity = value; }
        }

        /// <summary>
        /// 第二个操作数
        /// </summary>
        private string secondEntity;
        public string SecondEntity
        {
            get { return this.secondEntity; }
            set { this.secondEntity = value; }
        }

        /// <summary>
        /// 四元式的结果
        /// </summary>
        private object nextQuad;
        public object NextQuad
        {
            get { return this.nextQuad; }
            set { this.nextQuad = value; }
        }

        /// <summary>
        /// 四元式类型
        /// </summary>
        private ConstTable.QuaternionType quaternionType;
        public ConstTable.QuaternionType QuaternionType
        {
            get { return this.quaternionType; }
            set { this.quaternionType = value; }
        }

        public QuaternionItem(int selfQuad,string opString,string firstEntity,string secondEntity,object nextQuad,ConstTable.QuaternionType quaternionType)
        {
            this.selfQuad=selfQuad;
            this.opString = opString;
            this.firstEntity = firstEntity;
            this.secondEntity = secondEntity;
            this.nextQuad = nextQuad;
            this.quaternionType = quaternionType;
        }
    }
}
