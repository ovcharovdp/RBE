using CoreAPI.Operations;
using CoreAPI.Types;
using CoreAPI.Types.ObjParam;
using CoreDM;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CoreWeb.Models.Params
{
    /// <summary>
    /// Класс для описания параметра
    /// </summary>
    public class ParamSettings : ObjParam
    {
        /// <summary>
        /// Идентификатор объекта-владельца значений
        /// </summary>
        public long ParentID { get; set; }
        /// <summary>
        /// Конструктор
        /// </summary>
        public ParamSettings() { }
        private ObjParam _param;
        /// <summary>
        /// Параметр
        /// </summary>
        public ObjParam Param
        {
            get { return _param; }
            set
            {
                _param = value;
                this.Code = value.Code;
                this.Description = value.Description;
                this.Length = value.Length;
                this.MaxCount = value.MaxCount;
                this.Name = value.Name;
                this.Type = value.Type;
                this.Precision = value.Precision;
                this.ID = value.ID;
            }
        }
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="value">Значение параметра</param>
        public ParamSettings(ObjParamValue value)
        {
            _values = new List<ObjParamValue>();
            _values.Add(value);
        }
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="param">Параметр</param>
        public ParamSettings(ObjParam param)
        {
            this.Param = param;
        }

        private List<ObjParamValue> _values;
        private List<ParamItemValue> _items;
        /// <summary>
        /// Возвращает значения параметра
        /// </summary>
        public new List<ObjParamValue> Values
        {
            get
            {
                if (_values == null)
                {
                    if (_param == null)
                    {
                        _values = new List<ObjParamValue>();
                    }
                    else
                    {
                        _values = _param.Values.Where(p => p.ParentID == ParentID).ToList();
                    }
                }
                if (_values.Count == 0 && "SET".Equals(this.Type))
                {
                    ObjParamValue v = new ObjParamValue() { ParentID = this.ParentID, ParamID = this.ID, Order = 0 };
                    ObjParamValueOperations.New(DependencyResolver.Current.GetService<ICoreDBContext>().CoreEntities, v);
                    _values.Add(v);
                }
                return _values;
            }
            set
            {
                _values = value;
            }
        }
        /// <summary>
        /// Возвращает список возможных значений параметра
        /// </summary>
        public new List<ParamItemValue> Items
        {
            get
            {
                if (_items == null)
                {
                    if (_param == null)
                    {
                        _items = new List<ParamItemValue>();
                    }
                    else
                    {
                        _items = _param.Items.OrderBy(p => p.Order).Select(p => new ParamItemValue() { Label = p.Label, Name = p.Name }).ToList();
                    }
                }
                return _items;
            }
            set
            {
                _items = value;
            }
        }
        /// <summary>
        /// Возвращает следующий порядковый номер для значения (используется при добавлении значения)
        /// </summary>
        public int NextOrder
        {
            get
            {
                if (Values.Count == 0)
                {
                    return 1;
                }
                return Values.Max(p => p.Order) + 1;
            }
        }
    }
}
