using StringUtils;
using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotaDAL.Models
{
    public enum VerbType
    {
        ar,
        er,
        ir
    }

    [Table(Name = "Verbs")]
    public class Verb
    {
        #region Props

        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id { get; set; }

        [Column(CanBeNull = false)]
        public string Infinative { get; set; }

        [Column(CanBeNull = false)]
        public string Description { get; set; }

        [Column(CanBeNull = false)]
        public string EnglishInfinative { get; set; }

        public VerbType Type
        {
            get
            {
                if (Infinative.DiacriticsEndsWith("ar"))
                    return VerbType.ar;
                else if (Infinative.DiacriticsEndsWith("er"))
                    return VerbType.er;
                else if (Infinative.DiacriticsEndsWith("ir"))
                    return VerbType.ir;
                else
                {
                    throw new Exception("Verb infinative doesn't end with ar, er or ir");
                }
            }
        }

        #endregion        

        #region Ctors

        public Verb()
        {

        }

        public Verb(string inf, string englishInf, string desc)
        {
            this.Infinative = inf;
            this.Description = desc;
            this.EnglishInfinative = englishInf;
        }

        #endregion

        #region Expression-Bodies Members

        public string Stem => Infinative.Remove(Infinative.Length - 2);

        #endregion
    }
}
