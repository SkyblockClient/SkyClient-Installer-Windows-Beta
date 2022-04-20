using Newtonsoft.Json;
using Skyclient.Utilities;
using System.ComponentModel;

namespace Skyclient.JsonParts
{
    public abstract class RepoItem
    {
        public abstract string LocalFolderName { get; }
        public abstract string RepoFolderName { get; }
        public string DownloadLink
        {
            get
            {
                if (IsSetUrl())
                    return Url;
                return RepoUtils.GetQualifiedCdn(RepoFolderName + "/" + File);
            }
        }

        #region Non-Nullable 
        const string NON_NULLABLE = "NON-NULLABLE";

        [JsonProperty("id")]
        public string Id { get; set; } = NON_NULLABLE;

        [JsonProperty("file")]
        public string File { get; set; } = NON_NULLABLE;

        [JsonProperty("display")]
        public string Display { get; set; } = NON_NULLABLE;

        [JsonProperty("description")]
        public string Description { get; set; } = NON_NULLABLE;
        #endregion

        #region Booleans
        [JsonProperty("enabled"), DefaultValue(false)]
        public bool Enabled { get; set; }

        [JsonProperty("hidden"), DefaultValue(false)]
        public bool Hidden { get; set; }

        [JsonProperty("actions")]
        public RepoItemAction[] Actions { get; set; } = System.Array.Empty<RepoItemAction>();

        [JsonProperty("packages"), DefaultValue(new string[0])]
        public string[] Packages { get; set; } = new string[0];

        [JsonProperty("categories"), DefaultValue(new string[0])]
        public string[] Categiories { get; set; } = new string[0];

        #endregion

        #region Nullable
        public string[] LocalFiles { get; set; }

        [JsonProperty("hash")]
        public string Hash { get; set; }

        [JsonProperty("icon")]
        public string IconName { get; set; }

        [JsonProperty("url")]
        public string? Url { get; set; }

        [JsonProperty("creator")]
        public string? Creator { get; set; }
        #endregion

        #region Nullable IsSet
        public bool IsSetLocalFiles()
        {
            return GenericIsSet(LocalFiles);
        }
        public bool IsSetHash()
        {
            return GenericIsSet(Hash);
            //return !(Hash == null || Hash == "" || Hash == NON_NULLABLE);
        }

        public bool IsSetUrl()
        {
            return GenericIsSet(Url);
            //return !(Url == null || Url == "" || Url == NON_NULLABLE);
        }
        public bool IsSetIconName()
        {
            return GenericIsSet(IconName);
            //return !(IconName == null || IconName == "" || IconName == NON_NULLABLE);
        }

        public bool IsSetCreator()
        {
            return GenericIsSet(Creator);
            //return !(IconName == null || IconName == "" || IconName == NON_NULLABLE);
        }

        public bool GenericIsSet(string[] value)
        {
            return !(value == null);
        }

        public bool GenericIsSet(string value)
        {
            return !(value == null || value == "" || value == NON_NULLABLE);
        }
        #endregion

        public void SetEnabledStatus(bool status)
        {
            bool changed = status != Enabled;
            this.Enabled = status;
            // only fire the event when something actually changed
            if (changed)
            {
                EventUtils.RaiseRepoItemChangeSelectedStateEvent(this);
            }
        }
    }
}