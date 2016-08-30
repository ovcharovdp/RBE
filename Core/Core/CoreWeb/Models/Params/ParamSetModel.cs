using CoreDM;
using System.Collections.Generic;
using System.Linq;

namespace CoreWeb.Models.Params
{
    /// <summary>
    /// Модель настроек параметров
    /// </summary>
    public class ParamSetModel
    {
        /// <summary>
        /// Источник данных
        /// </summary>
        private ObjParamValue _v;
        /// <summary>
        /// Инициализация настройки параметра
        /// </summary>
        /// <param name="value">Значение набора параметров</param>
        public ParamSetModel(ObjParamValue value)
        {
            _v = value;
        }
        /// <summary>
        /// Список настроек параметра
        /// </summary>
        public IEnumerable<ParamSettings> Items
        {
            get
            {
                return _v.Param.SetItems.Select(p => new ParamSettings() { Param = p.Param, ParentID = _v.ID });
            }
        }
    }
}