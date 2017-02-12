using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rar.Model
{
    public class RarFIO
    {
        #region - Public Properties -
        public string Name { set; get; }
        public string Surname { set; get; }
        public string Middlename { set; get; }
        #endregion
        #region - Constructors -
        public RarFIO() { }
        public RarFIO(string name, string surname, string middlename)
        {
            Name = name;
            Surname = surname;
            Middlename = middlename;
        }
        #endregion

        public override string ToString()
        {
            return Surname+ " " + Name +" "+Middlename;
        }
    }
}
