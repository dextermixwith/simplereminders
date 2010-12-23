namespace simplereminders.web.models.facades
{
    public abstract class GoogleFacadeBase
    {
        protected string _googleUsername;
        protected string _googlePassword;
        protected string _applicationName = "EsendexSimpleReminders";

        protected GoogleFacadeBase(string googleUsername, string googlePassword)
        {
            _googleUsername = googleUsername;
            _googlePassword = googlePassword;
        }
    }
}