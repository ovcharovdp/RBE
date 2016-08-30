namespace BaseEntities
{
    ///<summary>
    ///Словарь значений
    ///</summary>
    public class SysDictionary : BaseEntity
    {
        ///<summary>
        ///Наименование
        ///</summary>
        public string Name { get; set; }
        ///<summary>
        ///Код
        ///</summary>
        public string Code { get; set; }
        ///<summary>
        ///Описание
        ///</summary>
        public string Description { get; set; }
    }
}
