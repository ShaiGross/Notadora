using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace APIModels
{
    [DataContract]
    public class VerbInfo
    {
        #region Data Members

        [DataMember]
        private int id;

        [DataMember]
        private string description;

        [DataMember]
        private string spanishInfinative;

        [DataMember]
        private string englishInfinative;

        [DataMember]
        private List<VerbConjugations> conjugations;

        [DataMember]
        private List<int> conjugationRulesIds;

        #endregion

        #region Ctor

        public VerbInfo(int id,
                    string desc,
                    string spanishInf,
                    string englishInf,
                    List<VerbConjugations> conjugations = null,
                    List<int> conjugationRulesIds = null)
        {
            this.id = id;
            this.description = desc;
            this.spanishInfinative = spanishInf;
            this.englishInfinative = englishInf;
            this.conjugations = conjugations;
            this.conjugationRulesIds = conjugationRulesIds;
        }

        #endregion
    }
}