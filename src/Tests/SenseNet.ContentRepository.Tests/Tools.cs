﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SenseNet.ContentRepository.Storage;
using SenseNet.ContentRepository.Storage.Search;

namespace SenseNet.SearchImpl.Tests
{
    public class Tools
    {
        public static IDisposable Swindle(Type @class, string memberName, object cheat)
        {
            return new Swindler(@class, memberName, cheat);
        }

        private class Swindler : IDisposable
        {
            private PrivateType _accessor;
            private string _memberName;
            private object _originalValue;

            public Swindler(Type @class, string memberName, object cheat)
            {
                _accessor = new PrivateType(@class);
                _memberName = memberName;
                _originalValue = _accessor.GetStaticFieldOrProperty(memberName);
                _accessor.SetStaticFieldOrProperty(_memberName, cheat);
            }
            public void Dispose()
            {
                _accessor.SetStaticFieldOrProperty(_memberName, _originalValue);
            }
        }

        public class SearchEngineSwindler : IDisposable
        {
            private readonly PrivateObject _accessor;
            private string _memberName = "_searchEngine";
            private readonly object _originalSearchEngine;

            public SearchEngineSwindler(ISearchEngine searchEngine)
            {
                var storageContextAcc = new PrivateType(typeof(StorageContext));
                var storageContextInstance = storageContextAcc.GetStaticFieldOrProperty("Instance");
                _accessor = new PrivateObject(storageContextInstance);
                _originalSearchEngine = _accessor.GetField(_memberName);
                _accessor.SetField(_memberName, searchEngine);
            }
            public void Dispose()
            {
                _accessor.SetField(_memberName, _originalSearchEngine);
            }
        }

        public class RepositorySupportSwindler : IDisposable
        {
            private readonly ISearchEngineSupport _savedSearchEngineSupport;
            public RepositorySupportSwindler(ISearchEngineSupport cheat)
            {
                _savedSearchEngineSupport = StorageContext.Search.ContentRepository;
                StorageContext.Search.ContentRepository = cheat;
            }
            public void Dispose()
            {
                StorageContext.Search.ContentRepository = _savedSearchEngineSupport;
            }
        }
    }
}
