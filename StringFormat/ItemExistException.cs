/************************************************
 *  Complany: 
 *  Author: alex.zhou
 *  Contact: zzdxym@gmail.com
 *  
 *  Create CLR: 3.5
 *  Min CLR: 3.5
 *  
 *  Create Time: 2013/3/28 16:07:26
 *  Description: 
 ************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace StringFormat
{
    public class ItemExistException : ApplicationException
    {
        #region Fields

        #endregion

        #region Properties

        #endregion

        #region Constructor

        public ItemExistException(){ }

        public ItemExistException(string message)
            : base(message)
        { }

        #endregion

        #region Methods

        #endregion
    }
}
