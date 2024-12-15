namespace UserTaskAPI.Helpers
{
    public static class StatusValidator
    {
        private static readonly string[] ValidStatuses = { "Pending", "Completed" };

        public static bool IsValidStatus(string status)
        {
            return !string.IsNullOrEmpty(status) && ValidStatuses.Contains(status, StringComparer.OrdinalIgnoreCase);
        }
    }
}
