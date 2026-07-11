#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Ziptide.Gameplay;

namespace Ziptide.Tests.EditMode
{
    public class ZiplineSignalTests
    {
        private readonly List<GameObject> _objects = new List<GameObject>();

        [TearDown]
        public void TearDown()
        {
            for (int i = 0; i < _objects.Count; i++)
                if (_objects[i] != null)
                    UnityEngine.Object.DestroyImmediate(_objects[i]);
            _objects.Clear();
        }

        [Test]
        public void BeginRide_PublishesOnceWhileRideIsActive_AndMissingRigIsSafe()
        {
            ZiplineRuntime line = CreateLine("line");
            int starts = 0;
            line.RideStarted += () => starts++;

            Assert.DoesNotThrow(() => Invoke(line, "BeginRide"));
            Assert.DoesNotThrow(() => Invoke(line, "BeginRide"));

            Assert.AreEqual(1, starts);
            Assert.IsNotNull(GetRide(line));

            Invoke(line, "EndRide", "released");
        }

        [Test]
        public void ArrivedEnd_PublishesReasonAndProgress_AfterRideIsCleared()
        {
            ZiplineRuntime line = CreateLine("designated");
            int ends = 0;
            string seenReason = null;
            float seenProgress = -1f;
            bool rideWasCleared = false;
            line.RideEnded += (reason, progress) =>
            {
                ends++;
                seenReason = reason;
                seenProgress = progress;
                rideWasCleared = GetRide(line) == null;
            };

            Invoke(line, "BeginRide");
            Invoke(line, "EndRide", "arrived");

            Assert.AreEqual(1, ends);
            Assert.AreEqual("arrived", seenReason);
            Assert.GreaterOrEqual(seenProgress, 0f);
            Assert.LessOrEqual(seenProgress, 1f);
            Assert.IsTrue(rideWasCleared);
            Assert.IsTrue(ZiplineRuntime.IsDesignatedArrival(line, line, seenReason));
        }

        [Test]
        public void ReleaseOrWrongLine_DoesNotSatisfyDesignatedArrival()
        {
            ZiplineRuntime designated = CreateLine("designated");
            ZiplineRuntime other = CreateLine("other");

            Assert.IsFalse(ZiplineRuntime.IsDesignatedArrival(designated, designated, "released"));
            Assert.IsFalse(ZiplineRuntime.IsDesignatedArrival(other, designated, "arrived"));
            Assert.IsFalse(ZiplineRuntime.IsDesignatedArrival(designated, null, "arrived"));
            Assert.IsFalse(ZiplineRuntime.IsDesignatedArrival(null, designated, "arrived"));
            Assert.IsFalse(ZiplineRuntime.IsDesignatedArrival(designated, designated, "Arrived"));
            Assert.IsTrue(ZiplineRuntime.IsDesignatedArrival(designated, designated, "arrived"));
        }

        [Test]
        public void ThrowingSubscribers_DoNotBlockLaterSubscribersOrRideCleanup()
        {
            ZiplineRuntime line = CreateLine("line");
            int laterStarts = 0;
            int laterEnds = 0;

            line.RideStarted += () => throw new InvalidOperationException("start expected");
            line.RideStarted += () => laterStarts++;
            line.RideEnded += (reason, progress) => throw new InvalidOperationException("end expected");
            line.RideEnded += (reason, progress) => laterEnds++;

            LogAssert.Expect(
                LogType.Warning,
                new Regex("ZIPTIDE: ZIPLINE_SUBSCRIBER_FAIL phase=start reason=start expected"));
            Invoke(line, "BeginRide");
            Assert.AreEqual(1, laterStarts);
            Assert.IsNotNull(GetRide(line));

            LogAssert.Expect(
                LogType.Warning,
                new Regex("ZIPTIDE: ZIPLINE_SUBSCRIBER_FAIL phase=end reason=end expected"));
            Invoke(line, "EndRide", "released");
            Assert.AreEqual(1, laterEnds);
            Assert.IsNull(GetRide(line));
        }

        [Test]
        public void RuntimeSource_PreservesRideMathRigDeltaReleaseAndArrivalPaths()
        {
            string path = Path.Combine(
                Application.dataPath,
                "Ziptide",
                "Gameplay",
                "Runtime",
                "World",
                "ZiplineRuntime.cs");
            Assert.IsTrue(File.Exists(path), path);

            string source = File.ReadAllText(path);

            StringAssert.Contains("public event System.Action RideStarted;", source);
            StringAssert.Contains("public event System.Action<string, float> RideEnded;", source);
            StringAssert.Contains("public static bool IsDesignatedArrival(", source);
            StringAssert.Contains("object.ReferenceEquals(reportedLine, designatedLine)", source);
            StringAssert.Contains("string.Equals(reason, \"arrived\", System.StringComparison.Ordinal)", source);

            Assert.AreEqual(1, Count(source, "_ride.Step(Time.deltaTime);"));
            Assert.AreEqual(1, Count(source, "if (_rig != null) _rig.position += delta;"));
            Assert.AreEqual(1, Count(source, "grab.selectExited.AddListener(_ => EndRide(\"released\"));"));
            Assert.AreEqual(1, Count(source, "if (_ride.Arrived) EndRide(\"arrived\");"));
            Assert.AreEqual(1, Count(source, "_idleT = Mathf.Max(0f, _idleT - Time.deltaTime * 0.25f);"));

            int startLog = source.IndexOf("ZIPTIDE: ZIPLINE_RIDE_START", StringComparison.Ordinal);
            int startPublish = source.IndexOf("PublishRideStarted();", startLog, StringComparison.Ordinal);
            Assert.Greater(startPublish, startLog);
            Assert.AreEqual(1, Count(source, "PublishRideStarted();"));

            int endLog = source.IndexOf("ZIPTIDE: ZIPLINE_RIDE_END reason=", StringComparison.Ordinal);
            int rideClear = source.IndexOf("_ride = null;", endLog, StringComparison.Ordinal);
            int endPublish = source.IndexOf("PublishRideEnded(reason, progress);", rideClear, StringComparison.Ordinal);
            Assert.Greater(rideClear, endLog);
            Assert.Greater(endPublish, rideClear);
            Assert.AreEqual(1, Count(source, "PublishRideEnded(reason, progress);"));

            StringAssert.DoesNotContain("_rig.SetParent", source);
            StringAssert.DoesNotContain("_rig.parent", source);
            StringAssert.DoesNotContain("SetParent(_rig", source);
            StringAssert.DoesNotContain("PlayerProfile", source);
            StringAssert.DoesNotContain("SaveSystem", source);
            StringAssert.DoesNotContain("FirstHour", source);
            StringAssert.DoesNotContain("Tutorial", source);
        }

        private ZiplineRuntime CreateLine(string name)
        {
            var go = new GameObject(name);
            _objects.Add(go);
            var line = go.AddComponent<ZiplineRuntime>();
            line.Init(new Vector3(0f, 3f, 0f), new Vector3(8f, 0f, 0f));
            return line;
        }

        private static object GetRide(ZiplineRuntime line)
        {
            FieldInfo field = typeof(ZiplineRuntime).GetField(
                "_ride",
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.IsNotNull(field);
            return field.GetValue(line);
        }

        private static void Invoke(ZiplineRuntime line, string methodName, params object[] args)
        {
            MethodInfo method = typeof(ZiplineRuntime).GetMethod(
                methodName,
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.IsNotNull(method, methodName);
            try
            {
                method.Invoke(line, args);
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException ?? ex;
            }
        }

        private static int Count(string source, string token)
        {
            int count = 0;
            int index = 0;
            while ((index = source.IndexOf(token, index, StringComparison.Ordinal)) >= 0)
            {
                count++;
                index += token.Length;
            }
            return count;
        }
    }
}
#endif
