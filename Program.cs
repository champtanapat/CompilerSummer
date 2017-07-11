using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CompilerSu59
{
    class Program
    {
        
        static void Main(string[] args)
        {
           
            //System.IO.StreamReader r = new System.IO.StreamReader("c:/users/cs-lab/documents/visual studio 2015/Projects/CompilerSu59/CompilerSu59/Test.txt");
            System.IO.StreamReader r = new System.IO.StreamReader("C:/Users/CHAMPHAHA/Desktop/CompilerSu59/Test.txt");
            string sc = r.ReadToEnd();
            Lexical lx = new Lexical(sc+"#");
            lx.WordLex();
            lx.printlex();
            
            Paser pas = new Paser(lx.list);
            pas.S();
            pas.printTable();
            pas.printTuple();
            pas.printInter();

            
            Codegen codege = new Codegen(pas.inter);
            codege.printCodegen();
            codege.printLabe();
        }
    }
}
