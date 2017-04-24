using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace APIModels
{
    [DataContract]
    public class VerbConjugations
    {
        #region Data Members

        [DataMember]
        private int tenseId;

        [DataMember]
        private int personId;

        [DataMember]
        private string conjugation;

        #endregion

        #region Ctor

        public VerbConjugations(int tenseId, int personId, string conjugation)
        {
            this.tenseId = tenseId;
            this.personId = personId;
            this.conjugation = conjugation;
        }

        #endregion
    }
}