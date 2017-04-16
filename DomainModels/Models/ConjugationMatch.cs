using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotaDAL.Models
{
    [Table(Name = "ConjugationMatches")]
    public class ConjugationMatch : NotaDbObject<ConjugationMatch>
    {
        #region Ctors

        public ConjugationMatch(int verbId, int conjugationRuleId, int personId, string conjugationString)
        {
            VerbId = verbId;
            ConjugationRuleId = conjugationRuleId;
            PersonId = personId;
            ConjugationString = conjugationString;
        }

        #endregion

        #region Properties
        [Column(IsDbGenerated = true, IsPrimaryKey = true)]
        public int Id { get; set; }

        [Column(CanBeNull = false)]
        public int VerbId { get; set; }

        [Column(CanBeNull = false)]
        public int ConjugationRuleId { get; set; }

        [Column(CanBeNull = false)]
        public int PersonId { get; set; }

        [Column(CanBeNull = true)]
        public string ConjugationString { get; set; }

        #endregion

        #region NotaDbObject Implementation

        public bool DbCompare(ConjugationMatch other)
        {
            return this.
        }

        #endregion
    }
}
