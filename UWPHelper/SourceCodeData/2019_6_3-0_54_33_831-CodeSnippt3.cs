//第一次读取的集合
        private ObservableCollection<Process> _firstCollection;

        //第二次读取的集合
        private ObservableCollection<Process> _secondCollection;

        //AddProgress页绑定的进程列
        public ObservableCollection<Process> addProgressCollection
        {
            get;
            private set;
        }

        //viewProgress页绑定的进程列
        public ObservableCollection<MyDatabaseContext.BlackListProgress> viewProgressCollection
        {
            get;
            private set;
        }

        //本地黑名单服务
        private ILocalBlackListProgressService _localBlackListProgressService;

        //服务器预设黑名单服务
        private readonly IWebBlackListProgressService _webBlackListProgressService;

        //进程服务
        private IProcessService _processService;

        //身份识别服务
        private readonly IIdentityService _identityService;