using System;
using System.Collections.Generic;
using UnityEngine;

public enum QuestStage { NotStarted = 0, NeedsItem = 1, Completed = 2 }

public static class QuestStateStore
{
    private const string SaveKey = "QSAVE_v1";
    private static bool _loaded = false;

    // id -> stage
    private static Dictionary<string, QuestStage> _map = new();

    // ---- API pública ----
    public static QuestStage Get(string questId)
    {
        EnsureLoaded();
        if (string.IsNullOrEmpty(questId)) return QuestStage.NotStarted;
        return _map.TryGetValue(questId, out var s) ? s : QuestStage.NotStarted;
    }

    public static void Set(string questId, QuestStage stage, bool autosave = true)
    {
        EnsureLoaded();
        if (string.IsNullOrEmpty(questId)) return;
        _map[questId] = stage;
        if (autosave) Save();
    }

    public static void Save()
    {
        var wrap = Wrapper.FromDict(_map);
        PlayerPrefs.SetString(SaveKey, JsonUtility.ToJson(wrap));
        PlayerPrefs.Save();
    }

    public static void Load()
    {
        _map.Clear();
        if (PlayerPrefs.HasKey(SaveKey))
        {
            var json = PlayerPrefs.GetString(SaveKey);
            var wrap = JsonUtility.FromJson<Wrapper>(json);
            _map = wrap != null ? wrap.ToDict() : new Dictionary<string, QuestStage>();
        }
        _loaded = true;
    }

    public static void ClearAll()
    {
        PlayerPrefs.DeleteKey(SaveKey);
        _map.Clear();
        _loaded = true;
    }

    // ---- Internals ----
    private static void EnsureLoaded()
    {
        if (!_loaded) Load();
    }

    [Serializable]
    private class Entry { public string id; public int s; }

    [Serializable]
    private class Wrapper
    {
        public List<Entry> list = new();
        public static Wrapper FromDict(Dictionary<string, QuestStage> d)
        {
            var w = new Wrapper();
            foreach (var kv in d)
                w.list.Add(new Entry { id = kv.Key, s = (int)kv.Value });
            return w;
        }
        public Dictionary<string, QuestStage> ToDict()
        {
            var d = new Dictionary<string, QuestStage>();
            if (list != null)
                foreach (var e in list) d[e.id] = (QuestStage)e.s;
            return d;
        }
    }
}
