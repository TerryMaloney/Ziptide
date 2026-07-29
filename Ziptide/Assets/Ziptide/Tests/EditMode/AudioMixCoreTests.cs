using NUnit.Framework;
using Ziptide.Core;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// The game shipped with no player volume control at all: a hard-coded 0.35 on the music profile
    /// and a hard-coded 0.5 on the ambience bed. For a VR game a child plays in headphones that is not
    /// a taste question, and a slider at zero has to mean SILENT — not "quiet enough that you probably
    /// won't notice". These tests pin exactly that.
    /// </summary>
    public sealed class AudioMixCoreTests
    {
        [Test]
        public void AMutedMaster_SilencesEverything_NoMatterHowLoudTheAuthorAskedFor()
        {
            Assert.AreEqual(0f, AudioMixCore.Effective(authored: 1f, busVolume: 1f, masterVolume: 0f));
        }

        [Test]
        public void AMutedBus_SilencesThatBusAlone()
        {
            Assert.AreEqual(0f, AudioMixCore.Effective(1f, 0f, 1f));
            Assert.Greater(AudioMixCore.Effective(1f, 1f, 1f), 0f);
        }

        [Test]
        public void NearZero_IsTreatedAsTrueSilence_NotAWhisper()
        {
            Assert.AreEqual(0f, AudioMixCore.Effective(1f, 1f, AudioMixCore.SilenceEpsilon));
            Assert.AreEqual(0f, AudioMixCore.Effective(AudioMixCore.SilenceEpsilon, 1f, 1f));
        }

        [Test]
        public void LevelsMultiply_SoTheAuthorsMixSurvivesThePlayersMix()
        {
            Assert.AreEqual(0.25f, AudioMixCore.Effective(0.5f, 1f, 0.5f), 0.0001f);
            Assert.AreEqual(0.125f, AudioMixCore.Effective(0.5f, 0.5f, 0.5f), 0.0001f);
        }

        [Test]
        public void OutOfRangeAndNonFiniteValues_CannotBlowThePlayersEars()
        {
            Assert.AreEqual(1f, AudioMixCore.Clamp01(4f, 0.5f));
            Assert.AreEqual(0f, AudioMixCore.Clamp01(-4f, 0.5f));
            Assert.AreEqual(0.5f, AudioMixCore.Clamp01(float.NaN, 0.5f));
            Assert.AreEqual(0.5f, AudioMixCore.Clamp01(float.PositiveInfinity, 0.5f));
            Assert.LessOrEqual(AudioMixCore.Effective(99f, 99f, 99f), 1f);
        }

        [Test]
        public void EveryBusHasADefault_AndNoneOfThemStartMuted()
        {
            foreach (AudioBus bus in System.Enum.GetValues(typeof(AudioBus)))
            {
                float value = AudioMixCore.DefaultFor(bus);
                Assert.Greater(value, 0f, bus + " must not ship muted");
                Assert.LessOrEqual(value, 1f);
            }
            Assert.Greater(AudioMixCore.DefaultMaster, 0f);
        }

        [Test]
        public void Ducking_ReturnsAMultiplier_SoABedCannotBeDuckedTwice()
        {
            Assert.AreEqual(1f, AudioMixCore.DuckMultiplier(voiceActive: false));
            Assert.Less(AudioMixCore.DuckMultiplier(voiceActive: true), 1f);
            Assert.Greater(AudioMixCore.DuckMultiplier(voiceActive: true), 0f,
                "ducking lowers a bed under a line; it does not delete it");
        }

        [Test]
        public void PrefKeys_AreStable_BecauseRenamingOneSilentlyResetsAPlayersMix()
        {
            Assert.AreEqual("ziptide.audio.music", AudioMixCore.PrefKey(AudioBus.Music));
            Assert.AreEqual("ziptide.audio.ambience", AudioMixCore.PrefKey(AudioBus.Ambience));
            Assert.AreEqual("ziptide.audio.sfx", AudioMixCore.PrefKey(AudioBus.Sfx));
            Assert.AreEqual("ziptide.audio.voice", AudioMixCore.PrefKey(AudioBus.Voice));
            Assert.AreEqual("ziptide.audio.master", AudioMixCore.MasterPrefKey);
        }

        [Test]
        public void BusOrderIsSerialized_SoItMayOnlyEverBeAppendedTo()
        {
            Assert.AreEqual(0, (int)AudioBus.Music);
            Assert.AreEqual(1, (int)AudioBus.Ambience);
            Assert.AreEqual(2, (int)AudioBus.Sfx);
            Assert.AreEqual(3, (int)AudioBus.Voice);
        }
    }
}
