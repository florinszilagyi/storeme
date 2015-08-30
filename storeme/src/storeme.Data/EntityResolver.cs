namespace storeme.Data
{
    public class EntityResolver
    {
        public static IDashboardRepository ResolveRepository()
        {
            return new SqlDashboardRepository();
        }
    }
}
