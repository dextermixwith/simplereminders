using System.Data.Entity.Database;

namespace simplereminders.web.models.database
{
    public class AppointmentsDbInitialiser : DropCreateDatabaseIfModelChanges<AppointmentsDb>
    {
    }
}