using System;
using System.Collections.Generic;
using UnityEngine;
using Ziptide.Core;

namespace Ziptide.Content
{
    /// <summary>How a RILL line is triggered.</summary>
    public enum RillTrigger
    {
        WorldEnter, // key = scene name; fires when that world loads
        FlagSet     // key = a ZiptideFlags name; fires when the profile gains that flag
    }

    /// <summary>One deliverable RILL line (subtitle now; a VO clip slots in at the art/audio pass).</summary>
    [Serializable]
    public class RillLine
    {
        [Tooltip("Stable id — used for the once-per-save latch (RILL_SAID_<id>) and the RILL_LINE log.")]
        public string id;

        public RillTrigger trigger = RillTrigger.FlagSet;

        [Tooltip("WorldEnter: the scene name. FlagSet: the flag name (use ZiptideFlags constants).")]
        public string key;

        [Tooltip("The subtitle text RILL speaks.")]
        [TextArea(1, 3)] public string text;

        [Tooltip("Say it once per save (latched via a RILL_SAID_<id> profile flag). Off = every time.")]
        public bool once = true;

        [Tooltip("Optional VO clip — empty until the M6 audio pass; subtitles carry the line until then.")]
        public AudioClip voClip;
    }

    /// <summary>
    /// The authored set of RILL's lines (GAME_PLAN M1). Data asset at Resources/Story/RillLines so the
    /// runtime companion can load it anywhere; AUTHORED BY RillLineAuthor (code is the source of truth
    /// for the arc beats — the story-change pipeline is BIBLE → WORLD_DATA → RillLineAuthor → build).
    /// </summary>
    public class RillLineLibrary : ScriptableObject
    {
        public List<RillLine> lines = new List<RillLine>();

        /// <summary>All lines matching a trigger+key (cheap linear scan — the library is small).</summary>
        public void Collect(RillTrigger trigger, string key, List<RillLine> into)
        {
            if (into == null || string.IsNullOrEmpty(key)) return;
            for (int i = 0; i < lines.Count; i++)
            {
                var l = lines[i];
                if (l != null && l.trigger == trigger && l.key == key) into.Add(l);
            }
        }
    }
}
