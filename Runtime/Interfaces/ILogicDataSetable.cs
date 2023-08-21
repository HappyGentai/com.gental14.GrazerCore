namespace GrazerCore.Interfaces
{
    /// <summary>
    /// Used to get target class data or set data let target class deal it.
    /// Example:
    /// Player or enemy logic data get and set.
    /// </summary>
    public interface ILogicDataSetable
    {
        /// <summary>
        /// Get target class logic data, foramt usually are json.
        /// </summary>
        /// <returns></returns>
        string GetLogicData();

        /// <summary>
        /// Set rawData then target class use to put value or do something.
        /// Foramt usually are json.
        /// </summary>
        void SetLogicData(string rawData);
    }
}
