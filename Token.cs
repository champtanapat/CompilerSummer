using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompilerSu59
{
    class Token
    {
        public string word;
        public string type;

        public Token(string word, string type)
        {
            this.word = word;
            this.type = type;
        }

        public string getWord()
        {
            return word;
        }

        public string getType()
        {
            return type;
        }

        public void setWord(string word)
        {
            this.word = word;
        }

        public void setType(string type)
        {
            this.type = type;
        }
    }
}
