using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CompliTest
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                System.Console.WriteLine("使用格式：");
                System.Console.WriteLine("              CompliTest[.exe] [参数] 源文件               ");
                System.Console.WriteLine("              参数如下：");
                System.Console.WriteLine("                  \\b             保留中间文件             ");
                return;
            }
            else if (args.Length == 1)
            {
                PreDealFile preDeal = new PreDealFile(args[0]);
                morphologyAnaly morAnaly = new morphologyAnaly(preDeal.FileNameNoExt);
                if (preDeal.DoPreDeal() == 0)
                {
                    if (morAnaly.Analize() == 0)
                    {
                        File.Delete(preDeal.FileNameNoExt);
                        morAnaly.VarTable.Add(new VariableItem(ConstTable.VariableType.Operation, "@@@", 100));
                        GrammarAnaly grammarAnaly = new GrammarAnaly(morAnaly.VarTable);
                        grammarAnaly.FunctionGram();
                        grammarAnaly.MsgTB.PrintByType(ConstTable.GrammarMsgType.error);

                        QuaternionGenerator quaternion = new QuaternionGenerator(morAnaly.VarTable);
                        quaternion.FunctionGram();
                        quaternion.QuaternionTB.PrintAll();
                        quaternion.CheckVar();


                    }

                }
            }
            else if (args.Length == 2)
            {
                if (args[0].Equals("\\b"))
                {
                    PreDealFile preDeal = new PreDealFile(args[1]);
                    morphologyAnaly morAnaly = new morphologyAnaly(preDeal.FileNameNoExt);
                    if (preDeal.DoPreDeal() == 0)
                    {
                        if (morAnaly.Analize() == 0)
                        {
                            morAnaly.VarTable.Add(new VariableItem(ConstTable.VariableType.Operation, "@@@", 100));
                            GrammarAnaly grammarAnaly = new GrammarAnaly(morAnaly.VarTable);
                            grammarAnaly.FunctionGram();
                            grammarAnaly.MsgTB.PrintByType(ConstTable.GrammarMsgType.error);

                            QuaternionGenerator quaternion = new QuaternionGenerator(morAnaly.VarTable);
                            quaternion.FunctionGram();
                            quaternion.QuaternionTB.PrintAll();
                            quaternion.CheckVar();
                        }
                    }
                }
                else
                {
                    System.Console.WriteLine("非法参数，请重新输入！");
                    return;
                }
            }
        }
    }
}
