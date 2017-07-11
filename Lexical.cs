using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
namespace CompilerSu59
{

    class Lexical
    {
        private string sc;
        private List<Lexical> lex = new List<Lexical>();
        public List<Token> list = new List<Token>();
        //private Stack stack = new Stack();
        private int index = 0;
        private int subindex = 0;
        private Stack stcomment = new Stack();
        private Stack sCopy = new Stack();
        

        public Lexical(string c)
        {
            this.sc = c;
        }

        public Token getToken()
        {
            return list[index++];
        }
       
        
        public void WordLex()
        {

            while (sc[index] != '#')
            {
                switch (sc[index])
                {
                    case ';':
                    case ':':
                    case ')':
                    case '(':
                    case '{':
                    case '}':
                    case ',':

                        list.Add(new Token(sc[index].ToString(), "SP"));
                        break;
                    case '\n':
                    case '\r':
                    case '\t':
                        break;
                    case '+':
                    case '-':
                    case '*':
                        Operater(sc[index].ToString());
                        break;
                    case '/':
                        subindex = index;
                        if (sc[index + 1] == '*')
                        {
                            index++;
                            if(sc[index] == '*')
                            {
                                index++;
                                Comment_();
                            }
                            else
                            {
                                subindex = index;
                            }
                            
                        }
                        else if(sc[index + 1] == '#')
                        {
                            list.Add(new Token(sc[index].ToString(), "OP"));
                        }
                        else
                        {
                           
                            stcomment = new Stack();
                            sCopy = new Stack();
                            stcomment.Push(sc[index]);
                            sCopy.Push(sc[index]);
                            index++;
                            Comment_First();
                            stcomment.Clear();
                            sCopy.Clear();

                        }
                        break;
                    case '=':
                        Operater(sc[index].ToString());
                        break;
                    case '>':
                    case '<':
                        string st_temp = "";
                        st_temp = st_temp + sc[index];
                        if (sc[index + 1] == '=')
                        {
                            index++;
                            st_temp = st_temp + sc[index];
                            Operater(st_temp);
                        }
                        else
                        {
                            Operater(sc[index].ToString());
                        }

                        break;

                    default:
                        //--int_const float_const
                        if (char.IsDigit(sc[index]) || sc[index] == '.')
                        {
                            string st = "";
                            Digit(st);
                        }
                        //--string_const
                        else if (sc[index] == '$')
                        {
                            String_Const();
                        }
                        //-- ID
                        else if (char.IsLetter(sc[index]))
                        {
                            Letter();
                        }

                        break;
                }
                index++;
            }
            if (sc[index] == '#')
            {
                list.Add(new Token(sc[index].ToString(), "#"));
            }
        }

        private void Comment_First()
        {
            string temp = "";
            int indextemp = 0;
            while (char.IsLetterOrDigit(sc[index]) && sc[index] != '^')
            {
                temp = temp + sc[index];
                stcomment.Push(sc[index]);
                sCopy.Push(sc[index]);
                index++;
            }
            if (sc[index] == '#')
            {
                index = subindex;
                list.Add(new Token(sc[index].ToString(), "OP"));
            }
            else
            {
                if (sc[index] == '^')
                {
                    stcomment.Push(sc[index]);
                    sCopy.Push(sc[index]);
                    index++;
                    indextemp = 0; 
                    while (sc[index]!='^' && sc[index] != '#' && sc[index] != '/')
                    {
                        index++;
                    }
                    if (sc[index] == '^')
                    {
                        index++;
                        Comment_Second(temp,indextemp);
                        
                    }
                    else if(sc[index] == '#')
                    {
                        index = subindex;
                        list.Add(new Token(sc[index].ToString(), "OP"));
                    }
                    else if(sc[index]=='/')
                    {
                        index = subindex;
                        list.Add(new Token(sc[index].ToString(), "OP"));
                    }
                    else
                    {
                        index = subindex;
                        list.Add(new Token(sc[index].ToString(), "OP"));
                    }
                }
                else if (sc[index] == '#')
                {
                    index = subindex;
                    list.Add(new Token(sc[index].ToString(), "OP"));
                }
                else if (sc[index] == '/')
                {
                    if(sc[index+1] == '*')
                    {
                        index++;
                        Comment_();
                    }
                    else
                    {
                        index = subindex;
                        list.Add(new Token(sc[index].ToString(), "OP"));
                    }
                }
                else
                {
                    while(sc[index] != '#')
                    {
                        index++;
                    }
                    index = subindex;
                    list.Add(new Token(sc[index].ToString(), "OP"));
                }
            } //--end else 

        }

        private void Comment_Second(string temp, int indextemp)
        {
         // /xy^A^
          bool falge = false;
            if (sc[index] == '^')
            {
                index++;
                // /xy^A^a
                if (char.IsLetterOrDigit(sc[index]))
                {
                    
                    falge = false;
                }
                // /xy^A^^
                else if (sc[index] == '^')
                {
                    index++;
                    while (char.IsLetterOrDigit(sc[index]))
                    {
                        index++;
                    }
                    if (char.IsLetterOrDigit(sc[index]))
                    {
                        falge = false;
                    }
                }
                indextemp = 0;
                Comement_Final(indextemp, temp, falge);

            }
            // /xy^A^ 
            else if (sc[index] == '#')
            {
                index = subindex;
                list.Add(new Token(sc[index].ToString(), "OP"));
                //index--; 
            }
            else if (char.IsLetterOrDigit(sc[index]))
            {
                Comement_Final(indextemp, temp, falge);
            }
            else if(!char.IsLetterOrDigit(sc[index]))
            {
                index++;
                Comment_Second(temp, indextemp);
            }
            else
            {
                index = subindex;
                list.Add(new Token(sc[index].ToString(), "OP"));
            }
            
           
        }

        private void Comement_Final(int indextemp,string temp,bool falge)
        {
            falge = false;
            // /xy^ ^xy
            // --xy
            while (indextemp < temp.Count() && falge == false)
            {
                if (sc[index] == temp[indextemp])
                {

                }
                // /xy^ ^
                else if(sc[index]=='#')
                {
                    index--;
                    falge = true;
                }
                else
                {
                    falge = true;

                }
                index++;
                indextemp++;
            }
            //xy^ ^xy
            if (sc[index] == '#')
            {
                index = subindex;
                falge = true;
                
            }
            //^xy ^xy^
            // find last  
            // /xy^ ^xy^xy 
            int i = 0;
            int j = 0;
            int countST = stcomment.Count;
            // --xy^
            while (i < countST && falge == false)
            {
                string c = stcomment.Pop().ToString();
                
                if (sc[index].ToString() == c)
                {
                    //j = index;
                    j++;
                    index++;
                }
                else if (sc[index] == '#')
                {
                    index--;
                    falge = true;
                }
                else
                {
                    //index = j;
                    index = index - j;
                    falge = true;
                }
                
                i++;
                /*index++;*/
            }
            if (falge == false)
            {
                Console.WriteLine("/------------Commment------------------/");
                index--;

            }
            else if (falge == true && sc[index] == '#')
            {
                index = subindex;
                list.Add(new Token(sc[index].ToString(), "OP"));
            }
            else
            {
                indextemp = 0;
                stcomment.Clear();
                stcomment = new Stack();
                Stack ST = new Stack(sCopy);
                int countS = sCopy.Count;
                for(int k = 0; k < countS; k++)
                {
                    stcomment.Push(ST.Pop() );
                }

                if (sc[index] == '#')
                {
                    index = subindex;
                    list.Add(new Token(sc[index].ToString(), "OP"));
                }
                else if (sc[index] == '/')
                {
                    index = subindex;
                    list.Add(new Token(sc[index].ToString(), "OP"));
                }
                else if(sc[index] == '\r' || sc[index] == '\n' || sc[index] == '\t')
                {
                    index = subindex;
                    list.Add(new Token(sc[index].ToString(), "OP"));
                }
                else
                {
                    if (sc[index] == '^')
                    {
                        index++;
                        Comement_Final(indextemp, temp, falge);
                    }
                    else
                    {
                        while (sc[index] != '^' && sc[index] != '#')
                        {
                            index++;
                        }
                        if (sc[index] == '^')
                        {
                            index++;
                            Comement_Final(indextemp, temp, falge);
                        }
                        else
                        {
                            index = subindex;
                            list.Add(new Token(sc[index].ToString(), "OP"));
                        }
                    }

                }

            }
        }

    
        private void Letter()
        {
            string st = "";
            st = st + sc[index];
            index++;
            //while(char.IsLetterOrDigit(sc[index]))
            while (char.IsLetter(sc[index]))
           {
                st = st + sc[index];
                index++;
            }
            if(sc[index]=='_')
            {
                st = st + sc[index];
                index++;
                while (char.IsLetter(sc[index]))
                {
                    st = st + sc[index];
                    index++;
                }
            }
            if (RW(st))
            {
                list.Add(new Token(st, "RW"));
            }
            else
            {
                list.Add(new Token(st, "ID"));
            }
            index--;
        }

        private void String_Const()
        {
            string st = "";
            int countTemp = 0;
            st = st + sc[index];
            index++;
            while (sc[index] != '$'&&sc[index]!='#')
            {
                st = st + sc[index];
                countTemp++;
                index++;
            }
            //if(sc[index]!='#')
            if (sc[index] == '$')
            {
               
                st = st + sc[index];
                list.Add(new Token(st, "string_const"));
            }
            else
            {
                Console.WriteLine("String_const Error");
                Environment.Exit(0);
            }


            //index = countTemp ;
            //index--;

        }

        private void Operater(string s)
        {
            list.Add(new Token(s,"OP"));
        }
       
        private void Digit(string st)
        {
            int tempE0 = 0;
            int flagDot = 0;
            string lateE = "";
            int countint7 = 0;
            bool flageBeforE = false;

            while (char.IsDigit(sc[index]) && countint7 <= 7)
            {
                st = st + sc[index];
                index++;
                countint7++;
                flagDot = 1;
            }
            if(countint7>=8)
            {
                Console.WriteLine("int_const Error");
                Environment.Exit(0);
            }
            countint7 = 0;
            //-- float_const 
            if (sc[index] == '.')
            {
                if (flagDot == 0)
                {
                    st = st + "0" + sc[index];
                }
                else
                {
                    st = st + sc[index];
                }
                int num_ = 0;
                index++;
                while (char.IsDigit(sc[index]))
                {
                    st = st + sc[index];
                    index++;
                    num_++;
                    flageBeforE = true;
                }
                // .1234567
                if(num_>7)
                {
                    Console.WriteLine("float_const Error");
                    Environment.Exit(0);
                }

                //check aftter . hava digit ? 
                //Yes after .1 not have E,e .1 flageBeforE = true 
                //No  after . have E,e .e flageBeforE = false

                if (flageBeforE == false)
                {
                    if (sc[index] == 'e' || sc[index] == 'E')
                    {
                        string tempConcat = "";
                        tempConcat = st;

                        st = st + sc[index];

                        index++;
                        if (sc[index] == '0')
                        {
                            st = st + sc[index];
                            index++;
                            if (char.IsDigit(sc[index]))
                            {
                                st = st + sc[index];
                                lateE = sc[index].ToString();
                                tempE0 = Int32.Parse(sc[index].ToString());

                                index++;
                            }
                            else
                            {
                                Console.WriteLine("float_const Error");
                                Environment.Exit(0);
                            }
                            // for e 
                        }
                        else
                        {
                            Console.WriteLine("float_const Error");
                            Environment.Exit(0);
                        }
                        //over digit 7 
                        //จุดหลังทศนิยมไม่เกิน 7 
                        if(tempE0>=0 && tempE0 <= 7)
                        {
                            string temp0 = "";
                            for (int i = 1; i < tempE0; i++)
                            {
                                temp0 = temp0 + 0;
                            }
                            st = tempConcat + temp0 + lateE;
                        }
                        else
                        {
                            Console.WriteLine("float_const Error");
                            Environment.Exit(0);
                        }
                        //--
                    }
                    else
                    {
                        Console.WriteLine("float_const Error");
                        Environment.Exit(0);
                    }
                    list.Add(new Token(st, "float_const"));
                }
                //Yes after .1 not have E,e .1 flageBeforE = true 
                else
                {
                    list.Add(new Token(st, "float_const"));
                }
            }
            //int_const
            else
            {
                list.Add(new Token(st, "int_const"));
            }
            index--;
        }

        private bool RW(string st)
        {
            switch(st)
            {
                case "int":
                case "float":
                case "string":
                case "call":
                 
                case "int_const":
                case "flaot_const":
                case "string_const":
                
                case "Func":
                
                case "default":
                case "switch":
                case "case":
                    return true;
                    break; 
                default:
                    return false;
                    break;
            }



        }
        
        public void printlex()
        {
            Console.WriteLine("================================");
            Console.WriteLine("Word\t\tType");
            foreach(var a  in list)
            {
                Console.WriteLine(a.getWord()+"\t\t"+a.getType());
            }
        }


        private void Comment_()
        {

        
            if (sc[index] == '*')
            {
                index++;
                if (sc[index] == '/')
                {
                    Console.WriteLine("/*------Commment-------*/");
                }
                else
                {
                    while (sc[index] != '*' && sc[index] != '#')
                    {
                        index++;
                    }
                    if (sc[index] == '*')
                    {
                        Comment_();
                    }
                    else
                    {
                        index = subindex;
                        list.Add(new Token(sc[index].ToString(), "OP"));
                    }

                }
            }
            else
            {

                while (sc[index] != '*' && sc[index] != '#')
                {
                    index++;
                }
                if (sc[index] == '*')
                {
                    index++;
                    if (sc[index] == '/')
                    {
                        Console.WriteLine("/*------Commment-------*/");
                    }
                    else
                    {
                        Comment_();
                    }
                }
                else
                {
                    index = subindex;
                    list.Add(new Token(sc[index].ToString(), "OP"));
                }
            }

        }
    }
}
