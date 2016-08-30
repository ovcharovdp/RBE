using System;

namespace CoreAPI.Types
{
    /// <summary>
    /// Класс по управлению ошибками
    /// </summary>
    public class ErrorManager
    {
        /// <summary>
        /// Формирует строку с полным описанием ошибки
        /// </summary>
        /// <param name="e">Исключение</param>
        /// <returns>Полное описание ошибки</returns>
        public static string GetFullMessage(Exception e)
        {
            Exception _exception = e;
            string _message = "";
            while (_exception != null)
            {
                if (string.IsNullOrEmpty(_message))
                {
                    _message = string.Concat(_message, _exception.Message);
                }
                else
                {
                    _message = string.Concat(_message, "\n", _exception.Message);
                }
                _exception = _exception.InnerException;
            }
            return _message;
        }
    }
}