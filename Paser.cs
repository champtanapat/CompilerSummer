using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace CompilerSu59
{
    class Paser
    {
        private int index = 0;
        private List<Token> list = new List<Token>();
        private Token current;
        string label = "";
        private int flageOpera = 0; 

        private List<Var> gobal = new List<Var>();
        private List<Var> local = new List<Var>();
        private List<Function> pro = new List<Function>();
        private List<Function> func = new List<Function>();
        private List<Function> call = new List<Function>();

        private Stack mystack = new Stack();
        private ArrayList array_Infix = new ArrayList();
        private ArrayList array_postfix = new ArrayList();
        private ArrayList array_Tuple = new ArrayList();
        private int syntax = 0 ;
        private int addrs = 1;
        private string sl;

        private ArrayList arraySW = new ArrayList();
        public List<Inter> inter = new List<Inter>();
        
        private int countTemp = 0;
        private int countLable = 1;
        private int countAgu = 0;
        private int flagecall = 0;
        private int flagesw = 0;
        private Stack stackSW = new Stack();
        private string nameFP = "";

        

        private Stack stacklabel = new Stack();

        public Paser()
        {
            
        }

        public Paser(List<Token> list)
        {
            this.list = list;
        }

        public Token getToken()
        {
           return list[index++];
        }

        private string Temp()
        {
            return "T"+countTemp++;
        }

        private string Label()
        {
            return "L" + countLable++;
        }

      
        public void S()
        {

            current = getToken();
            if(current.word == "int" || current.word == "float" || current.word == "string" )
            {
                string id = "";
                string para = ""; 
                addrs = 1;
                T(ref id, ref para,"");
                P();
                F();
            }
            else if(current.word == "#")
            {
               
            }
            else
            {
                printSystaxError("S()", current.word);
            }
            if (current.word == "#" && syntax==0)
            {
                Console.WriteLine("Syntax True");
            }
        }
        
        private void P()
        {
            if (current.type == "ID")
            {
                
                P1();
                P();
                if(current.word==";")
                {
                    current = getToken();
                }
            }
            else if (current.word == "Func" ||current.word == "#")
            {

            }
            else
            {
                printSystaxError("P()",current.word);
            }
        }

        private void P1()
        {
            if (current.type == "ID")
            {
                string id_temp = "";
                string para = "";
                string name = "";
                string nameScope = "";
                name = current.word;
                nameFP = "P";
                current = getToken();
                if (current.word == "(")
                {
                    current = getToken();
                    addrs = 0;
                    sl = "Local0";
                    T(ref id_temp, ref para, name);
                    if(findDupilcate(name, "Gobal", ref nameScope) == true)
                    {
                        SemanticError("Prototype Dupicate: ", name, "", "");
                        Environment.Exit(0);
                    }
                    /*if(findDupilcate(name, "P",ref nameScope) != true && findDupilcate(name, "Gobal",ref nameScope) != true)
                    {
                        pro.Add(new Function(name,para));
                    }
                    else
                    {
                        SemanticError("Prototype Dupicate: ", name, "", "");
                        Environment.Exit(0);
                    }
                    */
                    // prototype ซ้ำได้ แต่paraห้ามซ้ำ
                    // เช็คpara 
                    foreach (var f in pro)
                    {
                        if (name == f.getName() && para== f.getPara() )
                        {
                            SemanticError("Prototype Dupicate para: ", name, "", "");
                            Environment.Exit(0);
                        }
                    }
                    pro.Add(new Function(name, para));
                    /*if ()
                    {
                        pro.Add(new Function(name, para));
                    }

                    else
                    {
                        SemanticError("Prototype Dupicate: ", name, "", "");
                        Environment.Exit(0);
                    }
                    */
                    removeVar(sl);
                    if (current.word == ")")
                    {
                        current = getToken();
                        if(current.word == ";")
                        {
                            
                            nameFP = "";
                            current = getToken();
                            
                        }
                        else
                        {
                            printSystaxError("P1()", current.word);
                        }
                    }
                    else
                    {
                        printSystaxError("P1()", current.word);
                    }

                }
                else
                {
                    printSystaxError("P1()", current.word);
                }

            }
            else
            {
                printSystaxError("P1()", current.word);
            }
        }

        private void F()
        {
            string id_temp = "";
            string type_temp = "";
            string name = "";
            string para = "";
            string nameFunction = "";
            string nameScope = "";
            nameFP = "F";
            if (current.word == "Func")
            {
               
                current = getToken();
                name = current.word;
                sl = "Local1";
                if (current.type=="ID")
                {
                    /*
                    if (findDupilcate(name, "F",ref nameScope) == true)
                    {
                        Console.WriteLine("===========Function Duplication===========");
                        Console.WriteLine("Func " + name + "() ");
                        Environment.Exit(0);
                       
                    }*/
                    nameFunction = name;
                    current = getToken();
                    if (current.word=="(")
                    {
                        current = getToken();
                        T3(ref para);

                        //Check name Function Declare 
                        /*if(checkFunction(name,para,"f", nameFunction))
                        {
                            func.Add(new Function(name,para));
                        }
                        else
                        {
                            SemanticError("Function Not Declare: ", name, "", "");
                            Environment.Exit(0);
                        }
                        */
                        bool nameDec = false;
                        foreach(var i in pro)
                        {
                            if(name == i.getName())
                            {
                                nameDec = true;
                            }
                        }
                        if(nameDec==false)
                        {
                            SemanticError("Function Not Declare: ", name, "", "");
                            Environment.Exit(0);
                        }
                        if (!checkFunction(name, para, "f", nameFunction))
                        {
                            Console.WriteLine("===========Function Paramiter not equal:===========");
                            Console.WriteLine("Func: " + name + " (" + para + ") " + "");
                            Environment.Exit(0);
                        }
                        else
                        {
                            // check Dup Function 
                            foreach (var f in func)
                            {
                                if (name == f.getName() && para == f.getPara())
                                {
                                    Console.WriteLine("===========Function Duplication===========");
                                    Console.WriteLine("Func " + name + "() ");
                                    Environment.Exit(0);
                                }
                            }
                           
                            func.Add(new Function(name, para));
                        }
                        if (current.word==")")
                        {
                            current = getToken();
                            if (current.word == "{")
                            {
                                current = getToken();
                                T(ref id_temp,ref type_temp,nameFunction);
                                S1(nameFunction);
                                if (current.word == "}")
                                {
                                    nameFP = "";
                                    removeVar(sl);
                                    //removeFunc(nameFunction);
                                    current = getToken();
                                    F();
                                }
                                else
                                {
                                    printSystaxError("F()", current.word);
                                }
                            }
                            else
                            {
                                printSystaxError("F()", current.word);
                            }
                        }
                        else
                        {
                            printSystaxError("F()", current.word);
                        }

                    }
                    else
                    {
                        printSystaxError("F()", current.word);
                    }
                }
                else
                {
                    printSystaxError("F()", current.word);
                }
            }
            else if (current.word == "#")
            {

            }
            else
            {
                printSystaxError("F()", current.word);
            }
        }

        private void S1(string nameFunction)
        {
          
           if(current.type=="ID")
            {
                flagesw = 0; 
                A(nameFunction);
                S1(nameFunction);
            }
           else if(current.word=="switch")
            {
                flagesw = 1;
                W(nameFunction);
                S1(nameFunction);
            }
           else if(current.word=="call")
            {
                flagesw = 0;
                C(nameFunction);
                S1(nameFunction);
            }
           else if(current.word=="}" || current.word == "#" || current.word =="case" || current.word == "default")
            {

            }
           else
            {
                printSystaxError("S1()",current.word);
            }


        }

        private void C(string nameFunction)
        {
            string para ="";
            string name = "";
            string type = "";
            string nameScope = "";
            string nameCall = "";
            countAgu = 0;
            if (current.word=="call")
            {
                current = getToken();
                if(current.type=="ID")
                {
                    name = current.word;
                    nameCall = name;
                    flagecall = 1;
                    /*
                    if (chckeCallDup(name))
                    {
                        Console.WriteLine("===========Call Dupilcate  : in Function===========");
                        Console.WriteLine("Func " + nameFunction + "() " + "call " + name + "()");
                        Environment.Exit(0);
                    }
                    */
                    /*if(name==nameFunction)
                    {
                        Console.WriteLine("===========Call Dupilcate  : in Function===========");
                        Console.WriteLine("Func " + nameFunction + "() " + "call " + name + "()");
                        Environment.Exit(0);
                    }
                    */

                    // check Delare in Prototype ? 
                    /*if (findDupilcate(name, "P",ref nameScope) == false)
                    {

                        Console.WriteLine("===========Call not Delare  : in Function===========");
                        Console.WriteLine("Func " + nameFunction + "() " + "call "+name+"()");
                        Environment.Exit(0);
                    }
                    */
                    current = getToken();
                    if (current.word == "(")
                    {
                        current = getToken();
                        E(ref para, current.word,nameFunction,ref type);

                        /*if (checkFunction(name, para,"c",nameFunction))
                        {
                            call.Add(new Function(name, para));
                        }
                        else
                        {
                            Console.WriteLine("===========Call not Delare  : in Function===========");
                            Console.WriteLine("Func " + nameFunction + "() " + "call " + name + "()");
                            Environment.Exit(0);
                           
                        }
                        */
                        //check Delare 
                        if (!checkFunction(name, para, "c", nameFunction))
                        {
                            Console.WriteLine("===========Call not Delare  : in Function===========");
                            Console.WriteLine("Func " + nameFunction + "() " + "call " + name + "()");
                            Environment.Exit(0);
                        }
                        call.Add(new Function(name, para));
                        if (current.word == ")")
                        {
                            if(checkFunction(name, para, "c", nameFunction))
                            {
                                //call.Add(new Function(name, para));
                                /*
                                Console.WriteLine("===========Call Paramiter not equal in Function:===========");
                                Console.WriteLine("Func " + nameFunction + "() " + "Call: " + name + " (" + para + ") " + "");
                                Environment.Exit(0);*/
                            }
                            else
                            {
                                foreach (var p in pro)
                                {
                                    if (name != p.getName() && para != p.getPara())
                                    {
                                        /*Console.WriteLine("===========Call not Delare  : in Function===========");
                                        Console.WriteLine("Func " + nameFunction + "() " + "call " + name + "()");
                                        Environment.Exit(0);
                                        */
                                        Console.WriteLine("===========Call Paramiter not equal in Function:===========");
                                        Console.WriteLine("Func " + nameFunction + "() " + "Call: " + name + " (" + para + ") " + "");
                                        Environment.Exit(0);
                                        /*Console.WriteLine("===========Function Duplication===========");
                                        Console.WriteLine("Func " + name + "() ");
                                        Environment.Exit(0);
                                        */
                                    }
                                }

                                func.Add(new Function(name, para));
                            }
                            /*if (checkFunction(name, para, "c", nameFunction))
                            {
                                call.Add(new Function(name, para));
                            }
                            else
                            {
                                Console.WriteLine("===========Call not Delare  : in Function===========");
                                Console.WriteLine("Func " + nameFunction + "() " + "call " + name + "()");
                                Environment.Exit(0);

                            }
                            */
                            inter.Add(new Inter("CALL","-",countAgu.ToString(),nameCall));
                            array_Infix.Clear();
                            array_postfix.Clear();
                            
                            current = getToken();
                            if(current.word == ";")
                            {
                                current = getToken();
                                flagecall = 0;
                                countAgu = 0;
                            }
                            else
                            {
                                printSystaxError("C()", current.word);
                            }
                        }
                        else
                        {
                            printSystaxError("C()", current.word);
                        }
                    }
                    else
                    {
                        printSystaxError("C()", current.word);
                    }
                }
                else
                {
                    printSystaxError("C()", current.word);
                }
            }
            else
            {
                printSystaxError("C()",current.word);
            }
        }
        
        private void E(ref string para,string name,string nameFunction, ref string id)
        {
           
            string type = "";
            string temp_ = "";
            if (current.type == "ID" || current.type == "int_const" || current.type == "float_const" ||current.type =="string_const")
            {
                
                I(ref id,ref type);
                if (type == "int_const")
                {
                    type = "int";
                }
                else if (type == "float_const")
                {
                    type = "float";
                }
                else if (type == "string_const")
                {
                    type = "string";
                }
                else
                {
                    findTypeID(id, ref type,"Local");
                }
                
                para = para + type;
                countAgu++;
               if(flagesw==1)
                {
                    if (stackSW.Peek().ToString() != type)
                    {
                        Console.WriteLine("===========Assign Not Macthe in Function===========");
                        Console.WriteLine("Func " + nameFunction + "() switch()" + id + " " + type);
                        Environment.Exit(0);
                    }
                }
                if(current.word=="," && flagecall == 1)
                {
                    infixToPostfix(array_Infix);
                    //printPostfix();
                    postfixToTuple(ref array_postfix, ref temp_);
                    inter.Add(new Inter("ARG", "-", "-", temp_));
                    array_Infix.Clear();
                    array_postfix.Clear();
                    type = "";
                    name = "";
                }
                
                E1(ref para, id, type,nameFunction,ref temp_);
                

            }
            else if(current.word==")")
            {

            }
            else
            {
                printSystaxError("E()", current.word);
            }
        }

        private void E1(ref string para,string name,string type1,string nameFunction,ref string temp_)
        {
            string id = "";
            string type = "";
            string temppara = "";
            bool first = false;
            string Op = ""; 
            // notchage name , type 
            if (current.word == "+" || current.word == "-" || current.word == "*" || current.word == "/")
            {
                O(ref Op);
                
                I(ref id, ref type);
                if (type == "int_const")
                {
                    type = "int";
                }
                else if (type == "float_const")
                {
                    type = "float";
                }
                else if (type == "string_const")
                {
                    type = "string";
                }
                else
                {
                    findTypeID(id, ref type,"Local");
                }
                if (type == "string")
                {
                    if (Op == "/" || Op == "*")
                    {
                        Console.WriteLine("===========Operation String Not Macthe in Function===========");
                        Console.WriteLine("Func " + nameFunction + "() " + id + " " + type);
                        Environment.Exit(0);
                    }
                }
                if (checekdVarDelare(type, id) == false)
                {
                    Console.WriteLine("===========Var Not Delare in Function===========");
                    Console.WriteLine("Func " + nameFunction + "() " + id);
                    Environment.Exit(0);
                }
                if (type1 != type)
                {
                    Console.WriteLine("===========Assign Not Macthe in Function===========");
                    Console.WriteLine("Func " + nameFunction + "() " + id + " " + type);
                    Environment.Exit(0);
                }
                if (flageOpera == 1)
                {
                   
                    flageOpera = 0;
                }
                else
                {
                    para = para + type1;
                }
                if (flagecall == 1 && current.word == ")")
                {
                    infixToPostfix(array_Infix);
                    //printPostfix();
                    postfixToTuple(ref array_postfix, ref temp_);
                    inter.Add(new Inter("ARG", "-", "-", temp_));
                    array_Infix.Clear();
                    array_postfix.Clear();
                }
                else if (flagecall == 1 && current.word == ",")
                {
                    infixToPostfix(array_Infix);
                    //printPostfix();
                    postfixToTuple(ref array_postfix, ref temp_);
                    inter.Add(new Inter("ARG", "-", "-", temp_));
                    array_Infix.Clear();
                    array_postfix.Clear();
                    type1 = "";
                    name = "";
                }
                    E1(ref para,name,type1,nameFunction,ref temp_);
            }
            else if(current.word==",")
            {
                current = getToken() ;
                
                I(ref id, ref type);

                if (type == "int_const")
                {
                    type = "int";
                }
                else if (type == "float_const")
                {
                    type = "float";
                }
                else if (type == "string_const")
                {
                    type = "string";
                }
                else
                {
                    findTypeID(id, ref type, "Local");
                }
                if (checekdVarDelare(type, id) == false)
                {
                    Console.WriteLine("===========Var Not Delare in Function===========");
                    Console.WriteLine("Func " + nameFunction + "() " + id );
                    Environment.Exit(0);
                }
                
                if(first == false)
                {
                    name = id;
                    type1 = type;
                }
                else if(first==true)
                {
                    if (type1 != type)
                    {
                        Console.WriteLine("===========Assign Not Macthe in Function===========");
                        Console.WriteLine("Func " + nameFunction + "() " + id + " " + type);
                        Environment.Exit(0);
                    }
                }
                para = para + type;
                temppara = para;
                countAgu++;
                if (flagecall == 1 &&  current.word == ")" )
                {
                    infixToPostfix(array_Infix);
                    //printPostfix();
                    postfixToTuple(ref array_postfix, ref temp_);
                    inter.Add(new Inter("ARG", "-", "-", temp_));
                    array_Infix.Clear();
                    array_postfix.Clear();
                }
                else if (flagecall == 1 && current.word == ",")
                {
                    infixToPostfix(array_Infix);
                    //printPostfix();
                    postfixToTuple(ref array_postfix, ref temp_);
                    inter.Add(new Inter("ARG", "-", "-", temp_));
                    array_Infix.Clear();
                    array_postfix.Clear();
                    type1 = "";
                    name = "";
                }
                E1(ref para, name,type1,nameFunction,ref temp_);
            }
            //else if(current.word==")" || current.word ==";")
            else if (current.word == ")" || current.word == ";" || current.word == ":")
            {

            }
            else
            {
                printSystaxError("E1()", current.word);
            }
        }
        
        private void I(ref string id,ref string type)
        {
            if(current.type=="ID" || current.type=="int_const" || current.type == "float_const" || current.type =="string_const")
            {
                array_Infix.Add(current.word);
                if(current.type=="ID")
                {
                    id = current.word;
                    type = "";
                    
                }
                else
                {
                    //id = "";
                    id = current.word;
                    type = current.type;
                    
                }
                current = getToken();
            }
            else
            {
                printSystaxError("I()",current.word);
            }
        }

        private void O(ref string op)
        {
            
            if (current.word=="+"||current.word=="-" || current.word =="*" || current.word=="/")
            {
                op = current.word;
                array_Infix.Add(current.word);
                current = getToken();
                flageOpera = 1;
            }
            else
            {
                printSystaxError("O()",current.word);
            }
        }

        private void O1(ref string opera)
        {
            if (current.word == ">" || current.word == "<" || current.word == ">=" || current.word == "<=")
            {
                opera = current.word;
                array_Infix.Add(current.word);
                current = getToken();
            }
            else
            {
                printSystaxError("O1()", current.word);
            }
        }

        private void A(string nameFunction)
        {
            array_Infix.Clear();
            array_postfix.Clear();
            string id = "";
            string type = "";
            
            string name = "";
            string type1 = "";
            string temp = "";
            string temp_ = "";
            string para = "";
            string nameScope = "";
            if (current.type == "ID")
            {
                array_Infix.Add(current.word);
                name = current.word;

                // Check Var Delare in Gobal ? and Local1 (para)
                // first find var in Local1 
                // next find var in Gobal 
                if (findDupilcate(name, "Local1", ref nameScope) == false && findDupilcate(name, "Gobal",ref nameScope) == false )
                {
                    Console.WriteLine("===========Var Not Delare in Func:===========");
                    Console.WriteLine("Func " + nameFunction + "() " + " " + name + " " + type);
                    Environment.Exit(0);
                }
               
                //findTypeID(name, ref type1,nameScope);
                findTypeID(name, ref type1, "Local");
                current = getToken();
                if (current.word == "=")
                {
                    array_Infix.Add(current.word);
                    current = getToken();
                    I(ref id, ref type);
                    if (type == "ID")
                    {
                        if (findDupilcate(id, "Local1",ref  nameScope) == false)
                        {
                            Console.WriteLine("===========Var Not Delare in Func:===========");
                            Console.WriteLine("Func " + nameFunction + "() " + " " + name + " " + type);
                            Environment.Exit(0);
                        }
                        else
                        {
                            findTypeID(id, ref type, nameScope);

                        }
                    }
                   findTypeID(id, ref type,"Local");
                    if(type1!= type)
                    {
                        Console.WriteLine("===========Assign Not Macthe in Function===========");
                        Console.WriteLine("Func " + nameFunction + "() " + id + " " + type);
                        Environment.Exit(0);
                    }
                    E1(ref para,name,type1,nameFunction,ref temp_);
                    if (current.word == ";")
                    {
                        
                        infixToPostfix(array_Infix);
                        //printPostfix();
                        postfixToTuple(ref array_postfix,ref temp);
                        array_Infix.Clear();
                        array_postfix.Clear();
                        current = getToken();
                    }
                    else
                    {
                        printSystaxError("A()", current.word);
                    }
                }
                else
                {
                    printSystaxError("A()", current.word);
                }
            }
            else
            {
                printSystaxError("A()", current.word);
            }
        }
        
        private void W(string nameFunction)
        {
            string id = "";
            string type = "";
            string para = "";
            string temp_ = "";
           
            flagesw = 1; 
            array_Infix.Clear();
            
            if (current.word=="switch")
            {
                current = getToken();
                if (current.word=="(")
                {
                    current = getToken();
                    I(ref id,ref type);
                   
                    if (checekdVarDelare(type,id)==false)
                    {
                        Console.WriteLine("===========Var Not Delare in Function===========");
                        Console.WriteLine("Func " + nameFunction + "() " + id);
                        Environment.Exit(0);
                    }
                    findTypeID(id, ref type, "Local");
                    stackSW.Push(type.ToString());
                    E1(ref para,id, type, nameFunction,ref temp_);
                    if (current.word == ")")
                    {
                        
                        arraySW.AddRange(array_Infix);
                        infixToPostfix(array_Infix);
                        //printPostfix();
                        postfixToTuple(ref array_postfix,ref temp_);
                        array_Infix.Clear();
                        array_postfix.Clear();

                        current = getToken();
                        if (current.word == "{")
                        {
                            current = getToken();
                            W1(nameFunction,temp_);
                            
                            W3(nameFunction);
                            if(current.word =="}")
                            {
                                current = getToken();
                                while(stacklabel.Count!=0)
                                {
                                    inter.Add(new Inter("LBL", "-", "-", stacklabel.Pop().ToString()));
                                    
                                }
                                if (stackSW.Count!=0)
                                {
                                    stackSW.Pop();
                                }
                            }
                            else
                            {
                                printSystaxError("W()",current.word);
                            }
                        }
                        else
                        {
                            printSystaxError("W()", current.word);
                        }
                    }
                    else
                    {
                        printSystaxError("W()", current.word);
                    }

                }
                else
                {
                    printSystaxError("W()", current.word);
                }
            }
            else
            {
                printSystaxError("W()", current.word);
            }
        }
        
        private void W1(string nameFunction,string temp_)
        {
            ArrayList arrayTemp = new ArrayList();
            string para= "";
            string name = "";
            string opera = "";
            string id = "";
            string temp = "";
            string labelJMPF = "";
            string labelJMP = "";
            if (current.word=="case")
            {
                current = getToken();
                array_Infix.Add(temp_);
                O1(ref opera);
                E(ref para,name,nameFunction,ref id); 
                
                if (current.word == ":")
                {
                    
                    arrayTemp.AddRange(array_Infix);
                    infixToPostfix(arrayTemp);
                    //printPostfix();
                    postfixToTuple(ref array_postfix, ref temp);
                    array_Infix.Clear();
                    array_postfix.Clear();
                    current = getToken();

                    label = Label();
                    inter.Add(new Inter("JMPF", temp, "-", label));
                    labelJMPF = label;
                    S1(nameFunction);

                    flagesw = 1; 
                    label = Label();
                    inter.Add(new Inter("JMP", temp, "-", label));
                    stacklabel.Push(label);
                    labelJMP = label;
                    inter.Add(new Inter("LBL", "-", "-", labelJMPF));
                    W2(nameFunction,temp_);
                   
                }
                else
                {
                   printSystaxError("W1", current.word);
                }
            }
            else
            {
                printSystaxError("W1",current.word);
            }
        }

        private void W2(string nameFunction,string temp_)
        {
           
            if (current.word=="case")
            {
                W1(nameFunction, temp_);
                W2(nameFunction, temp_);
            }
            else if(current.word== "default" || current.word == "}" || current.word ==",")
            {

            }
            else
            {
                printSystaxError("W2()",current.word);
            }
            

        }

        private void W3(string nameFunction)
        {
            if (current.word== "default")
            {
                current = getToken();
                if(current.word == ":")
                {
                    current = getToken();
                    S1(nameFunction);
                }
                else
                {
                    printSystaxError("W3()", current.word);
                }
            
            }
            else if(current.word=="}")
            {

            }
            else
            {
                printSystaxError("W3()",current.word);
            }
        }

        private void T(ref string id, ref string para,string nameFunction)
        {
            if (current.word == "int" || current.word == "float" || current.word == "string")
            {
                string id_temp = "";
                string type_temp = "";

                T1(ref type_temp);
                para = para + type_temp;
                if (current.type == "ID")
                {
                    id = current.word;
                    chckeDuplicate(id, type_temp,addrs, nameFunction);
                    current = getToken();
                    T2(id, type_temp,ref para, nameFunction);
                    
                    if (current.word == ";")
                    {
                        current = getToken();
                        
                        T(ref id_temp, ref para, nameFunction);
                    }
                    else
                    {
                        printSystaxError("T()", current.word);
                    }
                }
                else
                {
                    printSystaxError("T()", current.word);
                }
            }
            else if (current.type == "ID" || current.word == "switch" || current.word == "call" || current.word == ";" || current.word == "}" || current.word == "#" || current.word == "Func" || current.word == ")")
            {

            }
            else
            {
                printSystaxError("T()", current.word);
            }
        }

        private void T1(ref string type)
        {
            
            if (current.word == "int" || current.word == "float" || current.word == "string")
            {
                type = current.word;
                current = getToken();
            }
            else
            {
                printSystaxError("T1()",current.word);
            }
        }
        
        private void T2(string id,string type,ref string  para,string nameFunction)
        {

            if(current.word==",")
            {

                current = getToken();
                id = current.word;
                if (current.type =="ID")
                {
                    chckeDuplicate(id, type,addrs, nameFunction);
                    para = para + type;
                    current = getToken();
                    T2(id,type,ref para, nameFunction);
                }
                else
                {
                    printSystaxError("T2()",current.word);
                }
            }
            else if(current.word==";")
            {
                
            }
            else
            {
                printSystaxError("T2()", current.word);
            }

        }
        
        private void T3(ref string para)
        {
            string type = "";
            string id = "";
            string nameFunction = "";
            if (current.word == "int" || current.word == "float" || current.word == "string")
            {
                T1(ref type);
                para = type; 
                if(current.type=="ID")
                {
                    id = current.word;
                    chckeDuplicate(id,type, addrs, nameFunction);
                    current = getToken();
                    T3(ref para);
                }
                else 
                {
                    printSystaxError("T3()", current.word);
                }
            }
            else if(current.word==",")
            {
                current = getToken();
                T1(ref type);
                para = para + type; 
                if (current.type == "ID")
                {
                    id = current.word;
                    chckeDuplicate(id, type, addrs, nameFunction);
                    current = getToken();
                    T3(ref para);
                }
                else
                {
                    printSystaxError("T3()", current.word);
                }

            }
            else if(current.word==")")
            {

            }
            else
            {
                printSystaxError("T3()", current.word);
            }
        }

        // Check name Duplicate in Local and Gobal
        // addrs = 1 is Gobal 
        // addrs = 0 
        private void chckeDuplicate(string id, string type, int addrs ,string nameFunction)
        {
            string nameScope = "";
            if (this.addrs == 1)
            {
                if (findDupilcate(id, "Gobal",ref nameScope))
                {
                    SemanticError("Var Duplicate: ", id, type, "Gobal");
                    Environment.Exit(0);
                }
                else
                {
                    gobal.Add(new Var(id, type, "Gobal"));
                }
            }
            else
            {
                if(nameFP == "F")
                {
                    if(findDupilcate(id, "P", ref nameScope))
                    {
                        Console.WriteLine("===========Var Duplicate in Function===========");
                        Console.WriteLine("Func " + nameFunction + "() " + id);
                        Environment.Exit(0);
                    }
                    if (findDupilcate(id, "Local", ref nameScope)==true)
                    {
                        //Console.WriteLine("===========Var Not Delare in Function===========");
                        Console.WriteLine("===========Var Duplicate in Function===========");
                        Console.WriteLine("Func " + nameFunction + "() " + id);
                        Environment.Exit(0);
                    }
                    else
                    {
                        local.Add(new Var(id, type, sl));
                    }
                }
                else if (nameFP == "P")
                {
                    if (findDupilcate(id, "Local", ref nameScope))
                    {
                        Console.WriteLine("===========Var Duplicate in Prototype===========");
                        Console.WriteLine(nameFunction + "(" + id + ")");
                        Environment.Exit(0);
                    }
                    else
                    {
                        local.Add(new Var(id, type, sl));
                    }

                }
            }
        }

        //Check VarDelare in Local and Gobal
        // 
        private bool checekdVarDelare(string type, string id)
        {
            if (type == "")
            {
                for (int i = 0; i <= local.Count - 1; i++)
                {
                    if (id == local[i].getId_name())
                    {
                        return true;
                    }
                }

                for (int i = 0; i <= gobal.Count - 1; i++)
                {
                    if (id == gobal[i].getId_name())
                    {
                        return true;
                    }

                }
            }
            else
            {
                return true;
            }
            return false;
        }

        //Check Para Function ,Call 
        // Para in Function , para in Call 
        private bool checkFunction(string name, string para, string v,string nameFunction)
        {
            bool flagepara = false;
            bool flagepara_ = false;
            if (v == "f")
            {
               
                for (int i = 0; i <= pro.Count - 1; i++)
                {
                    if (name == pro[i].getName() && para == pro[i].getPara())
                    {
                        return true;
                    }
                    else if (name == pro[i].getName() && para != pro[i].getPara())
                    {
                        if(para != pro[i].getPara())
                        {
                            flagepara = false;
                        }
                        else
                        {
                            flagepara = true;
                        }
                        /*Console.WriteLine("===========Function Paramiter not equal:===========");
                        Console.WriteLine("Func: " + name + " (" + para + ") " + "");
                        Environment.Exit(0);
                        */
                    }
                }
                return false;
                /*
                if(flagepara==false)
                {
                    Console.WriteLine("===========Function Paramiter not equal:===========");
                    Console.WriteLine("Func: " + name + " (" + para + ") " + "");
                    Environment.Exit(0);
                }*/

            }
            else if (v == "c")
            {
                for(int i = 0; i <= pro.Count - 1; i++)
                {
                    if (name == pro[i].getName() && para == pro[i].getPara())
                    {
                        return true;
                    }
                    else if (name == pro[i].getName() && para != pro[i].getPara())
                    {
                        /*
                        if (para != pro[i].getPara())
                        {
                            flagepara_ = false;
                        }
                        else
                        {
                            flagepara_ = true;
                        }
                        */
                        /*Console.WriteLine("===========Call Paramiter not equal in Function:===========");
                        Console.WriteLine("Func "+ nameFunction +"() "+"Call: " + name + " (" + para + ") " + "");
                        Environment.Exit(0);
                        return false;*/
                    }
                }
            }
            
            /*
            if (flagepara_ == false)
            {
                Console.WriteLine("===========Function Paramiter not equal:===========");
                Console.WriteLine("Func: " + name + " (" + para + ") " + "");
                Environment.Exit(0);
            }
            */
            return false;
        }
        

        //Find name Delare,Var Dupilcate?
        //Prototype Dupilcate var Gobla? , Prototype Dupilcate 
        //Func Delare,Func Dupilcate
        //Call Delare in Prototype? 
        //Var Delare,Var Dupilcate
        private bool findDupilcate(string word, string v, ref string nameScope)
        {
            if (v == "Gobal")
            {
                foreach (var g in gobal)
                {
                    if (word == g.getId_name())
                    {
                        nameScope = v;
                        return true;
                    }

                }
                return false;

            }
            else if (v == "Local" || v == "Local1")
            {
                foreach (var l in local)
                {
                    if (word == l.getId_name())
                    {
                        nameScope = v;
                        return true;
                    }
                }
                return false;
            }
            else if (v == "P")
            {
                foreach (var f in pro)
                {
                    if (word == f.getName())
                    {
                        nameScope = v;
                        return true;
                    }
                }
                return false;
            }
            /*
            else if (v == "P")
            {
                foreach (var f in pro)
                {
                    if (word == f.getName())
                    {
                        nameScope = v;
                        return true;
                    }
                }
                return false;
            }*/
            else if (v == "F")
            {
                foreach (var f in func)
                {
                    if (word == f.getName())
                    {
                        nameScope = v;
                        return true;
                    }
                }
                return false;
            }
            return false;
        }

        
        // Find name for get type
        // find Var in Gobla next find Local
        // find Var in Local next find Gobal  
        private void findTypeID(string id, ref string type ,string nameScope)
        {
            bool falge = false; 
            // id 
            if (type == "")
            {
                if (nameScope == "Gobal")
                {
                    foreach (var i in gobal)
                    {
                        if (i.getId_name() == id)
                        {
                            type = i.getType_name();
                            falge = true;
                            break;
                        }
                    }
                    if (falge == false)
                    {
                        foreach (var i in local)
                        {
                            if (i.getId_name() == id)
                            {
                                type = i.getType_name();
                                break;

                            }
                        }

                    }
                }
                else if (nameScope == "Local")
                {
                    foreach (var i in local)
                    {
                        if (i.getId_name() == id)
                        {
                            type = i.getType_name();
                            falge = true;
                            break;

                        }
                    }
                    if (falge == false)
                    {
                        foreach (var i in gobal)
                        {
                            if (i.getId_name() == id)
                            {
                                type = i.getType_name();
                                break;

                            }
                        }
                    }
                }
            }
            else
            {
                if (type == "int_const")
                {
                    type = "int";
                }
                else if (type == "float_const")
                {
                    type = "float";
                }
                else if (type == "string_const")
                {
                    type = "string";
                }
                
            }
        }

        private bool chckeCallDup(string name)
        {
            for (int i = 0; i <= call.Count - 1; i++)
            {
                if (name == call[i].getName())
                {
                    return true;
                }
            }
            return false;
        }

        //removVar int Local0 Prototype
        private void removeVar(string s)
        {
            for (int i = local.Count - 1; i >= 0; i--)
            {
                if (local[i].getStatus() == s)
                {
                    local.RemoveAt(i);
                }
            }
        }

        private void removeFunc(string nameFunction)
        {
            for (int i = func.Count - 1; i >= 0; i--)
            {
                if (nameFunction == func[i].getName())
                {
                    func.RemoveAt(i);
                }
            }
        }

        private void infixToPostfix(ArrayList arrInfix)
        {

            string postfix = "";
            string temp = "";
            foreach (string infix in arrInfix)
            {

                if (infix == "+" || infix == "-" || infix == "*" || infix == "/" || infix == "=" || infix == ">" || infix == "<" || infix == ">=" || infix == "<=")
                {
                    while (mystack.Count != 0 && priority(mystack.Peek().ToString()) <= priority(infix))
                    {
                        temp = mystack.Pop().ToString();
                        array_postfix.Add(temp);
                        postfix = postfix + temp;
                    }
                    mystack.Push(infix);

                }
                else
                {
                    postfix = postfix + infix;
                    array_postfix.Add(infix);
                }
            }
            while (mystack.Count != 0)
            {
                temp = mystack.Pop().ToString();
                postfix = postfix + temp;
                array_postfix.Add(temp);
            }
            mystack.Clear();
        }

        private void postfixToTuple(ref ArrayList array, ref string temp)
        {
            string L = "";
            string R = "";
            string opcode = "";
            string newtemp = "";

            foreach (string i in array)
            {
                if (i == "+" || i == "-" || i == "*" || i == "/" || i == ">" || i == "<" || i == "<=" || i == ">=" || i == "=")
                {
                    if (i == "=")
                    {
                        opcode = i;
                        L = mystack.Pop().ToString();
                        R = mystack.Pop().ToString();
                        addTuple(opcode, L, "-", R);
                        inter.Add(new Inter(opcode, L, "-", R));
                        //codegen.Add(new Codegen(opcode,"R",R) );


                    }
                    else
                    {

                        newtemp = Temp();
                        temp = newtemp;
                        opcode = i;
                        L = mystack.Pop().ToString();
                        R = mystack.Pop().ToString();
                        mystack.Push(newtemp);
                        addTuple(opcode, R, L, newtemp);
                        inter.Add(new Inter(opcode, R, L, temp));
                    }

                }
                else
                {
                    mystack.Push(i);
                    temp = i;
                }
            }
            mystack.Clear();
        }

        private void addTuple(string opcode, string L, string R, string temp)
        {
            array_Tuple.Add(opcode);
            array_Tuple.Add(L);
            array_Tuple.Add(R);
            array_Tuple.Add(temp);
        }

        private int priority(string i)
        {
            if (i == "*" || i == "/")
            {
                return 1;
            }
            else if (i == "+" || i == "-")
            {
                return 2;
            }
            else if (i == ">" || i == "<" || i == ">=" || i == "<=")
            {
                return 3;
            }
            else if (i == "=")
            {
                return 4;
            }
            return 0;
        }

        public void printTable()
        {
            Console.WriteLine("=================Gobal==========");
            Console.WriteLine("Name\t\t"+"Type");
            foreach(var g in gobal)
            {
                Console.WriteLine(""+g.getId_name()+"\t\t"+g.getType_name());
            }
            Console.WriteLine("=================================");

            Console.WriteLine("=================Lobal==========");
            Console.WriteLine("Name\t\t" + "Type");
            foreach (var l in local)
            {
                Console.WriteLine("" + l.getId_name() + "\t\t" + l.getType_name());
            }
            Console.WriteLine("=================================");
            Console.WriteLine("==============Prototype==========");
            Console.WriteLine("Name\t\t");
           foreach (var p in pro)
            {
                Console.WriteLine("" + p.getName());
            }
            Console.WriteLine("=================================");
            
            Console.WriteLine("=================Function==========");
            Console.WriteLine("Name\t\t");
           foreach (var f in func)
            {
                Console.WriteLine("" + f.getName());
            }
            Console.WriteLine("=================================");

            Console.WriteLine("=================Call===========");
            Console.WriteLine("Name\t\t");
            foreach (var f in call)
            {
                Console.WriteLine("" + f.getName());
            }
            Console.WriteLine("=================================");
        }

        public void printPostfix()
        {
            string post = "";
            string inf = "";
            Console.WriteLine("================Intfix==================");
            foreach (var i in array_Infix)
            {
                inf = inf + i;
            }
            Console.WriteLine(inf);
            Console.WriteLine("=======================================");
            Console.WriteLine("================Postfix================");
            foreach(var i in array_postfix)
            {
                
                post = post + i;
            }
            Console.WriteLine(post);
            Console.WriteLine("=======================================");
        }

        private void printSystaxError(string st1, string st2)
        {
            syntax = 1;
            Console.WriteLine("Systax Error " + st1 + ": " + st2);
            Environment.Exit(0);
            
        }

        public void printTuple()
        {
            int count = 0;
            Console.WriteLine("================Tuple==================");
            foreach (string i in array_Tuple)
            {
                if (count == 4)
                {
                    Console.WriteLine();
                    Console.Write(i + "\t");
                    count = 1;
                }
                else
                {
                    Console.Write(i + "\t");
                    count++;
                }

            }
            Console.WriteLine();
            Console.WriteLine("=======================================");


        }

        private void SemanticError(string v, string id, string type, string ad)
        {
            Console.WriteLine("===========" + v + "===========");
            Console.WriteLine("" + type + " " + id + " " + ad);
        }

        public void printInter()
        {
            Console.WriteLine("================Intermediate============");
            foreach (var i in inter)
            {
                Console.WriteLine(i.getOP() + "\t" +i.getOpr1() + "\t" + i.getOpr2() +"\t" + i.getResult());// +"\t" +i.getOpr1()+"\t"+i.getOpr2+"\t"+i.getResult);
            }
            Console.WriteLine("=======================================");
        }
      
        class Function
        {
            string name;
            string para;

            public Function(string name, string para)
            {
                this.name = name;
                this.para = para;
            }

            public String getName()
            {
                return name; 
            }
            public String getPara()
            {
                return para;
            }
        }
        
        class Var
        {
            string id_name;
            string type_name;
            string status;
           

            public Var(string v1, string v2, string v3)
            {
                this.id_name = v1;
                this.type_name = v2;
                this.status = v3;
            }

           
            public String getId_name()
            {
                return id_name;
            }
            public String getType_name()
            {
                return type_name;
            }
            public String getStatus()
            {
                return status;
            }
        }
    }
}
class Inter
{

    string opcode;
    string opr1;
    string opr2;
    string result;
    public Inter(string opcode, string opr1, string opr2, string result)
    {
       
        switch(opcode)
        {
            case "=":
                this.opcode = "MOV";
                break;
            case "+":
                this.opcode = "ADD";
                break;
            case "-":
                this.opcode = "SUB";
                break;
            case "*":
                this.opcode = "MULT";
                break;
            case "/":
                this.opcode = "DIV";
                break;
            case ">":
                this.opcode = "CMPGT";
                break;
            case "<":
                this.opcode = "CMPLT";
                break;
            case ">=":
                this.opcode = "CMPGET";
                break;
            case "<=":
                this.opcode = "CMPLET";
                break;
            default:
                this.opcode = opcode;
                break;
        }
        this.opr1 = opr1;
        this.opr2 = opr2;
        this.result = result;
    }
    
    public string getOP()
    {
        return opcode;
    }
    public string getOpr1()
    {
        return opr1;
    }
    public string getOpr2()
    {
        return opr2;
    }
    public string getResult()
    {
        return result;
    }



}

class LabelTable
{
    public string label;
    public string value;

    public LabelTable(string label, string value)
    {
        this.label = label;
        this.value = value;
    }

    public String getLabel()
    {
        return label;
    }

    public String getValue()
    {
        return value;
    }
   
}

class Codegen
{
    private List<Inter> inter;
    private List<Codegen> codegen = new List<Codegen>();
    private List<LabelTable> lablel = new List<LabelTable>(); 
    string opcode; 
    string opr1;
    string opr2;
    string result;
    int countT_ = 0;
    public String getAddrs(string label_)
    {
        
        for(int i = 0; i<lablel.Count;i++)
        {
            if(label_ == lablel[i].getLabel())
            {
                //sg = lablel[i].getValue() - 1; 
                return lablel[i].getValue();
            }
            
        }
        return "";
    }
    public Codegen(List<Inter> inter)
    {
        this.inter = inter;

        foreach (var i in inter)
        {
            if(i.getOP() == "LBL")
            {
                lablel.Add(new LabelTable(i.getResult(), "" + countT_));
            }
            else if(i.getOP() == "JMP")
            {
                countT_ = countT_ + 1;
            }
            else if(i.getOP() == "JMPF")
            {
                countT_ = countT_ + 2;
            }
            else
            {
                countT_ = countT_ + 3;
            }

        }

        
        foreach (var i in inter)
        {
            switch (i.getOP())
            {
                
                case "MOV":
                    this.opcode = "MOV";
                    codegen.Add(new Codegen("LOAD", "R", i.getOpr1(), ""));
                    codegen.Add(new Codegen("STORE", "R", i.getResult(), ""));
                    break;
                case "ADD":
                    this.opcode = "ADD";
                    codegen.Add(new Codegen("LOAD", "R", i.getOpr1(), ""));
                    codegen.Add(new Codegen("ADD", "R", i.getOpr2(), ""));
                    codegen.Add(new Codegen("STORE", "R", i.getResult(), ""));
                    break;
                case "SUB":
                    this.opcode = "SUB";
                    codegen.Add(new Codegen("LOAD", "R", i.getOpr1(), ""));
                    codegen.Add(new Codegen("SUB", "R", i.getOpr2(), ""));
                    codegen.Add(new Codegen("STORE", "R", i.getResult(), ""));
                    break;
                case "MULT":
                    this.opcode = "MULT";
                    codegen.Add(new Codegen("LOAD", "R", i.getOpr1(), ""));
                    codegen.Add(new Codegen("MUL", "R", i.getOpr2(), ""));
                    codegen.Add(new Codegen("STORE", "R", i.getResult(), ""));
                    break;
                case "DIV":
                    this.opcode = "DIV";
                    codegen.Add(new Codegen("LOAD", "R", i.getOpr1(), ""));
                    codegen.Add(new Codegen("DIV", "R", i.getOpr2(), ""));
                    codegen.Add(new Codegen("STORE", "R", i.getResult(), ""));
                    break;
                case "CMPGT":
                    this.opcode = "CMPGT";
                    codegen.Add(new Codegen("LOAD", "R", i.getOpr1(), ""));
                    codegen.Add(new Codegen("CMPGT", "R", i.getOpr2(), ""));
                    codegen.Add(new Codegen("STORE", "R", i.getResult(), ""));
                    break;
                case "CMPLT":
                    this.opcode = "CMPLT";
                    codegen.Add(new Codegen("LOAD", "R", i.getOpr1(), ""));
                    codegen.Add(new Codegen("CMPLT", "R", i.getOpr2(), ""));
                    codegen.Add(new Codegen("STORE", "R", i.getResult(), ""));
                    break;
                case "CMPGET":
                    this.opcode = "CMPGET";
                    codegen.Add(new Codegen("LOAD", "R", i.getOpr1(), ""));
                    codegen.Add(new Codegen("CMPGET", "R", i.getOpr2(), ""));
                    codegen.Add(new Codegen("STORE", "R", i.getResult(), ""));
                    break;
                case "CMPLET":
                    this.opcode = "CMPLET";
                    codegen.Add(new Codegen("LOAD", "R", i.getOpr1(), ""));
                    codegen.Add(new Codegen("CMPLET", "R", i.getOpr2(), ""));
                    codegen.Add(new Codegen("STORE", "R", i.getResult(), ""));
                    break;
                case "LBL":
                    break;
                    string sct;/*
                    sct = i.getResult().Replace("L","");
                    codegen.Add(new Codegen(sct, ":", "", ""));
                    break;*/
                case "JMPF":
                    codegen.Add(new Codegen("LOAD", "R", i.getOpr1(), ""));
                    /*codegen.Add(new Codegen("STORE", "R", i.getResult(), ""));*/
                    codegen.Add(new Codegen("JUMPF",getAddrs(i.getResult()), "",""));
                    break;
                case "JMP":
                    /*codegen.Add(new Codegen("LOAD", "R", i.getOpr1(), ""));
                    codegen.Add(new Codegen("STORE", "R", i.getResult(), ""));*/
                    codegen.Add(new Codegen("JUMP", getAddrs(i.getResult()), "", ""));
                    break;
                default:
                    break;
            }
        
        }
       
    }

    
    public Codegen(string opcode, string opr1, string opr2 ,string result)
    {
        this.opcode = opcode;
        this.opr1 = opr1;
        this.opr2 = opr2;
        this.result = result;
        

    }
    int countI = 0; 
    public string getOP()
    {
        return opcode;
    }
    public string getOpr1()
    {
        return opr1;
    }
    public string getOpr2()
    {
        return opr2;
    }
    public string getRe()
    {
        return result;
    }
    public void printCodegen()
    {
        foreach(var i in codegen)
        {
            Console.WriteLine(""+(countI++)+"\t"+i.getOP()+"\t"+i.getOpr1()+"\t"+i.getOpr2()+"\t"+i.getRe());
            //Console.WriteLine("" + i.getOP() + "\t" + i.getOpr1() + "\t" + i.getOpr2() + "\t" + i.getRe());
         }
        Console.WriteLine("" + (countI++) + "\t");
    }
    public void printLabe()
    {
        Console.WriteLine("========================");
        foreach (var i in lablel)
        {
            Console.WriteLine(i.getLabel()+"\t "+i.getValue());
        }
    }
}

