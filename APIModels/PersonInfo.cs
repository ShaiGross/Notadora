using NotaDAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace APIModels
{
    [DataContract]
    public class PersonInfo
    {
        #region Data Members

        [DataMember]
        private int id;

        [DataMember]
        private string description;

        [DataMember]
        private string spanishExpression;

        [DataMember]
        private PersonPlurality plurality;

        [DataMember]
        private PersonFormality formality;

        [DataMember]
        private PersonGender gender;

        [DataMember]
        private PersonOrder order;

        #endregion

        #region Ctors

        public PersonInfo(int id,
                          string desc,
                          string spanishExp,
                          PersonPlurality plurality,
                          PersonFormality formality,
                          PersonGender gender,
                          PersonOrder order)
        {
            this.id = id;
            this.description = desc;
            this.spanishExpression = spanishExp;
            this.plurality = plurality;
            this.formality = formality;
            this.gender = gender;
            this.order = order;
        }

        #endregion
    }
}
