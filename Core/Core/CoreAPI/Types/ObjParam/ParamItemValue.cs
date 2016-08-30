namespace CoreAPI.Types.ObjParam
{
    /// <summary>
    /// Класс, описывающий доступное значение для параметра
    /// </summary>
    public class ParamItemValue
    {
        /// <summary>
        /// Значение
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Надпись, отображаемая в элементе управления
        /// </summary>
        public string Label { get; set; }
    }
}