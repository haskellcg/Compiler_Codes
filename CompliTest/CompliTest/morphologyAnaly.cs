using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CompliTest
{
    /// <summary>
    /// 词法分析类
    /// </summary>
    public class morphologyAnaly
    {
        /// <summary>
        /// 预处理过的文件
        /// </summary>
        private FileInfo fileInfo;

        /// <summary>
        /// 单词表
        /// </summary>
        private VariableTable varTable;
        public VariableTable VarTable
        {
            get { return this.varTable; }
        }

        public morphologyAnaly(string tempFileName)
        {
           fileInfo=new FileInfo(tempFileName);
           varTable = new VariableTable();
        }

        #region  辅助函数

        /// <summary>
        /// 判断一个字符是否为数字
        ///             '0'  ---    '9'
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        private bool isNumber(char para)
        {
            return (para >= '0' && para <= '9');
        }

        /// <summary>
        /// 判断一个字符是否为字母
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        private bool isLetter(char para)
        {
            return ((para >= 'a' && para <= 'z') || (para >= 'A' && para <= 'Z'));
        }

        /// <summary>
        /// 是否是单位操作符
        /// </summary>
        /// <returns></returns>
        private bool isSingleOperator(char para)
        {
            bool flag = false;
            for (int i = 0; i < ConstTable.SingleOperator.Length; i++)
            {
                if (ConstTable.SingleOperator[i] == para)
                {
                    flag = true;
                }
            }
            return flag;
        }

        /// <summary>
        /// 获得标识符的结束位置
        ///     -1      起始位置不合法
        ///     -2      标识符的长度超过限制
        /// </summary>
        /// <param name="sentence">要分析的句子</param>
        /// <param name="nowIndex">标识符的开始位置</param>
        /// <returns></returns>
        private int TryGetIdentifier(char []sentence,int nowIndex)
        {
            int count=1;
            if (nowIndex <= sentence.Length - 1 && nowIndex >=0)
            {
                while (++nowIndex < sentence.Length)
                {
                    char para=sentence[nowIndex];
                    if (isNumber(para) || isLetter(para) || para == '_')
                    {
                        count++;
                        if (count > ConstTable.IdentifyMaxLength)
                        {
                            return -2;
                        }
                    }
                    else
                        break;
                }

                return (--nowIndex);
            }

            return -1;

        }

        /// <summary>
        /// 判断标识符是否保留字
        /// </summary>
        /// <param name="identify"></param>
        /// <returns></returns>
        private bool isReserve(string identify)
        {
            bool flag=false;
            for (int i = 0; i < ConstTable.ReserverStrings.Length; i++)
            {
                if (identify.Equals(ConstTable.ReserverStrings[i]))
                {
                    flag = true;
                    break;
                }
            }

            return flag;
        }


        /// <summary>
        /// 获得数字的结束位置
        ///     -1      起始位置不合法
        ///     -2      数字的长度超过限制
        /// </summary>
        /// <param name="sentence"></param>
        /// <param name="nowIndex"></param>
        /// <returns></returns>
        private int TryGetNumber(char[] sentence, int nowIndex)
        {
            int count = 1;
            if (nowIndex <= sentence.Length - 1 && nowIndex >= 0)
            {
                while (++nowIndex < sentence.Length)
                {
                    char para = sentence[nowIndex];
                    if (isNumber(para))
                    {
                        count++;
                        if (count > ConstTable.NumberMaxLength)
                        {
                            return -2;
                        }
                    }
                    else
                        break;
                }

                return (--nowIndex);
            }

            return -1;

 
        }

        /// <summary>
        /// 获得>=号的结束位置
        ///     -1      不是>=号
        /// </summary>
        /// <param name="sentence"></param>
        /// <param name="nowIndex"></param>
        /// <returns>结束位置</returns>
        private int TryGetGreaterEqual(char[] sentence, int nowIndex)
        {
            if (nowIndex <= sentence.Length - 1 && nowIndex >= 0)
            {
                if (nowIndex == sentence.Length - 1)
                    return -1;
                nowIndex++;
                if (sentence[nowIndex] == '=')
                    return nowIndex;
                else
                    return -1;
            }

            return -1;
        }

        /// <summary>
        /// 获得小于等于号的结束位置
        ///     -1      不是小于等于号
        /// </summary>
        /// <param name="sentence"></param>
        /// <param name="nowIndex"></param>
        /// <returns>结束位置</returns>
        private int TryGetSmallerEqual(char[] sentence, int nowIndex)
        {
            return TryGetGreaterEqual(sentence, nowIndex);
        }

        /// <summary>
        /// 获得<>号的结束位置
        ///     -1      不是<>号
        /// </summary>
        /// <param name="sentence"></param>
        /// <param name="nowIndex"></param>
        /// <returns>结束位置</returns>
        private int TryGetNotEqual(char[] sentence, int nowIndex)
        {
            if (nowIndex <= sentence.Length - 1 && nowIndex >= 0)
            {
                if (nowIndex == sentence.Length - 1)
                    return -1;
                nowIndex++;
                if (sentence[nowIndex] == '>')
                    return nowIndex;
                else
                    return -1;
            }

            return -1;
        }

        /// <summary>
        /// 获得临时文件中行号的结束位置
        ///     -1          开始位置不合法
        ///     -2          行号的标志不匹配
        /// </summary>
        /// <param name="sentence"></param>
        /// <param name="nowIndex"></param>
        /// <returns></returns>
        private int TryGetLineNumber(char[] sentence, int nowIndex)
        {
            if (nowIndex <= sentence.Length - 1 && nowIndex >= 0)
            {
                while (++nowIndex < sentence.Length)
                {
                    if (!isNumber(sentence[nowIndex]))
                        break;
                }

                if (sentence[nowIndex] == '#')
                    return nowIndex;
                else
                    return -2;
            }

            return -1;
        }

        #endregion

        /// <summary>
        /// 根据临时文件分析，并且填表
        ///     0       执行成功
        ///     -1      临时文件不存在
        ///     -2      遇到非法字符
        ///     -3      长度超过限值
        /// </summary>
        public int Analize()
        {
            if (fileInfo.Exists)
            {
                StreamReader readTempFile = new StreamReader(this.fileInfo.FullName);
                while (!readTempFile.EndOfStream)
                {
                    string lineString = readTempFile.ReadLine();
                    char[] lineChars = lineString.ToCharArray();
                    int currentLineNumber = 1;                                  //原文本行号
                    int currentIndex = -1;                                     //当前索引位置
                    int endIndex = 0;                                           //检索到一个标识符结束位置
                    while (++currentIndex < lineChars.Length)
                    {
                        if (lineChars[currentIndex] == '#')
                        {
                            endIndex = this.TryGetLineNumber(lineChars, currentIndex);
                            if (endIndex == -2)
                            {
                                System.Console.WriteLine("非法符号#,该符号系统保留，位置第" + currentLineNumber + "行附近");
                                readTempFile.Close();
                                fileInfo.Delete();
                                return -3;
                            }
                            currentLineNumber = int.Parse(lineString.Substring(currentIndex + 1, endIndex - currentIndex - 1));
                            currentIndex = endIndex;
                        }
                        else if (isNumber(lineChars[currentIndex]))
                        {
                            endIndex = this.TryGetNumber(lineChars,currentIndex);
                            if (endIndex == -2)
                            {
                                System.Console.WriteLine("数字的长度超过限制"+ConstTable.NumberMaxLength+"，位置第" + currentLineNumber + "行");
                                readTempFile.Close();
                                fileInfo.Delete();
                                return -3;
                            }
                            VariableItem numberItem = new VariableItem(ConstTable.VariableType.Number,lineString.Substring(currentIndex,endIndex-currentIndex+1),currentLineNumber);
                            varTable.Add(numberItem);
                            currentIndex = endIndex;
                        }
                        else if (this.isLetter(lineChars[currentIndex]) || lineChars[currentIndex] == '_')
                        {
                            endIndex = this.TryGetIdentifier(lineChars, currentIndex);
                            if (endIndex == -2)
                            {
                                System.Console.WriteLine("标识符的长度超过限制" + ConstTable.IdentifyMaxLength + "，位置第" + currentLineNumber + "行");
                                readTempFile.Close();
                                fileInfo.Delete();
                                return -3;
                            }
                            if (isReserve(lineString.Substring(currentIndex, endIndex - currentIndex + 1)))
                            {
                                VariableItem reserveItem = new VariableItem(ConstTable.VariableType.Reverse, lineString.Substring(currentIndex, endIndex - currentIndex + 1), currentLineNumber);
                                varTable.Add(reserveItem);
                            }
                            else
                            {
                                VariableItem idItem = new VariableItem(ConstTable.VariableType.Identity, lineString.Substring(currentIndex, endIndex - currentIndex + 1), currentLineNumber);
                                varTable.Add(idItem);
                            }
                            currentIndex = endIndex;
                        }
                        else if (this.isSingleOperator(lineChars[currentIndex]))
                        {
                            endIndex = currentIndex;
                            VariableItem singleOpeItem = new VariableItem(ConstTable.VariableType.Operation, lineString.Substring(currentIndex, endIndex - currentIndex + 1), currentLineNumber);
                            varTable.Add(singleOpeItem);
                        }
                        else if (lineChars[currentIndex] == '>')
                        {
                            endIndex = this.TryGetGreaterEqual(lineChars,currentIndex);
                            if (endIndex == -1)
                            {
                                endIndex = currentIndex;
                                VariableItem OpeItem = new VariableItem(ConstTable.VariableType.Operation, lineString.Substring(currentIndex, endIndex - currentIndex + 1), currentLineNumber);
                                varTable.Add(OpeItem);
                            }
                            else
                            {
                                VariableItem OpeItem = new VariableItem(ConstTable.VariableType.Operation, lineString.Substring(currentIndex, endIndex - currentIndex + 1), currentLineNumber);
                                varTable.Add(OpeItem);
                                currentIndex = endIndex;
                            }
                        }
                        else if (lineChars[currentIndex] == '<')
                        {
                            endIndex = this.TryGetSmallerEqual(lineChars,currentIndex);
                            if (endIndex != -1)
                            {
                                VariableItem OpeItem = new VariableItem(ConstTable.VariableType.Operation, lineString.Substring(currentIndex, endIndex - currentIndex + 1), currentLineNumber);
                                varTable.Add(OpeItem);
                                currentIndex = endIndex;
                            }
                            else
                            {
                                endIndex = this.TryGetNotEqual(lineChars, currentIndex);
                                if (endIndex != -1)
                                {
                                    VariableItem OpeItem = new VariableItem(ConstTable.VariableType.Operation, lineString.Substring(currentIndex, endIndex - currentIndex + 1), currentLineNumber);
                                    varTable.Add(OpeItem);
                                    currentIndex = endIndex;
                                }
                                else
                                {
                                    endIndex = currentIndex;
                                    VariableItem OpeItem = new VariableItem(ConstTable.VariableType.Operation, lineString.Substring(currentIndex, endIndex - currentIndex + 1), currentLineNumber);
                                    varTable.Add(OpeItem);
                                }
                            }
                        }
                        else if (lineChars[currentIndex] == ':')
                        {
                            endIndex = currentIndex + 1;
                            if (lineChars[endIndex] == '=')
                            {
                                VariableItem OpeItem = new VariableItem(ConstTable.VariableType.Operation, lineString.Substring(currentIndex, endIndex - currentIndex + 1), currentLineNumber);
                                varTable.Add(OpeItem);
                                currentIndex = endIndex;
                            }
                            else
                            {
                                System.Console.WriteLine("赋值号不完整，位置第"+currentLineNumber+"行");
                                readTempFile.Close();
                                fileInfo.Delete();
                                return -2;
                            }

                        }
                        else if (lineChars[currentIndex] == ' ' || lineChars[currentIndex] == '\t' || lineChars[currentIndex] == '\n')
                        {

                        }
                        else
                        {
                            System.Console.WriteLine("遇到非法字符"+lineChars[currentIndex]+"，错误位置第"+currentLineNumber+"行");
                            readTempFile.Close();
                            fileInfo.Delete();
                            return -2;
                        }
                    }

                }
                readTempFile.Close();
                return 0;
            }
            else
            {
                System.Console.WriteLine("分析时临时文件不存在！");
                return -1;
            }
        }
    }
}
