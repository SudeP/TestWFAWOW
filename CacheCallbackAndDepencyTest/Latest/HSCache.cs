﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Caching;

namespace HybridServer
{
    [Serializable]
    internal class HSCache
    {
        internal HSCache(string key, object outputCacheEntry, DateTime utcExpiry, HSSettings parentSettingsJson) => Update(key, outputCacheEntry, utcExpiry, parentSettingsJson);
        internal HSCache Update(string key, object outputCacheEntry, DateTime utcExpiry, HSSettings parentHSSettings)
        {
            Key = key;
            UtcExpiry = utcExpiry;
            if (outputCacheEntry is List<ResponseElement>)
            {
                OutputCacheEntry.GetType()
                    .GetField("_responseElements", Statics.bf)
                    .SetValue(OutputCacheEntry, outputCacheEntry);
            }
            else
            {
                OutputCacheEntry = outputCacheEntry;

                Guid = (Guid)OutputCacheEntry.GetType()
                    .GetField("_cachedVaryId", Statics.bf)
                    .GetValue(OutputCacheEntry);
            }

            hSSettings = parentHSSettings;

            RootPath = IOUtility.PathCombine(hSSettings.RootPath, IOUtility.KeyCrypte(key));

            if (!Directory.Exists(RootPath))
                Directory.CreateDirectory(RootPath);

            PhysicalPath = IOUtility.PathCombine(RootPath, IOUtility.CreateFileName());

            return this;
        }
        [NonSerialized]
        private HSSettings hSSettings;
        [NonSerialized]
        internal bool IsReloding;
        internal string RootPath { get; private set; }
        internal string PhysicalPath { get; private set; }
        internal object OutputCacheEntry { get; private set; }
        internal string Key { get; private set; }
        internal DateTime UtcExpiry { get; private set; }
        internal Guid Guid { get; private set; }
    }
}