/************************************************
 *  Complany: 
 *  Author: alex.zhou
 *  Contact: zzdxym@gmail.com
 *  
 *  Create CLR: 3.5
 *  Min CLR: 3.5
 *  
 *  Create Time: 2013/3/28 12:21:50
 *  Description: 
 ************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace StringFormat
{
    ///<summary>
    /// 提供参数化格式化字符串
    ///</summary>
    ///<example>
    /// <code>
    ///     var str = @"g.cn?{a=@a$}[&b=@b$][&c=@c$]";
    ///     var strFormat = new StringFormater(str);
    ///     strFormat.AddParameter("@a",1);
    ///     strFormat.AddParameter("@b",2);
    ///     str = strFormat.Format();
    /// </code>
    /// 最终str的值为 "g.cn?a=1&b=2"
    ///</example>
    public sealed class StringFormatter : IDisposable
    {
        #region Fields

        private const char EscapeChar = '\\';
        
        private const char ParamaterBeginChar = '@';
        private const char ParamaterEndChar = '$';

        private const char Required_Begin = '{';
        private const char Required_End = '}';

        private const char Optional_Begin = '[';
        private const char Optional_End = ']';

        private static readonly StringComparison _comparision = StringComparison.OrdinalIgnoreCase;

        private string _source;

        private List<_BaseDataStruct> _baseDataStructList = new List<_BaseDataStruct>();

        #endregion

        #region Properties

        public string Source
        {
            get
            {
                return _source;
            }
            set
            {
                var seted = false;
                if (string.IsNullOrEmpty(_source))
                {
                    _source = value;
                    seted = true;
                }
                else
                {
                    if (!_source.Equals(value, _comparision))
                    {
                        _source = value;
                        seted = true;
                    }
                }

                if (seted && !string.IsNullOrEmpty(_source))
                {
                    PreparBuffer();
                }
            }
        }

        private List<StringCommandParameter> Parameters { get; set; }

        #endregion

        private class _BaseDataStruct
        {
            public int index;
            public StringBuilder buffer = new StringBuilder();

            public bool unUsed = false;

            public _BaseDataStruct() { }
            public _BaseDataStruct(int index)
            {
                this.index = index;
            }
        }

        private class _DataStruct : _BaseDataStruct
        {
            public bool required;
            public StringBuilder paramName = new StringBuilder();

            public _DataStruct() { }

            public _DataStruct(int index,bool required)
            {
                this.index = index;
                this.required = required;
            }
        }

        #region Constructor

        public StringFormatter()
        {
            Parameters = new List<StringCommandParameter>();
        }

        public StringFormatter(string source)
            : this()
        {
            this.Source = source;
        }

        #endregion

        #region Methods

        private void PreparBuffer()
        {
            Debug.Assert(!string.IsNullOrEmpty(_source));

            var hadSpash = false;
            var hadBrace = false;
            var hadBracke = false;

            _DataStruct currentData = null;
            _BaseDataStruct currentBaseData = null;

            var index = 0;
            var hadAt = false;
            var afterAt = false;

            try
            {
                _source.ToCharArray().ToList<char>().ForEach(chr =>
                {
                    if (chr == EscapeChar)
                    {
                        hadSpash = true;
                    }
                    else
                    {
                        var speficSymbol = true;
                        if (!hadSpash)
                        {
                            switch (chr)
                            {
                                case Required_Begin:
                                    if (!(hadBrace || hadBracke))
                                    {
                                        hadBrace = true;
                                        currentData = new _DataStruct(index++, true);
                                        if (currentBaseData != null)
                                        {
                                            _baseDataStructList.Add(currentBaseData);
                                            currentBaseData = null;
                                        }
                                    }
                                    break;
                                case Optional_Begin:
                                    if (!(hadBrace || hadBracke))
                                    {
                                        hadBracke = true;
                                        currentData = new _DataStruct(index++, false);
                                        if (currentBaseData != null)
                                        {
                                            _baseDataStructList.Add(currentBaseData);
                                            currentBaseData = null;
                                        }
                                    }
                                    break;
                                case Required_End:
                                    if (hadBrace)
                                    {
                                        if (currentData != null)
                                        {
                                            if (afterAt)
                                            {
                                                currentData.buffer.Append("{0}");
                                                afterAt = false;
                                            }
                                        }

                                        hadBrace = false;
                                        _baseDataStructList.Add(currentData);
                                        currentData = null;
                                    }
                                    break;
                                case Optional_End:
                                    if (hadBracke)
                                    {
                                        if (currentData != null)
                                        {
                                            if (afterAt)
                                            {
                                                currentData.buffer.Append("{0}");
                                                afterAt = false;
                                            }
                                        }
                                        hadBracke = false;
                                        _baseDataStructList.Add(currentData);
                                        currentData = null;
                                    }
                                    break;
                                case ParamaterBeginChar:
                                    if (currentData != null)
                                    {
                                        hadAt = true;
                                    }
                                    break;
                                case ParamaterEndChar:
                                    if (currentData != null)
                                    {
                                        hadAt = false;
                                        afterAt = true;
                                    }
                                    break;
                                default:
                                    speficSymbol = false;
                                    break;
                            }
                        }
                        else
                        {
                            speficSymbol = false;
                        }

                        if (!speficSymbol)
                        {
                            var appendStr = chr.ToString();
                            if (chr == Required_Begin || chr == Required_End)
                            {
                                appendStr = string.Format("{0}{0}", appendStr);
                            }

                            if (currentData != null)
                            {
                                if (hadAt)
                                {
                                    currentData.paramName.Append(appendStr);
                                }
                                else
                                {
                                    if (afterAt)
                                    {
                                        currentData.buffer.Append("{0}");
                                        afterAt = false;
                                    }
                                    currentData.buffer.Append(appendStr);
                                }
                            }
                            else
                            {
                                if (currentBaseData == null)
                                {
                                    currentBaseData = new _BaseDataStruct(index++);
                                }
                                currentBaseData.buffer.Append(appendStr);
                            }
                        }

                        if (hadSpash)
                        {
                            hadSpash = false;
                        }

                    }
                });

                if (currentBaseData != null)
                {
                    _baseDataStructList.Add(currentBaseData);
                    currentBaseData = null;
                }
            }
            finally
            {
                currentData = null;
                currentBaseData = null;
            }
        }

        public void AddParameter(string paramName, object value)
        {
            AddParameter(paramName, value.ToString());
        }

        public void AddParameter(string paramName, string value)
        {
            AddParameter(new StringCommandParameter(paramName, value));
        }

        public void AddParameter(StringCommandParameter paramter)
        {
            if (Parameters.Count(exp => exp.ParamName.Equals(paramter.ParamName)) > 0)
            {
                throw new ItemExistException();
            }
            Parameters.Add(paramter);
        }

        public void AddParameter(IEnumerable<StringCommandParameter> paramters)
        {
            Parameters.AddRange(paramters);
        }

        public void AddParameter(params StringCommandParameter[] paramters)
        {
            Parameters.AddRange(paramters);
        }

        public void ClearParameter()
        {
            if (Parameters.Count > 0)
            {
                Parameters.Clear();
            }
        }

        private void ValidateSource()
        {
            if (string.IsNullOrEmpty(Source))
            {
                throw new FormatException("source could not be empty");
            }
        }

        public string Format()
        {
            var strBuffer = new StringBuilder();
            _DataStruct dataStructTmp = null;
            StringCommandParameter param = null;

            try
            {
                ValidateSource();

                _baseDataStructList.ForEach(dataStruct =>
                {
                    dataStructTmp = dataStruct as _DataStruct;
                    if (dataStructTmp != null)
                    {
                        param = Parameters.FirstOrDefault(exp => exp.ParamName.Equals(dataStructTmp.paramName.ToString()));
                        if (param != null)
                        {
                            strBuffer.AppendFormat(dataStructTmp.buffer.ToString(), param.Value);
                        }
                        else
                        {
                            if (dataStructTmp.required)
                            {
                                dataStructTmp.unUsed = true;
                            }
                        }
                    }
                    else
                    {
                        strBuffer.Append(dataStruct.buffer);
                    }
                });

                if (_baseDataStructList.FirstOrDefault(exp => exp.unUsed) != null)
                {
                    throw new FormatException("some param not fill");
                }

                return strBuffer.ToString();
            }
            finally
            {
                if (strBuffer != null)
                {
                    strBuffer = null;
                }

                if (dataStructTmp != null)
                {
                    dataStructTmp = null;
                }
                if (param != null)
                {
                    param = null;
                }
            }
        }

        #endregion

        public void Dispose()
        {
            Source = null;
            Parameters = null;
        }
    }
}
