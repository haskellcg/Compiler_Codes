using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CompliTest
{
    /// <summary>
    /// 预处理类，主要去除注释、空行、以及字符串左右两边的空格
    /// </summary>
    public class PreDealFile
    {
        /// <summary>
        /// 存储文件的信息
        /// </summary>
        private FileInfo fileInfo;

        /// <summary>
        /// 取得输入文件的文件名，无文件的扩展名
        /// </summary>
        private string fileNameNoExt;
        public string FileNameNoExt
        {
            get { return fileNameNoExt; }
        }

        public PreDealFile(string fileName)
        {
            fileInfo = new FileInfo(fileName);

            int ExtIndex = fileName.LastIndexOf(".");
            if (ExtIndex != -1)
                fileNameNoExt = fileName.Substring(0, ExtIndex - 0);
            else
                fileNameNoExt = fileName + ".mid";
        }

        /// <summary>
        /// 根据输入的文件名，预处理文件
        /// 错误代码:
        /// 0  正确
        /// 1  文件不存在
        /// 2  临时文件已存在
        /// 3  块注释不匹配
        /// </summary>
        public int DoPreDeal()
        {
            if (fileInfo.Exists)
            {
                StreamReader fileRead = new StreamReader(fileInfo.FullName);

                FileInfo fileNoExtInfo = new FileInfo(fileNameNoExt);
                if (!fileNoExtInfo.Exists)
                {
                    StreamWriter fileNoExtWrite = fileNoExtInfo.CreateText();

                    int currentLineNumber = 0;
                    //处理文件中的内容
                    while (!fileRead.EndOfStream)
                    {
                        
                        string lineString = fileRead.ReadLine().Trim();
                        currentLineNumber++;
                        lineString = "#" + currentLineNumber + "#" + lineString;

                        //*************************************处理块注释
                        while (lineString.IndexOf("/*") != -1)
                        {
                            int ComHeadIndex = lineString.IndexOf("/*");
                            //在本行
                            if (lineString.IndexOf("*/",ComHeadIndex) != -1)
                            {
                                int ComTailIndex = lineString.IndexOf("*/");
                                lineString = lineString.Substring(0, ComHeadIndex - 0) + lineString.Substring(ComTailIndex + 2, lineString.Length - (ComTailIndex + 2));
                            }
                            //不在本行
                            else
                            {
                                lineString = lineString.Substring(0, ComHeadIndex - 0);
                                bool isFind = false;
                                while (!fileRead.EndOfStream)
                                {
                                    string tempString = fileRead.ReadLine().Trim();
                                    currentLineNumber++;
                                    if (tempString.IndexOf("*/") != -1)
                                    {
                                        int ComTailIndex = tempString.IndexOf("*/");
                                        lineString = lineString + "#" + currentLineNumber + "#" + tempString.Substring(ComTailIndex + 2, tempString.Length - (ComTailIndex + 2));
                                        isFind = true;
                                        break;
                                    }


                                }

                                if (fileRead.EndOfStream && !isFind)
                                {
                                    System.Console.WriteLine("块注释不匹配，请确认！");
                                    return 3;
                                }
                            }
                        }

                        //*************************************处理行注释
                        if (lineString.IndexOf("//") != -1)
                        {
                            int ComHeadIndex = lineString.IndexOf("//");
                            lineString = lineString.Substring(0,ComHeadIndex-0);
                        }

                        lineString=lineString.Trim();
                        if(fileRead.EndOfStream)
                            fileNoExtWrite.Write(lineString);
                        else
                            fileNoExtWrite.WriteLine(lineString);
                    }

                    fileNoExtWrite.Close();
                    fileRead.Close();
                    return 0;

                }
                else
                {
                    System.Console.WriteLine("创建临时文件终止，临时文件已存在!");
                    fileRead.Close();
                    return 2;
                }
            }
            else
            {
                System.Console.WriteLine("文件不存在，请重新输入正确的路径!");
                return 1;
            }
        }

    }
}
