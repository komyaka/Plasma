// ============================================================================
// Constants.cs — Файл констант (УСТАРЕВШИЙ, сохранён для обратной совместимости)
// ============================================================================
// ВСЕ настройки теперь хранятся в AppConfig.cs.
// Этот файл оставлен как обёртка, чтобы не менять все ссылки сразу.
// При рефакторинге замените все обращения к Constants на AppConfig.
// ============================================================================

namespace Plazma.Controllers
{
    /// <summary>
    /// Устаревший файл констант. Перенаправляет на AppConfig.
    /// </summary>
    class Constants
    {
        /// <summary>Строка подключения к БД — берётся из AppConfig</summary>
        public const string bdconnectionstring = AppConfig.ConnectionString;

        /// <summary>Корневой путь к CNC-файлам — берётся из AppConfig</summary>
        public const string CNCPath = AppConfig.CNCPath;

        /// <summary>Путь к сетевой папке с расчётами — берётся из AppConfig</summary>
        public const string _plasmaPath = AppConfig.CalculationsPath;

        Constants() { }
    }
}
