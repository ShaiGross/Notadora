using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using NotaDAL.Models;

namespace APIModels
{
    [DataContract]
    public class ConjugationRuleInfo
    {
        #region Data Members

        [DataMember]
        private int id;

        [DataMember]
        private string name;

        [DataMember]
        private string description;

        [DataMember]
        private int tenseId;

        [DataMember]
        private bool isRegular;

        [DataMember]
        private ConjugationRuleType type;

        [DataMember]
        private int? patternIndex;

        [DataMember]
        private List<int> personsIds;

        [DataMember]
        private List<int> verbsIds;

        #endregion

        #region Ctor

        public ConjugationRuleInfo(int id,
                                   string name,
                                   string desc,
                                   int tenseId,
                                   bool regular,
                                   ConjugationRuleType type,
                                   int? patternIndex,                        
                                   List<int> personsIds,
                                   List<int> verbIds)
        {
            this.id = id;
            this.name = name;
            this.description = desc;
            this.tenseId = tenseId;
            this.isRegular = regular;
            this.type = type;
            this.patternIndex = patternIndex;
            this.personsIds = personsIds;
            this.verbsIds = verbIds;
        }

        #endregion
    }
}
