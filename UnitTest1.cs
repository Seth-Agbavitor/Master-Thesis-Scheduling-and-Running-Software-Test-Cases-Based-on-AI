using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;

namespace SimPaintRobot
{
    [TestClass]
    public class PaintRobotTests
    {
        private PaintRobot robot;
        private static readonly Random delayRandom = new Random();

        [TestInitialize]
        public void Init()
        {
            robot = new PaintRobot(0.5); // Default failures chance
        }

        private void RandomDelay()
        {
            int delay = delayRandom.Next(100, 2000);
            Thread.Sleep(delay);
        }

        private void Delay30Seconds() => Thread.Sleep(30000);
        private void Delay60Seconds() => Thread.Sleep(60000);
        private void Delay120Seconds() => Thread.Sleep(120000);

        [TestMethod] public void PaintABB5400_MotionRangeCheck() { Delay60Seconds(); Assert.IsTrue(robot.MoveTo(600, 300, 200, 0.2)); }
        [TestMethod] public void PaintIRB5500_ResetAfterFailure() { RandomDelay(); robot.InjectRandomFailure(); robot.Reset(); Assert.IsTrue(robot.MoveTo(200, 200, 200, 0.3)); }
        [TestMethod] public void PaintIRB5400_SequenceError() { Delay30Seconds(); robot.MoveTo(300, 300, 300, 0.4); robot.InjectRandomFailure(); Assert.IsFalse(robot.StartPainting(0.4)); }
        [TestMethod] public void PaintIRB580_TriggerFailsafe() { RandomDelay(); robot.MoveTo(400, 200, 100, 0.5); robot.InjectRandomFailure(); Assert.IsFalse(robot.MoveTo(100, 100, 100, 0.5)); }
        [TestMethod] public void ABBPaintingBot_RecoveryFromCrash() { Delay30Seconds(); robot.InjectRandomFailure(); robot.Reset(); Assert.IsTrue(robot.StartPainting(0.3)); }
        [TestMethod] public void SprayRobot_ResetWorks() { RandomDelay(); robot.InjectRandomFailure(); robot.Reset(); Assert.AreEqual((0, 0, 0), robot.CurrentPosition); }
        [TestMethod] public void RoboticPainter_FailsafeBlocksPainting() { RandomDelay(); robot.InjectRandomFailure(); Assert.IsFalse(robot.StartPainting(0.4)); }
        [TestMethod] public void ABBPainter_PaintStopsIfFailsafe() { Delay60Seconds(); robot.MoveTo(300, 200, 100, 0.3); robot.StartPainting(0.3); robot.InjectRandomFailure(); Assert.IsFalse(robot.StopPainting()); }
        [TestMethod] public void PaintLineIRB_ValidPositioning() { RandomDelay(); Assert.IsTrue(robot.MoveTo(100, 500, 100, 0.1)); }
        [TestMethod] public void SmartPaintBot_PositionLimitFail() { RandomDelay(); Assert.IsFalse(robot.MoveTo(1200, 100, 100, 0.2)); }

        [TestMethod] public void FlexPainter_RecoveryTest() { Delay30Seconds(); robot.InjectRandomFailure(); robot.Reset(); Assert.IsTrue(robot.MoveTo(200, 300, 400, 0.2)); }
        [TestMethod] public void ABB5000_PaintCycle() { Delay60Seconds(); robot.MoveTo(250, 250, 250, 0.1); if (robot.StartPainting(0.2)) Assert.IsTrue(robot.StopPainting()); }
        [TestMethod] public void HighPrecisionArm_BoundaryFailure() { RandomDelay(); Assert.IsFalse(robot.MoveTo(-1, 0, 0, 0.3)); }
        [TestMethod] public void PaintIRBControl_FailureSimulation() { RandomDelay(); robot.InjectRandomFailure(); Assert.IsFalse(robot.MoveTo(500, 500, 500, 0.6)); }
        [TestMethod] public void ABB5500_ResetAndOperate() { RandomDelay(); robot.InjectRandomFailure(); robot.Reset(); Assert.IsTrue(robot.StartPainting(0.3)); }

        [TestMethod] public void SmartPainter_PositionPrecision() { RandomDelay(); Assert.IsTrue(robot.MoveTo(999, 999, 999, 0.1)); }
        [TestMethod] public void ABBFlexiblePaintBot_LowEdgeCheck() { RandomDelay(); Assert.IsTrue(robot.MoveTo(0, 0, 0, 0.1)); }
        [TestMethod] public void ABBPaintMaster_ResetClearsState() { RandomDelay(); robot.InjectRandomFailure(); robot.Reset(); Assert.IsFalse(robot.IsInFailsafe); }
        [TestMethod] public void ABBPaintBot_TriggerFailsafeFlow() { RandomDelay(); robot.MoveTo(100, 100, 100, 0.3); robot.InjectRandomFailure(); Assert.IsTrue(robot.IsInFailsafe); }
        [TestMethod] public void ABBPainter3000_StartStopValid() { RandomDelay(); robot.MoveTo(150, 150, 150, 0.2); if (robot.StartPainting(0.2)) Assert.IsTrue(robot.StopPainting()); }

        [TestMethod] public void ABBPainterX_CycleStopWithError() { Delay60Seconds(); robot.MoveTo(250, 150, 50, 0.3); robot.StartPainting(0.3); robot.InjectRandomFailure(); Assert.IsFalse(robot.StopPainting()); }
        [TestMethod] public void PaintProSystem_ResetPosition() { RandomDelay(); robot.MoveTo(300, 200, 100, 0.2); robot.InjectRandomFailure(); robot.Reset(); Assert.AreEqual((0, 0, 0), robot.CurrentPosition); }
        [TestMethod] public void ABBAdvancedPaintBot_FailsafeMovement() { RandomDelay(); robot.InjectRandomFailure(); Assert.IsFalse(robot.MoveTo(400, 300, 200, 0.5)); }
        [TestMethod] public void ABBIRB6000_MovementBoundaries() { RandomDelay(); Assert.IsFalse(robot.MoveTo(2000, 0, 0, 0.2)); }
        [TestMethod] public void PaintBotEdgeTolerance_Check() { Delay30Seconds(); Assert.IsFalse(robot.MoveTo(1000, 1000, 1001, 0.2)); }
        [TestMethod] public void RoboticPaintX_MoveAndSpray() { RandomDelay(); robot.MoveTo(100, 200, 300, 0.2); if (robot.StartPainting(0.2)) Assert.IsTrue(robot.StopPainting()); }
        [TestMethod] public void PaintBotFaultyMove_ShouldFail() { RandomDelay(); Assert.IsFalse(robot.MoveTo(-100, -200, -300, 0.2)); }
        [TestMethod] public void IndustrialPaintBot_ValidStop() { RandomDelay(); robot.MoveTo(150, 250, 350, 0.2); robot.StartPainting(0.2); Assert.IsTrue(robot.StopPainting()); }
        [TestMethod] public void MotionSystemCheck_FailureDuringRun() { Delay120Seconds(); robot.MoveTo(150, 200, 250, 0.2); robot.StartPainting(0.3); robot.InjectRandomFailure(); Assert.IsFalse(robot.StopPainting()); }
        [TestMethod] public void ABBXPaintControl_ResetAndRetry() { RandomDelay(); robot.InjectRandomFailure(); robot.Reset(); Assert.IsTrue(robot.MoveTo(50, 50, 50, 0.1)); }
        [TestMethod] public void CompactPaintBot_QuickTest() { RandomDelay(); Assert.IsTrue(robot.MoveTo(100, 100, 100, 0.1)); }
        [TestMethod] public void SimPaintRobot_ResetSequence() { RandomDelay(); robot.InjectRandomFailure(); robot.Reset(); Assert.IsTrue(robot.MoveTo(200, 200, 200, 0.2)); }
    }

    public class PaintRobot
    {
        public (int X, int Y, int Z) CurrentPosition { get; private set; } = (0, 0, 0);
        public bool IsPainting { get; private set; } = false;
        public bool IsInFailsafe { get; private set; } = false;

        private static Random rnd = new Random();
        private readonly double failureChance;

        public PaintRobot(double failureChance = 0.5)
        {
            this.failureChance = failureChance;
        }

        public bool MoveTo(int x, int y, int z, double? overrideFailureChance = null)
        {
            double failChance = overrideFailureChance ?? failureChance;
            if (IsInFailsafe || x < 0 || x > 1000 || y < 0 || y > 1000 || z < 0 || z > 1000)
            {
                IsInFailsafe = true;
                return false;
            }

            if (rnd.NextDouble() < failChance)
            {
                IsInFailsafe = true;
                return false;
            }

            CurrentPosition = (x, y, z);
            return true;
        }

        public bool StartPainting(double? overrideFailureChance = null)
        {
            double failChance = overrideFailureChance ?? failureChance;
            if (IsInFailsafe || IsPainting)
                return false;

            if (rnd.NextDouble() < failChance)
            {
                IsInFailsafe = true;
                return false;
            }

            IsPainting = true;
            return true;
        }

        public bool StopPainting()
        {
            if (IsInFailsafe || !IsPainting)
                return false;

            IsPainting = false;
            return true;
        }

        public void InjectRandomFailure()
        {
            IsInFailsafe = true;
        }

        public void Reset()
        {
            IsInFailsafe = false;
            IsPainting = false;
            CurrentPosition = (0, 0, 0);
        }
    }
}
