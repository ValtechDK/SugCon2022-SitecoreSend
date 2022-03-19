using Sitecore;
using Sitecore.Caching;
using Sitecore.Collections;
using Sitecore.Data;
using Sitecore.Data.DataProviders;
using Sitecore.Data.Items;
using Sitecore.Globalization;
using SugCon.SitecoreSend.Models;
using SugCon.SitecoreSend.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SugCon.SitecoreSend.Providers
{
    public class MooSendListsProvider : DataProvider
    {
        private readonly ID _contentRootItemId;
        private readonly ID _contentItemTemplateId;
        private readonly Lazy<ICache> _cache;
        private readonly ISendService _service;

        private static readonly TimeSpan CacheTime = TimeSpan.FromMinutes(5);

        public MooSendListsProvider(string contentRootItemId, string contentItemTemplateId) : this(
            contentRootItemId, contentRootItemId,
            new SitecoreSendService(Sitecore.Configuration.Settings.GetSetting("SitecoreSendApiKey"))
            ) { }

        private MooSendListsProvider(string contentRootItemId, string contentItemTemplateId, ISendService sendService)
        {
            if (!ID.TryParse(contentRootItemId, out _contentRootItemId))
                throw new InvalidOperationException($"Invalid root template ID {contentRootItemId}");

            if (!ID.TryParse(contentItemTemplateId, out _contentItemTemplateId))
                throw new InvalidOperationException($"Invalid item template ID {contentItemTemplateId}");

            _cache = new Lazy<ICache>(GetCache);
            _service = sendService;
        }

        private static ICache GetCache()
        {
            return CacheManager.GetNamedInstance(nameof(MooSendListsProvider), StringUtil.ParseSizeString("5MB"), true);
        }

        private Dictionary<Guid, MooSendList> GetModels()
        {
            var key = "sitecoresend-lists";
            if (_cache.Value.GetValue(key) is Dictionary<Guid, MooSendList> result1) return result1;

            lock (this)
            {
                if (_cache.Value.GetValue(key) is Dictionary<Guid, MooSendList> result) return result;

                try
                {
                    var lists = _service.GetLists().ConfigureAwait(true).GetAwaiter().GetResult();
                    result = lists.ToDictionary(x => Guid.Parse(x.ID), x => x);
                }
                catch (Exception exc)
                {
                    Sitecore.Diagnostics.Log.Fatal("Could not fetch lists: " + exc.Message, exc, this);
                    result = new Dictionary<Guid, MooSendList>(0);
                }

                _cache.Value.Add(key, result, DateTime.Now.Add(CacheTime));
                return result;
            }
        }

        private MooSendList GetModel(Guid id)
        {
            var list = GetModels();
            return list.TryGetValue(id, out var value) ? value : null;
        }

        private MooSendList GetModel(ItemDefinition def)
        {
            return GetModel(def.ID.Guid);
        }

        private bool IsOurModel(ItemDefinition def)
        {
            return GetModel(def) != null;
        }

        public override ItemDefinition GetItemDefinition(ID itemId, CallContext context)
        {
            var model = GetModel(itemId.Guid);
            if (model == null) return null;
            return new ItemDefinition(
                itemId,
                model.Name,
                _contentItemTemplateId,
                ID.Null);
        }

        public override IDList GetChildIDs(ItemDefinition itemDefinition, CallContext context)
        {
            if (itemDefinition.ID == _contentRootItemId)
            {
                var ids = GetModels().Keys.Select(x => new ID(x)).ToList();
                var result = new IDList(ids.Count);
                ids.ForEach(x => result.Add(x));
                return result;
            }

            if(IsOurModel(itemDefinition))
            {
                return new IDList(0);
            }

            return null;
        }

        public override ID GetParentID(ItemDefinition itemDefinition, CallContext context)
        {
            if (IsOurModel(itemDefinition))
            {
                context.Abort();
                return _contentRootItemId;
            }

            return base.GetParentID(itemDefinition, context);
        }

        public override FieldList GetItemFields(ItemDefinition itemDefinition, VersionUri versionUri, CallContext context)
        {
            var model = GetModel(itemDefinition);
            if(model == null)
            {
                return null;
            }

            var fields = base.GetItemFields(itemDefinition, versionUri, context) ?? context.CurrentResult as FieldList ?? new FieldList();
            fields.Add(FieldIDs.NeverPublish, "1");
            return fields;
        }

        public override VersionUriList GetItemVersions(ItemDefinition itemDefinition, CallContext context)
        {
            if (!IsOurModel(itemDefinition))
            {
                return base.GetItemVersions(itemDefinition, context);
            }

            context.Abort();
            var versions = new VersionUriList
            {
                { Language.Parse("en"), Sitecore.Data.Version.First }
            };

            return versions;
        }

        #region Write methods that just needs to be there, but never does anything
        public override bool CreateItem(ID itemId, string itemName, ID templateId, ItemDefinition parent, CallContext context)
        {
            return false;
        }
        public override bool SaveItem(ItemDefinition itemDefinition, ItemChanges changes, CallContext context)
        {
            return false;
        }
        public override bool DeleteItem(ItemDefinition itemDefinition, CallContext context)
        {
            return false;
        }
        public override LanguageCollection GetLanguages(CallContext context)
        {
            return null;
        }
        #endregion
    }
}