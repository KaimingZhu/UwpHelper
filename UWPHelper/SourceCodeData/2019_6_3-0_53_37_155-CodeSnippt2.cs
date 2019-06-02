public AddProgressViewModel(IProcessService processService, ILocalBlackListProgressService localBlackListProgressService,
            IWebBlackListProgressService webBlackListProgress, IIdentityService identityService, IRootNavigationService iNavigationService)
        {
            _secondCollection = new ObservableCollection<Process>();
            _firstCollection = new ObservableCollection<Process>();
            addProgressCollection = new ObservableCollection<Process>();
            viewProgressCollection = new ObservableCollection<MyDatabaseContext.BlackListProgress>();
            _localBlackListProgressService = localBlackListProgressService;
            _processService = processService;
            _webBlackListProgressService = webBlackListProgress;
            _identityService = identityService;
            _rootNavigationService = iNavigationService;

            Messenger.Default.Register<AddProgressMessage>(this, async (message) =>
            {
                //选择添加
                if (message.ifSelectToAdd == true)
                {
                    switch (message.choice)
                    {
                        case 1:
                            {
                                //清空数组
                                _firstCollection.Clear();
                                _secondCollection.Clear();

                                //第一次读取
                                _firstCollection = _processService.GetProcessNow();

                                break;
                            }
                        case 2:
                            {
                                //第二次读取
                                _secondCollection = _processService.GetProcessNow();

                                //获得不同的进程
                                var temp = _processService.GetProcessDifferent(_firstCollection, _secondCollection);

                                //清空数组，加入
                                addProgressCollection.Clear();
                                if (temp != null)
                                {
                                    foreach (var template in temp)
                                    {
                                        addProgressCollection.Add(template);
                                    }
                                }
                                break;
                            }
                        case 3:
                            {

                                if (message.parameter == null)
                                {
                                    await new MessageDialog("添加失败").ShowAsync();
                                }

                                //传入参数，写入数据库
                                //UWP进程
                                Motherlibrary.MyDatabaseContext.BlackListProgress temp;
                                temp = _localBlackListProgressService.GetBlackListProgress(message.parameter.ID, message.parameter.FileName, message.newName, message.parameter.Type);

                                bool judge = await _localBlackListProgressService.AddBlackListProgressAsync(temp);

                                _rootNavigationService.Navigate((typeof(ViewProgress)));

                                break;
                            }
                    }
                }
                else
                {
                    //删除、更新、刷新
                    switch (message.choice)
                    {

                        case 1:
                            {
                                Task t1 = Task.Factory.StartNew(delegate
                                {
                                    _localBlackListProgressService.DeleteBlackListProgressAsync(message.deleteList);
                                    RefreshTheCollection();
                                });
                                break;
                            }

                        case 2:
                            {
                                //更新
                                await RefreshTheCollection();
                                break;
                            }

                        case 3:
                            {
                                //刷新
                                await RefreshTheCollection();
                                break;
                            }
                    }
                }
            });
        }
    }