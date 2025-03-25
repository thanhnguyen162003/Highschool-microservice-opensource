namespace TransactionalService.Application
{
    public class SystemAppService : IDynamicApiController
    {
        private readonly ISystemService _systemService;
        public SystemAppService(ISystemService systemService)
        {
            _systemService = systemService;
        }

        public string GetDescription()
        {
            return _systemService.GetDescription();
        }
    }
}
