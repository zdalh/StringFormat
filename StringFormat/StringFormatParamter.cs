/************************************************
 *  Complany:
 *  Author: alex.zhou
 *  Contact: zzdxym@gmail.com
 *  
 *  Create CLR: 3.5
 *  Min CLR: 3.5
 *  
 *  Create Time: 2013/3/28 12:23:50
 *  Description: 
 ************************************************/

namespace StringFormat
{
    public class StringCommandParameter
    {
        #region Fields

        #endregion

        #region Properties
        public string ParamName { get; set; }
        public string Value { get; set; }
        #endregion

        #region Constructor

        public StringCommandParameter() { }

        public StringCommandParameter(string paramName, string value)
        {
            this.ParamName = paramName;
            this.Value = value;
        }

        #endregion

        #region Methods

        #endregion
    }
}
